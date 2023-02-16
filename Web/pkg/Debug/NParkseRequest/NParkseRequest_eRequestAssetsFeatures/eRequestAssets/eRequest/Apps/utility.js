var Utility = Utility || {};
Utility.helpers = {
    //initializePeoplePicker: function (peoplePickerElementId, AllowMultipleValues) {
    //    // Create a schema to store picker properties, and set the properties.
    //    var schema = {};
    //    schema['PrincipalAccountType'] = 'User,DL,SecGroup,SPGroup';
    //    schema['SearchPrincipalSource'] = 15;
    //    schema['ResolvePrincipalSource'] = 15;
    //    schema['AllowMultipleValues'] = AllowMultipleValues;
    //    schema['MaximumEntitySuggestions'] = 50;
    //    schema['Width'] = '280px';
    //    //schema['SharePointGroupID'] = 6
    //    // Render and initialize the picker.
    //    // Pass the ID of the DOM element that contains the picker, an array of initial
    //    // PickerEntity objects to set the picker value, and a schema that defines
    //    // picker properties.
    //    SPClientPeoplePicker_InitStandaloneControlWrapper(peoplePickerElementId, null, schema);
    //},
    initializePeoplePicker: function (peoplePickerElementId, displayName, userName, width, AllowMultipleValues) {
        var schema = {};
        schema['PrincipalAccountType'] = 'User,DL,SecGroup,SPGroup';
        schema['SearchPrincipalSource'] = 15;
        schema['ResolvePrincipalSource'] = 15;
        schema['AllowMultipleValues'] = AllowMultipleValues;
        schema['MaximumEntitySuggestions'] = 50;
        schema['Width'] = width + 'px';
        schema['margin-left'] = '5px';
        schema['height'] = '50px';
        //            schema['SharePointGroupID'] =10;
        var users = null;
        if (displayName != null) {
            users = new Array(1);
            var user = new Object();
            user.AutoFillDisplayText = displayName;
            user.AutoFillKey = userName;
            user.AutoFillSubDisplayText = "";
            user.DisplayText = displayName;
            user.EntityType = "User";
            user.IsResolved = true;
            user.Key = userName;
            user.ProviderDisplayName = "Tenant";
            user.ProviderName = "Tenant";
            user.Resolved = true;
            users[0] = user;
        }
        SPClientPeoplePicker_InitStandaloneControlWrapper(peoplePickerElementId, users, schema);
    },
    /// username should be passed as 'domain\username'
    getUserInfo: function (peoplePickerElementId) {
        var UsersID = "";
        var peoplePicker = SPClientPeoplePicker.SPClientPeoplePickerDict[peoplePickerElementId + "_TopSpan"];

        if (peoplePicker.HasInputError) return false; // if any error
        else if (!peoplePicker.HasResolvedUsers()) return false; // if any invalid users
        else if (peoplePicker.TotalUserCount > 0) {
            // Get information about all users.
            var users = peoplePicker.GetAllUserInfo();
            //var userInfo = '';
            //var promise = '';
            for (var i = 0; i < users.length; i++) {
                UsersID += users[i].DisplayText + "\n";
                UsersID += users[i].EntityData.Email;
            }
            // Get user keys.
            var keys = peoplePicker.GetAllUserKeys();
            var finalusers = new Array();
            for (var i = 0; i < users.length; i++) {
                var arryuser = users[i];
                finalusers.push(SP.FieldUserValue.fromUser(arryuser.Key));
            }
            return finalusers;
        }
    },
    getUrlParameter: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    },
    SetUserFieldValue: function (fieldName, LoginName) {
        var _PeoplePicker = $("div[title='" + fieldName + "']");
        var _PeoplePickerTopId = _PeoplePicker.attr('id');
        var _PeoplePickerEditer = $("input[title='" + fieldName + "']");
        var usrObj = { 'Key': LoginName };
        var _PeoplePickerOject = SPClientPeoplePicker.SPClientPeoplePickerDict[_PeoplePickerTopId + "_TopSpan"];
        _PeoplePickerOject.AddUnresolvedUser(usrObj, true);
        //disable the field
        // _PeoplePickerOject.SetEnabledState(false);
        //hide the delete/remove use image from the people picker
        //  $('.sp-peoplepicker-delImage').css('display', 'none');
    },
    // Utility function to remove base64 URL prefix and store base64-encoded string in a Uint8Array
    convertDataURIToBinary: function (dataURI) {
        var BASE64_MARKER = ';base64,';
        var base64Index = dataURI.indexOf(BASE64_MARKER) + BASE64_MARKER.length;
        var base64 = dataURI.substring(base64Index);
        var raw = window.atob(base64);
        var rawLength = raw.length;
        var array = new Uint8Array(new ArrayBuffer(rawLength));

        for (i = 0; i < rawLength; i++) {
            array[i] = raw.charCodeAt(i);
        }
        return array;
    },
    registerPPOnChangeEvent: function (ppElement) {
        var ppId = ppElement.attr('id') + "_TopSpan";
        var addOnChanged = function (ctx) {
            if (SPClientPeoplePicker && SPClientPeoplePicker.SPClientPeoplePickerDict && SPClientPeoplePicker.SPClientPeoplePickerDict[ppId]) {
                var picker = SPClientPeoplePicker.SPClientPeoplePickerDict[ppId];
                picker.oldChanged = picker.OnControlResolvedUserChanged;
                picker.OnControlResolvedUserChanged = function () {
                    if (picker.TotalUserCount == 0) {
                        var originalPPlId = ppId.replace("_TopSpan", "");
                        $("#" + originalPPlId).parent().find(".userId").html("");
                        

                    }
                    else {
                        // If user name is resolved  
                        setTimeout(function () {
                            Execute(ppId);
                        }, 100);
                    }
                    picker.oldChanged();
                };
            } else {
                setTimeout(function () { addOnChanged(ctx); }, 100);
            }
        };
        addOnChanged();
    } ,
    SendMail: function (sendmailRESTUrl, toList, ccList, subject, mailContent) {

        restHeaders = {
            "Accept": "application/json;odata=verbose",
            "X-RequestDigest": $("#__REQUESTDIGEST").val(),
            "Content-Type": "application/json;odata=verbose"
        },
            mailObject = {
                'properties': {
                    '__metadata': {
                        'type': 'SP.Utilities.EmailProperties'
                    },
                    'To': {
                        'results': toList
                    },
                    //'CC': {
                    //    'results': ccList
                    //},
                    'Subject': subject,
                    'Body': mailContent,
                    "AdditionalHeaders":
                    {
                        "__metadata":
                            { "type": "Collection(SP.KeyValue)" },
                        "results":
                            [
                                {
                                    "__metadata": {
                                        "type": 'SP.KeyValue'
                                    },
                                    "Key": "content-type",
                                    "Value": 'text/html',
                                    "ValueType": "Edm.String"
                                }
                            ]
                    }

                }
            };
        return $.ajax({
            contentType: "application/json",
            url: sendmailRESTUrl,
            type: "POST",
            data: JSON.stringify(mailObject),
            headers: restHeaders,
            success: function (data) {
                console.log("An email was sent.");
            },
            error: function (args) {
                console.log("We had a problem and an email was not sent.");
            }
        });
    },
    sendEmail: function (to, subject, body) {
        //Get the relative url of the site
        var siteurl = _spPageContextInfo.webServerRelativeUrl;
        var urlTemplate = siteurl + "/_api/SP.Utilities.Utility.SendEmail";
        $.ajax({
            contentType: 'application/json',
            url: urlTemplate,
            type: "POST",
            data: JSON.stringify({
                'properties': {
                    '__metadata': {
                        'type': 'SP.Utilities.EmailProperties'
                    },
                    //'From': from,
                    'To': {
                        'results': [to]
                    },
                    'Body': body,
                    'Subject': subject
                }
            }),
            headers: {
                "Accept": "application/json;odata=verbose",
                "content-type": "application/json;odata=verbose",
                "X-RequestDigest": jQuery("#__REQUESTDIGEST").val()
            },
            success: function (data) {
                console.log('Email Sent Successfully');
            },
            error: function (err) {
                console.log('Error in sending Email: ' + JSON.stringify(err));
            }
        });
    },
    sendEmailwithCC: function (to, ccList,subject, body) {
        //Get the relative url of the site
        var siteurl = _spPageContextInfo.webServerRelativeUrl;
        var urlTemplate = siteurl + "/_api/SP.Utilities.Utility.SendEmail";
        $.ajax({
            contentType: 'application/json',
            url: urlTemplate,
            type: "POST",
            data: JSON.stringify({
                'properties': {
                    '__metadata': {
                        'type': 'SP.Utilities.EmailProperties'
                    },
                    //'From': from,
                    'To': {
                        'results': [to]
                    },
                     'CC': {
                        'results': ccList
                    },
                    'Body': body,
                    'Subject': subject
                }
            }),
            headers: {
                "Accept": "application/json;odata=verbose",
                "content-type": "application/json;odata=verbose",
                "X-RequestDigest": jQuery("#__REQUESTDIGEST").val()
            },
            success: function (data) {
                console.log('Email Sent Successfully');
            },
            error: function (err) {
                console.log('Error in sending Email: ' + JSON.stringify(err));
            }
        });
    },
    SendGMailService: function (hostName, portNumber, enableSSL, password, fromEmail, dispName, toEmail, cc, bcc, subject, body) {
       
        $.ajax({
            type: "POST",
            url: _spPageContextInfo.webAbsoluteUrl+"/_layouts/15/MailService.asmx/SendGmail",
            data: "{ hostName: '" + hostName + "',portNumber: " + portNumber + ",enableSSL: " + enableSSL + ",password: '" + password + "',fromEmail: '" + fromEmail + "',dispName: '" + dispName + "',toEmail: '" + toEmail + "',cc: '" + cc + "',bcc: '" + bcc + "', subject: '" + subject + "', body: '" + body + "' }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (r) {
               console.log(r.d);
            },
            error: function (r) {
                console.log(r.responseText);
            },
            failure: function (r) {
                console.log(r.responseText);
            }
        });
        return false;
    },
    SendMailService: function (hostName, portNumber, fromEmail, dispName, toEmail, cc, bcc, subject, body) {
        
        $.ajax({
            type: "POST",
            url: _spPageContextInfo.webAbsoluteUrl +"/_layouts/15/MailService.asmx/SendEmail",
            data: "{ hostName: '" + hostName + "',portNumber: " + portNumber + ",fromEmail: '" + fromEmail + "',dispName: '" + dispName + "',toEmail: '" + toEmail + "',cc: '" + cc + "',bcc: '" + bcc + "', subject: '" + subject + "', body: '" + body + "' }",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (r) {
                console.log(r.d);
            },
            error: function (r) {
                console.log(r.responseText);
            },
            failure: function (r) {
                console.log(r.responseText);
            }
        });
        return false;
    }
};
