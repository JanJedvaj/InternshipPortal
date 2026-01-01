using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// Parametri pretrage, filtriranja i sortiranja oglasa (Internship).
    /// Ovo se puni iz query stringa na /api/Internships/search.
    /// </summary>
    public class InternshipSearchCriteria
    {
        // Opći tekstualni upit (naslov, opis…)
        public string? Keyword { get; set; }

        // Filtri
        public int? CategoryId { get; set; }
        public int? CompanyId { get; set; }
        public bool? Remote { get; set; }
        public string? Location { get; set; }

        // Samo aktivni oglasi (Deadline >= danas ili null)
        public bool OnlyActive { get; set; } = true;

        // Sortiranje: "date", "deadline", "title"
        public string? SortBy { get; set; } = "date";

        // true = desc, false = asc
        public bool SortDescending { get; set; } = true;

        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
