"use strict";
(function () {
    app.controller("ITServiceRequestController", ["$scope", "ITService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
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
        var portNumber = 587;

        var enableSSL = true;
        var password = "";
        var toEmails = "";
        var fromEmail = "";
        var dispName = "";
        var bodyMsg = "";
         //end config
        //init variable
        $scope.isSiteAdmin = false;
        $scope.IsLoginUserDivisonHead = false; //init
        $scope.IsLoginUserBranchHead = false; //init
        $scope.IsITProjectManager = false; //init
        $scope.IsImplementationOfficer = false; //init        
        //check required validation
        $scope.appProperties = [];
        $scope.appProperties = ({
            //List Properties-exact columnname on ITServiceRequests(save/edit) and additional properties for ui purpose
            ITServiceRequests: ({
                ID: "",
                Title: "",
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker
                BranchHead: ({ Id: "", EMail: "", Title: "" }),
                RequestedFor: ({ Id: "", EMail: "", Title: "" }),
                ImplementationOfficer: ({ Id: "", EMail: "", Title: "" }),
                ITProjectManager: ({ Id: "", EMail: "", Title: "" }),
                GroupDirector: ({ Id: "", EMail: "", Title: "" }),
                CIO: ({ Id: "", EMail: "", Title: "" }),
                InstallUpgradeSWHostName: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                InstallUpgradeSWHostNameTitle:"",
                RemovalSWHostName: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                RemovalSWHostNameTitle: "",
                RemovalSWHostID:"",
                //date
                SubmittedDate: null,
                BranchHeadDate: null,
                ImplementationOfficerDate: null,
                ITProjectManagerDate: null,
                GroupDirectorDate: null,
                ImplemeCIODatentationOfficerDate: null,
                //status              
                BranchHeadStatus: "Pending",//set default
                ImplementationOfficerStatus: "",
                ITProjectManagerStatus: "",
                GroupDirectorStatus: "",
                CIOStatus: "",

                //others
                ApplicationID: "0",
                WorkAreaCode: "", // use this code to level of workflow
                RequestTypeCode: "", //use this code 
                WorkFlowCode: "",
                IndicateAccountNo: "",
                RemarksReasons: "",
                OffRenoFundDetail: "",
                OffRenoLocationAddress: "",
                InstallUpgradeSWLink: "",              
                NewADAcc: "0",
                DateRequiredBy: null,
                OffRenoProjectStartDate: null,
                OffRenoProjectEndDate: null,
                ApplicationDate: $filter('date')(new Date(), 'dd/MM/yyyy'), //dd-MM-yyyy hh:mm a   
                ApplicationStatus: "New",
                WorkFlowStatus: "0",
                DisplayStatus: "New", //UI purpose
                //-chekcbox list column                
                LoginAccountType: ({ results: [] }),
                OffRenoServices: ({ results: [] }),
                InstallUpgradeSW: ({ results: [] }),
                EmailServices: ({ results: [] }),
                EmailInformation:[]
            }),
            //WorkAreaMaster-List Properties
            WorkAreaMaster: ({ ID: "", Title: "", WorkAreaCode: "" }),
            //RequestTypeMaster-List Properties
            RequestTypeMaster: ({ ID: "", Title: "", WorkAreaCode: "", WorkFlowCode: "", RequestTypeCode: "" }),
            //Approvers
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            ImplementationOfficerMaster: ({ Id: "", EMail: "", Title: "" }),
            ITProjectManagerMaster: ({ Id: "", EMail: "", Title: "" }),
            GroupDirectorMaster: ({ Id: "", EMail: "", Title: "" }),
            CIOMaster: ({ Id: "", EMail: "", Title: "" }),
          
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
            appInstallUpgradeSWDocumentsFiles: [],
            appFileInstallUpgradeSWDocumentsProperties: [],
            appFloorPlanFiles: [],
            appFileFloorPlanProperties: [],
            appChecklistFiles: [],
            appFileChecklistProperties: [],
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", DomainName: "" }),
        });
        //EmailService-checkbox
        $scope.CheckboxEmailServiceInfo = ["User Mailbox", "Create Shared Mailbox", "Upgrade/Change Shared Mailbox", "Create Distribution List", "Upgrade/Change Distribution List Owner", "Upgrade Mailbox Plan", "Application Mail Relay (AMR) Service", "Application Mailbox Services (AMS)"];
        $scope.SelectionChkboxEmailServiceInfo = [];
        //Login Account Type -checkbox
        $scope.CheckboxLoginAccountTypeInfo = ["NPARKSNET", "WOG"];
        $scope.SelectionChkboxLoginAccountTypeInfo = [];
        //Install / Upgrade-checkbox
        $scope.CheckboxSotwareReqInfo = ["Existing", "New"];
        $scope.SelectionChkboxSotwareReqInfo = [];
        //renovation -bind checkbox using repeater
        $scope.CheckboxInformation = ["SG-WAN", "Wireless@SG", "Corp Wi-Fi"];
        // Selected PJInformation- push selected info in this varaible
        $scope.SelectionCheckboxInformation = [];
        $scope.onBehalfList = [{ value: false }, { value: true }]
        $scope.appChildProperties = ({
            //Current User Properties    
            DelegateUserItem: "",
            CurrentUser: ({ Id: "", Email: "", Title: "" }),
            DelegateUser: ({ Id: "", EMail: "", Title: "" }),
            DelegateManagerMaster: ({ Id: "", EMail: "", Title: "" })
        });
        $scope.DomainName = "";
        //Angular methods ----------------------------------------------------------------------------------start-while using html then create scope variable or method


        //init start
        $scope.init = function (init) {
            Utility.helpers.initializePeoplePicker('peoplePickerRequestedFor');
            Utility.helpers.initializePeoplePicker('peoplePickerInstallUpgradeSWHostName');
            Utility.helpers.initializePeoplePicker('peoplePickerRemovalSWHostName');
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerRequestedFor_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    $scope.IsRequestedReq = false;
                    $('#userID').val(userData.Key.split('\\')[1]);
                    $scope.appProperties.ITServiceRequests.RequestedFor.LoginName = userData.Key.split('\\')[1];
                    // Get the first user's ID by using the login name.
                    getUserId(userData.Key).done(function (user) {
                        $scope.appProperties.ITServiceRequests.RequestedFor.EMail = user.d.Email;
                        $scope.appProperties.ITServiceRequests.RequestedFor.Id = user.d.Id;
                    });
                    $scope.$apply();
                } else {
                    $scope.appProperties.ITServiceRequests.RequestedFor.Id = "";
                    $scope.appProperties.ITServiceRequests.RequestedFor.LoginName = "";
                    $scope.IsRequestedReq = true;
                    $scope.$apply();
                }
            };
            ////OnValueChangedClientScript- while user assing fire this event
            //SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerRemovalSWHostName_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
            //    var userData = selectedUsersInfo[0];
            //    if (userData !== undefined) {
            //        $('#userHostID').val(userData.Key.split('\\')[1]);
            //        $scope.appProperties.ITServiceRequests.RemovalSWHostName.LoginName = userData.Key.split('\\')[1];
            //        // Get the first user's ID by using the login name.
            //        getUserId(userData.Key).done(function (user) {
            //            $scope.appProperties.ITServiceRequests.RemovalSWHostName.Id = user.d.Id;
            //        });
            //    }
            //};
            //OnValueChangedClientScript- while user assing fire this event
            //SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerInstallUpgradeSWHostName_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
            //    var userData = selectedUsersInfo[0];
            //    if (userData !== undefined) {
            //        $scope.appProperties.ITServiceRequests.InstallUpgradeSWHostName.LoginName = userData.Key.split('\\')[1];
            //        // Get the first user's ID by using the login name.
            //        getUserId(userData.Key).done(function (user) {
            //            $scope.appProperties.ITServiceRequests.InstallUpgradeSWHostName.Id = user.d.Id;
            //        });
            //    }
            //};

            //load function
            getCurrentLoginUsersAndBindAllData(); //get current usser - After success call loading data         

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
        $scope.submitITServiceRequest = function (type, $event, Form) {
            $scope.loaded = true; //automate false while redirect page
            var appStatus = "0"; //maintain applciation field
            var cmtStatus = "0"; //maintain coments field
            $event.preventDefault();
            //init validation false 
            switch (type) {
                case 1:
                    //while submit neet to set true this field, to required msg will be dispaly
                    if (Form.$invalid && type != 0) {
                        $scope.loaded = false;
                        if (Form.DateRequiredBy.$invalid) Form.DateRequiredBy.$touched = true;
                        if (Form.RemarksReasons.$invalid) Form.RemarksReasons.$touched = true;
                        if (Form.ddlWorkArea.$invalid) Form.ddlWorkArea.$touched = true;
                        if (Form.ddlRequestType.$invalid) Form.ddlRequestType.$touched = true;
                        //while IsOfficeRenovation true
                        if ($scope.IsOfficeRenovation == true) {
                            if (Form.OffRenoFundDetail.$invalid) Form.OffRenoFundDetail.$touched = true;
                            if (Form.OffRenoLocationAddress.$invalid) Form.OffRenoLocationAddress.$touched = true;
                            if (Form.OffRenoProjectStartDate.$invalid) Form.OffRenoProjectStartDate.$touched = true;
                            if (Form.OffRenoProjectEndDate.$invalid) Form.OffRenoProjectEndDate.$touched = true;
                            if ($scope.SelectionCheckboxInformation.length == 0) {
                                alert('Please select Office Rennovation');
                                return;
                            }
                        }
                        //while IsInstallUpgrade true
                        if ($scope.IsInstallUpgrade == true) {
                            if (Form.InstallUpgradeSWHostNameTitle.$invalid) Form.InstallUpgradeSWHostNameTitle.$touched = true;
                            if (Form.InstallUpgradeSWLink.$invalid) Form.InstallUpgradeSWLink.$touched = true;
                            if ($scope.SelectionChkboxSotwareReqInfo.length == 0) {
                                alert('Please select Install / Upgrad checkbox');
                                return;
                            }
                            angular.forEach($scope.SelectionChkboxSotwareReqInfo, function (value, key) {
                                if (value == "New") {
                                    if ($scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.length == 0) {
                                        alert('Please select upload supporting document');
                                        return;
                                    }
                                }
                            });
                        }
                        if ($scope.IsUserLoginAccount == true) {
                            if ($scope.SelectionChkboxSotwareReqInfo.length == 0) {
                                alert('Please select User Login Account type');
                                return;
                            }
                        }
                        //while IsITSoftwareRemoval true
                        if ($scope.IsITSoftwareRemoval == true) {
                            if (Form.RemovalSWHostID.$invalid) Form.RemovalSWHostID.$touched = true;
                            if (Form.RemovalSWHostNameTitle.$invalid) Form.RemovalSWHostNameTitle.$touched = true;
                        }
                        return;
                    }
                    if ($scope.OnBehalf) {                       
                       
                        if ($scope.appProperties.ITServiceRequests.RequestedFor.Id == "") {                           
                            $scope.appProperties.ITServiceRequests.RequestedFor.LoginName = "";
                            $scope.IsRequestedReq = true; $scope.loaded = false;
                            return;
                        } 
                    }                    
                    //if (Form.DateRequiredBy.$invalid || Form.RemarksReasons.$invalid || Form.ddlWorkArea.$invalid || Form.ddlRequestType.$invalid) {
                    //    $scope.loaded = false;
                    //    if (Form.DateRequiredBy.$invalid) Form.DateRequiredBy.$touched = true; 
                    //    if (Form.RemarksReasons.$invalid) Form.RemarksReasons.$touched = true;
                    //    if (Form.ddlWorkArea.$invalid) Form.ddlWorkArea.$touched = true;
                    //    if (Form.ddlRequestType.$invalid) Form.ddlRequestType.$touched = true;
                    //    return;
                    //}
                    //$("#DateRequiredBy").focus();
                    //    else if () {
                    //        $("#RemarksReasons").focus();
                    //    }
                    //    else if () {
                    //        $("#ddlWorkArea").focus();
                    //    }
                    //    else if () {
                    //        $("#ddlRequestType").focus();
                    //    }
                    //else if (Form.OffRenoFundDetail.$invalid) {
                    //   // $("#OffRenoFundDetail").focus();
                    //}
                    //else if (Form.OffRenoLocationAddress.$invalid) {
                    // //   $("#OffRenoLocationAddress").focus();
                    //}

                    //  Form.OffRenoFundDetail.$touched = true;
                    // Form.OffRenoLocationAddress.$touched = true;


                    if ($scope.appProperties.ITServiceRequests.RequestTypeCode == "ITSEMAILSERVICE") {

                        if (!$scope.SelectionChkboxEmailServiceInfo.length != 0) {
                            $scope.loaded = false;
                            return;
                        }
                    }
                    else if ($scope.appProperties.ITServiceRequests.RequestTypeCode == "ITSUSRLGNACC") {

                    }
                    else if ($scope.appProperties.ITServiceRequests.RequestTypeCode == "HWLINETLAP") {

                    }
                    else if ($scope.appProperties.ITServiceRequests.RequestTypeCode == "SWRMV") {

                    }
                    else if ($scope.appProperties.ITServiceRequests.RequestTypeCode == "SWINTLUPGD") {

                    }
                    else if ($scope.appProperties.ITServiceRequests.RequestTypeCode == "ITSOFFRNV") {

                    }
                    else {
                        //
                    }
                    //reset status alwys -Rework
                    if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Multi" || $scope.appProperties.ITServiceRequests.WorkFlowCode == "Dynamic") { //6-Level
                        $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Pending";
                        $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "";
                        $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "";
                        $scope.appProperties.ITServiceRequests.CIOStatus = "";
                        $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "";
                    }
                    else {
                        $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Pending";
                        $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "N/A";
                        $scope.appProperties.ITProjectManagerMaster.length = 0;// N/A ITManager
                        $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "N/A";
                        $scope.appProperties.GroupDirectorMaster.length = 0;// N/A GroupDirector
                        $scope.appProperties.ITServiceRequests.CIOStatus = "N/A";
                        $scope.appProperties.CIOMaster.length = 0;// N/A CIO
                        $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "";
                    }
                    cmtStatus = 1;
                    insertUpdateListItem(type, "1", cmtStatus, "Submitted Successfully");     //workflowstaus,appstatus ,cmtStatus        
                    // code block
                    break;
                case 2:
                    //date -approve/rejecte date assing in disabled function                   
                    if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Multi") { //6-Level
                        //Branch head  approval
                        if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Pending"
                            && ($scope.appProperties.ITServiceRequests.WorkFlowStatus == "1" || $scope.appProperties.ITServiceRequests.WorkFlowStatus == "21")
                            && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.BranchHead.Id) {
                            //enable only BranchHead-approve/rejectbutton
                            $scope.appProperties.ITServiceRequests.BranchHeadDate = new Date();
                            $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "Pending";
                            $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Approved";
                            type = 2 // Approved by BranchHead
                            appStatus = 2; //inprogress
                            cmtStatus = 2;// Approved by BranchHead
                        }
                        //ITProjectManager approval-open after branch head approved
                        else if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved"
                            && ($scope.appProperties.ITServiceRequests.WorkFlowStatus == "2" || $scope.appProperties.ITServiceRequests.WorkFlowStatus == "21")
                            && $scope.IsITProjectManager) {
                            $scope.appProperties.ITServiceRequests.ITProjectManagerDate = new Date();
                            $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "Pending";
                            $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "Approved";
                            type = 4// Approved by ITManager
                            appStatus = 2; //inprogress
                            cmtStatus = 4;// Approved by ITManager
                        }
                        //GroupDirector approval-open after ITProjectManager approval
                        else if ($scope.appProperties.ITServiceRequests.ITProjectManagerStatus == "Approved"
                            && ($scope.appProperties.ITServiceRequests.WorkFlowStatus == "4" || $scope.appProperties.ITServiceRequests.WorkFlowStatus == "14" || $scope.appProperties.ITServiceRequests.WorkFlowStatus == "21")
                            && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.GroupDirector.Id) {
                            $scope.appProperties.ITServiceRequests.GroupDirectorDate = new Date();
                            $scope.appProperties.ITServiceRequests.CIOStatus = "Pending";
                            $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "Approved";
                            type = 6;// Approved by ITManager
                            appStatus = 2; //inprogress
                            cmtStatus = 6;// Approved by ITManager

                        }
                        //CIOS approval-open after ITProjectManager approval
                        else if ($scope.appProperties.ITServiceRequests.GroupDirectorStatus == "Approved"
                            && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.CIO.Id) {
                            $scope.appProperties.ITServiceRequests.CIOStatusDate = new Date();
                            $scope.appProperties.ITServiceRequests.CIOStatus = "Approved";
                            $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Pending";
                            type = 8;// Approved by CIO
                            appStatus = 2; //inprogress
                            cmtStatus = 8;//Approved by CIO

                        }
                        //ImplementationOfficerStatus approval-open after ITProjectManager approval
                        else if ($scope.appProperties.ITServiceRequests.CIOStatus == "Approved" && $scope.IsImplementationOfficer) {
                            $scope.appProperties.ITServiceRequests.ImplementationOfficerDate = new Date();
                            $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Approved";
                            type = 10;// Approved by ImplementationOfficer
                            appStatus = 3; //Completed
                            cmtStatus = 10;//Approved by ImplementationOfficer                            
                        }
                        //multi
                        insertUpdateListItem(type, appStatus, cmtStatus, "Approved Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    }
                    //dyanmic
                    else if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Dynamic") { //6-Level) {
                        //re-work
                        //Branch head  approval
                        if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.BranchHead.Id) {
                            //enable only BranchHead-approve/rejectbutton
                            if ($scope.appProperties.ITServiceRequests.BranchHeadDate == null || $scope.appProperties.ITServiceRequests.BranchHeadDate == undefined) {
                                $scope.appProperties.ITServiceRequests.BranchHeadDate = new Date();
                                $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "Pending";
                                $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Approved";
                                type = 2; //workflowstatus Approved by BranchHead
                                appStatus = 2; //inprogress
                                cmtStatus = 2;//Approved by BranchHead                              
                            }
                        }
                        //ITProjectManager
                        else if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved" && $scope.IsITProjectManager) {
                            if ($scope.appProperties.ITServiceRequests.ITProjectManagerDate == null || $scope.appProperties.ITServiceRequests.ITProjectManagerDate == undefined) {
                                $scope.appProperties.ITServiceRequests.ITProjectManagerDate = new Date();
                                $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "N/A";
                                $scope.appProperties.GroupDirectorMaster.length = 0;// GroupDirector N/A 
                                $scope.appProperties.ITServiceRequests.CIOStatus = "N/A";
                                $scope.appProperties.CIOMaster.length = 0;// CIO N/A 
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Pending";
                                $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "Approved";
                                type = 13; //workflowstatus -Approved by ITManager direct to IMO office
                                appStatus = 2; //inprogress
                                cmtStatus = 4;// Approved by ITManager

                            }
                        }
                        //CIO approval-open after ITProjectManager approval
                        else if (($scope.appProperties.ITServiceRequests.GroupDirectorStatus == "Approved" || $scope.appProperties.ITServiceRequests.ITProjectManagerStatus == "ReRouted") && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.CIO.Id) {
                            if ($scope.appProperties.ITServiceRequests.CIOStatusDate == null && $scope.appProperties.ITServiceRequests.CIOStatusDate == undefined) {
                                $scope.appProperties.ITServiceRequests.CIOStatusDate = new Date();
                                $scope.appProperties.ITServiceRequests.CIOStatus = "Approved";
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Pending";
                                type = 8; //workflowstatus -Approved by CIO
                                appStatus = 2; //inprogress
                                cmtStatus = 8;// Approved by CIO                                
                            }
                        }
                        else if (($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved" || $scope.appProperties.ITServiceRequests.GroupDirectorStatus == "ReRouted") && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.CIO.Id) {
                            if ($scope.appProperties.ITServiceRequests.CIOStatusDate == null && $scope.appProperties.ITServiceRequests.CIOStatusDate == undefined) {
                                $scope.appProperties.ITServiceRequests.CIOStatusDate = new Date();
                                $scope.appProperties.ITServiceRequests.CIOStatus = "Approved";
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Pending";
                                type = 8; //workflowstatus -Approved by CIO
                                appStatus = 2; //inprogress
                                cmtStatus = 8;// Approved by CIO                                
                            }
                        }
                        //ImplementationOfficerStatus approval-open after ITProjectManager approval
                        else if ((($scope.appProperties.ITServiceRequests.ITProjectManagerStatus == "Approved" || $scope.appProperties.ITServiceRequests.ITProjectManagerStatus == "ReRouted") || $scope.appProperties.ITServiceRequests.CIOStatus == "Approved") && $scope.IsImplementationOfficer) {
                            if ($scope.appProperties.ITServiceRequests.ImplementationOfficerDate == null && $scope.appProperties.ITServiceRequests.ImplementationOfficerDate == undefined) {
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerDate = new Date();
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Approved";
                                type = 10; //workflowstatus -Approved by ImplementationOfficer
                                appStatus = 3; //Approved -final stage
                                cmtStatus = 10;// Approved by ImplementationOfficer                                
                            }
                        }
                        else { }

                        insertUpdateListItem(type, appStatus, cmtStatus, "Approved Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    }
                    //2 Level -single
                    else {
                        //Branch head  approval
                        if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.BranchHead.Id) {
                            //enable only BranchHead-approve/rejectbutton
                            $scope.appProperties.ITServiceRequests.BranchHeadDate = new Date();
                            $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Pending";
                            $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Approved";
                            type = 18 // level2 approved by Branchhead Redirect to IMO
                            appStatus = 2; //inprogress
                            cmtStatus = 2;// Approved by BranchHead

                        }
                        //ImplementationOfficerStatus approval-open after ITProjectManager approval
                        else if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved" && $scope.IsImplementationOfficer) {
                            $scope.appProperties.ITServiceRequests.ImplementationOfficerDate = new Date();
                            $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Approved";
                            type = 10;// Approved by ImplementationOfficer
                            appStatus = 3; //Completed
                            cmtStatus = 10;//Approved by ImplementationOfficer

                        }
                        insertUpdateListItem(type, appStatus, cmtStatus, "Approved Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    }
                    // Approved block
                    break;
                case 3: //Rejected section
                    //date -approve/rejecte date assing in disabled function
                    if ($scope.appProperties.appComments.Comments != "" && $scope.appProperties.appComments.Comments != undefined) {
                        if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Multi" || $scope.appProperties.ITServiceRequests.WorkFlowCode == "Dynamic") { //6-Level
                            //Branch head  approval
                            if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.BranchHead.Id) {
                                //enable only BranchHead-approve/rejectbutton
                                $scope.appProperties.ITServiceRequests.BranchHeadDate = new Date();
                                $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Rejected";
                                type = 3; // Rejected by BranchHead
                                appStatus = 4; //Rejected 
                                cmtStatus = 3;// Rejected by BranchHead

                            }
                            //ITProjectManager approval-open after branch head approved
                            else if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved" && $scope.IsITProjectManager) {
                                $scope.appProperties.ITServiceRequests.ITProjectManagerDate = new Date();
                                $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "Rejected";
                                type = 5; // Rejected by ITManager
                                appStatus = 4; //Rejected 
                                cmtStatus = 5;// Rejected by ITManager

                            }
                            //GroupDirector approval-open after ITProjectManager approval
                            else if ($scope.appProperties.ITServiceRequests.ITProjectManagerStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.GroupDirector.Id) {
                                $scope.appProperties.ITServiceRequests.GroupDirectorDate = new Date();
                                $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "Rejected";
                                type = 7; // Rejected by GroupDirector
                                appStatus = 4; //Rejected 
                                cmtStatus = 7;// Rejected by GroupDirector

                            }
                            //GroupDirector approval-open after ITProjectManager approval
                            else if ($scope.appProperties.ITServiceRequests.GroupDirectorStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.CIO.Id) {
                                $scope.appProperties.ITServiceRequests.CIOStatusDate = new Date();
                                $scope.appProperties.ITServiceRequests.CIOStatus = "Rejected";
                                type = 9; // Rejected by CIO
                                appStatus = 4; //Rejected 
                                cmtStatus = 9;// Rejected by CIO

                            }
                            //ImplementationOfficerStatus approval-open after ITProjectManager approval
                            else if ($scope.appProperties.ITServiceRequests.CIOStatus == "Approved" && $scope.IsImplementationOfficer) {
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerDate = new Date();
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Rejected";
                                type = 11// maintain to FinanceHead -ApplicationStatus
                                appStatus = 4; //final 

                            }
                            insertUpdateListItem(type, appStatus, cmtStatus, "Rejected Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                        }
                        else { //2 Level -single -rject
                            //Branch head  approval
                            if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.ITServiceRequests.BranchHead.Id) {
                                //enable only BranchHead-approve/rejectbutton
                                if ($scope.appProperties.ITServiceRequests.BranchHeadDate == null || $scope.appProperties.ITServiceRequests.BranchHeadDate == undefined) {
                                    $scope.appProperties.ITServiceRequests.BranchHeadDate = new Date();
                                    $scope.appProperties.ITServiceRequests.BranchHeadStatus = "Rejected";
                                    type = 3; // Rejected by BranchHead
                                    appStatus = 4; //Rejected 
                                    cmtStatus = 3;// Rejected by BranchHead
                                }
                            }
                            //ImplementationOfficerStatus approval-open after ITProjectManager approval
                            else if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved" && $scope.IsImplementationOfficer) {
                                if ($scope.appProperties.ITServiceRequests.ImplementationOfficerDate == null && $scope.appProperties.ITServiceRequests.ImplementationOfficerDate == undefined) {
                                    $scope.appProperties.ITServiceRequests.ImplementationOfficerDate = new Date();
                                    $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "Rejected";
                                    type = 11;// maintain to FinanceHead -ApplicationStatus
                                    appStatus = 4; //Rejected 
                                    cmtStatus = 11;
                                }
                            }
                            insertUpdateListItem(type, appStatus, cmtStatus, "Rejected Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                        }
                        // Rejected block
                    }
                    else {
                        alert('Please enter rejection comments.');
                        $scope.loaded = false; //spinner stop - service call end
                        return;
                    }
                    break;
                   
                case 4:
                    //Re-work- ITMANAGER
                    if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Multi") { //6-Level
                        //ITProjectManager approval-open after branch head approved
                        if ($scope.appProperties.ITServiceRequests.BranchHeadStatus == "Approved" && $scope.IsITProjectManager) {
                            if ($scope.appProperties.ITServiceRequests.ITProjectManagerDate == null && $scope.appProperties.ITServiceRequests.ITProjectManagerDate == undefined) {
                                $scope.appProperties.ITServiceRequests.ITProjectManagerDate = new Date();
                                $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "ReSend";
                                type = 19; // Re-Work -ITManagerSendTo User
                                appStatus = 1; //Pending-Starting again
                                cmtStatus = 14;// Approved by ITManager
                            }
                        }
                        insertUpdateListItem(type, appStatus, cmtStatus, "ReRouted to User Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    }
                    // code block                  
                    break;
                case 5:
                    //Re-work- IMNO office
                    if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Multi" || $scope.appProperties.ITServiceRequests.WorkFlowCode == "Single") { //6-Level
                        if ($scope.IsImplementationOfficer) {
                            if ($scope.appProperties.ITServiceRequests.ImplementationOfficerDate == null && $scope.appProperties.ITServiceRequests.ImplementationOfficerDate == undefined) {
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerDate = new Date();
                                $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus = "ReSend";
                                type = 20; //workflowstatus -Re-Work -IMNOOfficerSendTo User
                                appStatus = 1; //Pending -starting stage
                                cmtStatus = 15;// Approved by ImplementationOfficer
                            }
                        }
                        insertUpdateListItem(type, appStatus, cmtStatus, "ReRouted to User Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    }
                    // code block                  
                    break;
                case 15:
                    //Re-route- 
                    if ($scope.SelectionChkboxDelegateUserInfo.length == 1) {
                        $('.modal-backdrop').remove();
                        $('#my_modal').modal('hide');                     
                        //update list
                        if ($scope.IsITProjectManager) {
                            cmtStatus = 12
                            if ($scope.SelectionChkboxDelegateUserInfo[0] == "CIO") {
                                $scope.appProperties.ITServiceRequests.CIOStatus = "Pending";
                                $scope.appProperties.ITServiceRequests.WorkFlowStatus = 15; //ITManager Re-Routed pending CIO
                            }
                            else {
                                $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "Pending";
                                $scope.appProperties.ITServiceRequests.WorkFlowStatus = 14; //ITManager Re-Routed pending GroupDirector
                            }
                            $scope.appProperties.ITServiceRequests.ITProjectManagerDate = new Date();
                            $scope.appProperties.ITServiceRequests.ITProjectManagerStatus = "ReRouted";
                            $scope.appProperties.ITServiceRequests.ApplicationStatus = 2; //inprogress
                        }
                        if ($scope.appProperties.ITServiceRequests.GroupDirector.Id == $scope.appProperties.CurrentUser.Id) {
                            cmtStatus = 13
                            if ($scope.SelectionChkboxDelegateUserInfo[0] == "CIO") {
                                $scope.appProperties.ITServiceRequests.GroupDirectorDate = new Date();
                                $scope.appProperties.ITServiceRequests.GroupDirectorStatus = "ReRouted";
                                $scope.appProperties.ITServiceRequests.CIOStatus = "Pending";
                                $scope.appProperties.ITServiceRequests.WorkFlowStatus = 17; //Group Director Re-Routed pending CIO
                            }
                        }
                        insertUpdateListItemPop(cmtStatus, $scope.SelectionChkboxDelegateUserInfo[0], $scope.appProperties.ITServiceRequests.WorkFlowStatus);
                    }
                    else {
                        alert("Please select delegate approver ITManager or CIO !..")
                        $scope.loaded = false; //spinner stop - service call end 
                    }
                    // code block                  
                    break;
                case 0:
                    //Close
                    window.location.href = siteAbsoluteUrl+"/Pages/ITServiceDashboard.aspx";
                    // code block                  
                    break;
                default:
                // code block
            }
        };
        function insertUpdateListItemPop(cmntStaus, delegateUserName, Worfkflowstatus) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.ITRequests + "");
            var listItem = "";
            if ($scope.appProperties.ITServiceRequests.Id != "" && $scope.appProperties.ITServiceRequests.Id != undefined) {
                //update list    
                listItem = list.getItemById($scope.appProperties.ITServiceRequests.Id);
                //all people picker field check value                 
                //date                
                listItem.set_item("ITProjectManagerDate", $scope.appProperties.ITServiceRequests.ITProjectManagerDate);
                listItem.set_item("GroupDirectorDate", $scope.appProperties.ITServiceRequests.GroupDirectorDate);
                listItem.set_item("CIODate", $scope.appProperties.ITServiceRequests.CIODate);
                listItem.set_item("ImplementationOfficerDate", $scope.appProperties.ITServiceRequests.ImplementationOfficerDate);
                //status 
                listItem.set_item("ITProjectManagerStatus", $scope.appProperties.ITServiceRequests.ITProjectManagerStatus);
                listItem.set_item("GroupDirectorStatus", $scope.appProperties.ITServiceRequests.GroupDirectorStatus);
                listItem.set_item("CIOStatus", $scope.appProperties.ITServiceRequests.CIOStatus);
                listItem.set_item("ImplementationOfficerStatus", $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus);
                listItem.set_item("WorkFlowStatus", $scope.appProperties.ITServiceRequests.WorkFlowStatus);
                listItem.set_item("ApplicationStatus", $scope.appProperties.ITServiceRequests.ApplicationStatus);
                listItem.update();
                ctx.load(listItem);
                ctx.executeQueryAsync(function () {
                    if (cmntStaus != "") {
                        var redirectUrl = siteAbsoluteUrl+"/Pages/ITServiceDashboard.aspx";
                        insertCommentsPop($scope.appProperties.ITServiceRequests.Id, cmntStaus, delegateUserName); //status number
                        //sendmail to approver                 
                        switch (Worfkflowstatus) {

                            case 14:                               
                                if ($scope.appProperties.ITServiceRequests.GroupDirector.EMail != "" && $scope.appProperties.ITServiceRequests.GroupDirector.EMail != null && $scope.appProperties.ITServiceRequests.GroupDirector.EMail != undefined) {
                                   Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.GroupDirector.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                                }
                                break;                          
                            case 15:
                            case 17:
                                if ($scope.appProperties.ITServiceRequests.CIO.EMail != "" && $scope.appProperties.ITServiceRequests.CIO.EMail != null && $scope.appProperties.ITServiceRequests.CIO.EMail != undefined) {
                                   Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.CIO.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                                }                              
                                break;                        
                            default:
                            //alert('Invalid case');
                        }//switch   
                        alert("Re-delegated successfully");
                        $timeout(function () {
                            $scope.loaded = true;                          
                            window.location.href = redirectUrl;
                        }, 3000);
                    }
                }, function (sender, args) {
                    console.log("something went wrong");
                    console.log('Err: ' + args.get_message());
                });

            }
        };
        //insert comments
        function insertCommentsPop(listItemId, status, delegateUserName) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.ITComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            //clistItem.set_item('Title', "Task delegated to " + delegateUserName + " (System Comments)");
            //clistItem.set_item('ITRequestsID', listItemId);
            clistItem.set_item('UserComments', Config.ITCommentStatus[status] + " (System Comments)");
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('ITListItemID', listItemId);
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
        // Check checkbox checked or not
        $scope.checkVal = function () {
            if ($scope.OnBehalf) {
                $scope.appProperties.ITServiceRequests.OnBehalf = "1";
                $scope.OnBehalf = true;
                if ($scope.appProperties.ITServiceRequests.RequestedFor.Id == "") {
                    $scope.appProperties.ITServiceRequests.RequestedFor.LoginName = "";
                    $scope.IsRequestedReq = true;
                } else { $scope.IsRequestedReq = false; }
            } else {
                $scope.IsRequestedReq = false;
                $scope.OnBehalf = false;
                $scope.appProperties.ITServiceRequests.OnBehalf = "0";
            }
            $scope.$apply();
        }
        // Check checkbox checked or not
        $scope.checkValADNew = function () {
            if ($scope.NewADAcc) {
                $scope.appProperties.ITServiceRequests.NewADAcc = "1";
                $scope.NewADAcc = true;
            } else {
                $scope.NewADAcc = false;
                $scope.appProperties.ITServiceRequests.NewADAcc = "0";
            }
            $scope.$apply();
        }
        // Toggle selection for a given chkbox by name
        $scope.toggleSelectionChkboxLoginAccountTypeInfo = function toggleSelectionChkboxLoginAccountTypeInfo(information) {
            var idx = $scope.SelectionChkboxLoginAccountTypeInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxLoginAccountTypeInfo.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxLoginAccountTypeInfo.push(information);
            }
        };
        // Toggle selection for a given chkbox by name
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
        };
        // Toggle selection for a given chkbox by name
        $scope.toggleSelectionChkboxSotwareReqInfo = function toggleSelectionChkboxSotwareReqInfo(information) {
            var idx = $scope.SelectionChkboxSotwareReqInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxSotwareReqInfo.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxSotwareReqInfo.push(information);
            }
        };

     
        $scope.toggleCheckboxEmailServiceSelection = function toggleCheckboxInfoSelection(information) {

            var idx = $scope.SelectionChkboxEmailServiceInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxEmailServiceInfo.splice(idx, 1);     
                $scope.appProperties.ITServiceRequests.EmailInformation.splice(idx, 1);             
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxEmailServiceInfo.push(information);              
            }
        };
        $scope.isAvailMsg = false;
        $scope.getRequestType = function () {

            $scope.appProperties.RequestTypeMaster.length = 0; //initially clear
            if ($scope.appProperties.ITServiceRequests.WorkAreaCode != "") {
                if ($scope.appProperties.ITServiceRequests.WorkAreaCode == "ITHARDWAREPERM" || $scope.appProperties.ITServiceRequests.WorkAreaCode == "ITHARDWARELOAN") {
                    $scope.isAvailMsg = true;
                }
                else { $scope.isAvailMsg = false;}
                $scope.loaded = true; //spinner start -service call start
                uService.getAllRequestTypeByWorkAreaID($scope.appProperties.ITServiceRequests.WorkAreaCode).then(function (response) {
                    $scope.loaded = false; //spinner stop - service call end 
                    if (response.data.d.results.length > 0) {
                        $scope.appProperties.RequestTypeMaster = response.data.d.results;
                    }
                });
            };
        }
        $scope.SelectRequestTypeFields = function () {
            if ($scope.appProperties.ITServiceRequests.RequestTypeCode != "--Select--") {
                ReqTypeHideShowControls($scope.appProperties.ITServiceRequests.RequestTypeCode);
                if ($scope.appProperties.ITServiceRequests.RequestTypeCode != null && $scope.appProperties.ITServiceRequests.RequestTypeCode != "" && $scope.appProperties.ITServiceRequests.RequestTypeCode != undefined) {

                    //chk Master data
                    var arryList = $filter('filter')($scope.appProperties.RequestTypeMaster, { RequestTypeCode: $scope.appProperties.ITServiceRequests.RequestTypeCode });
                    if (arryList.length > 0) {
                        $scope.appProperties.ITServiceRequests.WorkFlowCode = arryList[0].WorkFlowCode
                    }
                }
            };
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
                $scope.appProperties.ITServiceRequests.Author.Id = result.data.d.Id; //while init 
                $scope.appProperties.ITServiceRequests.Author.LoginName = result.data.d.Title; //while init                
                $scope.appProperties.ITServiceRequests.Author.Title = result.data.d.Title; //while init 
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];                   
                    getBrancGroupDirFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                }
                else {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName;//result.data.d.Title.split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.Title.split("\\")[0];
                  //  getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, result.data.d.LoginName); //chk only
                    getBrancGroupDirFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);

                }
                //all time load function                  
                bindAuthorization(); //         
                getDivisionHeadByGroupDir($scope.appProperties.CurrentUser.ADID); //get group dir from DivisionHeads list 
                getAllWorkAreaMaster(); //get all workarea
                getAllRequestTypeMaster();
                chkLoginUserIsBrachHeadorDivHead($scope.appProperties.CurrentUser.ADID); //chk user bracnhhead or divison head
                //get query string value
                $scope.appProperties.ITServiceRequests.ID = Utility.helpers.getUrlParameter('ReqId');
                if ($scope.appProperties.ITServiceRequests.ID != "" && $scope.appProperties.ITServiceRequests.ID != undefined) {
                    loadITServiceRequestsListData($scope.appProperties.ITServiceRequests.ID);//Load data     
                    //load doucments
                    loadFilesDocument($scope.appProperties.ITServiceRequests.ID, Config.ITRequestDocuments); //getITRequestDoc 
                    loadFileFloorDocuments($scope.appProperties.ITServiceRequests.ID, Config.OfficeRenovationDocuments); //OfficeRenovationDocuments 
                    loadChekListDocument($scope.appProperties.ITServiceRequests.ID, Config.OfficeRenovationChkListDocuments); //OfficeRenovationChkListDocuments 
                    loadFilesInstallUpgradeSWDocuments($scope.appProperties.ITServiceRequests.ID, Config.InstallUpgradeSWDocuments); //InstallUpgradeSWDocuments                     
                    //load comments
                    loadCommentsListData($scope.appProperties.ITServiceRequests.ID); //PriceComparison  
                }
                else {
                    //new req
                    getUniqueNumber(); //generate unique number-only for new Req   
                    DisableHideShowControls("New", $scope.appProperties.CurrentUser.Id,"0") //init stage
                }
                // chk is SiteAdmin  
                if (!result.data.d.IsSiteAdmin) {
                    $scope.isSiteAdmin = false;
                }
                else {
                    $scope.isSiteAdmin = true;
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
                        if ($scope.IsLoginUserBranchHead == true && $scope.IsLoginUserDivisonHead!=false) {
                            commonService.getStaffInfoByAdId(ADID).then(function (response) {
                                if (response.data.d.results.length > 0) {
                                    var Division = response.data.d.results[0].Division;
                                    if (Division != null) {
                                        commonService.getStaffDivisionBranchHead(Config.DivisionHeads, "Division", Division).then(function (response) {
                                            $scope.loaded = false; //spinner stop  
                                            if (response.data.d.results.length > 0) {
                                                if (response.data.d.results[0].DivisionHead != null && response.data.d.results[0].DivisionHead != "" && response.data.d.results[0].DivisionHead != undefined) {
                                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].DivisionHead;
                                                    commonService.getUserbyId(response.data.d.results[0].DivisionHeadId).then(function (response) {
                                                        //check out of office
                                                        commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                                            if (response.data.d.results.length > 0) {
                                                                if (response.data.d.results[0].CoveringApprovingOfficer1UserID != "N/A") {
                                                                    getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                                        $scope.appProperties.BranchHeadMaster = user.d;
                                                                    });
                                                                }
                                                            }
                                                        });
                                                    });
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        }
                        else {
                            //
                            getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
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
                                                    commonService.getUserbyId(response.data.d.results[0].ClusterHeadId).then(function (response) {
                                                        //check out of office
                                                        commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                                            if (response.data.d.results.length > 0) {
                                                                if (response.data.d.results[0].CoveringApprovingOfficer1UserID != "N/A") {
                                                                    getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                                        $scope.appProperties.BranchHeadMaster = user.d;
                                                                    });
                                                                }
                                                            }
                                                        });
                                                    });
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
        function getBrancGroupDirFromStaffDir(Domain, ADID) {
            $scope.loaded = true;
            try {
                commonService.getStaffInfoByAdId(ADID).then(function (response) {
                    $scope.loaded = false; //spinner stop  
                    if (response.data.d.results.length > 0) {
                        var Division = response.data.d.results[0].Division;                          
                        commonService.getStaffBranchHead("Division", Division).then(function (response) {
                                    $scope.loaded = false; //spinner stop  
                                    if (response.data.d.results.length > 0) {
                                        if (response.data.d.results[0].Division != null && response.data.d.results[0].Division != "" && response.data.d.results[0].Division != undefined) {                                        
                                            $scope.appProperties.GroupDirectorMaster = response.data.d.results[0].DivisionHead; //acting as grodup director
                                            commonService.getUserbyId(response.data.d.results[0].DivisionHeadId).then(function (response) {
                                                //check out of office
                                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                                    if (response.data.d.results.length > 0) {
                                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                            $scope.appProperties.GroupDirectorMaster = user.d
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
                        commonService.getStaffBranchHead(Config.BranchHeads,"Branch", branch).then(function (response) {
                            $scope.loaded = false; //spinner stop  
                            if (response.data.d.results.length > 0) {
                                if (response.data.d.results[0].Branch != null && response.data.d.results[0].Branch != "" && response.data.d.results[0].Branch != undefined) {
                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].BranchHead;
                                }
                            }
                            else {
                                commonService.getStaffDivisionBranchHead(Config.DivisionHeads,"Division", branch).then(function (response) {
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
        //insert/update ITServiceRequests 
        function insertUpdateListItem(Worfkflowstatus, ApplicationStatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.ITRequests + "");
            var listItem = "";
            if ($scope.appProperties.ITServiceRequests.ID == "" || $scope.appProperties.ITServiceRequests.ID == undefined) {
                //insert data to sharepoint

                var listCreationInformation = new SP.ListItemCreationInformation();
                listItem = list.addItem(listCreationInformation);
                listItem.set_item("RequestorLoginID", $scope.appProperties.CurrentUser.LoginName);

            } else {

                listItem = list.getItemById($scope.appProperties.ITServiceRequests.ID);
            }
            //assign properties while insert/update
            listItem.set_item("Title", $scope.appProperties.ITServiceRequests.ApplicationID);
            listItem.set_item("ApplicationID", $scope.appProperties.ITServiceRequests.ApplicationID);
            listItem.set_item("WorkAreaCode", $scope.appProperties.ITServiceRequests.WorkAreaCode);
            listItem.set_item("RequestTypeCode", $scope.appProperties.ITServiceRequests.RequestTypeCode);
            listItem.set_item("RemarksReasons", $scope.appProperties.ITServiceRequests.RemarksReasons);
            listItem.set_item("OffRenoFundDetail", $scope.appProperties.ITServiceRequests.OffRenoFundDetail);
            listItem.set_item("OffRenoLocationAddress", $scope.appProperties.ITServiceRequests.OffRenoLocationAddress);
            listItem.set_item("WorkFlowCode", $scope.appProperties.ITServiceRequests.WorkFlowCode);
            listItem.set_item("OnBehalf", $scope.appProperties.ITServiceRequests.OnBehalf);

            //version1
            listItem.set_item("RemovalSWHostID", $scope.appProperties.ITServiceRequests.RemovalSWHostID);
            listItem.set_item("RemovalSWHostNameTitle", $scope.appProperties.ITServiceRequests.RemovalSWHostNameTitle);
            listItem.set_item("InstallUpgradeSWHostNameTitle", $scope.appProperties.ITServiceRequests.InstallUpgradeSWHostNameTitle);
            
            listItem.set_item("InstallUpgradeSWLink", $scope.appProperties.ITServiceRequests.InstallUpgradeSWLink); //link         
            listItem.set_item("NewADAcc", $scope.appProperties.ITServiceRequests.NewADAcc); //choice

            //checkbox information
            
            listItem.set_item("EmailServices", $scope.SelectionChkboxEmailServiceInfo); //choice
            if ($scope.appProperties.ITServiceRequests.EmailInformation != null) {
                if ($scope.appProperties.ITServiceRequests.EmailInformation.length > 0) {
                    listItem.set_item("EmailInformation", $scope.appProperties.ITServiceRequests.EmailInformation); //choice      
                }
            }
            listItem.set_item("LoginAccountType", $scope.SelectionChkboxLoginAccountTypeInfo); //choice
            listItem.set_item("InstallUpgradeSW", $scope.SelectionChkboxSotwareReqInfo); //choice
            listItem.set_item("OffRenoServices", $scope.SelectionCheckboxInformation);  //-chekcbox        

            // datefield    
            if ($scope.appProperties.ITServiceRequests.DateRequiredBy != null && $scope.appProperties.ITServiceRequests.DateRequiredBy != undefined) {
                var dateParts = $scope.appProperties.ITServiceRequests.DateRequiredBy.split("/");
                // month is 0-based, that's why we need dataParts[1] - 1
                var DteRequiredBy = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                listItem.set_item("DateRequiredBy", DteRequiredBy);
            }
            if ($scope.appProperties.ITServiceRequests.OffRenoProjectStartDate != null && $scope.appProperties.ITServiceRequests.OffRenoProjectStartDate != undefined) {
                var dateParts = $scope.appProperties.ITServiceRequests.OffRenoProjectStartDate.split("/");
                // month is 0-based, that's why we need dataParts[1] - 1
                var ProjectStartDate = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                listItem.set_item("OffRenoProjectStartDate", ProjectStartDate);
            }
            if ($scope.appProperties.ITServiceRequests.OffRenoProjectEndDate != null && $scope.appProperties.ITServiceRequests.OffRenoProjectEndDate != undefined) {
                var datePartsExpiry = $scope.appProperties.ITServiceRequests.OffRenoProjectEndDate.split("/");
                // month is 0-based, that's why we need dataParts[1] - 1
                var ProjectEndDate = new Date(+datePartsExpiry[2], datePartsExpiry[1] - 1, +datePartsExpiry[0]);
                listItem.set_item("OffRenoProjectEndDate", ProjectEndDate);
            }
            //all people picker field check value 
            if ($scope.appProperties.ITServiceRequests.RequestedFor != undefined) {
                if ($scope.appProperties.ITServiceRequests.RequestedFor.Id) {
                    listItem.set_item("RequestedFor", $scope.appProperties.ITServiceRequests.RequestedFor.Id);
                    listItem.set_item("RequestedForLoginID", $scope.appProperties.ITServiceRequests.RequestedFor.LoginName);
                }
            }
            if ($scope.appProperties.ITServiceRequests.InstallUpgradeSWHostName != undefined) {
                if ($scope.appProperties.ITServiceRequests.InstallUpgradeSWHostName.Id) {
                    listItem.set_item("InstallUpgradeSWHostName", $scope.appProperties.ITServiceRequests.InstallUpgradeSWHostName.Id);
                }
            }
            if ($scope.appProperties.ITServiceRequests.RemovalSWHostName != undefined) {
                if ($scope.appProperties.ITServiceRequests.RemovalSWHostName.Id) {
                    listItem.set_item("RemovalSWHostName", $scope.appProperties.ITServiceRequests.RemovalSWHostName.Id);
                }
            }
            //  //all manager Name-people picker 
            if ($scope.appProperties.ITServiceRequests.WorkFlowCode == "Single") {
                if ($scope.appProperties.ITServiceRequests.BranchHead.Id) {
                    listItem.set_item("BranchHead", $scope.appProperties.ITServiceRequests.BranchHead.Id);
                }
                else {
                    if ($scope.appProperties.BranchHeadMaster != null && $scope.appProperties.BranchHeadMaster != undefined) {
                        listItem.set_item("BranchHead", $scope.appProperties.BranchHeadMaster.Id);
                    }
                }
                //ImplementationOfficer
                if ($scope.appProperties.ITServiceRequests.ImplementationOfficer.Id) {
                    listItem.set_item("ImplementationOfficer", $scope.appProperties.ITServiceRequests.ImplementationOfficer.Id);
                }
                else {
                    if ($scope.appProperties.ImplementationOfficerMaster.length > 0) {
                        if ($scope.IsImplementationOfficer) {
                            listItem.set_item("ImplementationOfficer", $scope.appProperties.CurrentUser.Id);
                        }
                      //  listItem.set_item("ImplementationOfficer", $scope.appProperties.ImplementationOfficerMaster[0].Id);
                       
                    }
                }
            }
            else {
                if ($scope.appProperties.ITServiceRequests.BranchHead.Id) {
                    listItem.set_item("BranchHead", $scope.appProperties.ITServiceRequests.BranchHead.Id);
                }
                else {
                    if ($scope.appProperties.BranchHeadMaster != null && $scope.appProperties.BranchHeadMaster != undefined) {
                        listItem.set_item("BranchHead", $scope.appProperties.BranchHeadMaster.Id);
                        $scope.appProperties.ITServiceRequests.BranchHead = $scope.appProperties.BranchHeadMaster;
                    }
                }
                //ITProjectManager
                if ($scope.appProperties.ITServiceRequests.ITProjectManager.Id != "" && $scope.appProperties.ITServiceRequests.ITProjectManager.Id != null) {
                    listItem.set_item("ITProjectManager", $scope.appProperties.ITServiceRequests.ITProjectManager.Id);
                }
                else {
                    if ($scope.appProperties.ITProjectManagerMaster.length > 0) {
                        if ($scope.IsITProjectManager) {
                            listItem.set_item("ITProjectManager", $scope.appProperties.CurrentUser.Id);
                        }
                      //  listItem.set_item("ITProjectManager", $scope.appProperties.ITProjectManagerMaster[0].Id);
                       // $scope.appProperties.ITServiceRequests.ITProjectManager = $scope.appProperties.ITProjectManagerMaster[0];
                    }
                   
                }
                //ImplementationOfficer
                if ($scope.appProperties.ITServiceRequests.ImplementationOfficer.Id) {
                    listItem.set_item("ImplementationOfficer", $scope.appProperties.ITServiceRequests.ImplementationOfficer.Id);
                }
                else {
                    if ($scope.appProperties.ImplementationOfficerMaster.length > 0) {
                        
                        if ($scope.IsImplementationOfficer) {
                            listItem.set_item("ImplementationOfficer", $scope.appProperties.CurrentUser.Id);
                        }
                       // listItem.set_item("ImplementationOfficer", $scope.appProperties.ImplementationOfficerMaster[0].Id);
                        //$scope.appProperties.ITServiceRequests.ImplementationOfficer = $scope.appProperties.ImplementationOfficerMaster[0];
                    }
                }
                //GroupDirector
                if ($scope.appProperties.ITServiceRequests.GroupDirector.Id) {
                    listItem.set_item("GroupDirector", $scope.appProperties.ITServiceRequests.GroupDirector.Id);
                }
                else {
                    if ($scope.appProperties.GroupDirectorMaster != null && $scope.appProperties.GroupDirectorMaster != undefined) {
                        listItem.set_item("GroupDirector", $scope.appProperties.GroupDirectorMaster.Id);
                        $scope.appProperties.ITServiceRequests.GroupDirector = $scope.appProperties.GroupDirectorMaster;
                    }
                }
                //CIO
                if ($scope.appProperties.ITServiceRequests.CIO.Id) {
                    listItem.set_item("CIO", $scope.appProperties.ITServiceRequests.CIO.Id);
                }
                else {
                    if ($scope.appProperties.CIOMaster.length > 0) {
                        listItem.set_item("CIO", $scope.appProperties.CIOMaster[0].Id);
                        $scope.appProperties.ITServiceRequests.CIO = $scope.appProperties.CIOMaster[0];
                    }
                }
            }

            //date
            listItem.set_item("BranchHeadDate", $scope.appProperties.ITServiceRequests.BranchHeadDate);
            listItem.set_item("ITProjectManagerDate", $scope.appProperties.ITServiceRequests.ITProjectManagerDate);
            listItem.set_item("GroupDirectorDate", $scope.appProperties.ITServiceRequests.GroupDirectorDate);
            listItem.set_item("CIODate", $scope.appProperties.ITServiceRequests.CIODate);
            listItem.set_item("ImplementationOfficerDate", $scope.appProperties.ITServiceRequests.ImplementationOfficerDate);
            //status 
            listItem.set_item("BranchHeadStatus", $scope.appProperties.ITServiceRequests.BranchHeadStatus);
            listItem.set_item("ITProjectManagerStatus", $scope.appProperties.ITServiceRequests.ITProjectManagerStatus);
            listItem.set_item("GroupDirectorStatus", $scope.appProperties.ITServiceRequests.GroupDirectorStatus);
            listItem.set_item("CIOStatus", $scope.appProperties.ITServiceRequests.CIOStatus);
            listItem.set_item("ImplementationOfficerStatus", $scope.appProperties.ITServiceRequests.ImplementationOfficerStatus);
            //workflow general status                  
            listItem.set_item("ApplicationStatus", ApplicationStatus);
            listItem.set_item("WorkFlowStatus", Worfkflowstatus);

            listItem.update();
            ctx.load(listItem);
            ctx.executeQueryAsync(function () {
                try {
                    var redirectUrl = siteAbsoluteUrl+"/Pages/ITServiceDashboard.aspx";
                    insertComments(listItem.get_id(), commentStatus);
                    if ($scope.appProperties.appFiles.length > 0) {
                        insertSupportingDocuments(listItem.get_id(), message, redirectUrl);
                    }
                    if ($scope.appProperties.appFloorPlanFiles.length > 0) {
                        insertSupportingOfficeRenovationDocuments(listItem.get_id(), message, redirectUrl);
                    }
                    if ($scope.appProperties.appChecklistFiles.length > 0) {
                        insertSupportingOfficeRenovationChkListDocuments(listItem.get_id(), message, redirectUrl);
                    }
                    if ($scope.appProperties.appInstallUpgradeSWDocumentsFiles.length > 0) {
                        insertSupportinguploadInstallUpgradeSWDocuments(listItem.get_id(), message, redirectUrl);
                    }

                    //sendmail to approver                 
                    switch (Worfkflowstatus) {

                        case 1:
                            //send email to branchHead/user- Submitted for approval
                            if ($scope.appProperties.BranchHeadMaster != null && $scope.appProperties.BranchHeadMaster != undefined) {
                                // alert("MAil sent to " + $scope.appProperties.BranchHeadMaster.EMail + " Status :" + Worfkflowstatus);
                                //Utility.helpers.SendGMailService(hostName, portNumber, true, password, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "IT Service Request Submitted " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID +" from " + $scope.appProperties.ITServiceRequests.Author.Title+" has been submitted. Please click here to <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>Check status</a>");
                                //Utility.helpers.SendGMailService(hostName, portNumber, true, password, fromEmail, dispName, $scope.appProperties.BranchHeadMaster.EMail, "", "", "IT Service Request Submitted" + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID +" from " + $scope.appProperties.ITServiceRequests.Author.Title+" has been submitted. Please click here to <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");


                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.BranchHeadMaster.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                            }
                            if ($scope.appProperties.ITServiceRequests.RequestedFor.EMail != "" && $scope.appProperties.ITServiceRequests.RequestedFor.EMail != null && $scope.appProperties.ITServiceRequests.RequestedFor.EMail != undefined) {
                                //Utility.helpers.sendEmail($scope.appProperties.ITServiceRequests.RequestedFor.EMail, "IT Service Request Submitted", "This is to notify you that a IT Service Request application <Application No> from <Staff Name> has been submitted. Please click here <a href='" + siteAbsoluteUrl + "/Pages/ITServiceRequest.aspx?=" + $scope.appProperties.ITServiceRequests.ID + "'>here.</a>");
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.RequestedFor.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                            }
                            break;
                        case 2:
                        case 3:
                            if (Worfkflowstatus == '3') {
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request rejected " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                            }
                            else {
                                //send email to ITManger/user- After Approved by BranchHead                           
                                //itmanager to all
                                angular.forEach($scope.appProperties.ITProjectManagerMaster, function (value, key) {
                                    toEmails += value.EMail + "|";
                                });
                                if (toEmails != "") {
                                    toEmails = toEmails.substring(0, toEmails.length - 1);
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, "" + toEmails.toString() + "", "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                                }
                                }
                            break;;
                        case 4:
                        case 5:
                            //send email to GroupDirc/user- After Approved by ITManger
                            if ($scope.appProperties.ITServiceRequests.GroupDirector.Id && Worfkflowstatus == '4') {
                               // alert("MAil sent to " + $scope.appProperties.CurrentUser.Email + " Status :" + Worfkflowstatus);
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.GroupDirector.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                            }
                            else {
                                //rejected email
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request rejected " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        case 6:
                        case 7:
                            //send email to CIO/user- After Approved by GroupDirecotr
                            if ($scope.appProperties.ITServiceRequests.CIO.Id && Worfkflowstatus == '6') {
                               // alert("MAil sent to " + $scope.appProperties.CurrentUser.Email + " Status :" + Worfkflowstatus);
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CIO.EMail, "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                            }
                            else {
                                //rejected email
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request rejected " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        case 8:
                        case 9:
                        case 18:
                            //send email to Imple/user- After Approved by CIO
                            if ($scope.appProperties.ImplementationOfficerMaster.length>0) {

                                //itmanager to all
                                angular.forEach($scope.appProperties.ImplementationOfficerMaster, function (value, key) {
                                    toEmails += value.EMail + "|";
                                });
                                if (toEmails != "") {
                                    toEmails = toEmails.substring(0, toEmails.length - 1);
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, "" + toEmails.toString() + "", "", "", "IT Service Request Submitted by " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here</a>");
                                }                               
                            }
                            else {
                                //rejected email
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request rejected " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        case 10:
                        case 11:
                            //send email to user- After Approved by implem -final
                            if (Worfkflowstatus == '10') {
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request Submitted " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                             }
                            else {
                                //rejected email
                               Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.ITServiceRequests.Author.EMail, "", "", "IT Service Request rejected " + $scope.appProperties.ITServiceRequests.Author.Title + "", "This is to notify you that a IT Service Request " + $scope.appProperties.ITServiceRequests.ApplicationID + " from " + $scope.appProperties.ITServiceRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/ITServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        default:
                        //alert('Invalid case');
                    }//switch            
                }
                catch (err) { //do some 
                }

                $timeout(function () {
                    $scope.loaded = true;
                    alert(message);
                    window.location.href = redirectUrl;
                }, 6000);

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
                    $scope.appProperties.ITServiceRequests.ApplicationID = "IT-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + count; //increment one
                }
                else {
                    $scope.appProperties.ITServiceRequests.ApplicationID = "IT-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + "1"
                }
            });

        };

        //get getAllWorkAreaMaster data from CustomList
        function getAllWorkAreaMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllWorkAreaMaster(Config.WorkArea).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.WorkAreaMaster = response.data.d.results;
                }
        }); 
        };
        function getAllRequestTypeMaster() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllRequestTypeMaster(Config.TypeOfRequest).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.RequestTypeMaster = response.data.d.results;
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
                        if (value.Title == Config.Roles['BranchHead']) {
                            // $scope.appProperties.BranchHeadMaster = value.Approvers.results; //Change request -geting from statff directoy
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
                       
                        if (value.Title == Config.Roles['CIO']) {
                            $scope.appProperties.CIOMaster = value.Approvers.results;
                            commonService.getUserbyId(response.data.d.results[0].DivisionHeadId).then(function (response) {
                                //check out of office
                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                    if (response.data.d.results.length > 0) {
                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                            $scope.appProperties.CIOMaster = user.d
                                        });
                                    }
                                });
                            });

                        }
                    });
                }
            });
        };
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
            if ($window.confirm("Do you want to remove " + Title)) {
                if ($scope.appProperties.ITServiceRequests.ID != null && $scope.appProperties.ITServiceRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.ITServiceRequests.ID).then(function (response) {
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
                var oList = web.get_lists().getByTitle(Config.ITRequestDocuments);
                uService.getDocLibByFolderName(listItemId, Config.ITRequestDocuments).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        //alread exit folder      
                        uploadFile(listItemId, $scope.appProperties.appFiles, redirectUrl, message);
                    }
                    else {
                        // Folder doesn't exist at all. so create folder here
                        var oListItem = oList.get_rootFolder().get_folders().add(listItemId); //foldername used by PJApproval listItemID
                        ctx.load(oListItem);
                        ctx.executeQueryAsync(function () {
                            uploadFile(listItemId, $scope.appProperties.appFiles, redirectUrl, message);
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
            var serverRelativeUrlToFolder = _spPageContextInfo.siteServerRelativeUrl + '/' + Config.ITRequestDocuments + '/' + listItemId;
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
                                $scope.loaded = true;
                                //  alert(message);
                                //$scope.loaded = false;
                                // window.location.href = redirectUrl;
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
                    "/_api/Web/lists/GetByTitle('" + Config.ITRequestDocuments + "')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'ITRequestsID': listItemId

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

        //add file to repeat - UI screen
        $scope.uploadInstallUpgradeSWDocuments = function (obj) {
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
                                if ($scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.length == 0) {
                                    $scope.appProperties.appInstallUpgradeSWDocumentsFiles.push(file); //insert purpouse- suing this properties
                                    $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
                                    $scope.isSelectedFile = false;//for validation

                                    $scope.$apply(); // force digest cycle 
                                }
                                else {
                                    angular.forEach($scope.appProperties.appFileInstallUpgradeSWDocumentsProperties, function (value, key) {
                                        if (value.fileName.indexOf(file.name) !== -1) {
                                            fileExit = true;
                                            alert("File already added")
                                        }
                                    });
                                    if (!fileExit) {
                                        $scope.appProperties.appInstallUpgradeSWDocumentsFiles.push(file); //insert purpouse- suing this properties
                                        $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
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
        $scope.removeuploadInstallUpgradeSWDocumentsFiles = function (index, Title) {
            //Find the file using Index from Array.         
            if ($window.confirm("Do you want to remove ???" + Title)) {
                if ($scope.appProperties.ITServiceRequests.ID != null && $scope.appProperties.ITServiceRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.ITServiceRequests.ID).then(function (response) {
                        $scope.loaded = false; //spinner stop - service call end 
                        //Remove the item from Array using Index.
                        $scope.appProperties.appFiles.splice(index, 1);
                        $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.splice(index, 1);
                    }).catch(function (error) {
                        //Remove the item from Array using Index.
                        $scope.appProperties.appInstallUpgradeSWDocumentsFiles.splice(index, 1);
                        $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.splice(index, 1);
                        $scope.loaded = false; //spinner stop - service call end 
                    });
                }
                else {
                    //Remove the item from Array using Index.
                    $scope.appProperties.appInstallUpgradeSWDocumentsFiles.splice(index, 1);
                    $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.splice(index, 1);
                }
            }
        };
        //insert files-to folder list
        function insertSupportinguploadInstallUpgradeSWDocuments(listItemId, message, redirectUrl) {
            if ($scope.appProperties.appInstallUpgradeSWDocumentsFiles.length > 0) {
                var oList = web.get_lists().getByTitle(Config.InstallUpgradeSWDocuments);
                uService.getDocLibByFolderName(listItemId, Config.InstallUpgradeSWDocuments).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        //alread exit folder      
                        uploadFileInstallUpgrade(listItemId, $scope.appProperties.appInstallUpgradeSWDocumentsFiles, redirectUrl, message);
                    }
                    else {
                        // Folder doesn't exist at all. so create folder here
                        var oListItem = oList.get_rootFolder().get_folders().add(listItemId); //foldername used by PJApproval listItemID
                        ctx.load(oListItem);
                        ctx.executeQueryAsync(function () {
                            uploadFileInstallUpgrade(listItemId, $scope.appProperties.appInstallUpgradeSWDocumentsFiles, redirectUrl, message);
                        }, function (sender, args) {
                            console.log("something went wrong");
                            console.log('Err: ' + args.get_message());
                        });
                    }
                });
            }
        };
        function uploadFileInstallUpgrade(listItemId, fileInput, redirectUrl, message) {
            // Define the folder path for this example.
            // var targetUrl = _spPageContextInfo.webServerRelativeUrl + "/" + documentLibrary + "/" + listItemId;
            var serverRelativeUrlToFolder = _spPageContextInfo.siteServerRelativeUrl + '/' + Config.InstallUpgradeSWDocuments + '/' + listItemId;
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
                        template = $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties[i].template
                        filesUploaded++;
                        if (fileCount == filesUploaded) {
                            $timeout(function () {
                                $scope.loaded = true;
                                //  alert(message);
                                //$scope.loaded = false;
                                // window.location.href = redirectUrl;
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
                    "/_api/Web/lists/GetByTitle('" + Config.InstallUpgradeSWDocuments + "')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'ITRequestsID': listItemId

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

        //add file to repeat - UI screen
        $scope.uploadFloor = function (obj) {
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
                                if ($scope.appProperties.appFileFloorPlanProperties.length == 0) {
                                    $scope.appProperties.appFloorPlanFiles.push(file); //insert purpouse- suing this properties
                                    $scope.appProperties.appFileFloorPlanProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
                                    $scope.isSelectedFile = false;//for validation
                                    $scope.$apply(); // force digest cycle 
                                }
                                else {
                                    angular.forEach($scope.appProperties.appFileFloorPlanProperties, function (value, key) {
                                        if (value.fileName.indexOf(file.name) !== -1) {
                                            fileExit = true;
                                            alert("File already added")
                                        }
                                    });
                                    if (!fileExit) {
                                        $scope.appProperties.appFloorPlanFiles.push(file); //insert purpouse- suing this properties
                                        $scope.appProperties.appFileFloorPlanProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
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
        $scope.removeFloorPlanFiles = function (index, Title) {
            //Find the file using Index from Array.         
            if ($window.confirm("Do you want to remove ???" + Title)) {
                if ($scope.appProperties.ITServiceRequests.ID != null && $scope.appProperties.ITServiceRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.ITServiceRequests.ID).then(function (response) {
                        $scope.loaded = false; //spinner stop - service call end 
                        //Remove the item from Array using Index.
                        $scope.appProperties.appFiles.splice(index, 1);
                        $scope.appProperties.appFileProperties.splice(index, 1);
                    }).catch(function (error) {
                        //Remove the item from Array using Index.
                        $scope.appProperties.appFloorPlanFiles.splice(index, 1);
                        $scope.appProperties.appFileFloorPlanProperties.splice(index, 1);
                        $scope.loaded = false; //spinner stop - service call end 
                    });
                }
                else {
                    //Remove the item from Array using Index.
                    $scope.appProperties.appFloorPlanFiles.splice(index, 1);
                    $scope.appProperties.appFileFloorPlanProperties.splice(index, 1);
                }
            }
        };
        //insert files-to folder list
        function insertSupportingOfficeRenovationDocuments(listItemId, message, redirectUrl) {
            if ($scope.appProperties.appFloorPlanFiles.length > 0) {
                var oList = web.get_lists().getByTitle(Config.OfficeRenovationDocuments);
                uService.getDocLibByFolderName(listItemId, Config.OfficeRenovationDocuments).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        //alread exit folder      
                        uploadFileFloor(listItemId, $scope.appProperties.appFloorPlanFiles, redirectUrl, message, Config.OfficeRenovationDocuments);
                    }
                    else {
                        // Folder doesn't exist at all. so create folder here
                        var oListItem = oList.get_rootFolder().get_folders().add(listItemId); //foldername used by PJApproval listItemID
                        ctx.load(oListItem);
                        ctx.executeQueryAsync(function () {
                            uploadFileFloor(listItemId, $scope.appProperties.appFloorPlanFiles, redirectUrl, message, Config.OfficeRenovationDocuments);
                        }, function (sender, args) {
                            console.log("something went wrong");
                            console.log('Err: ' + args.get_message());
                        });
                    }
                });
            }
        };
        function uploadFileFloor(listItemId, fileInput, redirectUrl, message, docListName) {
            // Define the folder path for this example.
            // var targetUrl = _spPageContextInfo.webServerRelativeUrl + "/" + documentLibrary + "/" + listItemId;
            var serverRelativeUrlToFolder = _spPageContextInfo.siteServerRelativeUrl + '/' + docListName + '/' + listItemId;
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
                                $scope.loaded = true;
                                // alert(message);
                                // $scope.loaded = false;
                                //window.location.href = redirectUrl;
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
                    "/_api/Web/lists/GetByTitle('" + Config.OfficeRenovationDocuments + "')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'ITRequestsID': listItemId,
                        'DocType': "FloorPlan"

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

        //add file to repeat - UI screen
        $scope.uploadChecklist = function (obj) {
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
                                if ($scope.appProperties.appFileChecklistProperties.length == 0) {
                                    $scope.appProperties.appChecklistFiles.push(file); //insert purpouse- suing this properties
                                    $scope.appProperties.appFileChecklistProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
                                    $scope.isSelectedFile = false;//for validation
                                    $scope.$apply(); // force digest cycle 
                                }
                                else {
                                    angular.forEach($scope.appProperties.appFileChecklistProperties, function (value, key) {
                                        if (value.fileName.indexOf(file.name) !== -1) {
                                            fileExit = true;
                                            alert("File already added")
                                        }
                                    });
                                    if (!fileExit) {
                                        $scope.appProperties.appChecklistFiles.push(file); //insert purpouse- suing this properties
                                        $scope.appProperties.appFileChecklistProperties.push({ fileName: file.name, url: "#" }); //in dispay in UI html page using this properties
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
        $scope.removeChecklistFiles = function (index, Title) {
            //Find the file using Index from Array.         
            if ($window.confirm("Do you want to remove ???" + Title)) {
                if ($scope.appProperties.ITServiceRequests.ID != null && $scope.appProperties.ITServiceRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.ITServiceRequests.ID).then(function (response) {
                        $scope.loaded = false; //spinner stop - service call end 
                        //Remove the item from Array using Index.
                        $scope.appProperties.appChecklistFiles.splice(index, 1);
                        $scope.appProperties.appFileChecklistProperties.splice(index, 1);
                    }).catch(function (error) {
                        //Remove the item from Array using Index.
                        $scope.appProperties.appChecklistFiles.splice(index, 1);
                        $scope.appProperties.appFileChecklistProperties.splice(index, 1);
                        $scope.loaded = false; //spinner stop - service call end 
                    });
                }
                else {
                    //Remove the item from Array using Index.
                    $scope.appProperties.appChecklistFiles.splice(index, 1);
                    $scope.appProperties.appFileChecklistProperties.splice(index, 1);
                }
            }
        };
        //insert files-to folder list
        function insertSupportingOfficeRenovationChkListDocuments(listItemId, message, redirectUrl) {
            if ($scope.appProperties.appChecklistFiles.length > 0) {
                var oList = web.get_lists().getByTitle(Config.OfficeRenovationChkListDocuments);
                uService.getDocLibByFolderName(listItemId, Config.OfficeRenovationChkListDocuments).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        //alread exit folder      
                        uploadChkListFloor(listItemId, $scope.appProperties.appChecklistFiles, redirectUrl, message, Config.OfficeRenovationChkListDocuments);
                    }
                    else {
                        // Folder doesn't exist at all. so create folder here
                        var oListItem = oList.get_rootFolder().get_folders().add(listItemId); //foldername used by PJApproval listItemID
                        ctx.load(oListItem);
                        ctx.executeQueryAsync(function () {
                            uploadChkListFloor(listItemId, $scope.appProperties.appChecklistFiles, redirectUrl, message, Config.OfficeRenovationChkListDocuments);
                        }, function (sender, args) {
                            console.log("something went wrong");
                            console.log('Err: ' + args.get_message());
                        });
                    }
                });
            }
        };
        function uploadChkListFloor(listItemId, fileInput, redirectUrl, message, docListName) {
            // Define the folder path for this example.
            // var targetUrl = _spPageContextInfo.webServerRelativeUrl + "/" + documentLibrary + "/" + listItemId;
            var serverRelativeUrlToFolder = _spPageContextInfo.siteServerRelativeUrl + '/' + docListName + '/' + listItemId;
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
                                // alert(message);
                                // $scope.loaded = false;
                                //window.location.href = redirectUrl;
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
                    "/_api/Web/lists/GetByTitle('" + Config.OfficeRenovationChkListDocuments + "')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'ITRequestsID': listItemId


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
            var clist = web.get_lists().getByTitle("" + Config.ITComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            if ($scope.appProperties.appComments.Comments != null && $scope.appProperties.appComments.Comments != "") {
                clistItem.set_item('UserComments', $scope.appProperties.appComments.Comments + " ( " + Config.ITCommentStatus[status] + " )");
            } else {
                clistItem.set_item('UserComments', Config.ITCommentStatus[status] + " (System Comments)");
            }
           // clistItem.set_item('UserComments', Config.ITCommentStatus[status] + " (System Comments)");
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('ITListItemID', listItemId);
           // clistItem.set_item('ITRequestsID', listItemId);
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
        function loadITServiceRequestsListData(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getById(ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ITServiceRequests = response.data.d.results[0];

                    $scope.appProperties.ITServiceRequests.Author.LoginName = $scope.appProperties.ITServiceRequests.RequestorLoginID;
                    $scope.appProperties.ITServiceRequests.RequestedFor.LoginName = $scope.appProperties.ITServiceRequests.RequestedForLoginID;
                    $scope.appProperties.ITServiceRequests.RequestedFor.Title = response.data.d.results[0].RequestedFor.Title;
                    //enable controls
                    ReqTypeHideShowControls($scope.appProperties.ITServiceRequests.RequestTypeCode);
                    //enable approval button
                    DisableHideShowControls($scope.appProperties.ITServiceRequests.WorkFlowStatus, $scope.appProperties.CurrentUser.Id, $scope.appProperties.ITServiceRequests.WorkFlowCode) //disable controls while load and approval status
                    //format-date
                    $scope.appProperties.ITServiceRequests.ApplicationDate = $filter('date')($scope.appProperties.ITServiceRequests.Created, 'dd/MM/yyyy') //created 
                    $scope.appProperties.ITServiceRequests.DateRequiredBy = $filter('date')($scope.appProperties.ITServiceRequests.DateRequiredBy, 'dd/MM/yyyy') //dateRequrie
                    if ($scope.appProperties.ITServiceRequests.OffRenoProjectStartDate != null && $scope.appProperties.ITServiceRequests.OffRenoProjectStartDate != undefined) {
                        $scope.appProperties.ITServiceRequests.OffRenoProjectStartDate = $filter('date')($scope.appProperties.ITServiceRequests.OffRenoProjectStartDate, 'dd/MM/yyyy') //proejctstartdate 
                    }
                    if ($scope.appProperties.ITServiceRequests.OffRenoProjectEndDate != null && $scope.appProperties.ITServiceRequests.OffRenoProjectEndDate != undefined) {
                        $scope.appProperties.ITServiceRequests.OffRenoProjectEndDate = $filter('date')($scope.appProperties.ITServiceRequests.OffRenoProjectEndDate, 'dd/MM/yyyy') //prjecednddate 
                    }
                    // checkbox result
                    if (response.data.d.results[0].OffRenoServices != null) {
                        $scope.SelectionCheckboxInformation = response.data.d.results[0].OffRenoServices.results;
                    }
                    // checkbox result
                    if (response.data.d.results[0].EmailServices != null) {
                        $scope.SelectionChkboxEmailServiceInfo = response.data.d.results[0].EmailServices.results;
                    }
                    // checkbox result
                    if (response.data.d.results[0].EmailInformation != null) {
                        $scope.appProperties.ITServiceRequests.EmailInformation = response.data.d.results[0].EmailInformation.results;
                    }
                    // checkbox result
                    if (response.data.d.results[0].InstallUpgradeSW != null) {
                        $scope.SelectionChkboxSotwareReqInfo = response.data.d.results[0].InstallUpgradeSW.results;
                    }
                    // checkbox result
                    if (response.data.d.results[0].LoginAccountType != null) {
                        $scope.SelectionChkboxLoginAccountTypeInfo = response.data.d.results[0].LoginAccountType.results;
                    }

                 
                    if ($scope.appProperties.ITServiceRequests.OnBehalf =="1") {
                        $scope.OnBehalf = true;

                    } else {
                        $scope.OnBehalf = false;
                    }
                    if ($scope.appProperties.ITServiceRequests.NewADAcc== "1") {
                        $scope.NewADAcc = true;
                    } else {
                        $scope.NewADAcc= false;
                    }
                    
                    $("#chkboxBehalf").prop('disabled', true);
                    //display staus on ui
                    $scope.appProperties.ITServiceRequests.DisplayStatus = Config.ITWFStatus[$scope.appProperties.ITServiceRequests.WorkFlowStatus]

                    //display in opopu reroute
                    if ($scope.IsITProjectManager)     
                    {
                        $scope.CheckboxDelegateUserInfo = ["Groupdirector", "CIO"];
                    } else {
                        $scope.CheckboxDelegateUserInfo = ["CIO"];
                    }
                }
            });
        };
        //ITReqdoc
        function loadFilesDocument(ListItemId, ListName) {
            $scope.loaded = true; //spinner start -service call start
            uService.getDocLibById(ListItemId, ListName).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.appFileProperties.push({ fileName: value.Name, url: value.ServerRelativeUrl }); //in dispay in UI html page using this properties
                    });
                }
            });
        };
        //InstallUpgradeSWDocuments
        function loadFilesInstallUpgradeSWDocuments(ListItemId, ListName) {
            $scope.loaded = true; //spinner start -service call start
            uService.getDocLibById(ListItemId, ListName).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.appFileInstallUpgradeSWDocumentsProperties.push({ fileName: value.Name, url: value.ServerRelativeUrl }); //in dispay in UI html page using this properties
                    });
                }
            });
        };
        //FloorPlanFiles
        function loadFileFloorDocuments(ListItemId, ListName) {
            $scope.loaded = true; //spinner start -service call start
            uService.getDocLibById(ListItemId, ListName).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.appFileFloorPlanProperties.push({ fileName: value.Name, url: value.ServerRelativeUrl }); //in dispay in UI html page using this properties
                    });
                }
            });
        };
        //appChecklistFiles
        function loadChekListDocument(ListItemId, ListName) {
            $scope.loaded = true; //spinner start -service call start
            uService.getDocLibById(ListItemId, ListName).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.appFileChecklistProperties.push({ fileName: value.Name, url: value.ServerRelativeUrl }); //in dispay in UI html page using this properties
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
        function ReqTypeHideShowControls(ReqTypeCode) {
            $scope.IsEmailService = false;
            $scope.IsUserLoginAccount = false;
            $scope.IsIntranetLaptop = false;
            $scope.IsITSoftwareRemoval = false;
            $scope.IsInstallUpgrade = false;
            $scope.IsOfficeRenovation = false;
            $scope.IsINSTRUCTIONS = false;
            
            if (ReqTypeCode == "ITSEMAILSERVICE") {
                $scope.IsEmailService = true;
            }
            else if (ReqTypeCode == "ITSUSRLGNACC") {
                $scope.IsUserLoginAccount = true;
            }
            else if (ReqTypeCode == "HWPINTRALAP" || ReqTypeCode =="HWLINTRALAP") {
                $scope.IsIntranetLaptop = true;
            }
            else if (ReqTypeCode == "SWRMV") {
                $scope.IsITSoftwareRemoval = true;
            }
            else if (ReqTypeCode == "SWINTLUPGD") {
                $scope.IsInstallUpgrade = true;
            }
            else if (ReqTypeCode == "ITSOFFRNV") {
                $scope.IsOfficeRenovation = true;
            }
            else if (ReqTypeCode == "ITSPRFSRV") {
                $scope.IsINSTRUCTIONS = true;
            }
            
            else { }
        };
        function DisableHideShowControls(status, currentUsrId, ReqTypeCode) {
            //initally disble/show 
            $scope.IsBranchHeadDisabled = false; // disbale true        
            $scope.IsFinanceHeadDisabled = false;
            //manager status- button  -disable             
            $scope.IsBranchHeadStatusDisabled = false;
            $scope.IsFinanceHeadStatusDisabled = false;
            //while submit and save disable date field and button
            $scope.IsSubmitButton = false;
            $scope.IsSApproveButton = false;
            $scope.IsRejectButton = false;
            $scope.IsSubmitMode = false; // using this while submit button enable control hide   
            $scope.IsAfterSubmitDisabled = true; // using this while submit button enable control show   
            $scope.IsReRouteButton = false;
            $scope.IsUserReRouteButton = false;
            $scope.IsCommentsSection = true;
            $scope.IsUserReRouteIMNOButton = false;
            //saved mode
            if (status == "New" && (currentUsrId == $scope.appProperties.ITServiceRequests.Author.Id)) {
                $scope.IsSubmitButton = true;
                $scope.IsCommentsSection = false;      
                 $scope.IsAfterSubmitDisabled = false;
            }
            //re-rote
            else if ((status == "19" || status == "20") && (currentUsrId == $scope.appProperties.ITServiceRequests.Author.Id)) {
                //enable re-rourte
                $scope.IsSubmitButton = true;
                $scope.IsAfterSubmitDisabled = false;
                $scope.IsCommentsSection = true;
            }
            //Branch head  approval
            else if ((status == "1" || status == "21") && (currentUsrId == $scope.appProperties.ITServiceRequests.BranchHead.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            // After Branch head  approval- enable itmanager Head
            else if ((status == "2" || status == "21") && ($scope.IsITProjectManager && (ReqTypeCode == "Dynamic"))) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsUserReRouteButton = true;
                $scope.IsRejectButton = true;
                $scope.IsReRouteButton = true;
            }
            else if ((status == "2") && ($scope.IsImplementationOfficer && (ReqTypeCode == "Dynamic"))) {
                //enable 
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                $scope.IsUserReRouteIMNOButton = true;
            }
            else if (((status == "15") && (currentUsrId == $scope.appProperties.ITServiceRequests.CIO.Id && (ReqTypeCode == "Dynamic")))) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                $scope.IsReRouteButton = false;
            }
            // After ITProjectManager head  approval- enable GroupDirector Head
            else if (((status == "4" || status == "21" || status == "14") && (currentUsrId == $scope.appProperties.ITServiceRequests.GroupDirector.Id && (ReqTypeCode == "Dynamic")))) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = false;
                $scope.IsRejectButton = true;
                $scope.IsReRouteButton = true;
            }
            // After Branch head  approval- enable itmanager Head
            else if ((status == "2" || status == "21") && ($scope.IsITProjectManager && (ReqTypeCode == "Multi"))) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                $scope.IsUserReRouteButton = true;
            }
            // After ITProjectManager head  approval- enable GroupDirector Head
            else if ((status == "4" || status == "21" || status == "14") && (currentUsrId == $scope.appProperties.ITServiceRequests.GroupDirector.Id && (ReqTypeCode == "Multi"))) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            // After Branch head  approval- enable itmanager Head
            else if ((status == "2" || status == "21") && (currentUsrId == $scope.IsImplementationOfficer && ReqTypeCode == "Single")) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            // After GroupDirector head  approval- enable CIO Head
            else if ((status == "6" || status == "21" || status == "17") && (currentUsrId == $scope.appProperties.ITServiceRequests.CIO.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            // After GroupDirector head  approval- enable CIO Head
            else if ((status == "8" || status == "13" || status == "21" || status == "18") && ($scope.IsImplementationOfficer)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                $scope.IsUserReRouteIMNOButton = true;
            }
            // Rejected status and not equal currentuser and approver (disable/Hide all control)
            else {
                //initailly alardy disbles
            }
        };      
        $scope.SelectionChkboxDelegateUserInfo = [];
        // Toggle selection for a given chkbox by name
        $scope.toggleSelectionChkboxDelegateUser = function toggleSelectionChkboxDelegateUser(information) {
            var idx = $scope.SelectionChkboxDelegateUserInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxDelegateUserInfo.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxDelegateUserInfo.push(information);
            }
        };
        //javascript methods---------------------------------------------------------------------------------------- end
        //Show Model windows
        $scope.showModal = function (listItemId) {

            $uibModal.open({
                templateUrl: 'myModal.html',
                controller: 'ModalDialogController',
                resolve: {
                    Id: function () { return listItemId; },
                    items: function () {
                        return $scope.appProperties;
                    },
                }
            })
                .result.then(
                    function () {
                        //alert("OK");
                    },
                    function () {
                        // alert("Cancel");
                    }
                );
        };
    }]).controller("ModalDialogController", function ($scope, $uibModalInstance, Config, Id, items) {
    //init
    //sharepoint context
    var ctx = new SP.ClientContext.get_current();
    var web = ctx.get_web();
    $scope.appChildProperties = ({
        //Current User Properties    
        DelegateUserItem: "",
        CurrentUser: ({ Id: "", Email: "", Title: "" }),
        DelegateUser: ({ Id: "", EMail: "", Title: "" }),
        DelegateManagerMaster: ({ Id: "", EMail: "", Title: "" })
    });
    if (items.ITServiceRequests.ITProjectManager.Id == items.CurrentUser.Id) {
        $scope.CheckboxDelegateUserInfo = ["Groupdirector", "CIO"];
    } else {
        $scope.CheckboxDelegateUserInfo = ["CIO"];
    }
    $scope.SelectionChkboxDelegateUserInfo = [];
    // Toggle selection for a given chkbox by name
    $scope.toggleSelectionChkboxDelegateUser = function toggleSelectionChkboxDelegateUser(information) {
        var idx = $scope.SelectionChkboxDelegateUserInfo.indexOf(information);
        // Is currently selected
        if (idx > -1) {
            $scope.SelectionChkboxDelegateUserInfo.splice(idx, 1);
        }
        // Is newly selected
        else {
            $scope.SelectionChkboxDelegateUserInfo.push(information);
        }
    };

    //assing parent value to child        
    $scope.submitITServiceRequest = function (type, $event, form) {

        var cmntStaus = 0;
        if ($scope.SelectionChkboxDelegateUserInfo.length == 1) {
            //update list
            if (items.ITServiceRequests.ITProjectManager.Id == items.CurrentUser.Id) {
                cmntStaus = 12
                if ($scope.SelectionChkboxDelegateUserInfo[0] == "CIO") {
                    items.ITServiceRequests.CIOStatus = "Pending";
                    items.ITServiceRequests.WorkFlowStatus = 15; //ITManager Re-Routed pending CIO
                }
                else {
                    items.ITServiceRequests.GroupDirectorStatus = "Pending";
                    items.ITServiceRequests.WorkFlowStatus = 14; //ITManager Re-Routed pending GroupDirector
                }
                items.ITServiceRequests.ITProjectManagerDate = new Date();
                items.ITServiceRequests.ITProjectManagerStatus = "ReRouted";
                items.ITServiceRequests.ApplicationStatus = 2; //inprogress
            }
            if (items.ITServiceRequests.GroupDirector.Id == items.CurrentUser.Id) {
                cmntStaus = 13
                if ($scope.SelectionChkboxDelegateUserInfo[0] == "CIO") {
                    items.ITServiceRequests.GroupDirectorDate = new Date();
                    items.ITServiceRequests.GroupDirectorStatus = "ReRouted";
                    items.ITServiceRequests.CIOStatus = "Pending";
                    items.ITServiceRequests.WorkFlowStatus = 17; //Group Director Re-Routed pending CIO
                }
            }
            insertUpdateListItem(items, cmntStaus, $scope.SelectionChkboxDelegateUserInfo[0]);
        }
        else {
            alert("Please select delegate approver ITManager or CIO !..")
        }

        $uibModalInstance.close();
    };

    $scope.cancelModal = function () {
        $uibModalInstance.dismiss('cancel');
    };

});
}) ();