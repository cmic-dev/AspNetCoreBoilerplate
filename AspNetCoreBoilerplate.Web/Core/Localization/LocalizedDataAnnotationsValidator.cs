using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Web.Core.Localization;

public class LocalizedDataAnnotationsValidator : ComponentBase, IAsyncDisposable
{
    private ValidationMessageStore? _messageStore;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    [Inject]
    private MudLocalizer Localizer { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(LocalizedDataAnnotationsValidator)} requires a cascading parameter of type {nameof(EditContext)}. " +
                $"For example, you can use {nameof(LocalizedDataAnnotationsValidator)} inside an EditForm.");
        }

        _messageStore = new ValidationMessageStore(CurrentEditContext);

        // Hook into validation events
        CurrentEditContext.OnValidationRequested += ValidateModel;
        CurrentEditContext.OnFieldChanged += ValidateField;
    }

    private void ValidateModel(object? sender, ValidationRequestedEventArgs e)
    {
        _messageStore?.Clear();

        var validationContext = new ValidationContext(CurrentEditContext!.Model);
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateObject(
            CurrentEditContext.Model,
            validationContext,
            validationResults,
            validateAllProperties: true);

        foreach (var validationResult in validationResults)
        {
            if (validationResult.MemberNames.Any())
            {
                // FIX: Changed from validationResults.SelectMany to validationResult.MemberNames
                foreach (var memberName in validationResult.MemberNames)
                {
                    var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, memberName);

                    // Get validation attribute to extract parameters
                    var property = CurrentEditContext.Model.GetType().GetProperty(memberName);
                    var validationAttributes = property?.GetCustomAttributes(typeof(ValidationAttribute), false)
                        .Cast<ValidationAttribute>();

                    var matchingAttribute = validationAttributes?.FirstOrDefault(attr =>
                        attr.ErrorMessage == validationResult.ErrorMessage);

                    // Extract parameters for StringLength, Range, etc.
                    var parameters = ExtractValidationParameters(matchingAttribute);

                    var localizedMessage = LocalizeMessage(validationResult.ErrorMessage, memberName, parameters);
                    _messageStore?.Add(fieldIdentifier, localizedMessage);
                }
            }
            else
            {
                // Model-level validation error
                var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, string.Empty);
                var localizedMessage = LocalizeMessage(validationResult.ErrorMessage, null);
                _messageStore?.Add(fieldIdentifier, localizedMessage);
            }
        }

        CurrentEditContext.NotifyValidationStateChanged();
    }

    private void ValidateField(object? sender, FieldChangedEventArgs e)
    {
        var fieldIdentifier = e.FieldIdentifier;

        _messageStore?.Clear(fieldIdentifier);

        var validationContext = new ValidationContext(fieldIdentifier.Model)
        {
            MemberName = fieldIdentifier.FieldName
        };

        var validationResults = new List<ValidationResult>();

        var propertyInfo = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName);
        if (propertyInfo != null)
        {
            var propertyValue = propertyInfo.GetValue(fieldIdentifier.Model);

            Validator.TryValidateProperty(
                propertyValue,
                validationContext,
                validationResults);

            foreach (var validationResult in validationResults)
            {
                var property = fieldIdentifier.Model.GetType().GetProperty(fieldIdentifier.FieldName);
                var validationAttributes = property?.GetCustomAttributes(typeof(ValidationAttribute), false)
                    .Cast<ValidationAttribute>();

                var matchingAttribute = validationAttributes?.FirstOrDefault(attr =>
                    attr.ErrorMessage == validationResult.ErrorMessage);

                var parameters = ExtractValidationParameters(matchingAttribute);

                var localizedMessage = LocalizeMessage(validationResult.ErrorMessage, fieldIdentifier.FieldName, parameters);
                _messageStore?.Add(fieldIdentifier, localizedMessage);
            }
        }

        CurrentEditContext?.NotifyValidationStateChanged();
    }

    private string LocalizeMessage(string? message, string? fieldName = null, params object[] args)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;

        // Try to get localized version
        var localizedString = Localizer[message];

        // If the key doesn't exist in resources, return original message
        if (localizedString.ResourceNotFound)
            return message;

        // If we have a field name and the message contains placeholders
        if (!string.IsNullOrEmpty(fieldName) && localizedString.Value.Contains("{0}"))
        {
            // Get the localized field name
            var displayAttribute = CurrentEditContext?.Model.GetType()
                .GetProperty(fieldName)?
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            var localizedFieldName = displayAttribute?.Name != null
                ? Localizer[displayAttribute.Name].Value
                : fieldName;

            // Combine field name with any additional arguments
            var allArgs = new object[] { localizedFieldName }.Concat(args).ToArray();

            return string.Format(localizedString.Value, allArgs);
        }

        // If there are arguments but no field name
        if (args.Length > 0)
        {
            return string.Format(localizedString.Value, args);
        }

        return localizedString.Value;
    }

    private object[] ExtractValidationParameters(ValidationAttribute? attribute)
    {
        if (attribute == null)
            return Array.Empty<object>();

        return attribute switch
        {
            StringLengthAttribute stringLength => new object[] { stringLength.MaximumLength, stringLength.MinimumLength },
            RangeAttribute range => new object[] { range.Minimum, range.Maximum },
            MinLengthAttribute minLength => new object[] { minLength.Length },
            MaxLengthAttribute maxLength => new object[] { maxLength.Length },
            _ => Array.Empty<object>()
        };
    }

    public ValueTask DisposeAsync()
    {
        if (CurrentEditContext != null)
        {
            CurrentEditContext.OnValidationRequested -= ValidateModel;
            CurrentEditContext.OnFieldChanged -= ValidateField;
        }
        return ValueTask.CompletedTask;
    }
}
