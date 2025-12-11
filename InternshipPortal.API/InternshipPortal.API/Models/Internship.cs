namespace InternshipApi.Models
{
    public class Internship
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public bool IsFeatured { get; set; }
        public bool Remote { get; set; }
        public string Location { get; set; }
        public DateTime PostedAt { get; set; }
        public DateTime? Deadline { get; set; }

        public int CompanyId { get; set; }
        public int CategoryId { get; set; }
    }
}
