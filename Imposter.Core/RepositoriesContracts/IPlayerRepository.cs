using Imposter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.RepositoriesContracts
{
    public interface IPlayerRepository
    {
        Task<int> AddPlayer(Player player);
        Task<Player?> GetPlayerById(Guid playerId);
        Task<Player?> GetPlayerByIdWithAll(Guid playerId);
        Task<bool> IsPlayerExist(Guid playerId);
        Task<int> RemovePlayer(Guid playerId);
        Task<int> UpdatePlayer(Player player);
        Task<int> AddPlayerToRoom(Guid playerId, Guid roomId);
        Task<int> RemovePlayerFromRoom(Guid playerId, Guid roomId);
        Task<List<Player>> GetAllPlayers();
        Task<Player> ChangePlayerName(Guid playerId,string Name);
        Task<Player> CreatePlayer(string Name, int score, bool state);

    }
}
