"use strict";
(function () {
    app.controller("UtilityServiceRequestController", ["$scope", "UtilityService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
        //sharepoint context
        var siteAbsoluteUrl = _spPageContextInfo.siteAbsoluteUrl;
        var urlTemplate = siteAbsoluteUrl + "/_api/SP.Utilities.Utility.SendEmail";
        var ctx = new SP.ClientContext.get_current();
        var web = ctx.get_web();
        var today = new Date();
        $scope.CurrentDateTime = $filter('date')(new Date(), 'dd-MM-yyyy hh:mm a');
        var dateAsString = $filter("date")(today, "dd-MMM-yyyy");
        //mail config
        var hostName = "";
        var portNumber = 25;

        var enableSSL = true;
        var password = "";

        var fromEmail = "";
        var dispName = "";
        var bodyMsg = "";
         //end config
        //init variable
        var isSiteAdmin = false;
        $scope.IsLoginUserDivisonHead = false; //init
        $scope.IsLoginUserBranchHead = false; //init
        $scope.IsOpenNewAccount = false;
        $scope.IsChkUtilitiesReuire = false;
        var ccEmailUsers = "";
        //check required validation
        $scope.appProperties = [];
        $scope.appProperties = ({
            //List Properties-exact columnname on UtilityRequests(save/edit) and additional properties for ui purpose
            UtilityRequests: ({
                ID: "",
                Title: "",               
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker
                BranchHead: ({ Id: "", EMail: "", Title: "" }),
                RequestedFor: ({ Id: "", EMail: "", Title: "" }),
                FinanceHead: ({ Id: "", EMail: "", Title: "" }),               
                //date
                SubmittedDate: null,               
                BranchHeadDate: null,
                FinanceHeadDate: null,              
                //status              
                BranchHeadStatus: "Pending",//set default
                FinanceHeadStatus: "Pending",               
                //others
                BillingDescription: "",
                GLCode: "",
                FundCentre:"",
                ApplicationID: "0",
                Purpose: "",
                IndicateAccountNo: "",
                RemarksReasons: "",
                CostCentre: "",               
                Vendor: "",
                PremisesAddress: "",
                WaterVendor: "",
                ElectricityVendor:"",
                SavingAdderExtPriceTotal: "",
                ApplicationDate: $filter('date')(new Date(), 'dd/MM/yyyy'), //dd-MM-yyyy hh:mm a                
                SubmittedFormatDate: $filter('date')(new Date(), 'dd/MM/yyyy'),
                ApplicationStatus: "New",
                WorkFlowStatus:"",
                DisplayStatus: "New", //UI purpose
                //-chekcbox list column
                UtilitiesApplyingFor: ({ results: [] })
            }),            
            //VendorMaster-List Properties
            VendorMaster: ({ ID: "", Title: "" }),
            UtilitiesVendor1: [],
            UtilitiesVendor2: [],
            UtilitiesVendor3: [],
            //CostCentreMaster-List Properties
            CostCentreMaster: ({ ID: "", Title: "", CID:"" }),
            //PurposeMaster-List Properties
            PurposeMaster: ({ ID: "", Title: "" }),
            //currencyMaster-List Properties
            CurrencyMaster: ({ ID: "", Title: "" }),
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            FinanceHeadMaster: ({ Id: "", EMail: "", Title: "" }),         
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
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", ADID:""}),
        });
        //PJInfo -bind checkbox using repeater
        $scope.CheckboxInformation = ["Electricity", "Water"];
        // Selected PJInformation- push selected info in this varaible
        $scope.SelectionCheckboxInformation = [];
        $scope.DomainName = "";
//Angular methods ----------------------------------------------------------------------------------start-while using html then create scope variable or method
        //init start
        $scope.init = function (init) {
            Utility.helpers.initializePeoplePicker('peoplePickerRequestedFor');   
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerRequestedFor_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];             
                if (userData !== undefined) {                  
                    $('#userID').val(userData.Key.split('\\')[1]);
                    $scope.appProperties.UtilityRequests.RequestedFor.LoginName = userData.Key.split('\\')[1];
                    // Get the first user's ID by using the login name.
                    getUserId(userData.Key).done(function (user) {
                        $scope.appProperties.UtilityRequests.RequestedFor.Id = user.d.Id;
                    });      
                }
            };
            //load function
            getCurrentLoginUsersAndBindAllData(); //get current usser - After success call loading data               
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
            uService.getAllUsersFromGroup("UtilityAdmin").then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        ccEmailUsers += ","+value.Email;
                    });                  
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

        }; //init end   
        //submit request
        $scope.IsRequire = false;
        $scope.IsRequire1 = false;
        $scope.submitListItem = function (type, $event, Form) {
            $scope.loaded = true; //automate false while redirect page
            var isValidPage = true; //automate false while redirect page
            $event.preventDefault();
            var appStatus = "0"; //maintain applciation field
            var cmtStatus = "0"; //maintain coments field
            //init validation false 
            switch (type) {
                case 1:
                    if ($scope.SelectionCheckboxInformation.length == 0) {
                        $scope.IsChkUtilitiesReuire = true;
                        $scope.loaded = false;
                        isValidPage = false;
                    }
                    else {
                        $scope.IsChkUtilitiesReuire = false;
                        angular.forEach($scope.SelectionCheckboxInformation, function (value, key) {
                            if (value == "Electricity") {
                                $scope.IsRequire = true;
                            }
                            else {
                                $scope.IsRequire = false;
                            }
                            if (value == "Water") {
                                $scope.IsRequire1 = true;
                            }
                            else {
                                $scope.IsRequire1 = false;
                            }
                        });
                        if ($scope.IsRequire == true) {
                            if ($scope.appProperties.UtilityRequests.ElectricityVendor == "") {
                                $("#VendorName1").focus();
                                Form.VendorName1.$touched = true;
                                $scope.loaded = false;
                                isValidPage = false;
                            }

                        }
                        if ($scope.IsRequire1 == true) {
                            if ($scope.appProperties.UtilityRequests.WaterVendor == "") {
                                $("#VendorName2").focus();
                                Form.VendorName2.$touched = true;
                                $scope.loaded = false;
                                isValidPage = false;
                            }

                        }
                    }
                    if (Form.$invalid && type != 0) {
                        $scope.loaded = false;
                        if (Form.Purpose.$invalid) {
                            $("#Purpose").focus();
                        }
                        else if (Form.CostCentreName.$invalid) {
                            $("#CostCentreName").focus();
                        }
                       
                        else if (Form.PremisesAddress.$invalid) {
                            $("#PremisesAddress").focus();
                        }
                        else if (Form.RemarksReasons.$invalid) {
                            $("#RemarksReasons").focus();
                        }                        
                        if ($scope.IsOpenNewAccount == true) {
                           
                                                    
                            if (Form.FundCentre.$invalid) {
                                $("#FundCentre").focus();                              
                            }
                            if (Form.BillingDescription.$invalid) {
                                $("#BillingDescription").focus();
                            }                        
                          
                            Form.FundCentre.$touched = true;
                            Form.BillingDescription.$touched = true; 
                        }                        
                        Form.Purpose.$touched = true; //while submit neet to set true this field, to required msg will be dispaly
                        Form.CostCentreName.$touched = true;
                        Form.PremisesAddress.$touched = true;
                        Form.RemarksReasons.$touched = true;  
                         
                        isValidPage = false;
                       
                    }
                    if ($scope.IsAccountNo == true) {
                        $("#AccountNo").focus();
                        Form.AccountNo.$touched = true;

                    }
                    if ($scope.IsOpenNewAccount == true) {
                        if ($scope.appProperties.UtilityRequests.GLCode != "") {
                            if ($scope.appProperties.UtilityRequests.GLCode.length < 7 || $scope.appProperties.UtilityRequests.GLCode.length > 7) {
                                alert("GL Code is strictly 7 digits");
                                $scope.loaded = false;
                                isValidPage = false;
                            }
                        }
                        if ($scope.appProperties.UtilityRequests.FundCentre != "") {
                            if ($scope.appProperties.UtilityRequests.FundCentre.length < 8 || $scope.appProperties.UtilityRequests.FundCentre.length > 8) {
                                alert("Fund Centre is strictly 8 digits");
                                $scope.loaded = false;
                                isValidPage = false;
                            }
                        }
                    }

                    if (isValidPage == false) {
                        return;
                    }
                    else {

                        $scope.appProperties.UtilityRequests.BranchHeadStatus = "Pending";
                        $scope.appProperties.UtilityRequests.FinanceHeadStatus = "";
                        type = 1 // submitted
                        appStatus = 1; //inprogress
                        cmtStatus = 1;// submitted by user
                        insertUpdateListItem(type, appStatus, cmtStatus, "Submitted Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus      
                    }
                    // code block
                    break;
                case 2:
                    //date -approve/rejecte date assing in disabled function
                    //Branch head  approval
                    var cms = "Approved Successfully";                    
                    if ($scope.appProperties.UtilityRequests.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.UtilityRequests.BranchHead.Id) {
                        //enable only BranchHead-approve/rejectbutton
                        $scope.appProperties.UtilityRequests.BranchHeadDate = new Date();
                        $scope.appProperties.UtilityRequests.FinanceHeadStatus = "Pending";
                        $scope.appProperties.UtilityRequests.BranchHeadStatus = "Approved";
                        type = 2 // Approved by BranchHead
                        appStatus = 2; //inprogress
                        cmtStatus = 2;// Approved by BranchHead
                        insertUpdateListItem(type, appStatus, cmtStatus, cms); //Worfkflowstatus, ApplicationStatus,commentStatus 

                    }
                    //finance head  approval-open after branch head approved
                    else if ($scope.appProperties.UtilityRequests.BranchHeadStatus == "Approved"
                        && $scope.appProperties.UtilityRequests.ApplicationStatus == "2"
                        && $scope.appProperties.CurrentUser.Id == $scope.appProperties.UtilityRequests.FinanceHead.Id) {
                       
                        $scope.appProperties.UtilityRequests.FinanceHeadDate = new Date();
                        $scope.appProperties.UtilityRequests.FinanceHeadStatus = "Approved";
                        type = 3// Approved by FinanceHead
                        appStatus = 3; //all approved
                        cmtStatus = 3;// Approved by FinanceHead
                        insertUpdateListItem(type, appStatus, cmtStatus, cms); //Worfkflowstatus, ApplicationStatus,commentStatus 

                    }
                    //finance head  Branch head approved approved
                    else if ($scope.appProperties.UtilityRequests.BranchHeadStatus == "Approved" && $scope.appProperties.UtilityRequests.FinanceHeadStatus == "Approved"
                        && ($scope.appProperties.CurrentUser.Id == $scope.appProperties.UtilityRequests.Author.Id || $scope.appProperties.CurrentUser.Id == $scope.appProperties.UtilityRequests.FinanceHead.Id)) {
                        if ($scope.appProperties.appFiles.length == 0) {
                            $scope.loaded = false;
                            alert("Please attach support document !!");                           
                            return;
                        }
                        $scope.appProperties.UtilityRequests.ClosedDate = new Date();
                        $scope.appProperties.UtilityRequests.ClosedStatus = "Closed";
                        type = 6; // Closed by user
                        appStatus = 5; //closed                        
                        cmtStatus = 6;// Closed by user    

                        insertUpdateListItem(type, appStatus, cmtStatus, "Closed Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus 
                    }
                    // Approved block
                    break;
                case 3: //Rejected section
                    //Branch head  approval
                    if ($scope.appProperties.appComments.Comments != "" && $scope.appProperties.appComments.Comments != undefined) {
                        if ($scope.appProperties.UtilityRequests.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.UtilityRequests.BranchHead.Id) {
                            //enable only BranchHead-approve/rejectbutton
                                $scope.appProperties.UtilityRequests.BranchHeadDate = new Date();
                                $scope.appProperties.UtilityRequests.BranchHeadStatus = "Rejected";
                                type = 4; // Rejected level-1
                                appStatus = 4; //Rejected -Appstatus
                                cmtStatus = 4;// Rejected by BranchHead
                            insertUpdateListItem(type, appStatus, cmtStatus, "Rejected Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                        }
                        else if ($scope.appProperties.UtilityRequests.BranchHeadStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.UtilityRequests.FinanceHead.Id) {
                            //enable only BranchHead-approve/rejectbutton
                                $scope.appProperties.UtilityRequests.FinanceHeadDate = new Date();
                                $scope.appProperties.UtilityRequests.FinanceHeadStatus = "Rejected";
                                type = 5; // Rejected level-2
                                appStatus = 4; //Rejected -Appstatus
                                cmtStatus = 5;// Rejected by FinanceHead     
                            insertUpdateListItem(type, appStatus, cmtStatus, "Rejected Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                        }
                        
                    }
                    else {
                        $scope.loaded = false;
                        alert("Please enter rejected comments !!");
                        $("#Comments").focus();
                        return;
                    }
                    // Rejected block
                    break;
                case 0:
                    //Close
                    window.location.href = "/erequest/Pages/UtilityDashboard.aspx";
                    // code block                  
                    break;
                default:
                // code block
            }
        };         // Toggle selection for a given pjInformation by name
       
        $scope.toggleCheckboxInfoSelection = function toggleCheckboxInfoSelection(information) {
            var idx = $scope.SelectionCheckboxInformation.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionCheckboxInformation.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionCheckboxInformation.push(information);
               
            }
            //clear
            $scope.appProperties.UtilitiesVendor1.length = 0;
            $scope.appProperties.UtilitiesVendor2.length = 0;
            $scope.appProperties.UtilitiesVendor3.length = 0;
            angular.forEach($scope.SelectionCheckboxInformation, function (value, key) {
                if (value == "Electricity") {
                    var arryList = $filter('filter')($scope.appProperties.VendorMaster, { Utilities: value });
                    if (arryList.length > 0) {
                        $scope.appProperties.UtilitiesVendor1 = arryList;
                    }
                }
                if (value == "Water") {
                    var arryList = $filter('filter')($scope.appProperties.VendorMaster, { Utilities: value });
                    if (arryList.length > 0) {
                        $scope.appProperties.UtilitiesVendor2 = arryList;
                    }
                }                
            });
        };
        //initiall
        $scope.IsAccountNo = false;
        $scope.IsAccountNoDisabled = true;
        $scope.CheckIsNewAccountNo = function (Form) {
            if ($scope.appProperties.UtilityRequests.Purpose == "Open New Account") {
                $scope.IsOpenNewAccount = true;
                $scope.IsAccountNo = false;
                $scope.IsAccountNoDisabled = true;
                $scope.appProperties.UtilityRequests.IndicateAccountNo = "";
            }
           else if ($scope.appProperties.UtilityRequests.Purpose == "Terminate Existing Account" || $scope.appProperties.UtilityRequests.Purpose == "Transfer of Account") {

                $scope.IsAccountNoDisabled = false;
                $scope.IsAccountNo = true;
                if (Form.AccountNo.$invalid) {
                    $("#AccountNo").focus();
                    Form.AccountNo.$touched = true;
                }

            }
            else {
                $scope.IsOpenNewAccount = false;
                $scope.IsAccountNo = false;
                $scope.IsAccountNoDisabled = true;
            }
        }
       //not in use
        $scope.CheckIndicateAccountNo = function (Form){          
            if ($scope.appProperties.UtilityRequests.Purpose != "--Select--") {
                if ($scope.appProperties.UtilityRequests.Purpose == "Terminate Existing Account" || $scope.appProperties.UtilityRequests.Purpose == "Transfer of Account"){
                  
                    $scope.IsAccountNoDisabled = false;
                    $scope.IsAccountNo = true;
                    if (Form.AccountNo.$invalid) {
                        $("#AccountNo").focus();
                        Form.AccountNo.$touched = true;
                    }
                   
                }
                else {
                    $scope.IsAccountNo = false;
                    $scope.IsAccountNoDisabled = true;
                }
            }
        }
        // Check checkbox checked or not
        $scope.checkVal = function () {
            if ($scope.OnBehalf) {
                $scope.appProperties.UtilityRequests.OnBehalf = "1";
                $scope.OnBehalf = true;
            } else {
                $scope.OnBehalf = false;
                $scope.appProperties.UtilityRequests.OnBehalf = "0";
            }
            $scope.$apply();
        }
      
//Angular methods --------------------------------------------------------------------------------------end


        //javascript methods ----------------------------------------------------------------------------------start

        //get login users details
        function getCurrentLoginUsersAndBindAllData() {
            $scope.loaded = true; //spinner start -service call start
            commonService.getCurrentUser().then(function (result) {
                $scope.loaded = false; //spinner stop - service call end 
                $scope.appProperties.CurrentUser = result.data.d;
                $scope.DomainName = result.data.d.LoginName.split('\\')[0];
                $scope.appProperties.CurrentUser.LoginName = result.data.d.LoginName.split('\\')[1];              
                $scope.appProperties.UtilityRequests.Author.LoginName = result.data.d.Title; //while init                
                $scope.appProperties.UtilityRequests.Author.Title = result.data.d.Title; //while init 
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1], // getting login name without domain   
                        $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];
                        getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                }
                else {     
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName;//result.data.d.Title.split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.Title.split("\\")[0];
                    getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                }
                getDivisionHeadByGroupDir($scope.appProperties.CurrentUser.ADID); //get group dir from DivisionHeads list 
                //all time load function   
                bindAuthorization(); // 
                getAllVendorMaster(); //get all Vendor
                getAllPurposeMaster(); //get all Purpose
                getAllCostCentreMaster(); //get all CostCentre
                chkLoginUserIsBrachHeadorDivHead($scope.appProperties.CurrentUser.ADID); //chk user bracnhhead or divison head
                //get query string value
                $scope.appProperties.UtilityRequests.ID = Utility.helpers.getUrlParameter('ReqId');
                if ($scope.appProperties.UtilityRequests.ID != "" && $scope.appProperties.UtilityRequests.ID != undefined) {
                    loadUtilityRequestsListData($scope.appProperties.UtilityRequests.ID);//Load data     
                    loadFilesDocument($scope.appProperties.UtilityRequests.ID); //PriceComparison 
                    loadCommentsListData($scope.appProperties.UtilityRequests.ID); //PriceComparison  
                }
                else {
                    //new req
                    getUniqueNumber(); //generate unique number-only for new Req   
                    DisableHideShowControls("New", 0) //init stage
                }
                // chk is SiteAdmin  
                if (!result.data.d.IsSiteAdmin) {
                    isSiteAdmin = false;
                }
                else {
                    isSiteAdmin = true;
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
        //insert/update UtilityRequests 
        function insertUpdateListItem(Worfkflowstatus, ApplicationStatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.UtilityRequests + "");
            var listItem = "";
            if ($scope.appProperties.UtilityRequests.ID == "" || $scope.appProperties.UtilityRequests.ID == undefined) {
                //insert data to sharepoint
                var listCreationInformation = new SP.ListItemCreationInformation();
                listItem = list.addItem(listCreationInformation);
                listItem.set_item("RequestorLoginID", $scope.appProperties.CurrentUser.LoginName);
            } else {

                listItem = list.getItemById($scope.appProperties.UtilityRequests.ID);
            }
            //assign properties while insert/update
            listItem.set_item("Title", $scope.appProperties.UtilityRequests.ApplicationID);
            listItem.set_item("ApplicationID", $scope.appProperties.UtilityRequests.ApplicationID);
            listItem.set_item("Purpose", $scope.appProperties.UtilityRequests.Purpose);
            listItem.set_item("IndicateAccountNo", $scope.appProperties.UtilityRequests.IndicateAccountNo);
            listItem.set_item("RemarksReasons", $scope.appProperties.UtilityRequests.RemarksReasons);
            listItem.set_item("CostCentre", $scope.appProperties.UtilityRequests.CostCentre);
            listItem.set_item("PremisesAddress", $scope.appProperties.UtilityRequests.PremisesAddress);
            listItem.set_item("Vendor", $scope.appProperties.UtilityRequests.Vendor);
            listItem.set_item("FundCentre", $scope.appProperties.UtilityRequests.FundCentre);
            listItem.set_item("GLCode", $scope.appProperties.UtilityRequests.GLCode);
            listItem.set_item("BillingDescription", $scope.appProperties.UtilityRequests.BillingDescription);
            listItem.set_item("ElectricityVendor", $scope.appProperties.UtilityRequests.ElectricityVendor);
            listItem.set_item("WaterVendor", $scope.appProperties.UtilityRequests.WaterVendor);
            //checkbox information
            listItem.set_item("UtilitiesApplyingFor", $scope.SelectionCheckboxInformation);  //-chekcbox         
            listItem.set_item("OnBehalf", $scope.appProperties.UtilityRequests.OnBehalf);
            //all people picker field check value 
            if ($scope.appProperties.UtilityRequests.RequestedFor != undefined) {
                if ($scope.appProperties.UtilityRequests.RequestedFor.Id) {
                    listItem.set_item("RequestedFor", $scope.appProperties.UtilityRequests.RequestedFor.Id);
                    listItem.set_item("RequestedForLoginID", $scope.appProperties.UtilityRequests.RequestedFor.LoginName);
                }
            }
            //all manager Name-people picker 
            if ($scope.appProperties.UtilityRequests.BranchHead.Id) { //query string load
                listItem.set_item("BranchHead", $scope.appProperties.UtilityRequests.BranchHead.Id);
            }
            else {
                if ($scope.appProperties.BranchHeadMaster.Id) { //initial load
                    listItem.set_item("BranchHead", $scope.appProperties.BranchHeadMaster.Id);
                    $scope.appProperties.UtilityRequests.BranchHead = $scope.appProperties.BranchHeadMaster;
                }
            }
            if ($scope.appProperties.UtilityRequests.FinanceHead.Id) {
                listItem.set_item("FinanceHead", $scope.appProperties.UtilityRequests.FinanceHead.Id);
            }
            else {
                if ($scope.appProperties.FinanceHeadMaster.Id > 0) {
                    listItem.set_item("FinanceHead", $scope.appProperties.FinanceHeadMaster.Id);
                    $scope.appProperties.UtilityRequests.FinanceHead = $scope.appProperties.FinanceHeadMaster.Id;
                }
            }
            //status
            listItem.set_item("BranchHeadStatus", $scope.appProperties.UtilityRequests.BranchHeadStatus);
            listItem.set_item("FinanceHeadStatus", $scope.appProperties.UtilityRequests.FinanceHeadStatus);
            listItem.set_item("BranchHeadDate", $scope.appProperties.UtilityRequests.BranchHeadDate);
            listItem.set_item("FinanceHeadDate", $scope.appProperties.UtilityRequests.FinanceHeadDate);
            listItem.set_item("ClosedDate", $scope.appProperties.UtilityRequests.ClosedDate);
            listItem.set_item("ClosedStatus", $scope.appProperties.UtilityRequests.ClosedStatus);

            //workflow general status     
            listItem.set_item("ApplicationStatus", ApplicationStatus);
            listItem.set_item("WorkFlowStatus", Worfkflowstatus);

            listItem.update();
            ctx.load(listItem);
            ctx.executeQueryAsync(function () {
                try {
                    var redirectUrl = "/erequest/Pages/UtilityDashboard.aspx";
                    insertComments(listItem.get_id(), commentStatus);
                    if ($scope.appProperties.appFiles.length > 0) {
                        insertSupportingDocuments(listItem.get_id(), message, redirectUrl);
                    }
                    else {
                        //
                    }

                    //sendEmail to approver                 
                    switch (Worfkflowstatus) {
                        case 1:
                            //send email to branchHead/user- Submitted for approval
                            if ($scope.appProperties.UtilityRequests.BranchHead.Id) {
                                // alert("Send aprpoval Emil to : " + $scope.appProperties.UtilityRequests.BranchHead.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "New eFaciliates Application Submitted for approval", "This is to notify you that a NEW/CHANGE IN EFACILIATES application <Application No> from <Staff Name> has been submitted.click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/UtilityService.aspx?=" + $scope.appProperties.UtilityRequests.ID + "'>View</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "eFaciliates request Submitted " + $scope.appProperties.UtilityRequests.Author.Title + "", "This is to notify you that a eFaciliates Request " + $scope.appProperties.UtilityRequests.ApplicationID + " from " + $scope.appProperties.UtilityRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/UtilityDashboard.aspx>here</a>");
                                //Utility.helpers.sendEmail($scope.appProperties.BranchHead.EMail, "New eFaciliates Application Submitted for your approval", "This is to notify you that a NEW/CHANGE IN EFACILIATES application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/UtilityService.aspx?=" + $scope.appProperties.UtilityRequests.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.UtilityRequests.BranchHead.EMail, "", "", "eFaciliates request Submitted " + $scope.appProperties.UtilityRequests.Author.Title + "", "This is to notify you that a eFaciliates Request " + $scope.appProperties.UtilityRequests.ApplicationID + " from " + $scope.appProperties.UtilityRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/UtilityDashboard.aspx>here</a>");
                            }
                            break;
                        case 2:
                        case 3:
                            //send email to finace head/user- After Approved by BranchHead
                            if ($scope.appProperties.UtilityRequests.FinanceHead.Id && Worfkflowstatus == '2') { //approved email
                                // alert("Send aprpoval Emil to : " + $scope.appProperties.UtilityRequests.FinanceHead.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, null, "subject", "mailconetbody");
                                //Utility.helpers.sendEmail($scope.appProperties.UtilityRequests.FinanceHead.EMail, null, "eFaciliates Application for your approval", "This is to notify you that a NEW/CHANGE IN EFACILIATES application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/UtilityService.aspx?=" + $scope.appProperties.UtilityRequests.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.UtilityRequests.FinanceHead.EMail, "", "", "eFaciliates request Submitted " + $scope.appProperties.UtilityRequests.Author.Title + "", "This is to notify you that a eFaciliates Request " + $scope.appProperties.UtilityRequests.ApplicationID + " from " + $scope.appProperties.UtilityRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/UtilityDashboard.aspx>here</a>");
                            }
                            else {
                                //rejected email
                                // alert("Send rejected Emil to : " + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "eFaciliates Application Rejected", "This is to notify you that a NEW/CHANGE IN EFACILIATES application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/UtilityService.aspx?=" + $scope.appProperties.UtilityRequests.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.UtilityRequests.Author.EMail, "", "", "eFaciliates request rejected " + $scope.appProperties.UtilityRequests.Author.Title + "", "This is to notify you that a eFaciliates Request " + $scope.appProperties.UtilityRequests.ApplicationID + " from " + $scope.appProperties.UtilityRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/UtilityDashboard.aspx>here</a>");
                            }
                            break;
                        case 4:
                        case 5:
                            //send email to user- After Approved by financehead
                            if (Worfkflowstatus == '4') {
                                //alert("Send aprpoval Emil to : "+ $scope.appProperties.CurrentUser.Email);                         
                                var ccEmailFormat = ccEmailUsers.substring(1); //cc user assign while pageload
                                Utility.helpers.sendEmailwithCC($scope.appProperties.UtilityRequests.Author.EMail, ccEmailFormat, "eFaciliates request Submitted " + $scope.appProperties.UtilityRequests.Author.Title + "", "This is to notify you that a eFaciliates Request " + $scope.appProperties.UtilityRequests.ApplicationID + " from " + $scope.appProperties.UtilityRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/UtilityDashboard.aspx>here</a>");

                            }
                            else {
                                //rejected email
                                //alert("Send rejected Emil to : " + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "eFaciliates Application Rejected", "This is to notify you that EFACILIATES application <Application No> from <Staff Name> has been Rejected. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/UtilityService.aspx?=" + $scope.appProperties.UtilityRequests.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.UtilityRequests.Author.EMail, "", "", "eFaciliates request rejected " + $scope.appProperties.UtilityRequests.Author.Title + "", "This is to notify you that a eFaciliates Request " + $scope.appProperties.UtilityRequests.ApplicationID + " from " + $scope.appProperties.UtilityRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/UtilityDashboard.aspx>here</a>");
                            }
                            break;
                        default:
                        //alert('Invalid case');
                    }//switch  
                }
                catch (err) { //do some 
                }
                $scope.loaded = true;
                $timeout(function () {
                    alert(message);
                    window.location.href = redirectUrl;
                }, 1000);
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };       
        //get randon number  -text-year-Randomnumber-lastitemid(2019_001)
        function getUniqueNumber() {
            //Generate Document Numbr -getting NTP lasitem id
            uService.getLastListITem().then(function (response) {
                if (response.data.d.results.length > 0) {
                    var count = response.data.d.results[0].Id + 1;
                    $scope.appProperties.UtilityRequests.ApplicationID = "UTIL-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + count; //increment one
                }
                else {
                    $scope.appProperties.UtilityRequests.ApplicationID = "UTIL-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + "1"
                }
            });
          
        }; 
        //get vendor from CsutomList
        function getAllVendorMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllVendorMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.VendorMaster = response.data.d.results;
                }
            });
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
        //get getAllCostCentreMaster data from CustomList
        function getAllCostCentreMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllCostCentreMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.CostCentreMaster = response.data.d.results;
                }
            });
        };
        //get Authorization from csutom list
        function bindAuthorization() {
            $scope.loaded = true;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == Config.Roles['Division']) {
                          //  $scope.appProperties.BranchHeadMaster = value.Approvers.results;
                        }
                        if (value.Division == "Finance and Procurement") {
                            $scope.appProperties.FinanceHeadMaster = value.DivisionHead;
                            commonService.getUserbyId(value.DivisionHeadId).then(function (response) {
                                //check out of office
                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                    if (response.data.d.results.length > 0) {
                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                            $scope.appProperties.FinanceHeadMaster = user.d;                                           
                                        });
                                    }
                                });
                            });
                        }
                    });
                }
            });
        };
        //while login cheking user inside rbrahchead or divison head list
        function chkLoginUserIsBrachHeadorDivHead(ADID) {
            $scope.loaded = true;
            try {
                //pass here list name / columnaname
                commonService.getUserDeptHeadList(Config.BranchHeads, "BranchHead").then(function (response) {
                    $scope.loaded = false; //spinner stop  
                    if (response.data.d.results.length > 0) {
                        angular.forEach(response.data.d.results, function (value, key) {
                            if (value.BranchHead.EMail == $scope.appProperties.CurrentUser.Email) {
                                $scope.IsLoginUserBranchHead = true; //init
                            }
                        });
                        if ($scope.IsLoginUserBranchHead == true) {
                            commonService.getStaffInfoByAdId(ADID).then(function (response) {
                                if (response.data.d.results.length > 0) {
                                    var Division = response.data.d.results[0].Division;
                                    if (Division != null) {
                                        commonService.getStaffDivisionBranchHead(Config.DivisionHeads, "Division", Division).then(function (response) {
                                            $scope.loaded = false; //spinner stop  
                                            if (response.data.d.results.length > 0) {
                                                if (response.data.d.results[0].DivisionHead != null && response.data.d.results[0].DivisionHead != "" && response.data.d.results[0].DivisionHead != undefined) {
                                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].DivisionHead;
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
                //divisioh head - pass here list name / columnaname
                commonService.getUserDeptHeadList(Config.DivisionHeads, "DivisionHead").then(function (response) {
                    $scope.loaded = false; //spinner stop  
                    if (response.data.d.results.length > 0) {
                        angular.forEach(response.data.d.results, function (value, key) {
                            if (value.DivisionHead.EMail == $scope.appProperties.CurrentUser.Email) {
                                $scope.IsLoginUserDivisonHead = true; //init                             
                            }
                        });
                        if ($scope.IsLoginUserDivisonHead == true) {
                            commonService.getStaffInfoByAdId(ADID).then(function (response) {
                                if (response.data.d.results.length > 0) {
                                    var Cluster = response.data.d.results[0].Cluster;
                                    if (Cluster != null) {
                                        commonService.getStaffClusterhHead(Config.ClusterHeads, "Cluster", Cluster).then(function (response) {
                                            if (response.data.d.results.length > 0) {
                                                if (response.data.d.results[0].ClusterHead != null && response.data.d.results[0].ClusterHead != "" && response.data.d.results[0].ClusterHead != undefined) {
                                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].ClusterHead;
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
            }
            catch (err) {
                $scope.loaded = false;//do some
            }
        };
        //get BranchHeadFromStaffDir from csutom list-Root site
        function getDivisionHeadByGroupDir(ADID) {
            $scope.loaded = true;
            try {
                commonService.getStaffInfoByAdId(ADID).then(function (response) {
                    $scope.loaded = false; //spinner stop  
                    if (response.data.d.results.length > 0) {
                        var divson = response.data.d.results[0].Division;
                        if (divson != "") {

                            commonService.getStaffDivisionBranchHead(Config.DivisionHeads, "Division", divson).then(function (response) {
                                $scope.loaded = false; //spinner stop  
                                if (response.data.d.results.length > 0) {
                                    if (response.data.d.results[0].Division != null && response.data.d.results[0].Division != "" && response.data.d.results[0].Division != undefined) {

                                        $scope.appProperties.GroupDirectorMaster = response.data.d.results[0].DivisionHead; //acting as grodup director
                                    }
                                }
                            });
                        }
                    }
                });
            }
            catch (err) {
                $scope.loaded = false;//do some
            }
        };
        //get BranchHeadFromStaffDir from csutom list-Root site
        function getBranchHeadFromStaffDirxxx(Domain, ADID) {
            $scope.loaded = true;
            try {
                commonService.getStaffInfoByAdId(ADID).then(function (response) {
                    $scope.loaded = false; //spinner stop  
                    if (response.data.d.results.length > 0) {
                        var branch = response.data.d.results[0].Branch;
                        if (branch == null || branch == "") {
                            branch = response.data.d.results[0].Division;
                        }
                        commonService.getStaffBranchHead(Config.BranchHeads, "Branch", branch).then(function (response) {
                            $scope.loaded = false; //spinner stop  
                            if (response.data.d.results.length > 0) {
                                if (response.data.d.results[0].Branch != null && response.data.d.results[0].Branch != "" && response.data.d.results[0].Branch != undefined) {
                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].BranchHead;
                                }
                            }
                            else {
                                commonService.getStaffDivisionBranchHead(Config.DivisionHeads, "Division", branch).then(function (response) {
                                    $scope.loaded = false; //spinner stop  
                                    if (response.data.d.results.length > 0) {
                                        if (response.data.d.results[0].Division != null && response.data.d.results[0].Division != "" && response.data.d.results[0].Division != undefined) {
                                            $scope.appProperties.BranchHeadMaster = response.data.d.results[0].DivisionHead;
                                            $scope.appProperties.GroupDirectorMaster = response.data.d.results[0].DivisionHead; //acting as grodup director
                                        }
                                    }
                                });
                            }
                        });
                    }
                });
            }
            catch (err) {
                $scope.loaded = false;//do some
            }
        };
        //get BranchHeadFromStaffDir from csutom list-Root site
        function getBranchHeadFromStaffDir(Domain, ADID) {
            $scope.loaded = true;
            try {
                commonService.getStaffInfoByAdId(ADID).then(function (response) {
                    $scope.loaded = false; //spinner stop  
                    if (response.data.d.results.length > 0) {
                        var branch = response.data.d.results[0].Branch;
                        if (branch == null || branch == "") {
                            branch = response.data.d.results[0].Division;
                        }
                        commonService.getStaffBranchHead(Config.BranchHeads, "Branch", branch).then(function (response) {
                            $scope.loaded = false; //spinner stop  
                            if (response.data.d.results.length > 0) {
                                if (response.data.d.results[0].Branch != null && response.data.d.results[0].Branch != "" && response.data.d.results[0].Branch != undefined) {
                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].BranchHead;                                   
                                    commonService.getUserbyId(response.data.d.results[0].BranchHeadId).then(function (response) {
                                        //check out of office
                                        commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                            if (response.data.d.results.length > 0) {
                                                getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {                                               
                                                    $scope.appProperties.BranchHeadMaster = user.d
                                                });
                                            }
                                        });
                                    });
                                }
                            }
                            else {
                                commonService.getStaffDivisionBranchHead(Config.DivisionHeads, "Division", branch).then(function (response) {
                                    $scope.loaded = false; //spinner stop  
                                    if (response.data.d.results.length > 0) {
                                        if (response.data.d.results[0].Division != null && response.data.d.results[0].Division != "" && response.data.d.results[0].Division != undefined) {
                                            $scope.appProperties.BranchHeadMaster = response.data.d.results[0].DivisionHead;                                         
                                            commonService.getUserbyId(response.data.d.results[0].DivisionHeadId).then(function (response) {
                                                //check out of office
                                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                                    if (response.data.d.results.length > 0) {
                                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {                                                           
                                                            $scope.appProperties.BranchHeadMaster = user.d;
                                                            $scope.appProperties.GroupDirectorMaster = user.d; //acting as grodup director
                                                        });
                                                    }
                                                });
                                            });
                                        }
                                    }
                                });
                            }
                        });
                    }
                });
            }
            catch (err) {
                $scope.loaded = false;//do some
            }
        };
        /// username should be passed as 'domain\username'
        function GetUserIdFromLoginName(LoginNameWithDomain, Email, FullName) {
            /// change this prefix according to the environment. 
            /// In below sample, windows authentication is considered.
            var prefix = "i:0#.w|";
            /// get the site url
            var siteUrl = _spPageContextInfo.siteAbsoluteUrl;
            /// add prefix, this needs to be changed based on scenario
            var accountName = prefix + LoginNameWithDomain;

            /// make an ajax call to get the site user
            $.ajax({
                url: siteUrl + "/_api/web/siteusers(@v)?@v='" +
                    encodeURIComponent(accountName) + "'",
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: function (data) {
                    ///popup user id received from site users.                                    
                    var prop = [{
                        Id: data.d.Id, EMail: Email, Title: FullName
                    }];
                    $scope.appProperties.BranchHeadMaster = prop;
                    
                },
                error: function (data) {
                    console.log(JSON.stringify(data));
                    alert("Can't find Branch head :" + JSON.stringify(data)); 
                }
            });
        }
        //add file to repeat - UI screen
        $scope.upload = function (obj) {
            if (obj.files.length > 0) {
                $scope.loaded = true;
                var fileExit = false;
                for (var i = 0; i < obj.files.length; i++) {
                    var file = obj.files[i];
                    var reader = new FileReader();
                    //Getting the File name               
                    reader.filename = file.name;
                    //Getting the extention of the file
                    var ext = file.name.match(/\.([^\.]+)$/)[1];
                    reader.onloadend = function (e) {
                        //Validating the uploaded file Format
                        switch (ext) {
                            case 'pdf':
                            case 'png':
                            case 'jpeg':
                            case 'jpg':
                                if ($scope.appProperties.appFileProperties.length == 0) {
                                    $scope.appProperties.appFiles.push(file); //insert purpouse- suing this properties
                                    $scope.appProperties.appFileProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
                                    $scope.isSelectedFile = false;//for validation

                                    $scope.$apply(); // force digest cycle 
                                }
                                else {
                                    angular.forEach($scope.appProperties.appFileProperties, function (value, key) {
                                        if (value.fileName.indexOf(file.name) !== -1) {
                                            fileExit = true;
                                            alert("File already added")
                                        }
                                    });
                                    if (!fileExit) {
                                        $scope.appProperties.appFiles.push(file); //insert purpouse- suing this properties
                                        $scope.appProperties.appFileProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
                                        $scope.isSelectedFile = false;//for validation                                     
                                        $scope.$apply(); // force digest cycle 
                                    }
                                }
                                break;
                            default:
                                alert('Invalid format');
                        }//switch
                    };
                    reader.readAsDataURL(file);
                    $scope.loaded = false; //spinner stop   
                }
                $scope.$apply();//event callback.
            }
            else {
                alert('Please Select template !!!');
            }
        };
        //remove uploded one
        $scope.removeFiles = function (index, Title) {
            //Find the file using Index from Array.         
            if ($window.confirm("Do you want to remove ???" + Title)) {
                if ($scope.appProperties.UtilityRequests.ID != null && $scope.appProperties.UtilityRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.UtilityRequests.ID).then(function (response) {
                        $scope.loaded = false; //spinner stop - service call end 
                        //Remove the item from Array using Index.
                        $scope.appProperties.appFiles.splice(index, 1);
                        $scope.appProperties.appFileProperties.splice(index, 1);
                    }).catch(function (error) {
                        //Remove the item from Array using Index.
                        $scope.appProperties.appFiles.splice(index, 1);
                        $scope.appProperties.appFileProperties.splice(index, 1);
                        $scope.loaded = false; //spinner stop - service call end 
                    });
                }
                else {
                    //Remove the item from Array using Index.
                    $scope.appProperties.appFiles.splice(index, 1);
                    $scope.appProperties.appFileProperties.splice(index, 1);
                }
            }
        };
        //insert files-to folder list
        function insertSupportingDocuments(listItemId, message, redirectUrl) {
            if ($scope.appProperties.appFiles.length > 0) {
                var oList = web.get_lists().getByTitle(Config.UtilityDocuments);
                uService.getDocLibByFolderName(listItemId).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        //alread exit folder      
                        uploadFile(listItemId, $scope.appProperties.appFiles, redirectUrl,message);
                    }
                    else {
                        // Folder doesn't exist at all. so create folder here
                        var oListItem = oList.get_rootFolder().get_folders().add(listItemId); //foldername used by PJApproval listItemID
                        ctx.load(oListItem);
                        ctx.executeQueryAsync(function () {
                            uploadFile(listItemId, $scope.appProperties.appFiles, redirectUrl,message);
                        }, function (sender, args) {
                            console.log("something went wrong");
                            console.log('Err: ' + args.get_message());
                        });
                    }
                });
            }
        };
        function uploadFile(listItemId, fileInput, redirectUrl, message) {
            // Define the folder path for this example.
           // var targetUrl = _spPageContextInfo.webServerRelativeUrl + "/" + documentLibrary + "/" + listItemId;
            var serverRelativeUrlToFolder = '/erequest/UtilityDocuments/' + listItemId;
            var template = "Others"
            // Get test values from the file input and text input page controls.
            //var fileInput = jQuery('#getFile');
            //var newName = jQuery('#displayName').val();
            var fileCount = fileInput.length;
            // Get the server URL.
            var serverUrl = _spPageContextInfo.webAbsoluteUrl;
            var filesUploaded = 0;
            for (var i = 0; i < fileCount; i++) {
                // Initiate method calls using jQuery promises.
                // Get the local file as an array buffer.
               
                var getFile = getFileBuffer(i);
                getFile.done(function (arrayBuffer, i) {
                    // Add the file to the SharePoint folder.
                    var addFile = addFileToFolder(arrayBuffer, i);
                    addFile.done(function (file, status, xhr) {
                        //$("#msg").append("<div>File : "+file.d.Name+" ... uploaded sucessfully</div>");
                        template = $scope.appProperties.appFileProperties[i].template
                        filesUploaded++;
                        if (fileCount == filesUploaded) {                                     
                            $timeout(function () {
                                alert(message);
                                $scope.loaded = false;
                                window.location.href = redirectUrl;
                            }, 1000);                          
                        }

                    });
                    addFile.fail(onError);
                });
                getFile.fail(onError);              
            }

            // Get the local file as an array buffer.
            function getFileBuffer(i) {
                var deferred = jQuery.Deferred();
                var reader = new FileReader();
                reader.onloadend = function (e) {
                    deferred.resolve(e.target.result, i);
                }
                reader.onerror = function (e) {
                    deferred.reject(e.target.error);
                }
                reader.readAsArrayBuffer(fileInput[i]);
                return deferred.promise();
            }

            // Add the file to the file collection in the Shared Documents folder.
            function addFileToFolder(arrayBuffer, i) {
                var index = i;

                // Get the file name from the file input control on the page.
                var fileName = fileInput[index].name;
           
                // Construct the endpoint.
                var fileCollectionEndpoint = String.format(
                        "{0}/_api/web/getfolderbyserverrelativeurl('{1}')/files" +
                        "/add(overwrite=true, url='{2}')?$expand=ListItemAllFields",
                        serverUrl, serverRelativeUrlToFolder, fileName);

                // Send the request and return the response.
                // This call returns the SharePoint file.
                return jQuery.ajax({
                    url: fileCollectionEndpoint,
                    type: "POST",
                    data: arrayBuffer,
                    processData: false,
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "X-RequestDigest": jQuery("#__REQUESTDIGEST").val(),
                        "content-length": arrayBuffer.byteLength
                    },
                    success: onQuerySucceeded,
                    error: onQeuryFailed
                });
            }
            function onQeuryFailed(response, status, xhr) {
                console.log(status + ': ' + JSON.stringify(xhr));
            }
            function onQuerySucceeded(response, status, xhr) {
                //alert(response.d.Name + ' Uploaded Successfully');
                var itemId = response.d.ListItemAllFields.Id;              
                var fullUrl = _spPageContextInfo.webAbsoluteUrl +
                "/_api/Web/lists/GetByTitle('UtilityDocuments')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'UtilityRequestsID': listItemId
                       
                    }),
                    headers: {
                        'accept': 'application/json;odata=verbose',
                        'content-type': 'application/json;odata=verbose',
                        'X-RequestDigest': $('#__REQUESTDIGEST').val(),
                        'X-HTTP-Method': 'MERGE',
                        'IF-MATCH': response.d.ListItemAllFields.__metadata.etag,

                    },
                    success: onUpdateSuccess,
                    error: onUpdateFail
                });

            }
            function onUpdateFail(response, status, xhr) {
                console.log(status + ': ' + JSON.stringify(xhr));
            }
            function onUpdateSuccess(response, status, xhr) {
                //$('#Results').text(status+': '+JSON.stringify(xhr));
                console.log('Fields Updated Successfully');
            }
        }
        // Display error messages. 
        function onError(error) {
            console.log(error.responseText);
        }
        //insert comments
        function insertComments(listItemId, status) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.UtilityComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            if ($scope.appProperties.appComments.Comments != null && $scope.appProperties.appComments.Comments != "") {
                clistItem.set_item('UserComments', $scope.appProperties.appComments.Comments + " ( " + Config.CommentStatus[status] + " )");
            } else {
                clistItem.set_item('UserComments', Config.CommentStatus[status] + " (System Comments)");
            }
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('UtilityRequestsID', listItemId);            
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
        
        //Load data using querstring
        function loadUtilityRequestsListData(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getById(ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.UtilityRequests = response.data.d.results[0];
                    //enable approval button
                    DisableHideShowControls($scope.appProperties.UtilityRequests.WorkFlowStatus, $scope.appProperties.CurrentUser.Id) //disable controls while load and approval status
                    //format-date
                    $scope.appProperties.UtilityRequests.ApplicationDate = $filter('date')($scope.appProperties.UtilityRequests.Created, 'dd/MM/yyyy') //created  
                    $scope.appProperties.UtilityRequests.Author.LoginName = $scope.appProperties.UtilityRequests.RequestorLoginID;
                    $scope.appProperties.UtilityRequests.RequestedFor.LoginName = $scope.appProperties.UtilityRequests.RequestedForLoginID;
                    //opn
                    if ($scope.appProperties.UtilityRequests.Purpose == "Open New Account") {
                        $scope.IsOpenNewAccount = true;
                    }
                    if ($scope.appProperties.UtilityRequests.OnBehalf == "1") {
                        $scope.OnBehalf = true;
                    } else {
                        $scope.OnBehalf = false;
                    }
                    // checkbox result
                    if (response.data.d.results[0].UtilitiesApplyingFor != null) {
                        $scope.SelectionCheckboxInformation = response.data.d.results[0].UtilitiesApplyingFor.results;
                        angular.forEach($scope.SelectionCheckboxInformation, function (value, key) {
                            if (value == "Electricity") {
                                var arryList = $filter('filter')($scope.appProperties.VendorMaster, { Utilities: value });
                                if (arryList.length > 0) {
                                    $scope.appProperties.UtilitiesVendor1 = arryList;
                                }
                            }
                            if (value == "Water") {
                                var arryList = $filter('filter')($scope.appProperties.VendorMaster, { Utilities: value });
                                if (arryList.length > 0) {
                                    $scope.appProperties.UtilitiesVendor2 = arryList;
                                }
                            }
                        });
                    }                
                    //display staus on ui
                    $scope.appProperties.UtilityRequests.DisplayStatus = Config.WFStatus[$scope.appProperties.UtilityRequests.ApplicationStatus]
                }
            });
        };
        function loadFilesDocument(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getDocLibById(ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.appFileProperties.push({ fileName: value.Name, url: value.ServerRelativeUrl }); //in dispay in UI html page using this properties
                    });
                }
            });
        };
        //Load data using querstring
        function loadCommentsListData(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getCommentsById(ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.appComments = response.data.d.results;
                }
            });
        };
        //disable-controls
        function DisableHideShowControls(workflowstatus, currentUsrId) {
            //initally disble/show 
            $scope.IsBranchHeadDisabled = false; // disbale true        
            $scope.IsCommentsSection = true;         
            $scope.IsFinanceHeadDisabled = false; 
            //manager status- button  -disable             
            $scope.IsBranchHeadStatusDisabled = false;
            $scope.IsFinanceHeadStatusDisabled = false; 
        
            //while submit and save disable date field and button
            $scope.IsSubmitButton = false;
            $scope.IsSApproveButton = false;
            $scope.IsRejectButton = false;
            $scope.IsClosedButton = false;      
            $scope.IsAllcontrolDisbable = true;
            $scope.IsAttchment = true;
            //saved mode
            if (workflowstatus == "New" || workflowstatus == "4" && (currentUsrId == $scope.appProperties.UtilityRequests.Author.Id)) { 
                $scope.IsSubmitButton = true;      
                $scope.IsAttchment = false;             
                $scope.IsAllcontrolDisbable = false;
                $scope.IsCommentsSection = false;
            }           
            //Branch head  approval
            else if ((workflowstatus == "1" || workflowstatus == "10") && (currentUsrId == $scope.appProperties.UtilityRequests.BranchHead.Id)) {              
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;        
           
            }
            // After Branch head  approval- enable finance Head
            else if ((workflowstatus == "2" || workflowstatus == "10") && (currentUsrId == $scope.appProperties.UtilityRequests.FinanceHead.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                $scope.IsAttchment = false;
              
            } 
            // final close
            else if (workflowstatus == "3" && (currentUsrId == $scope.appProperties.UtilityRequests.Author.Id || currentUsrId == $scope.appProperties.UtilityRequests.FinanceHead.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsClosedButton = true;       
                $scope.IsAttchment = true;
               
            } 
            // Rejected status and not equal currentuser and approver (disable/Hide all control)
            else {
               //initailly alardy disbles
            }
        };
        //javascript methods---------------------------------------------------------------------------------------- end
    }]);
})();