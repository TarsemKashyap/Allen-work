"use strict";
(function () {
    app.controller("HReMRAdminController", ["$scope", "HReMRAdminService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams","$timeout", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams, $timeout) {
        //sharepoint context
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
        var siteAbsoluteUrl = _spPageContextInfo.siteAbsoluteUrl;
        //init variable
        //mail config
        var hostName = "";
        var portNumber = 587;

        var enableSSL = true;
        var password = "";
        var toEmails = ""; //using multiples email
        var fromEmail = "";
        var dispName = "";
        var bodyMsg = "";
        $scope.gap = 2;
        $scope.filteredItems = [];
        $scope.groupedItems = [];
        $scope.itemsPerPage = 10;
        $scope.pagedItems = [];
        $scope.currentPage = 0;
        $scope.SysServiceRequestData = [];
        $scope.SysRequestDataAll = [];
        $scope.WorkAreaData = [];
        $scope.TypeOfReqData = []; 
        $scope.appChildProperties = ({
            //Current User Properties    
            DelegateUserItem: "",
            CurrentUser: ({ Id: "", Email: "", Title: "" }),
            DelegateUser: ({ Id: "", EMail: "", Title: "" }),
            DelegateManagerMaster: ({ Id: "", EMail: "", Title: "" }),
            appComments: ({
                ID: "",
                Title: "",
                Comments: "",
                ListItemID: "",
                Author: ({ Id: "", EMail: "", Title: "" }),
            }),
        });
        $scope.appProperties = ({ 
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            ImplementationOfficerMaster: ({ Id: "", EMail: "", Title: "" }),
            ITProjectManagerMaster: ({ Id: "", EMail: "", Title: "" }),
            GroupDirectorMaster: ({ Id: "", EMail: "", Title: "" }),
            CIOMaster: ({ Id: "", EMail: "", Title: "" }),
            HeadofDeptName: "",            
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", DomainName:"" }),
        });        
        $scope.searchText = '';
        // init
        $scope.sort = {
            sortingOrder: 'ID',
            reverse: false
        };
        $scope.init = function () {
            BindMasterData();
            Utility.helpers.initializePeoplePicker('peoplePickerDelegateUser');
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerDelegateUser_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                //chnage event
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    // Get the first user's ID by using the login name.
                    getUserId(userData.Key).done(function (user) {
                        $scope.appChildProperties.DelegateUser.Id = user.d.Id;
                        $scope.appChildProperties.DelegateUser.EMail = user.d.Email;
                        $scope.appChildProperties.DelegateUser.Title = user.d.Title;
                    });
                } else {
                    $scope.appChildProperties.DelegateUser.Id = "";
                }
            };
            commonService.getCurrentUser().then(function (result) {
                $scope.appProperties.CurrentUser = result.data.d  
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];                   
                }                    
               
                $scope.getAllRequests();
              //  getAllRequestTypeMaster();
                //load data
              //  bindUsersData();// data
            });          
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['HRAdminGroup'];
                //determine wether current user is a admin of group(s) 
                var userGroups = result.data.d.Groups.results;
                var foundGroups = userGroups.filter(function (g) { return groupNames.indexOf(g.LoginName) > -1 });
                if (foundGroups.length > 0) {
                    //get admin data
                    $scope.isAdminGroup = true;
                }
                else {
                    $scope.isAdminGroup = false;
                }
            });      
            //get mail settings
            commonService.getMailConfigFromRootSite(Config.EMailConfig).then(function (response) {
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == "hostName") {
                            hostName = value.Value;
                        }
                        if (value.Title == "portNumber") {
                            portNumber = parseInt(value.Value);
                        }
                        if (value.Title == "password") {
                            password = value.Value;
                        }
                        if (value.Title == "dispName") {
                            dispName = value.Value;
                        }
                        if (value.Title == "fromEmail") {
                            fromEmail = value.Value;
                        }
                    });

                }
            });
        };
        function BindMasterData() { //inprogress//dpplname,id        
            uService.getAllReq(Config.WorkArea).then(function (response) {
                $scope.WorkAreaData = response.data.d.results;
                $scope.loaded = false;// while success loading false              
            });
            uService.getAllReq(Config.TypeOfRequest).then(function (response) {
                $scope.TypeOfReqData = response.data.d.results;
                $scope.loaded = false;// while success loading false              
            });
        }
        $scope.getReqType = function (code) {
            var currentlistItem = $filter('filter')($scope.TypeOfReqData, { RequestTypeCode: code })[0];
            if (currentlistItem != null && currentlistItem != "") {
                return currentlistItem.Title // get the user name from it
            }
            else {
                return "IT Service" // get the user name from it
            }
        }
        $scope.getWorkAreadCode = function (code) {
            var currentlistItem = $filter('filter')($scope.WorkAreaData, { WorkAreaCode: code })[0];
            if (currentlistItem != null && currentlistItem != "") {
                return currentlistItem.Title // get the user name from it
            }
            else {
                return "IT Service" // get the user name from it
            }
        }
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.RequestTypeCode = "";
            $scope.appProperties.ApplicationID = "";
            $scope.appProperties.ManpowerRequestType = "";
            $scope.itemsPerPage = "";
            $scope.searchText = "";
            $scope.search($scope.SysServiceRequestData);
        }
        //Search...
        $scope.search = function (data) {

            if (data == undefined) {
                data =   $scope.SysServiceRequestData;
            }
            if ($scope.appProperties.ApplicationID || $scope.appProperties.ApplicationStatus) {

                if ($scope.appProperties.ApplicationID != "") {
                    $scope.searchText = $scope.appProperties.ApplicationID;
                }
                if ($scope.appProperties.ApplicationStatus != "") {
                    $scope.searchText = $scope.appProperties.ApplicationStatus;
                }
                $scope.filteredList = commonService.getSearchedHRRequests(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = "";
            }
            else if ($scope.appProperties.ManpowerRequestType) {
                $scope.searchText = $scope.appProperties.ManpowerRequestType;
                $scope.filteredList = commonService.getSearchedHRRequests(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = ""; //after search clear text
            }
            else if ($scope.appProperties.RequestTypeCode) {
                $scope.searchText = $scope.appProperties.RequestTypeCode;
                $scope.filteredList = commonService.getSearchedHRRequests(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = ""; //after search clear text
            }
            else if (($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) && ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null)) {
                if ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null) {
                    var dateParts = $scope.appProperties.ApplicationDate.split("/");
                    // month is 0-based, that's why we need dataParts[1] - 1
                    var fromDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                }
                if ($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) {
                    var dateParts = $scope.appProperties.ApplicationEndDate.split("/");
                    // month is 0-based, that's why we need dataParts[1] - 1
                    var toDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                }
                $scope.filteredList = commonService.getSearchedHRRequests(data, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else if ($scope.searchText != "") {
                $scope.filteredList = commonService.getSearchedHRRequests(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
            }
            else {
                $scope.filteredItems = data;
            }
            if ($scope.itemsPerPage == "") {
                $scope.itemsPerPage = data.length;

            }
            $scope.currentPage = 0;
            if ($scope.itemsPerPage == "") {
                $scope.itemsPerPage = $scope.filteredItems.length;
            }
            if ($scope.itemsPerPage == 10 || $scope.itemsPerPage == 30 || $scope.itemsPerPage == 50) {

                var newArr = [];
                for (var i = 0; i < $scope.filteredItems.length; i++) {
                    newArr[i] = $scope.filteredItems[$scope.filteredItems.length - i - 1];
                }
                $scope.filteredItems = newArr;
            }
            // now group by pages
            $scope.groupToPages();
        }
        //Search...
        $scope.searchold = function () {
            if ($scope.searchText == '') {
                $scope.filteredItems = $scope.SysServiceRequestData;
            }
            else {
                $scope.filteredItems = commonService.searchedHRAdmnin($scope.SysServiceRequestData, $scope.searchText);
            }
            $scope.currentPage = 0;
            if ($scope.itemsPerPage == "") {
                $scope.itemsPerPage = $scope.filteredItems.length;
            }
            if ($scope.itemsPerPage == 10 || $scope.itemsPerPage == 30 || $scope.itemsPerPage == 50) {

                var newArr = [];
                for (var i = 0; i < $scope.filteredItems.length; i++) {
                    newArr[i] = $scope.filteredItems[$scope.filteredItems.length - i - 1];
                }
                $scope.filteredItems = newArr;
            }
            // now group by pages
            $scope.groupToPages();
        }
        // calculate page in place
       
        $scope.groupToPages = function () {
            $scope.pagedItems = [];
            if ($scope.filteredItems.length < $scope.itemsPerPage) {               
                $scope.pagedItems.push($scope.filteredItems);
            }
            else if ($scope.filteredItems.length > 10) {
                for (var i = 0; i < $scope.filteredItems.length; i++) {
                    if (i % $scope.itemsPerPage === 0) {
                        $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)] = [$scope.filteredItems[i]];
                    } else {
                        $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)].push($scope.filteredItems[i]);
                    }
                }
            }
            else {
                $scope.pagedItems.push($scope.filteredItems);
            }
        };
        $scope.range = function (size, start, end) {
            var ret = [];         
            if (size < end) {
                end = size;
                start = size - $scope.gap;
            }
            for (var i = start; i < end; i++) {
                ret.push(i);
            }
            return ret;
        };
        $scope.prevPage = function () {
            if ($scope.currentPage > 0) {
                $scope.currentPage--;
            }
        };
        $scope.nextPage = function () {
            if ($scope.currentPage < $scope.pagedItems.length - 1) {
                $scope.currentPage++;
            }
        };
        $scope.setPage = function () {
            $scope.currentPage = this.n;
        };
        // Pass the `review` object as argument
        $scope.getStatus = function (val) {
            var status = ""
            if (val == 1)
                status = "Pending";
            if (val == 2)
                status = "Approved";
            if (val == 3)
                status = "Rejected";
            if (val == 4)
                status = "Closed";
            if (val == 5)
                status = "Re-Routed";
            return status; //Config.CRSRWFStatus[val]; // get the user name from it
        }
        $scope.ExportExcel = function () {
            $("#ExportXL").table2excel({
                filename: "HReMRFReport.xls"
            });
        }
        //get Authorization from csutom list
        function getAllRequestTypeMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllRequestTypeMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.RequestTypeMaster = response.data.d.results;
                }
            });
        };
        // Pass the `review` object as argument
        $scope.getWFStatus = function (item) {
            var status = "";
            if ((item.ApplicationStatus == 1 || item.ApplicationStatus == 5) && item.Level1Approver.Id != undefined && item.Level2Approver.Id == undefined && item.Level3Approver.Id == undefined)
                status = "Pending with " + item.Level1Approver.Title;
            else if (item.Level1ApprovalStatus == "Pending" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id == undefined)
                status = "Pending with " + item.Level1Approver.Title;
            else if (item.Level2ApprovalStatus == "Pending" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id != undefined)
                status = "Pending with " + item.Level2Approver.Title;
            else if (item.Level1ApprovalStatus == "Approved" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id == undefined)
                status = "Pending with " + item.Level2Approver.Title;
            else if (item.Level2ApprovalStatus == "Approved" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id != undefined)
                status = "Approved"
            else if (item.Level1ApprovalStatus == "Rejected" && item.Level1Approver.Id != undefined && item.Level2Approver.Id == undefined && item.Level3Approver.Id == undefined)
                status = "Rejected by " + item.Level1Approver.Title;
            else if (item.Level2ApprovalStatus == "Rejected" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id == undefined)
                status = "Rejected by " + item.Level2Approver.Title;
            else if (item.Level3ApprovalStatus == "Rejected" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id != undefined)
                status = "Rejected by " + item.Level3Approver.Title;
            return status;
        }

        $scope.getAllRequstorData = function () {
            //call service
            $scope.loaded = true;
            uService.getAllRequestorSysRequests (_spPageContextInfo.userId).then(function (response) {
                $scope.SysServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.SysServiceRequestData);
            });
        };
        $scope.getAllRequests = function () {     
            //call service
            $scope.loaded = true;
            uService.getAllSysRequests().then(function (response) {
                $scope.SysRequestDataAll = response.data.d.results;    //all data maintaine here
                $scope.SysServiceRequestData = response.data.d.results; //serch data
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.SysServiceRequestData);
            });
        };
        //pending results - while clickin then call service and bind data
        $scope.getPending = function () {
            $scope.SysServiceRequestData.length = 0;
            angular.forEach($scope.SysRequestDataAll, function (value, key) {
             
                if ($scope.appProperties.CurrentUser.Id == value.Level1Approver.Id && value.Level1ApprovalStatus == "Pending") {
                    $scope.SysServiceRequestData.push(value);
                }
                if ($scope.appProperties.CurrentUser.Id == value.Level2Approver.Id && value.Level1Approva2Status == "Pending") {
                    $scope.SysServiceRequestData.push(value);
                }               
                
            });

            $scope.search($scope.SysServiceRequestData);//this one binding for all sitution
        };
      
        // Get the user ID.
        function getUserId(userName) {
            var call = $.ajax({
                url: siteAbsoluteUrl + "/_api/web/siteusers(@v)?@v='" + encodeURIComponent(userName) + "'",
                method: "GET",
                async: false,
                headers: { "Accept": "application/json; odata=verbose" },
            });
            return call;
        }
        function bindUsersData() {
            $scope.loaded = true;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == Config.Roles['BranchHead']) {
                            //$scope.appProperties.BranchHeadMaster = value.Approvers.results; // change req -get from starff direct
                        }
                        if (value.Title == Config.Roles['ModuleOwner']) {
                            $scope.appProperties.ModuleOwnerMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['ImplementationOfficer']) {
                            $scope.appProperties.ImplementationOfficerMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['GroupDirector']) {
                            $scope.appProperties.GroupDirectorMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['CIO']) {
                            $scope.appProperties.CIOMaster = value.Approvers.results;
                        }
                    });
                }
            });
        };   

        //pending results- Get all application inprogress status based on user
        $scope.getOnGoing = function () {
            onGoingData(1)  // InProgress
        };
        function onGoingData(appStaus) { //inprogress//dpplname,id        
            $scope.loaded = true;
            uService.getOngoingRequestsWithAppStatus(appStaus).then(function (response) {
                $scope.SysServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.SysServiceRequestData);
            });
        }
        function closedData(approvedStaus, closedStaus) { //closed(4/5)//dpplname,id   
            $scope.loaded = true;
            uService.getClosedRequestsWithAppStatus(approvedStaus, closedStaus).then(function (response) {
                $scope.SysServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.SysServiceRequestData);
            });
        }
        //approved/rejected results-not equal to pending
        $scope.getClosed = function () {
            closedData(3, 4);
        };
        $scope.ListItemId = 0;
        $('#my_modal').on('show.bs.modal', function (e) {
            var listItemId = $(e.relatedTarget).data('id');
            $scope.ListItemId = $(e.relatedTarget).data('id');
           // $(e.currentTarget).find('input[name="ListItemId"]').val(listItemId);
        });
        //assing parent value to child        
        $scope.submitSysRequest = function (type, $event, Form) {
            if ($scope.appChildProperties.DelegateUser.Id != "" && $scope.appChildProperties.DelegateUser.Id != null) {
                $scope.loaded = true;
                var itemId = $('#ListItemId').val();

                var currentlistItem = $filter('filter')($scope.SysServiceRequestData, { Id: $scope.ListItemId })[0];
                if (currentlistItem != undefined) {
                    $('.modal-backdrop').remove();
                    $('#my_modal').modal('hide');

                    var ppobject = SPClientPeoplePicker.SPClientPeoplePickerDict["peoplePickerDelegateUser_TopSpan"];
                    ppobject.DeleteProcessedUser()
                    $('#resolvedUsers').html("");
                    $('#userKeys').html("");
                    $('#userProfileProperties').html("");
                    $('#userID').html("");

                    insertUpdateListItem(currentlistItem, 21, 21, "Re-Delegate successfully"); //10-Re-Delegate Approver, 10-Re-Delegate Approver
                }

            }
            else {
                alert('Please select delegate user..!');
            }

        };
        //insert/update IT 
        function insertUpdateListItem(currentlistItem, Worfkflowstatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.HRRequests + "");
            var listItem = "";
            listItem = list.getItemById(currentlistItem.Id);
            //date -approve/rejecte date assing in disabled function
            //Branch head  approval
            if ($scope.appChildProperties.DelegateUser.Id) {
                if (currentlistItem.Level1ApprovalStatus == "Pending") {
                    listItem.set_item("Level1Approver", $scope.appChildProperties.DelegateUser.Id);
                }             
                else if (currentlistItem.Level2ApprovalStatus == "Pending") {
                    listItem.set_item("Level2Approver", $scope.appChildProperties.DelegateUser.Id);
                }               
                else { //dosome 
                    alert("Status already Approved/Rejected");
                    return;
                }
                //workflow general status 
                listItem.set_item("ApplicationStatus", 5);
                listItem.update();
                ctx.load(listItem);
            }
            ctx.executeQueryAsync(function () {
                insertComments(currentlistItem.Id, commentStatus);
                //send email to DelegateUser/user- Submitted for admin
                if ($scope.appChildProperties.DelegateUser.EMail) {
                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appChildProperties.DelegateUser.EMail, "", "", "HR eMRF Application Submitted by " + currentlistItem.Author.Title + "", "This is to notify you that HR eMRF Application " + currentlistItem.ApplicationID + " from " + currentlistItem.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here</a>");
                }
                $timeout(function () {
                    $scope.loaded = false;
                    alert(message);
                    $scope.getAllRequests();
                    //  $uibModalInstance.close();

                    //  window.location.href = "/erequest/Pages/ITAdminDashboard.aspx";
                }, 3000);
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };
        //insert comments
        function insertComments(listItemId, status) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.HRComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            clistItem.set_item('UserComments', Config.SysCommentStatus[status] + " (System Comments)");
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('HRRequestID', listItemId);
            clistItem.update();
            ctx.load(clistItem);
            ctx.executeQueryAsync(function () {
                $scope.loaded = false;
                console.log("Comments -inserted")
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };
       
    }]);
})();