using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldBook.Models;
using WorldBook.Repositories;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;
using WorldBook.ViewModels;

namespace WorldBook.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuthService _authService;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _authService = authService;
        }

        public async Task CreateStaffAsync(UserCreateViewModel vm)
        {
            // Kiểm tra username trùng nhưng chỉ block nếu Active
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(vm.Username);
            if (existingUserByUsername != null && existingUserByUsername.IsActive)
                throw new Exception("Username already exists");

            var existingUserByEmail = await _userRepository.GetByEmailAsync(vm.Email);
            if (existingUserByEmail != null && existingUserByEmail.IsActive)
                throw new Exception("Email already exists");

            var existingUserByPhone = await _userRepository.GetByPhoneAsync(vm.Phone);
            if (existingUserByPhone != null && existingUserByPhone.IsActive)
                throw new Exception("Phone already exists");

            // Hash password
            var hashed = _authService.HashPassword(vm.Password ?? string.Empty);

            var user = new User
            {
                Username = vm.Username,
                Name = vm.Name,
                Email = vm.Email,
                Phone = vm.Phone,
                Address = vm.Address,
                DateOfBirth = vm.DateOfBirth,
                Gender = vm.Gender,
                Password = hashed,
                IsActive = true,   // luôn mặc định là Active khi tạo
                AddedAt = DateTime.Now
            };

            await _userRepository.AddAsync(user);

            // ensure Staff role exists
            var staffRole = await _roleRepository.GetByNameAsync("Staff");
            if (staffRole == null)
            {
                staffRole = await _roleRepository.AddAsync(new Role { Name = "Staff" });
            }

            await _userRepository.AssignRoleAsync(user.UserId, staffRole.RoleId);
        }


        public async Task DeleteStaffAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("Staff not found"); // vẫn giữ cho trường hợp id sai

            user.IsActive = false; // soft delete
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("Customer not found");

            // Chỉ xóa mềm (block account)
            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
        }


        public async Task<IEnumerable<User>> GetAllStaffsAsync()
        {
            return await _userRepository.GetStaffsAsync();
        }

        public async Task<IEnumerable<User>> GetAllCustomersAsync()
        {
            return await _userRepository.GetCustomersAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task UpdateStaffAsync(int id, UserEditViewModel vm)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Name = vm.Name;
            user.Address = vm.Address;
            user.DateOfBirth = vm.DateOfBirth;
            user.Gender = vm.Gender;

            await _userRepository.UpdateAsync(user);
        }
        public async Task<ProfileViewModel?> GetProfileAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return new ProfileViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Address = user.Address,
                Phone = user.Phone
            };
        }

        public async Task UpdateProfileAsync(ProfileViewModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId);
            if (user == null)
                throw new Exception("User not found");

            user.Name = model.Name;
            user.Email = model.Email;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.Address = model.Address;
            user.Phone = model.Phone;

            await _userRepository.UpdateAsync(user);
        }
    }
}
