using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Infrastructure
{
    public class DateTimeModelBinder : IModelBinder
    {
        private readonly IModelBinder baseBinder = new SimpleTypeModelBinder(typeof(DateTime));

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;

                DateTime? dateTime = null;

                if (!string.IsNullOrEmpty(valueAsString))
                {
                    dateTime = DateTime.ParseExact(valueAsString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                }

                bindingContext.Result = ModelBindingResult.Success(dateTime);

                return Task.CompletedTask;
            }

            return baseBinder.BindModelAsync(bindingContext);
        }
    }
}
