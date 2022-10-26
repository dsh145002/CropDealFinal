using CaseStudy.Dtos;
using CaseStudy.Dtos.UserDtos;
using CaseStudy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace CaseStudy.Repository
{
    public class UserRepository : IUserRepository
    {
        private enum Role
        {
            Farmer = 1,
            Dealer = 2
        }
        
	DatabaseContext _context;
        
	public UserRepository(DatabaseContext context)
        {
            _context = context;
        }
        
	public async Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
				return await _context.Users.Include("Account").Include("Address")
					.ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Returning the Users");
            }
            return null;
        }

	public async Task<ActionResult<User>> GetUserByIDAsync(int id)
	{
		try
		{
			var user = await _context.Users.Include("Account")
					.Include("Address").SingleOrDefaultAsync(u => u.UserId == id);
			if(user == null)
				{
					return null;
				}
			return user;
		}
		catch (Exception e)
		{
			Console.WriteLine("The user with the given Id could not be found");
		}
			return null;
		
    }

	public async Task<ActionResult<User>> UpdateUserAsync(UpdateUserDto givenUser,int id)
	{
		try
		{
			var user = await _context.Users
					.Include("Address")
					.Include("Account")
					.SingleOrDefaultAsync(u => u.UserId == id);
		    if(user == null)	
			{
				return null;
			}
                	user.Email = givenUser.Email;
                	user.Phone = givenUser.Phone;
			
			//var address = new Address();
                	user.Address.Line = givenUser.Line;
                	user.Address.City = givenUser.City;
                	user.Address.State = givenUser.State;
                	//user.Address = address;

			//Account acc = new Account();
                	user.Account.AccountNumber = givenUser.AccountNumber;
                user.Account.IFSCCode = givenUser.IFSC;
                user.Account.BankName = givenUser.BankName;
                //	user.Account = acc;

			_context.Update(user);
                	await _context.SaveChangesAsync();
                	return user;
		}
		catch (Exception e)
		{
			Console.WriteLine("Could not update the user details");
		}
		return null;
	}

	[Authorize]
	public async Task<HttpStatusCode> DeleteUserAsync(int id)
	{
		try
		{
			_context.Remove(_context.Users.Single(u => u.UserId == id));
			await _context.SaveChangesAsync();
			return HttpStatusCode.OK;
		}
		catch (Exception e)
		{
			Console.WriteLine("Could not delete user");
		}
		return HttpStatusCode.NotFound;
	}

	[Authorize]
	public async Task<ActionResult<User>> StatusUpdateAsync(string stat, int id)
	{
		try
		{
			User user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == id);
			user.Status = stat;
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return user;
		}
		catch( Exception e)
		{
			Console.WriteLine("Could not change the status");
		}
		return null;
	}

	[Authorize]
	public async Task<HttpStatusCode> AddRatingAsync(RatingDto ratinginfo, int id)
	{
		try{
			Rating rating = new Rating();
			rating.TotalRating = ratinginfo.TotalRating;
			rating.Review = ratinginfo.Review;
				rating.UserId = id;
			_context.Ratings.Add(rating);
			await _context.SaveChangesAsync();
			return HttpStatusCode.OK;
		}
		catch(Exception e)
		{
			Console.WriteLine("Could not add rating");
		}
		return HttpStatusCode.BadRequest;
	}

	[Authorize]
	public async Task<HttpStatusCode> UpdateRatingAsync(RatingDto ratinginfo, int id)
	{
		try
		{
			var rating = await _context.Ratings.SingleOrDefaultAsync(r => r.UserId == id);
			if(rating==null)
			{
				return HttpStatusCode.NotFound;
			}
			rating.TotalRating = ratinginfo.TotalRating;
			rating.Review = ratinginfo.Review;

			_context.Ratings.Update(rating);
			await _context.SaveChangesAsync();
				return HttpStatusCode.OK;
		}
		catch(Exception e)
		{
			Console.WriteLine("could not update rating");
		}
		return HttpStatusCode.BadRequest;
	}
    }
}
