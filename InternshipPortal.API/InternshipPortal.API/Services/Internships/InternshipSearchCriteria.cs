using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Internships
{

    public class InternshipSearchCriteria
    {
        // Naslov opis
        public string? Keyword { get; set; }

        // Filtri
        public int? CategoryId { get; set; }
        public int? CompanyId { get; set; }
        public bool? Remote { get; set; }
        public string? Location { get; set; }

       
        public bool OnlyActive { get; set; } = true;

       
        public string? SortBy { get; set; } = "date";

        // true = desc, false = asc
        public bool SortDescending { get; set; } = true;

        
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
