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
        Task<ICollection<Room>> GetRooms();
        Task<Room?> GetRoom(Guid? roomId);
        Task<bool> AddPlayerToRoom(Player player, Guid roomId);
        Task<bool> RemovePlayerFromRoom(Player playerId, Guid roomId);
        Task StartGame(Guid roomId);
        Task ResetRoom(Guid roomId);
    }
}
