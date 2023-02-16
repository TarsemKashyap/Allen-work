"use strict";
(function () {
    app.factory("ExportExcelService", ["baseSvc", "Config", "$http", "$q", function (baseService, Config, $http, $q) {
        var listEndPoint = '/_api/web/lists';
        var getAll = function (ListName) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/Items";
            switch (ListName) {
                case "ITComments":
                    query = listEndPoint + "/GetByTitle('" + ListName + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                        + ",Author/Id,Author/EMail,Author/Title"
                        + ",Editor/Id,Editor/EMail,Editor/Title"
                        + ",CommentsBy/Id,CommentsBy/EMail,CommentsBy/Title"
                        + "&$expand=Author/Id,Editor/Id,CommentsBy/Id"
                    break;              
                case "SystemAccessComments":
                    query = listEndPoint + "/GetByTitle('" + ListName + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
                        + ",Author/Id,Author/EMail,Author/Title"
                        + ",Editor/Id,Editor/EMail,Editor/Title"
                        + ",CommentsBy/Id,CommentsBy/EMail,CommentsBy/Title"
                        + "&$expand=Author/Id,Editor/Id,CommentsBy/Id"
                    break;  
                case "ITRequests":
                    query = listEndPoint + "/GetByTitle('" + ListName + "')/Items?$top=1000&$orderby=Modified desc&$select=*"
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

                    break; 
                default:
                // code block
            }
            return baseService.getRequestAll(query);
        };
        var getAllFieldsFromList = function (ListName) {
            var query = listEndPoint + "/GetByTitle('" + ListName + "')/fields?$select=Title,InternalName&$filter=Hidden eq false";// and ReadOnlyField eq false
            return baseService.getRequestAll(query);
        }; 

        return {
            getAll: getAll,
            getAllFieldsFromList: getAllFieldsFromList
        };
    }]);
})();