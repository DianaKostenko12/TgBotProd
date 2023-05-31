using Newtonsoft.Json;
using System.Text;
using TgBot.Models;

namespace TgBot.Services
{
    public class DietService
    {
        public static async Task AddToFavorites(DietRegistrationModel diet)
        {
            var httpclient = new HttpClient();
            string url = $"https://{AppConstants.Domain}/Diet/diet";

            string requestBody = JsonConvert.SerializeObject(diet);

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

        public static async Task AutoDeleteFavorites(long userId)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.DeleteAsync($"https://{AppConstants.Domain}/Diet/{userId}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Favourite diet deleted successfully.");
            }
            else
            {
                Console.WriteLine("An error occurred while deleting the favourite diet.");
            }
        }

        public static async Task<List<string>> GetAllDietById(long userId)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/Diet/diet/{userId}");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                List<string> diets = JsonConvert.DeserializeObject<List<string>>(responseContent);
                return diets;
            }
            else
            {
                return new List<string> { "You doesn't have any favorite diet" };
            }
        }

        public static async Task<bool> DeleteFavorites(long userId, int dietId)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.DeleteAsync($"https://{AppConstants.Domain}/Diet/{userId}/{dietId}");

            return response.IsSuccessStatusCode;
        }
    }
}
