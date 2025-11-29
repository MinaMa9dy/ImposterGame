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
    public class RoomMapping:Profile
    {
        public RoomMapping()
        {
            CreateMap<Room, RoomViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Host.Name))
                .ForMember(dest => dest.Count,
                       opt => opt.MapFrom(src => src.Players.Count()));
                
        }
    }
}
