using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using WorldBook.Services.Interfaces;
using WorldBook.ViewModel;

namespace WorldBook.Services
{
    public class RegisterService: IRegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        public RegisterService(
            IUserRepository userRepository,
            IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return false;
            return await _userRepository.PhoneExistsAsync(phone);
        }
        public async Task RegisterUserAsync(RegisterViewModel model)
        {
            // Validate dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(model.Username))
                throw new Exception("Username is required");

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new Exception("Email is required");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new Exception("Password is required");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new Exception("Full name is required");

            // Kiểm tra xem username đã tồn tại chưa
            if (await _userRepository.UsernameExistsAsync(model.Username))
                throw new Exception("Username already exists");

            // Kiểm tra xem email đã tồn tại chưa
            if (await _userRepository.EmailExistsAsync(model.Email))
                throw new Exception("Email already exists");

            // Kiểm tra xem phone đã tồn tại chưa (nếu được nhập)
            if (!string.IsNullOrEmpty(model.Phone))
            {
                if (await _userRepository.PhoneExistsAsync(model.Phone))
                    throw new Exception("Phone number already exists");
            }

            // Hash password
            var hashedPassword = _authService.HashPassword(model.Password);

            // Tạo user mới
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = hashedPassword,
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                IsActive = true,
                AddedAt = DateTime.Now
            };

            // Thêm user vào database
            await _userRepository.RegisterAsync(user);

            // Assign Customer role
            var customerRole = await _userRepository.GetRoleByNameAsync("Customer");
            if (customerRole == null)
                throw new Exception("Customer role not found in system");

            await _userRepository.AssignRoleAsync(user.UserId, customerRole.RoleId);
        }
    }
}
