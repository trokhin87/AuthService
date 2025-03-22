using DTO;

namespace Interfaces;

public interface IAuthService
{
    Task<string> AuthAsync(LoginDto loginDto);
}