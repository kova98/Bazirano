using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;

namespace Bazirano.Infrastructure.TagHelpers
{
    public class ThreadPostTextTagHelper : TagHelper
    {
        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string shortenedText = "";

            if (Text.Length > 100)
            {
                shortenedText = Text.Substring(0, 100);

                if (shortenedText.Last() == ' ')
                {
                    shortenedText.Remove(shortenedText.Last());
                }

                shortenedText += "...";
            }

            string textToDisplay = Text.Length > 100 ? shortenedText : Text;

            string greenText = PostTextHelper.GetGreenText(textToDisplay, true);

            output.Content.SetHtmlContent(greenText);
        }
    }
}