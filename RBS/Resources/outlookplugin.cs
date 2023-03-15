

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Resources
{
    [CompilerGenerated]
    [DebuggerNonUserCode]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    public class outlookplugin
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal outlookplugin()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)outlookplugin.resourceMan, (object)null))
                    outlookplugin.resourceMan = new ResourceManager("Resources.outlookplugin", typeof(outlookplugin).Assembly);
                return outlookplugin.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CultureInfo Culture
        {
            get => outlookplugin.resourceCulture;
            set => outlookplugin.resourceCulture = value;
        }

        public static string enddate_time_check => outlookplugin.ResourceManager.GetString(nameof(enddate_time_check), outlookplugin.resourceCulture);

        public static string err_fav_1 => outlookplugin.ResourceManager.GetString(nameof(err_fav_1), outlookplugin.resourceCulture);

        public static string non_asset_booking => outlookplugin.ResourceManager.GetString(nameof(non_asset_booking), outlookplugin.resourceCulture);

        public static string select_futuerdate => outlookplugin.ResourceManager.GetString(nameof(select_futuerdate), outlookplugin.resourceCulture);

        public static string start_end_datecheck => outlookplugin.ResourceManager.GetString(nameof(start_end_datecheck), outlookplugin.resourceCulture);

        public static string validate_no_enddate => outlookplugin.ResourceManager.GetString(nameof(validate_no_enddate), outlookplugin.resourceCulture);

        public static string validate_subject => outlookplugin.ResourceManager.GetString(nameof(validate_subject), outlookplugin.resourceCulture);
    }
}
