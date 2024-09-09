using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthenticationBase.Infrastructure.Binders.Authentication;

public sealed class SignInModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string? modelName = bindingContext.ModelName;
        ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}