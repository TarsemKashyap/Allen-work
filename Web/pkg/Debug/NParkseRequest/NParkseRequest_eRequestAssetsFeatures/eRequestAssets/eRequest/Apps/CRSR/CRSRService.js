"use strict";
(function () {
    app.factory("CRSRService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        var getAll = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
       // get all CRSRRequests data
        var getAllCRSRRequests = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get getAllRequestorCRSRRequests data based on createdby
        var getAllRequestorCRSRRequests = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$filter=Author/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
                return baseService.getRequestAll(query);
        };
        // get all CRSRRequests data
        var getAllDeptHeadOfCRSRRequests = function (userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$filter=" + CurrentUserDept + "/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
              return baseService.getRequestAll(query);
        };
        // get Pending CRSRequests data
        var getPendingCRSRRequests = function (status, userId,CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$filter=(ApplicationStatus eq " + status + ") and (" + CurrentUserDept+"/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
           return baseService.getRequestAll(query);
        };
        // get ongoing CRSRRequests data
        var getOngoingCRSRRequests = function (CurrentUserDept,ApproverStatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$filter=(" + ApproverStatus + " eq 'Pending')&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
             return baseService.getRequestAll(query);
        };
        // get closed CRSRRequests data
        var getClosedCRSRRequests = function (status) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$filter=(ApplicationStatus eq " + status + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
           return baseService.getRequestAll(query);
        };
      
        //get lastitem id for generate uniqu number (PJnumber)
        var getLastListITem = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/Items?$top=1&$select=* &$orderby=Created desc"
            return baseService.getRequestAll(query);
        };
        //get CRSRRequests by id
        var getById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSR + "')/items?$filter=ID eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",ITProjectManager/Id,ITProjectManager/EMail,ITProjectManager/Title"
                + ",FundingAuthority/Id,FundingAuthority/EMail,FundingAuthority/Title"
                + ",SystemOwner/Id,SystemOwner/EMail,SystemOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,ITProjectManager/Id,FundingAuthority/Id,SystemOwner/Id" //expend for peoplepciker
         return baseService.getRequestAll(query);
        };
      
        //masterlist
        var getAllCostCenterMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSRCostCentre + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAllFundCenterMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSRFundCentre + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
         //masterlist
        
        var getAllProjectSystemMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccessMaster + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };

        //masterlist
        var getAuthorization = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSRApprovers + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",Approvers/Id,Approvers/EMail,Approvers/Title"
                + "&$expand=Approvers/Id"
            return baseService.getRequest(query);
        };


        //masterlist
        var getApproversFromList = function (ListName) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",CRSRITProjectManager/Id,CRSRITProjectManager/EMail,CRSRITProjectManager/Title"
                + ",CRSRSystemOwner/Id,CRSRSystemOwner/EMail,CRSRSystemOwner/Title"
                + "&$expand=CRSRITProjectManager/Id,CRSRSystemOwner/Id"
            return baseService.getRequest(query);
        };

        //get comments by id
        var getCommentsById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.CRSRComments + "')/items?$filter=CRSRListItemID eq " + listItemId + "&$select=*"
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
            var url = "/_api/web/GetFileByServerRelativeUrl('" + _spPageContextInfo.webServerRelativeUrl + "/" + Config.CRSRComments + "/" + ID + "/" + docTitle + "')";
            return baseService.deleteRequest(url);
        };
        //checkFileIsExist
        var checkFileIsExist = function (fileName) {
            var url = "/_api/Web/lists/getByTitle('" + Config.CRSRComments + "')/Items?$select=FieldValuesAsText/FileRef&$expand=FieldValuesAsText&$filter=FileLeafRef eq '" + fileName + "'";
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
            getAllCRSRRequests: getAllCRSRRequests,
            getAllDeptHeadOfCRSRRequests: getAllDeptHeadOfCRSRRequests,
            getPendingCRSRRequests: getPendingCRSRRequests,
            getClosedCRSRRequests: getClosedCRSRRequests,
            getLastListITem: getLastListITem,
            getById: getById,
            getAllRequestorCRSRRequests: getAllRequestorCRSRRequests,
            getOngoingCRSRRequests: getOngoingCRSRRequests,
            
            //master           
            getAllFundCenterMaster: getAllFundCenterMaster,
            getAllCostCenterMaster: getAllCostCenterMaster,
            getAllProjectSystemMaster: getAllProjectSystemMaster,
            getAuthorization: getAuthorization,
            getApproversFromList: getApproversFromList,
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