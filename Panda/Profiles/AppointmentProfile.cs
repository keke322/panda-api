using AutoMapper;
using Panda.DTOs;
using Panda.Models;

namespace Panda.Profiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentDto>();
            CreateMap<AppointmentDto, Appointment>();

            CreateMap<Appointment, CreateAppointmentDto>();
            CreateMap<CreateAppointmentDto, Appointment>();

            CreateMap<Appointment, UpdateAppointmentDto>();
            CreateMap<UpdateAppointmentDto, Appointment>();
        }
    }
}
