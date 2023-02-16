"use strict";
(function () {
    app.controller("CRSRRequestController", ["$scope", "CRSRService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
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

        var fromEmail = "";
        var dispName = "";
        var bodyMsg = "";
         //end config
        //init variable
        var isSiteAdmin = false;
        $scope.isAdminGroup = false;
        $scope.IsLoginUserDivisonHead = false; //init
        $scope.IsLoginUserBranchHead = false; //init
        //check required validation
        $scope.appProperties = [];
        $scope.appProperties = ({
            //List Properties-exact columnname on CRSR(save/edit) and additional properties for ui purpose
            CRSR: ({
                ID: "",
                Title: "",               
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker
                BranchHead: ({ Id: "", EMail: "", Title: "" }),
                RequestedFor: ({ Id: "", EMail: "", Title: "" }),
                SystemOwner: ({ Id: "", EMail: "", Title: "" }),
                ITProjectManager: ({ Id: "", EMail: "", Title: "" }),
                FundingAuthority: ({ Id: "", EMail: "", Title: "" }),    
                CIO: ({ Id: "", EMail: "", Title: "" }),  
                //date
                SubmittedDate: null,               
                BranchHeadDate: null,
                SystemOwnerDate: null,
                ITProjectManagerDate: null,
                FundingAuthorityDate: null,
                //FundingAuthorityDate: null,
                //status              
                BranchHeadStatus: "Pending",//set default
                SystemOwnerStatus: "",
                ITProjectManagerStatus: "",
                FundingAuthorityrStatus: "",
                //CIOStatus: "Pending",
                
                //others
                ApplicationID: "0",
                WorkFlowStatus: "", // use this code to level of workflow
                RequestTypeCode: "", //use this code 
                WorkFlowCode:"",
                IndicateAccountNo: "",          
                
                                      
                Module: "",
                ExistingSituation: "",
                ProposedChanges: "",
                Justification: "",
                
                EstimatedEffort: "",
                EstimatedCost: "",
                TypeofRequest: "",
                ProjectSystem:"",
                FundCentre: "",
                CostCentre: "",
               
                UATRemarks: "", 
                OtherReasonforRequest:"",
                OtherUATStatus:"",
                ProductionDataPatchRemarks: "",
                DateRequiredBy: null,
                ExpectedCompletionDate: null,
                ProductionDataPatchsignoff: null,
                ApplicationDate: $filter('date')(new Date(), 'dd/MM/yyyy'), //dd-MM-yyyy hh:mm a   
                ApplicationStatus: "New",
                DisplayStatus: "New", //UI purpose
                //-chekcbox list column 
                ReasonforRequest: ({ results: [] }),
                Chargeable: ({ results: [] }),
                ExpectedCompletionDatefor: ({ results: [] }),
                UATStatus: ({ results: [] })
            }), 
           
            //Master-List Properties
            ProjectSystemMaster: ({ ID: "", Title: "" }),  
            FundCentreMaster: ({ ID: "", Title: "" }), 
            CostCentreMaster: ({ ID: "", Title: "" }), 
            //Approvers
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            FundingAuthorityMaster: ({ Id: "", EMail: "", Title: "" }), 
            ITProjectManagerMaster: ({ Id: "", EMail: "", Title: "" }),           
            SystemOwnerMaster: ({ Id: "", EMail: "", Title: "" }),
           
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
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName:""}),
        });
        //EmailService-checkbox
        $scope.CheckboxReasonforRequestInfo = ["New items – Audit Observation/Corporate Governance Compliance", "New items – Update/review of Statutory Policies & Procedures", "New items – Operational process improvement", "New items – Usage improvement", "New items – Aesthetic improvement", "Missed out in system design specs baseline", "Data Patch", "Ad-hoc Report","Others"];
        $scope.SelectionChkboxReasonforRequestInfo = [];
        //Login Account Type -checkbox
        $scope.CheckboxChargeableInfo = ["Yes", "No"]; // not in use -changed now 
        $scope.entities = [{
            name: 'Yes',
            checked: false
        }, {
            name: 'No',
            checked: false
        }
        ];
        $scope.SelectionChkboxChargeableInfo = [];
        //ExpectedCompletionDatefor
        $scope.CheckboxExpectedCompletionDateforInfo = ["User Acceptance Test (UAT) required", "Data Patch required"];
        $scope.SelectionChkboxExpectedCompletionDateforInfo = [];
        //UAt Status Type -checkbox
        $scope.CheckboxUATStatusInfo = ["Pass", "Fail","Others"];
        $scope.SelectionCheckboxUATStatusInfo = [];
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
                    $scope.IsRequestedReq = false; //cchk ondition
                    $('#userID').val(userData.Key.split('\\')[1]);
                    $scope.appProperties.CRSR.RequestedFor.LoginName = userData.Key.split('\\')[1];
                    // Get the first user's ID by using the login name.
                    getUserId(userData.Key).done(function (user) {
                        $scope.appProperties.CRSR.RequestedFor.Id = user.d.Id;
                    });
                    $scope.$apply();
                } else {
                    $scope.appProperties.CRSR.RequestedFor.Id = "";
                    $scope.IsRequestedReq = true;
                    $scope.$apply();
                }
            };           
            //load function
            getCurrentLoginUsersAndBindAllData(); //get current usser - After success call loading data               
            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['CRSRAdmin'];
                //determine wether current user is a admin of group(s) 
                var userGroups = result.data.d.Groups.results;
                var foundGroups = userGroups.filter(function (g) { return groupNames.indexOf(g.LoginName) > -1 });
                if (foundGroups.length > 0) {
                    $scope.isAdminGroup = true;                  
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
        // Check checkbox checked or not
        $scope.OnBehalf = false;
        $scope.checkVal = function () {
            if ($scope.OnBehalf) {
                $scope.appProperties.CRSR.OnBehalf = "1";
                $scope.OnBehalf = true;
                if ($scope.appProperties.CRSR.RequestedFor.Id == "") {
                    $scope.IsRequestedReq = true;
                } else { $scope.IsRequestedReq = false; }
            } else {
                $scope.IsRequestedReq = false;
                $scope.OnBehalf = false;
                $scope.appProperties.CRSR.OnBehalf = "0";
            }
        }
        //submit request
        $scope.Ismandtry = false;
        $scope.CostCentreIsmandtry = false;
        $scope.EstimatedCostIsmandtry = false;
        $scope.EstimatedEffortIsmandtry = false;
        $scope.FundCentreIsmandtry = false;
        $scope.IsmandtryCompletionDatefor = false;
        $scope.IsmandtryUATStatusInfo = false;
        $scope.IsmandtryReasonforRequestInfo = false;
        $scope.FundingAuthorityIsmandtry = false;
        $scope.submitCRSRRequest = function (type, $event, Form) {
            $scope.loaded = true; //automate false while redirect page
            $event.preventDefault(); 
            var appStatus = "0"; //maintain applciation field
            var cmtStatus = "0"; //maintain coments field
            //init validation false 
            $scope.Ismandtry = false;
            $scope.CostCentreIsmandtry = false;
            $scope.EstimatedCostIsmandtry = false;
            $scope.EstimatedEffortIsmandtry = false;
            $scope.FundCentreIsmandtry = false;
            $scope.IsmandtryCompletionDatefor = false;
            $scope.IsmandtryUATStatusInfo = false;
            $scope.IsmandtryReasonforRequestInfo = false;
            $scope.chargebleIsmandtry = false;
            $scope.FundingAuthorityIsmandtry = false;
            switch (type) {             
                case 1:

                    if (Form.ddlRequestType.$invalid || Form.ProjectSystem.$invalid || Form.ExpectedCompletionDate.$invalid 
                        || Form.Justification.$invalid || Form.Module.$invalid
                        || Form.uatRemarkstxt.$invalid) {
                        if (Form.ddlRequestType.$invalid) Form.ddlRequestType.$touched = true;
                        if (Form.ProjectSystem.$invalid) Form.ProjectSystem.$touched = true;
                        if (Form.ExpectedCompletionDate.$invalid) Form.ExpectedCompletionDate.$touched = true;
                        if (Form.ProposedChanges.$invalid) Form.ProposedChanges.$touched = true;
                        if (Form.Justification.$invalid) Form.Justification.$touched = true;                        
                        if (Form.Module.$invalid) Form.Module.$touched = true;
                        // if (Form.ProductionDataPatchsignoff.$invalid) Form.ProductionDataPatchsignoff.$touched = true;
                        // if (Form.ProductionDataPatchRemarks.$invalid) Form.ProductionDataPatchRemarks.$touched = true;
                        if (Form.uatRemarkstxt.$invalid) Form.uatRemarkstxt.$touched = true;                     
                        $scope.Ismandtry = true;
                    }
                    if ($scope.SelectionChkboxReasonforRequestInfo.length > 1 || $scope.SelectionChkboxReasonforRequestInfo.length == 0) {
                     
                      //  alert("Select any one option from Reason for Requests");
                        $scope.Ismandtry =   $scope.IsmandtryReasonforRequestInfo = true;
                    }
                    if ($scope.SelectionChkboxExpectedCompletionDateforInfo.length > 1 || $scope.SelectionChkboxExpectedCompletionDateforInfo.length == 0) {
                        $scope.loaded = false;
                       // alert("Select any one option from Expected Completion Date for");
                        $scope.Ismandtry = $scope.IsmandtryCompletionDatefor = true;
                       
                    }
                    if ($scope.SelectionCheckboxUATStatusInfo.length > 1 || $scope.SelectionCheckboxUATStatusInfo.length == 0) {
                   
                        //alert("UAT Status required and select any one");
                        $scope.Ismandtry =  $scope.IsmandtryUATStatusInfo = true;
                      
                    }
                    var cont = 0;
                    var funds = false;
                    angular.forEach($scope.entities, function (value, key) {
                        if (value.checked == false) {
                            cont += 1;
                        }
                        if (value.checked == true && value.name == "Yes") {                          
                            funds = true;                          
                            if ($scope.appProperties.CRSR.EstimatedEffort == "") {
                                $scope.Ismandtry = $scope.EstimatedEffortIsmandtry = true;
                            }
                            if ($scope.appProperties.CRSR.EstimatedCost == "") {
                                $scope.Ismandtry = $scope.EstimatedCostIsmandtry = true;
                            }
                            if ($scope.appProperties.CRSR.FundCentre == "") {
                                $scope.Ismandtry = $scope.FundCentreIsmandtry = true;

                            }
                            if ($scope.appProperties.CRSR.CostCentre == "") {
                                $scope.Ismandtry =  $scope.CostCentreIsmandtry = true;
                            }                          
                        }
                        if (value.checked == true && value.name == "No") {
                            $scope.EstimatedEffortIsmandtry = false;
                            $scope.EstimatedCostIsmandtry = false;
                            $scope.FundCentreIsmandtry = false;
                            $scope.CostCentreIsmandtry = false;
                            $scope.FundingAuthorityIsmandtry = false;

                        }
                       
                    });
                    if (cont == 2) {
                        // alert("Chargeable Info required and select any one");
                        $scope.Ismandtry = $scope.chargebleIsmandtry = true;
                    }
                    if (funds) {
                        if ($scope.appProperties.CRSR.FundingAuthority.Id == "") {
                            $scope.FundingAuthorityIsmandtry = true;
                            $scope.Ismandtry = true;
                            $scope.loaded = false;                           
                        }
                    }
                    if ($scope.appProperties.CRSR.OnBehalf == "1") {
                        if ($scope.appProperties.CRSR.RequestedFor.Id == "") {
                            $scope.IsRequestedReq = true;
                            $scope.Ismandtry = true;
                        } else { $scope.IsRequestedReq = false; }
                    }
                    if ($scope.Ismandtry) {
                        $scope.loaded = false;
                        return;
                    }

                    type = 1 // submitted
                    appStatus = 1; //inprogress
                    cmtStatus = 1;// submitted by user
                    insertUpdateListItem(type, appStatus, cmtStatus, "Submitted Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus         
                    // code block
                    break;    
                case 2:
                    //date -approve/rejecte date assing in disabled function
                       //Branch head  approval
                    if ($scope.appProperties.CRSR.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.BranchHead.Id) {
                        //enable only BranchHead-approve/rejectbutton
                        if ($scope.appProperties.CRSR.BranchHeadDate == null || $scope.appProperties.CRSR.BranchHeadDate == undefined) {
                            $scope.appProperties.CRSR.BranchHeadDate = new Date();
                            $scope.appProperties.CRSR.ITProjectManagerStatus = "Pending";
                            $scope.appProperties.CRSR.BranchHeadStatus = "Approved";
                            type = 2 // Approved by BranchHead
                            appStatus = 2; //inprogress
                            cmtStatus = 2;// Approved by BranchHead
                            //sendEmail to approver
                            
                            if ($scope.appProperties.ITProjectManagerMaster.length > 0) {

                                //Utility.helpers.sendEmail(urlTemplate, $scope.appProperties.ITProjectManagerMaster[0].EMail, null, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                            }  
                        }
                    }
                    //ITProjectManager approval-open after branch head approved
                    else if ($scope.appProperties.CRSR.BranchHeadStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.ITProjectManager.Id) {
                        if ($scope.appProperties.CRSR.ITProjectManagerDate == null && $scope.appProperties.CRSR.ITProjectManagerDate == undefined) {
                            $scope.appProperties.CRSR.ITProjectManagerDate = new Date();
                            $scope.appProperties.CRSR.SystemOwnerStatus = "Pending";
                            $scope.appProperties.CRSR.ITProjectManagerStatus = "Approved";
                            type = 4// Approved by ITManager
                            appStatus = 2; //inprogress
                            cmtStatus = 4;// Approved by ITManager
                            //sendEmail to approver
                            
                            if ($scope.appProperties.SystemOwnerMaster.length > 0) {

                                //Utility.helpers.sendEmail(urlTemplate, $scope.appProperties.SystemOwnerMaster[0].EMail, null, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                            }
                        }
                    }
                    //SystemOwner approval-open after ITProjectManager approval
                    else if ($scope.appProperties.CRSR.ITProjectManagerStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.SystemOwner.Id) {
                        $scope.appProperties.CRSR.SystemOwnerDate = new Date();
                        $scope.appProperties.CRSR.FundingAuthorityStatus = "Pending";
                        $scope.appProperties.CRSR.SystemOwnerStatus = "Approved";
                        type = 6// Approved by SystemOwner
                        appStatus = 2; //inprogress
                        cmtStatus = 6;// Approved by SystemOwner
                        if ($scope.appProperties.CRSR.FundingAuthority.Id) {

                            //Utility.helpers.sendEmail(urlTemplate, $scope.appProperties.CRSR.FundingAuthority.EMail, null, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                            //Utility.helpers.sendEmail(urlTemplate, $scope.appProperties.CurrentUser.Email, null, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                        }
                    }
                    //funding approval-open after ITProjectManager approval
                    else if ($scope.appProperties.CRSR.SystemOwnerStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.FundingAuthority.Id) {
                        $scope.appProperties.CRSR.FundingAuthorityDate = new Date();
                        $scope.appProperties.CRSR.closedStatus = "Pending";
                        $scope.appProperties.CRSR.FundingAuthorityStatus = "Approved";                      
                        type = 8;// Approved by FundingAuthority
                        appStatus = 3; //Completed
                        cmtStatus = 8;// Approved by FundingAuthority     
                    }  
                    //final closed
                    else if ($scope.appProperties.CRSR.FundingAuthorityStatus == "Approved" && ($scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.Author.Id || $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.ITProjectManager.Id)) {
                        $scope.appProperties.CRSR.closedDate = new Date();
                        $scope.appProperties.CRSR.closedStatus = "Approved";                    
                        type = 11; // Closed by user
                        appStatus = 5; //closed
                        cmtStatus = 17;// Closed by user    
                    }  
                    else { }
                    insertUpdateListItem(type, appStatus, cmtStatus, "Approved Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    // Approved block
                    break; 
                case 3: //Rejected section     
                    //date -approve/rejecte date assing in disabled function
                    if ($scope.appProperties.appComments.Comments != "" && $scope.appProperties.appComments.Comments != undefined) {

                        if ($scope.appProperties.CRSR.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.BranchHead.Id) {
                            //enable only BranchHead-approve/rejectbutton
                            $scope.appProperties.CRSR.BranchHeadDate = new Date();
                            $scope.appProperties.CRSR.BranchHeadStatus = "Rejected";
                            type = 3; // Rejected by BranchHead
                            appStatus = 4; //Rejected 
                            cmtStatus = 3;// Rejected by BranchHead
                        }
                        else if ($scope.appProperties.CRSR.BranchHeadStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.ITProjectManager.Id) {
                            //enable only ITProjectManager-approve/rejectbutton
                            $scope.appProperties.CRSR.ITProjectManagerDate = new Date();
                            $scope.appProperties.CRSR.ITProjectManagerStatus = "Rejected";
                            type = 5; // Rejected by ITManager
                            appStatus = 4; //Rejected 
                            cmtStatus = 5;// Rejected by ITManager                     
                        }
                        else if ($scope.appProperties.CRSR.ITProjectManagerStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.SystemOwner.Id) {
                            //enable only BranchHead-approve/rejectbutton                      
                            $scope.appProperties.CRSR.SystemOwnerDate = new Date();
                            $scope.appProperties.CRSR.SystemOwnerStatus = "Rejected";
                            type = 7; // Rejected by SystemOwner
                            appStatus = 4; //Rejected 
                            cmtStatus = 7;// Rejected by SystemOwner
                        }
                        else if ($scope.appProperties.CRSR.SystemOwnerStatus == "Approved" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.CRSR.FundingAuthority.Id) {
                            //enable only BranchHead-approve/rejectbutton                      
                            $scope.appProperties.CRSR.FundingAuthorityDate = new Date();
                            $scope.appProperties.CRSR.FundingAuthorityStatus = "Rejected";
                            type = 9; // Rejected by FundingAuthority
                            appStatus = 4; //Rejected 
                            cmtStatus = 9;// Rejected by FundingAuthority
                        }                      
                        insertUpdateListItem(type, appStatus, cmtStatus, "Rejected Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                        // Rejected block
                    }
                    else {
                        alert('Please enter rejection comments.');
                        $scope.loaded = false; //spinner stop - service call end
                        return;
                    }
                    break; 
                case 0:
                    //Close
                    window.location.href = siteAbsoluteUrl +"/Pages/CRSRDashboard.aspx";
                    // code block                  
                    break;
                default:
                // code block
            }
        };
        
        // Toggle selection for a given chkbox by name
        $scope.toggleSelectionChkboxExpectedCompletionDateforInfo = function toggleSelectionChkboxExpectedCompletionDateforInfo(information) {
            var idx = $scope.SelectionChkboxExpectedCompletionDateforInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxExpectedCompletionDateforInfo.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxExpectedCompletionDateforInfo.push(information);
            }

            if ($scope.SelectionChkboxExpectedCompletionDateforInfo.length > 1 || $scope.SelectionChkboxExpectedCompletionDateforInfo.length == 0) {

                //alert("UAT Status required and select any one");
                $scope.IsmandtryCompletionDatefor = true;

            }
            else { $scope.IsmandtryCompletionDatefor = false; }
        };

        // Toggle selection for a given chkbox by name
        $scope.toggleSelectionChkboxChargeableInfo = function toggleSelectionChkboxChargeableInfo(information) {
            var idx = $scope.SelectionChkboxChargeableInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxChargeableInfo.splice(idx, 1);
            }
            // Is newly selected
            else {
               
                $scope.SelectionChkboxChargeableInfo.push(information);
            }
        };
        $scope.IsEnabledReasonforRequestOthers = false;
        $scope.IsEnabledUATOthers = false;
        $scope.toggleCheckboxReasonforRequestInfoSelection = function toggleCheckboxReasonforRequestInfoSelection(information) {
            var idx = $scope.SelectionChkboxReasonforRequestInfo.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionChkboxReasonforRequestInfo.splice(idx, 1);
                if (information == "Others") {
                    $scope.IsEnabledReasonforRequestOthers = false;
                    $scope.appProperties.CRSR.OtherReasonforRequest = "";
                }
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxReasonforRequestInfo.push(information);
                if (information == "Others") {
                    $scope.IsEnabledReasonforRequestOthers = true;    
                }
            }
            if ($scope.SelectionChkboxReasonforRequestInfo.length > 1 || $scope.SelectionChkboxReasonforRequestInfo.length == 0) {

                //alert("UAT Status required and select any one");
                $scope.IsmandtryReasonforRequestInfo = true;

            }
            else { $scope.IsmandtryReasonforRequestInfo = false; }
        };
        $scope.toggleSelectionCheckboxUATStatusInfo = function toggleSelectionCheckboxUATStatusInfo(information) {
           
            var idx = $scope.SelectionCheckboxUATStatusInfo.indexOf(information);          
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionCheckboxUATStatusInfo.splice(idx, 1);
                if (information == "Others") {
                    $scope.IsEnabledUATOthers = false;
                    $scope.appProperties.CRSR.OtherUATStatus = "";
                }                
            }
            // Is newly selected
            else {
                $scope.SelectionCheckboxUATStatusInfo.push(information);
                if (information == "Others") {
                    $scope.IsEnabledUATOthers = true;
                } 
            }

            if ($scope.SelectionCheckboxUATStatusInfo.length > 1 || $scope.SelectionCheckboxUATStatusInfo.length == 0) {

                //alert("UAT Status required and select any one");
                $scope.IsmandtryUATStatusInfo = true;

            }
            else { $scope.IsmandtryUATStatusInfo = false; }
           
        };
       
       
        $scope.IsfundAuthoirty = false; 
        var chkbool = false;
        $scope.updateSelection = function (position, entities) {
            angular.forEach(entities, function (subscription, index) {
                if (position != index) {
                    subscription.checked = false;                   
                }
               
            });

            var contx = 0;
            $scope.IsfundAuthoirty = false; 
            var fundsx = false;
            angular.forEach($scope.entities, function (value, key) {
                if (value.checked == false) {
                    contx += 1;
                } 
                if (value.checked == true && value.name == "Yes") {
                    fundsx = true;
                    $scope.IsfundAuthoirty = true;  
                    if ($scope.appProperties.CRSR.EstimatedEffort == "") {
                        $scope.EstimatedEffortIsmandtry = true;
                    }
                    if ($scope.appProperties.CRSR.EstimatedCost == "") {
                        $scope.EstimatedCostIsmandtry = true;
                    }
                    if ($scope.appProperties.CRSR.FundCentre == "") {
                        $scope.FundCentreIsmandtry = true;

                    }
                    if ($scope.appProperties.CRSR.CostCentre == "") {
                        $scope.CostCentreIsmandtry = true;
                    }
                }
                if (value.checked == true && value.name == "No") {
                    $scope.EstimatedEffortIsmandtry = false;
                    $scope.EstimatedCostIsmandtry = false;
                    $scope.FundCentreIsmandtry = false;
                    $scope.CostCentreIsmandtry = false;
                    $scope.FundingAuthorityIsmandtry = false;

                }

            });
            if (fundsx) {
                if ($scope.appProperties.CRSR.FundingAuthority.Id == "") {
                    $scope.FundingAuthorityIsmandtry = true;                 
                }
            }


            $scope.chargebleIsmandtry = false;
            if (contx == 2) { //none selection

                $scope.chargebleIsmandtry = true;
                //false
                $scope.EstimatedEffortIsmandtry = false;
                $scope.EstimatedCostIsmandtry = false;
                $scope.FundCentreIsmandtry = false;
                $scope.CostCentreIsmandtry = false;
                $scope.FundingAuthorityIsmandtry = false;

            }
           
            
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
                $scope.appProperties.CRSR.Author.Id = result.data.d.Id; //while init 
                $scope.appProperties.CRSR.Author.LoginName = result.data.d.Title; //while init                
                $scope.appProperties.CRSR.Author.Title = result.data.d.Title; //while init 
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];
                    getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);                   
                }
                else {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName;//result.data.d.Title.split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.Title.split("\\")[0];
                    getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, result.data.d.LoginName); //chk only  
                }
                getDivisionHeadByGroupDir($scope.appProperties.CurrentUser.ADID); //get group dir from DivisionHeads list 
                //all time load function   
                bindAuthorization(); // 
                bindAllMasterList();
                chkLoginUserIsBrachHeadorDivHead($scope.appProperties.CurrentUser.ADID); //chk user bracnhhead or divison head
                //get query string value
                $scope.appProperties.CRSR.ID = Utility.helpers.getUrlParameter('ReqId');
                if ($scope.appProperties.CRSR.ID != "" && $scope.appProperties.CRSR.ID != undefined) {
                    loadCRSRListData($scope.appProperties.CRSR.ID);//Load data     
                    //load doucments
                    loadFilesDocument($scope.appProperties.CRSR.ID, Config.CRSRDocuments); //getITRequestDoc                                     
                    //load comments
                    loadCommentsListData($scope.appProperties.CRSR.ID); //PriceComparison  
                }
                else {
                    //new req
                    getUniqueNumber(); //generate unique number-only for new Req   
                    DisableHideShowControls("New", $scope.appProperties.CurrentUser.Id) //init stage
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
        //insert/update CRSR 
        function insertUpdateListItem(Worfkflowstatus, ApplicationStatus, commentStatus, message) {            
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.CRSR + "");
            var listItem = "";
            if ($scope.appProperties.CRSR.ID == "" || $scope.appProperties.CRSR.ID == undefined) {
                //insert data to sharepoint
                var listCreationInformation = new SP.ListItemCreationInformation();
                listItem = list.addItem(listCreationInformation);
                listItem.set_item("RequestorLoginID", $scope.appProperties.CurrentUser.LoginName);

            } else {

                listItem = list.getItemById($scope.appProperties.CRSR.ID);
            }
            //assign properties while insert/update
            listItem.set_item("Title", $scope.appProperties.CRSR.ApplicationID);
            listItem.set_item("ApplicationID", $scope.appProperties.CRSR.ApplicationID);            
            listItem.set_item("TypeofRequest", $scope.appProperties.CRSR.TypeofRequest);           
            listItem.set_item("ReasonforRequest", $scope.appProperties.CRSR.ReasonforRequest);
            listItem.set_item("Module", $scope.appProperties.CRSR.Module);
            listItem.set_item("ExistingSituation", $scope.appProperties.CRSR.ExistingSituation);
            listItem.set_item("ProposedChanges", $scope.appProperties.CRSR.ProposedChanges); 
            listItem.set_item("UATStatus", $scope.appProperties.CRSR.UATStatus);
            listItem.set_item("UATRemarks", $scope.appProperties.CRSR.UATRemarks);
            listItem.set_item("Chargeable", $scope.appProperties.CRSR.Chargeable);
            listItem.set_item("CostCentre", $scope.appProperties.CRSR.CostCentre);
            listItem.set_item("Justification", $scope.appProperties.CRSR.Justification); 
            listItem.set_item("FundCentre", $scope.appProperties.CRSR.FundCentre);
            listItem.set_item("EstimatedEffort", $scope.appProperties.CRSR.EstimatedEffort); 
            listItem.set_item("EstimatedCost", $scope.appProperties.CRSR.EstimatedCost);
            listItem.set_item("ProductionDataPatchRemarks", $scope.appProperties.CRSR.ProductionDataPatchRemarks);           
            listItem.set_item("ProjectSystem", $scope.appProperties.CRSR.ProjectSystem);
            listItem.set_item("OtherReasonforRequest", $scope.appProperties.CRSR.OtherReasonforRequest);
            listItem.set_item("OtherUATStatus", $scope.appProperties.CRSR.OtherUATStatus);
            //new status
            listItem.set_item("SystemOwnerStatus", $scope.appProperties.CRSR.SystemOwnerStatus);
         
            listItem.set_item("BranchHeadStatus", $scope.appProperties.CRSR.BranchHeadStatus);
            listItem.set_item("ITProjectManagerStatus", $scope.appProperties.CRSR.ITProjectManagerStatus);           
            listItem.set_item("FundingAuthorityStatus", $scope.appProperties.CRSR.FundingAuthorityStatus);   
            //checkbox information
            listItem.set_item("ReasonforRequest", $scope.SelectionChkboxReasonforRequestInfo); //choice
            angular.forEach($scope.entities, function (subscription, index) {
                if (subscription.checked == true) {
                    $scope.SelectionChkboxChargeableInfo.push(subscription.name);
                }
            });
          
            listItem.set_item("Chargeable", $scope.SelectionChkboxChargeableInfo); //choice
            listItem.set_item("ExpectedCompletionDatefor", $scope.SelectionChkboxExpectedCompletionDateforInfo); //choice
            listItem.set_item("UATStatus", $scope.SelectionCheckboxUATStatusInfo);  //-chekcbox
            listItem.set_item("OnBehalf", $scope.appProperties.CRSR.OnBehalf);
            // datefield    
            if ($scope.appProperties.CRSR.ExpectedCompletionDate != null && $scope.appProperties.CRSR.ExpectedCompletionDate != undefined) {
                var dateParts = $scope.appProperties.CRSR.ExpectedCompletionDate.split("/");
                // month is 0-based, that's why we need dataParts[1] - 1
                var datecompltion = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                listItem.set_item("ExpectedCompletionDate", datecompltion);
            }
            //if ($scope.appProperties.CRSR.ProductionDataPatchsignoff != null && $scope.appProperties.CRSR.ProductionDataPatchsignoff != undefined) {
            //    var dateParts = $scope.appProperties.CRSR.ProductionDataPatchsignoff.split("/");
            //    // month is 0-based, that's why we need dataParts[1] - 1
            //    var datesignoff = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
            //    listItem.set_item("ProductionDataPatchsignoff", datesignoff);
            //}
            //all people picker field check value 
            if ($scope.appProperties.CRSR.RequestedFor != undefined) {
                if ($scope.appProperties.CRSR.RequestedFor.Id) {
                    listItem.set_item("RequestedFor", $scope.appProperties.CRSR.RequestedFor.Id);
                    listItem.set_item("RequestedForLoginID", $scope.appProperties.CRSR.RequestedFor.LoginName);
                }
            }           
            //  all manager Name-people picker 
            if ($scope.appProperties.CRSR.BranchHead.Id) {
                listItem.set_item("BranchHead", $scope.appProperties.CRSR.BranchHead.Id);
            }
            else {              
                    if ($scope.appProperties.BranchHeadMaster != null && $scope.appProperties.BranchHeadMaster != undefined) {
                        listItem.set_item("BranchHead", $scope.appProperties.BranchHeadMaster.Id);
                        $scope.appProperties.CRSR.BranchHead = $scope.appProperties.BranchHeadMaster;
                    }                
            }  
            //ITProjectManager
            if ($scope.appProperties.CRSR.ITProjectManager.Id) {
                listItem.set_item("ITProjectManager", $scope.appProperties.CRSR.ITProjectManager.Id);
            }
            else {
                if ($scope.appProperties.ITProjectManagerMaster.length > 0) {
                    $scope.appProperties.CRSR.ITProjectManager = $scope.appProperties.ITProjectManagerMaster[0];
                    listItem.set_item("ITProjectManager", $scope.appProperties.ITProjectManagerMaster[0].Id);
                }
            } 
         
                //FundingAuthority
                if ($scope.appProperties.CRSR.FundingAuthority.Id) {
                    listItem.set_item("FundingAuthority", $scope.appProperties.CRSR.FundingAuthority.Id);
                }
                //else {
                //    if ($scope.appProperties.FundingAuthorityMaster.length > 0) {
                //        listItem.set_item("FundingAuthority", $scope.appProperties.FundingAuthorityMaster[0].Id);
                //        $scope.appProperties.CRSR.FundingAuthority = $scope.appProperties.FundingAuthorityMaster[0];
                //    }
                //}
            
            //SystemOwner
            if ($scope.appProperties.CRSR.SystemOwner.Id) {
                listItem.set_item("SystemOwner", $scope.appProperties.CRSR.SystemOwner.Id);
            }
            else {
                if ($scope.appProperties.SystemOwnerMaster.length > 0) {
                    listItem.set_item("SystemOwner", $scope.appProperties.SystemOwnerMaster[0].Id);
                    $scope.appProperties.CRSR.SystemOwner = $scope.appProperties.SystemOwnerMaster[0];
                }
            }
            //workflow general status     
            
            listItem.set_item("ApplicationStatus", ApplicationStatus);
            listItem.set_item("WorkFlowStatus", Worfkflowstatus);
            listItem.update();
            ctx.load(listItem);
            ctx.executeQueryAsync(function () {
                try {

                    var redirectUrl = siteAbsoluteUrl +"/Pages/CRSRDashboard.aspx";
                    insertComments(listItem.get_id(), commentStatus);
                    if ($scope.appProperties.appFiles.length > 0) {
                        insertSupportingDocuments(listItem.get_id(), message, redirectUrl);
                    }

                    //sendEmail to approver                 
                    switch (Worfkflowstatus) {

                        case 1:
                            //send email to branchHead/user- Submitted for approval
                            if ($scope.appProperties.CRSR.BranchHead.Id) {
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.CRSR.BranchHead.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                //Utility.helpers.sendEmail($scope.appProperties.CRSR.BranchHead.EMail, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.BranchHead.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            break;
                        case 2:
                        case 3:
                            //send email to ITManger/user- After Approved by BranchHead                       
                            if ($scope.appProperties.CRSR.ITProjectManager.Id && Worfkflowstatus == '2') { //approved email
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.CRSR.ITProjectManager.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail( $scope.appProperties.CurrentUser.Email,  "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                //Utility.helpers.sendEmail($scope.appProperties.CRSR.ITProjectManager.EMail, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.ITProjectManager.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            else {
                                //rejected email
                                //alert("Send rejected Emil to : " + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "CR/SR rejected", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application rejected " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            break;
                        case 4:
                        case 5:
                            //send email to systemowner/user- After Approved by ITProjectManager
                            if ($scope.appProperties.CRSR.SystemOwner.Id && Worfkflowstatus == '4') { //approved email
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.CRSR.SystemOwner.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                //Utility.helpers.sendEmail($scope.appProperties.CRSR.SystemOwner.EMail, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.SystemOwner.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            else {
                                //rejected email
                                //alert("Send rejected Emil to : " + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email,  "CR/SR Rejected", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application rejected " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            break;
                        case 6:
                        case 7:
                            //send email to funding/user- After Approved by systemowner
                            if ($scope.appProperties.CRSR.FundingAuthority.Id && Worfkflowstatus == '6') { //approved email
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.CRSR.FundingAuthority.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail( $scope.appProperties.CurrentUser.Email,  "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                //Utility.helpers.sendEmail( $scope.appProperties.CRSR.FundingAuthority.EMail,  "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.FundingAuthority.EMail, "", "", "CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            else if (Worfkflowstatus == '6') { //approved email
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.CRSR.FundingAuthority.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail( $scope.appProperties.CurrentUser.Email,  "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application approved " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been approved. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                //Utility.helpers.sendEmail( $scope.appProperties.CRSR.FundingAuthority.EMail,  "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                //Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.FundingAuthority.EMail, "", "", "fun a 10 CRSR Application Submitted " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                //Utility.helpers.SendGMailService(hostName, portNumber, true, password, fromEmail, dispName, $scope.appProperties.CRSR.ITProjectManager.EMail, "", "", "CRSR Application approved " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been approved. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            else {
                                //rejected email
                                //alert("Send rejected Emil to : " + $scope.appProperties.CurrentUser.Email);
                                //Utility.helpers.sendEmail( $scope.appProperties.CurrentUser.Email,  "CR/SR Rejected", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application rejected " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                            }
                            break;
                        case 8:
                        case 9:
                            //send email to user- After Approved by FundingAuthority
                            if (Worfkflowstatus == '8') { //approved email
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.CurrentUser.Email);
                                // Utility.helpers.sendEmail( $scope.appProperties.CurrentUser.Email,  "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                // Utility.helpers.sendEmail( $scope.appProperties.CRSR.FundingAuthority.EMail, null, "CR/SR Submitted", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application approved " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been approved. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.ITProjectManager.EMail, "", "", "CRSR Application approved " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been approved.");
                            }
                            else {
                                //rejected email
                                //alert("Send rejected Emil to : " + $scope.appProperties.CurrentUser.Email);
                                // Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "CR/SR Rejected", "This is to notify you that a CR/SR application <Application No> from <Staff Name> has been submitted. Please click here <a href='https://uat.nparksconnect.nparks.gov.sg/erequest/Pages/CRSRRequest.aspx?=" + $scope.appProperties.CRSR.ID + "'>endorse the application.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CRSR.Author.EMail, "", "", "CRSR Application rejected " + $scope.appProperties.CRSR.Author.Title + "", "This is to notify you that a CRSR Application " + $scope.appProperties.CRSR.ApplicationID + " from " + $scope.appProperties.CRSR.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/CRSRDashboard.aspx>here</a>");
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
                    }, 3000);
           
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
                    $scope.appProperties.CRSR.ApplicationID = "CRSR-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + count; //increment one
                }
                else {
                    $scope.appProperties.CRSR.ApplicationID = "CRSR-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + "1"
                }
            });
          
        }; 

        function bindAllMasterList() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllProjectSystemMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ProjectSystemMaster = response.data.d.results;
                }
            });
            uService.getAllCostCenterMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.CostCentreMaster = response.data.d.results;
                }
            });
            uService.getAllFundCenterMaster().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.FundCentreMaster = response.data.d.results;
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
                                                    $scope.appProperties.BranchHeadMaster = user.d;
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
                                            $scope.appProperties.GroupDirectorMaster = response.data.d.results[0].DivisionHead; //acting as grodup director
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
        //get Authorization from csutom list
        function bindAuthorization() {
            $scope.loaded = true;
            uService.getAuthorization().then(function (response) {
                $scope.loaded = false; //spinner stop  
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == Config.Roles['BranchHead']) {
                           // $scope.appProperties.BranchHeadMaster = value.Approvers.results; no use this one
                        }
                        if (value.Title == Config.Roles['ITProjectManager']) {
                          //  $scope.appProperties.ITProjectManagerMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['FundingAuthority']) {
                            $scope.appProperties.FundingAuthorityMaster = value.Approvers.results;
                            commonService.getUserbyId($scope.appProperties.FundingAuthorityMaster[0].Id).then(function (response) {
                                //check out of office
                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                    if (response.data.d.results.length > 0) {
                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                            $scope.appProperties.FundingAuthorityMaster[0].Id = user.d.Id;
                                            $scope.appProperties.FundingAuthorityMaster[0].EMail = user.d.Email;
                                            $scope.appProperties.FundingAuthorityMaster[0].Title = user.d.Title;
                                        });
                                    }
                                });
                            });

                        }
                        if (value.Title == Config.Roles['SystemOwner']) {
                          //  $scope.appProperties.SystemOwnerMaster = value.Approvers.results;
                        }
                    });
                }
            });
        };

        $scope.getRequestType = function () {
            if ($scope.appProperties.CRSR.ProjectSystem != "") {                
                uService.getApproversFromList(Config.SystemAccessMaster).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        angular.forEach(response.data.d.results, function (value, key) {                          
                            if (value.Title == $scope.appProperties.CRSR.ProjectSystem) {
                                $scope.appProperties.ITProjectManagerMaster = value.CRSRITProjectManager.results;
                                commonService.getUserbyId($scope.appProperties.ITProjectManagerMaster[0].Id).then(function (response) {
                                    //check out of office
                                    commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                        if (response.data.d.results.length > 0) {
                                            getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                $scope.appProperties.ITProjectManagerMaster[0].Id = user.d.Id;
                                                $scope.appProperties.ITProjectManagerMaster[0].EMail = user.d.Email;
                                                $scope.appProperties.ITProjectManagerMaster[0].Title = user.d.Title;
                                            });
                                        }
                                    });
                                });
                                $scope.appProperties.SystemOwnerMaster = value.CRSRSystemOwner.results;
                                commonService.getUserbyId($scope.appProperties.SystemOwnerMaster[0].Id).then(function (response) {
                                    //check out of office
                                    commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                        if (response.data.d.results.length > 0) {
                                            getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                $scope.appProperties.SystemOwnerMaster[0].Id = user.d.Id;
                                                $scope.appProperties.SystemOwnerMaster[0].EMail = user.d.Email;
                                                $scope.appProperties.SystemOwnerMaster[0].Title = user.d.Title;
                                            });
                                        }
                                    });
                                });
                            } 
                        });
                    }
                });
            }
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
                if ($scope.appProperties.CRSR.ID != null && $scope.appProperties.CRSR.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.CRSR.ID).then(function (response) {
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
                var oList = web.get_lists().getByTitle(Config.CRSRDocuments);
                uService.getDocLibByFolderName(listItemId, Config.CRSRDocuments).then(function (response) {
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
            var serverRelativeUrlToFolder = _spPageContextInfo.siteServerRelativeUrl + '/' + Config.CRSRDocuments+'/' + listItemId;
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
                    "/_api/Web/lists/GetByTitle('" + Config.CRSRDocuments+"')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'CRSRListItemID': listItemId
                       
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
            var clist = web.get_lists().getByTitle("" + Config.CRSRComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            if ($scope.appProperties.appComments.Comments != null && $scope.appProperties.appComments.Comments != "") {
                clistItem.set_item('UserComments', $scope.appProperties.appComments.Comments + " ( " + Config.CRSRCommentStatus[status] + " )");
            } else {
                clistItem.set_item('UserComments', Config.CRSRCommentStatus[status] + " (System Comments)");
            }
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('CRSRListItemID', listItemId);
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
        function loadCRSRListData(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getById(ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.CRSR = response.data.d.results[0];                   
                    $scope.appProperties.CRSR.Author.LoginName = $scope.appProperties.CRSR.RequestorLoginID;
                    $scope.appProperties.CRSR.RequestedFor.LoginName = $scope.appProperties.CRSR.RequestedForLoginID;
                    $scope.appProperties.CRSR.RequestedFor.Title = response.data.d.results[0].RequestedFor.Title;
                    //enable approval button
                    DisableHideShowControls($scope.appProperties.CRSR.WorkFlowStatus, $scope.appProperties.CurrentUser.Id) //disable controls while load and approval status
                    //format-date
                    $scope.appProperties.CRSR.ApplicationDate = $filter('date')($scope.appProperties.CRSR.Created, 'dd/MM/yyyy') //created 
                   
                    if ($scope.appProperties.CRSR.ProductionDataPatchsignoff != null && $scope.appProperties.CRSR.ProductionDataPatchsignoff != undefined) {
                        $scope.appProperties.CRSR.ProductionDataPatchsignoff = $filter('date')($scope.appProperties.CRSR.ProductionDataPatchsignoff, 'dd/MM/yyyy') //proejctstartdate 
                    }
                    if ($scope.appProperties.CRSR.ExpectedCompletionDate != null && $scope.appProperties.CRSR.ExpectedCompletionDate != undefined) {
                        $scope.appProperties.CRSR.ExpectedCompletionDate = $filter('date')($scope.appProperties.CRSR.ExpectedCompletionDate, 'dd/MM/yyyy') //prjecednddate 
                    } 
                    if ($scope.appProperties.CRSR.OnBehalf == "1") {
                        $scope.OnBehalf = true;
                    } else {
                        $scope.OnBehalf = false;
                    }
                    // checkbox result
                    if (response.data.d.results[0].ReasonforRequest != null) {
                        $scope.SelectionChkboxReasonforRequestInfo = response.data.d.results[0].ReasonforRequest.results;
                        if ($scope.appProperties.CRSR.OtherReasonforRequest != "") {
                            $scope.IsEnabledReasonforRequestOthers = true;
                        }
                    }  
                    // checkbox result
                    if (response.data.d.results[0].Chargeable != null) {
                        $scope.SelectionChkboxChargeableInfo = response.data.d.results[0].Chargeable.results;
                        angular.forEach($scope.entities, function (subscription, index) {
                            if ($scope.SelectionChkboxChargeableInfo[0] == "Yes" && subscription.name == "Yes") {
                                subscription.checked = true;
                                $scope.IsfundAuthoirty = true;
                            }
                            if ($scope.SelectionChkboxChargeableInfo[0] == "No" && subscription.name == "No") {
                                subscription.checked = true;
                            }

                        });
                    } 
                    // checkbox result
                    if (response.data.d.results[0].ExpectedCompletionDatefor != null) {
                        $scope.SelectionChkboxExpectedCompletionDateforInfo = response.data.d.results[0].ExpectedCompletionDatefor.results;
                    }  
                    // checkbox result
                    if (response.data.d.results[0].UATStatus != null) {
                        $scope.SelectionCheckboxUATStatusInfo = response.data.d.results[0].UATStatus.results;
                        if ($scope.appProperties.CRSR.CheckboxUATStatusInfo != "") {
                            $scope.IsEnabledUATOthers = true;
                        }
                    } 
                    //display staus on ui
                    $scope.appProperties.CRSR.DisplayStatus = Config.CRSRWFStatus[$scope.appProperties.CRSR.ApplicationStatus]
                }
            });
        };
        //ChkCRSRdoc
        function loadFilesDocument(ListItemId,ListName) {
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
 
        function DisableHideShowControls(status, currentUsrId, ReqTypeCode) {
            //initally disble/show 
            $scope.IsBranchHeadDisabled = false; // disbale true        
            $scope.IsFinanceHeadDisabled = false; 
            $scope.IsCommentsSection = true;
            //manager status- button  -disable             
            $scope.IsBranchHeadStatusDisabled = false;
            $scope.IsFinanceHeadStatusDisabled = false; 
            //while submit and save disable date field and button
            $scope.IsSubmitButton = false;
            $scope.IsSApproveButton = false;
            $scope.IsRejectButton = false;
            $scope.IsSubmitMode = false // using this while submit button enable control hide   
            $scope.IsClosedButton = false;
            //saved mode
            if (status == "New" && (currentUsrId == $scope.appProperties.CRSR.Author.Id)) { 
                $scope.IsSubmitButton = true;    
                $scope.IsCommentsSection = false;
            }           
            //Branch head  approval
            else if (status == "1" && (currentUsrId == $scope.appProperties.CRSR.BranchHead.Id)) {              
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;              
            }
            else if (status == "10" && (currentUsrId == $scope.appProperties.CRSR.BranchHead.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            else if (status == "10" && ($scope.appProperties.CRSR.BranchHeadStatus =='Approved' && currentUsrId == $scope.appProperties.CRSR.ITProjectManager.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            else if (status == "10" && ($scope.appProperties.CRSR.ITProjectManagerStatus =='Approved' && currentUsrId == $scope.appProperties.CRSR.SystemOwner.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            else if (status == "10" && ($scope.appProperties.CRSR.SystemOwnerStatus == 'Approved' && currentUsrId == $scope.appProperties.CRSR.FundingAuthority.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            // After Branch head  approval- enable itmanager Head
            else if (status == "2" && (currentUsrId == $scope.appProperties.CRSR.ITProjectManager.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            } 
            // After GroupDirector head  approval- enable CIO Head
            else if (status == "4" && (currentUsrId == $scope.appProperties.CRSR.SystemOwner.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }  
            // After GroupDirector head  approval- enable CIO Head
            else if (status == "6" && (currentUsrId == $scope.appProperties.CRSR.FundingAuthority.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }  
            // After final close by user or ITmager
            else if (status == "8" && (currentUsrId == $scope.appProperties.CRSR.ITProjectManager.Id || currentUsrId == $scope.appProperties.CRSR.Author.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsClosedButton = true;               
            }  
            // Rejected status and not equal currentuser and approver (disable/Hide all control)
            else {
               //initailly alardy disbles
            }
        };
        //javascript methods---------------------------------------------------------------------------------------- end
    }]);
})();