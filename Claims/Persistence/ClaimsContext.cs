using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Data
{
    public class ClaimsContext : DbContext
    {

        private DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        public ClaimsContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await Claims.ToListAsync();
        }

        public async Task<Claim> GetClaimAsync(string id)
        {
            return await Claims
                .Where(claim => claim.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task AddItemAsync(Claim item)
        {
            Claims.Add(item);
            await SaveChangesAsync();
        }

        public async Task DeleteItemAsync(string id)
        {
            var claim = await GetClaimAsync(id);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            return await Covers.ToListAsync();
        }

        public async Task<Cover> GetCoverAsync(string id)
        {
            return await Covers
                .Where(cover => cover.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task AddCoverAsync(Cover item)
        {
            Covers.Add(item);
            await SaveChangesAsync();
        }

        public async Task DeleteCoverAsync(string id)
        {
            Cover cover = await GetCoverAsync(id);
            if (cover is not null)
            {
                Covers.Remove(cover);
                await SaveChangesAsync();
            }
        }
    }
}
