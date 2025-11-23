using Imposter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.ServicesContracts
{
    public interface IGameService
    {
        
        Task<Room> CreateRoom();
        Task<bool> AddPlayer(Player player);
        Task<int> UpdatePlayer(Player player);
        Task<bool> RemovePlayer(Guid player);
        Task<ICollection<Room>> GetRooms();
        Task MakePlayerHost(Player player, Room room);
        Task<Room?> GetRoom(Guid? roomId);
        Task<bool> RemoveRoom(Guid? roomId);
        Task<bool> AddPlayerToRoom(Player player, Guid roomId);
        Task<bool> RemovePlayerFromRoom(Player playerId, Guid roomId);
        Task StartGame(Guid roomId);
        Task ResetRoom(Guid roomId);
        Task RemoveAllRooms();
        Task<Player?> GetPlayer(Guid Player);
        Task<int> AddConnectionToPlayer(Guid player, string connectionId);
        Task<int> RemoveConnectionFromPlayer(Guid player, string connectionId);
        Task<int> GetConnectionsCount(Guid playerId);
    }
}
