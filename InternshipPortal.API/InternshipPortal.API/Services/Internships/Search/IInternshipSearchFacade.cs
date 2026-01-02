namespace InternshipPortal.API.Services.Internships.Search
{

    public interface IInternshipSearchFacade
    {
        InternshipSearchResult Search(InternshipSearchCriteria criteria);
    }
}
