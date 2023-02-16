"use strict";
(function () {
    app.controller("UtilityEnquiryController", ["$scope", "UtilityService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
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
        $scope.appProperties = ({ 
                ApplicationID:"",
            ApplicationStatus: "",
            ApplicationDate: null,
            ApplicationEndDate: null,
                Created:"",
                PremisesAddress:"",
                Purpose:"",
                Vendor:"",
            //PurposeMaster-List Properties
            PurposeMaster: ({ ID: "", Title: "" }),
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
            getAllPurposeMaster(); //get all Purpose;
            //load data
            bindUsersData();// utitlityrequest data
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
            //commonService.chkIsMemberOfGroup("" + Config.AdminOwnersGroup + "", _spPageContextInfo.userId).then(function (resultdata) {
            //    if (resultdata.data.d.results[0] != undefined) {
            //        //User is a Member, do something or return tru
            //        $scope.isShowAllRequest = true;    
                    
            //    } else {
            //        //chk and laod data
            //        $scope.isShowAllRequest = false; 
            //    }                
            //});
           
        };
        //get purposeMaster data from CustomList
        function getAllPurposeMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllPurposeMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.PurposeMaster = response.data.d.results;
                }
            });
        };
        $scope.getAll = function () {          
            if ($scope.appProperties.BranchHeadMaster.length > 0 || $scope.appProperties.FinanceHeadMaster.length > 0) {
                if ($scope.appProperties.CurrentUser.Id == $scope.appProperties.BranchHeadMaster[0].Id) {
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
        }        
        //Load all siteAdmin data using querstring
        $scope.getAllData = function () {
            $scope.loaded = true;
            uService.getAllUtilityRequests().then(function (response) {
                $scope.UtilityRequestData = response.data.d.results;              
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search();
            });

        };
      
        $scope.search = function () {
            if ($scope.appProperties.ApplicationID || $scope.appProperties.Purpose || $scope.appProperties.PremisesAddress || $scope.appProperties.ApplicationStatus) {

                if ($scope.appProperties.ApplicationID != "") {
                    $scope.searchText = $scope.appProperties.ApplicationID;
                }
                if ($scope.appProperties.Purpose != "--Select--" && $scope.appProperties.Purpose != undefined) {
                    $scope.searchText = $scope.appProperties.Purpose;
                }
                if ($scope.appProperties.PremisesAddress != "") {
                    $scope.searchText = $scope.appProperties.PremisesAddress;
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
                $scope.filteredList = commonService.searchedEquiry($scope.UtilityRequestData, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText ="";
            }
            else if (($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) && ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null) ) {
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
                $scope.filteredList = commonService.searchedEquiry($scope.UtilityRequestData, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else {
                $scope.filteredItems = $scope.UtilityRequestData;
            }
            $scope.currentPage = 0;
            // now group by pages
            $scope.groupToPages();

        }
        // calculate page in place
        $scope.groupToPages = function () {
            $scope.pagedItems = [];
            for (var i = 0; i < $scope.filteredItems.length; i++) {
                if (i % $scope.itemsPerPage === 0) {
                    $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)] = [$scope.filteredItems[i]];
                } else {
                    $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)].push($scope.filteredItems[i]);
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
        function bindUsersData() {
            $scope.loaded = true;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == Config.Roles['BranchHead']) {
                            $scope.appProperties.BranchHeadMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['FinanceHead']) {
                            $scope.appProperties.FinanceHeadMaster = value.Approvers.results;
                        }
                    });
                    $scope.getAll(); //get deptHead data
                }
            });
        };    
        function getAllRequstorData() {
            //call service
            $scope.loaded = true;
            uService.getAllRequestorUtilityRequests(_spPageContextInfo.userId).then(function (response) {
                $scope.UtilityRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search();
            });
        };
        function getAllDeptHeadOfData(HeadofDeptName) {
            //call service
            $scope.loaded = true;
            if (HeadofDeptName != '') {
                uService.getAllDeptHeadOfUtilityRequests(_spPageContextInfo.userId, HeadofDeptName).then(function (response) {
                    $scope.UtilityRequestData = response.data.d.results;
                    $scope.loaded = false;// while success loading false 
                    // functions have been describe process the data for display
                    $scope.search();
                });
            }
        };
       
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