using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public static class CollectionExtensions
    {
        public static int KeywordMatches(this ICollection<string> list, ICollection<string> keywords)
        {
            int matchCount = 0;
            foreach (var item in list)
            {
                if (keywords.Contains(item))
                {
                    matchCount++;
                }
            }

            return matchCount;
        }
    }
}
