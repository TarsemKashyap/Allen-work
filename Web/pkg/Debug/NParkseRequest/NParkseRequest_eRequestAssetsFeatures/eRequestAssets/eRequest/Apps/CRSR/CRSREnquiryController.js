"use strict";
(function () {
    app.controller("CRSRServiceEnquiryController", ["$scope", "CRSRService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
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
        $scope.CRSRRequestData = [];
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
            getAllProjectSystemMaster(); //get all Purpose;
            //load data
            $scope.getAllData();// utitlityrequest data
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
        //get getAllProjectSystemMaster data from CustomList
        function getAllProjectSystemMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllProjectSystemMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ProjectSystemMaster = response.data.d.results;
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
                $scope.search();
            });
        };
      
        $scope.search = function () {
            if ($scope.appProperties.ApplicationID || $scope.appProperties.ProjectSystem || $scope.appProperties.TypeofRequest || $scope.appProperties.ApplicationStatus) {

                if ($scope.appProperties.ApplicationID != "") {
                    $scope.searchText = $scope.appProperties.ApplicationID;
                }
                if ($scope.appProperties.TypeofRequest != "--Select--" && $scope.appProperties.TypeofRequest != undefined) {
                    $scope.searchText = $scope.appProperties.TypeofRequest;
                }
                if ($scope.appProperties.ProjectSystem != "--Select--" && $scope.appProperties.ProjectSystem != undefined) {
                    $scope.searchText = $scope.appProperties.ProjectSystem;
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
                $scope.filteredList = commonService.searchedCRSRReqEquiry($scope.CRSRRequestData, $scope.searchText, null, null);
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
                $scope.filteredList = commonService.searchedCRSRReqEquiry($scope.CRSRRequestData, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else {
                $scope.filteredItems = $scope.CRSRRequestData;
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
            $("#tblCRSRApprovalData").table2excel({
                filename: "CRSRApprovalData.xls"
            });
        }
    }]);
})();