using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bazirano.Infrastructure.TagHelpers
{
    public class PostTextTagHelper : TagHelper
    {
        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";

            string greenText = PostTextHelper.GetGreenText(Text);
            string textWithLinks = PostTextHelper.GeneratePostAnchorLinks(greenText);

            output.Content.SetHtmlContent(textWithLinks);
        }

    }
}
