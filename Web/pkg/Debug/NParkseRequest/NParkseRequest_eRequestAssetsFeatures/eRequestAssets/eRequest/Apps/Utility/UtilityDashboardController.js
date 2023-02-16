"use strict";
(function () {
    app.controller("UtilityDashboardController", ["$scope", "UtilityService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
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
        $scope.UtilityRequestData = [];     
        $scope.MyPendingRequestData = [];
        $scope.MySubmittedRequestData = [];
        $scope.appProperties = ({
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            FinanceHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            HeadofDeptName: "",
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
            commonService.getCurrentUser().then(function (result) {
                $scope.appProperties.CurrentUser = result.data.d;
            });
            getAllRequestTypeMaster();
            //load data
           // bindUsersData();// utitlityrequest data     
            $scope.getAllData();
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['UtilityAdmin'];
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
        $scope.getAll = function () {
            if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.BranchHeadMaster.Id) {
                getAllDeptHeadOfData("BranchHead");
                DisableHideShowControls(false, true, true);//requstor false, approver true
            }
            else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.FinanceHeadMaster[0].Id) {
                getAllDeptHeadOfData("FinanceHead");
                DisableHideShowControls(false, true, true); //requstor false, approver true
            }
            else {
                //get reuqstor all data
                getAllRequstorData();
                DisableHideShowControls(true, false, false); //requstor true, admin false
            }
        }
    
        //Load all siteAdmin data using querstring
        $scope.getAllData = function () {
            $scope.loaded = true;
            $scope.IsMyPending = true;
            uService.getAllUtilityRequests().then(function (response) {
                $scope.UtilityRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                angular.forEach($scope.UtilityRequestData, function (value, key) {
                    if ($scope.appProperties.CurrentUser.Id == value.Author.Id) {
                        $scope.MySubmittedRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.BranchHead.Id && value.BranchHeadStatus == "Pending") {
                        $scope.MyPendingRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.FinanceHead.Id && value.FinanceHeadStatus == "Pending") {
                        $scope.MyPendingRequestData.push(value);
                    }
                });
              
                $scope.search($scope.MyPendingRequestData); //using paging /search
            });

        };
        //pending results
        $scope.getOnGoing = function () {         
            if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.BranchHeadMaster.Id) {
                    onGoingData("BranchHead", "FinanceHeadStatus")  // here pass financeheadstatus             
                }
                else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.FinanceHeadMaster[0].Id) {                  
                    onGoingData("BranchHead", "FinanceHeadStatus")  // here pass financeheadstatus          
                }
                else {
                    onGoingData("BranchHead", "FinanceHeadStatus")  //pass current user- createdby 
                }
                  
        };
        //pending results
        $scope.IsMyPending = false;
        $scope.IsPending = false;
        $scope.IsmySubmit = false;
        $scope.getPending = function () {
            $scope.IsPending = true;
            $scope.IsmySubmit = false;
            $scope.IsMyPending = true;
            $scope.search(1,$scope.MyPendingRequestData); //loaded initially         
        };
        //mysubmit results
        $scope.getSubmited = function () {
            $scope.IsmySubmit = true;
            $scope.IsPending = false;
            $scope.IsMyPending = false;
            $scope.search(2,$scope.MySubmittedRequestData); //using paging /search   
        };
      
        //approved/rejected results-not equal to pending
        $scope.getClosed = function () {
            if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.BranchHeadMaster.Id) {
                    closedData("BranchHead")
                }
                else if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.FinanceHeadMaster[0].Id) {
                    closedData("FinanceHead")
                }
                else {
                    closedData("Author"); //pass current user- createdby 
                }
            
        };
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.RequestTypeCode = "";
            $scope.appProperties.ApplicationID = "";
            $scope.search();
        }
        //Search...
        $scope.search = function (type, data) {   
            if (data == undefined && type == undefined) {
                if ($scope.IsmySubmit) {
                    data = $scope.MySubmittedRequestData;
                }
                else if ($scope.IsPending) {
                    data = $scope.MyPendingRequestData;
                }
            }
            if (data == undefined && type == 3) {
                if ($scope.IsmySubmit) {
                    data = $scope.MySubmittedRequestData;
                }
                else if ($scope.IsPending) {
                    data = $scope.MyPendingRequestData;
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
                if ($scope.appProperties.ApplicationStatus == "Pending") {
                    $scope.searchText = "1";
                }
                if ($scope.appProperties.ApplicationStatus == "Approved") {
                    $scope.searchText = "3";
                }
                if ($scope.appProperties.ApplicationStatus == "Rejected") {
                    $scope.searchText = "4";
                }
                $scope.filteredList = commonService.searchedEquiry(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = "";
            }
            else if ($scope.appProperties.Purpose) {
                $scope.searchText = $scope.appProperties.Purpose;
                $scope.filteredList = commonService.searchedEquiry(data, $scope.searchText, null, null);
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
                $scope.filteredList = commonService.searchedEquiry(data, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else if ($scope.searchText != "") {
                $scope.filteredList = commonService.searchedEquiry(data, $scope.searchText, null, null);
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
        function getAllRequestTypeMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllRequestTypeMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.PurposeMaster = response.data.d.results;
                }
            });
        };
        // calculate page in place
        $scope.groupToPages = function () {
            $scope.pagedItems = [];
            if ($scope.filteredItems != undefined) {
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
            return Config.WFStatus[val]; // get the user name from it
        }
        $scope.ExportExcel = function () {
            $("#tblPJApprovalData").table2excel({
                filename: "PJApprovalData.xls"
            });
        }
        //get Authorization from csutom list
         
        function getAllRequstorData() {
          
            ////call service
            //$scope.loaded = true;
            //uService.getAllRequestorUtilityRequests(_spPageContextInfo.userId).then(function (response) {
            //    $scope.UtilityRequestData = response.data.d.results;
            //    $scope.loaded = false;// while success loading false 
            //    // functions have been describe process the data for display
            //    $scope.search($scope.UtilityRequestData);
            //});
        };
        function getAllDeptHeadOfData(HeadofDeptName) {
            //call service
            $scope.loaded = true;
            if (HeadofDeptName != '') {
                uService.getAllDeptHeadOfUtilityRequests(_spPageContextInfo.userId, HeadofDeptName).then(function (response) {
                    $scope.UtilityRequestData = response.data.d.results;
                    $scope.loaded = false;// while success loading false 
                    // functions have been describe process the data for display
                    $scope.search($scope.UtilityRequestData);
                });
            }
        };
        function onPendingData(HeadofDeptName,appStatus) {
            if (HeadofDeptName != '') {
                $scope.loaded = true;
                uService.getPendingUtilityRequests(appStatus, _spPageContextInfo.userId, HeadofDeptName).then(function (response) {
                    $scope.UtilityRequestData = response.data.d.results;
                    $scope.loaded = false;// while success loading false 
                    // functions have been describe process the data for display
                    $scope.search($scope.UtilityRequestData);
                });
            }
        }
        function onGoingData(HeadofDeptName, approverStatus) { //approverStatus-filedName here
            if (HeadofDeptName != '') {
                $scope.loaded = true;
                uService.getOngoingUtilityRequests("1", _spPageContextInfo.userId, HeadofDeptName, approverStatus).then(function (response) {
                    $scope.UtilityRequestData = response.data.d.results;
                    $scope.loaded = false;// while success loading false 
                    // functions have been describe process the data for display
                    $scope.search($scope.UtilityRequestData);
                });
            }
        }
        function closedData(HeadofDeptName) {
            if (HeadofDeptName != '') {
                $scope.loaded = true;
                uService.getClosedUtilityRequests("5", _spPageContextInfo.userId, HeadofDeptName).then(function (response) {
                    $scope.UtilityRequestData = response.data.d.results;
                    $scope.loaded = false;// while success loading false 
                    // functions have been describe process the data for display
                    $scope.search($scope.UtilityRequestData);
                });
            }
        }
        //disable-controls
        function DisableHideShowControls(isRequestor,IsApprover,IsAdmin) {
            //initally disble/show 
            $scope.IsPendingMyActionBtn = true; // disbale true       
              
            //saved mode
            if (isRequestor) {
                $scope.IsPendingMyActionBtn = false;              
            }
        };
       
    }]);
})();