using System;
using System.Web;

namespace FlashCards.Models
{
    static class HttpExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri) { Query = httpValueCollection.ToString() };

            return ub.Uri;
        }
    }
}