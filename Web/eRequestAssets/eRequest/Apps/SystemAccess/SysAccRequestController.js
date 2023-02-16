"use strict";
(function () {
    app.controller("SysAccRequestController", ["$scope", "SysAccService", "CommonService", "Config", "$location", "$window", "$uibModal", "$filter", "$timeout", function ($scope, uService, commonService, Config, $location, $window, $uibModal, $filter, $timeout) {
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
        var toEmails = ""; //using multiples email
        var fromEmail = "";
        var dispName = "";
        var bodyMsg = "";
         //end config
        //init variable
        $scope.isSiteAdmin = false;
        $scope.IsModuleAccess = false;
        $scope.IsLayerAccess = false;
        $scope.IsMaven = false;
        $scope.IsEVEModules = false;
        $scope.IsACEModule = false;
        $scope.IsTypeOfRequest = false;      
        $scope.IsImplementationOfficer = false; //init  
        $scope.name = 'UploadAttachements';
        $scope.domainValue = []; 
        $scope.domainSelected = []; 
        $scope.files = []; //system access listitem-attachements
        //check required validation
        $scope.appProperties = [];
        $scope.appProperties = ({
            //List Properties-exact columnname on SystemAccess(save/edit) and additional properties for ui purpose
            SystemAccess: ({
                ID: "",
                Title: "",
                Author: ({ Id: "", EMail: "", Title: "", LoginName: "" }), //createdBy, Requested by also same
                //people picker              
                RequestedFor: ({ Id: "", EMail: "", Title: "" }),
                BranchHead: ({ Id: "", EMail: "", Title: "" }),
                SystemAccessHead: ({ Id: "", EMail: "", Title: "" }),
                Level1Approver: ({ Id: "", EMail: "", Title: "" }),
                ModuleOwner: ({ Id: "", EMail: "", Title: "" }),
                EVEModuleOwner: ({ Id: "", EMail: "", Title: "" }),
                EVEModuleLeader: ({ Id: "", EMail: "", Title: "" }),
                RequestorLoginID: "", //current user
                RequestedForLoginID: "",
                ImplementationOfficer: ({ Id: "", EMail: "", Title: "" }),
                //date
                SubmittedDate: null,
                RequestedDate: null,
                EVEModuleOwnerDate: null,
                EVEModuleLeaderDate:null,
                ModuleOwnerDate: null,
                SystemAccessHeadDate: null,
                Level1ApproverDate: null,
                ImplementationOfficerDate: null,
                ImplementationOfficerStatus: "",
                EVEModuleLeaderStatus: "",
                EVEModuleOwnerStatus:"",
                ModuleOwnerStatus: "",
                Level1ApproverStatus: "",
                SystemAccessHeadStatus: "",
                ApplicationDate: $filter('date')(new Date(), 'dd/MM/yyyy'), //dd-MM-yyyy hh:mm a   
                ApplicationStatus: "New",
                WorkFlowStatus: "0",
                DisplayStatus: "New", //UI purpose
                RemarksReasons: "",
                OnBehalf: "0",                
                AccessOption: "",
                TransferInAdvance: "0", 
                ApplyforTransferInAdvance: "",             
                ModuleAccess: "",
                AceMoudle: "",
                SystemAccess: "",
                SystemAccessCode: "",
                WorkFlowCode:"0",
                ACEDomain: "",
                //-chekcbox list column                
                TypeofRequest: ({ results: [] }),
                RoleInformation: ({ results: [] }),
                EveModules: ({ results: [] })

            }),
            SystemAccessHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            BranchHeadMaster: ({ Id: "", EMail: "", Title: "" }),
            ImplementationOfficerMaster: ({ Id: "", EMail: "", Title: "" }),
            MavenAccessOption: [],
            AccessOption: {
                EditSelected: [],
                ViewSelected: [],
                ModuleEditSelected: [],
                ModuleViewSelected: []
            },
            //appComments-List Properties
            appComments: ({
                ID: "",
                Title: "",
                UserComments: "",
                SysAccessListItemID: "",
                Author: ({ Id: "", EMail: "", Title: "" }),
            }),
            EveRoleMasterData: [],
            SystemAccessMaster: [],
            MavenGroupMaster: [],
            ACEDomainMasterData: [],
            ACERoleMasterData: [],
            ACERoleFilterData: [],
            ACEModuleMasterdata: [],
            AccessOptionMaster: [],
            MavenGroupFilterData: [],
            PALLICENSERoleMasterData: [],
            TypeOfRequestMaster: [],
            //Current User Properties
            CurrentUser: ({ Id: "", Email: "", Title: "", IsSiteAdmin: "", LoginName: "", DomainName: "" }),
        });
        // -bind checkbox using repeater
        //  MODULE ACCESS IN MAVEN APPLICATION
        $scope.SelectionModuleMavenCheckboxInformation = [];
        $scope.SelectionModuleACECheckboxInformation = [];
        $scope.SelectionCheckboxPALLICENSEmodules = [];
        $scope.SelectionModuleViewCheckboxInformation = [];
        //Type of Request-chkbox      
        $scope.SelectionCheckboxInformation = [];
        //Type of Request-chkbox
        $scope.CheckboxRoleInformation = [];
        $scope.SelectionCheckboxRoleInformation = [];
        $scope.CheckboxEVEmodules = [];
        $scope.CheckboxEVEmoduleMaster = [];
        $scope.CheckboxFilterModules = [];
        $scope.SelectionCheckboxEVEmodules = [];
        //domain array
        $scope.SelectedDoamin = [];
        $scope.onBehalfList = [{ value: false }, { value: true }]
        $scope.DomainName = "";
        //Angular methods ----------------------------------------------------------------------------------start-while using html then create scope variable or method
        //init start
        $scope.init = function (init) {
            Utility.helpers.initializePeoplePicker('peoplePickerRequestedFor');
            //OnValueChangedClientScript- while user assing fire this event
            SPClientPeoplePicker.SPClientPeoplePickerDict.peoplePickerRequestedFor_TopSpan.OnValueChangedClientScript = function (peoplePickerId, selectedUsersInfo) {
                var userData = selectedUsersInfo[0];
                if (userData !== undefined) {
                    $scope.IsRequestedReq = false;
                    $('#userID').val(userData.Key.split('\\')[1]);
                    $scope.appProperties.SystemAccess.RequestedFor.LoginName = userData.Key.split('\\')[1];
                    // Get the first user's ID by using the login name.
                    getUserId(userData.Key).done(function (user) {
                        $scope.appProperties.SystemAccess.RequestedFor.Id = user.d.Id;
                        $scope.appProperties.SystemAccess.RequestedFor.EMail = user.d.Email;
                      // getting login name without domain 
                        if (user.d.LoginName.indexOf("|") !== -1) {
                            getBranchHeadFromStaffDir(user.d.LoginName.split("|")[1].split("\\")[0], user.d.LoginName.split("|")[1].split("\\")[1]);
                        }
                    });
                    $scope.$apply();
                } else {
                    $scope.appProperties.SystemAccess.RequestedFor.Id = "";
                    $scope.appProperties.SystemAccess.RequestedFor.LoginName = "";
                    $scope.IsRequestedReq = true;
                    getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                    $scope.$apply();
                }
            };
            //load function
            $scope.loaded = true; //spinner start -service call start
            commonService.getCurrentUser().then(function (result) {
                $scope.loaded = false; //spinner stop - service call end 
                $scope.appProperties.CurrentUser = result.data.d;
                $scope.DomainName = result.data.d.LoginName.split('\\')[0];
                $scope.appProperties.CurrentUser.LoginName = result.data.d.LoginName.split('\\')[1];
                $scope.appProperties.SystemAccess.Author.Id = result.data.d.Id; //while init 
                $scope.appProperties.SystemAccess.Author.LoginName = result.data.d.LoginName; //while init  
                $scope.appProperties.SystemAccess.Author.Title = result.data.d.Title; //while init using in UI  
               
                       

                LoadAllMasterListData();
                //get query string value
                $scope.appProperties.SystemAccess.ID = Utility.helpers.getUrlParameter('ReqId');
                if ($scope.appProperties.SystemAccess.ID != "" && $scope.appProperties.SystemAccess.ID != undefined) {
                    loadListItemData($scope.appProperties.SystemAccess.ID);//Load data     
                    loadSystemAccessAceDomainListData(Config.SystemAccessAceDomain,"SysAccessListItemID", $scope.appProperties.SystemAccess.ID);
                    //load comments
                    loadCommentsListData($scope.appProperties.SystemAccess.ID); //PriceComparison  
                }
                else {
                    //new req
                    if (result.data.d.LoginName.indexOf("|") !== -1) {
                        $scope.appProperties.CurrentUser.ADID = result.data.d.LoginName.split("|")[1].split("\\")[1]; // getting login name without domain   
                        $scope.appProperties.CurrentUser.DomainName = result.data.d.LoginName.split("|")[1].split("\\")[0];
                        getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, $scope.appProperties.CurrentUser.ADID);
                    }
                    else {
                        $scope.appProperties.CurrentUser.ADID = result.data.d.Title.split("\\")[1]; // getting login name without domain   
                        $scope.appProperties.CurrentUser.DomainName = result.data.d.Title.split("\\")[0];
                        getBranchHeadFromStaffDir($scope.appProperties.CurrentUser.DomainName, result.data.d.LoginName); //chk only
                    }
                    getUniqueNumber(); //generate unique number-only for new Req   
                    DisableHideShowControls("New", $scope.appProperties.CurrentUser.Id) //init stage
                }
                // chk is SiteAdmin  
                if (!result.data.d.IsSiteAdmin) {
                    $scope.isSiteAdmin = false;
                }
                else {
                    $scope.isSiteAdmin = true;
                }
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
        //get BranchHeadFromStaffDir from csutom list-Root site
        $scope.department = "";
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
                        $scope.department = branch;
                        commonService.getStaffBranchHead(Config.BranchHeads, "Branch", branch).then(function (response) {
                            $scope.loaded = false; //spinner stop  
                            if (response.data.d.results.length > 0) {
                                if (response.data.d.results[0].Branch != null && response.data.d.results[0].Branch != "" && response.data.d.results[0].Branch != undefined) {
                                    $scope.appProperties.BranchHeadMaster = response.data.d.results[0].BranchHead;
                                    $scope.appProperties.SystemAccess.BranchHead = response.data.d.results[0].BranchHead;
                                    commonService.getUserbyId(response.data.d.results[0].BranchHeadId).then(function (response) {
                                        //check out of office
                                        commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                            if (response.data.d.results.length > 0) {
                                                getUserId($scope.DomainName +"\\"+ response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                    $scope.appProperties.SystemAccess.BranchHead.Id = user.d.Id;
                                                    $scope.appProperties.SystemAccess.BranchHead.EMail = user.d.Email;
                                                    $scope.appProperties.SystemAccess.BranchHead.Title = user.d.Title;
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
                                            $scope.appProperties.SystemAccess.BranchHead = response.data.d.results[0].DivisionHead;
                                            commonService.getUserbyId(response.data.d.results[0].DivisionHeadId).then(function (response) {
                                                //check out of office
                                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                                    if (response.data.d.results.length > 0) {
                                                        getUserId($scope.DomainName + "\\"+ response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                            $scope.appProperties.SystemAccess.BranchHead.Id = user.d.Id;
                                                            $scope.appProperties.SystemAccess.BranchHead.EMail = user.d.Email;
                                                            $scope.appProperties.SystemAccess.BranchHead.Title = user.d.Title;
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
        //submit request
        $scope.IsTypeOfRequestValid = false;
        $scope.IsModulesValid = false; 
        $scope.IsTypeOfRolesValid = false;
        $scope.submitListItem = function (type, $event, Form) {
            $scope.loaded = true; //automate false while redirect page
            var appStatus = "1"; //maintain applciation field
            var wfStatus = "1"; //maintain applciation field
            var cmtStatus = "1"; //maintain coments field
            $event.preventDefault();
            var invalid = false;      
           
            $scope.IsTypeOfRequestValid = false;
            $scope.IsModulesValid = false;
            $scope.IsTypeOfRolesValid = false;
            //init validation false 
            switch (type) {
                case 1:
                    if (Form.$invalid && type != 0) {
                        $scope.loaded = false;
                        if (Form.ddlSystemAccess.$invalid) Form.ddlSystemAccess.$touched = true;
                        if (Form.ddlAccessOption != undefined) {
                            if (Form.ddlAccessOption.$invalid) Form.ddlAccessOption.$touched = true;
                        }
                        if (Form.exampleFormControlTextarea1.$invalid) Form.exampleFormControlTextarea1.$touched = true;
                        if (Form.DateRequiredBy.$invalid) Form.DateRequiredBy.$touched = true;
                        invalid = true;
                    }       
                    if ($scope.IsTypeOfRequest == true) {
                        if ($scope.SelectionCheckboxInformation.length == 0) {
                            invalid = true;
                            if ($scope.IsFileShare == true) {
                                $scope.IsFileShareValid = true;
                            } else { $scope.IsFileShareValid = false; }

                            if ($scope.IsTypeOfRequest == true) {
                                $scope.IsTypeOfRequestValid = true;
                            } else { $scope.IsTypeOfRequestValid = false; }
                        }
                        else {
                            if ($scope.IsFileShare == true) {
                                $scope.IsFileShareValid = false;
                            }

                            if ($scope.IsTypeOfRequest == true) {
                                $scope.IsTypeOfRequestValid = false;
                            }
                        }
                    }
                    //module valid
                    if ($scope.IsEVEModules) {
                        if ($scope.SelectionCheckboxEVEmodules.length > 1) {
                            $scope.IsModulesValid = true;
                            invalid = true;
                        }
                      else if ($scope.SelectionCheckboxEVEmodules.length == 0) {
                            $scope.IsModulesValid = true;
                            invalid = true;
                        }
                        else {
                            $scope.IsModulesValid = false;
                        }
                    }
                    //roles valid
                    if ($scope.IsTypeOfRoles) {
                        if ($scope.CheckboxRoleInformation.length == 0) {
                            $scope.IsTypeOfRolesValid = true;
                            invalid = true;
                        }
                        else {
                            $scope.IsTypeOfRolesValid = false;
                        }
                    }
                    if ($scope.OnBehalf) {

                        if ($scope.appProperties.SystemAccess.RequestedFor.Id == "") {
                            $scope.appProperties.SystemAccess.RequestedFor.LoginName = "";
                            $scope.IsRequestedReq = true; 
                            invalid = true;
                        }
                    }
                    //final valid
                    if (invalid) {
                        $scope.loaded = false;
                        return;
                    }
                    cmtStatus = 1;
                   
                    $scope.appProperties.SystemAccess.BranchHeadStatus = "Pending" //initially 
                    insertUpdateListItem(wfStatus, appStatus, cmtStatus, "Submitted Successfully");     //workflowstaus,appstatus ,cmtStatus        
                    // code block
                    break;
                //approve mode
                case 2:
                    if (Form.$invalid && type != 0) {
                    }
                    if ($scope.appProperties.SystemAccess.SystemAccess == "EVE" || $scope.appProperties.SystemAccess.SystemAccess == "ACE"  ) {
                        if ($scope.appProperties.SystemAccess.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.BranchHead.Id) {
                            $scope.appProperties.SystemAccess.BranchHeadStatus = "Approved";
                            //$scope.appProperties.SystemAccess.SystemAccessHeadStatus = "Pending";
                            //if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id == undefined) {
                            //    $scope.appProperties.SystemAccess.EVEModuleLeader = response.data.d.results[0].ACEModuleLeader;
                            if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id == undefined && $scope.appProperties.SystemAccess.SystemAccess == "ACE") {
                                    $scope.appProperties.SystemAccess.EVEModuleLeaderStatus = "Pending";
                                }
                                else {
                                    //$scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending"; //if modlue leader empty assign next level(module owner) status pending
                                }
                            $scope.appProperties.SystemAccess.BranchHeadDate == new Date();
                            appStatus = "2" //inprgress
                            wfStatus = "6"; //BranchHeadStatus approved
                            cmtStatus = "2";
                        }
                        else if ($scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Pending" && $scope.appProperties.SystemAccess.SystemAccess == "EVE") {
                            $scope.appProperties.SystemAccess.EVEModuleLeaderStatus = "Approved";
                            $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending";
                            $scope.appProperties.SystemAccess.EVEModuleLeaderDate = new Date();
                            $scope.appProperties.SystemAccess.EVEModuleLeader.Id = $scope.appProperties.CurrentUser.Id;
                            appStatus = "2" //inprgress
                            wfStatus = "6";
                            cmtStatus = "6";
                        }
                        else if (($scope.appProperties.SystemAccess.SystemAccess == "EVE" && $scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Approved") && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                            $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Approved";
                            $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Pending";
                            $scope.appProperties.SystemAccess.EVEModuleOwnerDate = new Date();
                            appStatus = "3" //approveal complete
                            wfStatus = "8";
                            cmtStatus = "8";
                        }
                        else if ($scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Pending" && $scope.IsACEModuleLeader == true) {
                            $scope.appProperties.SystemAccess.EVEModuleLeaderStatus = "Approved";
                            $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending";
                            $scope.appProperties.SystemAccess.EVEModuleLeaderDate = new Date();
                            $scope.appProperties.SystemAccess.EVEModuleLeader.Id = $scope.appProperties.CurrentUser.Id;
                            appStatus = "2" //inprgress
                            wfStatus = "6";
                            cmtStatus = "6";
                        } else if (($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" && $scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Approved") && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                            $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Approved";
                            //$scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Pending";
                            $scope.appProperties.SystemAccess.EVEModuleOwnerDate = new Date();
                            appStatus = "3" //approveal complete
                            wfStatus = "10";
                            cmtStatus = "8";
                        }
                        else if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                            $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Approved";
                            $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Pending";
                            $scope.appProperties.SystemAccess.EVEModuleOwnerDate = new Date();
                            appStatus = "2" //inprgress
                            wfStatus = "8";
                            cmtStatus = "8";
                        }
                        else if ($scope.appProperties.SystemAccess.ImplementationOfficerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.ImplementationOfficer.Id) {
                            $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Approved";
                            $scope.appProperties.SystemAccess.ImplementationOfficerDate = new Date();
                            appStatus = "3" //approveal complete
                            wfStatus = "10";
                            cmtStatus = "10";
                        } else {
                            //
                        }
                    }
                    else {
                        if ($scope.appProperties.SystemAccess.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.BranchHead.Id) {
                            $scope.appProperties.SystemAccess.BranchHeadStatus = "Approved";
                            $scope.appProperties.SystemAccess.SystemAccessHeadStatus = "Pending";
                            $scope.appProperties.SystemAccess.BranchHeadDate == new Date();
                            appStatus = "2" //inprgress
                            wfStatus = "2";
                            cmtStatus = "2";
                        }
                        else if ($scope.appProperties.SystemAccess.SystemAccessHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.SystemAccessHead.Id) {
                            $scope.appProperties.SystemAccess.SystemAccessHeadStatus = "Approved";
                            $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Pending";
                            $scope.appProperties.SystemAccess.SystemAccessHeadDate = new Date();
                            appStatus = "2" //inprgress
                            wfStatus = "4";
                            cmtStatus = "4";
                        }
                       
                        else if ($scope.IsImplementationOfficer) {
                            $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Approved";
                            $scope.appProperties.SystemAccess.ImplementationOfficerDate = new Date();
                            appStatus = "3" //approveal complete
                            wfStatus = "10";
                            cmtStatus = "10";
                        } 
                        else {
                            //
                        }
                    }                   
                    insertUpdateListItem(wfStatus, appStatus, cmtStatus, "Approved Successfully");     //workflowstaus,appstatus ,cmtStatus  
                          
                    // code block
                    break;
                //Rejecte mode
                case 3:
                    if ($scope.appProperties.appComments.Comments != "" && $scope.appProperties.appComments.Comments != undefined) {

                        cmtStatus = 1;
                        //$scope.appProperties.SystemAccess.EVEModuleLeaderStatus = ""; 
                        //$scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "";
                        //$scope.appProperties.SystemAccess.ImplementationOfficerStatus = "";
                        if ($scope.appProperties.SystemAccess.SystemAccess == "EVE" || $scope.appProperties.SystemAccess.SystemAccess == "ACE") {
                            if ($scope.appProperties.SystemAccess.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.BranchHead.Id) {
                                $scope.appProperties.SystemAccess.BranchHeadStatus = "Rejected";
                                $scope.appProperties.SystemAccess.BranchHeadDate == new Date();
                                appStatus = "4" //rjected
                                wfStatus = "3";
                                cmtStatus = "3";
                            }
                            else if ($scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Pending" && $scope.IsACEModuleLeader == true) {
                                $scope.appProperties.SystemAccess.EVEModuleLeaderStatus = "Rejected";
                                $scope.appProperties.SystemAccess.EVEModuleLeaderDate = new Date();
                                appStatus = "4" //rjected
                                wfStatus = "7";
                                cmtStatus = "7";
                            }
                            else if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                                $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Rejected";
                                $scope.appProperties.SystemAccess.EVEModuleOwnerDate = new Date();
                                appStatus = "4" //rjected
                                wfStatus = "9";
                                cmtStatus = "9";
                            }
                            else if ($scope.appProperties.SystemAccess.ImplementationOfficerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.ImplementationOfficer.Id) {

                                $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Rejected";
                                $scope.appProperties.SystemAccess.SystemAccessHeadDate = new Date();
                                appStatus = "4" //rjeected
                                wfStatus = "11";
                                cmtStatus = "11";
                            }
                            else if ($scope.IsImplementationOfficer) {
                                $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Rejected";
                                $scope.appProperties.SystemAccess.ImplementationOfficerDate = new Date();
                                appStatus = "4" //rjected
                                wfStatus = "11";
                                cmtStatus = "11";
                            } else {
                                //
                            }
                        }
                        else {
                            if ($scope.appProperties.SystemAccess.BranchHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.BranchHead.Id) {
                                $scope.appProperties.SystemAccess.BranchHeadStatus = "Rejected";
                                $scope.appProperties.SystemAccess.BranchHeadDate == new Date();
                                appStatus = "4" //rjeected compelte
                                wfStatus = "3";
                                cmtStatus = "3";
                            }
                            else if ($scope.appProperties.SystemAccess.SystemAccessHeadStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.SystemAccessHead.Id) {
                                $scope.appProperties.SystemAccess.SystemAccessHeadStatus = "Rejected";
                                $scope.appProperties.SystemAccess.SystemAccessHeadDate = new Date();
                                appStatus = "4" //rjeected
                                wfStatus = "5";
                                cmtStatus = "5";
                            }
                           
                            else if ($scope.IsImplementationOfficer) {
                                $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Rejected";
                                $scope.appProperties.SystemAccess.ImplementationOfficerDate = new Date();
                                appStatus = "4" //rjeected
                                wfStatus = "11";
                                cmtStatus = "11";
                            }
                            else {

                                //
                            }
                        }
                        insertUpdateListItem(wfStatus, appStatus, cmtStatus, "Rejected Successfully");     //workflowstaus,appstatus ,cmtStatus        
                        // code block
                    }
                    else {
                        alert('Please enter rejection comments.');
                        $scope.loaded = false; //spinner stop - service call end
                        return;
                    }
                    break;
                case 0:
                    //Close
                    window.location.href = siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx";
                    // code block                  
                    break;
                default:
                    // code block
                    window.location.href = siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx";
            }
        };
        // Check checkbox checked or not
        $scope.checkVal = function () {
            if ($scope.OnBehalf) {
                $scope.appProperties.SystemAccess.OnBehalf = "1";
                $scope.OnBehalf = true;
                if ($scope.appProperties.SystemAccess.RequestedFor.Id == "") {
                    $scope.appProperties.SystemAccess.RequestedFor.LoginName = "";
                    $scope.IsRequestedReq = true;
                } else { $scope.IsRequestedReq = false; }
            } else {
                $scope.IsRequestedReq = false;
                $scope.OnBehalf = false;
                $scope.appProperties.SystemAccess.OnBehalf = "0";
            }
            $scope.$apply();
        }
        // Check checkbox checked or not
        $scope.checktransferVal = function () {
            if ($scope.Ontransfer) {
                $scope.appProperties.SystemAccess.TransferInAdvance = "1";
                $scope.Ontransfer = true;
            } else {
                $scope.Ontransfer = false;
                $scope.appProperties.SystemAccess.TransferInAdvance = "0";
            }
        }

        //checking access information
        $scope.toggleModuleViewCheckboxInfoSelection = function toggleModuleViewCheckboxInfoSelection(information) {
            var idx = $scope.SelectionModuleViewCheckboxInformation.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                if (information.EditAccess == "true" && information.ViewAccess == "true") {

                }
                else if (information.ViewAccess == "true") {

                }
                else if (information.EditAccess == "true") {

                }
                else {
                    $scope.SelectionModuleViewCheckboxInformation.splice(idx, 1);
                }
            }
            // Is newly selected
            else {
                $scope.SelectionModuleViewCheckboxInformation.push(information);

            }
        };
        $scope.toggleModuleMavenCheckboxInfoSelection = function toggleModuleMavenCheckboxInfoSelection(information) {
            var idx = $scope.SelectionModuleMavenCheckboxInformation.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                if (information.EditAccess == "true" && information.ViewAccess == "true") {

                }
                else if (information.ViewAccess == "true") {

                }
                else if (information.EditAccess == "true") {

                }
                else {
                    $scope.SelectionModuleMavenCheckboxInformation.splice(idx, 1);
                }
            }
            // Is newly selected
            else {
                $scope.SelectionModuleMavenCheckboxInformation.push(information);

            }
        };


        $scope.toggleModuleACECheckboxInfoSelection = function SelectionModuleACECheckboxInformation(information, index) {
            var idx = $scope.SelectionModuleACECheckboxInformation.indexOf(information);

            // Is currently selected
            if (idx > -1) {
                $('#ddlAcedomain' + index).prop('disabled', true);
                if (information.EditAccess == "true" && information.ViewAccess == "true") {

                }
                else if (information.ViewAccess == "true") {

                }
                else if (information.EditAccess == "true") {

                }
                else {
                    $scope.SelectionModuleACECheckboxInformation.splice(idx, 1);
                }
            }
            // Is newly selected
            else {
                $scope.SelectionModuleACECheckboxInformation.push(information);
                $('#ddlAcedomain' + index).prop('disabled', false);
            }
        };
        // Toggle selection for a given chkbox by name - Type of Request
        $scope.toggleCheckboxInfoSelection = function toggleCheckboxInfoSelection(information) {
            $scope.SelectionCheckboxInformation.length = 0;
            var idx = $scope.SelectionCheckboxInformation.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionCheckboxInformation.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionCheckboxInformation.push(information);
            }

            if ($scope.SelectionCheckboxInformation.length == 0) {
                if ($scope.IsFileShare == true) {
                    $scope.IsFileShareValid = true;
                } else { $scope.IsFileShareValid = false; }

                if ($scope.IsTypeOfRequest == true) {
                    $scope.IsTypeOfRequestValid = true;
                } else { $scope.IsTypeOfRequestValid = false; }
            }
            else {
                if ($scope.IsFileShare == true) {
                    $scope.IsFileShareValid = false;
                }

                if ($scope.IsTypeOfRequest == true) {
                    $scope.IsTypeOfRequestValid = false;
                } 
            }
        };
        //EVERole- Add/Remove
        $scope.toggleCheckboxEVEmodulesSelection = function toggleCheckboxEVEmodulesSelection(information) {
            $scope.SelectionCheckboxEVEmodules.length = 0;
            var idx = $scope.SelectionCheckboxEVEmodules.indexOf(information);
            $scope.CheckboxRoleInformation.length = 0;
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionCheckboxEVEmodules.splice(idx, 1);
                //get roles
                if ($scope.appProperties.EveRoleMasterData.length > 0) {
                    angular.forEach($scope.SelectionCheckboxEVEmodules, function (value, key) {
                        var filterEVERole = $filter('filter')($scope.appProperties.EveRoleMasterData, { EVEModule: value }, true);
                        angular.forEach(filterEVERole, function (value, key) {
                            $scope.CheckboxRoleInformation.push({ ID: key, Title: value.Title });
                        });
                    });
                }
            }
            // Is newly selected
            else {
                $scope.SelectionCheckboxEVEmodules.push(information);
                //get roles
                getEVERoles();
            }
            if ($scope.SelectionCheckboxEVEmodules.length > 1) {
                $scope.IsModulesValid = true;
            }
            else if ($scope.SelectionCheckboxEVEmodules.length == 0) {
                $scope.IsModulesValid = true;
            }
            else {
                $scope.IsModulesValid = false;
            }
          //  $scope.$apply();
        }

        //Role- Add/Remove
        $scope.toggleCheckboxRoleInfoSelection = function toggleCheckboxRoleInfoSelection(information) {
            //$scope.SelectionCheckboxRoleInformation.length = 0;
            //var idx = $scope.SelectionCheckboxInformation.indexOf(information);
            var idx = $scope.SelectionCheckboxRoleInformation.indexOf(information);
            // Is currently selected
            if (idx > -1) {
                $scope.SelectionCheckboxRoleInformation.splice(idx, 1);
            }
            // Is newly selected
            else {
                $scope.SelectionCheckboxRoleInformation.push(information);
            }
            //chk valid
            if ($scope.SelectionCheckboxRoleInformation.length == 0) {
                $scope.IsTypeOfRolesValid = true;
            }
            else {
                $scope.IsTypeOfRolesValid = false;
            }
        }
        //othres
        function getOthereRoles(accessOption) {
            $scope.CheckboxRoleInformation.length = 0; //initially clear
            if (accessOption != "") {
                //get roles
                if ($scope.appProperties.PALLICENSERoleMasterData.length > 0) {
                    var filterRole = $filter('filter')($scope.appProperties.PALLICENSERoleMasterData, { SystemName: accessOption }, true);
                    angular.forEach(filterRole, function (value, key) {
                        $scope.CheckboxRoleInformation.push({
                            ID: key, Title: value.Title, SystemName: value.SystemName.Title
                        });
                    });

                    
                }
            };
        }
        //ace filete
        $scope.getFilterACEData = function () {
            $scope.appProperties.ACERoleFilterData.length = 0; //initially clear
            if ($scope.appProperties.SystemAccess.AceMoudle != "") {
                if ($scope.appProperties.ACERoleMasterData.length > 0) {
                    //get roles
                    if ($scope.appProperties.ACERoleMasterData.length > 0) {
                        var filterACERole = $filter('filter')($scope.appProperties.ACERoleMasterData, { ACEModule: $scope.appProperties.SystemAccess.AceMoudle }, true);
                        angular.forEach(filterACERole, function (value, key) {
                            $scope.appProperties.ACERoleFilterData.push({
                                ID: key, NewID: "New", Title: value.Title, ACERoleDescription: value.ACERoleDescription, Domain: "", DomainIndex: "", ViewAccess: "0", EditAccess: "0" //0-false, 1-true
                            });
                        });
                    }
                }
            };
        }
        //dropdown-change
        $scope.getFilterMavanData = function () {

            $scope.appProperties.MavenGroupFilterData.length = 0; //initially clear
            if ($scope.appProperties.SystemAccess.GroupName != "") {
                if ($scope.appProperties.MavenGroupMaster.length > 0) {
                    var filterARy = $filter('filter')($scope.appProperties.MavenGroupMaster, { Title: $scope.appProperties.SystemAccess.GroupName }, true);
                    angular.forEach(filterARy, function (value, key) {
                        $scope.appProperties.MavenGroupFilterData.push({
                            ID: key, AccessOptionId: "", MavenGroupId: value.ID, Title: value.Title, Layer: value.Layer, Feature: value.Feature, DataSet: value.Title, ViewAccess: "0", EditAccess: "0" //0-false, 1-true
                        });
                    });
                }
            };

        }
        //dropdown
        $scope.IsShowFeatureClass = true;
        $scope.getAccessRequestType = function () {
            if ($scope.appProperties.SystemAccess.AccessOption != "--Select--") {
                if ($scope.appProperties.SystemAccess.AccessOption != null && $scope.appProperties.SystemAccess.AccessOption != "" && $scope.appProperties.SystemAccess.AccessOption != undefined) {

                    if ($scope.appProperties.SystemAccess.AccessOption == "Layer Access in Maven Application") {
                        $scope.IsShowFeatureClass = false;
                    }
                    else {
                        $scope.IsShowFeatureClass = true; //featue colum on table
                    }
                    //init and clear data while selection
                    $scope.appProperties.MavenGroupFilterData.length = 0; //initially clear                      
                    $scope.SelectionModuleViewCheckboxInformation.length = 0;
                    $scope.SelectionModuleACECheckboxInformation.length = 0;
                    $scope.SelectionModuleACECheckboxInformation.length = 0;
                    var arryList = $filter('filter')($scope.appProperties.AccessOptionMaster, { Title: $scope.appProperties.SystemAccess.AccessOption }, true);
                    if (arryList.length > 0) {
                        if (arryList[0].Title == "Module Access in Maven Application") {
                            $scope.IsModuleAccess = true;
                            $scope.IsLayerAccess = false;
                            //sublist
                            if ($scope.appProperties.SystemAccess.ID != "" && $scope.appProperties.SystemAccess.ID != undefined) {
                                uService.getSubListByParentId(Config.MavenAccessOption, $scope.appProperties.SystemAccess.ID).then(function (response) {
                                    $scope.loaded = false; //spinner stop - service call end 
                                    if (response.data.d.results.length > 0) {

                                        angular.forEach(response.data.d.results, function (value, key) {
                                            if ($scope.appProperties.SystemAccess.AccessOption == "Module Access in Maven Application") {
                                                if (value.EditAccess == "1") {
                                                    $scope.SelectionModuleEditCheckboxInformation.push({
                                                        ID: key, AccessOptionId: value.ID, MavenGroupId: value.MavenGroupId, Title: value.Title, Layer: value.Layer, Feature: value.GroupName, DataSet: value.DataSet, ViewAccess: "0", EditAccess: "1" //0-false, 1-true
                                                    });
                                                    $scope.appProperties.AccessOption.ModuleEditSelected[value.Id];
                                                }
                                                if (value.ViewAccess == "1") {
                                                    $scope.SelectionModuleViewCheckboxInformation.push({
                                                        ID: key, AccessOptionId: value.ID, MavenGroupId: value.MavenGroupId, Title: value.Title, Layer: value.Layer, Feature: value.GroupName, DataSet: value.DataSet, ViewAccess: "1", EditAccess: "0" //0-false, 1-true
                                                    });
                                                    $scope.appProperties.AccessOption.ModuleViewSelected[value.Id];
                                                }
                                            }
                                            else {
                                                if (value.EditAccess == "1") {
                                                    $scope.SelectionEditCheckboxInformation.push({
                                                        ID: value.Id, Title: value.Title, Layer: value.Layer, Feature: value.GroupName, DataSet: value.DataSet, ViewAccess: "0", EditAccess: "1" //0-false, 1-true
                                                    });
                                                    $scope.appProperties.AccessOption.ModuleViewSelected[value.Id];
                                                }
                                                if (value.ViewAccess == "1") {
                                                    $scope.SelectionViewCheckboxInformation.push({
                                                        ID: value.Id, Title: value.Title, Layer: value.Layer, Feature: value.GroupName, DataSet: value.DataSet, ViewAccess: "1", EditAccess: "0" //0-false, 1-true
                                                    });
                                                    $scope.appProperties.AccessOption.ModuleViewSelected[value.Id];
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                        }
                        else {
                            $scope.IsModuleAccess = false;
                            $scope.IsLayerAccess = true;
                        }
                    }
                }
            };
        }
        $scope.TypeOfRequesFilterFileShare = [];
        $scope.getSystemRequestType = function (item) {
            if (item != undefined) {
                $scope.IsFileShare = false;
                $scope.IsTypeOfRoles = false;   
                $scope.CheckboxRoleInformation.length = 0; //initially clear
                $scope.CheckboxFilterModules.length = 0;
                if (item.SystemAccessCode != null && item.SystemAccessCode != "" && item.SystemAccessCode != undefined) {
                    if (item.SystemAccessCode == "1") {
                        $scope.appProperties.SystemAccess.WorkFlowCode = item.WorkFlowCode;
                        $scope.IsMaven = true;
                        $scope.IsTypeOfRequest = false;
                        $scope.IsEVEModules = false;
                        $scope.IsACEModule = false;
                        $scope.appProperties.SystemAccess.AccessOption = "";
                        $scope.appProperties.MavenGroupFilterData.length = 0; //clear                       
                    }
                    else if (item.SystemAccessCode == "2") {
                        $scope.appProperties.SystemAccess.WorkFlowCode = item.WorkFlowCode;
                        $scope.IsACEModule = true;
                        $scope.IsMaven = false;
                        $scope.IsEVEModules = false;
                        $scope.IsTypeOfRequest = true;
                        $scope.IsModuleAccess = false;
                        $scope.IsLayerAccess = false;
                    }
                    else if (item.SystemAccessCode == "3") {
                        $scope.appProperties.SystemAccess.WorkFlowCode = item.WorkFlowCode;
                        $scope.IsEVEModules = true;
                        $scope.IsMaven = false;
                        $scope.IsACEModule = false;
                        $scope.IsTypeOfRequest = true;
                        $scope.IsTypeOfRoles = true;   
                        $scope.IsModuleAccess = false;
                        $scope.IsLayerAccess = false;
                        $scope.CheckboxFilterModules.length = 0;
                        var filterModule = $filter('filter')($scope.CheckboxEVEmoduleMaster, { System: item.Title },true);
                            angular.forEach(filterModule, function (value, key) {
                                $scope.CheckboxFilterModules.push({ ID: key, Title: value.Title });
                            });                          
                        
                    }
                    else {
                        $scope.TypeOfRequesFilterFileShare.length = 0;
                       
                        $scope.IsMaven = false;
                        $scope.IsTypeOfRequest = false;
                        $scope.IsEVEModules = false;
                        $scope.IsACEModule = false;
                        $scope.IsModuleAccess = false;
                        $scope.IsLayerAccess = false;
                        getOthereRoles(item.Title); //otheres
                        $scope.appProperties.SystemAccess.WorkFlowCode = item.WorkFlowCode;
                        if (item.Title == "FileShare") {  
                            $scope.IsFileShare = true;                               
                            $scope.IsTypeOfRoles = true;   
                            angular.forEach($scope.appProperties.TypeOfRequestMaster, function (value, key) {
                                if (value.Title == "Add Role" || value.Title == "Remove Role" )
                                $scope.TypeOfRequesFilterFileShare.push(value);
                            });                          
                        }
                        if ((item.Title).toLowerCase() == "kris" || (item.Title).toLowerCase() == "connect" || (item.Title).toLowerCase() == "pals") {
                            $scope.IsTypeOfRequest = true;
                            $scope.IsTypeOfRoles =  true;
                        }
                    }
                }
            }
            else {
                $scope.IsMaven = false;
                $scope.IsTypeOfRequest = false;
                $scope.IsEVEModules = false;
                $scope.IsACEModule = false;
                $scope.IsModuleAccess = false;
                $scope.IsLayerAccess = false;
                $scope.IsTypeOfRoles = false;
            }
          
        }
        //attacment remove
        $scope.removeFile = function (attachedFile, index) {
            var removedItem = $scope.files.indexOf(attachedFile);
            $scope.files.splice(removedItem, 1);
        };
        //save attacment
        $scope.saveAttachment = function (ListName, ListItemId) {

            var numberOfFiles = $scope.files.length;
            angular.forEach($scope.files, function (fileValues, key) {
                getFileBuffer(fileValues._file)
                    .then(function (bufferVal) {
                        uploadFileSP(bufferVal, fileValues._file.name, ListName, ListItemId);
                        numberOfFiles--;
                        if (numberOfFiles == 0) {
                            console.log("attachment insert success");
                        }
                    });
            });

        }
        $scope.options = [];
        $('#doaminPop').on('show.bs.modal', function (e) {       
            var listItemId = $(e.relatedTarget).data('id');
            $scope.indexId = $(e.relatedTarget).data('id'); //assing index i nscope then use submit button             
            $(e.currentTarget).find('input[name="ListItemId"]').val(listItemId);
            angular.forEach($scope.options, function (value1, key1) {
                if (value1.checked=="true") {
                    value1.checked = "0";  //chekc box clear
                }
            });
            angular.forEach($scope.appProperties.ACEDomainMasterData, function (value1, key1) {
                if (value1.checked) {
                    value1.checked = false; //setfalse
                }
            });
            $scope.$apply();
        });

        //assing parent value to child        
        $scope.addDomainRequest = function (type, $event, Form) {       

            //clear exissting
            angular.forEach($scope.appProperties.ACERoleFilterData, function (value, key) {
                if (key == $scope.indexId) { //while popu click get index
                    angular.forEach($scope.appProperties.ACEDomainMasterData, function (value1, key1) {                       
                            value.Domain = "";
                    });
                }
            });
            //newly selected
            angular.forEach($scope.appProperties.ACERoleFilterData, function (value, key) {
                if (key == $scope.indexId) { //while popu click get index
                    angular.forEach($scope.appProperties.ACEDomainMasterData, function (value1, key1) {
                        if (value1.checked) {
                            value.Domain += " ," + value1.Title;
                        }                      
                    });
                }
            });
            
          
            $('.modal-backdrop').remove();
            $('#doaminPop').modal('hide');
        };
   
       
        $scope.toggleDomainSelection = function (index) {
            $scope.appProperties.ACEDomainMasterData[index].checked = !$scope.appProperties.ACEDomainMasterData[index].checked;   
         
        };
        //Angular methods --------------------------------------------------------------------------------------end

        //javascript methods ----------------------------------------------------------------------------------start
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
        //insert/update SystemAccess 
        function insertUpdateListItem(Worfkflowstatus, ApplicationStatus, commentStatus, message) {
            //Save/update  -get list                 
            var list = web.get_lists().getByTitle("" + Config.SystemAccess + "");
            var listItem = "";
            if ($scope.appProperties.SystemAccess.ID == "" || $scope.appProperties.SystemAccess.ID == undefined) {
                //insert data to sharepoint
                var listCreationInformation = new SP.ListItemCreationInformation();
                listItem = list.addItem(listCreationInformation);
                listItem.set_item("RequestorLoginID", $scope.appProperties.CurrentUser.LoginName);
            } else {

                listItem = list.getItemById($scope.appProperties.SystemAccess.ID);
            }
            //assign properties while insert/update
            listItem.set_item("Title", $scope.appProperties.SystemAccess.ApplicationID);
            listItem.set_item("ApplicationID", $scope.appProperties.SystemAccess.ApplicationID);
            listItem.set_item("WorkFlowCode", $scope.appProperties.SystemAccess.WorkFlowCode);
            listItem.set_item("OnBehalf", $scope.appProperties.SystemAccess.OnBehalf);
            listItem.set_item("TransferInAdvance", $scope.appProperties.SystemAccess.TransferInAdvance); //chekcbox   
            listItem.set_item("ApplyforTransferInAdvance", $scope.appProperties.SystemAccess.ApplyforTransferInAdvance);  //- insert  ddl  
            if ($scope.appProperties.SystemAccess.SystemAccess.Title != "" && $scope.appProperties.SystemAccess.SystemAccess.Title != null && $scope.appProperties.SystemAccess.SystemAccess.Title != undefined) {
                listItem.set_item("SystemAccess", $scope.appProperties.SystemAccess.SystemAccess.Title);  //-dropdwpn  
                listItem.set_item("SystemAccessCode", $scope.appProperties.SystemAccess.SystemAccess.SystemAccessCode);  //-dropdwpn  
            }
            else {
                listItem.set_item("SystemAccess", $scope.appProperties.SystemAccess.SystemAccess);
                listItem.set_item("SystemAccessCode", $scope.appProperties.SystemAccess.SystemAccessCode);
            }
          
            listItem.set_item("RemarksReasons", $scope.appProperties.SystemAccess.RemarksReasons);  //-dropdwpn  
            listItem.set_item("ModuleAccess", $scope.appProperties.SystemAccess.ModuleAccess);  //-ModuleAccess    
            listItem.set_item("AceMoudle", $scope.appProperties.SystemAccess.AceMoudle);  //-AceMoudle 
            //checkbox information   
            listItem.set_item("EveModules", $scope.SelectionCheckboxEVEmodules);  //-hoice checkbox          
            listItem.set_item("TypeofRequest", $scope.SelectionCheckboxInformation);  //-hoice checkbox  
            listItem.set_item("RoleInformation", $scope.SelectionCheckboxRoleInformation);  //-hoice checkbox 
            listItem.set_item("AccessOption", $scope.appProperties.SystemAccess.AccessOption);  //-dropdwpn        
         
            // datefield    
            if ($scope.appProperties.SystemAccess.DateRequiredBy != null && $scope.appProperties.SystemAccess.DateRequiredBy != undefined) {
                var dateParts = $scope.appProperties.SystemAccess.DateRequiredBy.split("/");
                // month is 0-based, that's why we need dataParts[1] - 1
                var DteRequiredBy = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
                listItem.set_item("DateRequiredBy", DteRequiredBy);
            }
            if ($scope.appProperties.SystemAccess.RequestedDate != null && $scope.appProperties.SystemAccess.RequestedDate != undefined) {
            
                listItem.set_item("RequestedDate", $scope.appProperties.SystemAccess.RequestedDate);
            }
            if ($scope.appProperties.SystemAccess.BranchHeadDate != null && $scope.appProperties.SystemAccess.BranchHeadDate != undefined) {
                listItem.set_item("BranchHeadDate", $scope.appProperties.SystemAccess.BranchHeadDate);
              
            }
            if ($scope.appProperties.SystemAccess.SystemAccessHeadDate != null && $scope.appProperties.SystemAccess.SystemAccessHeadDate != undefined) {
             
                listItem.set_item("SystemAccessHeadDate", $scope.appProperties.SystemAccess.SystemAccessHeadDate);
            }
            if ($scope.appProperties.SystemAccess.EVEModuleLeaderDate != null && $scope.appProperties.SystemAccess.EVEModuleLeaderDate != undefined) {

                listItem.set_item("EVEModuleLeaderDate", $scope.appProperties.SystemAccess.EVEModuleLeaderDate);
            }
            if ($scope.appProperties.SystemAccess.EVEModuleOwnerDate != null && $scope.appProperties.SystemAccess.EVEModuleOwnerDate != undefined) {

                listItem.set_item("EVEModuleOwnerDate", $scope.appProperties.SystemAccess.EVEModuleOwnerDate);
            }
            if ($scope.appProperties.SystemAccess.ImplementationOfficerDate != null && $scope.appProperties.SystemAccess.ImplementationOfficerDate != undefined) {

               
                listItem.set_item("ImplementationOfficerDate", $scope.appProperties.SystemAccess.ImplementationOfficerDate);
            }
            //all people picker field check value 
            if ($scope.appProperties.SystemAccess.RequestedFor != undefined) {
                if ($scope.appProperties.SystemAccess.RequestedFor.Id) {
                    listItem.set_item("RequestedFor", $scope.appProperties.SystemAccess.RequestedFor.Id);
                    listItem.set_item("RequestedForLoginID", $scope.appProperties.SystemAccess.RequestedFor.LoginName);
                }
            }
            if ($scope.appProperties.SystemAccess.BranchHead.Id) {
                listItem.set_item("BranchHead", $scope.appProperties.SystemAccess.BranchHead.Id);
            }
            else {
                if ($scope.appProperties.BranchHeadMaster != null && $scope.appProperties.BranchHeadMaster != undefined) {
                    listItem.set_item("BranchHead", $scope.appProperties.BranchHeadMaster.Id);
                }
            }
            if ($scope.appProperties.SystemAccess.SystemAccessHead.Id) {

                listItem.set_item("SystemAccessHead", $scope.appProperties.SystemAccess.SystemAccessHead.Id);
            }
            if ($scope.appProperties.SystemAccess.Level1Approver != undefined) {
                if ($scope.appProperties.SystemAccess.Level1Approver.Id) {
                    listItem.set_item("Level1Approver", $scope.appProperties.SystemAccess.Level1Approver.Id);
                }
            }
            if ($scope.appProperties.SystemAccess.ModuleOwner != undefined) {
                if ($scope.appProperties.SystemAccess.ModuleOwner.Id) {
                    listItem.set_item("ModuleOwner", $scope.appProperties.SystemAccess.ModuleOwner.Id);
                }
            }
            //eve module
            if ($scope.appProperties.SystemAccess.EVEModuleOwner != undefined) {
                if ($scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                    listItem.set_item("EVEModuleOwner", $scope.appProperties.SystemAccess.EVEModuleOwner.Id);
                }
            }
            if ($scope.appProperties.SystemAccess.EVEModuleLeader != undefined) {
                if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id) {
                    listItem.set_item("EVEModuleLeader", $scope.appProperties.SystemAccess.EVEModuleLeader.Id);
                }
            }
            if ($scope.appProperties.SystemAccess.ImplementationOfficer != undefined) {
                if ($scope.appProperties.SystemAccess.ImplementationOfficer.Id) {
                    listItem.set_item("ImplementationOfficer", $scope.appProperties.SystemAccess.ImplementationOfficer.Id);
                }
                else {
                    if ($scope.appProperties.SystemAccess.ImplementationOfficer != undefined) {
                        if ($scope.IsImplementationOfficer) {
                            listItem.set_item("ImplementationOfficer", $scope.appProperties.CurrentUser.Id);
                        }
                    }
                }
            }           
            //status
            listItem.set_item("BranchHeadStatus", $scope.appProperties.SystemAccess.BranchHeadStatus);
            listItem.set_item("SystemAccessHeadStatus", $scope.appProperties.SystemAccess.SystemAccessHeadStatus);             
            listItem.set_item("EVEModuleLeaderStatus", $scope.appProperties.SystemAccess.EVEModuleLeaderStatus);
            listItem.set_item("EVEModuleOwnerStatus", $scope.appProperties.SystemAccess.EVEModuleOwnerStatus);
            listItem.set_item("ImplementationOfficerStatus", $scope.appProperties.SystemAccess.ImplementationOfficerStatus);
            //workflow general status                  
            listItem.set_item("ApplicationStatus", ApplicationStatus);
            listItem.set_item("WorkFlowStatus", Worfkflowstatus);

            listItem.update();
            ctx.load(listItem);
            ctx.executeQueryAsync(function () {
                try {
                    insertComments(ctx, web, listItem.get_id(), commentStatus);
                    
                    if ($scope.files.length > 0) {
                        $scope.saveAttachment(Config.SystemAccess, listItem.get_id());
                    }
                    if ($scope.appProperties.ACERoleFilterData.length > 0) {
                        insertUpdateDataDomain(ctx, web, listItem.get_id(), Config.SystemAccessAceDomain, $scope.appProperties.ACERoleFilterData);
                    }
                    //module
                    var dataAry = [];

                    //module-edit
                    angular.forEach($scope.SelectionModuleEditCheckboxInformation, function (value, key) {
                        dataAry.push({
                            ID: key, MavenGroupId: value.ID, Title: value.Title, Layer: value.Layer, Feature: value.Feature, DataSet: value.Title, ViewAccess: "0", EditAccess: "1" //0-false, 1-true
                        });
                    });
                    //view
                    angular.forEach($scope.SelectionModuleViewCheckboxInformation, function (value, key) {
                        dataAry.push({
                            ID: key, MavenGroupId: value.ID, Title: value.Title, Layer: value.Layer, Feature: value.Feature, DataSet: value.Title, ViewAccess: "1", EditAccess: "0"
                        });
                    });
                    //view
                    angular.forEach($scope.SelectionViewCheckboxInformation, function (value, key) {
                        dataAry.push({
                            ID: key, MavenGroupId: value.ID, Title: value.Title, Layer: value.Layer, Feature: value.Feature, DataSet: value.Title, ViewAccess: "1", EditAccess: "0"
                        });
                    });
                    //edit
                    angular.forEach($scope.SelectionEditCheckboxInformation, function (value, key) {
                        dataAry.push({
                            ID: key, MavenGroupId: value.ID, Title: value.Title, Layer: value.Layer, Feature: value.Feature, DataSet: value.Title, ViewAccess: "0", EditAccess: "1"
                        });
                    });
                    if (dataAry.length > 0) {
                        insertUpdateData(ctx, web, listItem.get_id(), Config.MavenAccessOption, dataAry);
                    }
                    //sendEmail to approver    
                    var AuthorNameTitle = "Hi";
                    if ($scope.appProperties.SystemAccess.Author.Title.indexOf('(') != -1) {
                        AuthorNameTitle = $scope.appProperties.SystemAccess.Author.Title.split('(')[0];
                    }
                    else { AuthorNameTitle = $scope.appProperties.SystemAccess.Author.Title }
                    switch (Worfkflowstatus) {

                        case "1":
                            //send email to branchHead/user- Submitted for approval
                            if ($scope.appProperties.SystemAccess.BranchHead.EMail) {
                                var htmlcontent = getHtMLContent($scope.appProperties.SystemAccess.BranchHead.Title, $scope.appProperties.SystemAccess.BranchHead.EMail);
                                //alert("Send aprpoval Emil to : " + $scope.appProperties.SystemAccess.BranchHead.EMail + " ," + $scope.appProperties.CurrentUser.Email);
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.CurrentUser.Email, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.BranchHead.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, "murali.sreedhar@vertexplus.com", "", "", "TEST System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", htmlcontent);
								}
                            //if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                            //    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.RequestedFor.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted for you. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");

                            //} 
                            break;
                        case "2":  //sendmail to SystemAccessHead
                        case "3":
                            if (Worfkflowstatus == '2') {
                                //send email to branchHead/user- Submitted for approval
                                if ($scope.appProperties.SystemAccess.SystemAccessHead.EMail) {
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.SystemAccessHead.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                }    
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Rejected " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                            }
                            break;
                        case "6": //sendmail to EVEModuleOwner
                            if (Worfkflowstatus == '6') {
                               
                                if ($scope.appProperties.SystemAccess.AceMoudle != "" && $scope.appProperties.SystemAccess.AceMoudle != undefined && $scope.appProperties.SystemAccess.AceMoudle != null) {
                                    if (!$scope.IsACEModuleLeader) {
                                            angular.forEach($scope.appProperties.SystemAccess.EVEModuleLeader.results, function (value, key) {
                                                toEmails += value.EMail + "|";
                                            });
                                            if (toEmails != "") {
                                                toEmails = toEmails.substring(0, toEmails.length - 1);
                                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, toEmails, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                            }
                                        }
                                        else {
                                              if ($scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.EVEModuleOwner.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                                  Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");

                                            }

                                        }
                                    }
                                
                                else {

                                    if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" && $scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Approved") {
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.EVEModuleOwner.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    }
                                    else if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" ) {
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.EVEModuleOwner.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    }
                                    else if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id) {
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.EVEModuleLeader.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    }
                                }
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Rejected " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                            }
                            break;
                        case "7"://sendmail to impoff
                        case "8":
                            if (Worfkflowstatus == '8') {
                                if ($scope.appProperties.SystemAccess.SystemAccess == "ACE") {
                                    var htmlcontent = getHtMLContent($scope.appProperties.SystemAccess.Author.Title, $scope.appProperties.SystemAccess.Author.EMail);
                                    //Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, "PSD_ACE_Servicedesk_From.Accenture@psd.gov.sg", "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", htmlcontent);
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");

                                    //var rcount = 0;

                                    //angular.forEach($scope.appProperties.ACERoleFilterData, function (value, key) {
                                    //    if (value.Title == "Z-NXXXX-PO_GR" && (value.Domain != "" && value.Domain != null)) {
                                    //        rcount++;
                                    //        if (rcount == 1) {
                                    //            Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Approved | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been Approved. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    //        }
                                    //    }
                                    //});
                                }
                                else {
                                    if ($scope.appProperties.SystemAccess.ImplementationOfficer.Id) {
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.ImplementationOfficer.EMail, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    }
                                }
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Rejected " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                            }
                            break;
                        case "4":  //send mail to ImplementationOfficer                               
                        case "5":
                            if (Worfkflowstatus == '4') {
                                angular.forEach($scope.appProperties.SystemAccess.ImplementationOfficer, function (value, key) {
                                    toEmails += value.EMail + "|";
                                });
                                if (toEmails != "") {
                                    toEmails = toEmails.substring(0, toEmails.length - 1);
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, toEmails, "", "", "System Access Request Submitted by " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been submitted. Please endorse the application by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                }
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Status Changed | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has changed status. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Rejected " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                            }
                            break;    
                        case "10":
                        case "11"://completed email
                            if (Worfkflowstatus == '10') {
                                if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                                    var htmlcontent = getHtMLContent($scope.appProperties.SystemAccess.RequestedFor.Title, $scope.appProperties.SystemAccess.RequestedFor.EMail);
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.RequestedFor.EMail, "", "", "System Access Request Approved | " + $scope.appProperties.SystemAccess.RequestedFor.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been Approved. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, "PSD_ACE_Servicedesk_From.Accenture@psd.gov.sg", "", "", "TEST ACE User Account Request Form for " + $scope.appProperties.SystemAccess.Author.Title + "", htmlcontent);
                                } else {
                                    var htmlcontent = getHtMLContent($scope.appProperties.SystemAccess.Author.Title, $scope.appProperties.SystemAccess.Author.EMail);
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Approved | " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been Approved. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, "PSD_ACE_Servicedesk_From.Accenture@psd.gov.sg", "", "", "TEST ACE User Account Request Form for " + $scope.appProperties.SystemAccess.Author.Title + "", htmlcontent);
									
                                }                               
                                angular.forEach($scope.appProperties.SystemAccess.TypeofRequest, function (Xvalue, key) {
                                    if (Xvalue[0] == "Create Account and Add Role" || Xvalue[0] == "Add Role") {
                                        var rcount = 0;
                                        if ($scope.appProperties.SystemAccess.SystemAccess == "ACE") {
                                        angular.forEach($scope.appProperties.ACERoleFilterData, function (value, key) {
                                            if (value.Title == "Z-NXXXX-PROC_PO_CREATOR" && (value.Domain != "" && value.Domain != null)) {
                                                rcount++;
                                                if (rcount == 1) {
                                                    if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.RequestedFor.EMail, "", "", "Appointment as Purchase/Variation Order Creator for Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsPurchaseVariationOrderCreatorForContracts.pdf>Appointment as Purchase Variation Order Creator for Contracts</a> <br/> <br/> This is a system generated email. Please do not reply.");
                                                         }
                                                    else {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Purchase/Variation Order Creator for Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsPurchaseVariationOrderCreatorForContracts.pdf>Appointment as Purchase Variation Order Creator for Contracts</a> <br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                }
                                            }
                                            if (value.Title == "Z-NXXXX-CONTRACT-APPROVER" && (value.Domain != "" && value.Domain != null)) {
                                                rcount++;
                                                if (rcount == 1) {
                                                    if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.RequestedFor.EMail, "", "", "Appointment as Superintending Officer of Tenders and Contracts ", "Dear  " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsSuperintendingOfficerOfTendersAndContracts.pdf>Appointment as Superintending Officer of Tenders and Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                    else {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Superintending Officer of Tenders and Contracts ", "Dear  " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsSuperintendingOfficerOfTendersAndContracts.pdf>Appointment as Superintending Officer of Tenders and Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                }
                                            }
                                            if (value.Title == "Z-NXXXX-PO_APPROVER" && (value.Domain != "" && value.Domain != null)) {
                                                rcount++;
                                                if (rcount == 1) {
                                                    if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.RequestedFor.EMail, "", "", "Appointment as Approving Officer for Procurement Transactions", "Dear  " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsApprovingOfficerForProcurementTransactions.pdf>Appointment as Approving Officer for Procurement Transactions</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                    else {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Approving Officer for Procurement Transactions", "Dear  " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsApprovingOfficerForProcurementTransactions.pdf>Appointment as Approving Officer for Procurement Transactions</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                }
                                            }
                                            if (value.Title == "Z-NXXXX-PO_GR" && (value.Domain != "" && value.Domain != null)) {
                                                rcount++;
                                                if (rcount == 1) {
                                                    if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.RequestedFor.EMail, "", "", "Appointment as Goods Receipts Notification Officer ", "Dear  " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsGoodsReceiptsNotificationOfficer.pdf>Appointment as Goods Receipts Notification Officer</a><br/> <br/> This is a system generated email. Please do not reply.");

                                                    }
                                                    else {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Goods Receipts Notification Officer ", "Dear  " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsGoodsReceiptsNotificationOfficer.pdf>Appointment as Goods Receipts Notification Officer</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                   
                                                }
                                            }
                                            if (value.Title == "Z-NXXXX-PO_CREATOR" && (value.Domain != "" && value.Domain != null)) {
                                                rcount++;
                                                if (rcount == 1) {
                                                    if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Purchase/Variation Order Creator for Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsPurchaseVariationOrderCreatorForContracts.pdf>Appointment as Purchase Variation Order Creator for Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                    else {
                                                        Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Purchase/Variation Order Creator for Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsPurchaseVariationOrderCreatorForContracts.pdf>Appointment as Purchase Variation Order Creator for Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    }
                                                }
                                            }
                                        });
                                        }
                                        if ($scope.appProperties.SystemAccess.SystemAccess == "EVE") {
                                            angular.forEach($scope.appProperties.SystemAccess.RoleInformation, function (value, key) {
                                                if (value[0] == "CMWOM_WOIO") {
                                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Purchase/Variation Order Creator for Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsPurchaseVariationOrderCreatorForContracts.pdf>Appointment as Purchase Variation Order Creator for Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");
                                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Goods Receipts Notification Officer ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsGoodsReceiptsNotificationOfficer.pdf>Appointment as Goods Receipts Notification Officer</a><br/> <br/> This is a system generated email. Please do not reply.");

                                                }
                                                if (value[0] == "CMWOM_ISSUE") {
                                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Purchase/Variation Order Creator for Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsPurchaseVariationOrderCreatorForContracts.pdf>Appointment as Purchase Variation Order Creator for Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");

                                                }
                                                if (value[0] == "CMWOM_AO") {
                                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Approving Officer for Procurement Transactions", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsApprovingOfficerForProcurementTransactions.pdf>Appointment as Approving Officer for Procurement Transactions</a><br/> <br/> This is a system generated email. Please do not reply.");

                                                }
                                                if (value[0] == "CMWOM_GRN") {
                                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Goods Receipts Notification Officer ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsGoodsReceiptsNotificationOfficer.pdf>Appointment as Goods Receipts Notification Officer</a><br/> <br/> This is a system generated email. Please do not reply.");

                                                }
                                                if (value[0] == "CMWOM_SO") {
                                                    Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "Appointment as Superintending Officer of Tenders and Contracts ", "Dear " + AuthorNameTitle + " <br/> <br/> <br/>  <br/> Please refer to the appointment letter of your roles and responsibilities below. <br/> <a href=" + siteAbsoluteUrl + "/Documents/AppointmentAsSuperintendingOfficerOfTendersAndContracts.pdf>Appointment as Superintending Officer of Tenders and Contracts</a><br/> <br/> This is a system generated email. Please do not reply.");

                                                }


                                            });
                                        }
                                    }
                                });
                            }
                            else {
                                //rejected email
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Rejected " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");
                            }
                            break;
                        case "9"://rejected email
                            if (Worfkflowstatus == '9') {
                                Utility.helpers.SendMailService(hostName, portNumber, fromEmail, dispName, $scope.appProperties.SystemAccess.Author.EMail, "", "", "System Access Request Rejected " + $scope.appProperties.SystemAccess.Author.Title + "", "This is to notify you that System Access Request " + $scope.appProperties.SystemAccess.ApplicationID + " from " + $scope.appProperties.SystemAccess.Author.Title + " has been rejected. Please check the application status by clicking <a href=" + siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx>here</a>");

                            }
                            break; 
                        default:                        
                    }//switch
                }
                catch (err) { //do some 
                    console.log(err);
                }
                $timeout(function () {
                    alert(message);
                    if (Worfkflowstatus != "1") {
                        window.location.href = siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx#pending";
                    } else {
                        window.location.href = siteAbsoluteUrl + "/Pages/SystemAccessGrid.aspx#all";
                    }
                }, 12000);

            }, function (sender, args) {
                console.log("something went wrong");
                console.log('Err: ' + args.get_message());
            });
        };
      
        function insertUpdateDataDomain(ctx, web, itemId, ListName, ListData) {
            if (ListData.length > 0) {
                // create the ListItemInformational object 
                var spxllist = web.get_lists().getByTitle(ListName);
                //insert data to sharepoint        
                var splistItem = "";
                angular.forEach(ListData, function (value, key) {
                    if (value.NewID == "New") {
                        var listCreationInformation = new SP.ListItemCreationInformation();
                        splistItem = spxllist.addItem(listCreationInformation);
                    } else {
                        splistItem = spxllist.getItemById(value.ID);
                    }
                    splistItem.set_item('Domain', value.Domain); //id here productname
                    splistItem.set_item('JobDescription', value.ACERoleDescription);
                    splistItem.set_item('JobRoleCode', value.Title);
                    splistItem.set_item('SysAccessListItemID', itemId);
                    splistItem.update();
                });
                ctx.load(splistItem);
                ctx.executeQueryAsync(function () {
                    console.log("insertUpdateDatDomain")
                }, function (sender, args) {
                    console.log("something went wrong");
                    console.log('Err: ' + args.get_message());
                });
            }
        };
        function insertUpdateData(ctx, web, itemId, ListName, ListData) {
            if (ListData.length > 0) {
                // create the ListItemInformational object 
                var spxllist = web.get_lists().getByTitle(ListName);
                //insert data to sharepoint        
                var splistItem = "";
                angular.forEach(ListData, function (value, key) {
                    if (value.AccessOptionId == "" || value.AccessOptionId == undefined) {
                        var listCreationInformation = new SP.ListItemCreationInformation();
                        splistItem = spxllist.addItem(listCreationInformation);
                    } else {
                        splistItem = spxllist.getItemById(value.AccessOptionId);
                    }
                    splistItem.set_item('Title', value.DataSet);
                    splistItem.set_item('MavenGroupId', value.MavenGroupId);
                    splistItem.set_item('EditAccess', value.EditAccess); //id here productname
                    splistItem.set_item('ViewAccess', value.ViewAccess); //id here productname
                    splistItem.set_item('GroupName', value.Feature);
                    splistItem.set_item('DataSet', value.DataSet);
                    splistItem.set_item('SystemAccessId', itemId);
                    splistItem.update();
                });
                ctx.load(splistItem);
                ctx.executeQueryAsync(function () {
                    console.log("insertUpdateData")
                }, function (sender, args) {
                    console.log("something went wrong");
                    console.log('Err: ' + args.get_message());
                });
            }
        };
        //insert comments
        function insertComments(ctx, web,listItemId, status) {
            $scope.loaded = true;
            var clist = web.get_lists().getByTitle("" + Config.SystemAccessComments + "");
            // create the ListItemInformational object             
            var clistItemInfo = new SP.ListItemCreationInformation();
            // add the item to the list  
            var clistItem = "";
            clistItem = clist.addItem(clistItemInfo);
            if ($scope.appProperties.appComments.Comments != null && $scope.appProperties.appComments.Comments != "") {
                clistItem.set_item('UserComments', $scope.appProperties.appComments.Comments + " ( " + Config.SysCommentStatus[status] + " )");
            } else {
                clistItem.set_item('UserComments', Config.SysCommentStatus[status] + " (System Comments)");
            }
            clistItem.set_item('CommentsBy', $scope.appProperties.CurrentUser.Id);
            clistItem.set_item('SysAccessListItemID', listItemId);
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
        //get randon number  -text-year-Randomnumber-lastitemid(2019_001)
        function getUniqueNumber() {
            //Generate Document Numbr -getting NTP lasitem id
            uService.getLastListITem(Config.SystemAccess).then(function (response) {
                if (response.data.d.results.length > 0) {
                    var count = response.data.d.results[0].Id + 1;
                    $scope.appProperties.SystemAccess.ApplicationID = "SA-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + count; //increment one
                }
                else {
                    $scope.appProperties.SystemAccess.ApplicationID = "SA-" + $filter('date')(new Date(), 'ddMMyyyy') + "-00" + "1"
                }
            });

        };
        //Load data using querstring
        function loadSystemAccessAceDomainListData(ListName, colName, ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getsubListItemByParentId(ListName,colName,ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.ACERoleFilterData.push({
                            ID: value.ID, NewID: "Old", Title: value.JobRoleCode, ACERoleDescription: value.JobDescription, Domain: value.Domain, DomainIndex: "", ViewAccess: "0", EditAccess: "0" //0-false, 1-true
                        });
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
        function LoadAllMasterListData() {
            //maven group master
            uService.getAllMasterListData(Config.MavenGroupMaster).then(function (response) {
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.MavenGroupMaster.push({
                            ID: key, AccessOptionId: "", MavenGroupId: value.ID, Title: value.Title, Layer: value.Layer, Feature: value.Feature, DataSet: value.Title, ViewAccess: "0", EditAccess: "1" //0-false, 1-true
                        });
                    });
                }
            });
            //EVEModuleMaster
            uService.getAllMasterListDataLookup(Config.EVEModuleMaster).then(function (response) {
                if (response.data.d.results.length > 0) {
                    $scope.CheckboxEVEmodules = response.data.d.results;
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.CheckboxEVEmoduleMaster.push({
                            ID: key, Code: value.System.Id, System: value.System.Title, Title: value.Title //expendcolumn
                        });
                    });
                }
            });

            //PAL/LicensceRoleMaster -SystemAccessRoleMaster
            uService.getMasterDataWithLookupColumn(Config.SystemAccessRoleMaster, "SystemName").then(function (response) {
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.PALLICENSERoleMasterData.push({
                            ID: key, Title: value.Title, SystemName: value.SystemName.Title //expendcolumn
                        });
                    });
                }
            });
            //EveRoleMaster 
            uService.getMasterDataWithLookupColumn(Config.EveRoleMaster, "EVEModule").then(function (response) {
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.EveRoleMasterData.push({
                            ID: key, Title: value.Title, EVEModule: value.EVEModule.Title //expendcolumn
                        });
                    });
                }
            });
            //systemaccessmaster
            uService.getAllMasterListData(Config.SystemAccessMaster).then(function (response) {
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.SystemAccessMaster = response.data.d.results;
                }
            });
            //systemaccessacedomainmaster
            uService.getAllMasterListData(Config.ACEDomainMaster).then(function (response) {
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ACEDomainMasterData = response.data.d.results;
                }
            });
            //ACEModule
            uService.getMasterDataWithLookupColumn(Config.ACERoleMaster, "ACEModule").then(function (response) {
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        $scope.appProperties.ACERoleMasterData.push({
                            ID: key, Title: value.Title, ACERoleDescription: value.ACERoleDescription, ACEModule: value.ACEModule.Title //expendcolumn
                        });
                    });
                }
            });
            //ACEModuleMaster
            uService.getAllMasterListData(Config.ACEModuleMaster).then(function (response) {
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.ACEModuleMasterData = response.data.d.results;
                }
            });
            //MavenAccessOptionMaster
            uService.getAllMasterListData(Config.MavenAccessOptionMaster).then(function (response) {
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.AccessOptionMaster = response.data.d.results;
                }
            });
            //SystemAccessTypeofRequest
            uService.getAllMasterListData(Config.SystemAccessTypeofRequest).then(function (response) {
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.TypeOfRequestMaster = response.data.d.results;
                }
            });
        };
        function getEVERoles() {
            //get roles
           
            if ($scope.appProperties.EveRoleMasterData.length > 0) {
                angular.forEach($scope.SelectionCheckboxEVEmodules, function (value, key) {
                    getEveModuleApprover(value); //get approver from this module
                    var filterEVERole = $filter('filter')($scope.appProperties.EveRoleMasterData, { EVEModule: value }, true);
                    angular.forEach(filterEVERole, function (value, key) {
                        $scope.CheckboxRoleInformation.push({ ID: key, Title: value.Title });                      
                    });
                });
            }
        }
      
        //Load data using querstring
        function loadListItemData(ListItemId) {
            $scope.loaded = true; //spinner start -service call start
            uService.getById(Config.SystemAccess, ListItemId).then(function (response) {
                $scope.loaded = false; //spinner stop - service call end 
                if (response.data.d.results.length > 0) {
                    $scope.appProperties.SystemAccess = response.data.d.results[0];  
                    $scope.appProperties.SystemAccess.SystemAccess = response.data.d.results[0].SystemAccess;
                    $scope.appProperties.SystemAccess.Author.LoginName = $scope.appProperties.SystemAccess.RequestorLoginID;
                    $scope.appProperties.SystemAccess.RequestedFor.LoginName = $scope.appProperties.SystemAccess.RequestedForLoginID;
                    $scope.appProperties.SystemAccess.RequestedFor.Title = response.data.d.results[0].RequestedFor.Title;
                    $scope.getAccessRequestType(); //bind selected data


                    $scope.getSystemRequestType({ SystemAccessCode: $scope.appProperties.SystemAccess.SystemAccessCode, Title: $scope.appProperties.SystemAccess.SystemAccess, WorkFlowCode: $scope.appProperties.SystemAccess.WorkFlowCode}); //after laoding chekcbx call this condition

                    //trype of request -Chekcobx
                    if ($scope.appProperties.SystemAccess.EveModules != null) {
                        $scope.SelectionCheckboxEVEmodules.length = 0;
                        $scope.SelectionCheckboxEVEmodules = $scope.appProperties.SystemAccess.EveModules.results;
                        if ($scope.appProperties.SystemAccess.EveModules.results.length > 0) {
                            getEVERoles(); //if eve roles enable means                           
                        }  
                    }
                    if ($scope.appProperties.SystemAccess.TypeofRequest != null) {
                        $scope.SelectionCheckboxInformation.length = 0;
                        $scope.SelectionCheckboxInformation = $scope.appProperties.SystemAccess.TypeofRequest.results;
                    }
                    if ($scope.appProperties.SystemAccess.RoleInformation != null) {
                        $scope.SelectionCheckboxRoleInformation.length = 0;
                        $scope.SelectionCheckboxRoleInformation = $scope.appProperties.SystemAccess.RoleInformation.results;                                         
                    }                   
                    //trnsfer-chekcobx
                    if ($scope.appProperties.SystemAccess.TransferInAdvance == "1") {
                        $scope.Ontransfer = true;
                    } else { $scope.Ontransfer = false; }
                    //chekcob-obbehhlaf
                    if ($scope.appProperties.SystemAccess.OnBehalf == "1") {
                        $scope.OnBehalf = true;
                    } else { $scope.OnBehalf = false; }

                    $scope.appProperties.SystemAccess.DateRequiredBy = $filter('date')($scope.appProperties.SystemAccess.DateRequiredBy, 'dd/MM/yyyy') //dateRequrie
                    //enable approval button
                    DisableHideShowControls("Edit", $scope.appProperties.CurrentUser.Id) //disable controls while load and approval status

                }
            });

        };
        //listname -paranmeter value
        function getNextLevelApprover(SysccessCode,workFlowcode) {
            uService.getNextLevelApprover(Config.SystemAccessMaster, SysccessCode,workFlowcode).then(function (response) {
                if (response.data.d.results.length > 0) {
                    
                    //if ($scope.appProperties.SystemAccess.SystemAccessHead.Id == undefined) {
                    //    $scope.appProperties.SystemAccess.SystemAccessHead = response.data.d.results[0].SystemAccessHead;                     
                    //}
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == $scope.appProperties.SystemAccess.SystemAccess) {
                            $scope.appProperties.SystemAccess.SystemAccessHead = value.SystemAccessHead;
                            if (value.SystemAccessHeadId != null && value.SystemAccessHeadId != "") {
                                commonService.getUserbyId(value.SystemAccessHeadId).then(function (response) {
                                    //check out of office
                                    commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                        if (response.data.d.results.length > 0) {
                                            getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                                $scope.appProperties.SystemAccess.SystemAccessHead.Id = user.d.Id;
                                                $scope.appProperties.SystemAccess.SystemAccessHead.EMail = user.d.Email;
                                                $scope.appProperties.SystemAccess.SystemAccessHead.Title = user.d.Title;
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
        //listname -paranmeter value
        function getNextLevelMultiImplemenationApprover(SysccessCode, workFlowcode) {
            uService.getNextLevelApprover(Config.SystemAccessMaster, SysccessCode, workFlowcode).then(function (response) {
                if (response.data.d.results.length > 0) {
                    angular.forEach(response.data.d.results, function (value, key) {
                        if (value.Title == "Kris" || value.Title == "PALS" || value.Title == "Connect" || value.Title == "LicenceOne" || value.Title == "FileShare") {
                            //$scope.appProperties.SystemAccess.ImplementationOfficer = value.ImplementationOfficer.results;
                            angular.forEach(value.ImplementationOfficer.results, function (value, key) {
                                //if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                //    $scope.IsImplementationOfficer = true;
                                //    $scope.IsSApproveButton = true;
                                //    $scope.IsRejectButton = true;
                                //}
                            });
                        }
                        if (value.Title == $scope.appProperties.SystemAccess.SystemAccess) {
                            $scope.appProperties.SystemAccess.ImplementationOfficer = value.ImplementationOfficer.results;
                            angular.forEach(value.ImplementationOfficer.results, function (value, key) {
                                if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                    $scope.IsImplementationOfficer = true;
                                    $scope.IsSApproveButton = true;
                                    $scope.IsRejectButton = true;
                                }
                            });
                            //if ($scope.appProperties.CurrentUser.Id == value.ImplementationOfficer.results[0].Id) {
                            //    $scope.IsImplementationOfficer = true;
                            //    $scope.IsSApproveButton = true;
                            //    $scope.IsRejectButton = true;
                            //}
                        }
                        
                    });
                }
            });
        };
        function getEveModuleApprover(moduleName) {
            $scope.loaded = true;
            uService.getEveModuleApprover(Config.EVEModuleMaster, moduleName).then(function (response) {
                $scope.loaded = false;
                if (response.data.d.results.length > 0) {
                   
                    if ($scope.appProperties.SystemAccess.EVEModuleOwner.Id == undefined) {
                        $scope.appProperties.SystemAccess.EVEModuleOwner = response.data.d.results[0].EVEModuleOwner;
                        commonService.getUserbyId(response.data.d.results[0].EVEModuleOwnerId).then(function (response) {
                            //check out of office
                            commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                if (response.data.d.results.length > 0) {
                                    getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                        $scope.appProperties.SystemAccess.EVEModuleOwner.Id = user.d.Id;
                                        $scope.appProperties.SystemAccess.EVEModuleOwner.EMail = user.d.Email;
                                        $scope.appProperties.SystemAccess.EVEModuleOwner.Title = user.d.Title;
                                    });
                                }
                            });
                        });
                       // $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending";
                    }
                    if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id == undefined) {
                        $scope.appProperties.SystemAccess.EVEModuleLeader = response.data.d.results[0].EVEModuleLeader;
                            commonService.getUserbyId(response.data.d.results[0].EVEModuleLeaderId).then(function (response) {
                                //check out of office
                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                    if (response.data.d.results.length > 0) {
                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                            $scope.appProperties.SystemAccess.EVEModuleLeader.Id = user.d.Id;
                                            $scope.appProperties.SystemAccess.EVEModuleLeader.EMail = user.d.Email;
                                            $scope.appProperties.SystemAccess.EVEModuleLeader.Title = user.d.Title;
                                        });
                                    }
                                });
                            });
                        
                        if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id) {
                            $scope.appProperties.SystemAccess.EVEModuleLeaderStatus = "Pending";
                        }
                        else {
                            if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus != "Approved") {
                            $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending"; //if modlue leader empty assign next level(module owner) status pending
                        }
                        }
                    }
                    if ($scope.appProperties.SystemAccess.ImplementationOfficer.Id == undefined) {
                        $scope.appProperties.SystemAccess.ImplementationOfficer = response.data.d.results[0].ImplementationOfficer;
                        commonService.getUserbyId(response.data.d.results[0].ImplementationOfficerId).then(function (response) {
                            //check out of office
                            commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                if (response.data.d.results.length > 0) {
                                    getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                        $scope.appProperties.SystemAccess.ImplementationOfficer.Id = user.d.Id;
                                        $scope.appProperties.SystemAccess.ImplementationOfficer.EMail = user.d.Email;
                                        $scope.appProperties.SystemAccess.ImplementationOfficer.Title = user.d.Title;
                                    });
                                }
                            });
                        });
                       // $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Pending";
                    }
                }
                else {
                    alert('Approver not configure this module...!');
                }
            });
        };
        $scope.IsACEModuleLeader = false;
        function getACEModuleApprover(moduleName) {
            $scope.loaded = true;
            uService.getACEModuleApprover(Config.ACEModuleMaster, moduleName).then(function (response) {
                $scope.loaded = false;
                if (response.data.d.results.length > 0){

                    if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id == undefined) {
                        $scope.appProperties.SystemAccess.EVEModuleLeader = response.data.d.results[0].ACEModuleLeader;
                        if ($scope.appProperties.SystemAccess.EVEModuleOwner.Id == undefined) {
                            $scope.appProperties.SystemAccess.EVEModuleOwner = response.data.d.results[0].ACEModuleOwner;
                            commonService.getUserbyId(response.data.d.results[0].ACEModuleOwnerId).then(function (response) {
                                //check out of office
                                commonService.getcoveringOfficerHead("CoveringOfficerList", "UserID", response.data.d.LoginName.split('\\')[1]).then(function (response) {
                                    if (response.data.d.results.length > 0) {
                                        getUserId($scope.DomainName + "\\" + response.data.d.results[0].CoveringApprovingOfficer1UserID).done(function (user) {
                                            $scope.appProperties.SystemAccess.EVEModuleOwner.Id = user.d.Id;
                                            $scope.appProperties.SystemAccess.EVEModuleOwner.EMail = user.d.Email;
                                            $scope.appProperties.SystemAccess.EVEModuleOwner.Title = user.d.Title;
                                        });
                                    }
                                });
                            });
                            //$scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending";
                        }
                        angular.forEach(response.data.d.results[0].ACEModuleLeader.results, function (value, key) {
                            if ($scope.appProperties.CurrentUser.Id == value.Id) {
                                $scope.IsACEModuleLeader = true;
                                $scope.IsSApproveButton = true;
                                $scope.IsRejectButton = true;
                                // this is to check next level approver
                               
                            }

                        });
                        //if ($scope.appProperties.SystemAccess.EVEModuleLeader.Id) {
                            
                        //}
                        //$scope.appProperties.SystemAccess.EVEModuleLeaderStatus = "Approved";
                        //else {
                        //    $scope.appProperties.SystemAccess.EVEModuleOwnerStatus = "Pending"; //if modlue leader empty assign next level(module owner) status pending
                        //}

                    }
                    //if ($scope.appProperties.SystemAccess.ImplementationOfficer.Id == undefined) {
                    //    $scope.appProperties.SystemAccess.ImplementationOfficer = response.data.d.results[0].ACEImplementationOfficer;
                    //    // $scope.appProperties.SystemAccess.ImplementationOfficerStatus = "Pending";
                    //}
                }
                else {
                    alert('Approver not configure this module...!');
                }
            });
        };
        //next level approver
        function DisableHideShowControls(status, currentUsrId) {
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
            if (status == "New") {
                $scope.IsSubmitButton = true;
                $scope.IsCommentsSection = false;
                $scope.IsAfterSubmitDisabled = false;
            }
            //Branch head  approval
            else if ($scope.appProperties.SystemAccess.BranchHeadStatus == "Pending" && currentUsrId == $scope.appProperties.SystemAccess.BranchHead.Id) {
                //enable re-rourte             
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
                if ($scope.appProperties.SystemAccess.SystemAccess == "Kris" || $scope.appProperties.SystemAccess.SystemAccess == "PALS" || $scope.appProperties.SystemAccess.SystemAccess == "Connect" || $scope.appProperties.SystemAccess.SystemAccess == "LicenceOne" || $scope.appProperties.SystemAccess.SystemAccess == "FileShare" ) {
                    //module master only for kris related
                    getNextLevelApprover($scope.appProperties.SystemAccess.SystemAccessCode, $scope.appProperties.SystemAccess.WorkFlowCode);
                }
                //modelOwener base d on selected module
                if ($scope.appProperties.SystemAccess.EveModules != null) {
                    if ($scope.appProperties.SystemAccess.EveModules.results.length > 0) {
                        angular.forEach($scope.appProperties.SystemAccess.EveModules.results, function (value, key) {
                            getEveModuleApprover(value); //get next lelvel approver from this module
                        });
                    }
                }
                if ($scope.appProperties.SystemAccess.AceMoudle != "" && $scope.appProperties.SystemAccess.AceMoudle != undefined && $scope.appProperties.SystemAccess.AceMoudle != null) {
                   // send email to ACE module leaders
                        getACEModuleApprover($scope.appProperties.SystemAccess.AceMoudle); //get next lelvel approver for ACE module
                    
                }
            }
            else if (($scope.appProperties.SystemAccess.BranchHeadStatus != "Pending"  && $scope.appProperties.SystemAccess.SystemAccessHeadStatus == "Pending") && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.SystemAccessHead.Id) {
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;  
                getNextLevelMultiImplemenationApprover($scope.appProperties.SystemAccess.SystemAccessCode, $scope.appProperties.SystemAccess.WorkFlowCode);
            }
            
            else if (($scope.appProperties.SystemAccess.ImplementationOfficerStatus != "Approved" && $scope.appProperties.SystemAccess.BranchHeadStatus == "Approved") && $scope.appProperties.SystemAccess.SystemAccessHeadStatus == "Approved") {
                //implemetaion approver
                getNextLevelMultiImplemenationApprover($scope.appProperties.SystemAccess.SystemAccessCode, $scope.appProperties.SystemAccess.WorkFlowCode);
            }
           
            else if ($scope.appProperties.SystemAccess.EVEModuleLeaderStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleLeader.Id) {
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            else if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }

            else if ($scope.appProperties.SystemAccess.ImplementationOfficerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.ImplementationOfficer.Id) {
                $scope.IsSApproveButton = true;
                $scope.IsRejectButton = true;
            }
            else if ($scope.appProperties.SystemAccess.BranchHeadStatus == "Approved" && $scope.appProperties.SystemAccess.ApplicationStatus != "4") {

                if ($scope.appProperties.SystemAccess.AceMoudle != "" && $scope.appProperties.SystemAccess.AceMoudle != undefined && $scope.appProperties.SystemAccess.AceMoudle != null) {
                    if ($scope.appProperties.SystemAccess.EVEModuleOwnerStatus == "Pending" && $scope.appProperties.CurrentUser.Id == $scope.appProperties.SystemAccess.EVEModuleOwner.Id) {
                        $scope.IsSApproveButton = true;
                        $scope.IsRejectButton = true;
                    }
                    else {
                        getACEModuleApprover($scope.appProperties.SystemAccess.AceMoudle); //get next lelvel approver for ACE module
                    }
                }
               
            }
            // Rejected status and not equal currentuser and approver (disable/Hide all control)
            else {
                //initailly alardy disbles
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
        function getHtMLContent(autherization,autherizationemail) {                      
            let str = '';
            str += '<html><head></head><body>';
            str += '<p style="color: #222222; font-family: arial; font-size: small; background-color: #ffffff;">New System Access Application received via NParks Connect</a>​​<br /></p>';
            str += ' <br />';
            str += ' <p style="color: #222222; font-family: arial; font-size: small; background-color: #ffffff;">This is to notify you that a New System Access Application For ACE (Reference No.SA_ACE190867) from Amanda Man Yi ( Community Engagement , 7) has been submitted and approved by system owners. The application is now pending for your action.</a>​​<br /> </p>';
            str += ' <br />';
            str += '<table cellpadding = "0" cellspacing = "0" border = "0" style = "color: #222222; font-family: arial; font-size: 11px; background-color: #ffffff;" > ';
            //Details Of User
            str += '<tr><td style="height:20px;" colspan="2"><b><u>Details Of User</u></b></td></tr>';
            str += ' <tr><td>Agency: </td><td>NPARKS</td></tr>';
            str += '<tr><td>Name: </td><td>' + $scope.appProperties.SystemAccess.Author.Title + '</td></tr>';
            //str += '<tr><td>Department: </td><td> </td></tr>';
            str += '<tr><td>Email Address: </td><td>' + $scope.appProperties.SystemAccess.Author.EMail + '</td></tr>';
            str += '<tr><td>User ID(LAN ID): </td><td>' + $scope.appProperties.SystemAccess.Author.LoginName + '</td></tr>';
            str += '<tr><td>Type Of Request: </td><td>' + $scope.appProperties.SystemAccess.SystemAccess + '</td></tr>';
            str += '<tr><td>Effective Date : </td><td>' + $scope.appProperties.SystemAccess.DateRequiredBy + '</td></tr';
            str += '<tr><td>Expiry Date: </td><td>' + $scope.appProperties.SystemAccess.DateRequiredBy + '</td></tr>';
            //Password Receipt Method
            str += '<tr><td style="height:20px;" colspan="2"><b><u>Password Receipt Method</u></b></td></tr> ';
            str += ' <tr><td>Hardcopy : </td><td>No</td></tr>';
            str += '<tr><td>Softcopy: </td><td>No</td></tr> ';
            str += '<tr><td>Supervisor/ Authorised Personnel Email: </td><td>' + autherizationemail+'</td></tr>';
            str += '<tr><td>VIP  : </td><td>N/A</td></tr> ';
            str += ' <tr><td>Indicate VIP designation: </td><td>N/A</td></tr>';
            //Authorisation Request Details
            str += '<tr><td style="height:20px;" colspan="2"><b><u>Authorisation Request Details</u></b></td></tr>';
            str += '<tr><td>User Type : </td><td>' + autherization+'</td></tr>';
            str += '<tr><td>Others (if-any): </td><td>N/A</td></tr>';
            str += '<tr><td>Reason For Reques: </td><td>' + $scope.appProperties.SystemAccess.RemarksReasons +'</td></tr>';
            //Details Of Requestor
            str += ' <tr><td style="height:20px;" colspan="2"><b><u>Details Of Requestor</u></b></td></tr>';      
            str += '<tr><td>Name : </td><td>' + $scope.appProperties.SystemAccess.RequestedFor.Title + '</td></tr>';
            str += '<tr><td>Designation : </td><td>' + $scope.appProperties.SystemAccess.RequestedFor.Title+'</td></tr>';
            str += '<tr><td>Email Address: </td><td>' + $scope.appProperties.SystemAccess.RequestedFor.EMail +'</td></tr>';
            str += '<tr><td>Telephone No: </td><td>89898989</td></tr>';
            str += ' <tr><td>Date : </td><td>' + $scope.appProperties.SystemAccess.RequestedDate +'</td></tr>';
            //Authorised By
            str += '<tr><td style="height:20px;" colspan="2"><b><u>Authorised By</u></b></td></tr>';
            str += '<tr><td>User ID: </td><td>' + autherization +'</td></tr>';
            str += ' <tr><td>Date : </td><td>' + $scope.appProperties.SystemAccess.RequestedDate+'</td></tr>';
            str += ' <tr><td colspan="2"> </td></tr>';
            str += '</table>';
            //Type of Role Selected By
            str += '<p style="color: #222222; font-family: arial; font-size: small; background-color: #ffffff;"><b><u>Type of Role Selected By</u></b></p>';
            str += ' <table cellpadding="0" cellspacing="0" border="1" style="color: #222222; font-family: arial; font-size: 11px; background-color: #ffffff;">';
            str += ' <tbody>';
            str += '<tr><td><strong>Agency</strong></td><td><strong>Process</strong></td><td><strong>Job Role Code</strong></td><td><strong>Job Role Description</strong></td><td><strong>Domain</strong></td></tr>';
            angular.forEach($scope.appProperties.ACERoleFilterData, function (value, key) {
                if (value.Domain != "" && value.Domain != null && value.Domain != undefined) {
                    str += '<tr>';
                    str += '<td>NParks</td>';
                    str += '<td>F&P</td>';
                    str += '<td>' + value.Title + '</td>';
                    str += '<td>' + value.ACERoleDescription + '</td>';
                    str += '<td>' + value.Domain + '</td>';
                    str += '</tr>';
                }
            });           
            str += '</tbody>';
            str += '</table>';
            str += ' <p style="color: #222222; font-family: arial; font-size: small; background-color: #ffffff;">Please view the details of and update application on NParks Connect:&#160;<a href="http://connect.nparks.gov.sg/" target="_blank" data-saferedirecturl="https://www.google.com/url?q=http://Connect.nparks.gov.sg&amp;source=gmail&amp;ust=1641375539867000&amp;usg=AOvVaw1NtLWlWF2vQqhAaVlL8D7e" style="color: #1155cc;">http://Connect.nparks.gov.sg</a>​​<br /></p>';
            str += ' <br />';
            str += '</body>';
            str += '</html>​';
            
            return str;
        }
        //javascript methods---------------------------------------------------------------------------------------- end      

    }]);
}) ();