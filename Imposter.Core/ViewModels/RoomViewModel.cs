using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.ViewModels
{
    public class RoomViewModel
    {
        public Guid? roomId { get; set; }
        public bool InGame { get; set; }
    }
}
