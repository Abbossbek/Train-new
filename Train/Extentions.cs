using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Train
{
    public static class Extentions
    {
        public static async Task SafeClearCache(this WebView2 wv2, bool reload = false)
        {
            try
            {
                await wv2.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.LocalStorage
                    | CoreWebView2BrowsingDataKinds.GeneralAutofill
                    | CoreWebView2BrowsingDataKinds.Cookies
                    | CoreWebView2BrowsingDataKinds.Settings
                    | CoreWebView2BrowsingDataKinds.BrowsingHistory
                    | CoreWebView2BrowsingDataKinds.PasswordAutosave);
                if (reload)
                    wv2.CoreWebView2.Reload();
            }
            catch
            {

            }
        }
    }
}
