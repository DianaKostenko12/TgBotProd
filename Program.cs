using Telegram.Bot;
using Telegram.Bot.Types;
using TgBot.Models;
using TgBot.Services;

namespace TgBot
{
    internal class Program
    {
        static Dictionary<long, Command> waitPause = new();
        static HashSet<long> isResponsing = new();

        static void Main(string[] args)
        {
            var client = new TelegramBotClient("6085511743:AAF80CzrWXbQoAjurIfDqZCFy2iPa1dGWh0");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine($"an error occurred {arg2.Message}");
            return Task.CompletedTask;
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message?.Text == null)
                return;

            if (isResponsing.Contains(message.Chat.Id) && message.Text.StartsWith("/"))
                return;

            if (waitPause.ContainsKey(message.Chat.Id))
            {
                switch (waitPause[message.Chat.Id])
                {
                    case Command.Case1RegisterUser:
                        var user = UserRegistrationModel.CreateInstanse(message.Chat.Id, message.Text);

                        if (user != null)
                        {
                            await UserService.SaveInfoPersonToDb(user);
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Thanks, choose the option");

                            waitPause.Remove(message.Chat.Id);
                            isResponsing.Remove(message.Chat.Id);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Oops, you entered the data incorrectly, try again");
                        }
                        break;

                    case Command.Case2AskProduct:
                        string caloriesCount = await CalorieService.GetCaloriesByFood(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Ok, count of kcal of: {message.Text} - {caloriesCount} ");

                        waitPause.Remove(message.Chat.Id);
                        isResponsing.Remove(message.Chat.Id);
                        break;

                    case Command.Case3ReturnAdvice:
                        string GetAdvice = await CalorieService.GetAdviceAboutRation(message.Text);
                        await botClient.SendTextMessageAsync(message.Chat.Id, GetAdvice);

                        waitPause.Remove(message.Chat.Id);
                        isResponsing.Remove(message.Chat.Id);
                        break;

                    case Command.Case4AnswerAboutFavoriteDiet:
                        if (message.Text != "Yes")
                        {
                            await DietService.AutoDeleteFavorites(message.Chat.Id);
                        }
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Successfully completed");

                        waitPause.Remove(message.Chat.Id);
                        isResponsing.Remove(message.Chat.Id);
                        break;

                    case Command.Case5AnswerAboutDelete:
                        try
                        {
                            int numOfDiet = Convert.ToInt32(message.Text);
                            var delSuccessfully = await DietService.DeleteFavorites(message.Chat.Id, numOfDiet);

                            await botClient.SendTextMessageAsync(message.Chat.Id, delSuccessfully ? "Successfully completed" : "Someting go wrong :(");
                            if (delSuccessfully)
                            {
                                isResponsing.Remove(message.Chat.Id);
                                waitPause.Remove(message.Chat.Id);
                            }
                        }
                        catch (Exception)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Incorrectly entered data");
                        }
                        break;
                    case Command.Case6UpdateUserData:
                        var newuser = UserRegistrationModel.CreateInstanse(message.Chat.Id, message.Text);

                        if (newuser != null)
                        {
                            await UserService.UpdateUser(newuser);
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Thank you, data changed successfully");

                            waitPause.Remove(message.Chat.Id);
                            isResponsing.Remove(message.Chat.Id);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Oops, you entered the data incorrectly, try again");
                        }
                        break;
                    case Command.Case7BuildChart:
                        string[] values = message.Text.Split(',');

                        bool isValidArray = true;
                        foreach (string value in values)
                        {
                            if (!double.TryParse(value.Trim(), out _))
                            {
                                isValidArray = false;
                                break;
                            }
                        }

                        if (isValidArray)
                        {
                            var url = await UserService.BuildChartByWeights(message.Text);
                            await botClient.SendTextMessageAsync(message.Chat.Id, url);

                            waitPause.Remove(message.Chat.Id);
                            isResponsing.Remove(message.Chat.Id);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "The string is not valid.");
                        }
                        //if (!Regex.IsMatch(text, @"^\d+(, \d+){6}(?![, ])"))
                        //{
                        //    await botClient.SendTextMessageAsync(message.Chat.Id, $"Oops, you entered the data incorrectly, try again");
                        //}

                        //string[] weights = text.Split(',')
                        //    .Select(str => str.Trim())
                        //    .ToArray();
                        //double[] doubleArray = new double[weights.Length];

                        //for (int i = 0; i < weights.Length; i++)
                        //{
                        //    doubleArray[i] = Convert.ToDouble(weights[i]);
                        //}

                        break;
                    default:
                        break;
                }
            }
            else if (message.Text.ToLower().StartsWith("/start"))
            {
                isResponsing.Add(message.Chat.Id);

                if (!await UserService.IsUserRegistered(message.Chat.Id))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Hello, first enter data about yourself \n" +
                    "Input scheme - Your age, Your height(cm), Your weight(kg), Your sex(wowan/man)" +
                    "\nExample: 23, 187, 67, woman");
                    waitPause.Add(message.Chat.Id, Command.Case1RegisterUser);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Hi, you have already registered");
                    isResponsing.Remove(message.Chat.Id);
                }
            }
            else if (message.Text.ToLower().StartsWith("/caloriesbyfood"))
            {
                isResponsing.Add(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the product and quantity ( grams, pieces, kg, liters)");

                waitPause.Add(message.Chat.Id, Command.Case2AskProduct);
            }
            else if (message.Text.ToLower().StartsWith("/dietforweek"))
            {
                isResponsing.Add(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Wait a minute");

                string dietForUser = await CalorieService.GetDietForAWeekByUserId(message.Chat.Id);
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Your diet for a week: {dietForUser}");

                DietRegistrationModel model = new DietRegistrationModel(message.Chat.Id, dietForUser);
                await DietService.AddToFavorites(model);

                await botClient.SendTextMessageAsync(message.Chat.Id, $"Do you want to add this diet to your favorites? Yes/No");
                waitPause.Add(message.Chat.Id, Command.Case4AnswerAboutFavoriteDiet);
            }
            else if (message.Text.ToLower().StartsWith("/dailycalories"))
            {
                isResponsing.Add(message.Chat.Id);

                string CountOfKcalForADay = await CalorieService.GetCaloriesADayByUserId(message.Chat.Id);
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Your desired number of calories per day: {CountOfKcalForADay}");

                isResponsing.Remove(message.Chat.Id);
                //waitPause.Remove(message.Chat.Id); //?
            }
            else if (message.Text.ToLower().StartsWith("/adviceaboutration"))
            {
                isResponsing.Add(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat.Id, "Briefly write your diet for the day. For example: Breakfast - toast with avocado, egg and asparagus, Lunch - side dish with meat, Dinner - salad with chicken and fresh lettuce");

                waitPause.Add(message.Chat.Id, Command.Case3ReturnAdvice);
            }
            else if (message.Text.ToLower().StartsWith("/viewfavorites"))
            {
                isResponsing.Add(message.Chat.Id);

                var diets = await DietService.GetAllDietById(message.Chat.Id);

                foreach (var diet in diets)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{diet}");
                }

                isResponsing.Remove(message.Chat.Id);
            }
            else if (message.Text.ToLower().StartsWith("/removefromfavorites"))
            {
                isResponsing.Add(message.Chat.Id);

                var diets = await DietService.GetAllDietById(message.Chat.Id);

                for (int i = 0; i < diets.Count; i++)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Diet #{i + 1}: \n{diets[i]}");
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Enter num of diet, which you want to delete");
                waitPause.Add(message.Chat.Id, Command.Case5AnswerAboutDelete);
            }
            else if (message.Text.ToLower().StartsWith("/updatedata"))
            {
                isResponsing.Add(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat.Id, "To update your details,please enter data about yourself \n" +
                   "Input scheme - Your age, Your height(cm), Your weight(kg), Your sex(wowan/man)" +
                   "\nExample: 23, 187, 67, woman");
                waitPause.Add(message.Chat.Id, Command.Case6UpdateUserData);
            }
            else if (message.Text.ToLower().StartsWith("/buildchart"))
            {
                isResponsing.Add(message.Chat.Id);

                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter info about your weight for the last 7 days" +
                    "\nExample: 78, 98, 67, 56, 87, 54, 89");
                waitPause.Add(message.Chat.Id, Command.Case7BuildChart);
            }
        }
    }
}