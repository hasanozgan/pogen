using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace Netology.Localization
{
 
    /// <summary>
    /// String: msgid
    /// Catalog...
    /// </summary>
    public class CatalogCollection : Dictionary<string, Catalog>
    {
    }

    public class Catalog
    {
        const string CatalogCacheName = "__MESSAGES__";

        List<string> fileNames;
        string msgid;
        string msgstr;

        public Catalog()
        {
            fileNames = new List<string>();
        }

        public List<string> FileNames
        {
            get { return fileNames; }
            set { fileNames = value; }
        }

        public string MessageId
        {
            get { return msgid; }
            set { msgid = value; }
        }

        public string Message
        {
            get { return msgstr; }
            set { msgstr = value; }
        }

        public static string GetText(string langCode, string msgid)
        {
            string[] lang = langCode.Split(new char[] { '-' });
            string language = lang[0];

            try
            {
                string msgstr = ((LanguageCollection)HttpContext.Current.Application[CatalogCacheName])[language].Catalogs[msgid].Message;
                
                return ((msgstr.Length > 0) ? msgstr : msgid);
            }
            catch
            {
                return msgid;
            }
        }

        public static void PrepareCatalog()
        {
            string path = string.Format("{0}\\PO_Files", HttpContext.Current.Request.PhysicalApplicationPath);
            DirectoryInfo d = new DirectoryInfo(path);
            LanguageCollection langCollection = new LanguageCollection();

            foreach (FileInfo f in d.GetFiles("*.po"))
            {
                Language language = new Language();
                language.LanguageName = f.Name.Substring(0, f.Name.IndexOf(".po"));

                try
                {
                    FileStream fs = f.Open(FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs);

                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Match mm = Regex.Match(line, "^msgid \"(.*)\"");

                        if (mm.Success)
                        {
                            string msgid = mm.Groups[1].Value;

                            if ((line = sr.ReadLine()) != null)
                            { 
                                Match mMm = Regex.Match(line, "^msgstr \"(.*)\"");

                                if (mMm.Success)
                                {
                                    string msgstr = mMm.Groups[1].Value;

                                    Catalog catalog = new Catalog();
                                    catalog.MessageId = msgid;
                                    catalog.Message = msgstr;

                                    language.Catalogs.Add(msgid, catalog);
                                }  
                            }
                        }
                    }

                    langCollection.Add(language.LanguageName, language);
                }
                catch
                { }

                if (langCollection.Count > 0)
                {
                    HttpContext.Current.Application.Remove(CatalogCacheName);
                    HttpContext.Current.Application.Add(CatalogCacheName, langCollection);
                }
            }
        }        
    }
}
