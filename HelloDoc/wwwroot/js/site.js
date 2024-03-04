
toastr.options = {
    positionClass: 'toast-top-right',
    closeButton: true,
    progressBar: true,
    showDuration: 400,
    hideDuration: 1000,
    timeOut: 2000,
    extendedTimeOut: 1000,
    toastClass: 'toast-red', // Add a custom class for red color styling
};
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
            // Add logic to handle triangle display or any other UI updates
        }
    }
    console.log("1")

    var storedPartial = localStorage.getItem('currentPartial');
    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));
    var storedTringle = localStorage.getItem("currentTringle")

    var currentPartial = storedPartial || "NewTablePartial";
    var currentStatus = storedStatus || [1];
    var currentPage = 1;
    var pageSize = 3;

    if (storedTringle) {
        $(".triangle").css('display', 'none');
        $("#triangle" + storedTringle).css('display', 'block')

    }

    //intial Only NewTablePartial Will load 
    filterTable(currentPartial, currentStatus, currentPage, pageSize);
    updateUIWithCounts();

    $('PendingTablePartial').hide();
    $('ActiveTablePartial').hide();
    $('ConcludeTablePartial').hide();
    $('ToCloseTablePartial').hide();
    $('UnpaidTablePartial').hide();


    $(document).on("click", "#pagination a.page-link", function () {
        console.log("Pagination link clicked!");
        currentPage = $(this).text().trim();
        console.log("Current Page: " + currentPage);
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });

    //onclick each partial will load

    $("#statuslink1").click(function (e) {
        $(".triangle").css('display', 'none');
        $("#triangle1").css('display', 'block').css('border-top-color', '#203f9a');

        currentPartial = "NewTablePartial"
        currentStatus = [1];
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        localStorage.setItem("currentTringle", 1);


        $('#statuschange').html('(new)');
        filterTable("NewTablePartial", currentStatus, currentPage, pageSize);
    });



    $("#statuslink2").click(function () {
        console.log("hii2")
        $(".triangle").css('display', 'none');
        $("#triangle2").css('display', 'block').css('border-top-color', '#00adef');

        currentPartial = "PendingTablePartial"
        currentStatus = [2];
        $('#statuschange').html('(Pending)');
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        localStorage.setItem("currentTringle", 2);
        console.log("jii", currentPage)
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });


    $("#statuslink3").click(function () {
        $(".triangle").css('display', 'none');
        $("#triangle3").css('display', 'block').css('border-top-color', '#228c20');

        currentStatus = [4, 5]
        currentPartial = "ActiveTablePartial";
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        localStorage.setItem("currentTringle", 3);

        $('#statuschange').html('(Active)');

        filterTable("ActiveTablePartial", currentStatus, currentPage, pageSize);
    });



    $("#statuslink4").click(function () {
        $(".triangle").css('display', 'none');
        $("#triangle4").css('display', 'block').css('border-top-color', '#da0f82');

        currentStatus = [6];
        currentPartial = "ConcludeTablePartial";
        $('#statuschange').html('(Conclude)');
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        localStorage.setItem("currentTringle", 4);

        filterTable("ConcludeTablePartial", currentStatus, currentPage, pageSize);
    });

    $("#statuslink5").click(function () {
        $(".triangle").css('display', 'none');
        $("#triangle5").css('display', 'block').css('border-top-color', '#0370d7');

        currentStatus = [3, 7, 8];
        currentPartial = "ToCloseTablePartial";
        $('#statuschange').html('(ToClose)');
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        localStorage.setItem("currentTringle", 5);

        filterTable("ToCloseTablePartial", currentStatus, currentPage, pageSize);
    });

    $("#statuslink6").click(function () {
        $(".triangle").css('display', 'none');
        $("#triangle6").css('display', 'block').css('border-top-color', '#9966cd');

        currentStatus = [9];
        currentPartial = "UnpaidTablePartial";
        $('#statuschange').html('(Unpaid)');
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));
        localStorage.setItem("currentTringle", 6);

        filterTable("UnpaidTablePartial", currentStatus, currentPage, pageSize);
    });

    //filter the data with passed currentpartial that will load only that data
    $("#filterSelect").on("input change", function () {
        console.log("inputchange")
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });


    $("#searchInput").on("input", function () {
        console.log("inputchange")

        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });

    $('.filter-item').click(function () {
        $('.filter-item').removeClass('active')
        $(this).addClass('active')
        filterTable(currentPartial, currentStatus, currentPage, pageSize)

    });


    //ajax for render that partialview

    //ajax for filterthe table using search
    function filterTable(partialName, currentStatus, page, pageSize) {

        console.log(partialName)
        var searchValue = $("#searchInput").val();
        var selectValue = $("#filterSelect").val();
        currentPage = 1;
        var selectedFilter = $('.filter-item.active').data('value');

        console.log(selectValue)
        console.log(searchValue)
        console.log(selectedFilter)



        $.ajax({
            type: "GET",
            url: "/Admin/SearchPatient",
            traditional: true,

            data: { searchValue: searchValue, selectValue: selectValue, partialName: partialName, selectedFilter: selectedFilter, currentStatus: currentStatus, page: page, pageSize: pageSize },
            success: function (data) {
                if (data != null && data.length > 0) {

                    $('#partialContainer').html(data);
                } else {
                    $('#partialContainer').html('<p>No data is Found</p>');

                }

            },
            error: function (error) {
                console.error('Error:', error);
            }
        });

    }

    $('#BlockCase').click(function (e) {
        e.preventDefault();
        var blockreason = $('#blockreason').val();
        var requestid = $('#requestIdInputBlock').val();
        $('#BlockModal').hide();
        $('body').css('overflow', '');

        $('.modal-backdrop').hide();
        $.ajax({
            method: 'POST',
            url: '/Admin/BlockRequest',
            data: { blockreason: blockreason, requestid: requestid },
            success: function (data) {
                if (data) {
                    var storedPartial = localStorage.getItem('currentPartial');
                    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));

                    filterTable(storedPartial, storedStatus, 1, 3);
                    updateUIWithCounts();
                    toastr.success('Block successful!');

                }
            }
        })
    })


    $("#CancelConfirm").click(function (e) {
        e.preventDefault();

        var requestid = $('#requestIdInputCancel').val();
        var cancelReason = $('#cancelReason').val();
        var additionalnote = $('.additionalnote').val();

        $('#exampleModal').hide();
        $('.modal-backdrop').hide();
        $('body').css('overflow', '');

        $.ajax({
            method: 'POST',
            url: '/Admin/CancelCase',
            data: { requestid: requestid, notes: additionalnote, CancelReason: cancelReason },
            success: function (data) {
                if (data) {
                    var storedPartial = localStorage.getItem('currentPartial');
                    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));

                    filterTable(storedPartial, storedStatus, 1, 3);
                    updateUIWithCounts();
                    toastr.success('CancelPatient successful!');

                }
            }
        })
    })



    $('#asigncasebutton').click(function (e) {
        e.preventDefault();
        var requestid = $('#requestIdInputCancel1').val();
        var regionid = $('#regionid').val();
        var physician = $('#physicianDropdown').val();
        var description = $('#description').val();
        $('#assigncase').hide();
        $('.modal-backdrop').hide();
        $('body').css('overflow', '');

        $.ajax({
            method: "POST",
            url: "Admin/AssignRequest",
            data: { requestid: requestid, regionid: regionid, physician: physician, description: description },
            success: function (data) {
                if (data) {
                    var storedPartial = localStorage.getItem('currentPartial');
                    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));
                    filterTable(storedPartial, storedStatus, 1, 3);
                    updateUIWithCounts();
                    console.log("toaster", toastr.success)
                    toastr.success('Assign successful!');

                }
            }
        })

    })

    $('.regionDropdown').on('change', function () {
        console.log("hii")
        var selectregion = $(this).val();
        $.ajax({
            method: 'GET',
            url: '/Admin/GetPhysician',
            data: { region: selectregion },
            success: function (physicians) {
                $('#physicianDropdown').empty();
                $.each(physicians, function (index, physician) {
                    console.log(physician)
                    $('#physicianDropdown').append($('<option>', {
                        value: physician.physicianid,
                        text: physician.firstname + ' ' + physician.lastname
                    }));

                });
            }
        })
    })


    $('.deletbtn').click(function () {
        var fileUrl = $(this).data("filename");
        var requestid = $(this).data("requestid");
        console.log(requestid);

        $.ajax({
            method: 'POST',
            url: '/Admin/DeleteFile',
            data: { filename: fileUrl },
            success: function (result) {
                $.get('/Admin/ViewUploads' + result.requestid, function (data) {
                    $('#view').html(data);
                });
            },
            error: function (error) {
                console.log(error)
            }
        });

        return true;
    })


   
});



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

