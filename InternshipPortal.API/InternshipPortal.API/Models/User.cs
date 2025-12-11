namespace InternshipApi.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        // Za buduce : stavit cemo funkciju za passhword hash a ne obican string , u DB se spremaju Hashovi passworda nikad direktno string/pass
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "User"; //rola za sad user ubuduce Student/Poslodavac/Admin (moze enum) pa prema tome ide login odnosno landing nakon logina
    }
}
