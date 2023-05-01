using Newtonsoft.Json;
using System.Threading.Tasks;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Networking
{
    public interface IApiClient
    {
        Task<Arena> GetArena();

        Task<ActiveBots> GetBots();

        Task<ActiveMessages> GetMessages();
    }

    public class ApiClient : IApiClient
    {
        private readonly string _baseUrl = "https://api.csharpwars.com";
        private Arena _arena;

        public Task<Arena> GetArena()
        {
            return Get<Arena>("arena/default");
        }

        public Task<ActiveBots> GetBots()
        {
            return Get<ActiveBots>("arena/default/bots");
        }

        public Task<ActiveMessages> GetMessages()
        {
            return Get<ActiveMessages>("arena/default/messages");
        }

        private async Task<TResult> Get<TResult>(string resource) where TResult : new()
        {
            UnityWebRequest web = UnityWebRequest.Get($"{_baseUrl}/{resource}");
            web.method = "GET";
            await web.SendWebRequest();

            if (web.result == UnityWebRequest.Result.ConnectionError || web.responseCode != 200)
            {
                Debug.LogError(web.error);
                web.Dispose();
                return default;
            }
            
            var arena = JsonConvert.DeserializeObject<TResult>(web.downloadHandler.text);
            web.Dispose();
            return arena;
        }
    }

    public static class UnityWebRequestExtension
    {
        public static TaskAwaiter<UnityWebRequest.Result> GetAwaiter(this UnityWebRequestAsyncOperation reqOp)
        {
            TaskCompletionSource<UnityWebRequest.Result> tsc = new();
            reqOp.completed += asyncOp => tsc.TrySetResult(reqOp.webRequest.result);

            if (reqOp.isDone)
                tsc.TrySetResult(reqOp.webRequest.result);

            return tsc.Task.GetAwaiter();
        }
    }
}