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
        private readonly ISecretWordRepository _secretWordRepository;
        private readonly IMapper _mapper;
        public GameService(IPlayerRepository playerRepository, IRoomRepository roomRepository, IConnectionRepository connectionRepository,ISecretWordRepository secretWordRepository,IMapper mapper)
        {
            _playerRepository = playerRepository;
            _roomRepositories = roomRepository;
            _connectionRepository = connectionRepository;
            _secretWordRepository = secretWordRepository;
            _mapper = mapper;
        }

        public async Task<int> AddConnectionToPlayer(Guid? playerId, string? connectionId , Guid? roomId)
        {
            var result = await _connectionRepository.AddConnectionToPlayer(playerId.Value,connectionId,roomId.Value);
            if (result == 0) return -1;
            else return await _connectionRepository.GetPlayerConnectionsInRoomCount(playerId.Value,roomId.Value);
        }

        public async Task<bool> AddPlayer(Player? player)
        {
            var res = await _playerRepository.AddPlayer(player);
            return res > 0;

        }
        public Task<Player> CreatePlayer(string? Name, int score = 0, bool state= false)
        {
            return _playerRepository.CreatePlayer(Name, score, state);
        }
        public async Task<int> AddPlayerToRoom(Guid? playerId, Guid? roomId)
        {
            var player = await _playerRepository.GetPlayerById(playerId.Value);
            var room = await _roomRepositories.GetRoomById(roomId.Value);
            if (player == null || room is null)
            {
                return 0;
            }
            return await _playerRepository.AddPlayerToRoom(playerId.Value, roomId.Value);
            
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

        public async Task<int> GetConnectionsCount(Guid? playerId,Guid? roomId)
        {
            return await _connectionRepository.GetPlayerConnectionsInRoomCount(playerId.Value,roomId.Value);
        }

        public async Task<Player?> GetPlayer(Guid? playerId)
        {
            return await _playerRepository.GetPlayerByIdWithAll(playerId.Value);
        }

        public async Task<Room?> GetRoom(Guid? roomId)
        {
            if(roomId == null)
            {
                return null;
            }
            Room? room = await _roomRepositories.GetRoomByIdWithAll(roomId.Value);
            return room;
        }

        public async Task<ICollection<Room>> GetRooms()
        {
            var rooms = await _roomRepositories.GetRooms();
            return rooms;
        }

        public async Task<int> IsPlayerInRoom(Guid? playerId, Guid? roomId)
        {
            return await _connectionRepository.IsPlayerInRoom(playerId.Value, roomId.Value);
        }

        public async Task<bool> MakePlayerHost(Guid? playerId, Guid? roomId)
        {
            await _roomRepositories.MakePlayerHost(playerId.Value, roomId.Value);
            return await _playerRepository.AddPlayerToRoom(playerId.Value, roomId.Value) > 0;
        }

        public async Task<bool> RemoveAllRooms()
        {
            List<Room> rooms = await _roomRepositories.GetRooms();
            foreach(var room in rooms)
            {
                var TheRoom = await _roomRepositories.GetRoomByIdWithAll(room.RoomId.Value);
                var players = TheRoom.Players.ToList();

                foreach(var player in players)
                {
                    await _connectionRepository.RemoveAllConnectionsFromPlayer(player.PlayerId.Value);
                    await _playerRepository.RemovePlayerFromRoom(player.PlayerId.Value, room.RoomId.Value);
                    await _playerRepository.RemovePlayer(player.PlayerId.Value);
                }

                await _roomRepositories.DeleteRoom(room.RoomId.Value);
                

            }
            var playerss = await _playerRepository.GetAllPlayers();
            foreach(var player in playerss)
            {
                await _connectionRepository.RemoveAllConnectionsFromPlayer(player.PlayerId.Value);
                await _playerRepository.RemovePlayer(player.PlayerId.Value);
            }
            return true;
        }

        public Task<bool> RemoveConnection(string? connectionId, Guid? player)
        {
            throw new NotImplementedException();
        }

        public async Task<int> RemoveConnectionFromPlayer(Guid? playerId, string? connectionId)
        {
            var res = await _connectionRepository.RemoveConnectionFromPlayer(playerId.Value, connectionId);
            return res;
        }

        public async Task<bool> RemovePlayer(Guid? playerId)
        {
            bool res0 = await _connectionRepository.RemoveAllConnectionsFromPlayer(playerId.Value);
            int res1 = await _playerRepository.RemovePlayer(playerId.Value);
            return res0 == true && res1 > 0;
        }


        public async Task<bool> RemovePlayerFromRoom(Guid? playerId, Guid? roomId)
        {
            if(playerId == null || roomId == null)
            {
                return false;
            }
            var player = await  _playerRepository.GetPlayerById(playerId.Value);
            if(player.RoomId != roomId)
            {
                return true;
            }
            var res = await _playerRepository.RemovePlayerFromRoom(playerId.Value, roomId.Value);
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
                await _connectionRepository.RemoveAllConnectionsFromPlayer(player.PlayerId.Value);
                await _playerRepository.RemovePlayer(player.PlayerId.Value);
            }
            var res = await _roomRepositories.DeleteRoom(room.RoomId.Value);
            return res > 0;

        }

        public Task<bool> ResetRoom(Guid? roomId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StartGame(Guid? roomId)
        {
            var room = await _roomRepositories.GetRoomByIdWithAll(roomId.Value);
            Random random = new Random();
            int val = random.Next(0, room.Players.Count());
            var imposter = room.Players.ElementAt(val);
            await SetSecretWord(roomId.Value);
            await MakePlayerImposter(imposter.PlayerId, roomId.Value); // just to ensure host is set
            await _roomRepositories.StartGame(roomId.Value);
            return true;

        }

        public async Task<Player?> UpdateNamePlayer(Guid? playerId, string? Name)
        {
            var player = await _playerRepository.GetPlayerById(playerId.Value);
            if (player == null)
            {
                return null;
            }
            return await _playerRepository.ChangePlayerName(playerId.Value, Name);
        }

        public async Task<int> UpdatePlayer(Player player)
        {
            return await _playerRepository.UpdatePlayer(player);
            
        }

        public async Task<int> NextStage(Guid? roomId)
        {
            return await _roomRepositories.NextStage(roomId.Value);
        }
        public async Task SetSecretWord(Guid? roomId)
        {
            var SecretWord = await _secretWordRepository.PickRandomSecretWord();
            await _roomRepositories.SetSecretWord(roomId.Value, SecretWord.SecretWordId.Value);
        }

        public async Task<bool> MakePlayerImposter(Guid? playerId, Guid? roomId)
        {
            await _roomRepositories.MakePlayerImposter(playerId.Value, roomId.Value);
            return await _playerRepository.AddPlayerToRoom(playerId.Value, roomId.Value) > 0;
        }
    }
}
