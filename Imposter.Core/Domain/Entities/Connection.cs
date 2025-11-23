using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.Domain.Entities
{
    public class Connection
    {
        public string? ConnectionId { get; set; }
        public Guid? PlayerId { get; set; }
    }
}
