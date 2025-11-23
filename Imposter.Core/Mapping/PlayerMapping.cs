using AutoMapper;
using Imposter.Core.Domain.Entities;
using Imposter.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.Mapping
{
    public class PlayerMapping : Profile
    {
        public PlayerMapping()
        {
            CreateMap<Player, PlayerViewModel>();
        }
    }
}
