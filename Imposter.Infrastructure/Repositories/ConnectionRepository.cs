using Imposter.Core.RepositoriesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imposter.Core.Domain.Entities;
using Imposter.Infrastructure.Dbcontext;
using Microsoft.EntityFrameworkCore;

namespace Imposter.Infrastructure.Repositories
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly AppDbContext _appDbContext;
        public ConnectionRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<int> AddConnectionToPlayer(Guid playerId, string connectionId)
        {
            var Connection = new Connection
            {
                ConnectionId = connectionId,
                PlayerId = playerId
            };
            await _appDbContext.connections.AddAsync(Connection);
            var result = await _appDbContext.SaveChangesAsync();
            return result;

        }
        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await _appDbContext.connections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
        }

        public async Task<int> RemoveConnectionFromPlayer(Guid playerId, string connectionId)
        {
            var connection = await GetConnection(connectionId);
            _appDbContext.connections.Remove(connection);
            int res = await _appDbContext.SaveChangesAsync();
            return res;

        }
        public async Task<int> GetPlayerConnectionsCount(Guid playerId)
        {
            return await _appDbContext.connections.CountAsync(p => p.PlayerId == playerId);
            
        }

        public async Task<List<Connection>> GetPlayerConnections(Guid playerId)
        {
            List<Connection> connections = new List<Connection>();
            connections = await _appDbContext.connections.ToListAsync();
            return connections;
        }

        public async Task<bool> RemoveAllConnectionsFromPlayer(Guid playerId)
        {
            var players = _appDbContext.connections.Where(r => r.PlayerId == playerId);
            _appDbContext.connections.RemoveRange(players);
            int res = await _appDbContext.SaveChangesAsync();
            return res > 0;
        }

        
    }
}
