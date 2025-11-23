using Imposter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.RepositoriesContracts
{
    public interface IRoomRepository
    {
        Task<int> CreateRoom(Room room);
        Task<Room?> GetRoomById(Guid roomId);
        Task<bool> IsRoomExist(Guid roomId);
        Task<int> DeleteRoom(Room room);
        Task<List<Room>> GetRooms();
        Task<int> AddPlayerToRoom(Player player,Guid roomId);
        Task<int> RemovePlayerFromRoom(Player player, Guid roomId);
        Task<int> UpdateRoom(Room room);
    }
}
