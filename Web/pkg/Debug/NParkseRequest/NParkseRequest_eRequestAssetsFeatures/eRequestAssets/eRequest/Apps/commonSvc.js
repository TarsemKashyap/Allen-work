"use strict";
(function () {
    app.factory("CommonService", ["baseSvc", "Config", "$q", function (baseService, Config, $q) {
        var listEndPoint = '/_api/web/lists';
        var webEndPoint = '/_api/web/';
       // var baseSiteUrl = window.location.protocol + "//" + window.location.hostname;//_spPageContextInfo.siteAbsoluteUrl;
        var baseSiteUrl = window.location.origin;
        var getCurrentUser = function () {
            var query = webEndPoint + "/currentuser";
            return baseService.getRequest(query);
        };
        var getPropertiesFor = function (LoginNameWithDomain) {
            var query = "/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(accountName=@v)?@v='" + LoginNameWithDomain + "'";
            return baseService.getRequest(query);
        };
        var getMyProperties = function () {
            var query = "/_api/SP.UserProfiles.PeopleManager/GetMyProperties";
            return baseService.getRequest(query);
        };
        var getUserbyId = function (Id) {
            var query = webEndPoint + "/GetUserById(" + Id + ")";
            return baseService.getRequest(query);
        };
        //from staffdirec
        var getMailConfigFromRootSite = function (ListName) {
            var query = baseSiteUrl + listEndPoint + "/GetByTitle('" + ListName + "')/Items?$top=100";
            return baseService.getRequestStaffInfo(query);
        };
        //from staffdirec
        var getStaffInfoByAdId = function (AdId) {
            var query = baseSiteUrl + listEndPoint + "/GetByTitle('" + Config.StaffDirectory + "')/Items?$filter=ADID eq '" + AdId + "'&$select=ID,KnownAs,Designation,ADID,Branch,Division,Location,Organisation,Cluster,Section,Unit,FirstName,LastName,FullName,Email";
            return baseService.getRequestStaffInfo(query);
        };
        //from erequest site branchHeadDiv list
        var getBranchHeadFromStaffDir = function (branch, designation) {
            var query = baseSiteUrl + listEndPoint + "/GetByTitle('" + Config.StaffDirectory + "')/Items?$filter=(Branch eq '" + encodeURIComponent(branch) + "') and (Designation eq '" + encodeURIComponent(designation) + "')&$select=ID,KnownAs,Designation,ADID,Branch,Division,Location,Organisation,Cluster,Section,Unit,FirstName,LastName,FullName,Email";
            return baseService.getRequestStaffInfo(query);
        };
        //from erequest site branchHeadDiv list
        var getStaffBranchHead = function (LisName,Field,Branch) {
            var query = listEndPoint + "/GetByTitle('" + LisName + "')/Items?$filter=" + Field+" eq '" + encodeURIComponent(Branch) + "'&$select=*"
            + ",BranchHead/Id,BranchHead/EMail,BranchHead/Title"           
            + "&$expand=BranchHead/Id"
            return baseService.getRequest(query);
        };
        //from erequest site branchHeadDiv list
        var getcoveringOfficerHead = function (LisName, Field, adName) {
            var query = baseSiteUrl + listEndPoint  + "/GetByTitle('" + LisName + "')/Items?$filter=" + Field + " eq '" + adName + "'&$select=*"             
            return baseService.getRequestStaffInfo(query);
        };
        //Finding Division Head
        var getStaffDivisionHead = function (divisionName) {
            var query = listEndPoint + "/GetByTitle('" + Config.DivisionHeads + "')/Items?$filter=Division eq '" + encodeURIComponent(divisionName) + "'&$select=*"
                + ",DivisionHead/Id,DivisionHead/EMail,DivisionHead/Title"
                + "&$expand=DivisionHead/Id"
            return baseService.getRequest(query);
        };
        //from erequest site branchHeadDiv list
        var getStaffDivisionBranchHead = function (LisName, Field, Branch) {
            var query = listEndPoint + "/GetByTitle('" + LisName + "')/Items?$filter=" + Field + " eq '" + encodeURIComponent(Branch) + "'&$select=*"             
                + ",DivisionHead/Id,DivisionHead/EMail,DivisionHead/Title"
                + "&$expand=DivisionHead/Id"
            return baseService.getRequest(query);
        };
        //from erequest site branchHeadDiv list
        var getStaffClusterhHead = function (LisName, Field, Branch) {
            var query = listEndPoint + "/GetByTitle('" + LisName + "')/Items?$filter=" + Field + " eq '" + encodeURIComponent(Branch) + "'&$select=*"
                + ",ClusterHead/Id,ClusterHead/EMail,ClusterHead/Title"
                + "&$expand=ClusterHead/Id"
            return baseService.getRequest(query);
        };
        //from erequest site branchHeadDiv list
        var getUserDeptHeadList = function (LisName, Field) {
            var query = listEndPoint + "/GetByTitle('" + LisName + "')/Items?$select=*"
                + "," + Field + "/Id," + Field + "/EMail," + Field+"/Title"             
                + "&$expand=" + Field+"/Id"
            return baseService.getRequest(query);
        };
        //_spPageContextInfo.webAbsoluteUrl + "/_api/web/sitegroups/getByName('"+ groupName +"')/Users?$filter=Id eq " + _spPageContextInfo.userId
        //working in sharepoint inbuilt group
        var chkIsMemberOfGroup = function (groupname, AdLoginId) {
            var query = webEndPoint + "/sitegroups/getByName('" + groupname + "')/Users?$filter=Id eq " +AdLoginId
            return baseService.getRequest(query);
        };
        var getCurrentUserWithDetails = function () {
            var query = '/_api/web/currentuser/?$expand=groups';
            return baseService.getRequest(query);
        };
        var GetUserIdFromLoginName = function (LoginNameWithDomain, Email, FullName) {
            /// change this prefix according to the environment. 
            /// In below sample, windows authentication is considered.
            var prefix = "i:0#.w|";
            /// get the site url
            var siteUrl = _spPageContextInfo.siteAbsoluteUrl;
            /// add prefix, this needs to be changed based on scenario
            var accountName = prefix + LoginNameWithDomain;
            var deferred = $q.defer();
            /// make an ajax call to get the site user
            $.ajax({
                url: siteUrl + "/_api/web/siteusers(@v)?@v='" +
                    encodeURIComponent(accountName) + "'",
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                success: function (data) {
                    ///popup user id received from site users.                                    
                    var prop = [{
                        Id: data.d.Id, EMail: Email, Title: FullName
                    }];
                    deferred.resolve(prop);
                    return; //here is not isAccount return, but anonymous inner function 'function (response)', you got it?
                },
                error: function (data) {
                    console.log(JSON.stringify(data));
                    alert("Can't find Branch head :" + JSON.stringify(data));
                }

            });
            return deferred.promise; //return as soon as creates the promise
        }
        //using folder
        function getFormDigest() {
            return $.ajax({
                url: _spPageContextInfo.webAbsoluteUrl + "/_api/contextinfo",
                method: "POST",
                headers: { "Accept": "application/json; odata=verbose" }
            });
        }
        var createfolder = function (folderName) {
            var documentLibraryName = "NewsHits";
            var requestUri = _spPageContextInfo.webAbsoluteUrl + "/_api/web/folders";
            return getFormDigest().then(function (data) {
                return $.ajax({
                    url: requestUri,
                    type: "POST",
                    contentType: "application/json;odata=verbose",
                    data: JSON.stringify({ '__metadata': { 'type': 'SP.Folder' }, 'ServerRelativeUrl': documentLibraryName + '/' + folderName }),
                    headers: {
                        "accept": "application/json;odata=verbose",
                        // "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                        "X-RequestDigest": data.d.GetContextWebInformation.FormDigestValue
                    }
                });
            });
        }
        var searchedHRAdmnin = function (valLists, toSearch) {
            return _.filter(valLists,
                function (i) {
                    /* Search Text in all 3 fields */
                    return searchHRadmin(i, toSearch);
                });
            function searchHRadmin(item, toSearch) {
                /* Search Text in all 3 fields */
                return (

                    item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Requestor.Title.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||                  
                    item.ManpowerRequestType.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ) ? true : false;
            }
        };
        var searcheUtilsdmnin = function (valLists, toSearch) {
            return _.filter(valLists,
                function (i) {
                    /* Search Text in all 3 fields */
                    return searchUtissAmin(i, toSearch);
                });
            function searchUtissAmin(item, toSearch) {
                /* Search Text in all 3 fields */
                if (item.SystemAccess == undefined || item.SystemAccess == null) {
                    item.SystemAccess = "";
                }

                return (

                    item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Author.Title.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||                  
                    item.Purpose.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.UtilitiesApplyingFor.results[0].toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.WorkFlowStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
            }
        };

        var searchedSysdmnin = function (valLists, toSearch, Startdate, EndDate) {
            return _.filter(valLists,
                function (i) {
                    return searchSysAmin(i, toSearch, Startdate, EndDate);
                });
            function searchSysAmin(item, toSearch, Startdate, EndDate) {
                if (toSearch != "") {
                    if (toSearch == "1" || toSearch == "2" || toSearch == "3" || toSearch == "4") {
                        return (
                            item.ApplicationStatus == toSearch
                        ) ? true : false;
                    } else {
                        return (
                           

                           item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.Author.Title.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.WorkFlowCode.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                          //  item.SystemAccess.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.WorkFlowStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
                    }
                }
                else if (Startdate != null && EndDate != null) {
                    var Reqdate = new Date(item.Created).getFullYear() + '/' + (new Date(item.Created).getMonth() + 1) + '/' + new Date(item.Created).getDate();
                    return (
                        (new Date(Reqdate) >= new Date(Startdate)) && (new Date(Reqdate) <= new Date(EndDate))
                    ) ? true : false;
                }
                else {
                    //                  
                }
            }

        };

        var xsearchedSysdmnin = function (valLists, toSearch) {
            return _.filter(valLists,
                function (i) {
                    /* Search Text in all 3 fields */
                    return searchSysAmin(i, toSearch);
                });
            function searchSysAmin(item, toSearch) {
                /* Search Text in all 3 fields */
                if (item.SystemAccess == undefined || item.SystemAccess == null) {
                    item.SystemAccess = "";
                }
                   
                return (

                    item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||                  
                    item.Author.Title.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.WorkFlowCode.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.SystemAccess.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.WorkFlowStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
            }
        };

        var searchedCRSRAdmnin = function (valLists, toSearch) {
            return _.filter(valLists,
                function (i) {
                    /* Search Text in all 3 fields */
                    return searchCRAdmin(i, toSearch);
                });
            function searchCRAdmin(item, toSearch) {
                /* Search Text in all 3 fields */
                return (

                    item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Author.Title.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.TypeofRequest.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.ProjectSystem.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.WorkFlowStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
            }
        };

        var searchedITAdmnin = function (valLists, toSearch) {
            return _.filter(valLists,
                function (i) {
                    /* Search Text in all 3 fields */
                    return searchITAdmin(i, toSearch);
                });
            function searchITAdmin(item, toSearch) {
                /* Search Text in all 3 fields */
                return (

                    item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Author.Title.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.WorkAreaCode.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.RequestTypeCode.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.WorkFlowStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
            }
        };
        var searched = function (valLists, toSearch) {        
            return _.filter(valLists,
                function (i) {
                    /* Search Text in all 3 fields */
                    return searchUtil(i, toSearch);
                });
            function searchUtil(item, toSearch) {
                /* Search Text in all 3 fields */           
                return (
                   
                    item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Created.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.PremisesAddress.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Purpose.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                    item.Vendor.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
            } 
        };

        var searchedEquiry = function (valLists, toSearch, Startdate, EndDate) {
            return _.filter(valLists,
                function (i) {
                    return searchUtil(i, toSearch, Startdate, EndDate);
                });
            function searchUtil(item, toSearch, Startdate, EndDate) {
                if (toSearch!="") {
                    if (toSearch == "1" || toSearch == "2" || toSearch == "3" || toSearch == "4") {
                        return (
                            item.ApplicationStatus == toSearch
                        ) ? true : false;
                    } else {
                        return (
                            item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.PremisesAddress.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.Purpose.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
                    }
                }
                else if (Startdate != null && EndDate != null) {
                    var Reqdate = new Date(item.Created).getFullYear() + '/' + (new Date(item.Created).getMonth() + 1) + '/' + new Date(item.Created).getDate();
                    return (
                        (new Date(Reqdate) >= new Date(Startdate)) && (new Date(Reqdate) <= new Date(EndDate))
                    ) ? true : false;
                }                
                else {
                    //                  
                }
            }

        };

        var searchedSysReqEquiry = function (valLists, toSearch, Startdate, EndDate) {
            return _.filter(valLists,
                function (i) {
                    return searchSysUtil(i, toSearch, Startdate, EndDate);
                });
            function searchSysUtil(item, toSearch, Startdate, EndDate) {
                if (toSearch != "") {
                    if (toSearch == "1" || toSearch == "2" || toSearch == "3" || toSearch == "4") {
                        return (
                            item.ApplicationStatus == toSearch
                        ) ? true : false;
                    } else {
                        return (
                            item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||                         
                            item.SystemAccess.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
                    }
                }
                else if (Startdate != null && EndDate != null) {
                    var Reqdate = new Date(item.Created).getFullYear() + '/' + (new Date(item.Created).getMonth() + 1) + '/' + new Date(item.Created).getDate();
                    return (
                        (new Date(Reqdate) >= new Date(Startdate)) && (new Date(Reqdate) <= new Date(EndDate))
                    ) ? true : false;
                }
                else {
                    //                  
                }
            }

        };

        var searchedITReqEquiry = function (valLists, toSearch, Startdate, EndDate) {
            return _.filter(valLists,
                function (i) {
                    return searchITUtil(i, toSearch, Startdate, EndDate);
                });
            function searchITUtil(item, toSearch, Startdate, EndDate) {
                if (toSearch != "") {
                    if (toSearch == "1" || toSearch == "2" || toSearch == "3" || toSearch == "4") {
                        return (
                            item.ApplicationStatus == toSearch
                        ) ? true : false;
                    } else {
                        return (
                            item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.WorkAreaCode.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.RequestTypeCode.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
                    }
                }
                else if (Startdate != null && EndDate != null) {
                    var Reqdate = new Date(item.Created).getFullYear() + '/' + (new Date(item.Created).getMonth() + 1) + '/' + new Date(item.Created).getDate();
                    return (
                        (new Date(Reqdate) >= new Date(Startdate)) && (new Date(Reqdate) <= new Date(EndDate))
                    ) ? true : false;
                }
                else {
                    //                  
                }
            }

        };

        var getSearchedHRRequests = function (valLists, toSearch, Startdate, EndDate) {
            return _.filter(valLists,
                function (i) {
                    return searchITUtil(i, toSearch, Startdate, EndDate);
                });
            function searchITUtil(item, toSearch, Startdate, EndDate) {
                if (toSearch != "") {
                    if (toSearch == "1" || toSearch == "2" || toSearch == "3" || toSearch == "4") {
                        return (
                            item.ApplicationStatus == toSearch
                        ) ? true : false;
                    } else {
                        return (
                            item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||                          
                            item.ManpowerRequestType.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
                    }
                }
                else if (Startdate != null && EndDate != null) {
                    var Reqdate = new Date(item.Created).getFullYear() + '/' + (new Date(item.Created).getMonth() + 1) + '/' + new Date(item.Created).getDate();
                    return (
                        (new Date(Reqdate) >= new Date(Startdate)) && (new Date(Reqdate) <= new Date(EndDate))
                    ) ? true : false;
                }
                else {
                    //                  
                }
            }

        };

        var searchedCRSRReqEquiry = function (valLists, toSearch, Startdate, EndDate) {
            return _.filter(valLists,
                function (i) {
                    return searchCRSUtil(i, toSearch, Startdate, EndDate);
                });
            function searchCRSUtil(item, toSearch, Startdate, EndDate) {
                if (item.ProjectSystem == null) {
                    item.ProjectSystem = "";
                }
                if (item.TypeofRequest == null) {
                    item.TypeofRequest = "";
                }
                if (toSearch != "") {
                    if (toSearch == "1" || toSearch == "2" || toSearch == "3" || toSearch == "4") {
                        return (
                            item.ApplicationStatus == toSearch
                        ) ? true : false;
                    } else {
                        return (
                            item.ApplicationID.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.ProjectSystem.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.TypeofRequest.toLowerCase().indexOf(toSearch.toLowerCase()) > -1 ||
                            item.ApplicationStatus.toLowerCase().indexOf(toSearch.toLowerCase()) > -1) ? true : false;
                    }
                }
                else if (Startdate != null && EndDate != null) {
                    var Reqdate = new Date(item.Created).getFullYear() + '/' + (new Date(item.Created).getMonth() + 1) + '/' + new Date(item.Created).getDate();
                    return (
                        (new Date(Reqdate) >= new Date(Startdate)) && (new Date(Reqdate) <= new Date(EndDate))
                    ) ? true : false;
                }
                else {
                    //                  
                }
            }

        };

        var paged = function (valLists, pageSize) {       
           var retVal = [];
            for (var i = 0; i < valLists.length; i++) {
                if (i % pageSize === 0) {
                    retVal[Math.floor(i / pageSize)] = [valLists[i]];
                } else {
                    retVal[Math.floor(i / pageSize)].push(valLists[i]);
                }
            }
            return retVal;
        };

        return {
            getCurrentUser: getCurrentUser,
            GetUserIdFromLoginName: GetUserIdFromLoginName,
            getMailConfigFromRootSite: getMailConfigFromRootSite,
            getPropertiesFor: getPropertiesFor,
            getStaffInfoByAdId: getStaffInfoByAdId,
            getStaffBranchHead: getStaffBranchHead,
            getcoveringOfficerHead: getcoveringOfficerHead,
            getStaffClusterhHead: getStaffClusterhHead,
            getStaffDivisionHead: getStaffDivisionHead,
            getUserDeptHeadList: getUserDeptHeadList,
            getMyProperties: getMyProperties,
            getUserbyId: getUserbyId,
            createfolder: createfolder,
            chkIsMemberOfGroup: chkIsMemberOfGroup,
            getCurrentUserWithDetails: getCurrentUserWithDetails,
            paged: paged,
            searched: searched,
            getStaffDivisionBranchHead: getStaffDivisionBranchHead,
            searchedEquiry: searchedEquiry,
            searchedITReqEquiry: searchedITReqEquiry,
            searchedSysReqEquiry: searchedSysReqEquiry,
            getSearchedHRRequests: getSearchedHRRequests,
            searchedCRSRReqEquiry: searchedCRSRReqEquiry,
            searchedITAdmnin: searchedITAdmnin,
            searchedCRSRAdmnin: searchedCRSRAdmnin,
            searchedHRAdmnin: searchedHRAdmnin,
            searchedSysdmnin: searchedSysdmnin,
            searcheUtilsdmnin: searcheUtilsdmnin,
            getBranchHeadFromStaffDir: getBranchHeadFromStaffDir
        };
    }]);
})();