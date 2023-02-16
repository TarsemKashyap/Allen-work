"use strict";
(function () {
    app.controller("ITAdminDashboardController", ["$scope", "ITAdminDashboardService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
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
        $scope.ITServiceRequestData = [];   
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
            //List Properties-exact columnname on ITServiceRequests(save/edit) and additional properties for ui purpose
            ITServiceRequests: ({
                ID: "",
                Title: "",
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker
                BranchHead: ({ Id: "", EMail: "", Title: "" }),
                RequestedFor: ({ Id: "", EMail: "", Title: "" }),
                ImplementationOfficer: ({ Id: "", EMail: "", Title: "" }),
                ITProjectManager: ({ Id: "", EMail: "", Title: "" }),
                GroupDirector: ({ Id: "", EMail: "", Title: "" }),
                CIO: ({ Id: "", EMail: "", Title: "" }),
                InstallUpgradeSWHostName: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                RemovalSWHostName: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                //date
                SubmittedDate: null,
                BranchHeadDate: null,
                ImplementationOfficerDate: null,
                ITProjectManagerDate: null,
                GroupDirectorDate: null,
                ImplemeCIODatentationOfficerDate: null,
                //status              
                BranchHeadStatus: "Pending",//set default
                ImplementationOfficerStatus: "",
                ITProjectManagerStatus: "",
                GroupDirectorStatus: "",
                CIOStatus: "",

                //others
                ApplicationID: "0",
                WorkAreaCode: "", // use this code to level of workflow
                RequestTypeCode: "", //use this code 
                WorkFlowCode: "",
                IndicateAccountNo: "",
                RemarksReasons: "",
                OffRenoFundDetail: "",
                OffRenoLocationAddress: "",
                InstallUpgradeSWLink: "",
                HWPremIntranetNewADAcc: "",
                HWLoanIntranetNewADAcc: "",
                DateRequiredBy: null,
                OffRenoProjectStartDate: null,
                OffRenoProjectEndDate: null,
                ApplicationDate: $filter('date')(new Date(), 'dd/MM/yyyy'), //dd-MM-yyyy hh:mm a   
                ApplicationStatus: "New",
                WorkFlowStatus: "0",
                DisplayStatus: "New", //UI purpose
                //-chekcbox list column                
                LoginAccountType: ({ results: [] }),
                OffRenoServices: ({ results: [] }),
                InstallUpgradeSW: ({ results: [] }),
                EmailServices: ({ results: [] })
            }),
            //WorkAreaMaster-List Properties
            WorkAreaMaster: ({ ID: "", Title: "", WorkAreaCode: "" }),
            //RequestTypeMaster-List Properties
            RequestTypeMaster: ({ ID: "", Title: "", WorkAreaCode: "", WorkFlowCode: "", RequestTypeCode: "" }),
            //Approvers
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            ImplementationOfficerMaster: ({ Id: "", EMail: "", Title: "" }),
            ITProjectManagerMaster: ({ Id: "", EMail: "", Title: "" }),
            GroupDirectorMaster: ({ Id: "", EMail: "", Title: "" }),
            CIOMaster: ({ Id: "", EMail: "", Title: "" }),
            DelegateApproverMaster: ({ Id: "", EMail: "", Title: "" }),
            //appComments-List Properties
            appComments: ({
                ID: "",
                Title: "",
                Comments: "",
                ListItemID: "",
                Author: ({ Id: "", EMail: "", Title: "" }),
            }),
            appFiles: [],
            appFileProperties: [],
            appInstallUpgradeSWDocumentsFiles: [],
            appFileInstallUpgradeSWDocumentsProperties: [],
            appFloorPlanFiles: [],
            appFileFloorPlanProperties: [],
            appChecklistFiles: [],
            appFileChecklistProperties: [],
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "" }),
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
                $scope.appProperties.CurrentUser = result.data.d;
            });
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['ITAdmin'];
                //determine wether current user is a admin of group(s) 
                var userGroups = result.data.d.Groups.results;
                var foundGroups = userGroups.filter(function (g) { return groupNames.indexOf(g.LoginName) > -1 });
                if (foundGroups.length > 0) {
                    //get admin data
                    bindAuthorization();                  
                    $scope.getAllData();
                   
                    $scope.isAdminGroup = true;
                }
                else {
                    $scope.isAdminGroup = false;        
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

        $scope.ExportExcel = function () {
            $("#ExportXL").table2excel({
                filename: "ITSRReport.xls"
            });
        }

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
        function BindMasterData() { //inprogress//dpplname,id        
            uService.getAll(Config.WorkArea).then(function (response) {
                $scope.WorkAreaData = response.data.d.results;
                $scope.loaded = false;// while success loading false              
            });
            uService.getAll(Config.TypeOfRequest).then(function (response) {
                $scope.TypeOfReqData = response.data.d.results;
                $scope.loaded = false;// while success loading false              
            });
        }
        //Load all siteAdmin data using querstring
        $scope.getAllData = function () {
            $scope.loaded = true;         
           
            // functions have been describe process the data for display
            uService.getAllITRequests().then(function (response) {
                $scope.ITServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.ITServiceRequestData);
            });
        };
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.RequestTypeCode = "";
            $scope.appProperties.ApplicationID = "";
            $scope.searchText = "";
            $scope.search($scope.ITServiceRequestData);
        }
        //Search...
        $scope.search = function (data) {
         
            if ($scope.appProperties.ApplicationID || $scope.appProperties.ApplicationStatus) {

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
                $scope.filteredList = commonService.searchedITReqEquiry(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = "";
            }
            else if ($scope.appProperties.RequestTypeCode) {
                $scope.searchText = $scope.appProperties.RequestTypeCode;
                $scope.filteredList = commonService.searchedITReqEquiry(data, $scope.searchText, null, null);
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
                $scope.filteredList = commonService.searchedITReqEquiry(data, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else if ($scope.searchText != "") {
                $scope.filteredList = commonService.searchedITReqEquiry(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
            }
            else {
                if ($scope.itemsPerPage == "") {
                    $scope.itemsPerPage = data.length;
                    $scope.filteredItems = data;
                }
                else {
                    $scope.filteredItems = data;
                }
            }
            $scope.currentPage = 0;
            // now group by pages
            $scope.groupToPages();
        }
        $scope.searchold = function () {          
            if ($scope.searchText == '') {
                $scope.filteredItems = $scope.ITServiceRequestData;
            }
            else {
                $scope.filteredItems = commonService.searchedITAdmnin($scope.ITServiceRequestData, $scope.searchText);              
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
        $scope.getAll = function () {
            if ($scope.appProperties.BranchHeadMaster.length > 0 || $scope.appProperties.ImplementationOfficerMaster.length > 0
                || $scope.appProperties.ImplementationOfficerMaster.length > 0
                || $scope.appProperties.GroupDirectorMaster.length > 0
                || $scope.appProperties.CIOMaster.length > 0
                || $scope.appProperties.ITProjectManagerMaster.length > 0) {
                if ($scope.appProperties.CurrentUser.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = $scope.appProperties.CurrentUser.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = $scope.appProperties.CurrentUser.LoginName.split("|")[1].split("\\")[0]
                    getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                }
                else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.ImplementationOfficerMaster[0].Id) {
                    getAllDeptHeadOfData("ImplementationOfficer"); //pass field name
                    DisableHideShowControls(false, true, true); //requstor false, approver true
                }
                else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.ITProjectManagerMaster[0].Id) {
                    getAllDeptHeadOfData("ITProjectManager"); //pass field name
                    DisableHideShowControls(false, true, true); //requstor false, approver true
                }
                else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.GroupDirectorMaster[0].Id) {
                    getAllDeptHeadOfData("GroupDirector"); //pass field name
                    DisableHideShowControls(false, true, true); //requstor false, approver true
                }
                else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.CIOMaster[0].Id) {
                    getAllDeptHeadOfData("CIO"); //pass field name
                    DisableHideShowControls(false, true, true); //requstor false, approver true
                }
                else {
                    //get reuqstor all data
                    getAllData();
                    DisableHideShowControls(true, false, false); //requstor true, admin false
                }
            }
            else {
                //get reuqstor all data
                getAllData();
                DisableHideShowControls(true, false, false); //requstor true, admin false
            }
        }       
        //pending results- Get all application inprogress status based on user
        $scope.getOnGoing = function () {
                   onGoingData(2)  // InProgress
        };
        function onGoingData(appStaus) { //inprogress//dpplname,id        
            $scope.loaded = true;
            uService.getOngoingITRequestsWithAppStatus(appStaus).then(function (response) {
                $scope.ITServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.ITServiceRequestData);
            });
        }
        function closedData(approvedStaus, closedStaus) { //closed(4/5)//dpplname,id   
            $scope.loaded = true;
            uService.getClosedITRequestsWithAppStatus(approvedStaus, closedStaus).then(function (response) {
                $scope.ITServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search($scope.ITServiceRequestData);
            });
        }      
        //approved/rejected results-not equal to pending
        $scope.getClosed = function () {
            closedData(3, 4);
        };
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
            return Config.ITAdminStatus[val]; // get the user name from it
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
        // Pass the `review` object as argument
        $scope.getWFStatus = function (item) {

            switch (item.WorkFlowStatus) {
                case "21":
                case "1": return item.BranchHead.Title;
                    break;
                case "2": return item.ITProjectManager.Title;
                    break;
                case "4": return item.GroupDirector.Title;
                    break;
                case "6": return item.ImplementationOfficer.Title;
                    break;
                case "8": return item.CIO.Title;
                    break;
                default:
                    return Config.ITWFStatus[item.WorkFlowStatus]; // get the user name from it
                    break;
            }
        }
      
        //$scope.ExportExcel = function () {
        //    $("#tblPJApprovalData").table2excel({
        //        filename: "ITServiceReqData.xls"
        //    });
        //}
        var listItemId = 0;
        $('#my_modal').on('show.bs.modal', function (e) {
            listItemId = $(e.relatedTarget).data('id');
            $(e.currentTarget).find('input[name="ListItemId"]').val(listItemId);
        });
        //assing parent value to child        
        $scope.submitITServiceRequest = function (type, $event, Form) {
            if ($scope.appChildProperties.DelegateUser.Id != "" && $scope.appChildProperties.DelegateUser.Id != null) {
                $scope.loaded = true;
               // var itemId = $('#ListItemId').val();
                //if (Form.ddlDelegateUser.$invalid) {
                //    if (Form.ddlDelegateUser.$invalid) Form.ddlDelegateUser.$touched = true;
                //    return;
                //}
              //  else {
                var currentlistItem = $filter('filter')($scope.ITServiceRequestData, { Id: listItemId })[0]; //getting this id from modlepopeu while click event
                    if (currentlistItem != undefined) {
                        $('.modal-backdrop').remove();
                        $('#my_modal').modal('hide');

                      
                        insertUpdateListItem(currentlistItem, 21, 21, "Re-Delegate successfully"); //10-Re-Delegate Approver, 10-Re-Delegate Approver

                        //clear
                        var ppobject = SPClientPeoplePicker.SPClientPeoplePickerDict["peoplePickerDelegateUser_TopSpan"];
                        ppobject.DeleteProcessedUser()
                        $('#resolvedUsers').html("");
                        $('#userKeys').html("");
                        $('#userProfileProperties').html("");
                        $('#userID').html("");

                    }
               // }
            }
            else {
                alert('Please select delegate user..!');
            }

        };
        //insert/update IT 
        function insertUpdateListItem(currentlistItem, Worfkflowstatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.ITRequests + "");
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
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.GroupDirectorStatus == "Pending") {
                    listItem.set_item("GroupDirector", $scope.appChildProperties.DelegateUser.Id);
                }
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.ImplementationOfficerStatus == "Pending") {
                    listItem.set_item("ImplementationOfficer", $scope.appChildProperties.DelegateUser.Id);
                }
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.CIOStatus == "Pending") {
                    listItem.set_item("CIO", $scope.appChildProperties.DelegateUser.Id);
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
                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appChildProperties.DelegateUser.EMail, "", "", "IT service Request Submitted by " + currentlistItem.Author.Title + "", "This is to notify you that IT service Request " + currentlistItem.ApplicationID + " from " + currentlistItem.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                }
                $timeout(function () {
                    $scope.loaded = false;
                    alert(message);
                    $scope.getAllData();
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
            var clist = web.get_lists().getByTitle("" + Config.ITComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            clistItem.set_item('UserComments', Config.ITCommentStatus[status] + " (System Comments)");
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('ITListItemID', listItemId);
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
        //javascript methods---------------------------------------------------------------------------------------- end
        //Show Model windows
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
    }]).controller("ModalDialogController", function ($scope, $uibModalInstance, $timeout, Config, Id, items) {
        //init
        //sharepoint context        
        ExecuteOrDelayUntilScriptLoaded(function () {

            var scriptbase = _spPageContextInfo.siteAbsoluteUrl + "/_layouts/15/";

            // Load the js files and continue to the successHandler
            $.getScript(scriptbase + "SP.RequestExecutor.js", function () {
                console.log("request executor is now loaded");
                // Logic here
            });
            $.getScript(scriptbase + "sp.init.js", function () {
                console.log("init executor is now loaded");
                // Logic here
            });
            $.getScript(scriptbase + "SP.ui.dialog.js", function () {
                console.log("dialog executor is now loaded");
                // Logic here
            });
            $.getScript(scriptbase + "SP.ui.dialog.js", function () {
                console.log("dialog executor is now loaded");
                // Logic here
            });
            $.getScript(scriptbase + "ScriptResx.ashx?name=sp.res&culture=en-us", function () {
                console.log("ScriptResx executor is now loaded");
                // Logic here
            });
            $.getScript(scriptbase + "SP.clientpeoplepicker.js", function () {
                console.log("request executor is now loaded");
                // Logic here
            });
        }, "sp.js");
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
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

        $scope.appChildProperties.DelegateManagerMaster = items.DelegateApproverMaster;//getting from parent

        //Utility.helpers.initializePeoplePicker('peoplePickerDiv');

        //assing parent value to child        
        $scope.submitITServiceRequest = function (type, $event, Form) {       
            var Id = $(this).data('id');
            if (Form.ddlDelegateUser.$invalid) {
                if (Form.ddlDelegateUser.$invalid) Form.ddlDelegateUser.$touched = true;
                return;
            }
            else {
                insertUpdateListItem(Id, 21, 21, "Re-Delegate successfully"); //10-Re-Delegate Approver, 10-Re-Delegate Approver
            }

        };

        $scope.cancelModal = function () {
            $uibModalInstance.dismiss('cancel');
        };

        //insert/update IT 
        function insertUpdateListItem(currentlistItem, Worfkflowstatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.ITRequests + "");
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
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.GroupDirectorStatus == "Pending") {
                    listItem.set_item("GroupDirector", $scope.appChildProperties.DelegateUser.Id);
                } 
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.ImplementationOfficerStatus == "Pending") {
                    listItem.set_item("ImplementationOfficer", $scope.appChildProperties.DelegateUser.Id);
                } 
                //ITProjectManager approval-open after branch head approved
                else if (currentlistItem.CIOStatus == "Pending") {
                    listItem.set_item("CIO", $scope.appChildProperties.DelegateUser.Id);
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
                $timeout(function () {
                    $scope.loaded = true;
                    alert(message);
                    $uibModalInstance.close();
                    window.location.href = "/erequest/Pages/ITAdminDashboard.aspx";
                }, 3000);
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };
        //insert comments
        function insertComments(listItemId, status) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.ITComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            clistItem.set_item('UserComments', Config.ITCommentStatus[status] + " (System Comments)");
            clistItem.set_item('CommentsBy', items.CurrentUser.Id);
            clistItem.set_item('ITListItemID', listItemId);
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
    });
})();