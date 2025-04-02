using FluentValidation;
using MuseoLibrary.ApplicationDomain.DTOs;
using MuseoLibrary.ApplicationDomain.Models;

namespace MuseoLibrary.Validations
{
    
    public class UserValidator : AbstractValidator<UserTrip>
    {
        public UserValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }

    public class StampValidator : AbstractValidator<Stamp>
    {
        public StampValidator()
        {
            RuleFor(s => s.id)
                .NotEmpty().WithMessage("ID is required.");

            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(s => s.Location)
                .NotNull().WithMessage("Location is required.");
        }
    }

    public class TripValidator : AbstractValidator<Trip>
    {
        public TripValidator()
        {
            RuleFor(t => t.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(t => t.Location)
                .NotEmpty().WithMessage("Location is required.");

            RuleFor(t => t.Images)
                .Must(images => images != null && images.Count > 0)
                .WithMessage("At least one image is required.");

            RuleFor(t => t.Date).NotNull();
            RuleFor(t => t.Description).NotEmpty();
            RuleFor(t => t.Confidence).NotNull();
            RuleFor(t => t.Tags).NotNull();
            RuleFor(t => t.DetectedObjects).NotNull();
            RuleFor(t => t.Categories).NotNull();
        }
    }

    public class TripDtoValidator : AbstractValidator<TripDto>
    {
        public TripDtoValidator()
        {
            RuleFor(t => t.UserId)
           .NotEmpty().WithMessage("User ID is required.");

            RuleFor(t => t.PlaceDescription)
                .NotEmpty().WithMessage("Location is required.");

        }
    }

    public class UserDtoValidator : AbstractValidator<UserDto> 
    {
        public UserDtoValidator()
        {
            RuleFor(u => u.UserName)
                .NotNull()
                .NotEmpty().WithMessage("Username is required.");
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
            RuleFor(u => u.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress().WithMessage("Email Adress required");
            RuleFor(u => u.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
        }
    }

    public class PasswordDtoValidator : AbstractValidator<PasswordDto>
    {
        public PasswordDtoValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("User ID is required.");
            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(p => p.OldPassword)
                .NotEmpty().WithMessage("Old password is required.");
            RuleFor(p => p.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
