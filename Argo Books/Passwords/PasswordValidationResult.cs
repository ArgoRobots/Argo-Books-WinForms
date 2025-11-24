namespace Argo_Books.Passwords
{
    /// <summary>
    /// Represents the result of password validation with individual requirement flags.
    /// </summary>
    public class PasswordValidationResult(bool isValid, bool lengthValid, bool uppercaseValid, bool digitValid, bool specialCharValid)
    {
        public bool IsValid { get; set; } = isValid;
        public bool LengthValid { get; set; } = lengthValid;
        public bool UppercaseValid { get; set; } = uppercaseValid;
        public bool DigitValid { get; set; } = digitValid;
        public bool SpecialCharValid { get; set; } = specialCharValid;
    }
}