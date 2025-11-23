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
        Task<bool> IsPlayerExist(Guid playerId);
        Task<int> RemovePlayer(Player player);
        Task<int> UpdatePlayer(Player player);
        Task<int> AddConnectionToPlayer(Player player, string connectionId);

    }
}
