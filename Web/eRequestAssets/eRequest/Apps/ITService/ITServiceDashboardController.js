"use strict";
(function () {
    app.controller("ITServiceDashboardController", ["$scope", "ITService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
        //sharepoint context
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
        //init variable
        $scope.IsITProjectManager = false; //init
        $scope.IsImplementationOfficer = false; //init        
        $scope.gap = 2;
        $scope.filteredItems = [];
        $scope.groupedItems = [];
        $scope.itemsPerPage = 10;
        $scope.pagedItems = [];
        $scope.currentPage = 0;
        $scope.ITServiceRequestData = [];
        $scope.ITServiceRequestDataAll = [];
        $scope.WorkAreaData = [];
        $scope.TypeOfReqData = []; 
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
          
            commonService.getCurrentUser().then(function (result) {
                $scope.appProperties.CurrentUser = result.data.d  
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];                   
                }
                     
              
                getAllITRequests();
                getAllRequestTypeMaster();
                //load data
                bindUsersData();// data
                //  $scope.getAllRequstorData(); //mySubmitted Request -inital load
              
            });          
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['ITAdmin'];
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
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.RequestTypeCode = "";
            $scope.appProperties.ApplicationID = "";
            $scope.search();
        }
        //Search...
        $scope.search = function (type,data) {
            if (data == undefined && type == undefined) {
             
                    data = $scope.ITServiceRequestData;                
              
            }
            if (data == undefined && type == 3) {
                data = $scope.ITServiceRequestData;
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
            else if ($scope.searchText!="") {               
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
            return Config.ITWFStatus[val]; // get the user name from it
        }
      
        $scope.ExportExcel = function () {
            $("#tblPJApprovalData").table2excel({
                filename: "ITServiceReqData.xls"
            });
        }
        //get Authorization from csutom list
        function getAllRequestTypeMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllRequestTypeMaster(Config.WorkArea).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.RequestTypeMaster = response.data.d.results;
                }
            });
        };
        // Pass the `review` object as argument
        $scope.getWFStatus = function (item) {

            switch (item.WorkFlowStatus) {
                case "18": 
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

        $scope.getAllRequstorData = function () {
            //call service
            $scope.loaded = true;
            $scope.IsMyPending = false;
            uService.getAllRequestorITRequests(_spPageContextInfo.userId).then(function (response) {
                $scope.ITServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search();
            });
        };
        function getAllITRequests() {
            //call service
            $scope.loaded = true;
            uService.getAllITRequests().then(function (response) {
                $scope.ITServiceRequestDataAll = response.data.d.results;              
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.getPending();//pending Request -inital load
            });
        };
        //pending results - while clickin then call service and bind data
        $scope.getPending = function () {
            $scope.IsMyPending = true;
            $scope.ITServiceRequestData.length = 0;
            angular.forEach($scope.ITServiceRequestDataAll, function (value, key) {

                if ($scope.appProperties.CurrentUser.Id == value.BranchHead.Id && value.BranchHeadStatus == "Pending") {
                    $scope.ITServiceRequestData.push(value);
                }
                if ($scope.IsITProjectManager && value.ITProjectManagerStatus == "Pending") {
                    $scope.ITServiceRequestData.push(value);
                }
                if ($scope.appProperties.CurrentUser.Id == value.GroupDirector.Id && value.GroupDirectorStatus == "Pending") {
                    $scope.ITServiceRequestData.push(value);
                }
                if ($scope.appProperties.CurrentUser.Id == value.CIO.Id && value.CIOStatus == "Pending") {
                    $scope.ITServiceRequestData.push(value);
                }
                if ($scope.IsImplementationOfficer && value.ImplementationOfficerStatus == "Pending") {
                    $scope.ITServiceRequestData.push(value);
                }
            });

            $scope.search();//this one binding for all sitution
        };
       
       
        function bindUsersData() {
            $scope.loaded = true;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == Config.Roles['BranchHead']) {
                            //$scope.appProperties.BranchHeadMaster = value.Approvers.results; // change req -get from starff direct
                        }
                        if (value.Title == Config.Roles['ITProjectManager']) {
                            $scope.appProperties.ITProjectManagerMaster = value.Approvers.results;
                            angular.forEach(value.Approvers.results, function (value, key) {
                                if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                    $scope.IsITProjectManager = true;
                                }
                            });
                        }
                        if (value.Title == Config.Roles['ImplementationOfficer']) {
                            $scope.appProperties.ImplementationOfficerMaster = value.Approvers.results;
                            angular.forEach(value.Approvers.results, function (value, key) {
                                if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                    $scope.IsImplementationOfficer = true;
                                }
                            });
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

      
       
      
       
    }]);
})();