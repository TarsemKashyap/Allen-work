"use strict";
(function () {
    app.factory("ITAdminDashboardService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        // get all ITRequests data
        var getAllITRequests = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=((ApplicationStatus eq '1') or (ApplicationStatus eq '2'))&$top=100&$orderby=Modified desc&$select=*"
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
        var getAll = function (Listname) {
            var query = listEndPoint + "/GetByTitle('" + Listname + "')/Items?$top=1000";
            return baseService.getRequestAll(query);
        };
        // get closed ITRequests data
        var getClosedITRequestsWithAppStatus = function (aprvdstatus, RejectedStatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=((ApplicationStatus eq " + aprvdstatus + ") or (ApplicationStatus eq " + RejectedStatus + "))&$top=100&$orderby=Modified desc&$select=*"
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
        var getOngoingITRequestsWithAppStatus = function (appstatus) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequests + "')/Items?$filter=(ApplicationStatus eq " + appstatus + ") &$top=100&$orderby=Modified desc&$select=*"
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
        var getAuthorization = function () {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequestApprovers + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                + ",Approvers/Id,Approvers/EMail,Approvers/Title"
                + "&$expand=Approvers/Id"
            return baseService.getRequest(query);
        };

        //get comments by id
        var getCommentsById = function (listItemId) {
            var query = listEndPoint + "/GetByTitle('" + Config.ITRequestComments + "')/items?$filter=ITRequestsID eq " + listItemId + "&$select=*"
                + ",Author/Id,Author/EMail,Author/Title"
                + "&$expand=Author/Id"
            return baseService.getRequestAll(query);
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
           
            getAllITRequests: getAllITRequests,
            getLastListITem: getLastListITem,
            getById: getById,
            getOngoingITRequestsWithAppStatus: getOngoingITRequestsWithAppStatus,
            getClosedITRequestsWithAppStatus: getClosedITRequestsWithAppStatus,
            getAll: getAll,
            //master 
            getAuthorization: getAuthorization,
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