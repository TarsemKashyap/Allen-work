"use strict";
(function () {
    app.factory("SysAccAdminService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        var getAll = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.PJApproval + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
        var getAllReq = function (Listname) {
            var query = listEndPoint + "/GetByTitle('" + Listname + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
        var getMasterDataWithLookupColumn = function (Listname,ColumnName) {
            var query = listEndPoint + "/GetByTitle('" + Listname + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + "," + ColumnName + "/Id," + ColumnName+"/Title"
                + "&$expand=" + ColumnName+"/Id" //expend for lookup colum
            return baseService.getRequestAll(query);
        };
        // get closed ITRequests data
        var getClosedRequestsWithAppStatus = function (aprvdstatus, RejectedStatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=((ApplicationStatus eq " + aprvdstatus + ") or (ApplicationStatus eq " + RejectedStatus + "))&$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + ",EVEModuleOwner/Id,EVEModuleOwner/EMail,EVEModuleOwner/Title"
                + ",EVEModuleLeader/Id,EVEModuleLeader/EMail,EVEModuleLeader/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id,EVEModuleOwner/Id,EVEModuleLeader/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get ongoing ITRequests data
        var getOngoingRequestsWithAppStatus = function (appstatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=(ApplicationStatus eq " + appstatus + ") &$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + ",EVEModuleOwner/Id,EVEModuleOwner/EMail,EVEModuleOwner/Title"
                + ",EVEModuleLeader/Id,EVEModuleLeader/EMail,EVEModuleLeader/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id,EVEModuleOwner/Id,EVEModuleLeader/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
       // get all SystemAccess data
        var getAllSysRequests = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + ",EVEModuleOwner/Id,EVEModuleOwner/EMail,EVEModuleOwner/Title"
                + ",EVEModuleLeader/Id,EVEModuleLeader/EMail,EVEModuleLeader/Title"              
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id,EVEModuleOwner/Id,EVEModuleLeader/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        // get getAllRequestorITRequests data based on createdby
        var getAllRequestorSysRequests = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=Author/Id eq " + userId + "&$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id" //expend for peoplepciker"
                return baseService.getRequestAll(query);
        };
        // get all SystemAccess data
        var getAllDeptHeadOfSysRequests = function (userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=" + CurrentUserDept + "/Id eq " + userId + "&$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id" //expend for peoplepciker" 
              return baseService.getRequestAll(query);
        };
        // get Pending SystemAccess data
        var getPendingSysRequests = function (CurrentUserStatusFiled,CurrrentUserStatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=(" + CurrentUserStatusFiled+" eq '" + CurrrentUserStatus + "') and (" + CurrrentUserPPLPickerName+"/Id eq " + userId + ")&$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id"  //expend for peoplepciker
           return baseService.getRequestAll(query);
        };
        
        // get ongoing SystemAccess data
        var getOngoingSysRequests = function (appstatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=(ApplicationStatus eq " + appstatus + ") and (" + CurrrentUserPPLPickerName + "/Id eq " + userId +") &$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id"
             return baseService.getRequestAll(query);
        };
        // get closed SystemAccess data
        var getClosedSysRequests = function (aprvdstatus, RejectedStatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccess + "')/Items?$filter=((ApplicationStatus eq " + aprvdstatus + ") or (ApplicationStatus eq " + RejectedStatus + ")) and (" + CurrrentUserPPLPickerName + "/Id eq " + userId + ")&$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id" //expend for peoplepciker
           return baseService.getRequestAll(query);
        };
      
        //get lastitem id for generate uniqu number (PJnumber)
        var getLastListITem = function (ListNanem) {
            var query = listEndPoint + "/GetByTitle('" + ListNanem + "')/Items?$top=1&$select=* &$orderby=Created desc"
            return baseService.getRequestAll(query);
        };
        //get System by id
        var getById = function (ListName, listItemId) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/items?$filter=ID eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"  
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title"
                + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + ",Level1Approver/Id,Level1Approver/EMail,Level1Approver/Title"
                + ",ModuleOwner/Id,ModuleOwner/EMail,ModuleOwner/Title"
                + ",EVEModuleOwner/Id,EVEModuleOwner/EMail,EVEModuleOwner/Title"
                + ",EVEModuleLeader/Id,EVEModuleLeader/EMail,EVEModuleLeader/Title"
                + ",AttachmentFiles,Title" 
                + "&$expand=Author/Id,Editor/Id,RequestedFor/Id,BranchHead/Id,SystemAccessHead/Id,ImplementationOfficer/Id,Level1Approver/Id,ModuleOwner/Id,EVEModuleOwner/Id,EVEModuleLeader/Id,AttachmentFiles" //expend for peoplepciker
         return baseService.getRequestAll(query);
        };
        //getNextApprover
        var getNextLevelApprover = function (ListName, systemacessTtitle) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/items?$filter=SystemAccess eq '" + systemacessTtitle + "'&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + "&$expand=Author/Id,Editor/Id,SystemAccessHead/Id,ImplementationOfficer/Id" //expend for peoplepciker         
            return baseService.getRequest(query);
        };
        //getNextApprover
        var getEveModuleApprover = function (ListName, systemacessTtitle) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/items?$filter=Title eq '" + systemacessTtitle + "'&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",EVEModuleOwner/Id,EVEModuleOwner/EMail,EVEModuleOwner/Title"
                + ",EVEModuleLeader/Id,EVEModuleLeader/EMail,EVEModuleLeader/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + "&$expand=Author/Id,Editor/Id,EVEModuleOwner/Id,EVEModuleLeader/Id,ImplementationOfficer/Id" //expend for peoplepciker         
            return baseService.getRequest(query);
        };
        //get System access by id
        var getSubListByParentId = function (ListName, listItemId) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/items?$filter=SystemAccessId eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + "&$expand=Author/Id,Editor/Id" //expend for peoplepciker
            return baseService.getRequestAll(query);
        };
        //masterlist
        var getAllMasterListData = function (ListName) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
         //masterlist
        var getAllRequestTypeMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.TypeOfRequest + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAllRequestTypeByWorkAreaID = function (WorkAreaCode) {
            var query = listEndPoint + "/GetByTitle('" + Config.TypeOfRequest + "')/Items?$filter=WorkAreaCode eq '" + WorkAreaCode + "'&$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAuthorization = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccessApprovers + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",SystemAccessHead/Id,SystemAccessHead/EMail,SystemAccessHead/Title"
                + ",ImplementationOfficer/Id,ImplementationOfficer/EMail,ImplementationOfficer/Title"
                + "&$expand=Author/Id,Editor/Id,SystemAccessHead/Id,ImplementationOfficer/Id" //expend for peoplepciker    
            return baseService.getRequest(query);
        };

        //get comments by id
        var getCommentsById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.SystemAccessComments + "')/items?$filter=ITRequestsID eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",CommentsBy/Id,CommentsBy/EMail,CommentsBy/Title"
                + "&$expand=Author/Id,CommentsBy/Id"
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
            var url = "/_api/web/GetFileByServerRelativeUrl('" + _spPageContextInfo.webServerRelativeUrl + "/" + Config.ITRequestComments + "/" + ID + "/" + docTitle + "')";
            return baseService.deleteRequest(url);
        };
        //checkFileIsExist
        var checkFileIsExist = function (fileName) {
            var url = "/_api/Web/lists/getByTitle('" + Config.ITRequestComments + "')/Items?$select=FieldValuesAsText/FileRef&$expand=FieldValuesAsText&$filter=FileLeafRef eq '" + fileName + "'";
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
            getSubListByParentId: getSubListByParentId,
            getOngoingRequestsWithAppStatus: getOngoingRequestsWithAppStatus,
            getClosedRequestsWithAppStatus: getClosedRequestsWithAppStatus,
            getAllSysRequests: getAllSysRequests,
            getAllDeptHeadOfSysRequests: getAllDeptHeadOfSysRequests,
            getPendingSysRequests: getPendingSysRequests,
            getOngoingSysRequests: getOngoingSysRequests,
            getClosedSysRequests: getClosedSysRequests,
            getLastListITem: getLastListITem,
            getEveModuleApprover: getEveModuleApprover,
            getById: getById,
            getNextLevelApprover: getNextLevelApprover,
            getAllRequestorSysRequests: getAllRequestorSysRequests,   
            getMasterDataWithLookupColumn: getMasterDataWithLookupColumn,
            //master   
            getAllMasterListData: getAllMasterListData,
            getAllRequestTypeMaster: getAllRequestTypeMaster,
            getAuthorization: getAuthorization,
            getAllRequestTypeByWorkAreaID: getAllRequestTypeByWorkAreaID,
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