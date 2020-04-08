using Bazirano.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Library.Extensions
{
    public static class NewsSourceExtensions
    {
        public static string DisplayName(this NewsSource newsSource)
        {
            return newsSource switch
            {
                NewsSource.Unknown => "Nepoznato",
                NewsSource.IndexHr => "Index",
                NewsSource.KonzervaHr => "Konzerva",
                NewsSource.PriznajemHr => "Priznajem",

                _ => throw new ArgumentException(message: "invalid enum value", paramName: "newsSource")
            };
        }

        public static string Url(this NewsSource newsSource)
        {
            return newsSource switch
            {
                NewsSource.Unknown => "Nepoznato",
                NewsSource.IndexHr => "https://www.index.hr/",
                NewsSource.KonzervaHr => "https://www.konzerva.hr/",
                NewsSource.PriznajemHr => "https://www.priznajem.hr/",

                _ => throw new ArgumentException(message: "invalid enum value", paramName: "newsSource")
            };
        }
    }
}
