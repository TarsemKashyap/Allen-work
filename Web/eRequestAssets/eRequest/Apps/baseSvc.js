"use strict";
(function () {
    app.factory("baseSvc", ["$http", "$q", function ($http, $q) {
            var baseUrl = _spPageContextInfo.webAbsoluteUrl;//siteAbsoluteUrl
           
            // The first time through, these three variables won't exist, so we create them. On subsequent calls, the variables will already exist.         
            var results = results || [];          
            results.data = results.data || [];          
            var getRequestAll = function (queryUrl) {
                var deferred = deferred || $q.defer();
                $http({
                    url: baseUrl+queryUrl,
                    method: "GET",
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "content-Type": "application/json;odata=verbose"
                    }
                }).then(function (response) {
                    // The first time through, we don't have any data, so we create the data object with the results
                    if (!results.data.d) {
                        results.data = response.data;
                    } else {
                        // If we already have some results from a previous call, we concatenate this set onto the existing array
                        results.data.d.results = results.data.d.results.concat(response.data.d.results);
                    }
                    // If there's more data to fetch, there will be a URL in the __next object; if there isn't, the __next object will not be present
                    if (response.data.d.__next) {
                        // When we have a next page, we call this function again (recursively).
                        // We change the url to the value of __next and pass in the current results and the deferred object we created on the first pass through
                        queryUrl = response.data.d.__next;
                        baseUrl = ""; //when recursive clear root url
                        getRequestAll(queryUrl);
                    }
                    // If there is no value for __next, we're all done because we have all the data already, so we resolve the promise with the results.
                    deferred.resolve(response);

                }, function (response, status) {
                    deferred.reject(status);
                });
                return deferred.promise;
            };
          
            var getRequest = function (query) {
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
            var getRequestImage = function (query) {
                var deferred = $q.defer();
                $http({
                    url: baseUrl + query,
                    method: "GET",
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "content-Type": "application/json;odata=verbose"
                    },
                   // headers: { 'Content-Type': 'image/png', 'X-Requested-With': 'XMLHttpRequest' },
                    dataType: 'binary',
                    processData: false
                }).then(function (result) {

                    deferred.resolve(result);

                }, function (result, status) {
                    deferred.reject(status);
                });
                return deferred.promise;
            };
            var getRequestStaffInfo = function (query) {
                var deferred = $q.defer();
                $http({
                    url: query,
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
            var postRequest = function (data, url) {
                var deferred = $q.defer();
                $http({
                    url: baseUrl + url,
                    method: "POST",
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "X-RequestDigest": document.getElementById("__REQUESTDIGEST").value,
                        "content-Type": "application/json;odata=verbose"
                    },
                    data: JSON.stringify(data)
                })
                    .then(function (result) {
                        deferred.resolve(result);
                    })
                    ,(function (result, status) {
                        deferred.reject(status);
                    });
                return deferred.promise;
            };
            var updateRequest = function (data, url) {
                var deferred = $q.defer();
                $http({
                    url: url,
                    method: "POST",
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "X-RequestDigest": document.getElementById("__REQUESTDIGEST").value,
                        "content-Type": "application/json;odata=verbose",
                        "X-Http-Method": "MERGE",
                        "If-Match": "*"
                    },
                    data: JSON.stringify(data)
                })
                    .then(function (result) {
                        deferred.resolve(result);
                    })
                    ,(function (result, status) {
                        deferred.reject(status);
                    });
                return deferred.promise;
            };
            var deleteRequest = function (url) {
                var deferred = $q.defer();
                $http({
                    url: baseUrl + url,
                    method: "DELETE",
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "X-RequestDigest": document.getElementById("__REQUESTDIGEST").value,
                        "IF-MATCH": "*"
                    }
                }).then(function (result) {

                    deferred.resolve(result);

                }, function (result, status) {
                    deferred.reject(status);
                });
                return deferred.promise;
            };
            var IsMemberOfGroup = function (query) {
                var deferred = $q.defer();
                $http({
                    url: baseUrl + query,
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
            return {
                getRequest: getRequest,
                IsMemberOfGroup: IsMemberOfGroup,
                getRequestImage:getRequestImage,
                getRequestStaffInfo:getRequestStaffInfo,
                getRequestAll: getRequestAll,
                postRequest: postRequest,
                updateRequest: updateRequest,
                deleteRequest: deleteRequest,               
            };
        }]);
})();