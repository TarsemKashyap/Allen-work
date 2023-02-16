"use strict";
(function () {
    app.controller("CRSRDashboardController", ["$scope", "CRSRService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
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
        $scope.isAdminGroup = false;
        $scope.CRSRRequestData = [];
        $scope.CRSRRequestDataAll = [];
        $scope.appProperties = ({ 
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
           
            ITProjectManagerMaster: ({ Id: "", EMail: "", Title: "" }),
           
            SystemOwnerMaster: ({ Id: "", EMail: "", Title: "" }),
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
            //load data
            $scope.getPending();
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['CRSRAdmin'];
                //determine wether current user is a admin of group(s) 
                var userGroups = result.data.d.Groups.results;
                var foundGroups = userGroups.filter(function (g) { return groupNames.indexOf(g.LoginName) > -1 });
                if (foundGroups.length > 0) {
                    $scope.isAdminGroup = true;
                }

            });
          //  bindUsersData();// utitlityrequest data
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
        //Load all siteAdmin data using querstring
        $scope.getAllData = function () {
            $scope.loaded = true;
            uService.getAllCRSRRequests().then(function (response) {             
                $scope.CRSRRequestDataAll = response.data.d.results; 
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search();
            });

        };
        $scope.IsMyPending = false;
        $scope.getAllRequstorData = function () {
            //call service
            $scope.loaded = true;
            $scope.IsMyPending = false;
            uService.getAllRequestorCRSRRequests(_spPageContextInfo.userId).then(function (response) {
                $scope.CRSRRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search();
            });
        };
        //pending results - while clickin then call service and bind data
      
        $scope.getPending = function () {
            $scope.loaded = true;
            $scope.IsMyPending = true;
            uService.getAllCRSRRequests().then(function (response) {
                $scope.CRSRRequestData.length = 0;
                angular.forEach(response.data.d.results, function (value, key) {

                    if ($scope.appProperties.CurrentUser.Id == value.BranchHead.Id && value.BranchHeadStatus == "Pending") {
                        $scope.CRSRRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.SystemOwner.Id && value.SystemOwnerStatus == "Pending") {
                        $scope.CRSRRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.ITProjectManager.Id && value.ITProjectManagerStatus == "Pending") {
                        $scope.CRSRRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.FundingAuthority.Id && value.FundingAuthorityStatus == "Pending") {
                        $scope.CRSRRequestData.push(value);
                    }
                });
                // functions have been describe process the data for display
                $scope.search();
                $scope.loaded = false;// while success loading false 
            });

        };
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.RequestTypeCode = "";
            $scope.appProperties.ApplicationID = "";
            $scope.itemsPerPage = "";
            $scope.search();
        }
        //Search...
        $scope.search = function (type, data) {
            if (data == undefined && type == undefined) {
               
                    data = $scope.CRSRRequestData;
                                
            }
            if (data == undefined && type == 3) {
                data = $scope.CRSRRequestData;
                var newArr = [];
                for (var i = 0; i < data.length; i++) {
                    newArr[i] = data[data.length - i - 1];
                }
                data = newArr;
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
            return Config.CRSRWFStatus[val]; // get the user name from it
        }
        $scope.ExportExcel = function () {
            $("#tblPJApprovalData").table2excel({
                filename: "CRSRServiceReqData.xls"
            });
        }
       
     
       
    }]);
})();