﻿"use strict";
(function () {
    app.factory("ITService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        var getAll = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.PJApproval + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
        var getAllReq = function (Listname) {
            var query = listEndPoint + "/GetByTitle('" + Listname + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
       // get all ITRequests data
        var getAllITRequests = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$top=100&$orderby=Modified desc&$select=*"
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
        // get getAllRequestorITRequests data based on createdby
        var getAllRequestorITRequests = function (userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=Author/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + ",Editor/Id,Editor/EMail,Editor/Title"
                + ",RequestedFor/Id,RequestedFor/EMail,RequestedFor/Title" 
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
        // get all ITRequests data
        var getAllDeptHeadOfITRequests = function (userId, CurrentUserDept) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=" + CurrentUserDept + "/Id eq " + userId + "&$top=100&$orderby=Modified desc&$select=*"
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
        // get Pending ITRequests data
        var getPendingITRequests = function (CurrentUserStatusFiled,CurrrentUserStatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=(" + CurrentUserStatusFiled+" eq '" + CurrrentUserStatus + "') and (" + CurrrentUserPPLPickerName+"/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
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
        
        // get ongoing ITRequests data
        var getOngoingITRequests = function (appstatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=(ApplicationStatus eq " + appstatus + ") and (" + CurrrentUserPPLPickerName + "/Id eq " + userId +") &$top=100&$orderby=Modified desc&$select=*"
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
        // get closed ITRequests data
        var getClosedITRequests = function (aprvdstatus, RejectedStatus, CurrrentUserPPLPickerName, userId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=((ApplicationStatus eq " + aprvdstatus + ") or (ApplicationStatus eq " + RejectedStatus + ")) and (" + CurrrentUserPPLPickerName + "/Id eq " + userId + ")&$top=100&$orderby=Modified desc&$select=*"
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
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$top=1&$select=* &$orderby=Created desc"
            return baseService.getRequestAll(query);
        };
        //get ITRequests by id
        var getById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/items?$filter=ID eq " + listItemId + "&$select=*"
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
      
        //masterlist
        var getAllWorkAreaMaster = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.WorkArea + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
         //masterlist
        var getAllRequestTypeMaster = function (Listname) {
            var query = listEndPoint + "/GetByTitle('" + Listname + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAllRequestTypeByWorkAreaID = function (WorkAreaCode) {
            var query = listEndPoint + "/GetByTitle('" + Config.TypeOfRequest + "')/Items?$filter=WorkAreaCode eq '" + WorkAreaCode + "'&$top=1000&$orderby=Modified desc&$select=*"
            return baseService.getRequest(query);
        };
        //masterlist
        var getAuthorization = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequestApprovers + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",Approvers/Id,Approvers/EMail,Approvers/Title"
                + "&$expand=Approvers/Id"
            return baseService.getRequest(query);
        };

        //get comments by id
        var getCommentsById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITComments + "')/items?$filter=ITListItemID eq " + listItemId + "&$select=*"
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
            getAllITRequests: getAllITRequests,
            getAllDeptHeadOfITRequests: getAllDeptHeadOfITRequests,
            getPendingITRequests: getPendingITRequests,
            getOngoingITRequests: getOngoingITRequests,
            getClosedITRequests: getClosedITRequests,
            getLastListITem: getLastListITem,
            getById: getById,
            getAllRequestorITRequests: getAllRequestorITRequests,          
            //master           
            getAllWorkAreaMaster: getAllWorkAreaMaster,
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