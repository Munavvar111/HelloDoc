toastr.options = {
    positionClass: 'toast-top-right',
    closeButton: true,
    progressBar: true,
    showDuration: 400,
    hideDuration: 1000,
    timeOut: 2000,
    extendedTimeOut: 1000,
    toastClass: 'toast-red' // Add a custom class for red color styling
}
$(document).ready(function () {
    function updateUIWithCounts() {
        $.ajax({
            type: "GET",
            url: "/Admin/GetStatusCounts",
            success: function (data) {
                console.log(data);
                updateUIWithCountsNumber(data);

            },
            error: function (error) {
                console.error('Error:', error);
            }
        });

        function updateUIWithCountsNumber(data) {
            // Update your UI elements using the data received from the server
            $('#statuslink1 .Status-Count').text(data.newCount);
            $('#statuslink2 .Status-Coun').text(data.pendingCount);
            $('#statuslink3 .Status-Coun').text(data.activeCount);
            $('#statuslink4 .Status-Coun').text(data.concludeCount);
            $('#statuslink5 .Status-Coun').text(data.toClosedCount);
            $('#statuslink6 .Status-Count').text(data.unpaidCount);
        }
    }
    console.log("1")

    var storedPartial = localStorage.getItem('currentPartial');
    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));
    var statustext = localStorage.getItem('statustext');

    var currentPartial = storedPartial || "NewTablePartial";
    var currentStatus = storedStatus || [1];
    var currentPage = localStorage.getItem("currentPage");
    var exportdata = false;
    var exportAllData = false;
    $(document).on("click", "#pagination a.page-link", function () {
        console.log("Pagination link clicked!");
        var id = $(this).attr("id");
        currentPage = $("#" + id).data("page");
        localStorage.setItem("currentPage", currentPage);
        console.log("Current Page: " + currentPage);
        filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });

    if (currentPage) {
        currentPage = currentPage
    }
    else {

        currentPage = 1;
    }
    var pageSize = 3;

    var status = localStorage.getItem('statuslink');
    $(".Status-btn").removeClass('activee');
    $(status).addClass("activee");
    if (statustext) {
        $('#statuschange').html(statustext);
    }
    else {
        $('#statuschange').html('(New)');

    }
    filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData);
    updateUIWithCounts();




    $("#statuslink1").click(function (e) {
        exportdata = false;
        exportAllData = false;
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink1").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink1')
        currentPartial = "NewTablePartial"
        currentStatus = [1];
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        $('#statuschange').html('(New)');
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        localStorage.setItem("statustext", '(New)')
        filterTable("NewTablePartial", currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });



    $("#statuslink2").click(function () {
        exportdata = false;
        exportAllData = false;
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink2").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink2')
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        console.log("hii2")

        currentPartial = "PendingTablePartial"
        currentStatus = [2];
        $('#statuschange').html('(Pending)');
        localStorage.setItem("statustext", '(Pending)')

        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });


    $("#statuslink3").click(function () {
        exportdata = false;
        exportAllData = false;
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink3").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink3')
        currentStatus = [4, 5];
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        currentPartial = "ActiveTablePartial";
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));

        $('#statuschange').html('(Active)');
        localStorage.setItem("statustext", '(Active)')

        filterTable("ActiveTablePartial", currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });



    $("#statuslink4").click(function () {
        exportdata = false;
        exportAllData = false;
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink4").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink4')
        currentStatus = [6];
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        currentPartial = "ConcludeTablePartial";
        $('#statuschange').html('(Conclude)');
        localStorage.setItem("statustext", '(Conclude)')

        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        filterTable("ConcludeTablePartial", currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });

    $("#statuslink5").click(function () {
        exportdata = false;
        exportAllData = false;
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink5").addClass("activee");
        currentStatus = [3, 7, 8];
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        currentPartial = "ToCloseTablePartial";
        $('#statuschange').html('(ToClose)');
        localStorage.setItem("statustext", '(ToClose)')

        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('statuslink', '#statuslink5');
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        filterTable("ToCloseTablePartial", currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });

    $("#statuslink6").click(function () {
        exportdata = false;
        exportAllData = false;
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink6").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink6')
        currentStatus = [9];
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        currentPartial = "UnpaidTablePartial";
        $('#statuschange').html('(Unpaid)');
        localStorage.setItem("statustext", '(Unpaid)')

        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        filterTable("UnpaidTablePartial", currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });

    //filter the data with passed currentpartial that will load only that data
    $("#filterSelect").on("input change", function () {
        exportdata = false;
        exportAllData = false;
        console.log("inputchange");

        filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });


    $("#searchInput").on("input", function () {
        exportdata = false;
        exportAllData = false;
        console.log("inputchange")
        currentPage = 1;
        filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData);
    });

    $('.filter-item').click(function () {
        exportdata = false;
        exportAllData = false
        $('.filter-item').removeClass('active')
        $(this).addClass('active')
        currentPage = 1;
        filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData)

    });

    $('#exportdata').click(function () {
        exportdata = true;
        exportAllData = false;
        filterTable(currentPartial, currentStatus, currentPage, pageSize, exportdata, exportAllData)
        currentPage = 1;
    });

    //ajax for render that partialview

    //ajax for filterthe table using search
    function filterTable(partialName, currentStatus, page, pageSize, exportdata, exportAllData) {


        console.log(partialName)
        var searchValue = $("#searchInput").val();
        var selectValue = $("#filterSelect").val();
        if (searchValue != null) {
            searchValue = searchValue.toLowerCase();
        }

        var selectedFilter = $('.filter-item.active').data('value');

        if (selectValue == " " && !selectedFilter && !searchValue) {
            currentPage = localStorage.getItem("currentPage");
            page = currentPage
            console.log(currentPage)
        }
        else {
            currentPage = 1;
        }


        $.ajax({
            type: "GET",
            url: "/Admin/SearchPatient",
            traditional: true,
            data: { searchValue: searchValue, selectValue: selectValue, partialName: partialName, selectedFilter: selectedFilter, currentStatus: currentStatus, page: page, pageSize: pageSize, exportdata: exportdata, exportAllData: exportAllData },
            success: function (data) {
                if (exportdata == true) {
                    var blob = new Blob([data], { type: 'text/csv' });
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'filtered_data.csv';
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                }
                else if (exportAllData == true) {
                    var blob = new Blob([data], { type: 'text/csv' });
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = 'filtered_data.csv';
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                }
                else {

                    if (data != null && data.length > 0 && !exportdata) {
                        $('#partialContainer').html(data);
                    } else {
                        $('#partialContainer').html('<p>No data is Found</p>');

                    }
                }
            },
            error: function (xhr) {
                if (xhr.status === 403) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.redirectToLogin) {
                        window.location.href = '/Login';
                    } else {
                    }
                } else {
                    toastr.error('An error occurred during the AJAX request.');
                }
            }
        });

    }
    

})


        // $.ajax({
        //     type: "POST",
        //     contentType: "application/json; charset=utf-8",
        //     url: SiteURL + "AutoCompleteData.aspx/" + method,
        //     data: "{'SearchText':'" + escape(document.getElementById('txtSearch').value) + "'}",
        //     dataType: "json",
        //     success: function (data) {
        //         response($.map(data.d, function (item) {
        //             return {
        //                 label: item.split('#~#')[0],
        //                 val: item.split('#~#')[1],
        //                 num: item.split('#~#')[2],
        //                 fkModuleType: item.split('#~#')[3]
        //             }
        //         }));
        //     },
        //     error: function (result) {

        //     }
        // });

