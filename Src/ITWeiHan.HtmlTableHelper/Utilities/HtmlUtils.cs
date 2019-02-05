using System.Globalization;
using System.Text;

namespace HtmlTableHelper.Utilities
{
    internal static class HtmlUtils
    {
        /// <summary>
        /// From [Westwind.Utilities/HtmlUtils.cs at master · RickStrahl/Westwind.Utilities]
        /// (https://github.com/RickStrahl/Westwind.Utilities/blob/master/Westwind.Utilities/Utilities/HtmlUtils.cs)
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="text">The text string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        public static string HtmlEncode(string text)
        {
            if (text == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(text.Length);

            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                switch (text[i])
                {

                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '\'':
                        sb.Append("&#39;");
                        break;
                    default:
                        if (text[i] > 159)
                        {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else
                        {
                            sb.Append(text[i]);
                        }

                        break;
                }
            }
            return sb.ToString();
        }

    }
}
