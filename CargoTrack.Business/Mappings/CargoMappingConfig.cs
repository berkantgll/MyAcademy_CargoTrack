
using Mapster;
using CargoTrack.Entity.Entities;
using CargoTrack.DTO.CargoDtos;

namespace CargoTrack.Business.Mappings;

public static class CargoMappingConfig
{
    public static void Register()
    {
       
        TypeAdapterConfig<Cargo, CargoDetailDto>
            .NewConfig()
            .Map(dest => dest.SenderFirstName,
                 src => src.Sender.FirstName)
            .Map(dest => dest.SenderLastName,
                 src => src.Sender.LastName)
            .Map(dest => dest.ReceiverFirstName,
                 src => src.Receiver.FirstName)
            .Map(dest => dest.ReceiverLastName,
                 src => src.Receiver.LastName)
            .Map(dest => dest.OriginBranchName,
                 src => src.OriginBranch.BranchName)
            .Map(dest => dest.DestinationBranchName,
                 src => src.DestinationBranch.BranchName)
            .Map(dest => dest.DeliveryCity,
                 src => src.DeliveryAddress.City)
            .Map(dest => dest.DeliveryDistrict,
                 src => src.DeliveryAddress.District)
            .Map(dest => dest.DeliveryNeighborhood,
                 src => src.DeliveryAddress.Neighborhood)
            .Map(dest => dest.DeliveryFullAddress,
                 src => src.DeliveryAddress.FullAddress)
            .Map(dest => dest.DeliveryAddressDescription,
                 src => src.DeliveryAddress.AddressDescription)
            .Map(dest => dest.CargoMovements,
                 src => src.CargoMovements);

       
        TypeAdapterConfig<Cargo, CargoListDto>
            .NewConfig()
            .Map(dest => dest.SenderFirstName,
                 src => src.Sender.FirstName)
            .Map(dest => dest.SenderLastName,
                 src => src.Sender.LastName)
            .Map(dest => dest.ReceiverFirstName,
                 src => src.Receiver.FirstName)
            .Map(dest => dest.ReceiverLastName,
                 src => src.Receiver.LastName)
            .Map(dest => dest.OriginBranchName,
                 src => src.OriginBranch.BranchName)
            .Map(dest => dest.DestinationBranchName,
                 src => src.DestinationBranch.BranchName);
    }
}
