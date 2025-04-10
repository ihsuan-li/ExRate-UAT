
$(function () {
    if (getCookieByName('dateStart') != '' && getCookieByName('dateEnd') != '') {
        document.querySelector("#dateStart").value = getCookieByName('dateStart');
        document.querySelector("#dateEnd").value = getCookieByName('dateEnd');
    }
    else {
        document.querySelector("#dateStart").value = getAddDate(-30);
        document.querySelector("#dateEnd").value = getAddDate(0);
    }
    setDate();
    getSearchForm();
});

function setDate() {
    var start = document.querySelector("#dateStart").value;
    var end = document.querySelector("#dateEnd").value;
    debugger;
    // 設定1H(3600秒)後失效
    document.cookie = 'dateStart=' + start + '; Max-Age=3600';
    document.cookie = 'dateEnd=' + end + '; Max-Age=3600';
}

function parseCookie() {
    var cookieObj = {};
    var cookieAry = document.cookie.split(';');
    var cookie;

    for (var i = 0, l = cookieAry.length; i < l; ++i) {
        cookie = cookieAry[i].trim(); // 使用 trim() 去除空白
        //cookie = jQuery.trim(cookieAry[i]);
        //cookie = cookie.split('=');
        var parts = cookie.split('=');
        var name = parts.shift();
        var value = parts.join('=');
        cookieObj[name] = value;
        //cookieObj[cookie[0]] = cookie[1];
    }

    return cookieObj;
}


function getCookieByName(name) {
    debugger;
    var value = parseCookie()[name];
    if (value) {
        value = decodeURIComponent(value);
    }
    if (typeof (value) == 'undefined')
        value = '';
    return value;
}

function getAddDate(days) {
    var Today = new Date();
    var AddDay = new Date(Today.getTime() + (days * 24 * 3600 * 1000));
    var month = (AddDay.getMonth() + 1);
    return AddDay.getFullYear() + "-" + addzero(month) + "-" + addzero(AddDay.getDate());
}

function addzero(num) {
    return num < 10 ? '0' + num : num;
}

/*
 日期轉字串
 */
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}


/**
 查詢日期區間
 */
async function getSearchForm(event) {
    //載入table data
    function loaddata() {
        debugger;
        var form = document.querySelector('#search-form');
        if (!form) {
            console.error("Form element not found");
            return;
        }

        const formData = new FormData(form);
        const rate_type = document.querySelector('#rate_type');
        if (!rate_type) {
            console.error("Rate type element not found");
            return;
        }

        const ExRateType = rate_type.innerHTML == 'opening' ? '1' : '2';
        formData.append('ExRateType', ExRateType);
        const value = Object.fromEntries(formData.entries());
        const startTime = formData.get('dateStart');
        const endTime = formData.get('dateEnd');

        if (!startTime || !endTime) {
            console.error("Start time or end time not found");
            return;
        }

        // 日期須將/轉為-，不然系統會誤判成字串
        const start = startTime == "" ? null : startTime.replace(/\//g, '-');
        const end = endTime == "" ? null : endTime.replace(/\//g, '-');

        value['dateStart'] = start;
        value['dateEnd'] = end;
        console.log('dateStart: ' + start + '，dateEnd: ' + end + '，ExRateType: ' + ExRateType);
        $.ajax({
            url: "/Home/SearchDates",
            type: "post",
            data: JSON.stringify(value),
            contentType: "application/json; charset=utf-8",
            async: true,
            success: function (data) {
                var htmlobj = "";
                $("#row-data").empty();
                if (data.TotalCount == 0) {
                    $("#row-data").append("<tr><td colspan='12' align='center'>查無資料</td></tr>");
                }
                $.each(data.Data, function (index, data) {
                    htmlobj = createTrElement(data, index);
                    $("#row-data").append(htmlobj);
                    htmlobj = "";
                });
            },
            error: function (e) {
                console.error("查詢日期區間失敗");
            }
        });
    }
    loaddata();
}

//async function getSearchForm(event) {
//    //載入table data
//    function loaddata() {
//        debugger;
//        var form = document.querySelector('#search-form');
//        const formData = new FormData(form);
//        const rate_type = document.querySelector('#rate_type');
//        const ExRateType = rate_type.innerHTML == 'opening' ? '1' : '2';
//        formData.append('ExRateType', ExRateType);
//        const value = Object.fromEntries(formData.entries());
//        const startTime = formData.get('dateStart');
//        const endTime = formData.get('dateEnd');

//        // 日期須將/轉為-，不然系統會誤判成字串
//        const start = startTime == "" ? null : startTime?.replace('/', '-').replace('/', '-');
//        const end = endTime == "" ? null : endTime?.replace('/', '-').replace('/', '-');

//        value['dateStart'] = start;
//        value['dateEnd'] = end;
//        console.log('dateStart: ' + start + '，dateEnd: ' + end + '，ExRateType: ' + ExRateType);
//        $.ajax({
//            url: "/Home/SearchDates",
//            type: "post",
//            data: JSON.stringify(value),
//            contentType: "application/json; charset=utf-8",
//            async: true,
//            success: function (data) {
//                var htmlobj = "";
//                $("#row-data").empty();
//                if (data.TotalCount == 0) {
//                    $("#row-data").append("<tr><td colspan='12' align='center'>查無資料</td></tr>");
//                }
//                $.each(data.Data, function (index, data) {
//                    htmlobj = createTrElement(data, index);
//                    $("#row-data").append(htmlobj);
//                    htmlobj = "";

//                });

//            },
//            error: function (e) {
//                console.error("查詢日期區間失敗")
//            }

//        });
//    }
//    loaddata();
//};


/**
建立表格tr內容
 */
function createTrElement(data, trIndex) {
    // 建立元素
    const $tr = document.createElement("tr");

    // 帶入 data
    $tr.dataset.dataDate = data.DataDate;
    const dataDate = formatDate(data.DataDate)
    const day_list = ['日', '一', '二', '三', '四', '五', '六'];
    const day = new Date(data.DataDate).getDay();

    $tr.innerHTML = `
                <td>
                    <div class="col-12 center">
                         <button
                            type="button"
                            name="table-btn--excel"
                            class="btn-outline-primary btnDate">${dataDate}(${day_list[day]})</button>
                    </div>
                </td>`;


    return $tr;
}


/*
 Table 預覽,更新或刪除事件
 */
async function tableBtnDelegation(event) {
    debugger;
    const { name } = event.target;

    // 如果沒有點到按鈕，就不做任何事情
    if (!name) return;

    const $closestTr = event.target.closest("tr");

    const dataDate = $closestTr.dataset["dataDate"];

    switch (name) {
        case "table-btn--excel":  // 下載excel
            exportExcel(dataDate);
            break;
        default:
            break;
    }
}


function exportExcel(dataDate) {
    debugger;
    const rate_type = document.querySelector('#rate_type');
    const ExRateType = rate_type.innerHTML == 'opening' ? '1' : '2'
    const ExRateTypeName = rate_type.innerHTML == 'opening' ? '早盤' : '收盤'
    const value = { dataDate: dataDate, ExRateType: ExRateType };

    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Home/ExportToExcel', true);
    xhr.responseType = 'blob';
    xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
    debugger;
    xhr.onload = function (e) {
        if (this.status == 200) {
            var blob = new Blob([this.response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var downloadUrl = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = downloadUrl;
            a.download = formatDate(dataDate).replaceAll('-', '') + "_" + ExRateTypeName + "匯率.xlsx";
            document.body.appendChild(a);
            a.click();
        } else {
            console.error('失敗:下載Excel錯誤');
        }
    };
    xhr.send(JSON.stringify(value));
    return false;
}
