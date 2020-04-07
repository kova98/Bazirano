using System.Linq;
using System.Text;

namespace Bazirano.Aggregator.Helpers
{
    public class KeywordHelper
    {
        public string GetKeywordsFromTitle(string title)
        {
            var stringBuilder = new StringBuilder();
            foreach (var c in title.Where(c => char.IsLetter(c) || c == ' '))
            {
                stringBuilder.Append(c);
            }

            var words = stringBuilder.ToString().ToUpper().Split(' ');
            var validWords = words.Where(w => IsWordValid(w));

            return string.Join(',', validWords);
        }

        private bool IsWordValid(string word)
        {
            if (word.Length < 4 ||
                word == "FOTO" ||
                word == "VIDEO")
            {
                return false;
            }

            return true;
        }
    }
}