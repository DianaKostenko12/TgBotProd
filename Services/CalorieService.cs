namespace TgBot.Services
{
    public class CalorieService
    {
        public static async Task<string> GetCaloriesByFood(string food)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/calorie?food={food}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                string stringWithoutNewLine = result.Replace("\n", "");
                return stringWithoutNewLine;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }
        public static async Task<string> GetDietForAWeekByUserId(long userId)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/calorie/ration/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                string stringWithoutNewLine = result.Replace("\n", "\r\n");
                return stringWithoutNewLine;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }

        public static async Task<string> GetCaloriesADayByUserId(long userId)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/calorie/count/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }

        public static async Task<string> GetAdviceAboutRation(string ration)
        {
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync($"https://{AppConstants.Domain}/calorie/advice?ration={ration}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return $"Error: {response.StatusCode}";
            }
        }
    }
}
