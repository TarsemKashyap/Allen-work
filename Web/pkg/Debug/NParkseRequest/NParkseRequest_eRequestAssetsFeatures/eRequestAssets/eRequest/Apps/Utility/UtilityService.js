"use strict";
(function () {
    app.factory("UtilityService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        var baseSiteUrl = window.location.protocol + "//" + window.location.hostname;//_spPageContextInfo.siteAbsoluteUrl;
        var getAll = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
       // get all UtilityRequests data
        var getAllUtilityRequests = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get getAllRequestorUtilityRequests data based on createdby
        var getAllRequestorUtilityRequests = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=Author/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get all UtilityRequests data
        var getAllDeptHeadOfUtilityRequests = function (userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=" + CurrentUserDept + "/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get Pending UtilityRequests data
        var getPendingUtilityRequests = function (status, userId,CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=(ApplicationStatus eq " + status + ") and (" + CurrentUserDept+"/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get ongoing UtilityRequests data
        var getOngoingUtilityRequests = function (status, userId, CurrentUserDept, ApproverStatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=(ApplicationStatus eq '2' or ApplicationStatus eq '1')&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get closed UtilityRequests data
        var getClosedUtilityRequests = function (status, userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=(ApplicationStatus eq " + status + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
      
        //get lastitem id for generate uniqu number (PJnumber)
        var getLastListITem = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$top=1&$select=* &$orderby=Created desc"
            return baseService.getRequestAll(query);
        };
        //get UtilityRequests by id
        var getById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/items?$filter=ID eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        //masterlist
        var getAllRequestTypeMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.PurposeMaster + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterList
        var getAllVendorMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.VendorMaster + "')/Items?$top=1000&$orderby=Title asc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAllPurposeMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.PurposeMaster + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAllCostCentreMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.CostCentreMaster + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAuthorization = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.DivisionHeads + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",DivisionHead/Id,DivisionHead/EMail,DivisionHead/Title"
                + "&$expand=DivisionHead/Id"
            return baseService.getRequest(query);
        };
       
        
        //get comments by id
        var getCommentsById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityComments + "')/items?$filter=UtilityRequestsID eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",CommentsBy/Id,CommentsBy/EMail,CommentsBy/Title"
                + "&$expand=Author/Id,CommentsBy/Id"
            return baseService.getRequestAll(query);
        };
        //get doc by id
        var getDocLibById = function (listItemId) {
            // var query = listEndPoint + "/GetByTitle('" + Config.PJDocuments + "')/Items?$select=File&$expand=File&$filter=PJListItemID eq " + listItemId + ""
            var query = "/_api/Web/GetFolderByServerRelativeUrl('" + Config.UtilityDocuments + "/" + listItemId + "')/Files?$expand=ListItemAllFields";
            return baseService.getRequestAll(query);
        };
        //get doc by foldername
        var getDocLibByFolderName = function (folderName) {
            var query = "/_api/Web/lists/getByTitle('" + Config.UtilityDocuments + "')/items?$select=FSObjType,BaseName&$filter=FileLeafRef eq '" + folderName + "'";        
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
            var url = "/_api/web/GetFileByServerRelativeUrl('" + _spPageContextInfo.webServerRelativeUrl + "/" + Config.PJDocuments + "/" + ID + "/" + docTitle + "')";
            return baseService.deleteRequest(url);
        };
        //checkFileIsExist
        var checkFileIsExist = function (fileName) {
            var url = "/_api/Web/lists/getByTitle('" + Config.UtilityDocuments + "')/Items?$select=FieldValuesAsText/FileRef&$expand=FieldValuesAsText&$filter=FileLeafRef eq '" + fileName + "'";
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
            getAllUtilityRequests: getAllUtilityRequests,
            getAllDeptHeadOfUtilityRequests: getAllDeptHeadOfUtilityRequests,
            getPendingUtilityRequests: getPendingUtilityRequests,
            getClosedUtilityRequests: getClosedUtilityRequests,
            getLastListITem: getLastListITem,
            getById: getById,
            getAllRequestorUtilityRequests: getAllRequestorUtilityRequests,
            getOngoingUtilityRequests: getOngoingUtilityRequests,
            getAllRequestTypeMaster: getAllRequestTypeMaster,

            //master
            getAllCostCentreMaster: getAllCostCentreMaster,
            getAllPurposeMaster: getAllPurposeMaster,
            getAuthorization: getAuthorization,
            getAllVendorMaster: getAllVendorMaster,
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