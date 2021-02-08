using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;

namespace Sockethead.Razor.Helpers
{
    public static class RequestHelper
    {
        public static string UrlWithoutQuery(this HttpRequest request)
            => string.Format($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}");

        public static string UrlUpdateQuery(this HttpRequest request, Dictionary<string, string> queryParams)
        {
            // original query
            var query = request.Query.ToDictionary(q => q.Key, q => q.Value);

            // update query with new params
            foreach (var kvp in queryParams)
                query[kvp.Key] = kvp.Value;

            // rebuild url with new query
            return QueryHelpers.AddQueryString(request.UrlWithoutQuery(), query);
        }
    }
}
