using System.Threading.Tasks;
using AuthApp.Models;

namespace AuthApp.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<IEnumerable<object>> SearchUsers(string searchTerm);

    }
}
