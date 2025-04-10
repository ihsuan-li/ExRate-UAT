using CDFHEXRETE.DTOS.ReqDTOs;
using CDFHEXRETE.DTOS.ResDTOs;
using CDFHEXRETE.Interfaces;
using CDFHEXRETE.Models;
using CDFHEXRETE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CDFHEXRETE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHomeService _homeService;

        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        public IActionResult Index(string type = "opening")
        {
            HomeModel model = new HomeModel() { type = type };
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> SearchDates([FromBody] ReqHomeQuery req)
        {
            var result = await _homeService.GetHomeDatesAsync(req);
            return Json(result);
        }

        [HttpPost]

        public async Task<IActionResult> ExportToExcel([FromBody] ReqHomeDate req)
        {
            var result = await _homeService.GetHomeDateRatesAsync(req);
            var resultBytes = await _homeService.ExportExcelAsync(result.Data);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // 檔名設定由home.js內處理
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";

            return File(resultBytes, contentType, fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}