using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using GodotLogger;
using Newtonsoft.Json.Linq;
using static Godot.HTTPClient;
using static Godot.HTTPRequest;

namespace Networking
{
    class ApiClient : Node
    {
        private static readonly Logger _log = LoggerHelper.GetLogger(typeof(ApiClient));

        private static readonly string USER_AGENT = "Mozilla/5.0 Godot/3.4.2 Mono/7.0";
        private static readonly string CONTENT_TYPE = "application/json";

        public event Action<int> OnResultError;
        public event Action<int> OnResponseCodeError;

        private string[] _headers;

        private readonly IDictionary<string, string> headers;

        private readonly HTTPRequest httpRequest = new HTTPRequest();

        public string BaseUrl { get; set; }

        private Action<JObject> _callback = null;

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
            var r = (Result)result;

            if (r == Result.Success)
            {
                var rc = (ResponseCode)response_code;

                if (rc == ResponseCode.Ok)
                {

                    var response = JObject.Parse(Encoding.UTF8.GetString(body));

                    _callback?.Invoke(response);
                    _callback = null;

                    _log.Debug($"response for {response}");

                }
                else
                {
                    OnResponseCodeError?.Invoke(response_code);
                }
            }
            else
            {
                OnResultError?.Invoke(result);
            }
        }

        private void _StoreCb(Action<JObject> callback)
        {
            _callback = callback;
        }

        public void ApiGet(string url)
        {

        }

        public void ApiPost(string url, object payload, Action<JObject> callback)
        {
            var body = JToken.FromObject(payload);
            var uri = BaseUrl + url;

            var error = httpRequest.Request(
                uri, _headers, BaseUrl.StartsWith("https://"), HTTPClient.Method.Post, body.ToString()
            );

            if (error == Error.Ok)
            {
                _log.Debug($"post to {uri} with payload {body}");

                _StoreCb(callback);
            }
            else
            {
                _log.Warn($"post to {uri} handle an error {error}");
            }
        }

        public void Login(object payload)
        {
            ApiPost("/v1/account/login", payload, _OnLogined);
        }

        private void _OnLogined(JObject loginResponse)
        {
            _log.Debug($"logined with response {loginResponse}");
        }
    }
}