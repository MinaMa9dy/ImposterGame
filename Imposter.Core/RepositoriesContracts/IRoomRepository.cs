using Imposter.Core.Domain.Entities;
using Imposter.Core.Domain.Enums;
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
        Task<bool> MakePlayerImposter(Guid playerId, Guid roomId);
        Task<int> NextStage(Guid roomId);
        Task<bool> SetSecretWord(Guid roomId, Guid secretWordId);
        Task StartGame(Guid roomId);
        Task<bool> SetCategory(Guid roomId, CategoryOptions category);
        Task<Player> GetRandomPlayerFromRoom(Guid roomId);
        Task<bool> IsAllPlayersReady(Guid roomId);
        Task ResetStateOfAllPlayersInRoom(Guid roomId);
        Task StopGame(Guid roomId);
    }
}
