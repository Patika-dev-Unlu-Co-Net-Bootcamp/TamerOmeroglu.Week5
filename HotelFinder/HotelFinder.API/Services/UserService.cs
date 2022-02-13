using HotelFinder.API.Authorization;
using HotelFinder.API.Extensions;
using HotelFinder.API.Helpers;
using HotelFinder.DataAccess;
using HotelFinder.Entity;
using HotelFinder.Entity.DTO;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using BcryptNet = BCrypt.Net.BCrypt;

namespace HotelFinder.API.Services
{

    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);

    }

    public class UserService : IUserService
    {
        private HotelFinderDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public UserService(
            HotelFinderDbContext context,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);

            // validate
            if (user == null || !BcryptNet.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful so generate jwt token
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken);
        }


        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }


        public bool UserIsValid(User user)
        {
            return user.IsValid();
        }
    }
    
}
