using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace BibliotekaLib
{
    public static class Helper
    {
        public static string GetDefaultDBConnectionString() => ConfigurationManager.ConnectionStrings["BibliotekaDatabase"].ConnectionString;
    }
}
