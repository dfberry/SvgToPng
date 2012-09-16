$(document).ready(function () {

    getPieChartData();

    // click for Next button - only shows after everything else is picked
    $("#convert").click(function () {
        getPieChartPngImage();
    });
});

function getPieChartPngImage() {

    "use strict";

    var piechart = $("#piechart").data("kendoChart");
    var svgPieString = escape(piechart.svg());

    $.ajax({
        url: '/Home/SvgToPng/',
        type: 'POST',
        data: {
            svgpie: svgPieString
        },
        success: function (webImageFileLocation) {

            $('#image').attr('src', webImageFileLocation);

        },
        error: function () {

            alert('error in call to /Home/SvgToPng/');

        }
    });
}

function getPieChartData() {

    "use strict";

    $.ajax({
        url: '/Home/PieChart/',
        type: 'GET',
        success: function (pieChartData) {

            LoadKendoPieChart(pieChartData);

        },
        error: function () {

            alert('error in call to /Home/PieChart/');

        }
    });
}

function LoadKendoPieChart(data) {

    var datetime = new Date().toGMTString();

    $("#piechart").kendoChart({
        title: {
            text: datetime
        },
        dataSource: {
            data: data
        },
        series: [{
            type: "pie",
            field: "Value",
            categoryField: "Name"
        }],
        legend: {
            position: "bottom",
            offsetY: 0,
            labels: {
                template: "#= text # "
            }
        },
        seriesDefaults: {
            labels: {
                visible: true,
                format: "{0}"
            }
        },

        tooltip: {
            visible: true,
            template: "${category} ${value} units"
        },
        chartArea: {
            height: 300,
            width: 300
        },
        plotArea: {
            margin: { 
                top: 20, 
                left: 0, 
                right: 0, 
                bottom: 0 
            }
        }
    });
}
