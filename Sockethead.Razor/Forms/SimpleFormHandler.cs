using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sockethead.Razor.Alert.Extensions;

namespace Sockethead.Razor.Forms
{
    public class SimpleFormHandler
    {
        private ModelStateDictionary ModelState { get; }

        private Func<IActionResult> _onSuccessResult, _onErrorResult;

        private Func<IActionResult, IActionResult> _onSuccess, _onError;
        
        internal SimpleFormHandler(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }

        public SimpleFormHandler Validate(Action<Validator> action)
        {
            Validator validator = new();
            action(validator);
            if (!validator.IsValid)
                ModelState.AddModelError(validator.Key, validator.ErrorMessage);
            return this;
        }

        public SimpleFormHandler Validate(bool condition, string key, string errorMessage)
        {
            if (condition)
                ModelState.AddModelError(key: key, errorMessage: errorMessage);
            return this;
        }
        
        public SimpleFormHandler OnError(Func<IActionResult, IActionResult> action)
        {
            _onError = action;
            return this;
        }

        public SimpleFormHandler OnError(string errorMessage)
        {
            _onError = result => result.Error(errorMessage);
            return this;
        }
        
        public SimpleFormHandler OnSuccess(Func<IActionResult, IActionResult> action)
        {
            _onSuccess = action;
            return this;
        }

        public SimpleFormHandler OnSuccess(string message)
        {
            _onSuccess = result => result.Success(message);
            return this;
        }

        public SimpleFormHandler OnResult(IActionResult result)
        {
            _onSuccessResult = _onErrorResult = () => result;
            return this;
        }
        
        public SimpleFormHandler OnResult(Func<IActionResult> resultAction)
        {
            _onSuccessResult = _onErrorResult = resultAction;
            return this;
        }

        public SimpleFormHandler OnResult(Func<IActionResult> onSuccessResult, Func<IActionResult> onErrorResult)
        {
            _onSuccessResult = onSuccessResult;
            _onErrorResult = onErrorResult;
            return this;
        }
        
        public IActionResult ProcessForm(Action action = null)
        {
            if (_onSuccessResult == null)
                throw new ArgumentException("Please call OnResult() before calling ProcessForm()");
            
            if (!ModelState.IsValid)
                return ProcessError();

            action?.Invoke();

            if (!ModelState.IsValid)
                return ProcessError();

            return ProcessSuccess();
        }

        private IActionResult ProcessError() => 
            _onError == null 
                ? _onErrorResult() 
                : _onError(_onErrorResult());

        private IActionResult ProcessSuccess() => 
            _onSuccess == null 
                ? _onSuccessResult() 
                : _onSuccess(_onSuccessResult());
    }
}
