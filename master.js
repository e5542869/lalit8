
$.getJSON('/Base/GetUserData', function (data) {
    var financialYearId = 0, finStartDate, finEndDate, finTitle;
    var userData = JSON.parse(data);
    $(document).ready('one', function () {
        financialYearChange(financialYearId, finStartDate, finEndDate, finTitle);
    });
    $(document).ready(function () {
        $('.dt-button').addClass('btn btn-primary');
        $(".navbar-toggle").click(function () {
            $('body ').toggleClass("offcanvas_open offcanvas_left");
        });
        var $body = $(document.body);
        var navHeight = $('.navbar').outerHeight(true) + 10;
        $('.sidebar').affix({
            offset: {
                top: 0,
                bottom: navHeight
            }
        });
        $body.scrollspy({
            target: '.navbar-default.sidebar',
            offset: navHeight
        });
        $.getJSON("/Admin/GetFinancialyear", function (data) {
            var items = [];
            var yearId = 0;
            $.each(data, function (i, item) {
                if (i == 0) {
                    financialYearId = item.Fin_ID;
                    finStartDate = item.Fin_StartDate;
                    finEndDate = item.Fin_EndDate;
                    finTitle = item.Fin_Title
                }
                items.push('<li><a href="#" onclick="financialYearChange(\'' + item.Fin_ID + '\'', ',\'' + item.Fin_StartDate + '\'', ',\'' + item.Fin_EndDate + '\'', ',\'' + item.Fin_Title + '\');return false">' + item.Fin_Title + '</a></li>');
            });  // close each()

            $("#ulyears").append(items.join(''));
        });
        $('#tblfinancialYears').DataTable({
            bProcessing: true,
            ajax: {
                url: "/Admin/GetFinancialyear",
                dataSrc: ''
            },
            order: [[0, "desc"]],
            columns: [
                { data: "Fin_ID", name: "Fin_ID", visible: false },
                { data: "Fin_Title", name: "Financial Year" },
                { data: "Fin_StartDate", name: "Financial StartDate" },
                { data: "Fin_EndDate", name: "Financial EndDate" },
                { data: "Inst_Title", name: "Org Name" }
            ]
        });
        var selectedYear = $('#spnSelectedYear').text();
        if (selectedYear.trim().length == 9)
            removeLoaderClass();
        else
            addLoaderClass();
        if (userData != undefined) {
            if (userData.IsTransactionAddAllow)
                $('#menuAddTransaction').addClass('visibility');
            else
                $('#menuAddTransaction').addClass('novisibility');
            if (userData.IsTransactionEditAllow)
                $('#menuUpdateTransaction').addClass('visibility');
            else
                $('#menuUpdateTransaction').addClass('novisibility');
        }

    });
});
function financialYearChange(financialId, finStartDate, finEndDate, finTitle) {
    if (financialId == undefined && financialId == '')
        financialId = financialYearId;
    $.ajax({
        type: "GET",
        url: '/Base/ChangeFinancialyear',
        contentType: "application/json; charset=utf-8",
        data: { yearId: financialId, startDate: finStartDate, endDate: finEndDate, finTitle: finTitle },
        dataType: "json",
        success: function () { window.location.href = "/Home/Index"; },
        error: function (error) { console.log(error); }
    });
}
function HideLoading() {
    $("#divLoading").removeClass('visibility');
    $("#divLoading").addClass('novisibility');
}
function ShowLoading() {
    if ($("#divLoading").hasClass('novisibility'))
        $("#divLoading").removeClass('novisibility');
    $("#divLoading").addClass('visibility');
}
function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}
function validateFloatKeyPress(el, evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    var number = el.value.split('.');
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    //just one dot
    if (number.length > 1 && charCode == 46) {
        return false;
    }
    //get the carat position
    var caratPos = getSelectionStart(el);
    var dotPos = el.value.indexOf(".");
    if (caratPos > dotPos && dotPos > -1 && (number[1].length > 1)) {
        return false;
    }
    return true;
}
function getSelectionStart(o) {
    if (o.createTextRange) {
        var r = document.selection.createRange().duplicate()
        r.moveEnd('character', o.value.length)
        if (r.text == '') return o.value.length
        return o.value.lastIndexOf(r.text)
    } else return o.selectionStart
}
function addLoaderClass() {
    var container = $('#page-wrapper');
    container.addClass('loader');
    var navMainMenu = $('#navMainMenu');
    navMainMenu.addClass('leftloader');
}
function removeLoaderClass() {
    const container = $('#page-wrapper');
    if (container.hasClass('loader')) {
        container.removeClass('loader');
    }
    var navMainMenu = $('#navMainMenu');
    if (navMainMenu.hasClass('leftloader'))
        navMainMenu.addClass('leftloader');
}
function setPermissions(instituteId) {
    if (instituteId == 300010) {
        $('#menuChangePassword').addClass('visibility');
        $('#menuGeneralRevenueReport').addClass('visibility');
        $('#menuScheduleWiseReprt').addClass('visibility');
        $('#menuPermission').addClass('visibility');
        $('#menuViewPassword').addClass('visibility');
        $('#commonIncomeExpenditureReprt').css("display", "block");
    }
    else {
        $('#menuChangePassword').addClass('novisibility');
        $('#menuGeneralRevenueReport').addClass('novisibility');
        $('#menuScheduleWiseReprt').addClass('novisibility');
        $('#menuPermission').addClass('novisibility');
        $('#menuViewPassword').addClass('novisibility');
        $('#commonIncomeExpenditureReprt').css("display", "none");
    }

}

function ConverttoDate(dateString) {
    var dateParts = dateString.split("/");
    return new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
}

