using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.RepositoriesContracts;
using Imposter.Core.ServicesContracts;

namespace Imposter.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoomRepository _roomRepositories;
        private readonly IMapper _mapper;
        public GameService(IPlayerRepository playerRepository,IRoomRepository roomRepository,IMapper mapper)
        {
            _playerRepository = playerRepository;
            _roomRepositories = roomRepository;
            _mapper = mapper;
        }
        

        public async Task<bool> AddPlayerToRoom(Player player, Guid roomId)
        {
            if(player == null)
            {
                return false;
            }
            var room = await _roomRepositories.GetRoomById(roomId);
            room.Players.Add(player);
            int res = await _roomRepositories.UpdateRoom(room);
            return res > 0;


        }

        public Task<Room> CreateRoom()
        {
            throw new NotImplementedException();
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

        public Task<bool> RemoveConnection(string connectionId, Player player)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemovePlayerFromRoom(Player player, Guid roomId)
        {
            throw new NotImplementedException();
        }

        public Task ResetRoom(Guid roomId)
        {
            throw new NotImplementedException();
        }

        public Task StartGame(Guid roomId)
        {
            throw new NotImplementedException();
        }
    }
}
