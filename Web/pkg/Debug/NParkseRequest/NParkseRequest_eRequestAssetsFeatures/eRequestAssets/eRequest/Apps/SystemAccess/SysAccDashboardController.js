"use strict";
(function () {
    app.controller("SysAccDashboardController", ["$scope", "SysAccService", "CommonService", "Config", "$location", "$window", "$filter", "NgTableParams", function ($scope, uService, commonService, Config, $location, $window, $filter, NgTableParams) {
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
        $scope.SysServiceRequestData = [];
        $scope.SysRequestDataAll = [];
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
        $scope.sort = function (keyname) {
            $scope.sortKey = keyname;   //set the sortKey to the param passed
            $scope.reverse = !$scope.reverse; //if true make it false and vice versa
            $scope.search();
        }
        $scope.init = function () {
            BindMasterData();
            bindUsersData();
            getACEModuleApprover();
          
            commonService.getCurrentUser().then(function (result) {
                $scope.appProperties.CurrentUser = result.data.d  
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];                   
                }
                     
               // $scope.getAllRequstorData(); //mySubmitted Request -inital load
                getAllRequests();
              
             
            });          
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['SystemAccessAdmin'];
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

        $scope.ACEModuleMasterList = [];
        $scope.IsACEModuleLeader = false;
        function getACEModuleApprover() {
            $scope.loaded = true;
            uService.getACEModuleMaster(Config.ACEModuleMaster).then(function (response) {
                $scope.loaded = false;
                if (response.data.d.results.length > 0) {
                    $scope.ACEModuleMasterList = response.data.d.results;
                    angular.forEach(response.data.d.results, function (value, key) {
                       
                        angular.forEach(value.ACEModuleLeader.results, function (value, key) {
                                if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                    $scope.IsACEModuleLeader = true;
                                }
                            });
                    });
                   
                }
                
            });
        };


        $scope.SystemAccessMasterList = [];

        function bindUsersData() {
            $scope.loaded = true;
            //$scope.IsACEmoduleleder = false;
            $scope.IsImplementationOfficer = false;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                       
                        $scope.SystemAccessMasterList = response.data.d.results;
                        //$scope.appProperties.ImplementationOfficerMaster = value.ImplementationOfficer.results;
                        if (value.Title == "Kris" || value.Title == "PALS" || value.Title == "Connect" || value.Title == "LicenceOne" || value.Title == "FileShare") {
                            angular.forEach(value.ImplementationOfficer.results, function (value, key) {
                                if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                    $scope.IsImplementationOfficer = true;
                                }                                
                            });
                        } 
                        
                    });
                }
            });
        };   

        function chkImplementationUsersData(type) {
            var istype = false;
            angular.forEach($scope.SystemAccessMasterList, function (value, key) {

                //$scope.appProperties.ImplementationOfficerMaster = value.ImplementationOfficer.results;
                if (value.Title == type) {
                    angular.forEach(value.ImplementationOfficer.results, function (value, key) {
                        if ($scope.appProperties.CurrentUser.Id == value.Id) {
                            istype = true;
                        }
                    });
                }
            });
            return istype
        }

        function chkACEmoduleleaderData(type) {
            var istype = false;
            angular.forEach($scope.ACEModuleMasterList, function (value, key) {

                //$scope.appProperties.ImplementationOfficerMaster = value.ImplementationOfficer.results;
                if (value.Title == type) {
                    angular.forEach(value.ACEModuleLeader.results, function (value, key) {
                        if ($scope.appProperties.CurrentUser.Id == value.Id) {
                            istype = true;
                        }
                    });
                }
            });
            return istype
        }


       
        function BindMasterData() { //inprogress//dpplname,id        
            uService.getAllReq(Config.WorkArea).then(function (response) {
                $scope.WorkAreaData = response.data.d.results;
                $scope.loaded = false;// while success loading false              
            });
            uService.getAllReq(Config.TypeOfRequest).then(function (response) {
                $scope.TypeOfReqData = response.data.d.results;
                $scope.loaded = false;// while success loading false              
            });
            uService.getAllReq(Config.SystemAccessMaster).then(function (response) {
                $scope.appProperties.RequestTypeMaster =  response.data.d.results;
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
        //clear
        $scope.celarsearch = function () {
            $scope.appProperties.ApplicationID = "";
                $scope.appProperties.ApplicationStatus = "";
            $scope.appProperties.ApplicationDate = "";
            $scope.appProperties.ApplicationEndDate = "";
            $scope.search();
        }
        //Search...
        $scope.search = function (type, data) {
            if (data == undefined && type == undefined) {
               
                    data = $scope.SysServiceRequestData;
                
            }
            if (data == undefined && type == 3) {
                data = $scope.SysServiceRequestData;
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
                $scope.filteredList = commonService.searchedSysReqEquiry(data, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = "";
            }
            else if ($scope.appProperties.RequestTypeCode) {
                $scope.searchText = $scope.appProperties.RequestTypeCode;
                $scope.filteredList = commonService.searchedSysReqEquiry(data, $scope.searchText, null, null);
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
                $scope.filteredList = commonService.searchedSysReqEquiry(data, "", fromDate, toDate);
                $scope.filteredItems = $scope.filteredList;
            }
            else if ($scope.searchText!="") {               
                $scope.filteredList = commonService.searchedSysReqEquiry(data, $scope.searchText, null, null);
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
        $scope.getStatus = function (item) {
            //return Config.SYSWFStatus[val]; // get the user name from it
            switch (item.WorkFlowStatus) {
                case "18":
                case "21":
                case "1": return "Pending with Branch Head";
                    break;
                case "2":
                    if (item.SystemAccess == "EVE") {
                        return "Pending with ModuleLeader";
                    }
                    else {
                        return "Pending with System Access Head";
                    }
                    break;
                case "3": return "Rejected by Branch head"; 
                    break;
                case "6":
                    if (item.SystemAccess == "EVE") {
                       
                        if (item.EVEModuleLeader.Title != undefined && item.EVEModuleLeaderStatus != "Approved") {
                            return "Pending with Module Leader"; 
                        }
                        else {
                            return "Pending with Module Owner";
                        }
                    }
                    else {
                        if (item.EVEModuleLeader != undefined && item.EVEModuleLeaderStatus == "Pending") {
                            //return "Module leaders";
                            return "Pending with Module Leader";

                        }
                        if (item.EVEModuleOwner != undefined && item.EVEModuleOwnerStatus == "Pending") {
                            //return item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';
                            return "Pending with Module Owner";
                        }

                    }
                    break;
                case "4":
                case "8": return "Approved";
                    break;
                default:
                    return Config.SYSWFStatus[item.WorkFlowStatus]; // get the user name from it
                    break;
            }

        }
      
        $scope.ExportExcel = function () {
            $("#ExportXL").table2excel({
                filename: "SystemAccessReport.xls"
            });
        }
       
        // Pass the `review` object as argument
        $scope.getWFStatus = function (item) {

            switch (item.WorkFlowStatus) {
                case "18":
                case "21":
                case "1": return item.BranchHead ? item.BranchHead.Title : '';
                    break;
                case "2":
                    if (item.SystemAccess == "EVE") {
                        return item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';
                    }
                    else {
                        return item.SystemAccessHead ? item.SystemAccessHead.Title : '';
                    }
                    break;
                case "3": return ""; //item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';
                    break;
                case "6":
                    if (item.SystemAccess == "EVE") {
                        //if (item.EVEModuleLeaderId != undefined ) {
                        //    //return "Module leaders";
                        //    return item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';

                        //}
                        if (item.EVEModuleLeader.Title != undefined && item.EVEModuleLeaderStatus != "Approved") {
                            return item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';
                        }
                        else {
                            return item.EVEModuleOwner ? item.EVEModuleOwner.Title : '';
                        }
                    }
                    else {
                        if (item.EVEModuleLeader != undefined && item.EVEModuleLeaderStatus == "Pending") {
                            //return "Module leaders";
                            return item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';

                        }
                        if (item.EVEModuleOwner != undefined && item.EVEModuleOwnerStatus == "Pending") {
                            //return item.EVEModuleLeader ? item.EVEModuleLeader.Title : '';
                            return item.EVEModuleOwner ? item.EVEModuleOwner.Title : '';
                        }
                        
                    }
                    break;
                case "4":
                case "8": return item.ImplementationOfficer ? item.ImplementationOfficer.Title : '';
                    break;
                default:
                    return Config.SYSWFStatus[item.WorkFlowStatus]; // get the user name from it
                    break;
            }
        }

        $scope.getAllRequstorData = function () {
            //call service
            $scope.IsMyPending = false;
            $scope.loaded = true;
            uService.getAllRequestorSysRequests (_spPageContextInfo.userId).then(function (response) {
                $scope.SysServiceRequestData = response.data.d.results;
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.search(1,$scope.SysServiceRequestData);
            });
        };
        function getAllRequests() {
            //call service
            $scope.loaded = true;
            uService.getAllSysRequests().then(function (response) {
                $scope.SysRequestDataAll = response.data.d.results;    //all data maintaine here
               // $scope.SysServiceRequestData = response.data.d.results; //serch data
                $scope.loaded = false;// while success loading false 
                // functions have been describe process the data for display
                $scope.getPending();
            });
        };
        //pending results - while clickin then call service and bind data
        $scope.getPending = function () {
            $scope.loaded = true;
            $scope.IsMyPending = true;
            uService.getAllSysRequests().then(function (response) {
                $scope.SysRequestDataAll = response.data.d.results;    //all data maintaine here
                // $scope.SysServiceRequestData = response.data.d.results; //serch data
                $scope.SysServiceRequestData.length = 0;
                angular.forEach(response.data.d.results, function (value, key) {

                    if ($scope.appProperties.CurrentUser.Id == value.BranchHead.Id && value.BranchHeadStatus == "Pending") {
                        $scope.SysServiceRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.SystemAccessHead.Id && value.SystemAccessHeadStatus == "Pending") {
                        $scope.SysServiceRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.ImplementationOfficer.Id && value.ImplementationOfficerStatus == "Pending" && value.SystemAccess == "EVE") {
                        $scope.SysServiceRequestData.push(value);
                    }
                    if ($scope.IsImplementationOfficer && value.ImplementationOfficerStatus == "Pending") {
                        if (chkImplementationUsersData(value.SystemAccess)) {
                            $scope.SysServiceRequestData.push(value);
                        }
                    }
                    if (value.EVEModuleLeaderStatus == "Pending") {
                        if (chkACEmoduleleaderData(value.AceMoudle)) {
                            $scope.SysServiceRequestData.push(value);
                        }
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.EVEModuleOwner.Id && value.EVEModuleOwnerStatus == "Pending") {
                        $scope.SysServiceRequestData.push(value);
                    }
                    if ($scope.appProperties.CurrentUser.Id == value.EVEModuleLeader.Id && value.EVEModuleLeaderStatus == "Pending") {
                        $scope.SysServiceRequestData.push(value);
                    }

                });
                // functions have been describe process the data for display
                $scope.search(2,$scope.SysServiceRequestData);
                $scope.loaded = false;// while success loading false 
            });
           
        };
       
    }]);
})();