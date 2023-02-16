"use strict";
(function () {
    app.factory("HReMR", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        var baseSiteUrl = window.location.protocol + "//" + window.location.hostname;//_spPageContextInfo.siteAbsoluteUrl;
        var getAll = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.PJApproval + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
        var getAllReq = function (Listname) {
            var query = listEndPoint + "/GetByTitle('" + Listname + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
        // get all CRSR Requests data
        var getAllPendingHReMRRequests = function () {

            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=(Level1ApprovalStatus eq 'Pending') or (Level2ApprovalStatus eq 'Pending') or (Level3ApprovalStatus eq 'Pending')&$top=100&$orderby=Modified desc&$select=*"
                + ",Requestor/Id,Requestor/EMail,Requestor/Title"
                + ",ManpowerRequestType"
                + ",ManpowerType"
                + ",SupportingOfficer/Id,SupportingOfficer/EMail,SupportingOfficer/Title"
                + ",ProposedBuddy/Id,ProposedBuddy/EMail,ProposedBuddy/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",Level1ApprovalStatus"
                + ",Level1ApprovalDate"
                + ",Level2Approver/Id,Level2Approver/EMail,Level2Approver/Title"
                + ",Level2ApprovalStatus"
                + ",Level2ApprovalDate"
                + ",Level3Approver/Id,Level3Approver/EMail,Level3Approver/Title"
                + ",Level3ApprovalStatus"
                + ",Level3ApprovalDate"
                + ",ApplicationStatus"
                + "&$expand=Requestor/Id,SupportingOfficer/Id,ProposedBuddy/Id,Level1Approver/Id,Level2Approver/Id,Level3Approver/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
       // get all CRSR Requests data
        var getAllHReMRRequests = function (userId) {
           
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=(Level1Approver/Id eq " + userId + " and Level1ApprovalStatus eq 'Pending') or (Level2Approver/Id eq " + userId + " and Level2ApprovalStatus eq 'Pending') or (Level3Approver/Id eq " + userId + " and Level3ApprovalStatus eq 'Pending')&$top=100&$orderby=Modified desc&$select=*"
                + ",Requestor/Id,Requestor/EMail,Requestor/Title"
                + ",ManpowerRequestType"
                + ",ManpowerType"
                + ",SupportingOfficer/Id,SupportingOfficer/EMail,SupportingOfficer/Title"
                + ",ProposedBuddy/Id,ProposedBuddy/EMail,ProposedBuddy/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",Level1ApprovalStatus"
                + ",Level1ApprovalDate"
                + ",Level2Approver/Id,Level2Approver/EMail,Level2Approver/Title"
                + ",Level2ApprovalStatus"
                + ",Level2ApprovalDate"
                + ",Level3Approver/Id,Level3Approver/EMail,Level3Approver/Title"
                + ",Level3ApprovalStatus"
                + ",Level3ApprovalDate"
                + ",ApplicationStatus"
                + "&$expand=Requestor/Id,SupportingOfficer/Id,ProposedBuddy/Id,Level1Approver/Id,Level2Approver/Id,Level3Approver/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get getAllRequestor CRSR Requests data based on createdby
        var getAllRequestorSubmittedRequests = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=Requestor/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Requestor/Id,Requestor/EMail,Requestor/Title"
                + ",ManpowerRequestType"
                + ",ManpowerType"
                + ",SupportingOfficer/Id,SupportingOfficer/EMail,SupportingOfficer/Title"
                + ",ProposedBuddy/Id,ProposedBuddy/EMail,ProposedBuddy/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",Level1ApprovalStatus"
                + ",Level1ApprovalDate"
                + ",Level2Approver/Id,Level2Approver/EMail,Level2Approver/Title"
                + ",Level2ApprovalStatus"
                + ",Level2ApprovalDate"
                + ",Level3Approver/Id,Level3Approver/EMail,Level3Approver/Title"
                + ",Level3ApprovalStatus"
                + ",Level3ApprovalDate"
                + ",ApplicationStatus"
                + "&$expand=Requestor/Id,SupportingOfficer/Id,ProposedBuddy/Id,Level1Approver/Id,Level2Approver/Id,Level3Approver/Id" //expend for peoplepciker
                return baseService.getRequestAll(query);
        };
        //get all Divisions and Branches
      
        //from staffdirec
        var getAllDivisionsAndBranches = function () {
            var query = baseSiteUrl + listEndPoint + "/GetByTitle('" + Config.StaffDirectory + "')/Items?$select=ID,KnownAs,Designation,ADID,Branch,Division,Location,Organisation,Cluster,Section,Unit,FirstName,LastName,FullName,Email";
            return baseService.getRequestStaffInfo(query);
        };
        // get all CRSR Requests data
        var getAllDeptHeadOfCRSRRequests = function (userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=" + CurrentUserDept + "/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title" 
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",GroupDirector/Id,GroupDirector/EMail,GroupDirector/Title"
                + ",CIO/Id,CIO/EMail,CIO/Title"
                + ",InstallUpgradeSWHostName/Id,InstallUpgradeSWHostName/EMail,InstallUpgradeSWHostName/Title"
                + ",RemovalSWHostName/Id,RemovalSWHostName/EMail,RemovalSWHostName/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ImplementationOfficer/Id,ITProjectManager/Id,GroupDirector/Id,CIO/Id,InstallUpgradeSWHostName/Id,RemovalSWHostName/Id" //expend for peoplepciker
              return baseService.getRequestAll(query);
        };
        // get Pending CRSR Requests data
        var getPendingCRSRRequests = function (CurrentUserStatusFiled,CurrrentUserStatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=(" + CurrentUserStatusFiled+" eq '" + CurrrentUserStatus + "') and (" + CurrrentUserPPLPickerName+"/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title" 
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",GroupDirector/Id,GroupDirector/EMail,GroupDirector/Title"
                + ",CIO/Id,CIO/EMail,CIO/Title"
                + ",InstallUpgradeSWHostName/Id,InstallUpgradeSWHostName/EMail,InstallUpgradeSWHostName/Title"
                + ",RemovalSWHostName/Id,RemovalSWHostName/EMail,RemovalSWHostName/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ImplementationOfficer/Id,ITProjectManager/Id,GroupDirector/Id,CIO/Id,InstallUpgradeSWHostName/Id,RemovalSWHostName/Id" //expend for peoplepciker
           return baseService.getRequestAll(query);
        };
        
        // get ongoing CRSR Requests data
        var getOngoingCRSRRequests = function (appstatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=(ApplicationStatus eq " + appstatus + ") and (" + CurrrentUserPPLPickerName + "/Id eq " + userId +") &$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title" 
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",GroupDirector/Id,GroupDirector/EMail,GroupDirector/Title"
                + ",CIO/Id,CIO/EMail,CIO/Title"
                + ",InstallUpgradeSWHostName/Id,InstallUpgradeSWHostName/EMail,InstallUpgradeSWHostName/Title"
                + ",RemovalSWHostName/Id,RemovalSWHostName/EMail,RemovalSWHostName/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ImplementationOfficer/Id,ITProjectManager/Id,GroupDirector/Id,CIO/Id,InstallUpgradeSWHostName/Id,RemovalSWHostName/Id" //expend for peoplepciker
             return baseService.getRequestAll(query);
        };
        // get closed CRSR Requests data
        var getClosedCRSRRequests = function (aprvdstatus, RejectedStatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$filter=((ApplicationStatus eq " + aprvdstatus + ") or (ApplicationStatus eq " + RejectedStatus + ")) and (" + CurrrentUserPPLPickerName + "/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title" 
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",GroupDirector/Id,GroupDirector/EMail,GroupDirector/Title"
                + ",CIO/Id,CIO/EMail,CIO/Title"
                + ",InstallUpgradeSWHostName/Id,InstallUpgradeSWHostName/EMail,InstallUpgradeSWHostName/Title"
                + ",RemovalSWHostName/Id,RemovalSWHostName/EMail,RemovalSWHostName/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ImplementationOfficer/Id,ITProjectManager/Id,GroupDirector/Id,CIO/Id,InstallUpgradeSWHostName/Id,RemovalSWHostName/Id" //expend for peoplepciker
           return baseService.getRequestAll(query);
        };
      
        //get lastitem id for generate uniqu number (PJnumber)
        var getLastListITem = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?$top=1&$select=* &$orderby=Created desc"
            return baseService.getRequestAll(query);
        };
        //get CRSR Requests by id
        var getById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/items?$filter=ID eq " + listItemId + "&$select=*"
                + ",Requestor/Id,Requestor/EMail,Requestor/Title,Requestor/Name"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title,Requestor/Name"
                + ",ManpowerRequestType"
                + ",ManpowerType"
                + ",SupportingOfficer/Id,SupportingOfficer/EMail,SupportingOfficer/Title,SupportingOfficer/Name"
                + ",ProposedBuddy/Id,ProposedBuddy/EMail,ProposedBuddy/Title,ProposedBuddy/Name"
                + ",PrimaryAdvisor/Id,PrimaryAdvisor/EMail,PrimaryAdvisor/Title,PrimaryAdvisor/Name"
                + ",SecondaryAdvisor/Id,SecondaryAdvisor/EMail,SecondaryAdvisor/Title,SecondaryAdvisor/Name"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",Level1ApprovalStatus"
                + ",Level1ApprovalDate"
                + ",Level2Approver/Id,Level2Approver/EMail,Level2Approver/Title"
                + ",Level2ApprovalStatus"
                + ",Level2ApprovalDate"
                + ",Level3Approver/Id,Level3Approver/EMail,Level3Approver/Title"
                + ",Level3ApprovalStatus"
                + ",Level3ApprovalDate"
                + ",ApplicationStatus"
                + "&$expand=Requestor/Id,RequestedFor/Id,SupportingOfficer/Id,ProposedBuddy/Id,PrimaryAdvisor/Id,SecondaryAdvisor/Id,Level1Approver/Id,Level2Approver/Id,Level3Approver/Id" //expend for peoplepciker
         return baseService.getRequestAll(query);
        };

        //get searched data
        var getSearchedHRRequests = function (applicationId, reqType, status, fromDate, toDate) {
            var searchFilter = "$filter=(";
            var appFilter = [];
            var reqFilter = "";
            var stFilter = "";
            var frmFilter = "";

            if (applicationId != "" && applicationId != undefined)
                appFilter.push("(ApplicationID eq '" + applicationId + "')");
            if (reqType != "" && reqType != undefined)
                appFilter.push("(ManpowerRequestType eq '" + reqType + "')");
            if (status != "" && status != undefined)
                appFilter.push("(ApplicationStatus eq '" + status + "')");
            if (fromDate != "" && fromDate != undefined && (toDate == "" || toDate == undefined))
                appFilter.push("(ApplicationDate eq '" + fromDate + "')");
            else if (fromDate != "" && fromDate != undefined && toDate != "" && toDate != undefined)
                appFilter.push("(ApplicationDate ge '" + fromDate.toISOString() + "') and (ApplicationDate le '" + toDate.toISOString() + "') and Requestor/Id eq " + _spPageContextInfo.userId + "");
            if (appFilter.length > 0) {
                angular.forEach(appFilter, function (value, key) {
                    if (key == 0)
                        searchFilter += value;
                    else
                        searchFilter += " and " + value;
                });
                searchFilter += ")";
               
                var query = listEndPoint + "/GetByTitle('" + Config.HRRequests + "')/Items?" + searchFilter + "&$top=100&$orderby=Modified desc&$select=*"
                    + ",Requestor/Id,Requestor/EMail,Requestor/Title,Requestor/Name"
                    + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title,Requestor/Name"
                    + ",ManpowerRequestType"
                    + ",ManpowerType"
                    + ",SupportingOfficer/Id,SupportingOfficer/EMail,SupportingOfficer/Title,SupportingOfficer/Name"
                    + ",ProposedBuddy/Id,ProposedBuddy/EMail,ProposedBuddy/Title,ProposedBuddy/Name"
                    + ",PrimaryAdvisor/Id,PrimaryAdvisor/EMail,PrimaryAdvisor/Title,PrimaryAdvisor/Name"
                    + ",SecondaryAdvisor/Id,SecondaryAdvisor/EMail,SecondaryAdvisor/Title,SecondaryAdvisor/Name"
                    + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                    + ",Level1ApprovalStatus"
                    + ",Level1ApprovalDate"
                    + ",Level2Approver/Id,Level2Approver/EMail,Level2Approver/Title"
                    + ",Level2ApprovalStatus"
                    + ",Level2ApprovalDate"
                    + ",Level3Approver/Id,Level3Approver/EMail,Level3Approver/Title"
                    + ",Level3ApprovalStatus"
                    + ",Level3ApprovalDate"
                    + ",ApplicationStatus"
                    + "&$expand=Requestor/Id,RequestedFor/Id,SupportingOfficer/Id,ProposedBuddy/Id,PrimaryAdvisor/Id,SecondaryAdvisor/Id,Level1Approver/Id,Level2Approver/Id,Level3Approver/Id" 
                return baseService.getRequestAll(query);
            }
        };

        //getting ManpowerRequestType
        var getManpowerRequestType = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.HRTypeOfManpowerRequest + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }

        //getting ManpowerType
        var getManpowerType = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.ManpowerType + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }

        //getting getDeploymentDuration
        var getDeploymentDuration = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.DurationOfDeployment + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }

        //getting ComputerRequirement
        var getComputerRequirement = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.ComputerRequirement + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }
        
        //getting HRFundingTypes
        var getHRFundingTypes = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.HRFundingTypes + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }
        //getting Intern candidates
        var getInterns = function (appID) {
            var query = listEndPoint + "/GetByTitle('" + Config.ProposedInterns + "')/Items?$filter=(HRParentID eq '" + appID + "')&$top=1000&$orderby=Modified asc&$select=*"
                + ",Attachments,AttachmentFiles&$expand=AttachmentFiles"
            return baseService.getRequest(query);
        }
        //getting Temporary candidates
        var getTemporaryCandidates = function (appID) {
            var query = listEndPoint + "/GetByTitle('" + Config.ProposedTemporaryWorker + "')/Items?$filter=(HRParentID eq '" + appID + "')&$top=1000&$orderby=Modified asc&$select=*"
                + ",Attachments,AttachmentFiles&$expand=AttachmentFiles"
            return baseService.getRequest(query);
        }
        //getting CRSR UAT Status
        var getUATStatus = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.UATStatus + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }

        //getting CRSR Reason For Request
        var getApplicationStatus = function () {
           
            var query = listEndPoint + "/GetByTitle('" + Config.ApplicationStatus + "')/Items?$top=1000&$orderby=Modified asc&$select=*"
            return baseService.getRequest(query);
        }

        var getApprovers = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.HReMRApprovers + "')/Items?$select=*"
                + ",Approver/Id,Approver/EMail,Approver/Title"
                + "&$expand=Approver/Id"
            var res = baseService.getRequest(query);
           
            return res;
        };
       
        //get comments by id
        var getCommentsById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.HRComments + "')/items?$filter=HRRequestID eq " + listItemId + "&$select=*"
              + ",Author/Id,Author/EMail,Author/Title"
              + "&$expand=Author/Id"
            return baseService.getRequestAll(query);
        };
        //get doc by id
        var getDocLibById = function (listItemId, ListName) {
            // var query = listEndPoint + "/GetByTitle('" + Config.PJDocuments + "')/Items?$select=File&$expand=File&$filter=PJListItemID eq " + listItemId + ""
            var query = "/_api/Web/GetFolderByServerRelativeUrl('" + ListName + "/" + listItemId + "')/Files?$expand=ListItemAllFields";
            return baseService.getRequestAll(query);
        };
        //get doc by foldername
        var getDocLibByFolderName = function (folderName, ListName) {
            var query = "/_api/Web/lists/getByTitle('" + ListName + "')/items?$select=FSObjType,BaseName&$filter=FileLeafRef eq '" + folderName + "'";        
            var deferred = $q.defer();
            $http({
                url: _spPageContextInfo.webAbsoluteUrl + query,
                method: "GET",
                headers: {
                    "accept": "application/json;odata=verbose",
                    "content-Type": "application/json;odata=verbose"
                }
            }).then(function (result) {

                deferred.resolve(result);

            }, function (result, status) {
                deferred.reject(status);
            });
            return deferred.promise;
        };
        //deleteFileIs by Title
        var deleteFilebyDocTitle = function (docTitle,ID) {
            var url = "/_api/web/GetFileByServerRelativeUrl('" + _spPageContextInfo.webServerRelativeUrl + "/" + Config.CRSRRequestComments + "/" + ID + "/" + docTitle + "')";
            return baseService.deleteRequest(url);
        };
        //checkFileIsExist
        var checkFileIsExist = function (fileName) {
            var url = "/_api/Web/lists/getByTitle('" + Config.CRSRRequestComments + "')/Items?$select=FieldValuesAsText/FileRef&$expand=FieldValuesAsText&$filter=FileLeafRef eq '" + fileName + "'";
            return baseService.getRequest(url);
        };
        var getByUserId = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.OOOHistory + "')/items?$filter=Approver/Id eq " + userId + "&$select=*"
                 + ",DelegateTo/Id,DelegateTo/EMail,DelegateTo/Title"
                + ",Approver/Id,Approver/EMail,Approver/Title"
                + "&$expand=DelegateTo/Id,Approver/Id"
            return baseService.getRequestAll(query);
        };
        var getDeptHeadByUserId = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.DepartmentHead + "')/items?$select=*"
                 + ",DepartmentHead/Id,DepartmentHead/EMail,DepartmentHead/Title"
                + ",Backup/Id,Backup/EMail,Backup/Title"
                + "&$expand=DepartmentHead/Id,Backup/Id&$filter=DepartmentHead/Id eq " + userId + ""
            return baseService.getRequestAll(query);
        };
        var getUser = function (id) {
            var url = "/_api/Web/GetUserById(" + id + ")";
            return baseService.getRequestAll(url);
        };
        var getUserGroup = function (id) {
            var url = "/_api/Web/sitegroups/getById(" + id + ")";
            return baseService.getRequestAll(url);
        };
        var getAllUsersFromGroup = function (groupName) {
            var url = "/_api/Web/sitegroups/getbyname('" + groupName + "')/users";
            return baseService.getRequestAll(url);
        };
        return {
            getAll: getAll,
            getAllReq: getAllReq,
            getAllHReMRRequests: getAllHReMRRequests,
            getAllPendingHReMRRequests: getAllPendingHReMRRequests,
            getAllDeptHeadOfCRSRRequests: getAllDeptHeadOfCRSRRequests,
            getPendingCRSRRequests: getPendingCRSRRequests,
            getOngoingCRSRRequests: getOngoingCRSRRequests,
            getClosedCRSRRequests: getClosedCRSRRequests,
            getLastListITem: getLastListITem,
            getById: getById,
            getAllRequestorSubmittedRequests: getAllRequestorSubmittedRequests,
            getApprovers: getApprovers,
            getSearchedHRRequests: getSearchedHRRequests,
           
            getManpowerRequestType: getManpowerRequestType,
            getManpowerType: getManpowerType,
            getDeploymentDuration: getDeploymentDuration,
            getComputerRequirement: getComputerRequirement,
            getHRFundingTypes: getHRFundingTypes,
            getApplicationStatus: getApplicationStatus,
            getAllDivisionsAndBranches: getAllDivisionsAndBranches,
            getInterns: getInterns,
            getTemporaryCandidates: getTemporaryCandidates,
            //pjdocu
            getDocLibByFolderName: getDocLibByFolderName,
            getDocLibById: getDocLibById,
            deleteFilebyDocTitle:deleteFilebyDocTitle,
            //comments
            getCommentsById: getCommentsById,
            getUser: getUser,
            getByUserId: getByUserId,
            getDeptHeadByUserId: getDeptHeadByUserId,
            getUserGroup: getUserGroup,
            getAllUsersFromGroup: getAllUsersFromGroup
        };
    }]);
})();