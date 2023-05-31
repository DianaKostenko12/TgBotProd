using System.Text.RegularExpressions;

namespace TgBot.Models
{
    public class UserRegistrationModel
    {
        public long Id { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Sex { get; set; }

        public static UserRegistrationModel CreateInstanse(long chatId, string text)
        {
            if (!Regex.IsMatch(text, @"^\d+, \d+, \d+, .+$"))
            {
                return null;
            }

            string[] props = text.Split(',')
                .Select(str => str.Trim())
                .ToArray();

            var correctAge = Convert.ToInt32(props[0]) <= 122;
            var correctHeight = Convert.ToDouble(props[1]) <= 272.00;
            var correctWeight = Convert.ToDouble(props[2]) <= 635.00;
            var correctSex = props[3] == "man" || props[3] == "woman";
            if (!correctAge || !correctHeight || !correctWeight || !correctSex)
            {
                return null;
            }

            return new UserRegistrationModel()
            {
                Id = chatId,
                Age = Convert.ToInt32(props[0]),
                Height = Convert.ToDouble(props[1]),
                Weight = Convert.ToDouble(props[2]),
                Sex = props[3]
            };
        }
    }
}
