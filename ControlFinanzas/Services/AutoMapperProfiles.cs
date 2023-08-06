using AutoMapper;
using ControlFinanzas.Models;
using ControlFinanzas.Models.Entidades;

namespace ControlFinanzas.Services
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap();

        }
    }
}
