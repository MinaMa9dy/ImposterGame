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

        public async Task<Player> CreatePlayer(string Name,int score ,bool state )
        {
            Player player = new Player()
            {
                PlayerId = Guid.NewGuid(),
                Name = Name,
                Score = score,
                State = state
            };
            await _appDbContext.players.AddAsync(player);
            return player;
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
        public async Task<Player?> GetPlayerByIdWithAll(Guid playerId)
        {
            Player? player = await _appDbContext.players.Include(p => p.Connections).Include(p => p.Room).FirstOrDefaultAsync(p => p.PlayerId == playerId);
            return player;
        }

        public async Task<bool> IsPlayerExist(Guid playerId)
        {
            bool IsExist = await _appDbContext.players.AnyAsync(p => p.PlayerId == playerId);
            return IsExist;
        }

        
        public async Task<int> RemovePlayer(Guid playerId)
        {
            var player = await GetPlayerById(playerId);
            _appDbContext.players.Remove(player);
            int res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> UpdatePlayer(Player player)
        {
            var existingPlayer = await GetPlayerById(player.PlayerId.Value);
            existingPlayer.Name = player.Name;
            existingPlayer.State = player.State;
            existingPlayer.Score = player.Score;
            //_appDbContext.players.Update(player);
            int res = await _appDbContext.SaveChangesAsync();
            return res;
        }
        public async Task<int> AddPlayerToRoom(Guid playerId, Guid roomId)
        {
            var player = await GetPlayerById(playerId);
            player.RoomId = roomId;
            _appDbContext.players.Update(player);
            return await _appDbContext.SaveChangesAsync();
        }
        public async Task<int> RemovePlayerFromRoom(Guid playerId, Guid roomId)
        {
            var player = await GetPlayerById(playerId);
            player.RoomId = null;
            _appDbContext.players.Update(player);
            return await _appDbContext.SaveChangesAsync();
        }
        public async Task<List<Player>> GetAllPlayers()
        {
            List<Player> players = new List<Player>();
            players = await _appDbContext.players.ToListAsync();
            return players;
        }

        public async Task<Player> ChangePlayerName(Guid playerId, string Name)
        {
            var player = await GetPlayerById(playerId);
            player.Name = Name;
            await _appDbContext.SaveChangesAsync();
            return player;
        }
        public async Task MakePlayerReady(Guid playerId)
        {
            var player = await GetPlayerById(playerId);
            player.State = true;
            await _appDbContext.SaveChangesAsync();
        }
        public async Task MakePlayerNotReady(Guid playerId)
        {
            var player = await GetPlayerById(playerId);
            player.State = false;
            await _appDbContext.SaveChangesAsync();
        }
        public async Task AddScore(Guid playerId)
        {
            var player = await GetPlayerById(playerId);
            player.Score += 1;
            await _appDbContext.SaveChangesAsync();
        }


    }
}
