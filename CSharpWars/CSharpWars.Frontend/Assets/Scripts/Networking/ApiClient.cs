using System.Threading.Tasks;
using Assets.Scripts.Model;
using RestSharp;

namespace Assets.Scripts.Networking
{
    public interface IApiClient
    {
        Task<Arena> GetArena();

        Task<ActiveBots> GetBots();
    }

    public class ApiClient : IApiClient
    {
        //private readonly string _baseUrl = "http://localhost:5133";
        private readonly string _baseUrl = "http://api.csharpwars.com";

        public Task<Arena> GetArena()
        {
            return Get<Arena>("arena/default");
        }

        public Task<ActiveBots> GetBots()
        {
            return Get<ActiveBots>("arena/default/bots");
        }

        private async Task<TResult> Get<TResult>(string resource) where TResult : new()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest(resource, Method.GET);
            var response = await client.ExecuteTaskAsync<TResult>(request);
            return response.Data;
        }
    }
}