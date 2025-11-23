using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.RepositoriesContracts;
using Imposter.Core.ServicesContracts;
using System.Reflection.Metadata.Ecma335;

namespace Imposter.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoomRepository _roomRepositories;
        private readonly IConnectionRepository _connectionRepository;
        private readonly IMapper _mapper;
        public GameService(IPlayerRepository playerRepository, IRoomRepository roomRepository, IConnectionRepository connectionRepository,IMapper mapper)
        {
            _playerRepository = playerRepository;
            _roomRepositories = roomRepository;
            _connectionRepository = connectionRepository;
            _mapper = mapper;
        }

        public async Task<int> AddConnectionToPlayer(Guid player, string connectionId)
        {
            var result = await _connectionRepository.AddConnectionToPlayer(player,connectionId);
            if (result == 0) return -1;
            else return await _connectionRepository.GetPlayerConnectionsCount(player);
        }

        public async Task<bool> AddPlayer(Player player)
        {
            var res = await _playerRepository.AddPlayer(player);
            return res > 0;

        }

        public async Task<bool> AddPlayerToRoom(Player player, Guid roomId)
        {
            if(player == null)
            {
                return false;
            }
            var room = await _roomRepositories.GetRoomById(roomId);
            if(room.Players.Any(p => p.PlayerId == player.PlayerId))
            {
                return true;
            }
            int res = await _roomRepositories.AddPlayerToRoom(player,roomId);
            return res > 0;


        }

        public async Task<Room> CreateRoom()
        {
            Room room = new Room
            {
                RoomId = Guid.NewGuid(),
                Players = new List<Player>(),
                InGame = false
            };
            await _roomRepositories.CreateRoom(room);
            return room;

        }

        public async Task<int> GetConnectionsCount(Guid playerId)
        {
            return await _connectionRepository.GetPlayerConnectionsCount(playerId);
        }

        public async Task<Player?> GetPlayer(Guid playerId)
        {
            Player? player = await _playerRepository.GetPlayerById(playerId);
            player.Connections = await _connectionRepository.GetPlayerConnections(playerId);
            return player;
        }

        public async Task<Room?> GetRoom(Guid? roomId)
        {
            if(roomId == null)
            {
                return null;
            }
            Room? room = await _roomRepositories.GetRoomById(roomId.Value);
            if (room == null)
            {
                return null;
            }
            return room;
        }

        public async Task<ICollection<Room>> GetRooms()
        {
            var rooms = await _roomRepositories.GetRooms();
            return rooms;
        }

        public async Task MakePlayerHost(Player player, Room room)
        {
            if(room.HostId == player.PlayerId)
            {
                return;
            }
            room.Host = player;
            room.HostId = player.PlayerId;
            await _roomRepositories.UpdateRoom(room);
        }

        public async Task RemoveAllRooms()
        {
            List<Room> rooms = await _roomRepositories.GetRooms();
            foreach(var room in rooms)
            {
                await RemoveRoom(room.RoomId);
                

            }
        }

        public Task<bool> RemoveConnection(string connectionId, Player player)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RemoveConnectionFromPlayer(Guid player, string connectionId)
        {
            var result = await _connectionRepository.RemoveConnectionFromPlayer(player, connectionId);
            if (result == 0) return -1;
            else return await _connectionRepository.GetPlayerConnectionsCount(player);
        }

        public async Task<bool> RemovePlayer(Guid player)
        {
            await _connectionRepository.RemoveAllConnections(player);
            var ThePlayer = await _playerRepository.GetPlayerById(player);
            return await _playerRepository.RemovePlayer(ThePlayer) > 0;
        }

        public async Task<bool> RemovePlayerFromRoom(Player player, Guid roomId)
        {
            var room = await _roomRepositories.GetRoomById(roomId);
            if (room == null)
            {
                return false;
            }
            int res = await _roomRepositories.RemovePlayerFromRoom(player,room.RoomId.Value);
            return res > 0;
        }

        public async Task<bool> RemoveRoom(Guid? roomId)
        {
            var room = await _roomRepositories.GetRoomById(roomId.Value);
            if (room == null)
            {
                return true;
            }
            List<Player> players = room.Players.ToList();
            foreach (var player in players)
            {
                await _connectionRepository.RemoveAllConnections(player.PlayerId.Value);
                await RemovePlayerFromRoom(player,roomId.Value);
                await _playerRepository.RemovePlayer(player);
            }
            var res = await _roomRepositories.DeleteRoom(room);
            return res > 0;

        }

        public Task ResetRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public Task StartGame(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdatePlayer(Player player)
        {
            return await _playerRepository.UpdatePlayer(player);
            
        }

        
    }
}
