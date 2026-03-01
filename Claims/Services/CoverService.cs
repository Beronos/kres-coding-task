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
            const decimal BaseDayRate = 1250;

            var premiumPerDay = coverType switch
            {
                CoverType.Yacht => BaseDayRate * 1.1m,
                CoverType.PassengerShip => BaseDayRate * 1.2m,
                CoverType.Tanker => BaseDayRate * 1.5m,
                _ => BaseDayRate * 1.3m
            };

            var totalDays = (int)(endDate - startDate).TotalDays;

            var firstBracketNrOfDays = Math.Min(totalDays, 30);
            var secondBracketNrOfDays = Math.Max(0, Math.Min(totalDays - 30, 150));
            var thirdBracketNrOfDays = Math.Max(0, totalDays - 180);

            var secondBracketMultiplier = coverType == CoverType.Yacht ? 0.95m : 0.98m;
            var thirdBracketMultiplier  = coverType == CoverType.Yacht ? 0.92m : 0.97m;

            return premiumPerDay * firstBracketNrOfDays
                 + premiumPerDay * secondBracketNrOfDays * secondBracketMultiplier
                 + premiumPerDay * thirdBracketNrOfDays * thirdBracketMultiplier;
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
