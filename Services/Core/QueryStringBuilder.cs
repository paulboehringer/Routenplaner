using System.Text;

namespace Services.Core
{
    /// <summary>
    /// Used to generate querys to access google api's
    /// </summary>
    public class QueryStringBuilder
    {
        public StringBuilder Sb { get; private set; }

        public QueryStringBuilder()
        {
            Sb = new StringBuilder();
        }

        public QueryStringBuilder Append(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (Sb.Length > 0) Sb.Append("&");
                Sb.Append(value);
            }
            return this;
        }

        public QueryStringBuilder Append(string key, string value)
        {
            if (string.IsNullOrEmpty(value) == false)
            {
                if (Sb.Length > 0) Sb.Append("&");
                Sb.Append(key).Append("=").Append(value);
            }
            return this;
        }

        public override string ToString()
        {
            return Sb.ToString();
        }
    }
}
