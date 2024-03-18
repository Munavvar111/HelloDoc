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

   
    $(document).on("click", "#pagination a.page-link", function () {
        console.log("Pagination link clicked!");
        currentPage = $(this).text().trim();
        localStorage.setItem("currentPage", currentPage);
        console.log("Current Page: " + currentPage);
        filterTable(currentPartial, currentStatus, currentPage, pageSize);
    });


    $("#statuslink1").click(function (e) {
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
        localStorage.setItem("statustext",'(New)')
        filterTable("NewTablePartial", currentStatus, currentPage, pageSize);
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
        
        currentPartial = "PendingTablePartial"
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
        currentPartial = "ActiveTablePartial";
        localStorage.setItem('currentPartial', currentPartial);
        localStorage.setItem('currentStatus', JSON.stringify(currentStatus));

        $('#statuschange').html('(Active)');
        localStorage.setItem("statustext", '(Active)')

        filterTable("ActiveTablePartial", currentStatus, currentPage, pageSize);
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

    $("#statuslink5").click(function () {
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
        filterTable("ToCloseTablePartial", currentStatus, currentPage, pageSize);
    });

    $("#statuslink6").click(function () {
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
    function filterTable(partialName, currentStatus, page, pageSize)
    {
        

                console.log(partialName)
                var searchValue = $("#searchInput").val();
        var selectValue = $("#filterSelect").val();
                if (searchValue != null) {
                    searchValue = searchValue.toLowerCase();
        }
       
                var selectedFilter = $('.filter-item.active').data('value');

        if (selectValue == " " && !selectedFilter && !searchValue) {
            currentPage = localStorage.getItem("currentPage");
            console.log(currentPage)
        }
        else {
            page = 1;
        }


        $('#loader').show();
                $.ajax({
                    type: "GET",
                    url: "/Admin/SearchPatient",
                    traditional: true,

                    beforSend: function () {
                        $('#loader').show();
                    },
                    data: { searchValue: searchValue, selectValue: selectValue, partialName: partialName, selectedFilter: selectedFilter, currentStatus: currentStatus, page: page, pageSize: pageSize },
                    success: function (data) {

                        if (data != null && data.length > 0) {
                        $('#loader').hide();
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
                        }                    }
                });
            
    }
    $("#blockCaseForm").validate({
        rules: {
            blockreason: {
                required: true
            }
        },
        messages: {
            blockreason: "Please enter your blockreason."
        },
        errorPlacement: function (error, element) {
            var errorSpan = $("span.error-" + element.attr("name"));

            if (errorSpan.length > 0) {
                // If the specific span is found, append the error message to it
                error.appendTo(errorSpan);
            } else {
                // If not found, use the default placement (after the input field)
                error.appendTo(errorSpan);
            }
        },
        submitHandler: function (form) {
            // If the form is valid, submit it
            var blockreason = $('#blockreason').val();
            var requestid = $('#requestIdInputBlock').val();
            $('#BlockModal').modal('hide');
            $.ajax({
                method: 'POST',
                url: '/Admin/BlockRequest',
                data: { blockreason: blockreason, requestid: requestid },
                success: function (data) {
                    if (data) {
                        var storedPartial = localStorage.getItem('currentPartial');
                        var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));

                        filterTable(storedPartial, storedStatus, 1, 5);
                        updateUIWithCounts();
                        toastr.success('Block successful!');

                    }
                }
            })
        }
    });
    
    $("#cancelCaseForm").validate({
        rules: {
            CancelReason: {
                required: true
            },
            notescancel: {
                required: true
            }
        },
        messages: {
            notescancel: "Please enter your Notes.",
            CancelReason: "Please enter your CancelReason.",
           
        },
        errorPlacement: function (error, element) {
            var errorSpan = $("span.error-" + element.attr("name"));

            if (errorSpan.length > 0) {
                // If the specific span is found, append the error message to it
                error.appendTo(errorSpan);
            } else {
                // If not found, use the default placement (after the input field)
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            // If the form is valid, submit it
            var requestid = $('#requestIdInputCancel').val();
            var cancelReason = $('#cancelReason').val();
            var additionalnote = $('.additionalnote').val();

            $('#exampleModal').modal('hide');
            $('#cancelcaseview').modal('hide');

            $.ajax({
                method: 'POST',
                url: '/Admin/CancelCase',
                data: { requestid: requestid, notes: additionalnote, CancelReason: cancelReason },
                success: function (data) {
                    if (data) {
                        var storedPartial = localStorage.getItem('currentPartial');
                        var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));

                        filterTable(storedPartial, storedStatus, 1, 5);
                        updateUIWithCounts();
                        toastr.success('CancelPatient successful!');

                    }
                }
            })
        }
    });
  $("#assigncaseform").validate({
        rules: {
            notes: {
                required: true
            },
            region: {
                required: true
            },
            physician: {
                required:true
            }
        },
        messages: {
            notes: "Please enter your Notes.",
            region: "Please enter your region.",
            physician: "Please enter your physician.",
          
        },
        errorPlacement: function (error, element) {
            var errorSpan = $("span.error-" + element.attr("name"));

            if (errorSpan.length > 0) {
                // If the specific span is found, append the error message to it
                error.appendTo(errorSpan);
            } else {
                // If not found, use the default placement (after the input field)
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            // If the form is valid, submit it
            var requestid = $('#requestIdInputCancel1').val();
            var regionid = $('#regionid').val();
            var physician = $('#physicianDropdown').val();
            var description = $('#description').val();
            $('#assigncase').modal('hide');
            $.ajax({
                method: "POST",
                url: "/Admin/AssignRequest",
                data: { requestid: requestid, regionid: regionid, physician: physician, description: description },
                success: function (data) {
                    console.log(data)
                    if (data) {
                        var storedPartial = localStorage.getItem('currentPartial');
                        var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));
                        filterTable(storedPartial, storedStatus, 1, 5);
                        updateUIWithCounts();
                        console.log("toaster", toastr.success)
                        toastr.success('Assign successful!');
                    }
                }
            })
        }
    });
    $('#transfercasebutton').click(function (e) {
        e.preventDefault();
        var requestid = $('#requestIdInputTransfer').val();
        var regionid = $('#regionidtransfer').val();
        var physician = $('#physicianDropdownTransfer').val();
        var description = $('#descriptiontransfer').val();
        if (!regionid) {
            toastr.error('Please Enter The Region');

            return;
        }
        if (!physician) {
            toastr.error('Please Enter The Physician');
            return;
        }

        if (!description) {
            toastr.error('Please Enter The Notes');
            return;
        }
        $('#assigncase').modal('hide');
        $('#transfercase').modal('hide');
       

        $.ajax({
            method: "POST",
            url: "Admin/TransferRequest",
            data: { requestid: requestid, regionid: regionid, physician: physician, description: description },
            success: function (data) {
                console.log(data)
                if (data) {
                    var storedPartial = localStorage.getItem('currentPartial');
                    var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));
                    filterTable(storedPartial, storedStatus, 1, 5);
                    updateUIWithCounts();
                    console.log("toaster", toastr.success)
                    toastr.success('Transfer successful!');

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
                $('.physicianDropdown').empty();
                $('.physicianDropdown').append($('<option>', {
                    value: 'selected disabled',
                    text: "please selected the value",
                    disabled: true
                }))
                $.each(physicians, function (index, physician) {
                    console.log(physician)
                   
                    $('.physicianDropdown').append($('<option>', {
                        value: physician.physicianid,
                        text: physician.firstname + ' ' + physician.lastname
                    }));

                });
            },
            error: function (xhr) {
                if (xhr.status === 403) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.redirectToLogin) {
                        window.location.href = '/Login';
                    } else {
                    }
                } else {
                    // Display toastr notification for other errors
                    toastr.error('An error occurred during the AJAX request.');
                }
            }
        });
        })
    
    document.getElementById("fileInput").onchange = function () {
        let filePath = this.value;
        let allowedExtensions = [".jpg", ".pdf"];

        let extension = filePath.substring(filePath.lastIndexOf('.'));
        extension = extension.toLowerCase();

        if (!allowedExtensions.includes(extension)) {
            toastr.error("Please Select The Pdf Or Jpg")
            this.value = ""; // Clear file selection if invalid
            return false; // Prevent further processing
        }

        let path = filePath.substr(filePath.lastIndexOf('\\') + 1);
        document.getElementById("fileName").innerText = path;
    };

    $('.deletbtn').click(function () {
        var fileUrl = $(this).data("filename");
        var requestid = $(this).data("requestid");
        console.log(requestid);

        $.ajax({
            method: 'POST',
            url: '/Admin/DeleteFile',
            data: { filename: fileUrl },
            success: function (result) {
                window.location.href = "/Admin/ViewUploads/" + result.id;
                

            },
            error: function (error) {
                console.log(error)
            }
        });

        return true;
    })

    $('#healthprofessionaltype').on('change', function () {
        console.log("hii");
        var helthprofessionaltype = $(this).val();
        console.log(helthprofessionaltype);

        $.ajax({
            method: "POST",
            url: "/Admin/VendorNameByHelthProfession",
            data: { helthprofessionaltype: helthprofessionaltype },
            success: function (vendorname) {
                $('#business').empty();
                $('#business').append($('<option>', {
                    value:'selected hidden disable',
                    text: "select Business"
                }));
                $.each(vendorname, function (index, vendor) {
                    $('#business').append($('<option>', {
                        value: vendor.vendorid,
                        text: vendor.vendorname
                    }));

                });
            }
        })
    })

    
        $("#ViewcaseReturnpage").validate({
        rules: {
                
                Email: {
                    required: true,
                    email: true
                },
                Contact: {
                    required:true
                },
                FaxNumber: {
                    required: true
                },
                Prescription: {
                    required: true,
                }
                
        },
            messages: {
                Email: {
                    required: "Please enter your email",
                    email: "Please enter a valid email address"
                },
                Contact: {
                    required: "Please Enter A Contact"
                },
                FaxNumber: {
                    required: "Please Enter A FaxNumber"
                },
                Prescription: {
                    required: "Please Enter A Prescription"
                }     
            },
            errorPlacement: function (error, element) {
                var errorSpan = $("span.error-" + element.attr("name"));

                if (errorSpan.length > 0) {
                    // If the specific span is found, append the error message to it
                    error.appendTo(errorSpan);
                } else {
                    // If not found, use the default placement (after the input field)
                    error.insertAfter(element);
                }
            },

       
        submitHandler: function (form) {
            // If the form is valid, submit it
            form.submit();
        }
    });
    $("#accountinginfo").validate({
        rules: {
            Address1: {
                required: true,
            },
            Address2: {
                required: true,
            },
            City: {
                required: true,
            },
            State: {
                required: true,
            },
            Zipcode: {
                required: true,
            },
            MobileNo: {
                required: true,
            }
        },
        messages: {
            Address1: {
                required: "Please enter a Address1",
            },
            Address2: {
                required: "Please enter a Address2",
            },
            City: {
                required: "Please enter a City",
            },
            State: {
                required: "Please enter a State",
            },
            Zipcode: {
                required: "Please enter a Zipcode",
            },
            MobileNo: {
                required:'Please Enter A PhoneNo'
            }
        },
        errorPlacement: function (error, element) {
            var errorSpan = $("span.error-" + element.attr("name"));

            if (errorSpan.length > 0) {
                // If the specific span is found, append the error message to it
                error.appendTo(errorSpan);
            } else {
                // If not found, use the default placement (after the input field)
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            // If the form is valid, submit it
            form.submit();
        }
    });
    $("#resetpassword").validate({
        rules: {
            Password: {
                required: true,
                minlength: 8, // Minimum password eelength (adjust as needed)
                complexity: true // Custom rule for password complexity (explained below)
            }
        },
        messages: {
            Password: {
                required: "Please enter a password",
                minlength: "Password must be at least {0} characters long",
                complexity: "Password must contain a mix of uppercase, lowercase letters, numbers, and special characters"
            }
        },
        errorPlacement: function (error, element) {
            var errorSpan = $("span.error-" + element.attr("name"));

            if (errorSpan.length > 0) {
                // If the specific span is found, append the error message to it
                error.appendTo(errorSpan);
            } else {
                // If not found, use the default placement (after the input field)
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            // If the form is valid, submit it
            form.submit();
        }
    });
    $("#administrationinfo").validate({
        rules: {
            firstname: {
                required: true
            },
            lastname: {
                required: true
            },
            email: {
                required: true,
                email: true
            },
            confirmmail: {
                required: true,
                equalTo: "#email"
            },
            PhoneNumber: {
                required: true,
                pattern: /^\(?([0-9]{3})\)?[-. ]([0-9]{3})[-. ]([0-9]{4})$/
            },
            adminRegion: {
                required: true,
                minlength: 1
            }
        },
        messages: {
            firstname: "Please enter your First Name.",
            lastname: "Please enter your Last Name.",
            email: {
                required: "Please enter an email address.",
                email: "Please enter a valid email address."
            },
            confirmmail: {
                required: "Please confirm your email address.",
                equalTo: "Please enter the same email as above."
            },
            PhoneNumber: {
                required: "Please enter your mobile phone number.",
                pattern: "Please enter a valid phone number (e.g., 123-456-7890)."
            },
            adminRegion: "Please select at least one region."

        },
        errorPlacement: function (error, element) {
            var errorSpan = $("span.error-" + element.attr("name"));

            if (errorSpan.length > 0) {
                // If the specific span is found, append the error message to it
                error.appendTo(errorSpan);
            } else {
                // If not found, use the default placement (after the input field)
                error.insertAfter(element);
            }
        },
        submitHandler: function (form) {
            // If the form is valid, submit it
            form.submit();
        }
    });

    $.validator.addMethod("complexity", function (value, element) {
        var complexityRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/; // Regular expression for password complexity
        return this.optional(element) || complexityRegex.test(value);
    }, "Password must contain a mix of uppercase, lowercase letters, numbers, and special characters");


    $('#business').on('change', function () {
        var vendorname = $(this).val();
        console.log(vendorname)
        $.ajax({
            method: "POST",
            url: "/Admin/BusinessDetails",
            data: { vendorname: vendorname },
            success: function (response) {
                console.log(response)
                console.log($('#contact'))

                $('#contact').val(response.businesscontact)
                $('#Email').val(response.email)
                $('#FaxNumber').val(response.faxnumber)
            }
        })
    })


    

    $('#SendAgreementBtn').click(function () {
        var requestid = $('#requestIdInputAgreement').val();
        var agreementemail = $('#agreementemail').val();
        var agreementphoneno = $('#agreementphoneno').val();

        $.ajax({
            method: "POST",
            url: '/Admin/SendAgreement',
            data: { requestid: requestid, agreementemail: agreementemail, agreementphoneno: agreementphoneno },
            success: function (response) {
                console.log(response)
                if (response) {
                    window.location.href = '/Admin';
                    
                }
                else {
                    toastr.error("Agreement Unsuccessful!")
                }
            }
        })
    })

    
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

