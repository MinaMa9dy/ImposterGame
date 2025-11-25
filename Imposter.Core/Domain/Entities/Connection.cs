using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.Domain.Entities
{
    public class Connection
    {
        public string? ConnectionId { get; set; }
        public Guid? playerId { get; set; }
        [ForeignKey(nameof(playerId))]
        public Player? Player { get; set; }
        public Guid? roomId { get; set; }
        [ForeignKey(nameof(roomId))]
        public Room? Room { get; set; }
    }
}
