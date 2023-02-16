"use strict";
(function () {
    app.factory("UtilityAdminDashboardService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
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
        // get UtilityRequests data based on createdby
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
        // get all CRSRRequests data
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
        // get Pending CRSRequests data
        var getPendingUtilityRequests = function (status, userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=(ApplicationStatus eq " + status + ") and (" + CurrentUserDept + "/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get ongoing CRSRRequests data
        var getOngoingUtilityRequests = function (appstatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=(ApplicationStatus eq " + appstatus + ")&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",FinanceHead/Id,FinanceHead/EMail,FinanceHead/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,FinanceHead/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get closed CRSRRequests data
        var getClosedUtilityRequests = function (aprvdstatus, RejectedStatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityRequests + "')/Items?$filter=((ApplicationStatus eq " + aprvdstatus + ") or (ApplicationStatus eq " + RejectedStatus + "))&$top=100&$orderby=Modified desc&$select=*"
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
            var query = listEndPoint + "/GetByTitle('" + Config.CRSRProjectSystem + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
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
            var query = listEndPoint + "/GetByTitle('" + Config.UtilityComments + "')/items?$filter=CRSRListItemID eq " + listItemId + "&$select=*"
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
        var deleteFilebyDocTitle = function (docTitle, ID) {
            var url = "/_api/web/GetFileByServerRelativeUrl('" + _spPageContextInfo.webServerRelativeUrl + "/" + Config.CRSRComments + "/" + ID + "/" + docTitle + "')";
            return baseService.deleteRequest(url);
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
            getAllRequestorUtilityRequests: getAllRequestorUtilityRequests,
            getOngoingUtilityRequests: getOngoingUtilityRequests,

            //master           
            getAllFundCenterMaster: getAllFundCenterMaster,
            getAllCostCenterMaster: getAllCostCenterMaster,
            getAllProjectSystemMaster: getAllProjectSystemMaster,
            getAuthorization: getAuthorization,

            //pjdocu
            getDocLibByFolderName: getDocLibByFolderName,
            getDocLibById: getDocLibById,
            deleteFilebyDocTitle: deleteFilebyDocTitle,
            //comments
            getCommentsById: getCommentsById,
            getUser: getUser,          
            getDeptHeadByUserId: getDeptHeadByUserId,
            getUserGroup: getUserGroup,
            getAllUsersFromGroup: getAllUsersFromGroup
        };
    }]);
})();