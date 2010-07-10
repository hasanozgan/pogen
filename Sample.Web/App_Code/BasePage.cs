using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : System.Web.UI.Page
{
    private const string LanguageSessionName = @"LanguageSession";

    protected override void InitializeCulture()
    {
        System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(CurrentLanguage);
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        base.InitializeCulture();
    }

    public static string CurrentLanguage
    {
        get
        {
            if (HttpContext.Current.Session[LanguageSessionName] == null)
            {
                HttpContext.Current.Session[LanguageSessionName] = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            }
            
            return (string)HttpContext.Current.Session[LanguageSessionName];
        }
        set
        {
            if (HttpContext.Current.Session[LanguageSessionName] == null)
            {
                HttpContext.Current.Session.Add(LanguageSessionName, value);
            }
            else
            {
                HttpContext.Current.Session[LanguageSessionName] = value;
            }
        }
    }

    public static string _(string msgid)
    {
        return Netology.Localization.Catalog.GetText(CurrentLanguage, msgid);
    }


}
