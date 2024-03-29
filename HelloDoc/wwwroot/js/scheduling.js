$(document).ready(function () {

    $(".open-modal").on("click", function () {
        $("#createShiftModal").modal("show");
    })





    var x = 1;
    var providerList = [];
    var extractedDataArray = [];
    //Get Provider List in array
    function getProviderList() {
        debugger;
        var regionId = $('#fregion').val();
        $.ajax({
            type: "GET",
            url: 'GetPhyscianDataForShift',
            data: { region: regionId },
            dataType: "json",
            success: function (response) {

                providerList = [];
                // Array to store converted objects
                response.forEach(function (item) {
                    var dayList = []; // Array to store converted dayList
                    if (item.dayList) {
                        item.dayList.forEach(function (dayItem) {
                            var dayItemObj = {
                                shiftid: dayItem.shiftid,
                                physicianid: dayItem.physicianid,
                                physicianName: dayItem.physicianName,
                                physicianPhoto: dayItem.physicianPhoto,
                                regionid: dayItem.regionid,
                                startdate: dayItem.startdate,
                                shiftdate: dayItem.shiftdate,
                                starttime: dayItem.starttime,
                                endtime: dayItem.endtime,
                                isrepeat: dayItem.isrepeat,
                                checkWeekday: dayItem.checkWeekday,
                                repeatupto: dayItem.repeatupto,
                                status: dayItem.status,
                                dayList: dayItem.dayList,
                                submit: dayItem.submit
                            };
                            dayList.push(dayItemObj);
                        });
                    }
                    var providerObj = {
                        shiftid: item.shiftid,
                        id: item.physicianid,
                        name: item.physicianName,
                        photo: item.physicianPhoto,
                        regionid: item.regionid,
                        startdate: item.startdate,
                        shiftdate: item.shiftdate,
                        starttime: item.starttime,
                        endtime: item.endtime,
                        isrepeat: item.isrepeat,
                        checkWeekday: item.checkWeekday,
                        repeatupto: item.repeatupto,
                        status: item.status,
                        dayList: dayList,
                        submit: item.submit
                    };
                    providerList.push(providerObj);
                });
                $('#week').click();
                console.log('dd ' + x);
            },
            error: function () {
                // Handle error
                console.error("Error fetching provider list");
            }
        });
    }

    function GetShiftForMonth(month) {
        var regionId = $('#fregion').val();
        $.ajax({
            type: "GET",
            url: 'GetShiftByMonth',
            data: { month: month, regionId: regionId },
            dataType: "json",
            success: function (response) {
                // Initialize an array to store extracted data
                extractedDataArray = [];

                // Iterate over the array
                response.forEach(function (shift) {
                    // Extract and store data from each shift object
                    var extractedData = {
                        //  date: shift.shidtDate.match(/\d{4}-(\d{2})-\d{2}/)[1],
                        date: shift.shiftdate.match(/\d{4}-\d{2}-(\d{2})/)[1],
                        shiftId: shift.shiftid,
                        physicianId: shift.physicianid,
                        physicianName: shift.physicianName,
                        shiftDate: shift.shidtDate, // Correct the typo to "shiftDate"
                        startTime: shift.starttime,
                        endTime: shift.endtime,
                        isRepeat: shift.isrepeat
                    };

                    // Check if dayList exists and is an array
                    if (Array.isArray(shift.dayList)) {
                        // Create an array to store day data
                        var dayDataArray = [];
                        // Iterate over dayList and store data
                        shift.dayList.forEach(function (day) {
                            var dayData = {
                                shiftid: day.shiftid,
                                physicianName: day.physicianName,
                                startTime: day.starttime,
                                endTime: day.endtime,
                                status: day.status,
                            };
                            // Push day data into dayDataArray
                            dayDataArray.push(dayData);
                        });
                        // Add dayDataArray to extractedData object
                        extractedData.dayDataArray = dayDataArray;
                    }

                    // Push extracted data into the array
                    extractedDataArray.push(extractedData);
                });
                console.log(extractedDataArray);
                $('#month').click();
                manipulate();
                // Output the extracted data array to the console
                //console.log(extractedDataArray);
            },
            error: function () {
                // Handle error
                console.error("Error fetching provider list");
            }
        });
    }

    // Call the function to get provider list initially
    getProviderList();

    let date = new Date();
    let year = date.getFullYear();
    let month = date.getMonth();
    console.log(month);
    GetShiftForMonth(month + 1);

    const day = document.querySelector(".calendar-dates");
    const weekdays = document.querySelector(".calendar-weekdays");
    const currdate = document.querySelector(".calendar-current-date");
    const prenexIcons = document.querySelectorAll(".calendar-navigation span");
    const months = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    const daysOfWeek = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    const manipulate = () => {

        weekdays.innerHTML = "";
        daysOfWeek.forEach(day => {
            weekdays.innerHTML += `<th class="text-center">${day}</th>`;
        });
        let dayone = new Date(year, month, 1).getDay();

        // Get the last date of the month
        let lastdate = new Date(year, month + 1, 0).getDate();

        // Get the day of the last date of the month
        let dayend = new Date(year, month, lastdate).getDay();

        // Get the last date of the previous month
        let monthlastdate = new Date(year, month, 0).getDate();

        // Variable to store the generated calendar HTML
        let lit = "<tbody><tr>";
        let dayCount = 0;
        // Loop to add the last dates of the previous month
        for (let i = dayone; i > 0; i--) {
            lit += `<td class="inactive"><div></div><div></div><div></div><div></div><div></div></td>
                        `;
            dayCount++;
        }

        // Loop to add the dates of the current month
        for (let i = 1; i <= lastdate; i++) {

            if (dayCount > 6) {
                lit += `</tr><tr>`;
                dayCount = 0;
            }
            let isToday = i === date.getDate() && month === new Date().getMonth() && year === new Date().getFullYear() ? "active" : "";

            let foundItem = extractedDataArray.find(o => {
                return parseInt(o.date) === i;
            });
            let dayDataArrayfinal = foundItem ? foundItem.dayDataArray : "";
            let ProviderNames = [];
            let Status = [];
            let buttonId = `viewAllButton_${i}`;

            // If dayDataArrayfinal is found, iterate over each item and push ProviderName to ProviderNames array
            if (dayDataArrayfinal) {
                dayDataArrayfinal.forEach(item => {
                    let providerObj = {
                        name: "Name: " + item.physicianName + ' <br/> ' + "Time: " + item.startTime + " to " + item.endTime,
                        status: item.status,
                        shiftid: item.shiftid
                    };
                    ProviderNames.push(providerObj);
                    Status.push(item.status);
                });
            }

            lit += `<td class="table-text ${isToday} p-0"> 
                            <div class="first">${i}</div>

                                    <div class="js-stkModal-btn ${Status.length > 0 ? 'Status-' + Status[0] : ''}"  data-form-id="formEdit_UMS"
                                   data-url='@Url.Action("_EditShift", "Scheduling")?id=${ProviderNames.length > 0 ? ProviderNames[0].shiftid : ''}'
                                           >${ProviderNames.length > 0 ? ProviderNames[0].name : ''}</div>

                                            <div class="${Status.length > 1 ? 'Status-' + Status[1] : ''}" ><a data-form-id="formEdit_UMS"
                                   data-url='@Url.Action("_EditShift", "Scheduling")?id=${ProviderNames.length > 1 ? ProviderNames[1].shiftid : ''}'
                                           class="js-stkModal-btn ">${ProviderNames.length > 1 ? ProviderNames[1].name : ''}</a></div>
                                            <div class="${Status.length > 2 ? 'Status-' + Status[2] : ''}"><a data-form-id="formEdit_UMS"
                                   data-url='@Url.Action("_EditShift", "Scheduling")?id=${ProviderNames.length > 2 ? ProviderNames[2].shiftid : ''}'
                                           class="js-stkModal-btn ${Status.length > 2 ? 'Status-' + Status[2] : ''}">${ProviderNames.length > 2 ? ProviderNames[2].name : ''}</a></div>
                                    <div type="button"  id="${buttonId}" data-bs-toggle="modal" data-bs-target="${ProviderNames.length > 3 ? '#exampleModal' : ''}" data-month="${month}" data-index="${i}" class="${Status.length > 3 ? 'btn btn-info w-100 text-white rounded-0 view-all-btn' : ''}">${ProviderNames.length > 3 ? 'View All' : ''}</div>
                        </td>`;
            dayCount++;
        }

        // Loop to add the first dates of the next month
        for (let i = dayend; i < 6; i++) {
            lit += `<td class="inactive">
                                    <div></div><div></div><div></div><div></div><div></div>
                        </td>`;
        }
        lit += "</tr></tbody>";
        // Update the text of the current date element with the formatted current month and year
        currdate.innerText = `${months[month]} ${year}`;

        // Update the HTML of the dates element with the generated calendar
        day.innerHTML = lit;
    };


    $(document).on('click', '.view-all-btn', function () {

        // Get the index associated with the clicked button
        let index = parseInt($(this).data('index'));
        let month = parseInt($(this).data('month'));
        $('#model-head').html(index + ' / ' + months[month] + ' / ' + year);
        // Retrieve the corresponding dayDataArrayfinal
        let foundItem = extractedDataArray.find(o => parseInt(o.date) === index);
        let dayDataArrayfinal = foundItem ? foundItem.dayDataArray : [];

        // Clear previous modal content
        $('.modal-body').empty();

        // Populate modal with shift details
        dayDataArrayfinal.forEach(item => {
            let shiftDetails = `
                                                            <div class=" my-1 p-2 ${'model-Status-' + item.status}" >
                            <div>Physician Name: ${item.physicianName}</div>
                            <div>Start Time: ${item.startTime}</div>
                            <div>End Time: ${item.endTime}</div>
                            <div>
                            `;
            $('.modal-body').append(shiftDetails);
        });
    });


    manipulate();

    // Attach a click event listener to each icon
    prenexIcons.forEach(icon => {
        // When an icon is clicked
        icon.addEventListener("click", () => {
            if (x == 1) {
                // Check if the icon is "calendar-prev" or "calendar-next"
                month = icon.id === "calendar-prev" ? month - 1 : month + 1;
                console.log(x);
                // Check if the month is out of range
                if (month < 0 || month > 11) {
                    // Set the date to the first day of the month with the new year
                    date = new Date(year, month, new Date().getDate());

                    // Set the year to the new year
                    year = date.getFullYear();

                    // Set the month to the new month
                    month = date.getMonth();
                } else {
                    // Set the date to the current date
                    date = new Date();
                }

                // Call the manipulate function to update the calendar display
                GetShiftForMonth(month + 1);
            }

        });
    });

    // Event listener for the "Day" button
    document.getElementById("day").addEventListener("click", () => {
        $('#monthcontainer').hide();
        $('#weekviewcontainer').hide();
        $('#daycontainer').show();

        x = 3;
        // Define variables for current date and month
        let currentDate = new Date();
        let currentMonth = currentDate.getMonth();

        // Event listener for the "Next" button
        $('#calendar-next').on('click', function () {
            if (x == 3) {

                currentDate.setDate(currentDate.getDate() + 1);

                updateEmployeeSchedules();
            }

        });

        $('#calendar-prev').on('click', function () {
            if (x == 3) {
                currentDate.setDate(currentDate.getDate() - 1);

                updateEmployeeSchedules();
            }
        });

        updateEmployeeSchedules();

        function updateEmployeeSchedules() {
            $('.calendar-current-date').text(currentDate.toLocaleDateString('en-US', { month: 'short', year: 'numeric', day: 'numeric' }));

            $('#daycontainer .schedule .employee-schedules tbody').empty();
            const employeeSchedulesContainer = document.querySelector('.employee-schedules tbody');
            const shiftArrays = [];
            for (let index = 0; index < providerList.length; index++) {
                const employee = providerList[index];
                const employeeRow = document.createElement('tr');
                const employeeCell = document.createElement('td');
                employeeCell.classList.add("w-25");

                // Create the image element
                const image = document.createElement('img');
                image.classList.add("img-fluid");
                image.classList.add("ph-img");
                image.src = '/Upload/Physician/' + employee.id + '/photo.png';
                image.alt = employee.name;
                employeeCell.appendChild(image);

                employeeCell.appendChild(document.createTextNode(employee.name));
                const shiftArray = Array(24).fill(0);
                employeeRow.appendChild(employeeCell);
                const cellDate = new Date(currentDate);

                const matchingDates = providerList[index].dayList.filter(item => {
                    const shiftdate = new Date(item.shiftdate);
                    return cellDate.getDate() === shiftdate.getDate() &&
                        cellDate.getMonth() === shiftdate.getMonth() &&
                        cellDate.getFullYear() === shiftdate.getFullYear()
                });

                const timeRanges = matchingDates.map(item => {
                    const startTime = item.starttime;
                    const endTime = item.endtime;
                    let status = 0; // Default status is 0 (not available)
                    const shiftid = item.shiftid;
                    const regionname = item.regionname;

                    // Check the status of the shift
                    if (item.status === 1) {
                        status = 1; // Available
                    } else if (item.status === 0) {
                        status = 0.5; // Partially available
                    }

                    return { startTime, endTime, status, shiftid, regionname };
                });
                const hoursArray = Array(24).fill(0);
                const shiftidArray = Array(24).fill(0);
                const regionArray = Array(24).fill(0);

                timeRanges.forEach(({ startTime, endTime, status, shiftid, regionname }) => {
                    // Parse start and end times to obtain hours
                    const startParts = startTime.split(' ')[0].split(':');
                    const endParts = endTime.split(' ')[0].split(':');

                    // Extract hours
                    const startHour = parseInt(startParts[0]);
                    const endHour = parseInt(endParts[0]);

                    // Determine the value to store based on status
                    const valueToStore = status;
                    for (let hour = startHour; hour <= endHour; hour++) {
                        shiftidArray[hour] = shiftid;
                    }

                    // Loop through hours and mark the corresponding index in the array if it falls within the range
                    for (let hour = startHour; hour <= endHour; hour++) {
                        hoursArray[hour] = valueToStore;
                        regionArray[hour] = regionname;
                    }
                });



                shiftArrays.push(shiftArray);

                for (let i = 0; i < 24; i++) {

                    const hour = i < 12 ? i === 0 ? 12 : i : i === 12 ? 12 : i - 12;
                    const amPm = i < 12 ? 'AM' : 'PM';
                    const timeSlot = document.createElement('td');

                    if (hoursArray[i] === 1) {
                        timeSlot.innerHTML += `<a data-form-id="formEdit_UMS" data-url="@Url.Action("_EditShift", "Scheduling")?id=${shiftidArray[i]}" class="mt-1 d-block text-dark js-stkModal-btn fs-5 Status-1">${regionArray[i]}</a>`;
                        timeSlot.classList.add('p-0');
                    } else if (hoursArray[i] === 0.5) {
                        timeSlot.innerHTML += `<a data-form-id="formEdit_UMS" data-url="@Url.Action("_EditShift", "Scheduling")?id=${shiftidArray[i]}" class="mt-1 d-block text-dark js-stkModal-btn fs-5 Status-0">${regionArray[i]}</a>`;
                        timeSlot.classList.add('p-0');
                    }
                    else {
                        timeSlot.textContent = '';
                        timeSlot.classList.add('shift');
                    }
                    employeeRow.appendChild(timeSlot);
                }
                employeeSchedulesContainer.appendChild(employeeRow);
            }
        }

        // Update the employee schedules table
        updateEmployeeSchedules();
    });

    // Event listener for the "Week" button
    document.getElementById("week").addEventListener("click", () => {
        x = 2;
        $('#monthcontainer').hide();
        $('#weekviewcontainer').show();
        $('#daycontainer').hide();

        if ($('#weekviewcontainer').length > 0) {
            Date.prototype.GetFirstDayOfWeek = function () {
                return new Date(this.setDate(this.getDate() - this.getDay() + (this.getDay() == 0 ? -6 : 1)));
            };
            Date.prototype.getWeek = function () {
                var onejan = new Date(this.getFullYear(), 0, 1);
                return Math.ceil((((this - onejan) / 86400000) + onejan.getDay() + 1) / 7);
            };

            var nav = 0;
            var cnav = 0;
            var inc = 1;
            var dec = 1;
            var wnav = 0;
            var wlnav = 0;
            var nd = new Date();
            var ndate = nd.getDate();
            var nday = nd.getDay();
            var weekOfMonth = Math.ceil((ndate - 1 - nday) / 7);
            nav = weekOfMonth;
            var fdweek = nd.GetFirstDayOfWeek();
            var getweekdt = nd.GetFirstDayOfWeek();

            // Define click event handler for the previous button
            $('#calendar-prev').click(function () {
                if (x == 2) {
                    if (nav == 5) {
                        wnav = 5;
                    }
                    nav--;
                    if (nav == -1) {
                        nav = 4;
                        fdweek.setDate(fdweek.getDate() - 5);
                        getweekdt.setDate(getweekdt.getDate() - 5);
                        inc = 0;
                        dec = 1;
                        cnav--;
                    } else {
                        fdweek.setDate(fdweek.getDate() - 12);
                        getweekdt.setDate(getweekdt.getDate() - 12);
                    }
                    loadWeekCalendar();
                }
                // Decrement week index

            });

            // Define click event handler for the next button
            $('#calendar-next').click(function () {
                if (x == 2) {
                    nav++;
                    wlnav = 5;
                    if ($('thead th[data-week="5"]').length > 0) {
                        wlnav = 6;
                    }
                    if (nav == wlnav) {
                        nav = 0;
                        fdweek.setDate(fdweek.getDate() - 5);
                        getweekdt.setDate(getweekdt.getDate() - 5);
                        dec = 0;
                        inc = 1;
                        cnav++;
                    } else {
                        fdweek.setDate(fdweek.getDate() + 2);
                        getweekdt.setDate(getweekdt.getDate() + 2);
                        wnav = 5;
                    }
                    loadWeekCalendar();
                    wnav = 0;
                }

            });

            $(document).on('click', '.view-all-btn-week', function () {
                // Clear modal body content
                $('.modal-body').empty();

                // Set modal title
                // Get the index associated with the clicked button
                let shiftdate = new Date($(this).data('shiftdate'));
                let index = parseInt($(this).data('index'));

                $('#model-head').html(shiftdate.getDate() + ' / ' + months[shiftdate.getMonth()] + ' / ' + shiftdate.getFullYear());

                const matchingDates = providerList[index].dayList.filter(item => {
                    const rshiftdate = new Date(item.shiftdate);
                    return shiftdate.getDate() == rshiftdate.getDate() && shiftdate.getMonth() == rshiftdate.getMonth() + 1 && shiftdate.getFullYear() == rshiftdate.getFullYear();
                });

                matchingDates.forEach((item, index) => {
                    // Construct HTML for request details
                    const requestHTML = `<div class=" my-1 p-2 ${'model-Status-' + item.status}" >
                                    <div>Physician Name: ${item.physicianName}</div>
                                            <div>Start Time: ${item.starttime}</div>
                                    <div>End Time: ${item.endtime}</div>
                                    <div>`;

                    // Append request details to modal body
                    $('.modal-body').append(requestHTML);
                });

                // Open the modal
                $('#exampleModal').modal('show');
            });

            // Define function to load the week calendar--------------------------WEEk-------------------------TABLE-----------------------------------------------
            function loadWeekCalendar() {
                x = 2;
                var tbody = document.querySelector('.main');
                tbody.innerHTML = "";
                // Check if tbody exists
                if (tbody) {
                    // Add rows dynamically
                    for (var i = 0; i < providerList.length; i++) {
                        var row = document.createElement('tr');
                        for (var j = 0; j < 8; j++) {
                            var cell = document.createElement('td');
                            if (j === 0) {
                                cell.classList.add("w-25");
                                var image = document.createElement('img');
                                image.classList.add("img-fluid");
                                image.classList.add("ph-img");
                                image.src = '/Upload/physician/' + providerList[i].id + '/' + 'photo.png'; // Set the image URL from ProviderData
                                image.alt = providerList[i].name; // Set the alt text for accessibility
                                cell.appendChild(image);
                                cell.appendChild(document.createTextNode(providerList[i].name)); // Add text content after the image

                                // First cell, add some content or leave it empty
                                // cell.textContent = ProviderData[i].name; + (i + 1);
                            } else {
                                // Other cells, you can add your content or leave them empty
                                const currentDate = new Date();
                                const cellDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate() + j - 3);
                                // cell.textContent = fdweek.getDate() + j - 2;
                                //cell.textContent += fdweek.getMonth() + 1;
                                //cell.textContent += fdweek.getFullYear();
                                //cell.textContent += cellDate;
                                // console.log(fdweek);
                                const matchingDates = providerList[i].dayList.filter(item => {
                                    const shiftdate = new Date(item.shiftdate);
                                    return fdweek.getDate() + j - 2 == shiftdate.getDate() && fdweek.getMonth() + 1 == shiftdate.getMonth() + 1 && fdweek.getFullYear() == shiftdate.getFullYear();
                                });
                                // If matching dates are found, count them and print the count in the cell
                                if (matchingDates.length > 0) {
                                    if (matchingDates.length <= 2) {
                                        matchingDates.forEach((item, index) => {
                                            cell.classList.add('p-0');
                                            cell.innerHTML += `<a data-form-id="formEdit_UMS" data-url="@Url.Action("_EditShift", "Scheduling")?id=${item.shiftid}" class="mt-1 d-block text-dark js-stkModal-btn w-Status-${item.status}">${item.starttime} - ${item.endtime}</a>`;
                                        });
                                    } else {
                                        let buttonId = `viewAllButton_${i}${j}`;
                                        for (let i = 0; i < 2; i++) {
                                            const item = matchingDates[i];
                                            cell.classList.add('p-0');
                                            cell.innerHTML += `<a data-form-id="formEdit_UMS" data-url="@Url.Action("_EditShift", "Scheduling")?id=${item.shiftid}" class="mt-1 d-block text-dark js-stkModal-btn w-Status-${item.status}">${item.starttime} - ${item.endtime}</a>`;
                                        }
                                        const newdate = new Date(fdweek.getFullYear(), fdweek.getMonth() + 1, fdweek.getDate() + j - 2);

                                        cell.innerHTML += `<div type="button" id="${buttonId}" data-bs-toggle="modal" data-bs-target="#exampleModal" data-shiftdate="${newdate}" data-index="${i}" class="btn btn-info w-100 text-white rounded-0 view-all-btn-week">View All</div>`;
                                    }
                                    const count = matchingDates.length;
                                }
                            }
                            row.appendChild(cell);
                        }
                        tbody.appendChild(row);
                    }
                }

                // Above function to load the week calendar--------------------------WEEk-------------------------TABLE-----------------------------------------------

                const formatDate = new Intl.DateTimeFormat("en", { day: "2-digit" });
                const headwdate = new Intl.DateTimeFormat("en", { year: 'numeric', month: 'short', day: 'numeric' });
                var options = { weekday: 'short', year: 'numeric', month: 'long', day: 'numeric' };
                const dt = new Date();
                if (cnav !== 0) { dt.setMonth(new Date().getMonth() + cnav); }
                const year = dt.getFullYear();
                const month = dt.getMonth();
                const day = dt.getDate();
                const week = dt.getWeek();
                const firstDayOfMonth = new Date(year, month, 1).toLocaleDateString('en-US', options);
                const lastDayOfMonth = new Date(year, month + 1, 0).toLocaleDateString('en-US', options);
                var firstday = firstDayOfMonth.split(',')[0];
                var lastday = lastDayOfMonth.split(',')[0];
                var daytype = new Array('Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat');
                var i = daytype.indexOf(firstday);
                var j = daytype.indexOf(lastday) + 1;
                var removefirst = daytype.slice(0, i);
                var removelast = daytype.slice(j, daytype.length);
                $('td,th').css("background-color", "");
                const firstweek = new Date(year, month, 1).getWeek();
                const lastweek = new Date(year, month + 1, 0).getWeek();

                var used = new Date(year, month, 1).getDay() + new Date(year, month + 1, 0).getDate();
                var numofweeks = Math.ceil(used / 7);

                removefirst.forEach(removefirstcol);
                removelast.forEach(removelastcol);
                function removefirstcol(item, index) { $('td[data-week="0"][data-name="' + item + '"').css({ "background-image": "none", "background-color": "#c9c9c9", "cursor": "not-allowed" }); }
                function removelastcol(item, index) {
                    if ($('thead th[data-week="5"]').length > 0) {
                        $('td[data-week="5"][data-name="' + item + '"').css({ "background-image": "none", "background-color": "#c9c9c9", "cursor": "not-allowed" });
                    } else {
                        $('td[data-week="4"][data-name="' + item + '"').css({ "background-image": "none", "background-color": "#c9c9c9", "cursor": "not-allowed" });
                    }
                }
                if (nav === 4 && $('thead th[data-week="5"]').length > 0 && wnav != 5) { nav = 5; }
                $('th[data-week="' + nav + '"').css({ "background-image": "none", "background-color": "#fff" });
                $('td[data-week="' + nav + '"').css({ "background-image": "none", "background-color": "#fff" });
                if (removelast.length === 0) {
                    if (inc === 0) {
                        fdweek.setDate(fdweek.getDate() - 7);
                        getweekdt.setDate(getweekdt.getDate() - 7);
                        inc++;
                    }
                }
                if (removefirst.length === 0) {
                    if (dec === 0) {
                        fdweek.setDate(fdweek.getDate() + 7);
                        getweekdt.setDate(getweekdt.getDate() + 7);
                        dec++;
                    }
                }

                const fweekdt = headwdate.format(getweekdt.setDate(getweekdt.getDate() - 1));
                console.log(fweekdt);
                const lweekdt = headwdate.format(getweekdt.setDate(getweekdt.getDate() + 6));
                var fmondt = fweekdt.split(' ')[0];
                var lmondt = lweekdt.split(' ')[0];
                var fdaydt = fweekdt.split(' ')[1];
                fdaydt = fdaydt.split(',')[0];
                var ldaydt = lweekdt.split(' ')[1];
                var fyeardt = fweekdt.split(',')[1];
                var lyeardt = lweekdt.split(',')[1];
                if (fyeardt == lyeardt && fmondt == lmondt) { $('.calendar-current-date').html(fmondt + " " + fdaydt + " - " + ldaydt + fyeardt); }
                if (fyeardt == lyeardt && fmondt != lmondt) { $('.calendar-current-date').html(fmondt + " " + fdaydt + " - " + lmondt + " " + ldaydt + fyeardt); }
                if (fyeardt != lyeardt && fmondt != lmondt) { $('.calendar-current-date').html(fweekdt + " - " + lweekdt); }
                $('.title[data-name="Sun"').html("Sun - " + formatDate.format(fdweek.setDate(fdweek.getDate() - 1)));
                $('.title[data-name="Mon"').html("Mon " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                $('.title[data-name="Tue"').html("Tue " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                $('.title[data-name="Wed"').html("Wed " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                $('.title[data-name="Thu"').html("Thu " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                $('.title[data-name="Fri"').html("Fri " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
                $('.title[data-name="Sat"').html("Sat " + formatDate.format(fdweek.setDate(fdweek.getDate() + 1)));
            }

            // Initial load of the week calendar
            loadWeekCalendar();
        }
    });

    // Event listener for the "Month" button
    document.getElementById("month").addEventListener("click", () => {
        $('#monthcontainer').show();
        $('#weekviewcontainer').hide();
        $('#daycontainer').hide();
        x = 1
        manipulate();
    });

    $('#fregion').on('change', function () {
        if (x == 1) {
            GetShiftForMonth(month + 1);
        } else if (x == 2) {
            getProviderList();
        } else {
            // Handle other cases if needed
        }
    });

});

window.onload = function () {
    $('#monthcontainer').show();
    $('#weekviewcontainer').hide();
    $('#daycontainer').hide();
};