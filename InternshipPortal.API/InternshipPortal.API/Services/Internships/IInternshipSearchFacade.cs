namespace InternshipPortal.API.Services.Internships
{
   
    public interface IInternshipSearchFacade
    {
        InternshipSearchResult Search(InternshipSearchCriteria criteria);
    }
}
