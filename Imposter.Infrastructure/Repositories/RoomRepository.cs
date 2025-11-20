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
            Room? room = await _appDbContext.rooms.Include(r=>r.Players).FirstOrDefaultAsync(r=>r.RoomId == roomId);
            return room;
        }

        public async Task<ICollection<Room>> GetRooms()
        {
            List<Room> rooms = new List<Room>();
            rooms = await _appDbContext.rooms.ToListAsync();
            return rooms;

        }

        public async Task<bool> IsRoomExist(Guid roomId)
        {
            bool IsExist = await _appDbContext.rooms.AnyAsync(r => r.RoomId == roomId);
            return IsExist;
        }

        public async Task<int> UpdateRoom(Room room)
        {
            _appDbContext.rooms.Update(room);
            var res = await _appDbContext.SaveChangesAsync();
            return res;
            
        }
    }
}
