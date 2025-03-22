using DTO;

namespace Interfaces;

public interface IJwtService
{
    string GenerateToken(string username);
}