using AutoMapper;
using Panda.DTOs;
using Panda.Models;

namespace Panda.Profiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientDto>();
            CreateMap<PatientDto, Patient>();

            CreateMap<Patient, CreatePatientDto>();
            CreateMap<CreatePatientDto, Patient>();

            CreateMap<Patient, UpdatePatientDto>();
            CreateMap<UpdatePatientDto, Patient>();
        }
    }
}
