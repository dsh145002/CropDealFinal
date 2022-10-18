using CaseStudy.Dtos;
using CaseStudy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace CaseStudy.Repository
{
    public class LoginRepository : ILoginRepository
    {
        DatabaseContext _context;
        public LoginRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<HttpStatusCode> Login(LoginDto loginUser)
        {
            if(loginUser.role == "Admin")
            {
                var user = await _context.Admins.SingleOrDefaultAsync(a => a.Email == loginUser.username);
                if (user == null) 
                    return HttpStatusCode.NotFound;

                else if (!user.Password.Equals(loginUser.password))
                {
                    return HttpStatusCode.Unauthorized;
                }
                return HttpStatusCode.OK;
            }
            else {

                var user = await _context.Users.SingleOrDefaultAsync(a => a.Email == loginUser.username);
                if (user == null) 
                    return HttpStatusCode.NotFound;

                else if (!VerifyPassword(loginUser.password,user.PasswordHash,user.PasswordSalt))
                {
                    return HttpStatusCode.Unauthorized;
                }
                return HttpStatusCode.OK;
                
            }
            
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var passHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return passHash.SequenceEqual(passwordHash);
            }
        }

    }
}
