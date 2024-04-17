$(document).ready(function () {

    $('#SendLinkModel').validate({
        rules: {
            FirstNameSendOrder: {
                required: true
            },
            LastNameSendOrder: {
                required: true
            },
            PhoneNo: {
                required: true
            },
            Email: {
                required: true
            },
        },
        messages: {
            FirstNameSendOrder: "Please Enter FirstName",
            LastNameSendOrder: "Please Enter Last",
            Email: "Please Enter Email",
            PhoneNo: "Please Enter Email",
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
            debugger
            form.submit();
        }

    });



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
    
    $("#transferproviderCaseForm").validate({
        rules: {
            transferdec: {
                required: true
            }
        },
        messages: {
            transferdec: "Please enter your Reanson."
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
            var TransferNotesByProvider = $('#transferdec').val();
            var requestid = $('#requestIdInputTransferProvider').val();
            $('#transfercaseprovider').modal('hide');
            $.ajax({
                method: 'POST',
                url: '/Provider/TransferCaseProvider',
                data: { TransferDec: TransferNotesByProvider, requestid: requestid },
                success: function (data) {
                    if (data) {
                        var storedPartial = localStorage.getItem('currentPartial');
                        var storedStatus = JSON.parse(localStorage.getItem('currentStatus'));

                        filterTable(storedPartial, storedStatus, 1, 5);
                        updateUIWithCounts();
                        toastr.success('Transfer Request SendTo Admin!');
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
                        window.location.href = '/admin'
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
                required: true
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
                        window.location.href = '/Admin/Dashboard'
                        filterTable(storedPartial, storedStatus, 1, 3);
                        updateUIWithCounts();
                        console.log("toaster", toastr.success)
                        toastr.success('Assign successful!');
                    }
                }
            })
        }
    });

    $("#transformcase").validate({
        rules: {
            notes: {
                required: true
            },
            region: {
                required: true
            },
            physician: {
                required: true
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
            e.preventDefault();
            var requestid = $('#requestIdInputTransfer').val();
            var regionid = $('#regionidtransfer').val();
            var physician = $('#physicianDropdownTransfer').val();
            var description = $('#descriptiontransfer').val();
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

        }
    });

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
                    value: 'selected hidden disable',
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
                required: true
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
                required: 'Please Enter A PhoneNo'
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
                required: false,
                equalTo: "#email"
            },
            PhoneNumber: {
                required: false,
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
            console.log(form)
            form.submit();
        }
    });

    $.validator.addMethod("complexity", function (value, element) {
        var complexityRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/; // Regular expression for password complexity
        return this.optional(element) || complexityRegex.test(value);
    }, "Password must contain a mix of uppercase, lowercase letters, numbers, and special characters");

    $('#createAdmin').validate({
        rules: {
            Username: {
                required: true
            },
            Password: {
                required: true
            },
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Email: {
                required: true,
                email: true
            },
            confirmmail: {
                required: false,
                equalTo: "#email"
            },

            adminRegion: {
                required: true,
                minlength: 1
            },
            State: {
                required: true,
            },
            RoleId: {
                required: true
            }
        },
        messages: {

            FirstName: "Please enter your First Name.",
            LastName: "Please enter your Last Name.",
            Password: "Please Enter Password",
            RoleId: "Please Enter Role",
            Email: {
                required: "Please enter an email address.",
                email: "Please enter a valid email address."
            },
            confirmmail: {
                required: "Please confirm your email address.",
                equalTo: "Please enter the same email as above."
            },

            adminRegion: "Please select at least one region.",
            State: "please Enter State"

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
            console.log(form)
            form.submit();
        }

    })


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
                if (response.isPhysician == true) {
                        window.location.href = '/Provider';
                    } else {
                        window.location.href = '/Admin';
                    }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.status === 500 && jqXHR.responseJSON != null && jqXHR.responseJSON.message === 'Session Is Not Found') {
                    toastr.error("Session has expired. Please log in again.");
                } else {
                    toastr.error("An error occurred. Please try again later.");
                }
            }
        })
    })
});