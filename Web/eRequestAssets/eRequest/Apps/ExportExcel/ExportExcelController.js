"use strict";
(function () {
    app.controller("ExportExcelController", ["$scope", "ExportExcelService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, objService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
        //sharepoint context
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
        //init variable
        $scope.isSiteAdmin = false;
        //mail config
        var hostName = "";
        var portNumber = 587;
        var enableSSL = true;
        var password = "";
        var toEmails = ""; //using multiples email
        var fromEmail = "";
        var dispName = "";
        var bodyMsg = "";
        //Current User Properties
        $scope.CurrentUser = { Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", DomainName: "" };
        $scope.ListName = ""; 
        $scope.ListHardcodeName = ""; 
        $scope.ListNames = [];
        $scope.ListFields = [];
        // Let's omit some SharePoint internal list/library names
        var excludedLists = ["fpdatasources","Pages","Site Pages", "Site Collection Images","Resulable Content", "Master Page Gallery", "MicroFeed", "Site Assets", "Style Library", "Tasks", "TaxonomyHiddenList", "User Information List", "wfpub", "wfsvc", "Workflow History", "Workflow Tasks", "Workflows"];
        var excludedFieldNames = ["ID","Version", "Edit", "Type", "AppCreatedBy", "AppModifiedBy", "ContentType", "Attachments", "ItemChildCount", "FolderChildCount", "LinkTitle", "AppAuthor", "AppEditor", "Author", "Editor", "Created", "Modified", "LinkTitleNoMenu", "DocIcon", "_UIVersionString"];
        $scope.IsITComments = false;
        $scope.IsSystemAccessComments = false;
        $scope.IsITRequests = false;
       
        $scope.init = function () {
            $scope.loaded = true;
            // Get all lists and libraries from the site
            var lists = web.get_lists();
            ctx.load(lists);
            ctx.executeQueryAsync(
                Function.createDelegate(this, function () {
                    // Code in next step
                    var enumerator = lists.getEnumerator();
                    while (enumerator.moveNext()) {
                        var list = enumerator.get_current();

                        // If you want to only show document libraries
                        //if(list.get_baseType() == 1) // If you want to only show lists //if(list.get_baseType() == 0)

                        // We're going to exclude some OOTB SharePoint lists that we do not want to display
                        if (excludedLists.indexOf(list.get_title()) < 0) {
                            // Create an object to store the views collection for the list/library
                            var viewObj = {
                                Title: list.get_title(),
                                views: list.get_views()
                            };
                            // Add the object to an array to access later
                            $scope.ListNames.push(viewObj);                           
                        }
                    }
                    $scope.loaded = false; $scope.$apply();
                }),
                Function.createDelegate(this, function (sender, args) {
                    console.log(args.get_message());
                })
            );
            commonService.getCurrentUser().then(function (result) {
                $scope.CurrentUser = result.data.d;
                // chk is SiteAdmin  
                if (!result.data.d.IsSiteAdmin) {
                    $scope.isSiteAdmin = false;
                }
                else {
                    $scope.isSiteAdmin = true;
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
        $scope.ExportExcelData = [];
        $scope.ExportListData = [];    //hardcode list
        $scope.getListData = function (ListName) {
        //Load all siteAdmin data using querstring      
            if ($scope.ListName != "") {
                $scope.ExportExcelData.length = 0; $scope.ListFields.length = 0;
                $scope.loaded = true;
                objService.getAll(ListName).then(function (response) {
                    $scope.ExportExcelData = response.data.d.results;
                    objService.getAllFieldsFromList(ListName).then(function (response) {
                        angular.forEach(response.data.d.results, function (value, key) {
                            if (excludedFieldNames.indexOf(value.InternalName) < 0) {
                                $scope.ListFields.push({ "Title": value.Title}); 
                            }
                        });
                    });
                    $scope.loaded = false;// while success loading false              
                });
            }
        };

      

        $scope.getHardCodeListData = function (ListName) {
            //Load all siteAdmin data using querstring      
            if (ListName != "") {   
                //set false while change the lists
                $scope.IsITComments = false;
                $scope.IsSystemAccessComments = false;
                $scope.IsITRequests = false;

                $scope.ExportListData.length = 0;
                $scope.loaded = true;
                switch (ListName) {
                    case "ITComments":      
                        $scope.IsITComments = true;
                        break;                  
                    case "SystemAccessComments":  
                        $scope.IsSystemAccessComments = true;
                        break;
                    case "ITRequests":
                        $scope.IsITRequests = true;
                        break; 
                    default:
                    // code block
                }
                objService.getAll(ListName).then(function (response) {
                    $scope.ExportListData = response.data.d.results;             
                    $scope.loaded = false;// while success loading false              
                });

            }
        };
        $scope.ExportExcel = function () {
            if ($scope.ListHardcodeName != "") {
                $("#" + $scope.ListHardcodeName).table2excel({
                    filename: $scope.ListHardcodeName + "listData.xls"
                });
            }
        }
        //javascript methods---------------------------------------------------------------------------------------- end
   

    }]);
})();