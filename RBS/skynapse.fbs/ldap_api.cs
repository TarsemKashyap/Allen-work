// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.ldap_api
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace skynapse.fbs
{
  public class ldap_api : api_base
  {
    private string path;
    private string domain;
    private SearchResult result;

    public ldap_api(string path, string domain)
    {
      this.path = path;
      this.domain = domain;
    }

    public activedirectory is_authenticated(
      string searchfield,
      string uname,
      string pswd,
      string returnfield)
    {
      string[] strArray = returnfield.Split(',');
      string str1 = "";
      string str2 = "";
      string str3 = "";
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (string.IsNullOrEmpty(pswd))
      {
        try
        {
          this.log.Info((object) "check AD without password.");
          DirectoryEntry searchRoot = new DirectoryEntry();
          searchRoot.Path = this.path;
          this.log.Info((object) ("path:" + searchRoot.Path));
          searchRoot.AuthenticationType = AuthenticationTypes.None;
          DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot);
          directorySearcher.Filter = string.Format("(" + searchfield + "={0})", (object) uname);
          foreach (string str4 in strArray)
            directorySearcher.PropertiesToLoad.Add(str4.Trim());
          this.log.Info((object) "searcing. find one..");
          this.result = directorySearcher.FindOne();
          this.log.Info((object) "searcing complete.");
        }
        catch (Exception ex)
        {
          this.log.Error((object) ("1. Active Directory : " + ex.Message.ToString()));
        }
      }
      else
      {
        this.log.Info((object) "check AD with password.");
        string username = this.domain + "\\" + uname;
        DirectoryEntry searchRoot = new DirectoryEntry(this.path, username, pswd, AuthenticationTypes.None);
        this.log.Info((object) ("dom:" + username));
        this.log.Info((object) ("path:" + searchRoot.Path));
        try
        {
          this.log.Info((object) "getting nativeobject");
          object nativeObject = searchRoot.NativeObject;
          searchRoot.AuthenticationType = AuthenticationTypes.None;
          this.log.Info((object) "initializing directorysearch");
          DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot);
          this.log.Info((object) "settings up main filter criteria");
          directorySearcher.Filter = string.Format("(" + searchfield + "={0})", (object) uname);
          this.log.Info((object) ("filtering..." + (object) strArray.Length));
          foreach (string str5 in strArray)
          {
            this.log.Info((object) ("loading item:" + str5));
            directorySearcher.PropertiesToLoad.Add(str5.Trim());
          }
          this.log.Info((object) "getting results.");
          this.result = directorySearcher.FindOne();
          this.log.Info((object) "get result");
        }
        catch (Exception ex)
        {
          this.log.Error((object) ("2. Active Directory : " + ex.Message.ToString()));
        }
      }
      activedirectory activedirectory = new activedirectory();
      if (this.result != null)
      {
        this.log.Info((object) "Result is not null.");
        try
        {
          str1 = (string) this.result.Properties["mail"][0];
          if (str1 != "")
            this.log.Info((object) ("LDAP search done sucessfully.User email id was found: " + str1));
          else
            this.log.Info((object) "LDAP search done sucessfully.But User email id not found.");
        }
        catch (Exception ex)
        {
          this.log.Info((object) ("3. Checking Active Directory : " + ex.ToString()));
          str1 = "";
        }
        try
        {
          str2 = (string) this.result.Properties["displayName"][0];
          if (str2 != "")
            this.log.Info((object) ("LDAP search done sucessfully.User display name was found: " + str2));
          else
            this.log.Info((object) "LDAP search done sucessfully.But User display name is not found.");
        }
        catch (Exception ex)
        {
          this.log.Info((object) ("4. Checking Active Directory : " + ex.ToString()));
          str2 = "";
        }
        try
        {
          str3 = (string) this.result.Properties["company"][0];
          if (str3 != "")
            this.log.Info((object) ("LDAP search done sucessfully.User organisation was found: " + str3));
          else
            this.log.Info((object) ("LDAP search done sucessfully.But User organisation is not found." + str3));
        }
        catch (Exception ex)
        {
          this.log.Info((object) ("5. Checking Active Directory : " + ex.ToString()));
          str3 = "";
        }
      }
      else
        this.log.Info((object) "No Result from AD.");
      activedirectory.email = str1;
      activedirectory.displayname = str2;
      activedirectory.organisation = str3;
      return activedirectory;
    }
  }
}
