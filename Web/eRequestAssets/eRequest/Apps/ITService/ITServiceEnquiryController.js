"use strict";
(function () {
    app.controller("ITServiceEnquiryController", ["$scope", "ITService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
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
        $scope.ITRequestData = [];
        $scope.appProperties = ({ 
             ApplicationID:"",
            ApplicationStatus: "",
            ApplicationDate: null,
            ApplicationEndDate: null,
                Created:"",         
            WorkAreaCode: "", // use this code to level of workflow
            RequestTypeCode: "", //use this code 
            WorkFlowCode: "",
            //WorkAreaMaster-List Properties
            WorkAreaMaster: ({ ID: "", Title: "", WorkAreaCode: "" }),
            //RequestTypeMaster-List Properties
            RequestTypeMaster: ({ ID: "", Title: "", WorkAreaCode: "", WorkFlowCode: "", RequestTypeCode: "" }), 
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
            //get all workarea
            getAllWorkAreaMaster(); 
            getAllRequestTypeMaster();
            //load data
            bindITRequestAllData();// request data

            //chk owners
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
        //get getAllWorkAreaMaster data from CustomList
        function getAllWorkAreaMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllWorkAreaMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.WorkAreaMaster = response.data.d.results;
                }
            });
        };
        function getAllRequestTypeMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllRequestTypeMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.RequestTypeMaster = response.data.d.results;
                }
            });
        };
        //Load all siteAdmin data using querstring
        function bindITRequestAllData() {
            $scope.loaded = true;
            uService.getAllITRequests().then(function (response) {
                $scope.ITRequestData = response.data.d.results;              
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search();
            });
        };
      //Search...
        $scope.search = function () {
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
                $scope.filteredList = commonService.searchedITReqEquiry($scope.ITRequestData, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText ="";
            }
            else if ($scope.appProperties.RequestTypeCode) {
                $scope.searchText = $scope.appProperties.RequestTypeCode;
                $scope.filteredList = commonService.searchedITReqEquiry($scope.ITRequestData, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = ""; //after search clear text
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
                $scope.filteredList = commonService.searchedITReqEquiry($scope.ITRequestData, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else {
                $scope.filteredItems = $scope.ITRequestData;
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
            return Config.ITWFStatus[val]; // get the user name from it
        }
        //excel
        $scope.ExportExcel = function () {
            $("#tblPJApprovalData").table2excel({
                filename: "PJApprovalData.xls"
            });
        }
       
    }]);
})();