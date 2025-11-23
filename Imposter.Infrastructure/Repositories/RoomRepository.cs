using Imposter.Core.Domain.Entities;
using Imposter.Core.RepositoriesContracts;
using Imposter.Infrastructure.Dbcontext;
using Microsoft.EntityFrameworkCore;

namespace Imposter.Infrastructure.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _appDbContext;
        public RoomRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> AddPlayerToRoom(Player player, Guid roomId)
        {
            var room = await GetRoomById(roomId);
            room.Players.Add(player);
            _appDbContext.rooms.Update(room);
            var res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> CreateRoom(Room room)
        {
            await _appDbContext.rooms.AddAsync(room);
            int res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> DeleteRoom(Room room)
        {
            _appDbContext.rooms.Remove(room);
            var res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<Room?> GetRoomById(Guid roomId)
        {
            Room? room = await _appDbContext.rooms.Include(r => r.Players).Include(r => r.Host).Include(r => r.SecretWord).FirstOrDefaultAsync(r=>r.RoomId == roomId);
            return room;
        }

        public async Task<List<Room>> GetRooms()
        {
            List<Room> rooms = new List<Room>();
            rooms = await _appDbContext.rooms.Include(r => r.Players).Include(r => r.SecretWord).ToListAsync();
            return rooms;

        }

        public async Task<bool> IsRoomExist(Guid roomId)
        {
            bool IsExist = await _appDbContext.rooms.AnyAsync(r => r.RoomId == roomId);
            return IsExist;
        }

        public async Task<int> RemovePlayerFromRoom(Player player, Guid roomId)
        {
            var room = await GetRoomById(roomId);
            room.Players.Remove(player);
            var res = await _appDbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> UpdateRoom(Room dto)   // dto coming from client
        {
            var existing = await GetRoomById(dto.RoomId.Value);
            if (existing == null) return 0;

            // copy only the fields the user is allowed to change
            existing.InGame = dto.InGame;
            existing.HostId = dto.HostId;
            existing.Stage = dto.Stage;
            existing.Category = dto.Category;
            
            existing.RoomId = dto.RoomId;

            _appDbContext.Update(existing);

            // … other editable fields …

            // timestamp is still the db value, so concurrency check succeeds
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
