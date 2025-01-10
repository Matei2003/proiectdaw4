namespace proiectdaw4.Services
{
    public class ValidationService
    {

        public bool InvalidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        public bool InvalidPassword(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8;
        }

    }
}
