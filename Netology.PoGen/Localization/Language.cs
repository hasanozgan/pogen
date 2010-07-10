using System;
using System.Collections.Generic;
using System.Text;

namespace Netology.Localization
{
    public class LanguageCollection : Dictionary<string, Language>
    {

    }

    public class Language
    {
        string languageName;
        CatalogCollection catalogs;

        public string LanguageName
        {
            get { return languageName; }
            set { languageName = value; }
        }

        public CatalogCollection Catalogs
        {
            get { return catalogs; }
            set { catalogs = value; }
        }

        public Language()
        {
            catalogs = new CatalogCollection();
        }

    }

}
