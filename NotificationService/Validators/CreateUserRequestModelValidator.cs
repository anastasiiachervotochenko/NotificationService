using System.Text.RegularExpressions;
using FluentValidation;
using NotificationService.Domain.Contracts.Models.RequestModel;

namespace NotificationService.Validators;

public class CreateUserRequestModelValidator : AbstractValidator<CreateUserRequestModel>
{
    public CreateUserRequestModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .Matches(new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            .WithMessage("Invalid email address");
    }
}