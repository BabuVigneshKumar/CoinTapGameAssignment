public interface IUserValidator
{
    string Validate(string phoneNumber, string password);
}
public class LoginUserValidate : IUserValidator
{
    public string Validate(string phoneNumber, string password)
    {
        if (string.IsNullOrEmpty(phoneNumber)) return "Phone Number is Required";
        if (phoneNumber.Length < 10) return "Phone Number should be 10 digits";
        if (string.IsNullOrEmpty(password)) return "Password is Required";
        if (password.Length < 6) return "Password must be at least 6 characters.";
        return string.Empty;
    }
}