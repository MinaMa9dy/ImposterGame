using Imposter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.RepositoriesContracts
{
    public interface IConnectionRepository
    {
        Task<int> AddConnectionToPlayer(Guid playerId, string connectionId);
        Task<int> RemoveConnectionFromPlayer(Guid playerId, string connectionId);
        Task<int> GetPlayerConnectionsCount(Guid playerId);
        Task<List<Connection>> GetPlayerConnections(Guid playerId);
        Task<bool> RemoveAllConnections(Guid playerId);
    }
}
