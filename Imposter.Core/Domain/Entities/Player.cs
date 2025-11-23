using System.ComponentModel.DataAnnotations.Schema;

namespace Imposter.Core.Domain.Entities
{
    public class Player
    {
        public Guid? PlayerId { get; set; }
        public string? Name { get; set; }
        public int Score { get; set; }
        public bool State { get; set; }
        public Guid? RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }
        public List<Connection> Connections { get; set; } = new List<Connection>();
    }
}
