using FluentValidation;

namespace Alertmanager.Receiver.AzureDevOps;

public class SettingsValidator : AbstractValidator<Settings>
{
    public SettingsValidator()
    {
        RuleFor(x => x.Organization).NotEmpty();
        RuleFor(x => x.Project).NotEmpty();
        RuleFor(x => x.PAT).NotEmpty();

        RuleFor(x => x.NewWorkItemFields).NotEmpty();
        RuleForEach(x => x.NewWorkItemFields).SetValidator(new FieldValidator());

        RuleFor(x => x.ResolvedWorkItemFields).NotEmpty();
        RuleForEach(x => x.ResolvedWorkItemFields).SetValidator(new FieldValidator());
    }
}

public class FieldValidator : AbstractValidator<Field>
{
    public FieldValidator()
    {
        RuleFor(x => x.ReferenceName).NotEmpty();

        // Format is required when DirectValue is not set
        RuleFor(x => x.Format).NotEmpty().When(x => string.IsNullOrEmpty(x.DirectValue));

        // JSONPaths is required when Format is set
        RuleFor(x => x.JSONPaths).NotEmpty().When(x => !string.IsNullOrEmpty(x.Format));
        RuleForEach(x => x.JSONPaths).NotEmpty();

        // DirectValue is required when Format is not set
        RuleFor(x => x.DirectValue).NotEmpty().When(x => string.IsNullOrEmpty(x.Format));
    }
}
