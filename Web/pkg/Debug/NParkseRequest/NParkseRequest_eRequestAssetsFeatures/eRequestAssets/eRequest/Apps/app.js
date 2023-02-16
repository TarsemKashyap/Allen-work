var app = ""; //declare global, so you can access controller,services
"use strict";
(function () {
    app = angular.module("eRequestApp", ["ngSanitize", "ui.bootstrap", "ngGrid", "ngTable", "angular.filter", "ui.filters"]); //ngSanitize- using html binding

    app.config(['$locationProvider', function ($locationProvider) {
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
    }]);
    app.constant('Config', {
        //SharePoint List
        UtilityRequests: 'UtilityRequests',
        SystemAccessRoleMaster:'SystemAccessRoleMaster',
        MavenAccessOptionMaster: 'MavenAccessOptionMaster',
        MavenAccessOption: 'MavenAccessOption', //sublist
        MavenGroupMaster: 'MavenGroupMaster',
        ACEDomainMaster: 'ACEDomainMaster',
        ACERoleMaster: 'ACERoleMaster',
        EVEModuleMaster:'EVEModuleMaster',
        EveRoleMaster: 'EveRoleMaster',
        ACEModuleMaster:'ACEModuleMaster',
        SystemAccessMaster: 'SystemAccessMaster',
        EVEModuleMaster:'EVEModuleMaster',
        SystemAccessApprovers: 'SystemAccessApprovers',
        SystemAccessComments: 'SystemAccessComments',
        SystemAccessAceDomain:'SystemAccessAceDomain',
        SystemAccessTypeofRequest:'SystemAccessTypeofRequest',
        SystemAccess: 'SystemAccess',
        StaffDirectory: 'StaffDirectory',
        EMailConfig: 'EMailConfig',
        UtilityApprovers: 'UtilityApprovers',
        CostCentreMaster: 'CostCentreMaster',
        PurposeMaster: 'PurposeMaster',
        VendorMaster: 'VendorMaster',      
        UserLog: 'UserLog',
        UtilityDocuments: 'UtilityDocuments',
        UtilityComments: 'UtilityComments',   
        //ITService
        ITRequests: 'ITRequests',
        ITRequestApprovers: 'ITRequestApprovers',
        WorkArea: "WorkArea",
        TypeOfRequest: "TypeOfRequest",
        BranchDivisionHeads: 'BranchDivisionHeads',
        DivisionHeads: 'DivisionHeads',
        BranchHeads: 'BranchHeads',
        ClusterHeads: 'ClusterHeads',
        
        ITRequestDocuments: "ITRequestDocuments",
        OfficeRenovationDocuments: "OfficeRenovationDocuments",
        OfficeRenovationChkListDocuments: "OfficeRenovationChkListDocuments",
        InstallUpgradeSWDocuments: "InstallUpgradeSWDocuments",
        ITComments: "ITComments",
        ITRequestComments:"ITRequestComments",
        AdminOwnersGroup: 'eRequest Owners', //checking export function
        ITAdminGroup: 'ITAdmin', //checking export function
        CRSRAdminGroup: ' CRSRAdmin', //checking export function
        UtilityAdminGroup: 'UtilityAdmin', //checking export function
        Roles: {
            //AuthorizationName(Tile) ITRequestApprovers -Custom List title value
            BranchHead: 'BranchHead',
            ImplementationOfficer: 'ImplementationOfficer',
            ITProjectManager: 'ITProjectManager',
            GroupDirector: 'GroupDirector',
            FinanceHead:'FinanceHead',
            CIO: 'CIO',
            FundingAuthority: 'FundingAuthority',
            SystemOwner: 'SystemOwner',
            DelegateApprover: 'DelegateApprover'
        }, 
        //HReMRF constants
        HRRequests: "HRRequests",
        HRComments: "HRComments",
        HRTypeOfManpowerRequest: "HRTypeOfManpowerRequest",
        ManpowerType: "ManpowerType",
        ComputerRequirement: "ComputerRequirement",
        DurationOfDeployment: "DurationOfDeployment",
        HRFundingTypes: "HRFundingTypes",
        HReMRApprovers: "HReMRApprovers",
        ProposedInterns: "ProposedInterns",
        ProposedTemporaryWorker: "ProposedTemporaryWorker",
        HRDocuments: "HRDocuments",
        HRResumes: "HRResumes",
        //CRSRService
        CRSR: 'CRSR',
        CRSRCostCentre: 'CRSRCostCentre',
        CRSRFundCentre:'CRSRFundCentre',
        CRSRDocuments:'CRSRDocuments',
        CRSRApprovers: 'CRSRApprovers',
        CRSRComments: "CRSRComments",
        CRSRProjectSystem: "CRSRProjectSystem",
        CRSROwnersGroup: 'eRequest Owners', //checking export function
        //using dashbaord page
        CRSRCommentStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead',
            4: 'Approved by ITManager',
            5: 'Rejected by ITManager',
            6: 'Approved by SystemOwner',
            7: 'Rejected by SystemOwner',
            8: 'Approved by FundingAuthority',
            9: 'Rejected by FundingAuthority',
            10: 'Re-Delegate Approver',
            11: 'Closed'
        },
        //using dashbaord page -WorkFlowStatus
        CRSRWFStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead',
            4: 'Approved by ITManager',
            5: 'Rejected by ITManager',
            6: 'Approved by SystemOwner',
            7: 'Rejected by SystemOwner',
            8: 'Approved by FundingAuthority', 
            9: 'Rejected by FundingAuthority',
            10: 'Re-Delegated Approver',
            11: 'Closed'
        },
        //using dashbaord page -WorkFlowStatus
        CRSRWFAdminStatus: {
            1: 'Pending level-1',
            2: 'Pending level-2',          
            4: 'Pending level-3',          
            6: 'Pending level-4',           
            8: 'Pending level-5',
            10: 'Re-Delegated Approver'
        },
        //using dashbaord page -WorkFlowStatus
        ITAdminStatus: {
            1: 'Pending level-1',
            2: 'Pending level-2',
        
            4: 'Pending level-3',
           
            6: 'Pending level-4',
           
            8: 'Pending level-5',
           
            10: 'Approved', ////approved by-flow completed
          
            12: 'Re-routed-Pending level-1',
            13: 'ITManager Approved pending Implementation officer', //dynamic level
            14: 'ITManager Re-Routed pending GroupDirector',
            15: 'ITManager Re-Routed pending CIO',
            16: 'Group Director Approved pending Implementation officer',
            17: 'Group Director Re-Routed pending CIO',
            18: 'level2 approved pending Implementation officer',
            19: 'Re-routed to requester',
            20: 'Re-routed to requester',
            21: 'Re-Delegated Approver'
        },        
        //using dashbaord page -WorkFlowStatus
        UtilityAdminStatus: {
            1: 'Pending level-1',
            2: 'Pending level-2',
            3: 'Approved',
            4: 'Rejected',
            5: 'Rejected',
            10: 'Re-Delegated Approver'
        },
        //using while insert comments list and status number for list
        CommentStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Approved', //approved by financehead-flow completed
            4: 'Rejected by BranchHead',
            5: 'Rejected by FinanceHead',
            6: 'Closed',
            10: 'Re-Delegated Approver'
        },
        //using dashbaord page-utitlity
        WFStatus: {
            1: 'Pending level-1',
            2: 'Pending level-2',
            3: 'Approved', //approved by financehead-flow completed
            4: 'Rejected level-1',
            5: 'Rejected level-2',
            6: 'Closed',
            10: 'Re-Delegated Approver'
        },
        ApplicationStatus: {
            1: 'Pending',
            2: 'InProgress',
            3: 'Approved', 
            4: 'Rejected',
            5: 'Closed'
        },
        //using dashbaord page -WorkFlowStatus
        ITWFStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead', 
            4: 'Approved by ITManager',
            5: 'Rejected by ITManager',
            6: 'Approved by GroupDirector',
            7: 'Rejected by GroupDirector',
            8: 'Approved by CIO',
            9: 'Rejected by CIO',
            10: 'Approved', ////approved by-flow completed
            11: 'Rejected by IMNOfficer',
            12: 'Re-routed',
            13: 'ITManager Approved pending IMOOffice', //dynamic level
            14: 'ITManager Re-Routed pending GroupDirector',
            15: 'ITManager Re-Routed pending CIO',
            16: 'Group Director Approved pending IMOfficer',         
            17: 'Group Director Re-Routed pending CIO',
            18: 'Approved by (Delegated)Branch Head',
            19: 'Re-Work -ITManager SendTo User',
            20: 'Re-Work -IMOfficer SendTo User',
            21: 'Re-Delegated Approver'
        },
        //using dashbaord page
        ITCommentStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead',
            4: 'Approved by ITManager',
            5: 'Rejected by ITManager',
            6: 'Approved by GroupDirector',
            7: 'Rejected by GroupDirector',
            8: 'Approved by CIO',
            9: 'Rejected by CIO',
            10: 'Approved', ////approved by -flow completed
            11: 'Rejected by IMNOfficer',
            12: 'Re-routed by ITManager',
            13: 'Re-routed by GroupDirector',
            14: 'ITManager Re-Routed to User',
            15: 'IMOOfficer Re-Routed to User',
            21: 'Re-Delegated Approver'
        },
        //using dashbaord page
        HRCommentStatus: {
            1: 'New',
            2: 'Approved1',
            3: 'Rejected',
            4: 'Approved',
            5: 'Re-routed'            
        },
        //using dashbaord page -WorkFlowStatus -systemaccess
        SYSWFStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead',
            4: 'Approved by SystemHead',
            5: 'Rejected by SystemAcessHead',
            6: 'Approved by SystemAcessHead',
            7: 'Rejected by ModuleLeader',
            8: 'Approved by ModuleOwner',
            9: 'Rejected by ModuleOwner',
            10: 'Approved', ////approved by-flow completed
            11: 'Rejected by IMNOfficer',
            12: 'Re-routed',
            21: 'Re-Delegated Approver'
        },
        //using dashbaord page -WorkFlowStatus -systemaccess
        SYSACCESSEveStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead',
            4: 'Approved by SystemHead',
            5: 'Rejected by SystemAcessHead',
            6: 'Pending with Module Owner ',
            7: 'Rejected by ModuleLeader',
            8: 'Approved',
            9: 'Rejected by ModuleOwner',
            10: 'Approved', ////approved by-flow completed
            11: 'Rejected by IMNOfficer',
            12: 'Re-routed',
            21: 'Re-Delegated Approver'
        },
        //using dashbaord page -systemaccess
        SysCommentStatus: {
            1: 'New',
            2: 'Approved by BranchHead',
            3: 'Rejected by BranchHead',
            4: 'Approved by SystemAcessHead',
            5: 'Rejected by SystemAcessHead',
            6: 'Approved by ModuleLeader',
            7: 'Rejected by ModuleLeader',
            8: 'Approved by ModuleOwner',
            9: 'Rejected by ModuleOwner',
            10: 'Approved by IMNOfficer', ////approved by-flow completed
            11: 'Rejected by IMNOfficer',
            12: 'Re-routed'  
        },
    });
    //limit to char
    app.filter('ellipsis', function () {
        return function (text, length) {
            if (text.length > length) {
                return text.substr(0, length) + "...";
            }
            return text;
        }
    });
    //numbersOnly
    app.directive('numbersOnly', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, ngModelCtrl) {
                function fromUser(text) {
                    if (text) {
                        // var transformedInput = text.replace(/[^0-9]/g, '');
                        var transformedInput = text.replace(/[^-0-9\.]/g, '');

                        if (transformedInput !== text) {
                            ngModelCtrl.$setViewValue(transformedInput);
                            ngModelCtrl.$render();
                        }
                        return transformedInput;
                    }
                    return undefined;
                }
                ngModelCtrl.$parsers.push(fromUser);
            }
        };
    });
    //customSort
    app.directive("customSort", function () {
        return {
            restrict: 'A',
            transclude: true,
            scope: {
                order: '=',
                sort: '='
            },
            template:
                ' <a ng-click="sort_by(order)" style="color: #555555;">' +
                '    <span ng-transclude></span>' +
                '    <i ng-class="selectedCls(order)"></i>' +
                '</a>',
            link: function (scope) {
                // change sorting order
                scope.sort_by = function (newSortingOrder) {
                    var sort = scope.sort;

                    if (sort.sortingOrder == newSortingOrder) {
                        sort.reverse = !sort.reverse;
                    }

                    sort.sortingOrder = newSortingOrder;
                };
                scope.selectedCls = function (column) {
                    if (column == scope.sort.sortingOrder) {
                        return ('' + ((scope.sort.reverse) ? 'arrow down' : 'arrow up'));
                    }
                    else {
                        return 'icon-sort'
                    }
                };
            }// end link
        }
    });
    app.directive('ngFileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var model = $parse(attrs.ngFileModel);
                var isMultiple = attrs.multiple;
                var modelSetter = model.assign;
                element.bind('change', function () {
                    var values = [];
                    angular.forEach(element[0].files, function (item) {
                        var value = {
                            // File Name
                            name: item.name,
                            //File Size
                            size: item.size,
                            //File URL to view
                            url: URL.createObjectURL(item),
                            // File Input Value
                            _file: item
                        };
                        values.push(value);
                    });
                    scope.$apply(function () {
                        if (isMultiple) {
                            modelSetter(scope, values);
                        } else {
                            modelSetter(scope, values[0]);
                        }
                    });
                });
            }
        };
    }]);
   
    //customSort
})();