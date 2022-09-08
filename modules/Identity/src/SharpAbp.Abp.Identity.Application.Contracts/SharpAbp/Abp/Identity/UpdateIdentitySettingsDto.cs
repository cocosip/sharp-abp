using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharpAbp.Abp.Identity
{
    public class UpdateIdentitySettingsDto
    {
        public UpdateIdentityLockoutSettingsDto Lockout { get; set; }
        public UpdateIdentityPasswordSettingsDto Password { get; set; }
        public UpdateIdentitySignInSettingsDto SignIn { get; set; }
        public UpdateIdentityUserSettingsDto User { get; set; }
    }

    public class UpdateIdentityLockoutSettingsDto : IValidatableObject
    {
        [Required]
        public bool AllowedForNewUsers { get; set; }

        [Required]
        public int LockoutDuration { get; set; }

        [Required]
        public int MaxFailedAccessAttempts { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LockoutDuration <= 0)
            {
                yield return new ValidationResult("LockoutDuration should greater than 0.");
            }

            if (MaxFailedAccessAttempts < 1)
            {
                yield return new ValidationResult("MaxFailedAccessAttempts should greater than 1.");
            }

            yield break;
        }
    }

    public class UpdateIdentityPasswordSettingsDto : IValidatableObject
    {
        [Required]
        public bool RequireDigit { get; set; }

        [Required]
        public bool RequireLowercase { get; set; }

        [Required]
        public bool RequireNonAlphanumeric { get; set; }

        [Required]
        public bool RequireUppercase { get; set; }

        [Required]
        public int RequiredLength { get; set; }

        [Required]
        public int RequiredUniqueChars { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RequiredLength < 3)
            {
                yield return new ValidationResult("RequiredLength should greater than 3.");
            }

            if (RequiredUniqueChars < 1)
            {
                yield return new ValidationResult("RequiredUniqueChars should greater than 1.");
            }

            yield break;
        }
    }

    public class UpdateIdentitySignInSettingsDto
    {
        [Required]
        public bool EnablePhoneNumberConfirmation { get; set; }

        [Required]
        public bool RequireConfirmedEmail { get; set; }

        [Required]
        public bool RequireConfirmedPhoneNumber { get; set; }
    }

    public class UpdateIdentityUserSettingsDto
    {
        [Required]
        public bool IsEmailUpdateEnabled { get; set; }

        [Required]
        public bool IsUserNameUpdateEnabled { get; set; }
    }
}
