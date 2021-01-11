
namespace Application
{
    public class WrongFileExtensionException : CustomException
    {
        public WrongFileExtensionException() { }

        public override string ErrorMessage()
        {
            var error = " You have not enterd a valid file extension";
            return error;
        }
    }
}
