using Imposter.Core.Domain.Entities;
using Imposter.Core.RepositoriesContracts;
using Imposter.Infrastructure.Dbcontext;
using Microsoft.EntityFrameworkCore;


namespace Imposter.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _appDbContext;
        public PlayerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> AddConnectionToPlayer(Player player, string connectionId)
        {
            var ThePlayer = await GetPlayerById(player.PlayerId.Value);
            var Connection = new Connection
            {
                ConnectionId = connectionId,
                PlayerId = ThePlayer.PlayerId.Value
            };
            ThePlayer.Connections.Add(Connection);
            return await _appDbContext.SaveChangesAsync();

        }

        public async Task<int> AddPlayer(Player player)
        {
            await _appDbContext.players.AddAsync(player);
            var res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<Player?> GetPlayerById(Guid playerId)
        {
            Player? player = await _appDbContext.players.FirstOrDefaultAsync(p => p.PlayerId == playerId);
            return player;
        }

        public async Task<bool> IsPlayerExist(Guid playerId)
        {
            bool IsExist = await _appDbContext.players.AnyAsync(p => p.PlayerId == playerId);
            return IsExist;
        }

        
        public async Task<int> RemovePlayer(Player player)
        {
            _appDbContext.players.Remove(player);
            int res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> UpdatePlayer(Player player)
        {
            _appDbContext.players.Update(player);
            int res = await _appDbContext.SaveChangesAsync();
            return res;
        }
    }
}
