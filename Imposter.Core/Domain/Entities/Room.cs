using Imposter.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imposter.Core.Domain.Entities
{
    public class Room
    {
        public Guid? RoomId { get; set; }
        public CategoryOptions? Category { get; set; } = CategoryOptions.General;
        public Guid? SecretWordId { get; set; }
        public SecretWord? SecretWord { get; set; }
        public Guid? HostId { get; set; }
        [ForeignKey(nameof(HostId))]
        public Player? Host { get; set; }
        public Guid? ImposterId { get; set; }
        [ForeignKey(nameof(ImposterId))]
        public Player? Imposter { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public bool InGame { get; set; }
        public int Stage { get; set; }
        public List<Connection> connections { get; set; } = new List<Connection>();


    }
}
