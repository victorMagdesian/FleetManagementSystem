using AutoMapper;
using FleetManager.Application.DTOs;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Enums;

namespace FleetManager.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Vehicle mappings
        CreateMap<Vehicle, VehicleResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateVehicleRequest, Vehicle>()
            .ConstructUsing(src => new Vehicle(src.Plate, src.Model, src.Year, src.Mileage, null));

        // Driver mappings
        CreateMap<Driver, DriverResponse>();

        CreateMap<CreateDriverRequest, Driver>()
            .ConstructUsing(src => new Driver(src.Name, src.LicenseNumber, src.Phone));

        // MaintenanceRecord mappings
        CreateMap<MaintenanceRecord, MaintenanceRecordResponse>();

        CreateMap<CreateMaintenanceRecordRequest, MaintenanceRecord>()
            .ConstructUsing(src => new MaintenanceRecord(src.VehicleId, src.Date, src.Description, src.Cost));

        // Trip mappings
        CreateMap<Trip, TripResponse>();

        CreateMap<StartTripRequest, Trip>()
            .ConstructUsing(src => new Trip(src.VehicleId, src.DriverId, src.Route, DateTime.UtcNow));
    }
}
