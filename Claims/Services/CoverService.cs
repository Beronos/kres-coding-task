using Claims.Repositories;
using Claims.Validators;

namespace Claims.Services
{
    public class CoverService : ICoverService
    {
        private readonly ICoverRepository _coverRepository;
        private readonly IAuditer _auditer;
        public CoverService(ICoverRepository coverReposity, IAuditer auditer)
        {
            _coverRepository = coverReposity;
            _auditer = auditer;
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var multiplier = 1.3m;
            if (coverType == CoverType.Yacht)
            {
                multiplier = 1.1m;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = 1.2m;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = 1.5m;
            }

            var premiumPerDay = 1250 * multiplier;
            var insuranceLength = (endDate - startDate).TotalDays;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30) totalPremium += premiumPerDay;
                if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }

            return totalPremium;
        }

        public async Task<Cover> CreateCoverAsync(Cover cover)
        {
            CoverValidator.Validate(cover);

            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            await _coverRepository.AddAsync(cover);
            await _auditer.AuditCoverAsync(cover.Id, "POST");
            return cover;
        }

        public async Task DeleteCoverAsync(string id)
        {
            await _auditer.AuditCoverAsync(id, "DELETE");
            await _coverRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<Cover>> GetAllCoversAsync()
        {
            return _coverRepository.GetAllAsync();
        }

        public Task<Cover?> GetCoverAsync(string id)
        {
            return _coverRepository.GetByIdAsync(id);
        }
    }
}
