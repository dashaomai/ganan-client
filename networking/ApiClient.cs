using System.Collections.Generic;
using System.Text;
using Godot;
using GodotLogger;
using Newtonsoft.Json.Linq;

namespace Networking
{
    class ApiClient : Node
    {
        private static readonly Logger _log = LoggerHelper.GetLogger(typeof(ApiClient));

        private static readonly string USER_AGENT = "Mozilla/5.0 Godot/3.4.2 Mono/7.0";
        private static readonly string CONTENT_TYPE = "application/json";

        private string[] _headers;

        private readonly IDictionary<string, string> headers;

        private readonly HTTPRequest httpRequest = new HTTPRequest();

        public string BaseUrl { get; set; }

        public ApiClient()
        {
            headers = new Dictionary<string, string>();

            BuildHeaders();
        }

        public override void _Ready()
        {
            AddChild(httpRequest);

            httpRequest.Connect("request_completed", this, nameof(_OnRequestCompleted));
        }

        /// <summary>构造下次请求时使用的 headers </summary>
        private void BuildHeaders()
        {
            List<string> newHeaders = new List<string>();

            newHeaders.Add($"user-agent: {USER_AGENT}");
            newHeaders.Add($"Content-Type: {CONTENT_TYPE}");

            foreach (KeyValuePair<string, string> kv in headers)
            {
                newHeaders.Add($"{kv.Key}: {kv.Value}");
            }

            _headers = newHeaders.ToArray();
        }

        private void _OnRequestCompleted(int result, int response_code, string[] headers, byte[] body)
        {
            string response = Encoding.UTF8.GetString(body);

            _log.Debug($"response for {response}");
        }

        public void ApiGet(string url)
        {

        }

        public void ApiPost(string url, object payload)
        {
            var body = JToken.FromObject(payload);

            var error = httpRequest.Request(
                BaseUrl + url, _headers, BaseUrl.StartsWith("https://"), HTTPClient.Method.Post, body.ToString()
            );

            if (error == Error.Ok)
            {
                _log.Debug($"post to {url} with payload {body}");
            }
            else
            {
                _log.Warn($"post to {url} handle an error {error}");
            }
        }
    }
}