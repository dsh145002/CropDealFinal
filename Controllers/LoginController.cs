using CaseStudy.Dtos;
using CaseStudy.Models;
using CaseStudy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private DatabaseContext _databaseContext;
        LoginService _loginService;
        public LoginController(DatabaseContext context,LoginService service)
        {
            _databaseContext = context;
            _loginService = service;
        }



        //[HttpPost("register")]
        //[AllowAnonymous]
        //public async Task<ActionResult<User>> Register(CreateUserDto newUser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new User();
        //        user.Name = newUser.Name;
        //        user.Email = newUser.Email;
        //        user.Phone = newUser.Phone;
        //        user.Status = newUser.Status;
        //        user.RoleId = (int)Enum.Parse(typeof(Role), newUser.Role);
        //        CreatePasswordHash(newUser.Password, out byte[] passwordHash, out byte[] passwordSalt);
        //        user.PasswordSalt = passwordSalt;
        //        user.PasswordHash = passwordHash;


        //        //Address
        //        var address = new Address();
        //        address.Line = newUser.Line;
        //        address.City = newUser.City;
        //        address.State = newUser.State;

        //        user.Address = address;

        //        Account acc = new Account();
        //        //Account
        //        acc.AccountNumber = newUser.AccountNumber;
        //        acc.IFSCCode = newUser.IFSC;
        //        acc.BankName = newUser.BankName;
        //        user.Account = acc;

        //        _databaseContext.Users.Add(user);
        //        _databaseContext.SaveChanges();
        //        return Ok();
        //    }
        //    return BadRequest();

        //}
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginDto loginData)
        {
            var res = await _loginService.Login(loginData);

            if (res == HttpStatusCode.OK)
            {
                var token = GenerateToken(loginData);
                return token;
            }
            else if (res == HttpStatusCode.Unauthorized)
            {
                return Unauthorized("Wrong Password!!");
            }
            else if (res == HttpStatusCode.NotFound)
            {
                return BadRequest("User not Found. Check username or Register");
            }

            return BadRequest("Some Error Occured"); 
            
        }


        private string GenerateToken(LoginDto user)
        {
            IEnumerable<Claim> claims = new List<Claim>
            {
                new Claim("email",user.username),
                new Claim("role",user.role),
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisadummytokenkey"));
            var signCred = new SigningCredentials(symmetricKey,SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signCred);

            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       
    }
}
