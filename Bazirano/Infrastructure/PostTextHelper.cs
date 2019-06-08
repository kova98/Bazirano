using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

            bool isPostStart = true;
            bool emptyLineAdded = false;

            foreach (var row in rows)
            {
                if (row.Length == 0 || row[0] == '\r')
                {
                    if (isPostStart)
                    {
                        // Start of the post, no empty lines allowed
                    }
                    else
                    {
                        if (emptyLineAdded == false && rows[rows.Length-1] != row)
                        {
                            // The first empty line in a row, add it and set the state to true.
                            builder.Append($"{row}<br/>");

                            emptyLineAdded = true;
                        }
                    }
                }
                else
                {
                    // The loop went past all the empty lines from the post start
                    isPostStart = false;

                    if (row[0] == '>')
                    {
                        builder.Append("<span style=\"color:lightgreen\">");
                        builder.Append(row);
                        builder.Append("</span><br/>");

                        emptyLineAdded = false;
                    }
                    else
                    {
                        builder.Append($"{row}<br/>");

                        emptyLineAdded = false;
                    }
                }
            }

            return builder.ToString();
        }

        public static string GeneratePostAnchorLinks(string text)
        {
            MatchCollection matches = Regex.Matches(text, "#([0-9])+");
            List<string> matchList = new List<string>();

            foreach (var m in matches)
            {
                matchList.Add(m.ToString());
            }

            string newText = "";

            foreach(var m in matchList)
            {
                string id = m.Remove(0, 1);
                newText = text.Replace(m, $"<a href=\"{m}\" onclick=\"scrollToAnchor({id})\" class=\"btn-link\">{m}</a>");
            }

            return newText;
        }
    }
}
