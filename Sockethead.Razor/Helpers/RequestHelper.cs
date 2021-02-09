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

        public static Dictionary<string, string> QueryParamDictionary(this HttpRequest request)
            => request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

        public static string UrlUpdateQuery(this HttpRequest request, Dictionary<string, string> queryParams)
        {
            // original query
            var query = request.Query.ToDictionary(q => q.Key, q => q.Value);

            // update query with new params
            foreach (var kvp in queryParams)
            {
                if (string.IsNullOrEmpty(kvp.Value))
                {
                    if (query.ContainsKey(kvp.Key))
                        query.Remove(kvp.Key);
                }
                else
                {
                    query[kvp.Key] = kvp.Value;
                }
            }

            // rebuild url with new query
            return QueryHelpers.AddQueryString(request.UrlWithoutQuery(), query);
        }
    }
}
