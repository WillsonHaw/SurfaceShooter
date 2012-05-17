using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Newtonsoft.Json;
using RestSharp.Contrib;

namespace SS.Surface.XNA.Classes
{
    public class PubNubObservable : IObservable<string>
    {
        private const string Origin = "http://pubsub.pubnub.com/";
        private string _currentTimeToken = "0";
        private readonly IObservable<string> _observable;

        public PubNubObservable(string subscriberKey, string channel)
        {
            _observable = Observable.Create<string>(obs => Request(subscriberKey, channel, obs))
                                   .Repeat().Publish().RefCount();
        }

        private IDisposable Request(string subscriberKey, string channel, IObserver<string> obs)
        {
            var urlComponents = new[] { "subscribe", subscriberKey, channel, "0", _currentTimeToken }
                                   .Select(HttpUtility.UrlEncode);
            var url = Origin + string.Join("/", urlComponents);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 200000;
            request.ReadWriteTimeout = 200000;

            return Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                             .Select(HandleResponse)
                             .Subscribe(obs);
        }

        private string HandleResponse(WebResponse response)
        {
            List<object> deserialized;
            var serializer = new JsonSerializer();
            var responseStream = response.GetResponseStream();
            if (responseStream == null || !responseStream.CanRead) return null;
            using (var st = new StreamReader(responseStream))
            {
                deserialized = serializer.Deserialize<List<object>>(new JsonTextReader(st));
            }
            if (deserialized[1].ToString().Length > 0)
                _currentTimeToken = deserialized[1].ToString();
            return deserialized[0].ToString();
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _observable.Subscribe(observer);
        }
        
        public static void Publish(string channel, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var urlComponents = new[]
                                    {
                                        "publish", 
                                        Constants.PUBLISH_KEY,
                                        Constants.SUBSCRIBE_KEY,
                                        "0",
                                        channel,
                                        "0",
                                        json
                                    };
            var url = Origin + string.Join("/", urlComponents.Select(HttpUtility.UrlEncode));
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 200000;
            request.ReadWriteTimeout = 200000;
            var response = request.GetResponse();
            response.Close();
        }
    }
}
