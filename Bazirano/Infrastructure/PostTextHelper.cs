using System;
using System.Collections.Generic;
using System.Text;

namespace Bazirano.Infrastructure
{
    public class PostTextHelper
    {
        public static string GetGreenText(string text, bool isThreadPost = false)
        {
            string[] rows = text.Split('\n');

            if (isThreadPost)
            {
               if (rows.Length > 5)
                {
                    string[] shortened = new string[5];
                    Array.Copy(rows, shortened, 5);
                    rows = shortened;
                }               
            }

            StringBuilder builder = new StringBuilder();

            foreach (var row in rows)
            {
                if (row[0] == '>')
                {
                    builder.Append("<span style=\"color:lightgreen\">");
                    builder.Append(row);
                    builder.Append("</span><br/>");
                }
                else
                {
                    builder.Append($"{row}<br/>");
                }
            }

            return builder.ToString();
        }
    }
}
