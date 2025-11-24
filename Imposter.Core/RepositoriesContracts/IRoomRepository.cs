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
        Task<Room?> GetRoomByIdWithAll(Guid roomId);
        Task<bool> IsRoomExist(Guid roomId);
        Task<int> DeleteRoom(Guid roomId);
        Task<List<Room>> GetRooms();
        Task<int> UpdateRoom(Room room);
        Task<bool> MakePlayerHost(Guid playerId, Guid roomId);
    }
}
