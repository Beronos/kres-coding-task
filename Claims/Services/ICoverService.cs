namespace Claims.Services
{

    public interface ICoverService
    {
        Task<IEnumerable<Cover>> GetAllCoversAsync();
        Task<Cover?> GetCoverAsync(string id);
        Task<Cover> CreateCoverAsync(Cover cover);
        Task DeleteCoverAsync(string id);
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
