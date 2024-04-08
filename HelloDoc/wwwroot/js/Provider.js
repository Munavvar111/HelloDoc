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
    console.log("kjhsdfk")

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
        }
    }
    var storedPartial = localStorage.getItem('currentPartial');
    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));
    var statustext = localStorage.getItem('statustext');

    var currentPartial = storedPartial || "NewProviderTablePartial";
    var currentStatus = storedStatus || [1];
    var currentPage = localStorage.getItem("currentPage") || 1;
    
    $(document).on("click", "#pagination a.page-link", function () {
        console.log("Pagination link clicked!");
        var id = $(this).attr("id");
        currentPage = $("#" + id).data("page");
        localStorage.setItem("currentPage", currentPage);
        console.log("Current Page: " + currentPage);
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
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
    filterTable(currentPartial, currentStatus, currentPage, pageSize);
    updateUIWithCounts();




    $("#statuslink1").click(function (e) {
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink1").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink1')
        currentPartial = "NewProviderTablePartial"
        currentStatus = [1];
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        $('#statuschange').html('(New)');
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        localStorage.setItem("statustext", '(New)')
        filterTable("NewProviderTablePartial", currentStatus, currentPage, pageSize);
    });



    $("#statuslink2").click(function () {
        
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink2").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink2')
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        console.log("hii2")

        currentPartial = "PendingProviderTablePartial"
        currentStatus = [2];
        $('#statuschange').html('(Pending)');
        localStorage.setItem("statustext", '(Pending)')

        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });


    $("#statuslink3").click(function () {
      
        $("#searchInput").val("");
        $("#filterSelect").val(" ");
        $('.filter-item').removeClass('active')
        $(".Status-btn").removeClass('activee');
        $("#statuslink3").addClass("activee");
        localStorage.setItem('statuslink', '#statuslink3')
        currentStatus = [4, 5];
        currentPage = 1;
        localStorage.setItem("currentPage", currentPage);
        currentPartial = "ActiveProviderTablePartial";
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));

        $('#statuschange').html('(Active)');
        localStorage.setItem("statustext", '(Active)')

        filterTable("ActiveProviderTablePartial", currentStatus, currentPage, pageSize,);
    });



    $("#statuslink4").click(function () {
        
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
        filterTable("ConcludeTablePartial", currentStatus, currentPage, pageSize);
    });
    $("#searchInput").on("input", function () {
   
        console.log("inputchange")
        currentPage = 1;
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });
    $('.filter-item').click(function () {
   
        $('.filter-item').removeClass('active')
        $(this).addClass('active')
        currentPage = 1;
        filterTable(currentPartial, currentStatus, currentPage, pageSize)

    });

    function filterTable(partialName, currentStatus, page, pageSize) {


        console.log(partialName)
        var searchValue = $("#searchInput").val();
        if (searchValue != null) {
            searchValue = searchValue.toLowerCase();
        }

        var selectedFilter = $('.filter-item.active').data('value');

        if (searchValue == "" && !selectedFilter) {
            currentPage = localStorage.getItem("currentPage");
            page = currentPage
            console.log(currentPage)
        }
        else {
            currentPage = 1;
        }


        $.ajax({
            type: "GET",
            url: "/Provider/FilterPatient",
            traditional: true,
            data: { searchValue: searchValue, partialName: partialName, selectedFilter: selectedFilter, currentStatus: currentStatus, page: page, pageSize: pageSize},
            success: function (data) {
               
                    if (data != null && data.length > 0 ) {
                        $('#partialContainer').html(data);
                    } else {
                        $('#partialContainer').html('<p>No data is Found</p>');

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