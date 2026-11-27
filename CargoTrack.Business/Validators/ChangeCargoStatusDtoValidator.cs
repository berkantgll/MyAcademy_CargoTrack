
using CargoTrack.DTO.CargoDtos;
using CargoTrack.Entity.Enums;
using FluentValidation;

namespace CargoTrack.Business.Validators;

public class ChangeCargoStatusDtoValidator
    : AbstractValidator<ChangeCargoStatusDto>
{
    public ChangeCargoStatusDtoValidator()
    {
        RuleFor(x => x.CargoId)
            .NotEmpty()
            .WithMessage("Kargo seçilmelidir.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Geçersiz kargo durumu.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Açıklama en fazla 500 karakter olabilir.");

        When(x => x.NewStatus == CargoStatus.Delivered, () =>
        {
            RuleFor(x => x.VerificationCode)
                .NotEmpty()
                .Matches("^[0-9]{6}$")
                .WithMessage(
                    "Teslimat kodu 6 rakamdan oluşmalıdır.");

            RuleFor(x => x.ReceivedName)
                .NotEmpty()
                .MaximumLength(120)
                .WithMessage(
                    "Teslim alan kişinin adı gereklidir.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage(
                    "Teslimatı yapan çalışan seçilmelidir.");
        });

        When(x => x.NewStatus == CargoStatus.DeliveryFailed, () =>
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(
                    "Başarısız teslimat için açıklama gereklidir.");
        });
    }
}
