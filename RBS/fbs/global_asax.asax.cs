using log4net.Config;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Profile;

namespace ASP
{
    [CompilerGlobalScope]
    public class global_asax : System.Web.HttpApplication
    {

        private static bool __initialized;

        private void Application_Start(object sender, EventArgs e) => XmlConfigurator.Configure();

        private void Application_End(object sender, EventArgs e)
        {
        }

        private void Application_Error(object sender, EventArgs e)
        {
            HttpException lastError = this.Server.GetLastError() as HttpException;
            try
            {
                if (lastError.GetHttpCode() != 404)
                    return;
                this.Response.Redirect("~/login.aspx");
            }
            catch
            {
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication httpApplication = sender as HttpApplication;
            if (!httpApplication.Request.RawUrl.Contains(".aspx") || httpApplication.Request.RawUrl.Contains("/display/"))
                return;
            httpApplication.Response.Filter = (Stream)new WhitespaceFilter(httpApplication.Response.Filter);
        }

        private void Session_Start(object sender, EventArgs e)
        {
        }

        private void Session_End(object sender, EventArgs e) => this.Session.Clear();

        [DebuggerNonUserCode]
        public global_asax()
        {
            if (global_asax.__initialized)
                return;
            global_asax.__initialized = true;
        }

        protected DefaultProfile Profile => (DefaultProfile)this.Context.Profile;
    }
}