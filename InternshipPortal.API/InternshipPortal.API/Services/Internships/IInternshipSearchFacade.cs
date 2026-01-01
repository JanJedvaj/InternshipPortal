namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// FACADE sučelje za kompleksnu logiku pretrage/prikaza oglasa.
    /// Controller vidi samo ovaj "jednostavan" API.
    /// </summary>
    public interface IInternshipSearchFacade
    {
        InternshipSearchResult Search(InternshipSearchCriteria criteria);
    }
}
