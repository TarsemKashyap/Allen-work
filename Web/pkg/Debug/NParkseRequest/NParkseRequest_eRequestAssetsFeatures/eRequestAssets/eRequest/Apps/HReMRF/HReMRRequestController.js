"use strict";
(function () {
    app.controller("HReMRRequestController", ["$scope", "HReMR", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
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
        $scope.isSiteAdmin = false;
        $scope.isExistingRequest = false;
        $scope.isIntern = false;
        $scope.isTemporary = false;
        $scope.Workflows = [];
        //check required validation
        //$scope.appProperties = [];
        $scope.appProperties = ({
            //List Properties-exact columnname on CRSRRequests(save/edit) and additional properties for ui purpose
            HRRequests: ({
                ID: "",
                Title: "",
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker
                SupportingOfficer: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                ProposedBuddy: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                PrimarySupervisor: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                SecondarySupervisor: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                DivisionHead: ({ Id: "", EMail: "", Title: "" }),
                Level1Approver: ({ Id: "", EMail: "", Title: "" }),
                Level2Approver: ({ Id: "", EMail: "", Title: "" }),
                Level3Approver: ({ Id: "", EMail: "", Title: "" }),
                Requestor: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                RequestedFor: ({ Id: "", EMail: "", Title: "", LoginName: "" }),
                //status              
                Level1ApprovalStatus: "Pending",//set default
                Level2ApprovalStatus: "",
                Level3ApprovalStatus: "",
                //Approval Dates
                Level1ApprovalDate: "",//set default
                Level2ApprovalDate: "",
                Level3ApprovalDate: "",
                Level4ApprovalDate: "",
                //New Position and Replacement
                ApplicationID: "0",
                RequestorID: "",
                ManpowerRequestType: "", // use this code to level of workflow
                ReplacementFullname: "", //use this code 
                ManpowerType: "",
                JobDesignation: "",
                DeploymentDuration: "",
                NoOfManpower: "",
                NewRequiredAccess: "",               
                ComputerRequirement: "",
                AccessToSecretFiles: "",
                HandPhoneSubsidy: "",
                RosterOffday: "",
                ApprovalRejectionComments: "",
                //Intern fields
                NoOfInternRequired: "",
                NameOfInstitution: "",
                NameOfCourse: "",
                InternStartDate: "",
                InternEndDate: "",
                WorkingHour: "",
                FilesAccessCardRequired: "",
                TempAccessCardRequired: "",
                InternInterviewRequired: "",
                JobDescription: "",
                LearningOutcome: "",
                OtherRequirement: "",
                DivisionToAttach: "",
                BranchToAttach: "",
                NameOfIntern: "",
                InternContactNo: "",
                InternEmail: "",
                ResumeFile: "",
                InternContactInfos: [],
                Divisions: [],
                Branches: [],
                //Temporary workers
                NoOfTempStaff: "",
                TempDurationRequired: "",
                Justification: "",
                JobDescription: "",
                Degree: "",
                Diploma: "",
                NITEC: "",
                ALevel: "",
                OLevel: "",
                NLevel: "",
                OnBehalf:"0",
                Skillsets: "",
                WorkingHour: "",
                WorkLocation: "",
                OtherRequirement: "",
                TempAccessCard: "",
                RosteredOffdays: "",
                TemporaryCandidates: [],
                TemporaryStaffName: "",
                TemporaryContactNo: "",
                TemporaryEmail: "",
                TemporaryResume: "",
                //funding info - common for all
                FundingType: "",
                FundingName: "",
                FundCentre: "",
                CostCentre: "",
                FundingDuration: "",
                WorkingHourOtherText: "",
                isTemporaryOther: "0",
                isInternOther: "0",
                InternOtherText:"",
                ApplicationDate: $filter('date')(new Date(), 'MM/dd/yyyy'), //dd-MM-yyyy hh:mm a   
                ApplicationStatus: "",
                WorkFlowStatus: "0",
                DisplayStatus: "New", //UI purpose
                //-chekcbox list column                
                LoginAccountType: ({ results: [] }),
                OffRenoServices: ({ results: [] }),
                InstallUpgradeSW: ({ results: [] }),
                EmailServices: ({ results: [] })
            }),
            //List properties
            ManpowerRequestTypes: ({ ID: "", Title: "", TypeOfManpowerRequestID: "" }),
            ManpowerTypes: ({ ID: "", Title: "", ManpowerTypeID: "" }),
            DeploymentDurations: ({ ID: "", Title: "" }),
            FundingTypes: ({ ID: "", Title: "" }),
            CostCentres: ({ ID: "", Title: "", CostCentreID: "" }),
            FundCentres: ({ ID: "", Title: "", FundCentreID: "" }),
            ComputerRequirements: ({ ID: "", Title: "" }),
            //Approvers
            //BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            //GroupDirectorMaster: ({ Id: "", EMail: "", Title: "" }),
            //ITProjectManagerMaster: ({ Id: "", EMail: "", Title: "" }),
            //SystemOwner: ({ Id: "", EMail: "", Title: "" }),
            //FundingAutorityMaster: ({ Id: "", EMail: "", Title: "" }),
            HReMRApprovers: ({ Id: "", EMail: "", Title: ""}),
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
            apptempFiles: [],
            apptempFileProperties: [],
            //appInstallUpgradeSWDocumentsFiles: [],
            //appFileInstallUpgradeSWDocumentsProperties: [],
            //appFloorPlanFiles: [],
            //appFileFloorPlanProperties: [],
           // appChecklistFiles: [],
            //appFileChecklistProperties: [],
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", DomainName: "" }),
        });
        //Expected Completion Date For -checkbox
        $scope.CheckboxNewRequiredAccess = ""; //["UAT Test Required", "Data Patch"];
        $scope.SelectionCheckboxNewRequiredAccess = [];
        $scope.CheckboxHandPhoneSubsidy = "";
        $scope.SelectionCheckboxHandPhoneSubsidy = [];
        $scope.CheckboxRosterOffday = "";
        $scope.SelectionCheckboxRosterOffday = [];
        $scope.CheckboxTempAccessCardRequired = "";
        $scope.SelectionCheckboxTempAccessCardRequired = [];
        $scope.CheckboxInternInterviewRequired = "";
        $scope.SelectionCheckboxInternInterviewRequired = [];
        $scope.onBehalfList = [{ value: false }, { value: true }]
        $scope.appChildProperties = ({
            //Current User Properties    
            DelegateUserItem: "",
            CurrentUser: ({ Id: "", Email: "", Title: "" }),
            DelegateUser: ({ Id: "", EMail: "", Title: "" }),
            DelegateManagerMaster: ({ Id: "", EMail: "", Title: "" })
        });
        $scope.entities = [{
            name: 'Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm',
            checked: false
        }, {
            name: 'Others',
            checked: false
        }
        ];
        $scope.DomainName = "";
        //Angular methods ----------------------------------------------------------------------------------start-while using html then create scope variable or method


        //init start
        $scope.init = function (init) {
            Utility.helpers.initializePeoplePicker('peoplePickerSupportingOfficer');
            Utility.helpers.initializePeoplePicker('peoplePickerProposedBuddy');
            Utility.helpers.initializePeoplePicker('peoplePickerPrimarySupervisor');
            Utility.helpers.initializePeoplePicker('peoplePickerSecondarySupervisor');
            Utility.helpers.initializePeoplePicker('peoplePickerRequestedFor');
            //OnValueChangedClientScript- while user assing fire this event
            $scope.ProposedPeopleBuddyReq = true; //cchk ondition
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerSupportingOfficer_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    if (userData.Key !== undefined) {
                        $scope.ProposedPeopleBuddyReq = false; //cchk ondition
                        $scope.isProposedPeopleBuddyReq = false;
                        $('#userHostID').val(userData.Key.split('\\')[1]);
                        $scope.appProperties.HRRequests.SupportingOfficer.LoginName = userData.Key.split('\\')[1];
                        // Get the first user's ID by using the login name.
                        getUserId(userData.Key).done(function (user) {
                            $scope.appProperties.HRRequests.SupportingOfficer.Id = user.d.Id;
                        });
                        //$scope.$apply();
                    }
                    else if (userData.Id !== '' && userData.Id !== undefined) {
                        $scope.appProperties.HRRequests.SupportingOfficer.Id = userData.Id;
                    }
                    
                } else {
                 
                    $scope.appProperties.HRRequests.SupportingOfficer.Id = "";
                    $scope.isProposedPeopleBuddyReq = true;
                    $scope.$apply();
                }
            };
            //OnValueChangedClientScript- while user assing fire this event
            $scope.ProposedPeoplePicReq = true;
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerProposedBuddy_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    if (userData.Key !== undefined) {
                        $scope.ProposedPeoplePicReq = false;
                        $scope.isProposedPeoplePicReq = false;
                        $('#userHostID').val(userData.Key.split('\\')[1]);
                        $scope.appProperties.HRRequests.ProposedBuddy.LoginName = userData.Key.split('\\')[1];
                        // Get the first user's ID by using the login name.
                        getUserId(userData.Key).done(function (user) {
                            $scope.appProperties.HRRequests.ProposedBuddy.Id = user.d.Id;                          
                        });
                        //$scope.$apply();
                    }
                    else if (userData.Id !== '' && userData.Id !== undefined) {
                        $scope.appProperties.HRRequests.ProposedBuddy.Id = userData.Id;
                        $scope.isProposedPeoplePicReq = false;
                    }                   
                }
                else {
                 
                    $scope.isProposedPeoplePicReq = true;
                    $scope.appProperties.HRRequests.ProposedBuddy.Id = "";
                    $scope.$apply();
                }
              
            };
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerSecondarySupervisor_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    if (userData.Key !== undefined) {
                        $scope.IsSecondarySupervisor = false;
                        $('#userHostID').val(userData.Key.split('\\')[1]);
                        $scope.appProperties.HRRequests.SecondarySupervisor.LoginName = userData.Key.split('\\')[1];
                        // Get the first user's ID by using the login name.
                        getUserId(userData.Key).done(function (user) {
                            $scope.appProperties.HRRequests.SecondarySupervisor.Id = user.d.Id;
                        });
                        $scope.$apply();
                    } else if (userData.Id !== '' && userData.Id !== undefined) {
                        $scope.appProperties.HRRequests.SecondarySupervisor.Id = userData.Id;
                    }
                }
                else {

                    $scope.IsSecondarySupervisor = true;
                    $scope.appProperties.HRRequests.SecondarySupervisor.Id = "";
                    $scope.$apply();
                }
            };
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerPrimarySupervisor_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    if (userData.Key !== undefined) {
                        $scope.IsPrimarySupervisor = false;
                        $('#userHostID').val(userData.Key.split('\\')[1]);
                        $scope.appProperties.HRRequests.PrimarySupervisor.LoginName = userData.Key.split('\\')[1];
                        // Get the first user's ID by using the login name.
                        getUserId(userData.Key).done(function (user) {
                            $scope.appProperties.HRRequests.PrimarySupervisor.Id = user.d.Id;
                        });
                      //  $scope.$apply();
                    } else if (userData.Id !== '' && userData.Id !== undefined) {
                        $scope.appProperties.HRRequests.PrimarySupervisor.Id = userData.Id;
                    }
                }
                else {

                    $scope.IsPrimarySupervisor = true;
                    $scope.appProperties.HRRequests.PrimarySupervisor.Id = "";
                    $scope.$apply();
                }
            };
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerRequestedFor_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    if (userData.Key !== undefined) {
                        $scope.IsRequestedReq = false; //cchk ondition
                        $('#userID').val(userData.Key.split('\\')[1]);
                        $scope.appProperties.HRRequests.RequestedFor.LoginName = userData.Key.split('\\')[1];
                        // Get the first user's ID by using the login name.
                        getUserId(userData.Key).done(function (user) {
                            $scope.appProperties.HRRequests.RequestedFor.EMail = user.d.Email;
                            $scope.appProperties.HRRequests.RequestedFor.Id = user.d.Id;
                        });
                      //  $scope.$apply();
                    }
                   
                } else {
                    $scope.appProperties.HRRequests.RequestedFor.Id ="";
                    $scope.IsRequestedReq = true;
                    $scope.$apply();                  
                }
            };
            //load function
            getCurrentLoginUsersAndBindAllData(); //get current usser - After success call loading data         

            commonService.getCurrentUserWithDetails().then(function (result) {
                var groupNames = ['HRAdminGroup'];
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
       
        //Selecting files
        //$scope.selectFile = function () {
        //    angular.element("#fileUploadField").click();
        //}
        //$scope.changeFile = function ($event) {
        //    $scope.appProperties.HRRequests.ResumeFile = $event.target.value;
        //    
        //}
        $scope.checkWoringHr = function () {
            $scope.IsWorkingCheckboxReq = false;
            if ($scope.appProperties.HRRequests.WorkingHour == "Yes" && $scope.isTemporaryOther == true) {
                $scope.IsWorkingCheckboxReq = true;
            }
            if ($scope.appProperties.HRRequests.WorkingHour == "Yes" && $scope.isTemporaryOther == undefined) {
                $scope.IsWorkingCheckboxReq = true;
            }
            if ($scope.appProperties.HRRequests.WorkingHour == "Yes" && $scope.isInternOther == true) {
                $scope.IsWorkingCheckboxReq = true;
            }
            if ($scope.appProperties.HRRequests.WorkingHour == false && $scope.isInternOther == undefined) {
                $scope.IsWorkingCheckboxReq = true;
            }
        }
        
        // Check checkbox checked or not
        $scope.checkValTemp = function () {
            $scope.IsWorkingCheckboxReq = false;
            if ($scope.isTemporaryOther) {
                $scope.appProperties.HRRequests.isTemporaryOther = "1";
                $scope.isTemporaryOther = true;
            } else {
                $scope.isTemporaryOther = false;
             
                $scope.appProperties.HRRequests.isTemporaryOther = "0";
                $scope.appProperties.HRRequests.WorkingHourOtherText = ""; //celar other textbox

            }
            if ($scope.appProperties.HRRequests.WorkingHour == "Yes" && $scope.isTemporaryOther == true) {
               $scope.IsWorkingCheckboxReq = true;
            }
        }; 
        $scope.checkValInter = function () {
            $scope.IsWorkingCheckboxReq = false;
            if ($scope.isInternOther) {
                $scope.appProperties.HRRequests.isInternOther = "1";
                $scope.isInternOther = true;
            } else {
                $scope.isInternOther = false;
                $scope.appProperties.HRRequests.isInternOther = "0";
                $scope.appProperties.HRRequests.InternOtherText = ""; //celar other textbox               
            }
            if ($scope.appProperties.HRRequests.WorkingHour == "Yes" && $scope.isInternOther == true) {
                $scope.IsWorkingCheckboxReq = true;
            }
        };
        
        $scope.addIntern = function () {
            //Add the new item to the Array.
            var tempintern = {}; var isfileExit = false;;
            if (($scope.appProperties.HRRequests.NameOfIntern != "" && $scope.appProperties.HRRequests.NameOfIntern != undefined)
                || ($scope.appProperties.HRRequests.InternEmail != undefined && $scope.appProperties.HRRequests.InternEmail != "")
                || ($scope.appProperties.HRRequests.InternContactNo != undefined && $scope.appProperties.HRRequests.InternContactNo != "")
                
            ) {
                tempintern.ApplicationID = $scope.appProperties.HRRequests.ID;
                tempintern.NameOfIntern = $scope.appProperties.HRRequests.NameOfIntern;
                tempintern.InternEmail = $scope.appProperties.HRRequests.InternEmail;
                tempintern.InternContactNo = $scope.appProperties.HRRequests.InternContactNo;
                if ($scope.appProperties.HRRequests.InternContactInfos.length > 0) {
                    //chk file addedd already
                    angular.forEach($scope.appProperties.HRRequests.InternContactInfos, function (value, key) {
                        if (value["internFiles"] != undefined) {
                            if (value.internFiles["name"].indexOf($scope.internfiles._file["name"]) !== -1) {
                                isfileExit = true;
                            }
                        }
                    });
                    if (!isfileExit) {
                        tempintern.internFiles = $scope.internfiles;
                        tempintern.ResumeFile = $scope.internfiles._file["name"];
                    }
                    $scope.appProperties.HRRequests.InternContactInfos.push(tempintern);
                }
                else {
                    if ($scope.internfiles != undefined) {
                        if ($scope.internfiles._file["name"] != undefined) {
                            tempintern.internFiles = $scope.internfiles;
                            tempintern.ResumeFile = $scope.internfiles._file["name"];
                        }
                    }
                    $scope.appProperties.HRRequests.InternContactInfos.push(tempintern);
                }

                //Clear the TextBoxes.
                $scope.appProperties.HRRequests.NameOfIntern = "";
                $scope.appProperties.HRRequests.InternEmail = "";
                $scope.appProperties.HRRequests.InternContactNo = "";
                $scope.appProperties.HRRequests.ResumeFile = "";
                $('#fileIntern').val('');
            }
        };
        $scope.Removeintern = function (index) {
            //Find the record using Index from Array.
            var name = $scope.appProperties.HRRequests.InternContactInfos[index].NameOfIntern;
            if ($window.confirm("Do you want to delete: " + name)) {
                //Remove the item from Array using Index.
                $scope.appProperties.HRRequests.InternContactInfos.splice(index, 1);
            }
        }
        //adding Temporary workers in rows       
        $scope.addTemporaryWorker = function () {
            //Add the new item to the Array.
            var tempstaf = {};
            if ($scope.appProperties.HRRequests.Title != ""
                || ($scope.appProperties.HRRequests.Email != undefined && $scope.appProperties.HRRequests.Email != "")
                || ($scope.appProperties.HRRequests.ContactNo != undefined && $scope.appProperties.HRRequests.ContactNo != "")
                || ($scope.appProperties.HRRequests.FileName != undefined && $scope.appProperties.HRRequests.FileName != "")
                ) {
                tempstaf.ApplicationID = $scope.appProperties.HRRequests.ID;
                tempstaf.Title = $scope.appProperties.HRRequests.Title;
                tempstaf.ContactNo = $scope.appProperties.HRRequests.ContactNo;
                tempstaf.Email = $scope.appProperties.HRRequests.Email;    
                if ($scope.tempfiles.length > 0) {
                    tempstaf.Files = $scope.tempfiles;
                    tempstaf.FileName = $scope.tempfiles[0].name;
                }
                $scope.appProperties.HRRequests.TemporaryCandidates.push(tempstaf);

                //Clear the TextBoxes.
                $scope.appProperties.HRRequests.Title = "";
                $scope.appProperties.HRRequests.ContactNo = "";
                $scope.appProperties.HRRequests.Email = "";
                $scope.appProperties.HRRequests.FileName = "";             
                //clear files  
                $("#uploadFileID").find(".files").empty();
              
                  
            }
        };
       
      
        $scope.Remove = function (index) {
            //Find the record using Index from Array.
            var name = $scope.appProperties.HRRequests.TemporaryCandidates[index].Title;
            if ($window.confirm("Do you want to delete: " + name)) {
                //Remove the item from Array using Index.
                $scope.appProperties.HRRequests.TemporaryCandidates.splice(index, 1);
            }
        }
        //This will hide the DIV by default.
        $scope.IsIntAccessCardRequired = false;
        $scope.ShowHide = function () {
            //If DIV is visible it will be hidden and vice versa.
            $scope.IsIntAccessCardRequired = $scope.appProperties.HRRequests.TempAccessCardRequired;
            if ($scope.IsIntAccessCardRequired == false) {
                $scope.appProperties.HRRequests.InternInterviewRequired = false;
                $scope.appProperties.HRRequests.FilesAccessCardRequired = false;
                $scope.appProperties.HRRequests.RosterOffday = false;
                $scope.Isdivtext = false;
            }          
        }
        $scope.Isdivtext = false;
        $scope.ShowHidetext = function () {
            //If DIV is visible it will be hidden and vice versa.
            $scope.Isdivtext= $scope.appProperties.HRRequests.RosterOffday;
          
        }
        $scope.IsWorkingCheckboxReq = false;
        //submit request
        $scope.submitHRRequest = function (type, $event, Form) {
            $scope.loaded = true; //automate false while redirect page
            var appStatus = "0"; //maintain applciation field
            var cmtStatus = "0"; //maintain coments field
            var validInput = true;
            $event.preventDefault();          
            //init validation false 
            $scope.IsWorkingCheckboxReq = false;
            //alert($scope.appProperties.CRSRRequests.ProjectSystem);
            if (Form.ddlManpowerRequestType.$invalid || Form.replacementStaff.$invalid) {
                $scope.loaded = false;
                if (Form.ddlManpowerRequestType.$invalid) Form.ddlManpowerRequestType.$touched = true; //while submit neet to set true this field, to required msg will be dispaly
                if (Form.replacementStaff.$invalid) Form.replacementStaff.$touched = true;
                //return;
               
            }
            if (Form.ddlFundingType != undefined) {
            
            if (Form.ddlFundingType.$invalid  || Form.fundCentre.$invalid || Form.costCentre.$invalid ) {
                $scope.loaded = false;
                if (Form.ddlFundingType.$invalid) Form.ddlFundingType.$touched = true; //while submit neet to set true this field, to required msg will be dispaly
                
                if (Form.fundCentre.$invalid) Form.fundCentre.$touched = true;
                if (Form.costCentre.$invalid) Form.costCentre.$touched = true;              
                //return;
                validInput = false;
                }
            }
            if (Form.fundingName != undefined) {
                $scope.loaded = false;

                if (Form.fundingName.$invalid) {
                    Form.fundingName.$touched = true;

                    //return;
                    validInput = false;
                }
            }
            if ($scope.appProperties.HRRequests.ManpowerRequestType == "New Position" || $scope.appProperties.HRRequests.ManpowerRequestType == "Replacement") {
                if (Form.ddlManpowerType.$invalid || Form.ddlDeploymentDuration.$invalid || Form.jobDesignation.$invalid || Form.ddlComputerRequirement.$invalid) {
                    $scope.loaded = false;
                    if (Form.ddlManpowerType.$invalid) Form.ddlManpowerType.$touched = true;
                    if (Form.ddlComputerRequirement.$invalid) Form.ddlComputerRequirement.$touched = true;
                    if (Form.ddlDeploymentDuration.$invalid) Form.ddlDeploymentDuration.$touched = true;
                    if (Form.jobDesignation.$invalid) {
                        Form.jobDesignation.$touched = true;
                    }
                }
                if (Form.noOfManpower.$invalid || Form.workLocation.$invalid || Form.supportingOfficer.$invalid || Form.proposedBuddy.$invalid) {
                    $scope.loaded = false;
                    if (Form.noOfManpower.$invalid) Form.noOfManpower.$touched = true;
                    if (Form.workLocation.$invalid) Form.workLocation.$touched = true;
                    if ($scope.appProperties.HRRequests.SupportingOfficer.Id == "") Form.supportingOfficer.$touched = true;
                    if ($scope.ProposedPeopleBuddyReq) { $scope.isProposedPeopleBuddyReq = true; } else { $scope.isProposedPeopleBuddyReq = false; }
                    if ($scope.ProposedPeoplePicReq) { $scope.isProposedPeoplePicReq = true; } else { $scope.isProposedPeoplePicReq = false;}
                  
                }
                
            } else if ($scope.appProperties.HRRequests.ManpowerRequestType == "Intern") {
                if (Form.noOfInternRequired.$invalid) Form.noOfInternRequired.$touched = true;
                if (Form.workLocation.$invalid) Form.workLocation.$touched = true;
                if (Form.nameOfInstitution.$invalid) Form.nameOfInstitution.$touched = true;
                if (Form.nameOfCourse.$invalid) Form.nameOfCourse.$touched = true;
                //if (Form.internStartDate.$invalid) Form.internStartDate.$touched = true;
                //if (Form.internEndDate.$invalid) Form.internEndDate.$touched = true;

                if (Form.internStartDate.$invalid) {
                    Form.internStartDate.$touched = true;
                    validInput = false;
                }
                else {
                    if (document.getElementById("internStartDate").value == "") {
                        validInput = false;
                        alert("Internship start date required");
                    }
                }
                if (Form.internEndDate.$invalid) {
                    Form.internEndDate.$touched = true;
                    validInput = false;
                } else {
                    if (document.getElementById("internEndDate").value == "") {
                        validInput = false;
                        alert("Internship End Date required");
                    }
                }

                if (Form.internJobDescription.$invalid) Form.internJobDescription.$touched = true;
                if (Form.internLearningOutcome.$invalid) Form.internLearningOutcome.$touched = true;
                if (Form.internOtherRequirement.$invalid) Form.internOtherRequirement.$touched = true;
                if (Form.ddlDivisionAttached.$invalid) Form.ddlDivisionAttached.$touched = true;
                if (Form.ddlBranchAttached.$invalid) Form.ddlBranchAttached.$touched = true;
            } else if ($scope.appProperties.HRRequests.ManpowerRequestType == "Temporary") {
                if (Form.noOfTempStaff.$invalid) Form.noOfTempStaff.$touched = true;
                if (Form.tempDurationRequired.$invalid) Form.tempDurationRequired.$touched = true;
                if (Form.tempStaffJustification.$invalid) Form.tempStaffJustification.$touched = true;
                if (Form.tempJobDescription.$invalid) Form.tempJobDescription.$touched = true;
                if (Form.chkTempDegree.$invalid) Form.chkTempDegree.$touched = true;
                if (Form.chkTempDiploma.$invalid) Form.chkTempDiploma.$touched = true;
                if (Form.chkTempNITEC.$invalid) Form.chkTempNITEC.$touched = true;
                if (Form.chkTempALevel.$invalid) Form.chkTempALevel.$touched = true;
                if (Form.chkTempOLevel.$invalid) Form.chkTempOLevel.$touched = true;
                if (Form.chkTempNLevel.$invalid) Form.chkTempNLevel.$touched = true;
                if (Form.tempSkillsets.$invalid) Form.tempSkillsets.$touched = true;
                if (Form.chkTempWHStandard.$invalid) Form.chkTempWHStandard.$touched = true;
               
                if (Form.workLocation.$invalid) Form.workLocation.$touched = true;
                if (Form.tempOtherRequirement.$invalid) Form.tempOtherRequirement.$touched = true;
                if (Form.chkTempAccessCard.$invalid) Form.chkTempAccessCard.$touched = true;
                if (Form.chkTempAccessCard.$invalid) Form.chkTempAccessCard.$touched = true;
            }            
            switch (type) {
                case 1:
                    //reset status alwys -Rework ManpowerType: "",
                     $scope.IsQualification = false;
                    if ($scope.appProperties.HRRequests.ManpowerRequestType == "") {
                        return;
                    } else {
                       
                        if ($scope.appProperties.HRRequests.ManpowerRequestType == "New Position" || $scope.appProperties.HRRequests.ManpowerRequestType == "Replacement") {
                            if ($scope.appProperties.HRRequests.ManpowerType == "" || $scope.appProperties.HRRequests.ComputerRequirement == "") validInput = false;
                            if ($scope.appProperties.HRRequests.NoOfManpower == "" || $scope.appProperties.HRRequests.WorkLocation == "") validInput = false;
                            if ($scope.appProperties.HRRequests.SupportingOfficer == "" || $scope.appProperties.HRRequests.ProposedBuddy == "") validInput = false;                          
                            if ($scope.ProposedPeoplePicReq == true) {
                                $scope.isProposedPeoplePicReq = true; validInput = false;
                            }
                            if ($scope.ProposedPeopleBuddyReq == true) {
                                $scope.isProposedPeopleBuddyReq = true; validInput = false;
                            }
                        } else if ($scope.appProperties.HRRequests.ManpowerRequestType == "Intern") {
                            if ($scope.appProperties.HRRequests.NoOfInternRequired == "" || $scope.appProperties.HRRequests.NameOfInstitution == "" || $scope.appProperties.HRRequests.NameOfCourse == "") validInput = false;
                            if ($scope.appProperties.HRRequests.InternStartDate == "" || $scope.appProperties.HRRequests.InternEndDate == "" || $scope.appProperties.HRRequests.WorkLocation == "") validInput = false;
                            if ($scope.appProperties.HRRequests.JobDescription == "" || $scope.appProperties.HRRequests.LearningOutcome == "" || $scope.appProperties.HRRequests.OtherRequirement == "") validInput = false;
                            if ($scope.appProperties.HRRequests.DivisionToAttach == "" || $scope.appProperties.HRRequests.BranchToAttach == "" || $scope.appProperties.HRRequests.PrimarySupervisor == "" || $scope.appProperties.HRRequests.SecondarySupervisor == "") validInput = false;
                            if ($scope.appProperties.HRRequests.FundingType == "") validInput = false;
                            if ($scope.isInternOther) {
                                if ($scope.appProperties.HRRequests.InternOtherText == "" || $scope.appProperties.HRRequests.InternOtherText == undefined) {
                                    validInput = false;
                                    Form.otherWorkingHours.$touched = true;
                                }
                            }
                            if (($scope.appProperties.HRRequests.WorkingHour == undefined || $scope.appProperties.HRRequests.WorkingHour == "")) {
                                validInput = false; $scope.WorkingHrsIsmandtry = true;
                              
                            }
                           
                            if ($scope.appProperties.HRRequests.PrimarySupervisor.Id == "") {
                                $scope.IsPrimarySupervisor = true;
                                validInput = false;
                            } else { $scope.IsPrimarySupervisor = false; }
                            //secondr
                            if ($scope.appProperties.HRRequests.SecondarySupervisor.Id == "") {
                                $scope.IsSecondarySupervisor = true;
                                validInput = false;
                            } else { $scope.IsSecondarySupervisor = false; }
                        } else if ($scope.appProperties.HRRequests.ManpowerRequestType == "Temporary") {
                            if ($scope.appProperties.HRRequests.FundingType == "") validInput = false;
                            if ($scope.appProperties.HRRequests.NoOfTempStaff == "" || $scope.appProperties.HRRequests.TempDurationRequired == "") validInput = false;
                            if ($scope.appProperties.HRRequests.Justification == "" || $scope.appProperties.HRRequests.JobDescription == "") validInput = false;
                            if ($scope.appProperties.HRRequests.Degree == "" && $scope.appProperties.HRRequests.Diploma == "" && $scope.appProperties.HRRequests.NITEC == "" && $scope.appProperties.HRRequests.ALevel == "" && $scope.appProperties.HRRequests.OLevel == "" && $scope.appProperties.HRRequests.NLevel == "")
                            {
                                validInput = false; $scope.IsQualification = true; //erromsg true
                            };
                            if (($scope.appProperties.HRRequests.WorkingHour == undefined || $scope.appProperties.HRRequests.WorkingHour == "")) {
                                validInput = false; $scope.WorkingtmpHrsIsmandtry = true;

                            }
                            if ($scope.isTemporaryOther) {
                                if ($scope.appProperties.HRRequests.WorkingHourOtherText == "" || $scope.appProperties.HRRequests.WorkingHourOtherText == undefined) {
                                    validInput = false;
                                    Form.tempOtherWorkingHours.$touched = true;
                                }
                            }
                            if (($scope.appProperties.HRRequests.WorkingHour == undefined || $scope.appProperties.HRRequests.WorkingHour == "")) {
                                validInput = false; $scope.IsWorkingCheckboxReq = true;
                            }
                           
                            if ($scope.appProperties.HRRequests.Skillsets == "") validInput = false;
                            if ($scope.appProperties.HRRequests.WorkLocation == "") validInput = false;
                        }
                    }
                    if ($scope.appProperties.HRRequests.OnBehalf == "1") {
                        if ($scope.appProperties.HRRequests.RequestedFor.Id=="") {
                            $scope.IsRequestedReq = true;
                            validInput = false;
                        } else { $scope.IsRequestedReq = false;}
                    }
                    
                    if (validInput) {
                        $('pills-home-tab').attr("disabled", "true");
                        $scope.appProperties.HRRequests.Level1ApprovalStatus = "Pending";
                        insertUpdateListItem(type, "1", cmtStatus, "Submitted Successfully");
                        
                    } else return; //workflowstaus,appstatus ,cmtStatus
                    // code block
                    
                   
                   
                    break;
                case 2: // Approved block
                    //date -approve/rejecte date assing in disabled function (dhrub)

                    var currDate = $filter("date")(new Date(), "MM-dd-yyyy");
                    appStatus = 1;
                    if ($scope.appProperties.HRRequests.Level1ApprovalStatus == "Pending") {
                       // $scope.appProperties.HRRequests.Level2Approver = getLevelApprover("Level2Approver");
                        $scope.appProperties.HRRequests.Level1ApprovalStatus = "Approved";
                        $scope.appProperties.HRRequests.Level1ApprovalDate = currDate;
                        $scope.appProperties.HRRequests.Level2ApprovalStatus = "Pending";
                        
                    } else if ($scope.appProperties.HRRequests.Level2ApprovalStatus == "Pending") { //&& ($scope.appProperties.HRRequests.ManpowerRequestType == "New Position" || $scope.appProperties.HRRequests.ManpowerRequestType == "Replacement" || $scope.appProperties.HRRequests.ManpowerRequestType == "Intern")) {
                        $scope.appProperties.HRRequests.Level2ApprovalStatus = "Approved";
                        $scope.appProperties.HRRequests.Level2ApprovalDate = currDate;
                        appStatus = 2;
                    //} else if ($scope.appProperties.HRRequests.Level2ApprovalStatus == "Pending" && ($scope.appProperties.HRRequests.ManpowerRequestType !== "Temporary")) {
                    //    $scope.appProperties.HRRequests.Level2ApprovalStatus = "Approved";
                    //    $scope.appProperties.HRRequests.Level2ApprovalDate = currDate;
                    //    $scope.appProperties.HRRequests.Level3Approver = getLevelApprover("Level3Approver");
                    //    $scope.appProperties.HRRequests.Level3ApprovalStatus = "Pending";
                    //} else if ($scope.appProperties.HRRequests.Level3ApprovalStatus == "Pending") {
                    //    $scope.appProperties.HRRequests.Level3ApprovalStatus = "Approved";
                    //    $scope.appProperties.HRRequests.Level3ApprovalDate = currDate;
                    //    appStatus = 2;
                    } 
                    
                    insertUpdateListItem(type, appStatus, cmtStatus, "Approved Successfully");
                    break;
                case 3: //Rejected section
                    if ($scope.appProperties.appComments.Comments != "" && $scope.appProperties.appComments.Comments != undefined) {
                    //date -approve/rejecte date assing in disabled function
                    var currDate = $filter("date")(new Date(), "MM-dd-yyyy");
                    appStatus = 3;
                    if ($scope.appProperties.HRRequests.Level1ApprovalStatus == "Pending") {
                        $scope.appProperties.HRRequests.Level1ApprovalStatus = "Rejected";
                        $scope.appProperties.HRRequests.Level1ApprovalDate = currDate;
                        
                    } else if ($scope.appProperties.HRRequests.Level2ApprovalStatus == "Pending") {
                        $scope.appProperties.HRRequests.Level2ApprovalStatus = "Rejected";
                        $scope.appProperties.HRRequests.Level2ApprovalDate = currDate;
                    } //else if ($scope.appProperties.HRRequests.Level3ApprovalStatus == "Pending") {
                    //    $scope.appProperties.HRRequests.Level3ApprovalStatus = "Rejected";
                    //    $scope.appProperties.HRRequests.Level3ApprovalDate = currDate;
                    //} 
                        insertUpdateListItem(type, appStatus, cmtStatus, "Rejected Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus
                    }
                    else {
                        alert('Please enter rejection comments.');
                        $scope.loaded = false; //spinner stop - service call end
                        return;
                    }
                    break;
                case 4:
                    //Close
                    appStatus = 4;
                    insertUpdateListItem(type, appStatus, cmtStatus, "Closed Successfully"); //Worfkflowstatus, ApplicationStatus,commentStatus         
                    break;
                case 5:
                    //Re-Route
                    if ($scope.appProperties.appComments.Comments != "" && $scope.appProperties.appComments.Comments != undefined) {
                        appStatus = 5;
                        $scope.appProperties.HRRequests.Level1ApprovalStatus = "";
                        $scope.appProperties.HRRequests.Level2ApprovalStatus = "";
                        $scope.appProperties.HRRequests.Level3ApprovalStatus = "";
                        insertUpdateListItem(type, appStatus, cmtStatus, "eMRF will be routed back to requestor");

                    }
                    else {
                        alert('Please enter route back comments.');
                        $scope.loaded = false; //spinner stop - service call end
                        return;
                    }
                    break;
                 default:
                // code block
            }
        };
        $scope.isPermanent = false;
        $scope.selectRequestManType = function (type) {
            if (type == "Permanent") {

                $scope.isPermanent = true;
            }
            else {
                $scope.isPermanent = false;
            }
        }
        $scope.selectRequestType = function (type) {
            
            if (type == "New Position" || type == "Replacement") {
                $scope.isNewRequirement = true;
                $scope.isIntern = false;
                $scope.isTemporary = false;
              
            }           
            else if (type == "Intern") {
                $scope.isNewRequirement = false;
                $scope.isIntern = true;
                $scope.isTemporary = false;
              
            } else if (type == "Temporary") {
                $scope.isNewRequirement = false;
                $scope.isIntern = false;
                $scope.isTemporary = true;
              
            }
                
        }

        //getting selected Project System Value
        $scope.getSelectedProjectSystem = function (value) {
            alert(value);
        }
        $scope.IsRequestedReq = false;
        // Check checkbox checked or not
        $scope.checkVal = function () {
            $scope.IsRequestedReq = false;
            if ($scope.OnBehalf) {
                $scope.appProperties.HRRequests.OnBehalf = "1";
                $scope.OnBehalf = true;
                if ($scope.appProperties.HRRequests.RequestedFor.Id=="") {
                    $scope.IsRequestedReq = true;
                }
            } else {
                $scope.OnBehalf = false;
                $scope.appProperties.HRRequests.OnBehalf = "0";
            }
          //  $scope.$apply();
        }
        // Check checkbox checked or not
        $scope.checkValADNew = function () {
            if ($scope.NewADAcc) {
                $scope.appProperties.CRSRRequests.NewADAcc = "1";
                $scope.NewADAcc = true;
            } else {
                $scope.NewADAcc = false;
                $scope.appProperties.CRSRRequests.NewADAcc = "0";
            }
           // $scope.$apply();
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
            }
            // Is newly selected
            else {
                $scope.SelectionChkboxEmailServiceInfo.push(information);
            }
        };
        var chkbool = false;
        $scope.updateSelection = function (position, entities) {
            angular.forEach(entities, function (subscription, index) {
                if (position != index) {
                    subscription.checked = false;
                }

            });
            var contx = 0;
            $scope.WorkingHrsIsmandtry = false;
            $scope.isInternOther = false;
          
            var fundsx = false;
            angular.forEach($scope.entities, function (value, key) {
                $scope.WorkingHrsIsmandtry = false;
                $scope.isInternOther = false;
                if (value.checked == false) {
                    contx += 1;
                }
                if (value.checked == true && value.name == "Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm") {
                    fundsx = true;
                    $scope.appProperties.HRRequests.WorkingHour = value.name;
                }
                if (value.checked == true && value.name == "Others") {
                    $scope.appProperties.HRRequests.WorkingHour = value.name;
                    $scope.isInternOther = true;
                    if ($scope.appProperties.HRRequests.InternOtherText == "" || $scope.appProperties.HRRequests.InternOtherText == undefined) {
                        $scope.IsotherWorkingHours = true;
                    } else {
                        $scope.IsotherWorkingHours = false;
                    }
                }
              
            });           
            if (contx == 2) { //none selection
                $scope.WorkingHrsIsmandtry = true;
                //false             
            }
        }
        $scope.clearReq = function () {
            if ($scope.appProperties.HRRequests.InternOtherText == "" || $scope.appProperties.HRRequests.InternOtherText == undefined) {
                $scope.IsotherWorkingHours = true;
            } else {
                $scope.IsotherWorkingHours = false;
            }
        };
        $scope.updateSelection1 = function (position, entities) {
            angular.forEach(entities, function (subscription, index) {
                if (position != index) {
                    subscription.checked = false;
                }

            });
            var contx1 = 0;
            $scope.WorkingtmpHrsIsmandtry = false;
            $scope.isTemporaryOther = false;

            var fundsx = false;
            angular.forEach($scope.entities, function (value, key) {
                $scope.WorkingtmpHrsIsmandtry = false;
                $scope.isTemporaryOther = false;
                if (value.checked == false) {
                    contx1 += 1;
                }
                if (value.checked == true && value.name == "Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm") {
                    fundsx = true;
                    $scope.appProperties.HRRequests.WorkingHour = value.name;

                }
                if (value.checked == true && value.name == "Others") {
                    $scope.appProperties.HRRequests.WorkingHour = value.name;
                    $scope.isTemporaryOther = true;
                    if ($scope.appProperties.HRRequests.WorkingHourOtherText == "" || $scope.appProperties.HRRequests.WorkingHourOtherText == undefined) {
                        $scope.IsWorkingHourOtherText = true;
                    } else {
                        $scope.IsWorkingHourOtherText = false;
                    }
                }
              
            });
            if (contx1 == 2) { //none selection
                $scope.WorkingtmpHrsIsmandtry = true;
                //false             
            }
        }
        $scope.clearReq1 = function () {
            if ($scope.appProperties.HRRequests.WorkingHourOtherText == "" || $scope.appProperties.HRRequests.WorkingHourOtherText == undefined) {
                $scope.IsWorkingHourOtherText = true;
            } else {
                $scope.IsWorkingHourOtherText = false;
            }
        };

      /*  $scope.getRequestType = function () {
            $scope.appProperties.RequestTypeMaster.length = 0; //initially clear
            if ($scope.appProperties.CRSRRequests.WorkAreaCode != "") {
                $scope.loaded = true; //spinner start -service call start
                uService.getAllRequestTypeByWorkAreaID($scope.appProperties.CRSRRequests.WorkAreaCode).then(function (response) {
                    $scope.loaded = false; //spinner stop - service call end 
                    if (response.data.d.results.length > 0) {
                        $scope.appProperties.RequestTypeMaster = response.data.d.results;
                    }
                });
            };
        }
        $scope.SelectRequestTypeFields = function () {
            if ($scope.appProperties.CRSRRequests.RequestTypeCode != "--Select--") {
                ReqTypeHideShowControls($scope.appProperties.CRSRRequests.RequestTypeCode);
                if ($scope.appProperties.CRSRRequests.RequestTypeCode != null && $scope.appProperties.CRSRRequests.RequestTypeCode != "" && $scope.appProperties.CRSRRequests.RequestTypeCode != undefined) {

                    //chk Master data
                    var arryList = $filter('filter')($scope.appProperties.RequestTypeMaster, { RequestTypeCode: $scope.appProperties.CRSRRequests.RequestTypeCode });
                    if (arryList.length > 0) {
                        $scope.appProperties.CRSRRequests.WorkFlowCode = arryList[0].WorkFlowCode
                    }
                }
            };
        }*/
        //Angular methods --------------------------------------------------------------------------------------end


        //javascript methods ----------------------------------------------------------------------------------start
        //get login users details
        function getCurrentLoginUsersAndBindAllData() {
            $scope.loaded = true; //spinner start -service call start
            commonService.getCurrentUser().then(function (result) {
                $scope.loaded = false; //spinner stop - service call end 
                $scope.appProperties.CurrentUser = result.data.d;
                $scope.appProperties.CurrentUser.LoginName = result.data.d.LoginName.split('\\')[1];
                $scope.appProperties.HRRequests.Author.Id = result.data.d.Id; //while init 
                $scope.appProperties.HRRequests.Author.LoginName = result.data.d.Title; //while init                
                $scope.appProperties.HRRequests.Author.Title = result.data.d.Title; //while init 
                
                if (result.data.d.LoginName.indexOf("|") !== -1) {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];
                    //getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                   // getBrancGroupDirFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                }
                else {
                    $scope.appProperties.CurrentUser.ADID = result.data.d.Title.split("\\")[1]; // getting login name without domain   
                    $scope.appProperties.CurrentUser.DomainName = result.data.d.Title.split("\\")[0];
                   // getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, result.data.d.LoginName); //chk only
                   // getBrancGroupDirFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);

                }
                //all time load function   
                //bindAuthorization(); // 
               
                getManpowerRequestType();
                getManpowerType();
                getDeploymentDuration();
                getComputerRequirement();
                getHRFundingTypes();
                getAllDivisionAndBranch();
               

                //get query string value
                $scope.appProperties.HRRequests.ID = Utility.helpers.getUrlParameter('ReqId');
                if ($scope.appProperties.HRRequests.ID != "" && $scope.appProperties.HRRequests.ID != undefined) {
                    $scope.isExistingRequest = true;
                    getApprovers();
                    loadHRRequestsListData($scope.appProperties.HRRequests.ID);//Load data     
                    //load doucments
                    loadFilesDocument($scope.appProperties.HRRequests.ID, Config.HRDocuments); //getCRSRRequestDoc 
                    //loadFileFloorDocuments($scope.appProperties.CRSRRequests.ID, Config.OfficeRenovationDocuments); //OfficeRenovationDocuments 
                    //loadChekListDocument($scope.appProperties.CRSRRequests.ID, Config.OfficeRenovationChkListDocuments); //OfficeRenovationChkListDocuments 
                    //loadFilesInstallUpgradeSWDocuments($scope.appProperties.CRSRRequests.ID, Config.InstallUpgradeSWDocuments); //InstallUpgradeSWDocuments                     
                    //load comments
                    loadCommentsListData($scope.appProperties.HRRequests.ID); //PriceComparison 
                    
                }
                else {
                    //new req
                    getUniqueNumber(); //generate unique number-only for new Req 
                    
                    getLevel1Approver(); 
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
                        commonService.getStaffBranchHead("Branch", branch).then(function (response) {
                            $scope.loaded = false; //spinner stop  
                            if (response.data.d.results.length > 0) {
                                if (response.data.d.results[0].Branch != null && response.data.d.results[0].Branch != "" && response.data.d.results[0].Branch != undefined) {
                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].BranchHead;
                                }
                            }
                            else {
                                commonService.getStaffBranchHead("Division", branch).then(function (response) {
                                    $scope.loaded = false; //spinner stop  
                                    if (response.data.d.results.length > 0) {
                                        if (response.data.d.results[0].Branch != null && response.data.d.results[0].Branch != "" && response.data.d.results[0].Branch != undefined) {
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
        //insert/update CRSRRequests 
        function insertUpdateListItem(Worfkflowstatus, ApplicationStatus, commentStatus, message) {
            //Save/update  -get list                 
            $scope.loaded = true;
            var list = web.get_lists().getByTitle("" + Config.HRRequests + "");
            var listItem = "";
            var startDate = $filter("date")(new Date(), 'MM/dd/yyyy');
            var endDate = $filter("date")(new Date(), 'MM/dd/yyyy');
            
            if ($scope.appProperties.HRRequests.ID == "" || $scope.appProperties.HRRequests.ID == undefined) {
                //insert data to sharepoint
                var listCreationInformation = new SP.ListItemCreationInformation();
                listItem = list.addItem(listCreationInformation);
                listItem.set_item("RequestorID", $scope.appProperties.CurrentUser.LoginName);
                listItem.set_item("Title", $scope.appProperties.HRRequests.ApplicationID);
                listItem.set_item("ApplicationID", $scope.appProperties.HRRequests.ApplicationID);
                listItem.set_item("Requestor", $scope.appProperties.CurrentUser.Id);
                listItem.set_item("ManpowerRequestType", $scope.appProperties.HRRequests.ManpowerRequestType);
                listItem.set_item("ApplicationDate", $scope.appProperties.HRRequests.ApplicationDate);
                listItem.set_item("Level1Approver", $scope.appProperties.HRRequests.Level1Approver.Id);
                listItem.set_item("Level1ApprovalStatus", "Pending");
                // datefield 

            } else {

                //ctx.load(listItem);
                //
                listItem = list.getItemById($scope.appProperties.HRRequests.ID);
            }
          
            //Setting new Request Item
            if ($scope.appProperties.HRRequests.ManpowerRequestType == "New Position" || $scope.appProperties.HRRequests.ManpowerRequestType == "Replacement") {
                listItem.set_item("ManpowerType", $scope.appProperties.HRRequests.ManpowerType);
                listItem.set_item("ReplacementFullname", $scope.appProperties.HRRequests.ReplacementFullname);
                listItem.set_item("JobDesignation", $scope.appProperties.HRRequests.JobDesignation);
                listItem.set_item("DeploymentDuration", $scope.appProperties.HRRequests.DeploymentDuration);
                listItem.set_item("NoOfManpower", $scope.appProperties.HRRequests.NoOfManpower);
                listItem.set_item("WorkLocation", $scope.appProperties.HRRequests.WorkLocation);
                listItem.set_item("ComputerRequirement", $scope.appProperties.HRRequests.ComputerRequirement);
                listItem.set_item("SupportingOfficer", $scope.appProperties.HRRequests.SupportingOfficer.Id);
                listItem.set_item("ProposedBuddy", $scope.appProperties.HRRequests.ProposedBuddy.Id);
                listItem.set_item("RequiredAccessToSecretFiles", $scope.appProperties.HRRequests.AccessToSecretFiles);
                listItem.set_item("HandPhoneSubsidy", $scope.appProperties.HRRequests.HandPhoneSubsidy);
                listItem.set_item("RosterOffday", $scope.appProperties.HRRequests.RosterOffday);
                listItem.set_item("FundingType", $scope.appProperties.HRRequests.FundingType);
                listItem.set_item("FundingName", $scope.appProperties.HRRequests.FundingName);
                listItem.set_item("FundCentre", $scope.appProperties.HRRequests.FundCentre);
                listItem.set_item("CostCentre", $scope.appProperties.HRRequests.CostCentre);
                //listItem.set_item("FundingDuration", $scope.appProperties.HRRequests.FundingDuration);
            } else if ($scope.appProperties.HRRequests.ManpowerRequestType == "Intern") {
                listItem.set_item("NoOfInternRequired", $scope.appProperties.HRRequests.NoOfInternRequired);
                listItem.set_item("NameOfInstitution", $scope.appProperties.HRRequests.NameOfInstitution);
                listItem.set_item("NameOfCourse", $scope.appProperties.HRRequests.NameOfCourse);
                if ($scope.appProperties.HRRequests.InternStartDate != null && $scope.appProperties.HRRequests.InternStartDate != undefined) {
                    var dateParts = $scope.appProperties.HRRequests.InternStartDate.split("/");
                    // month is 0-based, that's why we need dataParts[1] - 1
                    var DteCompletion = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                    listItem.set_item("InternStartDate", DteCompletion);
                }
                if ($scope.appProperties.HRRequests.InternEndDate != null && $scope.appProperties.HRRequests.InternEndDate != undefined) {
                    var dateParts = $scope.appProperties.HRRequests.InternStartDate.split("/");
                    // month is 0-based, that's why we need dataParts[1] - 1
                    var DteCompletion = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                    listItem.set_item("NewColumn1", DteCompletion);
                }
                listItem.set_item("WorkingHour", $scope.appProperties.HRRequests.WorkingHour);
                listItem.set_item("TempAccessCardRequired", $scope.appProperties.HRRequests.TempAccessCardRequired);
                listItem.set_item("FilesAccessCardRequired", $scope.appProperties.HRRequests.FilesAccessCardRequired);
                listItem.set_item("InterviewRequired", $scope.appProperties.HRRequests.InternInterviewRequired);
                listItem.set_item("RosterOffday", $scope.appProperties.HRRequests.RosterOffday);
                listItem.set_item("JobDescription", $scope.appProperties.HRRequests.JobDescription);
                listItem.set_item("LearningOutcome", $scope.appProperties.HRRequests.LearningOutcome);
                listItem.set_item("OtherRequirement", $scope.appProperties.HRRequests.OtherRequirement);
                listItem.set_item("DivisionToAttach", $scope.appProperties.HRRequests.DivisionToAttach);
                listItem.set_item("BranchToAttach", $scope.appProperties.HRRequests.BranchToAttach);

                listItem.set_item("PrimaryAdvisor", $scope.appProperties.HRRequests.PrimarySupervisor.Id);
                listItem.set_item("SecondaryAdvisor", $scope.appProperties.HRRequests.SecondarySupervisor.Id);

                listItem.set_item("FundingType", $scope.appProperties.HRRequests.FundingType);
                listItem.set_item("FundingName", $scope.appProperties.HRRequests.FundingName);
                listItem.set_item("FundCentre", $scope.appProperties.HRRequests.FundCentre);
                listItem.set_item("CostCentre", $scope.appProperties.HRRequests.CostCentre);
                //listItem.set_item("FundingDuration", $scope.appProperties.HRRequests.FundingDuration);
                listItem.set_item("WorkLocation", $scope.appProperties.HRRequests.WorkLocation);

                listItem.set_item("isInternOther", $scope.appProperties.HRRequests.isInternOther); //chekcbox
                listItem.set_item("InternOtherText", $scope.appProperties.HRRequests.InternOtherText);//other text


            } else if ($scope.appProperties.HRRequests.ManpowerRequestType == "Temporary") {
                listItem.set_item("NoOfManpower", $scope.appProperties.HRRequests.NoOfTempStaff);
                listItem.set_item("TempDurationRequired", $scope.appProperties.HRRequests.TempDurationRequired);
                listItem.set_item("Justification", $scope.appProperties.HRRequests.Justification);
                listItem.set_item("JobDescription", $scope.appProperties.HRRequests.JobDescription);
                listItem.set_item("Degree", $scope.appProperties.HRRequests.Degree);
                listItem.set_item("Diploma", $scope.appProperties.HRRequests.Diploma);
                listItem.set_item("NITEC", $scope.appProperties.HRRequests.NITEC);
                listItem.set_item("ALevel", $scope.appProperties.HRRequests.ALevel);
                listItem.set_item("OLevel", $scope.appProperties.HRRequests.OLevel);
                listItem.set_item("NLevel", $scope.appProperties.HRRequests.NLevel);
                listItem.set_item("Skillsets", $scope.appProperties.HRRequests.Skillsets);
                listItem.set_item("WorkingHour", $scope.appProperties.HRRequests.WorkingHour);
                listItem.set_item("WorkLocation", $scope.appProperties.HRRequests.WorkLocation);
                listItem.set_item("OtherRequirement", $scope.appProperties.HRRequests.OtherRequirement);
                listItem.set_item("TempAccessCardRequired", $scope.appProperties.HRRequests.TempAccessCard);
                listItem.set_item("RosterOffday", $scope.appProperties.HRRequests.RosterOffday);

                listItem.set_item("FundingType", $scope.appProperties.HRRequests.FundingType);
                listItem.set_item("FundingName", $scope.appProperties.HRRequests.FundingName);
                listItem.set_item("FundCentre", $scope.appProperties.HRRequests.FundCentre);
                listItem.set_item("CostCentre", $scope.appProperties.HRRequests.CostCentre);
                //listItem.set_item("FundingDuration", $scope.appProperties.HRRequests.FundingDuration);

                listItem.set_item("isTemporaryOther", $scope.appProperties.HRRequests.isTemporaryOther); //chekcbox
                listItem.set_item("WorkingHourOtherText", $scope.appProperties.HRRequests.WorkingHourOtherText);//other text

            }
                //Approver
                if (Worfkflowstatus == 1) {
                    listItem.set_item("Level1Approver", $scope.appProperties.HRRequests.Level1Approver.Id);
                    listItem.set_item("Level1ApprovalStatus", $scope.appProperties.HRRequests.Level1ApprovalStatus);
                    listItem.set_item("Level2ApprovalStatus", $scope.appProperties.HRRequests.Level2ApprovalStatus);
                   // listItem.set_item("Level3ApprovalStatus", $scope.appProperties.HRRequests.Level3ApprovalStatus);
                } else if (Worfkflowstatus == 2) {
                    listItem.set_item("ApprovalRejectionComments", $scope.appProperties.HRRequests.ApprovalRejectionComments);
                    if ($scope.appProperties.HRRequests.Level2Approver != undefined) {
                        if ($scope.appProperties.HRRequests.Level2Approver.Id) {
                            listItem.set_item("Level2Approver", $scope.appProperties.HRRequests.Level2Approver.Id);
                        }
                    }
                    //if ($scope.appProperties.HRRequests.Level3Approver.Id != "" && $scope.appProperties.HRRequests.Level3Approver.Id != undefined)
                    //    listItem.set_item("Level3Approver", $scope.appProperties.HRRequests.Level3Approver.Id);
                }
                listItem.set_item("Level1ApprovalStatus", $scope.appProperties.HRRequests.Level1ApprovalStatus);
                listItem.set_item("Level2ApprovalStatus", $scope.appProperties.HRRequests.Level2ApprovalStatus);
                //listItem.set_item("Level3ApprovalStatus", $scope.appProperties.HRRequests.Level3ApprovalStatus);
                if ($scope.appProperties.HRRequests.Level1ApprovalDate != "" && $scope.appProperties.HRRequests.Level1ApprovalDate !=undefined)
                    listItem.set_item("Level1ApprovalDate", $scope.appProperties.HRRequests.Level1ApprovalDate);
                if ($scope.appProperties.HRRequests.Level2ApprovalDate != "" && $scope.appProperties.HRRequests.Level2ApprovalDate != undefined)
                    listItem.set_item("Level2ApprovalDate", $scope.appProperties.HRRequests.Level2ApprovalDate);
                //if ($scope.appProperties.HRRequests.Level3ApprovalDate != "" && $scope.appProperties.HRRequests.Level2ApprovalDate != undefined)
                //    listItem.set_item("Level3ApprovalDate", $scope.appProperties.HRRequests.Level3ApprovalDate);
            

            if ($scope.appProperties.HRRequests.RequestedFor != undefined) {
                if ($scope.appProperties.HRRequests.RequestedFor.Id) {
                    listItem.set_item("RequestedFor", $scope.appProperties.HRRequests.RequestedFor.Id);
                    listItem.set_item("RequestedForLoginID", $scope.appProperties.HRRequests.RequestedFor.LoginName);
                }
            } 
            listItem.set_item("OnBehalf", $scope.appProperties.HRRequests.OnBehalf);
            //workflow general status                  
            listItem.set_item("ApplicationStatus", ApplicationStatus);
            //listItem.set_item("WorkFlowStatus", Worfkflowstatus);
            
            listItem.update();
           
            
            ctx.load(listItem);
            
            ctx.executeQueryAsync(function () {
                try {
                    var redirectUrl = "/erequest/Pages/HReMRServiceDashboard.aspx"; 
                    $scope.appProperties.HRRequests.ID = listItem.get_id();
                    insertComments(listItem.get_id(), ApplicationStatus);
                    //listName = "ProposedTemporaryWorker";
                    if (Utility.helpers.getUrlParameter('ReqId') == "") {
                        if ($scope.appProperties.HRRequests.TemporaryCandidates.length > 0) {
                            //if there is inters added then insert
                            angular.forEach($scope.appProperties.HRRequests.TemporaryCandidates, function (value, key) {
                                if (value.FileName != "") {
                                    insertCandidates($scope.appProperties.HRRequests.ID, "ProposedTemporaryWorker", value, key, true, value.Files);
                                }
                                else {
                                    insertCandidates($scope.appProperties.HRRequests.ID, "ProposedTemporaryWorker", value, key, false, value.Files);
                                }
                            });
                        }
                        //listName = "ProposedInterns";
                        if ($scope.appProperties.HRRequests.InternContactInfos.length > 0) {
                            //if there is inters added then insert
                            angular.forEach($scope.appProperties.HRRequests.InternContactInfos, function (value, key) {
                                if (value.ResumeFile != "" && value.ResumeFile != undefined) {
                                    insertInternCandidates($scope.appProperties.HRRequests.ID, "ProposedInterns", value, key, true, value.internFiles._file);
                                }
                                else {
                                    insertInternCandidates($scope.appProperties.HRRequests.ID, "ProposedInterns", value, key, false, null);
                                }
                            });
                        }
                    }
                 
                    //sendEmail to approver                 
                    switch (Worfkflowstatus) {
                        case 1:
                            //send email to branchHead/user- Submitted for approval
                                bodyMsg = getBodyMsg($scope.appProperties.HRRequests.Title, "This is to notify you that a IT Service Request application");
                                //alert("Curent user email:" +$scope.appProperties.CurrentUser.Email);
                            //Utility.helpers.SendGMailService(hostName, portNumber, true, password, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "HR eMRF Application Submitted req - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF Application " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "HR eMRF Application Submitted  - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            bodyMsg = getBodyMsg($scope.appProperties.HRRequests.Title, "Pending for your approval");
                            Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Level1Approver.EMail, "", "", "HR eMRF Application Submitted - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");

                                // alert("Approver email : " + $scope.appProperties.HRRequests.Level1Approver.EMail);
                              
                            
                            break;
                        case 2:
                        case 3:
                            //send email to ITManger/user- After Approved by BranchHead
                            if (Worfkflowstatus == '2') { //approved email

                                angular.forEach($scope.appProperties.HReMRApprovers, function (value, key) {
                                    angular.forEach(value.Approver.results, function (value, key) {
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, value.EMail, "", "", "HR eMRF Application Submitted - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please access the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                    });
                                });
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Rejected - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");

                            }
                            break;
                        case 4:
                        case 5:
                            //send email to GroupDirc/user- After Approved by ITManger
                            if (Worfkflowstatus == '4') {
                                //alert("MAil sent to " + $scope.appProperties.CurrentUser.Email + " Status :" + Worfkflowstatus);
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application closed - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been closed. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "HR eMRF Application closed - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF Application " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                //Utility.helpers.sendEmail($scope.appProperties.Level3Approver.EMail, "HR eMRF Application closed - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF Application " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.Level3Approver.EMail, "", "", "HR eMRF Application closed - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been closed. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            }
                            else {
                                //Re-Routed email
                                //Utility.helpers.sendEmail($scope.appProperties.CurrentUser.Email, "HR eMRF Service Request re-routed - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF Application " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been re routed. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Service Request Re-routed - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been re routed. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        case 6:
                        case 7:
                            //send email to CIO/user- After Approved by GroupDirecotr
                            if ($scope.appProperties.CRSRRequests.CIO.Id && Worfkflowstatus == '6') {
                                //alert("MAil sent to " + $scope.appProperties.CurrentUser.Email + " Status :" + Worfkflowstatus);
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Submitted - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                //Utility.helpers.SendGMailService($scope.appProperties.CIO.EMail, "HR eMRF Service Request Submitted - " + $scope.appProperties.CurrentUser.Title + "", "This is to notify you that a HR eMRF Request " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.CurrentUser.Title + " has been submitted. Please click here to <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>endorse the application.</a>");
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Rejected - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        case 8:
                        case 9:
                            //send email to Imple/user- After Approved by CIO
                            if ($scope.appProperties.CRSRRequests.ImplementationOfficer.Id && Worfkflowstatus == '8') {
                                //alert("MAil sent to " + $scope.appProperties.CurrentUser.Email + " Status :" + Worfkflowstatus);
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Submitted - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                                //Utility.helpers.SendGMailService($scope.appProperties.ImplementationOfficer.EMail, "HR eMRF Service Request Submitted - " + $scope.appProperties.CurrentUser.Title + "", "This is to notify you that a HR eMRF Request " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.CurrentUser.Title + " has been submitted. Please click here to <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>endorse the application.</a>");
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Rejected - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        case 10:
                        case 11:
                            //send email to user- After Approved by implem -final
                            if (Worfkflowstatus == '10') {
                                //alert("MAil sent to " + $scope.appProperties.CurrentUser.Email + " Status :" + Worfkflowstatus);
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Submitted - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.HRRequests.Author.EMail, "", "", "HR eMRF Application Rejected - " + $scope.appProperties.HRRequests.Author.Title + "", "This is to notify you that a HR eMRF " + $scope.appProperties.HRRequests.ManpowerRequestType + " Application  " + $scope.appProperties.HRRequests.ApplicationID + " from " + $scope.appProperties.HRRequests.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx>here.</a>");
                            }
                            break;
                        default:
                        //alert('Invalid case');
                    }//switch 
                    
                  
                }
                catch (err) { //do some 
                    console.log("Error: " + err);
                    
                }
              
                    if ($scope.appProperties.appFiles.length > 0) {
                        insertAttachedDocuments($scope.appProperties.HRRequests.ID, Config.HRDocuments, message, redirectUrl);
                    }
                    else {
                        setTimeout(function () {
                            alert(message);
                            window.location.href = _spPageContextInfo.siteAbsoluteUrl + "/Pages/HReMRServiceDashboard.aspx";
                        }, 6000); // Execute something() 6 second later.                      
                    }
                
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };
        //inserting proposed candidates
        function insertCandidatesx(listTitle,parentId, itemsProperties, OnItemsAdded, OnItemsError) {
            var context = new SP.ClientContext.get_current();
            var myweb = context.get_web();
            var list = myweb.get_lists().getByTitle(listTitle);
            var listItem = "";
            var items = [];
            $.each(itemsProperties, function (i, itemProperties) {

                var itemCreateInfo = new SP.ListItemCreationInformation();
                listItem = list.addItem(itemCreateInfo);
                for (var propName in itemProperties) {
                    if (propName != "$$hashKey")
                        listItem.set_item(propName, itemProperties[propName]);
                    listItem.set_item("HRParentID", parentId);
                    listItem.update();
                    context.load(listItem);
                    items.push(listItem);
                  
                } 
            });
            
            context.executeQueryAsync(
                function () {
                    OnItemsAdded(items);
                   
                },
                OnItemsError
            );
        }
        //insert comments
      
        function insertCandidates(listItemId, ListName, data, count,isExitFile,files) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle(ListName);
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";                
                clistItem = clist.addItem(clistItemInfo);
                clistItem.set_item('Title', data.Title);
                clistItem.set_item('ContactNo', data.ContactNo);
                clistItem.set_item('Email', data.Email);
                clistItem.set_item('FileName', data.FileName);
                clistItem.set_item('HRParentID', listItemId);
                clistItem.update();
                ctx.load(clistItem);
                ctx.executeQueryAsync(function () {
                    $scope.loaded = false; 
                    //ListName, ListItemId, tempfiles
                    if (isExitFile) {
                        $scope.saveAttachment(ListName, clistItem.get_id(), files[0]);
                    }
                    console.log("attch -inserted")
                }, function (sender, args) {
                    console.log("something went wrong");
                    console.log('Err: ' + args.get_message());
                });
            
        };  
        function insertInternCandidates(listItemId, ListName, data, count, isExitFile, files) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle(ListName);
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            clistItem.set_item('Title', data.NameOfIntern);
            clistItem.set_item('ContactNo', data.InternContactNo);
            clistItem.set_item('Email', data.InternEmail);
            if (data.ResumeFile != "" && data.ResumeFile != undefined) {
                clistItem.set_item('FileName', data.ResumeFile);
            }
            clistItem.set_item('HRParentID', listItemId);
            clistItem.update();
            ctx.load(clistItem);
            ctx.executeQueryAsync(function () {
                $scope.loaded = false;
                //ListName, ListItemId, tempfiles
                if (isExitFile) {
                    $scope.saveAttachment(ListName, clistItem.get_id(), files);
                }
                console.log("attch -inserted")
            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });

        };

        //Finding Level1 Approver
        function getLevel1Approver() {
            
            commonService.getStaffInfoByAdId($scope.appProperties.CurrentUser.LoginName).then(function (response) {
                $scope.loaded = false;
                if (response.data.d.results.length > 0) {
                    commonService.getStaffDivisionHead(response.data.d.results[0].Division).then(function (response) {
                        $scope.loaded = false;
                        if (response.data.d.results.length > 0) {
                            
                            $scope.appProperties.HRRequests.Level1Approver = response.data.d.results[0].DivisionHead;
                        }
                    });
                }
            });
        }
        //get randon number  -text-year-Randomnumber-lastitemid(2019_001)
        function getUniqueNumber() {
            //Generate Document Numbr -getting NTP lasitem id
            uService.getLastListITem().then(function (response) {
                if (response.data.d.results.length > 0) {
                    
                    var count = response.data.d.results[0].Id + 1;
                    $scope.appProperties.HRRequests.ApplicationID = "HR-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + count; //increment one
                }
                else {
                    $scope.appProperties.HRRequests.ApplicationID = "HR-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + "1"
                }
            });

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

        function getManpowerRequestType() {
            $scope.loaded = true; //spinner start -service call start
            uService.getManpowerRequestType().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ManpowerRequestTypes = response.data.d.results;
                }
            });
        };

        function getManpowerType() {
            $scope.loaded = true; //spinner start -service call start
            uService.getManpowerType().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ManpowerTypes = response.data.d.results;
                }
            });
        };

        function getDeploymentDuration() {
            $scope.loaded = true; //spinner start -service call start
            uService.getDeploymentDuration().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.DeploymentDurations = response.data.d.results;
                }
            });
        };

        function getComputerRequirement() {
            $scope.loaded = true; //spinner start -service call start
            uService.getComputerRequirement().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ComputerRequirements = response.data.d.results;
                }
            });
        };

        function getHRFundingTypes() {
            $scope.loaded = true; //spinner start -service call start
            uService.getHRFundingTypes().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.FundingTypes = response.data.d.results;
                }
            });
        };
        function getAllDivisionAndBranch() {
            $scope.loaded = true; //spinner start -service call start
            uService.getAllDivisionsAndBranches().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    var divisions = groupBy(response.data.d.results, 'Division');
                  
                    $scope.appProperties.Divisions = divisions;
                    var branches = groupBy(response.data.d.results, 'Branch');
                   
                    $scope.appProperties.Branches = branches;
                }
            });
        }
        //find unique values
        function groupBy(items, propertyName) {
            var temp = [];
            var result = [];
            $.each(items, function (index, item) {
                if (temp.indexOf(item[propertyName]) == -1) {
                    temp.push(item[propertyName]);
                    result.push({ Title: item[propertyName] });
                }
            });
            return result;
        }
        //Search HR request
        //Search...
        $scope.search = function () {
            if (($scope.appProperties.ApplicationEndDate != "" && $scope.appProperties.ApplicationEndDate != null) && ($scope.appProperties.ApplicationDate != "" && $scope.appProperties.ApplicationDate != null)) {
                $scope.appProperties.ApplicationStatus = ""; $scope.appProperties.RequestTypeCode = ""; $scope.appProperties.ApplicationID = "";
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
                $scope.filteredList = commonService.searchedCRSRReqEquiry($scope.CRSRRequestData, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = "";
            }
            else if ($scope.appProperties.RequestTypeCode) {
                $scope.searchText = $scope.appProperties.RequestTypeCode;
                $scope.filteredList = commonService.searchedCRSRReqEquiry($scope.CRSRRequestData, $scope.searchText, null, null);
                $scope.filteredItems = $scope.filteredList;
                $scope.searchText = ""; //after search clear text
            }

            else if ($scope.searchText != "") {
                $scope.filteredList = commonService.searchedCRSRReqEquiry($scope.CRSRRequestData, $scope.searchText, null, null);
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
        function getUATStatus() {
            $scope.loaded = true; //spinner start -service call start
            uService.getUATStatus().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.UATStatus = response.data.d.results;
                }
            });
        };

        function getApplicationStatus() {
            $scope.loaded = true; //spinner start -service call start
            uService.getApplicationStatus().then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ApplicationStatus = response.data.d.results;
                }
            });
        };
        //Getting approvers
        function getApprovers() {
            
            $scope.loaded = true; //spinner start -service call start
            uService.getApprovers().then(function (response) {
                
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.HReMRApprovers = response.data.d.results;
                }
            });
        };

        function getLevelApprover(lvlApprover) {
          
            angular.forEach($scope.appProperties.HReMRApprovers, function (value, key) {
                if (value.Title == lvlApprover) {
                    angular.forEach(value.Approver.results, function (value, key) {
                        if (value.Id == $scope.appProperties.CurrentUser.Id)
                            $scope.appProperties.HRRequests.Level2Approver = value;
                    });
                }       
            });
        }
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
                        }
                        if (value.Title == Config.Roles['ImplementationOfficer']) {
                            $scope.appProperties.ImplementationOfficerMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['GroupDirector']) {
                          //  $scope.appProperties.GroupDirectorMaster = value.Approvers.results;
                        }
                        if (value.Title == Config.Roles['CIO']) {
                            $scope.appProperties.CIOMaster = value.Approvers.results;
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
                    if (file.size >= 2109440) {
                        alert("Filesize should not exceed 2MB");
                        $scope.loaded = false;
                    }
                    else {
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
                                case 'doc':
                                case 'docx':
                                case 'txt':
                                case 'png':
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
                }

                $scope.$apply();//event callback.
            }

        };
        //remove uploded one
        $scope.removeFiles = function (index, Title) {
            //Find the file using Index from Array.         
            if ($window.confirm("Do you want to remove " + Title)) {
                if ($scope.appProperties.CRSRRequests != undefined && $scope.appProperties.CRSRRequests.ID != null && $scope.appProperties.CRSRRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.CRSRRequests.ID).then(function (response) {
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
                    $scope.appProperties.appFileProperties.splice(index, 1);
                    $scope.appProperties.appFiles.splice(index, 1);                  
                }
            }
        };
        //insert attached files-to folder list for HReMRF
        function insertAttachedDocuments(listItemId, docLib, message, redirectUrl) {
            
            if ($scope.appProperties.appFiles.length > 0) {
                var oList = web.get_lists().getByTitle(Config.HRDocuments);
                
                uService.getDocLibByFolderName(listItemId, docLib).then(function (response) {
                    if (response.data.d.results.length > 0) {
                        
                        //alread exist folder      
                        uploadFile(listItemId, docLib, $scope.appProperties.appFiles, redirectUrl, message);
                    }
                    else {
                        // Folder doesn't exist at all. so create folder here
                        var oListItem = oList.get_rootFolder().get_folders().add(listItemId); //foldername used by PJApproval listItemID
                        ctx.load(oListItem);
                        ctx.executeQueryAsync(function () {
                            uploadFile(listItemId, docLib, $scope.appProperties.appFiles, redirectUrl, message);
                            
                        }, function (sender, args) {
                            console.log("something went wrong");
                            console.log('Err: ' + args.get_message());
                        });
                    }
                });
            }
        };
function uploadFile(listItemId, docLibName, fileInput, redirectUrl, message) {
            // Define the folder path for this example.
            // var targetUrl = _spPageContextInfo.webServerRelativeUrl + "/" + documentLibrary + "/" + listItemId;
            var serverRelativeUrlToFolder = '/erequest/' + docLibName + '/' + listItemId;
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
                    "/_api/Web/lists/GetByTitle('" + Config.HRDocuments + "')/items(" + itemId + ")";
                $.ajax({
                    //url:fullUrl,
                    url: response.d.ListItemAllFields.__metadata.uri,
                    type: 'POST',
                    data: JSON.stringify({
                        '__metadata': { type: response.d.ListItemAllFields.__metadata.type },
                        'HRRequestID': listItemId

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
                            case 'doc':
                            case 'docx':
                            case 'txt':
                            case 'png':
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
                if ($scope.appProperties.CRSRRequests.ID != null && $scope.appProperties.CRSRRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.CRSRRequests.ID).then(function (response) {
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
                            case 'doc':
                            case 'docx':
                            case 'txt':
                            case 'png':
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
                if ($scope.appProperties.CRSRRequests.ID != null && $scope.appProperties.CRSRRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.CRSRRequests.ID).then(function (response) {
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
                            case 'doc':
                            case 'docx':
                            case 'txt':
                            case 'png':
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
                if ($scope.appProperties.CRSRRequests.ID != null && $scope.appProperties.CRSRRequests.ID != "") {
                    $scope.loaded = true; //spinner start -service call start
                    uService.deleteFilebyDocTitle(Title, $scope.appProperties.CRSRRequests.ID).then(function (response) {
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
      
        
        // Display error messages. 
        function onError(error) {
            console.log(error.responseText);
        }

        //insert comments
        function insertComments(listItemId, status) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.HRComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            if ($scope.appProperties.appComments.Comments != null && $scope.appProperties.appComments.Comments != "") {
                clistItem.set_item('UserComments', $scope.appProperties.appComments.Comments + " ( " + Config.HRCommentStatus[status] + " )");
            } else {
                clistItem.set_item('UserComments', Config.HRCommentStatus[status] + " (System Comments)");
            }
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('HRRequestID', listItemId);
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
        function loadHRRequestsListData(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getById(ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                      //setting the display based on Manpower Request Type
                   // $scope.appProperties.HRRequests = response.data.d.results[0];
                    $scope.selectRequestType(response.data.d.results[0].ManpowerRequestType);
                    $scope.appProperties.HRRequests.OnBehalf = response.data.d.results[0].OnBehalf;
                 
                    $scope.appProperties.HRRequests.ApplicationID = response.data.d.results[0].ApplicationID;
                    $scope.appProperties.HRRequests.Author.Id = response.data.d.results[0].Requestor.Id;
                    $scope.appProperties.HRRequests.Level1Approver.Id = response.data.d.results[0].Level1Approver.Id;
                    $scope.appProperties.HRRequests.Level2Approver.Id = response.data.d.results[0].Level2Approver.Id;
                    $scope.appProperties.HRRequests.Level1Approver.EMail = response.data.d.results[0].Level1Approver.EMail;
                    $scope.appProperties.HRRequests.Level2Approver.EMail = response.data.d.results[0].Level2Approver.EMail;
                    
                  
                    if (response.data.d.results[0].Requestor.Name !== '' || response.data.d.results[0].Requestor.Name != undefined){
                        $scope.appProperties.HRRequests.Author.LoginName = response.data.d.results[0].Requestor.Name.split('\\')[1];
                        $scope.appProperties.HRRequests.Author.EMail = response.data.d.results[0].Requestor.EMail;
                    }
                    if (response.data.d.results[0].Requestor.Title !== '' || response.data.d.results[0].Requestor.Title != undefined){
                        $scope.appProperties.HRRequests.Author.Title = response.data.d.results[0].Requestor.Title;
                    }
                    
                    if (response.data.d.results[0].RequestedFor != null) {
                        $scope.appProperties.HRRequests.RequestedFor.LoginName = response.data.d.results[0].RequestedFor.LoginName;
                        $scope.appProperties.HRRequests.RequestedFor.Title = response.data.d.results[0].RequestedFor.Title;
                      
                    }
                    $scope.appProperties.HRRequests.ManpowerRequestType = response.data.d.results[0].ManpowerRequestType;
                  
                    if (response.data.d.results[0].ManpowerRequestType == "New Position" || response.data.d.results[0].ManpowerRequestType == "Replacement") {
                        //$scope.appProperties.HRRequests.ManpowerRequestType = response.data.d.results[0].ManpowerRequestType;
                        $scope.appProperties.HRRequests.ReplacementFullname = response.data.d.results[0].ReplacementFullname;
                        $scope.appProperties.HRRequests.ManpowerType = response.data.d.results[0].ManpowerType;
                        $scope.appProperties.HRRequests.JobDesignation = response.data.d.results[0].JobDesignation;
                        $scope.appProperties.HRRequests.DeploymentDuration = response.data.d.results[0].DeploymentDuration;
                        $scope.appProperties.HRRequests.NoOfManpower = response.data.d.results[0].NoOfManpower;
                        $scope.appProperties.HRRequests.WorkLocation = response.data.d.results[0].WorkLocation;
                        $scope.appProperties.HRRequests.ComputerRequirement = response.data.d.results[0].ComputerRequirement;
                        $scope.selectRequestManType(response.data.d.results[0].ManpowerType);

                        $scope.appProperties.HRRequests.SupportingOfficer.Title = response.data.d.results[0].SupportingOfficer.Title;
                        $scope.appProperties.HRRequests.ProposedBuddy.Title = response.data.d.results[0].ProposedBuddy.Title;

                       // var divSupportingOfficer = SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerSupportingOfficer_TopSpan;
                        //PeoplePickerDiv_TopSpan - Specify the unique ID of the DOM element where the picker will render.
                       // var upportingOfficerObj = { 'Id': response.data.d.results[0].SupportingOfficer.Id, 'Key': response.data.d.results[0].SupportingOfficer.Name};
                      //  divSupportingOfficer.AddUnresolvedUser(upportingOfficerObj, true);
                      //  var divProposedBuddy = SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerProposedBuddy_TopSpan;
                        //PeoplePickerDiv_TopSpan - Specify the unique ID of the DOM element where the picker will render.
                        var proposedBuddyObj = { 'Title': response.data.d.results[0].ProposedBuddy.Title };
                        var upportingOfficerObj = { 'Title': response.data.d.results[0].SupportingOfficer.Title };
                        //divProposedBuddy.AddUnresolvedUser(proposedBuddyObj, true);
                         //$scope.appProperties.HRRequests.SupportingOfficer.Id = response.data.d.results[0].SupportingOfficer.Id;
                        //$scope.appProperties.HRRequests.ProposedBuddy.Id = response.data.d.results[0].ProposedBuddy.Id;

                        ////in this field we allow the PeoplePicker column - People and Groups and Allowmultiple=true
                        //angular.forEach(upportingOfficerObj, function (value, key) {
                        //    Utility.helpers.SetUserFieldValue('peoplePickerSupportingOfficer', value.Title);
                        //});
                        ////in this field we allow the PeoplePicker column - People and Groups and Allowmultiple=true
                        //angular.forEach(proposedBuddyObj, function (value, key) {
                        //    Utility.helpers.SetUserFieldValue('peoplePickerProposedBuddy', value.Title);
                        //});

                        //people only -get login name and pass the value instead of title(find exact match)
                        uService.getUser(response.data.d.results[0].SupportingOfficer.Id).then(function (response) {
                            Utility.helpers.SetUserFieldValue('peoplePickerSupportingOfficer', response.data.d.LoginName);
                        });
                        uService.getUser(response.data.d.results[0].ProposedBuddy.Id).then(function (response) {
                            Utility.helpers.SetUserFieldValue('peoplePickerProposedBuddy', response.data.d.LoginName);
                        });

                        $scope.appProperties.HRRequests.AccessToSecretFiles = response.data.d.results[0].RequiredAccessToSecretFiles;
                        $scope.appProperties.HRRequests.HandPhoneSubsidy = response.data.d.results[0].HandPhoneSubsidy;
                        $scope.appProperties.HRRequests.RosterOffday = response.data.d.results[0].RosterOffday;
                    } else if (response.data.d.results[0].ManpowerRequestType == "Intern") {
                        console.log("Intern");
                        $scope.appProperties.HRRequests.NoOfInternRequired = response.data.d.results[0].NoOfInternRequired;
                        $scope.appProperties.HRRequests.NameOfInstitution = response.data.d.results[0].NameOfInstitution;
                        $scope.appProperties.HRRequests.NameOfCourse = response.data.d.results[0].NameOfCourse;
                        $scope.appProperties.HRRequests.InternStartDate = $filter("date")(response.data.d.results[0].InternStartDate, "dd-MMM-yyyy"); 
                        $scope.appProperties.HRRequests.InternEndDate = $filter("date")(response.data.d.results[0].NewColumn1, "dd-MMM-yyyy");
                        $scope.appProperties.HRRequests.WorkLocation = response.data.d.results[0].WorkLocation;
                        $scope.appProperties.HRRequests.WorkingHour = response.data.d.results[0].WorkingHour;
                        $scope.appProperties.HRRequests.InternOtherText = response.data.d.results[0].InternOtherText;
                        angular.forEach($scope.entities, function (subscription, index) {
                            if ($scope.appProperties.HRRequests.WorkingHour == "Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm" && subscription.name == "Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm") {
                                subscription.checked = true;                             
                            }
                            if ($scope.appProperties.HRRequests.WorkingHour == "Others" && subscription.name == "Others") {
                                subscription.checked = true;
                                $scope.isInternOther = true;                               
                            }

                        });
                        if (response.data.d.results[0].TempAccessCardRequired == "True") {
                            $scope.IsIntAccessCardRequired = true;
                            $('#chkIntTempAccessCard').prop('checked', true);
                        } else { $scope.IsIntAccessCardRequired =  false;}
                        $scope.appProperties.HRRequests.InternInterviewRequired = response.data.d.results[0].InterviewRequired;
                        $scope.appProperties.HRRequests.FilesAccessCardRequired = response.data.d.results[0].FilesAccessCardRequired;
                        $scope.appProperties.HRRequests.RosterOffday = response.data.d.results[0].RosterOffday;
                        $scope.appProperties.HRRequests.JobDescription = response.data.d.results[0].JobDescription;
                        $scope.appProperties.HRRequests.LearningOutcome = response.data.d.results[0].LearningOutcome;
                        $scope.appProperties.HRRequests.OtherRequirement = response.data.d.results[0].OtherRequirement;
                        $scope.appProperties.HRRequests.DivisionToAttach = response.data.d.results[0].DivisionToAttach;
                        $scope.appProperties.HRRequests.BranchToAttach = response.data.d.results[0].BranchToAttach;
                        $scope.appProperties.HRRequests.SecondarySupervisor = response.data.d.results[0].SecondaryAdvisor;
                        $scope.appProperties.HRRequests.PrimarySupervisor = response.data.d.results[0].PrimaryAdvisor;
                        //var divPrimarySupervisor = SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerPrimarySupervisor_TopSpan;
                        //PeoplePickerDiv_TopSpan - Specify the unique ID of the DOM element where the picker will render.
                       // var primarySupervisorObj = { 'Id': response.data.d.results[0].PrimaryAdvisor.Id, 'Key': response.data.d.results[0].PrimaryAdvisor.Name };
                       // divPrimarySupervisor.AddUnresolvedUser(primarySupervisorObj, true);
                       // var divSecondarySupervisor = SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerSecondarySupervisor_TopSpan;
                        //PeoplePickerDiv_TopSpan - Specify the unique ID of the DOM element where the picker will render.
                      //  var secondarySupervisorObj = { 'Id': response.data.d.results[0].SecondaryAdvisor.Id, 'Key': response.data.d.results[0].SecondaryAdvisor.Name };
                      //  divSecondarySupervisor.AddUnresolvedUser(secondarySupervisorObj, true);
                        $scope.appProperties.HRRequests.FundingType = response.data.d.results[0].FundingType;
                        $scope.appProperties.HRRequests.FundingName = response.data.d.results[0].FundingName;
                        $scope.appProperties.HRRequests.FundCentre = response.data.d.results[0].FundCentre;
                        $scope.appProperties.HRRequests.CostCentre = response.data.d.results[0].CostCentre;
                        //$scope.appProperties.HRRequests.FundingDuration = response.data.d.results[0].FundingDuration;
                        
                        getInterns(ListItemId);
                        
                       
                    } else if (response.data.d.results[0].ManpowerRequestType == "Temporary") {
                        console.log("temporary");
                        if (response.data.d.results[0].TempAccessCardRequired == "Yes") {
                            $('#chkTempAccessCard').prop('checked', true);
                        } 
                        if (response.data.d.results[0].RosterOffday == "Yes") {
                            $('#chkTempRosteredOffdays').prop('checked', true);
                        } 
                        $scope.appProperties.HRRequests.NoOfTempStaff = response.data.d.results[0].NoOfManpower;
                        $scope.appProperties.HRRequests.TempDurationRequired = response.data.d.results[0].TempDurationRequired;
                        $scope.appProperties.HRRequests.Justification = response.data.d.results[0].Justification;
                        $scope.appProperties.HRRequests.JobDescription = response.data.d.results[0].JobDescription;
                        $scope.appProperties.HRRequests.Degree = response.data.d.results[0].Degree;
                        $scope.appProperties.HRRequests.Diploma = response.data.d.results[0].Diploma;
                        $scope.appProperties.HRRequests.NITEC = response.data.d.results[0].NITEC;
                        $scope.appProperties.HRRequests.ALevel = response.data.d.results[0].ALevel;
                        $scope.appProperties.HRRequests.OLevel = response.data.d.results[0].OLevel;
                        $scope.appProperties.HRRequests.NLevel = response.data.d.results[0].NLevel;
                        $scope.appProperties.HRRequests.Skillsets = response.data.d.results[0].Skillsets;
                        $scope.appProperties.HRRequests.WorkingHour = response.data.d.results[0].WorkingHour;
                        $scope.appProperties.HRRequests.WorkLocation = response.data.d.results[0].WorkLocation;
                        $scope.appProperties.HRRequests.OtherRequirement = response.data.d.results[0].OtherRequirement;
                        $scope.appProperties.HRRequests.TempAccessCard = response.data.d.results[0].TempAccessCardRequired;
                        $scope.appProperties.HRRequests.RosteredOffdays = response.data.d.results[0].RosterOffday;
                        $scope.appProperties.HRRequests.WorkingHour = response.data.d.results[0].WorkingHour;
                        angular.forEach($scope.entities, function (subscription, index) {
                            if ($scope.appProperties.HRRequests.WorkingHour == "Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm" && subscription.name == "Mon to Thurs: 8:30am to 6:00pm; Fri: 8:30am to 5:30pm") {
                                subscription.checked = true;
                            }
                            if ($scope.appProperties.HRRequests.WorkingHour == "Others" && subscription.name == "Others") {
                                subscription.checked = true;
                                $scope.isTemporaryOther = true;
                            }

                        });
                       
                        $scope.appProperties.HRRequests.WorkingHourOtherText = response.data.d.results[0].WorkingHourOtherText;
                        getTemporaryCandidates(ListItemId);
                    }
                    $scope.appProperties.HRRequests.FundingType = response.data.d.results[0].FundingType;
                    $scope.appProperties.HRRequests.FundingName = response.data.d.results[0].FundingName;
                    $scope.appProperties.HRRequests.FundCentre = response.data.d.results[0].FundCentre;
                    $scope.appProperties.HRRequests.CostCentre = response.data.d.results[0].CostCentre;
                    //$scope.appProperties.HRRequests.FundingDuration = response.data.d.results[0].FundingDuration;
                   

                    if (response.data.d.results[0].ApplicationStatus == 1) {
                        $scope.appProperties.HRRequests.ApplicationStatus = "Pending";
                    } else if (response.data.d.results[0].ApplicationStatus == 2) {
                        $scope.appProperties.HRRequests.ApplicationStatus = "Approved";
                    } else if (response.data.d.results[0].ApplicationStatus == 3) {
                        $scope.appProperties.HRRequests.ApplicationStatus = "Rejected";
                    } else if (response.data.d.results[0].ApplicationStatus == 4) {
                        $scope.appProperties.HRRequests.ApplicationStatus = "Closed";
                    }
                    else if (response.data.d.results[0].ApplicationStatus == 5) {
                        $scope.appProperties.HRRequests.ApplicationStatus = "Re-Routed";
                    }
                    $scope.appProperties.HRRequests.Level1ApprovalStatus = response.data.d.results[0].Level1ApprovalStatus;
                    $scope.appProperties.HRRequests.Level2ApprovalStatus = response.data.d.results[0].Level2ApprovalStatus;

                    //$scope.appProperties.CRSRRequests.ProdDataPatchRemarks = response.data.d.results[0].ProdDataPatchRemarks;
                    
                    //enable controls 
                    //ReqTypeHideShowControls($scope.appProperties.CRSRRequests.RequestTypeCode);
                    //enable approval button
                    DisableHideShowControls($scope.appProperties.HRRequests.ApplicationStatus, $scope.appProperties.CurrentUser.Id, $scope.appProperties.HRRequests.Status) //disable controls while load and approval status
                    //format-date
                    $scope.appProperties.HRRequests.ApplicationDate = $filter('date')(response.data.d.results[0].ApplicationDate, 'dd/MM/yyyy') //created 
                    //setting loaded approvers
                    if (response.data.d.results[0].Level1ApprovalStatus != "" && response.data.d.results[0].Level1ApprovalStatus != null && response.data.d.results[0].Level1ApprovalStatus != undefined){
                        $scope.appProperties.HRRequests.Level1Approver = response.data.d.results[0].Level1Approver;
                        $scope.appProperties.HRRequests.Level1ApprovalStatus = response.data.d.results[0].Level1ApprovalStatus;
                        $scope.appProperties.HRRequests.Level1ApprovalDate = response.data.d.results[0].Level1ApprovalDate;
                        $scope.Workflows.push({ Level: "Level1", ApprovingOfficer: $scope.appProperties.HRRequests.Level1Approver.Title, Date: $scope.appProperties.HRRequests.Level1ApprovalDate, Comments: $scope.appProperties.HRRequests.ApprovalRejectionComments, ApplicationStatus: $scope.appProperties.HRRequests.Level1ApprovalStatus});
                    }
                    if (response.data.d.results[0].Level2ApprovalStatus != "" && response.data.d.results[0].Level2ApprovalStatus != null && response.data.d.results[0].Level2ApprovalStatus != undefined) {
                        $scope.appProperties.HRRequests.Level2Approver = response.data.d.results[0].Level2Approver;
                        $scope.appProperties.HRRequests.Level2ApprovalStatus = response.data.d.results[0].Level2ApprovalStatus;
                        $scope.appProperties.HRRequests.Level3ApprovalDate = response.data.d.results[0].Level3ApprovalDate;
                        $scope.Workflows.push({ Level: "Level2", ApprovingOfficer: $scope.appProperties.HRRequests.Level2Approver.Title, Date: $scope.appProperties.HRRequests.Level2ApprovalDate, Comments: $scope.appProperties.HRRequests.ApprovalRejectionComments, ApplicationStatus: $scope.appProperties.HRRequests.Level2ApprovalStatus });
                    }
                    if (response.data.d.results[0].Level3ApprovalStatus != "" || response.data.d.results[0].Level3ApprovalStatus != undefined) {
                        $scope.appProperties.HRRequests.Level3Approver = response.data.d.results[0].Level3Approver;
                        $scope.appProperties.HRRequests.Level3ApprovalStatus = response.data.d.results[0].Level3ApprovalStatus;
                        $scope.appProperties.HRRequests.Level3ApprovalDate = response.data.d.results[0].Level3ApprovalDate;
                    }
                    ////setting buttons for display
                    //if ($scope.appProperties.HRRequests.Author.Id == $scope.appProperties.CurrentUser.Id) {
                    //    DisableHideShowControls("Close", $scope.appProperties.CurrentUser.Id, "0")
                    //} else {
                    //    DisableHideShowControls("Process", $scope.appProperties.CurrentUser.Id, "0")
                    //}
                    
                    if ($scope.appProperties.HRRequests.OnBehalf =="1") {
                        $scope.OnBehalf = true;
                    } else {
                        $scope.OnBehalf = false;
                    }
                    
                    
                    $("#chkboxBehalf").prop('disabled', true);
                    //display staus on ui
                    $scope.appProperties.HRRequests.DisplayStatus = Config.ITWFStatus[$scope.appProperties.HRRequests.WorkFlowStatus]

                 }
            });

            
        };
        //CRSRReqdoc
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
            else { }
        };
        function DisableHideShowControls(status, currentUsrId, ReqTypeCode) {
            //initally disble/show 
            //$scope.IsBranchHeadDisabled = false; // disbale true        
            //$scope.IsFinanceHeadDisabled = false;
            //manager status- button  -disable             
            //$scope.IsBranchHeadStatusDisabled = false;
            //$scope.IsFinanceHeadStatusDisabled = false;
            //while submit and save disable date field and button
            
            $scope.IsSubmitButton = false;;
            $scope.IsSApproveButton = false;
            $scope.IsRejectButton = false;
            $scope.IsSubmitMode = false; // using this while submit button enable control hide   
            $scope.IsCloseButton = false;
            $scope.IsReRouteButton = false
            //$scope.IsReRouteButton = false;
            //$scope.IsUserReRouteButton = false;
            //$scope.IsCommentsSection = true;
            //$scope.IsUserReRouteIMNOButton = false;
            //saved mode
            if ((status == "New" && (currentUsrId == $scope.appProperties.HRRequests.Author.Id)) || $scope.appProperties.HRRequests.ApplicationStatus == "Re-Routed") {
                $scope.IsSubmitButton = true;
                $scope.IsSApproveButton = false;
                $scope.IsRejectButton = false;
                $scope.IsCloseButton = false;
                $scope.IsReRouteButton = false;
               // $scope.IsCommentsSection = false;               
            }
            else if ($scope.appProperties.HRRequests.Level1ApprovalStatus == "Pending" && ($scope.appProperties.CurrentUser.Id == $scope.appProperties.HRRequests.Level1Approver.Id)) {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSubmitButton = false;
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                $scope.IsReRouteButton = true;
                $scope.IsCloseButton = false;
            }
            else if ($scope.appProperties.HRRequests.Level2ApprovalStatus == "Pending") {
                //enable only BranchHead-approve/rejectbutton
              
                getLevelApprover("Level2Approver");
                if ($scope.appProperties.HRRequests.Level2Approver != undefined) {
                    if ($scope.appProperties.HRRequests.Level2Approver.Id) {
                        $scope.IsSubmitButton = false;
                        $scope.IsSApproveButton = true;
                        $scope.IsRejectButton = true;
                        $scope.IsReRouteButton = true;
                        $scope.IsCloseButton = false;
                        document.getElementById("pills-profile-tab-close").innerHTML="Complete"
                    }
                }
            } 
            else if ($scope.appProperties.HRRequests.Level1ApprovalStatus == "Approved" && $scope.appProperties.HRRequests.Level2ApprovalStatus == "Approved") {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSubmitButton = false;
                $scope.IsSApproveButton = false;
                $scope.IsRejectButton = false;
                $scope.IsReRouteButton = false;
                $scope.IsCloseButton = true;
            }
            //Branch head  approval
            else if (status == "Close") {
                //enable only BranchHead-approve/rejectbutton
                $scope.IsSubmitButton = false;
                $scope.IsSApproveButton = false;
                $scope.IsRejectButton = false;
                $scope.IsCloseButton = false;
                $scope.IsReRouteButton = false;
            }

           
        };      
        //loading Intern Candidates
        function getInterns(appID) {
            $scope.loaded = true; //spinner start -service call start
            uService.getInterns(appID).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {                    
                    angular.forEach(response.data.d.results, function (value, key) {
                        var tempintern = {
                            NameOfIntern: value.Title,
                            InternEmail: value.Email,
                            InternContactNo: value.ContactNo,
                            ResumeFile: value.FileName,
                            AttachmentFiles: value.AttachmentFiles
                        }                       
                        $scope.appProperties.HRRequests.InternContactInfos.push(tempintern);
                    });
                }
            });
        };
        //loading Temporary Candidates
        function getTemporaryCandidates(appID) {
            $scope.loaded = true; //spinner start -service call start
            uService.getTemporaryCandidates(appID).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.HRRequests.TemporaryCandidates = response.data.d.results;
                }
            });
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


        $scope.tempfiles = [];
        $scope.removeFile = function (attachedFile, index) {
            var removedItem = $scope.tempfiles.indexOf(attachedFile);
            $scope.tempfiles.splice(removedItem, 1);
        };
        $scope.saveAttachment = function (ListName, ListItemId, tempfiles) {
            if (ListItemId!=-1) {
               // var numberOfFiles = tempfiles.length;
              //  angular.forEach(tempfiles, function (fileValues, key) {
            getFileBuffer(tempfiles)
                        .then(function (bufferVal) {
                            uploadFileSP(bufferVal, tempfiles.name, ListName, ListItemId);
                           // numberOfFiles--;
                           // if (numberOfFiles == 0) {
                                console.log("attachment insert success");
                           // }
                        });
               // });
            }
        };
        function uploadFileSP(bufferVal, fileName, listName, itemID) {
            var urlValue = _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/GetByTitle('" + listName + "')/items(" + itemID + ")/AttachmentFiles/add(FileName='" + fileName + "')";

            $.ajax({
                url: urlValue,
                type: "POST",
                data: bufferVal,
                async: false,
                processData: false,
                headers: {
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "accept": "application/json;odata=verbose",
                    "content-type": "application/json; odata=verbose"
                },
                success: fileSuccess,
                error: fileError
            });
            function fileSuccess(data) {
                console.log('File Added Successfully.');
            }
            function fileError(error) {
                console.log(error.statusText + "\n\n" + error.responseText);
            }
        }
        function getFileBuffer(file) {
            var deferred = $.Deferred();
            var reader = new FileReader();
            reader.onloadend = function (e) {
                deferred.resolve(e.target.result);
            }
            reader.onerror = function (e) {
                deferred.reject(e.target.error);
            }
            reader.readAsArrayBuffer(file);
            return deferred.promise();
        }

        function getBodyMsg(appNumber,staffName) {
            var body = '<html><body><table><tr><td><a href="http://win-qgdhpl3b82d/erequest/Pages/HReMRServiceRequest.aspx?ReqId=' + $scope.appProperties.HRRequests.ID +'">click here</a></td><td>' + staffName+'</td></tr></table></body></html>';
            return body;
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
    if (items.CRSRRequests.ITProjectManager.Id == items.CurrentUser.Id) {
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

   
    $scope.cancelModal = function () {
        $uibModalInstance.dismiss('cancel');
    };

    });

}) ();