"use strict";
(function () {
    app.factory("loginUserService", ["baseSvc", "SharePointList", function (baseService, SharePointList) {
            var listEndPoint = '/_api/web/lists';
            var webEndPoint = '/_api/web';
            var baseSiteUrl = _spPageContextInfo.siteAbsoluteUrl;
            var getAll = function () {
               
                var query = listEndPoint + "/GetByTitle('" + SharePointList.UserLog +"')/Items";
                return baseService.getRequestAll(query);
            };
            var getUserByDate = function (item) {               
                var query = listEndPoint + "/GetByTitle('" + SharePointList.UserLog +"')/Items?$filter=(LoginId eq " + item.LoginId + " and LastLoginDate ge '" + item.DateOnly+"')"; //2018-05-21T11:50:50Z
                return baseService.getRequestAll(query);
            };
            var checkUserAlreadyExist = function (item) {
                var query = listEndPoint + "/GetByTitle('" + SharePointList.UserLog +"')/Items?$filter=(LoginId eq " + item.LoginId + ")"; 
                return baseService.getRequestAll(query);
            };
            var addNew = function (item) {
                //datatype must be match here
                var data = {
                    __metadata: { 'type': 'SP.Data.' + SharePointList.UserLog+'ListItem' },
                    Title: item.Title,
                    LoginName: item.LoginName,                    
                    LastLoginDate: item.LoginDate.toISOString(),
                    LoginId: item.LoginId,
                    ADID: item.ADID,
                    Email: item.Email
                };
                var url = listEndPoint + "/GetByTitle('" + SharePointList.UserLog +"')/Items";
                return baseService.postRequest(data, url);
            };
            var update = function (item, listId) {
                var data = {
                    __metadata: { 'type': 'SP.Data.' + SharePointList.UserLog +'ListItem' },
                    Title:item.Title,
                    LoginName: item.LoginName,
                    LastLoginDate: item.LoginDate.toISOString(),
                    LoginId: item.LoginId,
                    ADID: item.ADID,
                    Email:item.Email,
                };
                var url = baseSiteUrl + listEndPoint + "/GetByTitle('" + SharePointList.UserLog+"')/Items(" + listId + ")";
                return baseService.updateRequest(data, url);
            };
            var getCurrentUser = function () {
                var query = webEndPoint + "/currentuser";
                return baseService.getRequest(query);
            };
                  
            return {
                getAll: getAll,
                addNew: addNew,
                getUserByDate: getUserByDate,
                getCurrentUser: getCurrentUser,  
                update: update,
                checkUserAlreadyExist: checkUserAlreadyExist
            };
        }]);
})();