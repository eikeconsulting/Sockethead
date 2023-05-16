using System;

namespace Sockethead.Razor.Forms
{
    public class Validator
    {
        internal bool IsValid { get; private set; }
        internal string Key { get; private set; }
        internal string ErrorMessage { get; private set; }

        public Validator For(string key)
        {
            Key = key;
            return this;
        }

        public Validator Message(string errorMessage)
        {
            ErrorMessage = errorMessage;
            return this;
        }

        public Validator Must(bool condition)
        {
            IsValid = IsValid && condition;
            return this;
        }

        public Validator Must(Func<bool> condition)
        {
            IsValid = IsValid && condition();
            return this;
        }
    }
}
 