using Newtonsoft.Json;
using System.Text;
using TgBot.Models;


namespace TgBot.Services
{
    public class UserService
    {
        public static async Task SaveInfoPersonToDb(UserRegistrationModel user)
        {
            var httpclient = new HttpClient();
            string url = $"https://{AppConstants.Domain}/Auth";

            string requestBody = JsonConvert.SerializeObject(user);

            StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpclient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Successful response: " + responseContent);
            }
            else
            {
                Console.WriteLine("An error occurred while executing the request. Status code: " + response.StatusCode);
            }
        }

        public static async Task<bool> IsUserRegistered(long chatId)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/Auth/{chatId}");

            if (response.IsSuccessStatusCode)
            {
                bool isUserRegistered = Convert.ToBoolean(await response.Content.ReadAsStringAsync());

                return isUserRegistered;
            }

            return false;
        }

        public static async Task UpdateUser(UserRegistrationModel user)
        {
            var httpclient = new HttpClient();

            string url = $"https://{AppConstants.Domain}/Auth/update";

            string requestBody = JsonConvert.SerializeObject(user);

            StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpclient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Successful response: " + responseContent);
            }
            else
            {
                Console.WriteLine("An error occurred while executing the request. Status code:" + response.StatusCode);
            }
        }

        public static async Task<string> BuildChartByWeights(string weights)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/Auth/weight?weights={weights}");

            if (response.IsSuccessStatusCode)
            {
                string chartUrl = await response.Content.ReadAsStringAsync();

                return chartUrl;
            }
            else
            {
                return "Error: " + response.StatusCode;
            }
        }
    }
}
