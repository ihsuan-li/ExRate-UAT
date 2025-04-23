using CDFHEXRATE.Repository.Contexts;
using CDFHEXRATE.Repository.Models;
using CDFHEXRETE.Common;
using CDFHEXRETE.Common.Extensions;
using CDFHEXRETE.Common.Repository;
using CDFHEXRETE.Extensions;
using CDFHEXRETE.Interfaces;
using CDFHEXRETE.Invocables;
using CDFHEXRETE.Middleware;
using CDFHEXRETE.Models;
using CDFHEXRETE.Models.Config;
using CDFHEXRETE.Services;
using Coravel;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings()
    .GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);
//var env = builder.Environment.EnvironmentName; //取環境名稱
var env = builder.Configuration.GetValue<string>("ENV"); //取環境名稱

var aesKey = builder.Configuration.GetValue<string>("AES_KEY") ?? throw new ArgumentNullException("AES_KEY");
var ivKey = builder.Configuration.GetValue<string>("IV_KEY") ?? throw new ArgumentNullException("IV_KEY");

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.Encoder =
             JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
});

builder.Logging.AddNLog("nlog.config");


// Add custom configuration provider.
builder.Configuration.AddEncryptConfProvider($"appsettings.{env}.json", aesKey, ivKey, true);

// Bind configurations.
var databaseConfigDetails = builder.Configuration.GetSection("Database").Get<DatabaseConfigDetail[]>();
builder.Services.Configure<DatabaseConfig>(options => options.Details = databaseConfigDetails!);
SystemShared.Init(); //初始化設定

builder.Services.Configure<ConnectAccountDataConfig>(builder.Configuration.GetSection("ConnectAccountData"));

//新增EF Core資料庫服務
var ConnectionString = SystemShared.BuildConnectionString("DbConnect");
builder.Services.AddDbContext<DBContext>(option => option.UseSqlServer(ConnectionString));
builder.Services.AddTransient<DBContext>();

#region 系統介面服務初始化
builder.Services.AddDiContainer();
#endregion
// 添加 HttpClient 服務
builder.Services.AddHttpClient();

// 註冊 AAMService
builder.Services.AddScoped<IAAMService, AAMService>();

// Add services to the container.
builder.Services.AddTransient<ScheduleRateService>();

// Register the scheduler and invocables.
builder.Services.AddScheduler();
builder.Services.AddTransient<GetRateJob>();

var app = builder.Build();

logger.Info($"Current using settings: appsettings.{env}.json");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use the custom middleware for http logging.
app.UseMiddleware<LoggingMiddleware>();

// Use middleware for global handle.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (System.Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync<GeneralResModel>(new GeneralResModel
        {
            Status = 500,
            Message = ex.Message
        });
    }
});

app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Config scheduling.
SchedulerConfig schedulerConfig = app.Configuration.GetSection("Scheduler").Get<SchedulerConfig>()!;
if (schedulerConfig.Enable)
{
    app.Services.UseScheduler(scheduler =>
    {
        logger.Info($"GetRateJob is scheduled at cron \"{schedulerConfig.Time}\".");
        scheduler
            .Schedule<GetRateJob>()
            .Cron(schedulerConfig.Time)
            .Zoned(TimeZoneInfo.Local);
    });
}


// 測試「取得連線至DB-[TRMDDB]的連線資訊」
app.MapPost("/cdf/schedule/ConnectAccountData", async () =>
{
    // Only allow one request at a time.
    await locker.WaitAsync(TimeSpan.FromSeconds(1));

    // Rerun SP.
    var scheduleRateService = app.Services.GetRequiredService<ScheduleRateService>();
    scheduleRateService.GetConnectAccountData();

    locker.Release();
    return Results.Json(new GeneralResModel
    {
        Status = 200,
        Message = @"成功「取得連線至DB-[FHMDDB]的連線資訊」",
        Success = true
    });
});

// 測試「取得當日所有匯率」
app.MapPost("/cdf/schedule/AllExRateData", async (ConnectAccountData req, string date) =>
{
    // Only allow one request at a time.
    await locker.WaitAsync(TimeSpan.FromSeconds(1));

    // Rerun SP.
    var scheduleRateService = app.Services.GetRequiredService<ScheduleRateService>();
    await scheduleRateService.GetAllExRateData(req, date);

    locker.Release();
    return Results.Json(new GeneralResModel
    {
        Status = 200,
        Message = @"成功「取得當日所有匯率」",
        Success = true
    });
});

// 測試「更新當日所有匯率」
app.MapPost("/cdf/schedule/UpdateAllExRateData", async (List<ExRate> req) =>
{
    // Only allow one request at a time.
    await locker.WaitAsync(TimeSpan.FromSeconds(1));

    // Rerun SP.
    var scheduleRateService = app.Services.GetRequiredService<ScheduleRateService>();
    await scheduleRateService.UpdateAllExRateData(req);

    locker.Release();
    return Results.Json(new GeneralResModel
    {
        Status = 200,
        Message = @"成功「更新當日所有匯率」",
        Success = true
    });
});

// Test schedule
app.MapPost("/cdf/scheduleRate/test", async (HttpContext context) =>
{
    var getRateJob = app.Services.GetRequiredService<GetRateJob>();
    await getRateJob.Invoke();

    return Results.Json(new GeneralResModel
    {
        Status = 200,
        Success = true
    });
});

app.Run();


// Use partial class to define the static field.
public partial class Program
{
    static SemaphoreSlim locker = new SemaphoreSlim(1, 1);
}
