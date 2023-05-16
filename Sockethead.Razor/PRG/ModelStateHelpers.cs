﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Sockethead.Razor.PRG
{
    public static class ModelStateHelpers
    {
        private class ModelStateTransferValue
        {
            public string Key { get; init; }
            public string AttemptedValue { get; init; }
            public object RawValue { get; set; }
            public ICollection<string> ErrorMessages { get; init; } = new List<string>();
        }

        public static string SerializeModelState(ModelStateDictionary modelState)
        {
            return JsonConvert.SerializeObject(modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                }));
        }

        public static ModelStateDictionary DeserializeModelState(string serialisedErrorList)
        {
            var errorList = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>(serialisedErrorList);
            ModelStateDictionary modelState = new();

            foreach (ModelStateTransferValue item in errorList)
            {
                item.RawValue = item.RawValue is JArray jArray 
                    ? jArray.ToObject<object[]>() 
                    : item.RawValue;
                
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (string error in item.ErrorMessages)
                    modelState.AddModelError(item.Key, error);
            }

            return modelState;
        }
    }
}
