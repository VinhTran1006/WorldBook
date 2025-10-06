using System.Collections.Generic;
using System.Threading.Tasks;
using WorldBook.Models;
using WorldBook.ViewModels;

namespace WorldBook.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllStaffsAsync();
        Task<User> GetByIdAsync(int id);
        Task CreateStaffAsync(UserCreateViewModel vm);
        Task UpdateStaffAsync(int id, UserEditViewModel vm);
        Task DeleteStaffAsync(int id);
    }
}
