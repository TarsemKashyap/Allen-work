"use strict";
(function () {
    app.controller("HReMRDashboardController", ["$scope", "HReMR", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
        //sharepoint context
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
        //init variable
        $scope.gap = 2;
        $scope.filteredItems = [];
        $scope.groupedItems = [];
        $scope.itemsPerPage = 10;
        $scope.pagedItems = [];
        $scope.currentPage = 0;
        $scope.HRRequestSubmittedData = [];
        $scope.HRRequestDataAll = [];
        $scope.WorkAreaData = [];
        $scope.TypeOfReqData = []; 
        $scope.appProperties = ({ 
            ManpowerRequestTypes: ({ ID: "", Title: "", TypeOfManpowerRequestID: "" }),
            ApplicationStatuss: ({ ID: "", Title: "" }),
            HeadofDeptName: "",            
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", DomainName: "" }),
            ManpowerRequestType: "",
            ManpowerType: "",
            Division: "",
            Branch: "",
            ApplicationStatus: "",
        });
        $scope.HRRequestData = "";
        $scope.searchText = '';
        // init
        $scope.sort = {
            sortingOrder: 'ID',
            reverse: false
        };
        $scope.init = function () {
            //BindMasterData();
          
            commonService.getCurrentUser().then(function (result) {
                $scope.appProperties.CurrentUser = result.data.d  
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];  
                    $scope.appProperties.CurrentUser.Id = result.data.d.Id;
                }
               
                getApprovers();
                getAllPendingHReMRRequests();
                //$scope.getAllRequstorSubmittedData(); //mySubmitted Request -inital load
               // getAllHReMRRequests($scope.appProperties.CurrentUser.Id);
                getManpowerRequestType();
                getAllDivisionAndBranch();
                //getApplicationStatus();
                //load data
               // bindUsersData();// data
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

        //getting search data
        $scope.getSearchData = function () {
            var fromDate = "";
            var toDate = "";
            if (($scope.appProperties.ApplicationID == "" || $scope.appProperties.ApplicationID == undefined) && ($scope.appProperties.ManpowerRequestType == "" || $scope.appProperties.ManpowerRequestType == undefined) && ($scope.appProperties.ApplicationStatus == "" || $scope.appProperties.ApplicationStatus == undefined) && ($scope.appProperties.ApplicationDate == "" || $scope.appProperties.ApplicationDate == undefined) && ($scope.appProperties.ApplicationEndDate == "" || $scope.appProperties.ApplicationEndDate == undefined)) {
                console.log("No filter selected");
            } else {
                if (($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) && ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null)) {
                    if ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null) {
                        var dateParts = $scope.appProperties.ApplicationDate.split("/");
                        // month is 0-based, that's why we need dataParts[1] - 1
                         fromDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                    }
                    if ($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) {
                        var dateParts = $scope.appProperties.ApplicationEndDate.split("/");
                        // month is 0-based, that's why we need dataParts[1] - 1
                         toDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                    }
                }
                uService.getSearchedHRRequests($scope.appProperties.ApplicationID, $scope.appProperties.ManpowerRequestType, $scope.appProperties.ApplicationStatus, fromDate, toDate).then(function (response) {
                    
                    $scope.filteredItems = response.data.d.results;
                    $scope.currentPage = 0;
                    // now group by pages
                    $scope.groupToPages();
                });
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
            if ($scope.IsmySubmit) {
                $scope.search(2, $scope.HRRequestSubmittedData);     
               
            }
             if ($scope.IsPending) {
                $scope.search(1, $scope.HRRequestDataAll);
            }
           
        }
        //Search...
        $scope.search = function (type, data) {
            if (data == undefined && type == undefined) {
                if ($scope.IsmySubmit) {
                    data = $scope.HRRequestSubmittedData;
                }
                else if ($scope.IsPending) {
                    data = $scope.HRRequestDataAll;
                }
            }
            if (data == undefined && type == 3) {
                if ($scope.IsmySubmit) {
                    data = $scope.HRRequestSubmittedData;
                }
                else if ($scope.IsPending) {
                    data = $scope.HRRequestDataAll;
                }
                var newArr = [];
                for (var i = 0; i < data.length; i++) {
                    newArr[i] = data[data.length - i - 1];
                }
                data = newArr;
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
            var status=""
            if (val == 1)
                status = "Pending";
            if (val == 2)
                status = "Completed";
            if (val == 3)
                status = "Rejected";
            if (val == 4)
                status = "Closed";
            if (val == 5)
                status = "Re-Routed";
            return status; //Config.CRSRWFStatus[val]; // get the user name from it
        }
      
        $scope.ExportExcel = function () {
            $("#tblPJApprovalData").table2excel({
                filename: "CRSRReqData.xls"
            });
        }
        //get Authorization from csutom list
        function getManpowerRequestType() {
            $scope.loaded = true; //spinner start -service call start
            uService.getManpowerRequestType().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ManpowerRequestTypes = response.data.d.results;
                }
            });
        };
        //Get all Branches and Divisions
        function getAllDivisionAndBranch() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllDivisionsAndBranches().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    var divisions = groupBy(response.data.d.results, 'Division');
                   
                    $scope.appProperties.Divisions = divisions;
                    var branches = groupBy(response.data.d.results, 'Branch');
                 
                    $scope.appProperties.Branches = branches;
                }
            });
        }
        //find unique values
        function groupBy(items, propertyName) {
            var temp = [];
            var result = [];
            $.each(items, function (index, item) {
                if (temp.indexOf(item[propertyName]) == -1) {
                    temp.push(item[propertyName]);
                    result.push({ Title: item[propertyName] });
                }
            });
            return result;
        }
        //getting application status
        //function getApplicationStatus() {
        //    $scope.loaded = true; //spinner start -service call start
        //    uService.getApplicationStatus().then(function (response) {
        //        $scope.loaded = false; //spinner stop - service call end 
        //        if (response.data.d.results.length > 0) {
        //            $scope.appProperties.ApplicationStatuss = response.data.d.results;
        //        }
        //    });
        //};

        // Pass the `review` object as argument
        $scope.getWFStatus = function (item) {
            var status = ""; 
            if ((item.ApplicationStatus == 1 || item.ApplicationStatus == 5) && (item.Level1Approver.Id != undefined && item.Level1ApprovalStatus == "Pending"))
                status = "Pending with " + item.Level1Approver.Title;
            else if (item.Level2ApprovalStatus == "Pending" && $scope.IsLevel2Approver)
                status = "Pending with " + $scope.appProperties.CurrentUser.Title;
            else if (item.Level2ApprovalStatus == "Approved" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id != undefined)
                status = "Approved"
            else if (item.Level1ApprovalStatus == "Rejected" && item.Level1Approver.Id != undefined && item.Level2Approver.Id == undefined && item.Level3Approver.Id == undefined)
                status = "Rejected by " + item.Level1Approver.Title;
            else if (item.Level2ApprovalStatus == "Rejected" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id == undefined)
                status = "Rejected by " + item.Level2Approver.Title;
            else if (item.Level3ApprovalStatus == "Rejected" && item.Level1Approver.Id != undefined && item.Level2Approver.Id != undefined && item.Level3Approver.Id != undefined)
                status = "Rejected by " + item.Level3Approver.Title;
            else if (item.Level2ApprovalStatus == "Pending" && item.Level1ApprovalStatus == "Approved")
                status = "Pending with Level2 Approver";
            else if ((item.ApplicationStatus == 5 ) && (item.Level1ApprovalStatus == null))
                status = "Pending with Requestor";
            return status;     
        }
        $scope.IsPending = false;
        $scope.IsmySubmit = false;
        $scope.getAllRequstorSubmittedData = function () {
            //call service
            $scope.loaded = true;
            $scope.IsmySubmit = true;
            $scope.IsPending = false;
            $scope.IsMyPending = false;
            uService.getAllRequestorSubmittedRequests(_spPageContextInfo.userId).then(function (response) {
                $scope.HRRequestSubmittedData = response.data.d.results;
                
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search(2,$scope.HRRequestSubmittedData);               
            });
        };
        function getAllHReMRRequests(userId) {
            if (userId == 1)
                userId = 3;
            //call service
            $scope.loaded = true;
            uService.getAllHReMRRequests(userId).then(function (response) {
                
                $scope.HRRequestDataAll = response.data.d.results; 
                
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search(1,$scope.HRRequestDataAll);
            });
        };
        //Getting approvers
        $scope.IsLevel2Approver = false;
        function getApprovers() {
            $scope.loaded = true; //spinner start -service call start
            uService.getApprovers().then(function (response) {

                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == "Level2Approver") {
                            angular.forEach(value.Approver.results, function (value, key) {
                                if (value.Id == $scope.appProperties.CurrentUser.Id) {
                                    $scope.IsLevel2Approver = true;
                                }
                            });
                        }
                    });
                }
            });
        };
    
        function getAllPendingHReMRRequests() {
            $scope.IsPending = true;
            $scope.IsmySubmit = false;
            //call service
            $scope.loaded = true;
            $scope.HRRequestDataAll.length = 0;
            uService.getAllPendingHReMRRequests().then(function (response) {
                angular.forEach(response.data.d.results, function (value, key) {

                    if ($scope.appProperties.CurrentUser.Id == value.Level1Approver.Id && value.Level1ApprovalStatus == "Pending") {
                        $scope.HRRequestDataAll.push(value);
                    }
                    if ($scope.IsLevel2Approver && value.Level2ApprovalStatus == "Pending") {
                        $scope.HRRequestDataAll.push(value);
                    }
                });
              
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search(1,$scope.HRRequestDataAll);
            });
        };
        
        //pending results - while clickin then call service and bind data
        $scope.getPending = function () {
            $scope.IsMyPending = true;
           /* $scope.CRSRRequestData.length = 0;
            angular.forEach($scope.CRSRRequestDataAll, function (value, key) {

                if ($scope.appProperties.CurrentUser.Id == value.Level1Approver.Id && value.Level1ApprovalStatus == "Pending") {
                    $scope.CRSRRequestData.push(value);
                }
                if ($scope.appProperties.CurrentUser.Id == value.Level2Approver.Id && value.Level2ApprovalStatus == "Pending") {
                    $scope.CRSRRequestData.push(value);
                }
                if ($scope.appProperties.CurrentUser.Id == value.Level3Approver.Id && value.Level3ApprovalStatus == "Pending") {
                    $scope.CRSRRequestData.push(value);
                }
                if ($scope.appProperties.CurrentUser.Id == value.Level4Approver.Id && value.Level4ApprovalStatus == "Pending") {
                    $scope.CRSRRequestData.push(value);
                }
                
            });*/
           // getAllHReMRRequests(userId);
            getAllPendingHReMRRequests();
            //$scope.search(0);//this one binding for all sitution
        };
       
       
      

      
      
       
    }]);
})();