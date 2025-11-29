using Imposter.Core.Domain.Entities;
using Imposter.Core.Domain.Enums;
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
        Task<bool> AddPlayer(Player? player);
        Task<Player> CreatePlayer(string? Name, int score = 0, bool state = false);
        Task<int> IsPlayerInRoom(Guid? playerId, Guid? roomId);
        Task<bool> RemovePlayer(Guid? playerId);
        Task<Player?> UpdateNamePlayer(Guid? playerId, string? Name);
        Task<ICollection<Room>> GetRooms();
        Task<bool> MakePlayerHost(Guid? playerId, Guid? roomId);
        Task<bool> MakePlayerImposter(Guid? playerId, Guid? roomId);
        Task<Room?> GetRoom(Guid? roomId);
        Task<bool> RemoveRoom(Guid? roomId);
        Task<int> AddPlayerToRoom(Guid? playerId, Guid? roomId);
        Task<bool> RemovePlayerFromRoom(Guid? playerId, Guid? roomId);
        Task<bool> StartGame(Guid? roomId);
        Task<int> NextStage(Guid? roomId);
        Task<bool> ResetRoom(Guid? roomId);
        Task<bool> RemoveAllRooms();
        Task<Player?> GetPlayer(Guid? Player);
        Task<int> AddConnectionToPlayer(Guid? player, string? connectionId, Guid? roomId);
        Task<int> RemoveConnection(string? connectionId);
        Task<int> GetConnectionsCount(Guid? playerId, Guid? roomId);
        Task SetSecretWord(Guid? roomId);
        Task SetCategoryForRoom(Guid? roomId, CategoryOptions? category);
        Task MakePlayerReady(Guid? playerId);
        Task<bool> IsAllPlayersReady(Guid? roomId);
        Task ResetStateOfAllPlayersInRoom(Guid? roomId);
        Task MakePlayerNotReady(Guid? playerId);
        Task<Connection> GetConnection(string? connectionId);
        Task AddScore(Guid? playerId);
        Task StopGame(Guid? roomId);

    }
}
