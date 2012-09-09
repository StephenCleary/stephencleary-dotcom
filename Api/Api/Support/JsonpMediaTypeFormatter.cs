using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api.Support
{
    /// <summary>
    /// Handles JsonP requests.
    /// </summary>
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public JsonpMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            CallbackParameterName = "callback";
        }

        /// <summary>
        /// The query string parameter for the callback function. Defaults to "callback" (compatible with jQuery).
        /// </summary>
        public string CallbackParameterName { get; set; }

        /// <summary>
        /// The value of <see cref="CallbackParameterName"/> for this request.
        /// </summary>
        private string callback;

        /// <summary>
        /// Saves the callback query parameter.
        /// </summary>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var formatter = new JsonpMediaTypeFormatter()
            {
                callback = GetCallbackFunction(request, CallbackParameterName)
            };

            formatter.SerializerSettings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            return formatter;
        }

        /// <summary>
        /// Parses the callback parameter from the request query string.
        /// </summary>
        /// <param name="request">The request to parse.</param>
        /// <param name="parameterName">The query string parameter name.</param>
        /// <returns>The value of the query string parameter, or <c>null</c> if it was not specified.</returns>
        private static string GetCallbackFunction(HttpRequestMessage request, string parameterName)
        {
            if (request.Method != HttpMethod.Get)
                return null;

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var queryVal = query[parameterName];

            if (string.IsNullOrEmpty(queryVal))
                return null;

            return queryVal;
        }

        /// <summary>
        /// Determines whether this is a JSON or JSON-P request. If it's a JSON request, the base class handles it; otherwise, it's passed to <see cref="DoWriteToStreamAsync"/>.
        /// </summary>
        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            if (callback == null)
                return base.WriteToStreamAsync(type, value, stream, content, transportContext);

            return DoWriteToStreamAsync(type, value, stream, content, transportContext);
        }

        /// <summary>
        /// Handles JSON-P formatting by wrapping the regular JSON formatting.
        /// </summary>
        private async Task DoWriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(callback + "(");
                await writer.FlushAsync();
                await base.WriteToStreamAsync(type, value, stream, content, transportContext);
                await writer.WriteAsync(")");
                await writer.FlushAsync();
            }
        }

        /// <summary>
        /// Registers the JSON-P formatter.
        /// </summary>
        public static void Register()
        {
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var defaultJsonFormatter = formatters.JsonFormatter;
            int index = formatters.IndexOf(defaultJsonFormatter);
            if (index != -1)
                formatters.RemoveAt(index);
            GlobalConfiguration.Configuration.Formatters.Insert(index, new JsonpMediaTypeFormatter());
        }
    }
}