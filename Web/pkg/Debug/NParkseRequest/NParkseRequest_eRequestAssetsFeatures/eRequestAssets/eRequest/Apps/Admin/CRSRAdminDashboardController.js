"use strict";
(function () {
    app.controller("CRSRAdminDashboardController", ["$scope", "CRSRAdminDashboardService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
        //sharepoint context
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
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
        var siteAbsoluteUrl = _spPageContextInfo.siteAbsoluteUrl;
        $scope.gap = 2;
        $scope.filteredItems = [];
        $scope.groupedItems = [];
        $scope.itemsPerPage = 10;
        $scope.pagedItems = [];
        $scope.currentPage = 0;
        $scope.CRSRRequestData = [];
        
        $scope.isAdminGroup = false;
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
            //List Properties-exact columnname on CRSR(save/edit) and additional properties for ui purpose
            CRSR: ({
                ID: "",
                Title: "",
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker
                BranchHead: ({ Id: "", EMail: "", Title: "" }),
                RequestedFor: ({ Id: "", EMail: "", Title: "" }),
                SystemOwner: ({ Id: "", EMail: "", Title: "" }),
                ITProjectManager: ({ Id: "", EMail: "", Title: "" }),
                FundingAuthority: ({ Id: "", EMail: "", Title: "" }),
                CIO: ({ Id: "", EMail: "", Title: "" }),
                //date
                SubmittedDate: null,
                BranchHeadDate: null,
                SystemOwnerDate: null,
                ITProjectManagerDate: null,
                FundingAuthorityDate: null,
                //FundingAuthorityDate: null,
                //status              
                BranchHeadStatus: "Pending",//set default
                SystemOwnerStatus: "",
                ITProjectManagerStatus: "",
                FundingAuthorityrStatus: "",
                //CIOStatus: "Pending",

                //others
                ApplicationID: "0",
                WorkFlowStatus: "", // use this code to level of workflow
                RequestTypeCode: "", //use this code 
                WorkFlowCode: "",
                IndicateAccountNo: "",


                Module: "",
                ExistingSituation: "",
                ProposedChanges: "",
                Justification: "",

                EstimatedEffort: "",
                EstimatedCost: "",
                TypeofRequest: "",
                ProjectSystem: "",
                FundCentre: "",
                CostCentre: "",

                UATRemarks: "",
                OtherReasonforRequest: "",
                OtherUATStatus: "",
                ProductionDataPatchRemarks: "",
                DateRequiredBy: null,
                ExpectedCompletionDate: null,
                ProductionDataPatchsignoff: null,
                ApplicationDate: $filter('date')(new Date(), 'dd/MM/yyyy'), //dd-MM-yyyy hh:mm a   
                ApplicationStatus: "New",
                DisplayStatus: "New", //UI purpose
                //-chekcbox list column 
                ReasonforRequest: ({ results: [] }),
                Chargeable: ({ results: [] }),
                ExpectedCompletionDatefor: ({ results: [] }),
                UATStatus: ({ results: [] })
            }),
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            DelegateApproverMaster: ({ Id: "", EMail: "", Title: "" }),
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "" }),
            ApplicationEndDate: "",
            ApplicationDate: ""
        });
        $scope.searchText = '';
        // init
        $scope.sort = {
            sortingOrder: 'ID',
            reverse: false
        };
        $scope.init = function () {
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
                $scope.appProperties.CurrentUser = result.data.d;
            });
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['CRSRAdmin'];
                //determine wether current user is a admin of group(s) 
                var userGroups = result.data.d.Groups.results;
                var foundGroups = userGroups.filter(function (g) { return groupNames.indexOf(g.LoginName) > -1 });
                if (foundGroups.length > 0) {
                    $scope.isAdminGroup = true;
                    //get admin data
                    bindAuthorization();
                    $scope.getAllData();
                }
                else {
                    alert('You do not have permission for this page');
                    window.Location.href = "/erequest/Pages/eRequestHome.aspx";
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
        //get Authorization from csutom list
        function bindAuthorization() {
            $scope.loaded = true;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == Config.Roles['DelegateApprover']) {
                            $scope.appProperties.DelegateApproverMaster = value.Approvers.results;
                        }
                    });
                }
            });
        };
        //Load all siteAdmin data using querstring
        $scope.getAllData = function () {
            $scope.loaded = true;
            uService.getAllCRSRRequests().then(function (response) {
                $scope.CRSRRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.CRSRRequestData);
            });

        };
        //pending results- Get all application inprogress status based on user
        $scope.getOnGoing = function () {
            onGoingData(2)  // InProgress
        };
        function onGoingData(appStaus) { //inprogress//dpplname,id        
            $scope.loaded = true;
            uService.getOngoingCRSRRequests(appStaus).then(function (response) {
                $scope.CRSRRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.CRSRRequestData);
            });
        }
        function closedData(approvedStaus, closedStaus) { //closed(4/5)//dpplname,id   
            $scope.loaded = true;
            uService.getClosedCRSRRequests(approvedStaus, closedStaus).then(function (response) {
                $scope.CRSRRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.CRSRRequestData);
            });
        }
        //approved/rejected results-not equal to pending
        $scope.getClosed = function () {
            closedData(3, 4);
        };
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.RequestTypeCode = "";
            $scope.appProperties.ApplicationID = "";
            $scope.itemsPerPage = "";
            $scope.searchText = "";
            $scope.search($scope.CRSRRequestData);
        }
        //Search...
        $scope.search = function (data) {
            if (data == undefined) {

                data = $scope.CRSRRequestData;

            }           
            if (($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) && ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null)) {
                $scope.appProperties.ApplicationStatus = "";
                $scope.appProperties.RequestTypeCode = "";
                $scope.appProperties.ApplicationID = "";
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
                $scope.filteredList = commonService.searchedCRSRReqEquiry(data, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else if ($scope.appProperties.ApplicationID || $scope.appProperties.ApplicationStatus) {

                if ($scope.appProperties.ApplicationID != "") {
                    $scope.searchText = $scope.appProperties.ApplicationID;
                }
                if ($scope.appProperties.ApplicationStatus == "Pending") {
                    $scope.searchText = "1";
                }
                if ($scope.appProperties.ApplicationStatus == "Approved") {
                    $scope.searchText = "3";
                }
                if ($scope.appProperties.ApplicationStatus == "Rejected") {
                    $scope.searchText = "4";
                }
                $scope.filteredList = commonService.searchedCRSRReqEquiry(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = "";
            }
            else if ($scope.appProperties.RequestTypeCode) {
                $scope.searchText = $scope.appProperties.RequestTypeCode;
                $scope.filteredList = commonService.searchedCRSRReqEquiry(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = ""; //after search clear text
            }

            else if ($scope.searchText != "") {
                $scope.filteredList = commonService.searchedCRSRReqEquiry(data, $scope.searchText, null, null);
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
        $scope.searchold = function () {
            if ($scope.searchText == '') {
                $scope.filteredItems = $scope.CRSRRequestData;
            }
            else {
                $scope.filteredItems = commonService.searchedCRSRAdmnin($scope.CRSRRequestData, $scope.searchText);
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
            if ($scope.filteredItems.length > 10) {
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
            return Config.CRSRWFAdminStatus[val]; // get the user name from it
        }
        // Pass the `review` object as argument
        $scope.getWFStatus = function (item) {

            switch (item.WorkFlowStatus) {
               
                case "1": return item.BranchHead ? item.BranchHead.Title : '';
                    break; 
                case "2": return item.ITProjectManager ? item.ITProjectManager.Title : '';
                    break;
                case "4": return item.SystemOwner ? item.SystemOwner.Title : '';
                    break;
                case "6": return item.FundingAuthority ? item.FundingAuthority.Title : '';
                    break;
                default:
                    return Config.SYSWFStatus[item.WorkFlowStatus]; // get the user name from it
                    break;
            }
        }
        $scope.ExportExcel = function () {
            $("#ExportXL").table2excel({
                filename: "CRSRReport.xls"
            });
        }
        //javascript methods---------------------------------------------------------------------------------------- end
        //Show Model windows
        //insert/update CRSR 
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
        $scope.ListItemId = 0;
        $('#my_modal').on('show.bs.modal', function (e) {
            var listItemId = $(e.relatedTarget).data('id');
            $scope.ListItemId = $(e.relatedTarget).data('id');
            // $(e.currentTarget).find('input[name="ListItemId"]').val(listItemId);
        });
        //assing parent value to child        
        $scope.submitCRSRServiceRequest = function (type, $event, Form) {
            if ($scope.appChildProperties.DelegateUser.Id != "" && $scope.appChildProperties.DelegateUser.Id != null) {
                $scope.loaded = true;
                //  var itemId = $('#ListItemId').val();

                var currentlistItem = $filter('filter')($scope.CRSRRequestData, { Id: $scope.ListItemId })[0];
                if (currentlistItem != undefined) {
                    $('.modal-backdrop').remove();
                    $('#my_modal').modal('hide');

                    var ppobject = SPClientPeoplePicker.SPClientPeoplePickerDict["peoplePickerDelegateUser_TopSpan"];
                    ppobject.DeleteProcessedUser()
                    $('#resolvedUsers').html("");
                    $('#userKeys').html("");
                    $('#userProfileProperties').html("");
                    $('#userID').html("");

                    insertUpdateListItem(currentlistItem, 10, 10, "Re-Delegate successfully"); //10-Re-Delegate Approver, 10-Re-Delegate Approver
                }

            }
            else {
                alert('Please select delegate user..!');
            }

        };
        function insertUpdateListItem(currentlistItem, Worfkflowstatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.CRSR + "");
            var listItem = "";
            listItem = list.getItemById(currentlistItem.Id);
            //date -approve/rejecte date assing in disabled function
            //Branch head  approval
            if ($scope.appChildProperties.DelegateUser.Id) {
                if (currentlistItem.BranchHeadStatus == "Pending") {
                    listItem.set_item("BranchHead", $scope.appChildProperties.DelegateUser.Id);
                }
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.ITProjectManagerStatus == "Pending") {
                    listItem.set_item("ITProjectManager", $scope.appChildProperties.DelegateUser.Id);
                }
                //SystemOwner approval-open after ITProjectManager approval
                else if (currentlistItem.SystemOwnerStatus == "Pending") {
                    listItem.set_item("SystemOwner", $scope.appChildProperties.DelegateUser.Id);
                }
                //funding approval-open after ITProjectManager approval
                else if (currentlistItem.FundingAuthorityStatus == "Pending") {
                    listItem.set_item("FundingAuthority", $scope.appChildProperties.DelegateUser.Id);
                }
                else { //dosome 
                }
                //workflow general status 
                listItem.set_item("WorkFlowStatus", Worfkflowstatus);
                listItem.update();
                ctx.load(listItem);
            }
            ctx.executeQueryAsync(function () {
                insertComments(currentlistItem.Id, commentStatus);
                //send email to DelegateUser/user- Submitted for admin
                if ($scope.appChildProperties.DelegateUser.EMail) {
                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appChildProperties.DelegateUser.EMail, "", "", "CRSR Application Submitted by " + currentlistItem.Author.Title + "", "This is to notify you that CRSR Application " + currentlistItem.ApplicationID + " from " + currentlistItem.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                }
                $timeout(function () {
                  
                    alert(message); 
                    $scope.loaded = false;
                    window.location.href = "/erequest/Pages/CRSRAdminDashboard.aspx";
                }, 3000);
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };
        //insert comments
        function insertComments(listItemId, status) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.CRSRComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            clistItem.set_item('UserComments', Config.CRSRCommentStatus[status] + " (System Comments)");
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('CRSRListItemID', listItemId);
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
        $scope.showModal = function (listItemId) {

            $uibModal.open({
                templateUrl: 'myModal.html',
                controller: 'ModalDialogController',
                resolve: {
                    Id: function () { return listItemId; },
                    items: function () {
                        return $scope.appProperties;
                    },
                }
            })
                .result.then(
                    function () {
                        //alert("OK");
                    },
                    function () {
                        // alert("Cancel");
                    }
                );
        };
        //child controller - call ShowModel finction in parent controller
    }]);
})();