namespace InternshipPortal.BL.DTOi.Internships
{
    public class InternshipResponseDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string FullDescription { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }
        public bool Remote { get; set; }

        public string Location { get; set; } = string.Empty;

        public System.DateTime PostedAt { get; set; }
        public System.DateTime? Deadline { get; set; }

        public int CompanyId { get; set; }
        public int CategoryId { get; set; }

        public string CompanyName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
