"use strict";
(function () {
    app.controller("loginUserController", ["$scope", "loginUserService", "$location", "$timeout", "$filter", function ($scope, loginUserService, $location, $timeout, $filter) {

        var today = new Date();
        var dateAsString = $filter('date')(today, "yyyy-MM-dd");
        $scope.initLogin = function () {
            loginUserService.getCurrentUser().then(function (result) {
                //success function  
                if (result.data.d.LoginName.indexOf('|') > -1) {
                    $scope.userLogProperties = {
                        Title: result.data.d.Title,
                        LoginName: result.data.d.LoginName.split('|')[1], // getting login name with domain
                        LoginDate: today,
                        LoginId: result.data.d.Id,
                        ADID: result.data.d.LoginName.split('|')[1].split('\\')[1], // getting login name without domain
                        Email: result.data.d.Email, // getting displayname(fullName)  
                        DateOnly: dateAsString,
                    };
                    loginUserService.checkUserAlreadyExist($scope.userLogProperties).then(function (resultsLogin) {
                        if (resultsLogin.data.d.results.length != 0) {
                            //update
                            updateUserLoginTime(resultsLogin.data.d.results[0].Id); //insert
                        } else {
                            //insert
                            saveUserLoginTime();
                        }
                    });
                }
            });
        } //init function end

        //save login user
        function saveUserLoginTime() {

            loginUserService.addNew($scope.userLogProperties).then(function (result) {
                //success
            });
        }
        //update login user
        function updateUserLoginTime(listId) {

            loginUserService.update($scope.userLogProperties, listId).then(function (result) {
                //success
            });
        }


    }]);
})();