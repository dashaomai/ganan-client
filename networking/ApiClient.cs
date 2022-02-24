using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Account;
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

        /// 收到响应后，有错误码时的回调
        public event Action<int> OnResultError;
        public event Action<int> OnResponseCodeError;

        public delegate void OnLoginedHandler(in LoginResult result);
        /// <summary>
        /// 登录成功
        /// </summary>
        public event OnLoginedHandler OnLogined;

        /// 请求头管理
        private string[] _headers;

        private readonly IDictionary<string, string> headers;

        private readonly HTTPRequest httpRequest = new HTTPRequest();

        /// <summary>基础 URL</summary>
        public string BaseUrl { get; set; }

        private Action<JObject> _callback = null;


        /// 计时
        private float _exchangeOffset = 0f;
        private float _exchangeDelta = 0f;

        /// 状态记录
        private bool _isLogon = false;

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

        public override void _Process(float delta)
        {
            if (_exchangeOffset > 0f && _exchangeDelta < _exchangeOffset)
            {
                // 进行更换 jwt 的计时
                _exchangeDelta += delta;

                if (_exchangeDelta >= _exchangeOffset)
                {
                    _exchangeOffset = 0f;

                    Exchange();
                }
            }
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

        public void ApiGet(string url, Action<JObject> callback)
        {
            var uri = BaseUrl + url;

            var error = httpRequest.Request(
                uri, _headers, BaseUrl.StartsWith("https://"), HTTPClient.Method.Get
            );

            if (error == Error.Ok)
            {
                _log.Debug($"get to {uri} with headers {string.Join(",", _headers)}");

                _StoreCb(callback);
            }
            else
            {
                _log.Warn($"get to {uri} handle an error {error}");
            }
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
                _log.Debug($"post to {uri} with headers {string.Join(",", _headers)} and payload {body}");

                _StoreCb(callback);
            }
            else
            {
                _log.Warn($"post to {uri} handle an error {error}");
            }
        }

        public void Login(object payload)
        {
            OnResultError += _OnResultError;
            OnResponseCodeError += _OnResponseCodeError;

            _isLogon = true;

            ApiPost("/v1/account/login", payload, _OnBearerResult);
        }

        private void Exchange()
        {
            OnResultError += _OnResultError;
            OnResponseCodeError += _OnResponseCodeError;

            _isLogon = false;

            var payload = new {};
            ApiPost("/v1/account/exchange", payload, _OnBearerResult);
        }

        private void _OnBearerResult(JObject loginResponse)
        {
            OnResultError -= _OnResultError;
            OnResponseCodeError -= _OnResponseCodeError;

            var bearer = (string)loginResponse["bearer"];
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(bearer).Payload;

            var exp = jwt.Exp.GetValueOrDefault(0);
            var iat = jwt.Iat.GetValueOrDefault(0);

            int offset = exp - iat;
            if (offset > 0)
            {
                // 仅当时间值有效时才做定期更换 token 的任务
                _exchangeOffset = Math.Max(0.01f, offset - 500f);
                _exchangeDelta = 0f;

                // 添加 HTTP 头
                headers["Authorization"] = $"Bearer {bearer}";
                BuildHeaders();

                _log.Debug($"setup jwt with offset {_exchangeOffset}");

                // 转为业务响应，通过登录事件送出
                if (_isLogon)
                {
                    jwt.TryGetValue("playerId", out object playerIdObj);
                    jwt.TryGetValue("sessionId", out object sessionIdObj);
                    jwt.TryGetValue("vendorId", out object vendorIdObj);
                    jwt.TryGetValue("codeId", out object codeIdObj);

                    string playerId = (string)playerIdObj;
                    string sessionId = (string)sessionIdObj;
                    int vendorId = Convert.ToInt32(vendorIdObj);
                    int codeId = Convert.ToInt32(codeIdObj);

                    LoginResult result = new LoginResult(playerId, sessionId, vendorId, codeId);

                    OnLogined?.Invoke(result);
                }
            }
            else
            {
                _log.Warn($"wrong of exp {exp} and iat {iat}");
                
                // 进行重试逻辑
                _ScheduleRetry();
            }

        }

        private void _OnResultError(int result)
        {
            _log.Warn($"result error {result}");

            // 执行重试逻辑
            _ScheduleRetry();
        }

        private void _OnResponseCodeError(int responseCode)
        {
            _log.Warn($"response code error {responseCode}");

            // 执行重试逻辑
            _ScheduleRetry();
        }

        private void _ScheduleRetry()
        {
            OnResultError -= _OnResultError;
            OnResponseCodeError -= _OnResponseCodeError;

        }
    }
}