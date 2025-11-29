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
        Task<int> AddConnectionToPlayer(Guid playerId, string connectionId,Guid roomId);
        Task<int> RemoveConnection(string connectionId);
        Task<int> GetPlayerConnectionsInRoomCount(Guid playerId, Guid roomId);
        Task<List<Connection>> GetPlayerConnections(Guid playerId);
        Task<bool> RemoveAllConnectionsFromPlayer(Guid playerId);
        Task<int> IsPlayerInRoom(Guid playerId, Guid roomId);
        Task<Connection> GetConnection(string connectionId);
    }
}
