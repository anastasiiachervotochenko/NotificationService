using FluentValidation;
using NotificationService.Domain.Contracts.Models.RequestModel;

namespace NotificationService.Validators;

public class CreateNotificationRequestModelValidator : AbstractValidator<CreateNotificationRequestModel>
{
    public CreateNotificationRequestModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required")
            .MaximumLength(500).WithMessage("Body cannot exceed 500 characters");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required")
            .Must(BeValidGuid).WithMessage("Invalid Sender ID format");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("Receiver ID is required")
            .Must(BeValidGuid).WithMessage("Invalid Receiver ID format");

        RuleFor(x => x.Channel)
            .IsInEnum().WithMessage("Invalid Channel Type");
    }

    private bool BeValidGuid(string senderId)
    {
        return Guid.TryParse(senderId, out _);
    }
}