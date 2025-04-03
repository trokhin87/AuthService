using DTO;
using Swashbuckle.AspNetCore.Filters;

namespace AuthWeb.Examples;

public class LoginExample : IExamplesProvider<LoginDto>
{
    public LoginDto GetExamples()
    {
        return new LoginDto
        {
            Login = "user1",
            Password = "password1"
        };
    }
}