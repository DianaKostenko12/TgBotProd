namespace TgBot.Models
{
    public class DietRegistrationModel
    {
        public long UserId { get; set; }
        public string FavoriteDiet { get; set; }
        public DietRegistrationModel(long userId, string diet)
        {
            UserId = userId;
            FavoriteDiet = diet;
        }
    }
}
