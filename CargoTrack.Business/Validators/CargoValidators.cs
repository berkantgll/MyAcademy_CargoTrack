
using CargoTrack.DTO.CargoDtos;
using FluentValidation;

namespace CargoTrack.Business.Validators;

public class CreateCargoValidator : AbstractValidator<CreateCargoDto>
{
    public CreateCargoValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Gönderici seçilmelidir.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("Alıcı seçilmelidir.");

        RuleFor(x => x.OriginBranchId)
            .NotEmpty().WithMessage("Çıkış şubesi seçilmelidir.");

        RuleFor(x => x.DestinationBranchId)
            .NotEmpty().WithMessage("Varış şubesi seçilmelidir.");

        RuleFor(x => x.DeliveryAddressId)
            .NotEmpty().WithMessage("Teslimat adresi seçilmelidir.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Geçerli gönderi tipi seçilmelidir.");

        RuleFor(x => x.WeightKg)
            .GreaterThan(0).WithMessage("Ağırlık sıfırdan büyük olmalıdır.");

        RuleFor(x => x.WidthCm)
            .GreaterThan(0).WithMessage("En sıfırdan büyük olmalıdır.");

        RuleFor(x => x.HeightCm)
            .GreaterThan(0).WithMessage("Yükseklik sıfırdan büyük olmalıdır.");

        RuleFor(x => x.LengthCm)
            .GreaterThan(0).WithMessage("Boy sıfırdan büyük olmalıdır.");
    }
}

public class UpdateCargoValidator : AbstractValidator<UpdateCargoDto>
{
    public UpdateCargoValidator()
    {
        RuleFor(x => x.CargoId).NotEmpty();

        RuleFor(x => x.OriginBranchId).NotEmpty();
        RuleFor(x => x.DestinationBranchId).NotEmpty();
        RuleFor(x => x.DeliveryAddressId).NotEmpty();

        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.WeightKg).GreaterThan(0);
        RuleFor(x => x.WidthCm).GreaterThan(0);
        RuleFor(x => x.HeightCm).GreaterThan(0);
        RuleFor(x => x.LengthCm).GreaterThan(0);
    }
}
