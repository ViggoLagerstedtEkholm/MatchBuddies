using DataLayer.Data;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// Repository regarding the diffrent users of the system
namespace Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MatchBuddieContext _dbContext;
        private readonly UserManager<User> _userManager;
        public UserRepository(MatchBuddieContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public IEnumerable<User> AllUsers
        {
            get
            {
                return _dbContext.User.Where(user => user.isActive && user.IsEnabled).ToList();
            }
        }
        public User GetUserByID(string id)
        {
            return _dbContext.User.FirstOrDefault(x => x.Id == id);
        }
        public async Task SaveUser(ClaimsPrincipal claims, User user)
        {
            var _user = await _userManager.GetUserAsync(claims);
            _user.Firstname = user.Firstname;
            _user.Lastname = user.Lastname;
            _user.age = user.age;
            _user.City = user.City;
            _user.Preference = user.Preference;
            _user.Gender = user.Gender;
            _user.Description = user.Description;
            await _userManager.UpdateAsync(_user);

        }
        public async Task DeleteAccount(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            user.isActive = false;
            await _userManager.UpdateAsync(user);

        }

        public async Task HideAccount(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            user.IsEnabled = !user.IsEnabled;
            await _userManager.UpdateAsync(user);
        }

        public async Task SaveImage(ClaimsPrincipal claims, User user)
        {
            var _user = await _userManager.GetUserAsync(claims);
            await _userManager.UpdateAsync(_user);
        }
        public async Task SaveInterest(ClaimsPrincipal claims, Interests interest)
        {
            var _user = await _userManager.GetUserAsync(claims);
            _user.Interests.Add(interest);
            await _userManager.UpdateAsync(_user);
        }
        public async Task DeleteInterest(ClaimsPrincipal claims, Interests interest)
        {
            var _user = await _userManager.GetUserAsync(claims);
            var interestUser = _dbContext.Interests
                .Include(x => x.Users.Where(z => z.Id == _user.Id))
                .Single(s => s.InterestsID == interest.InterestsID);
            var usersInterest = interestUser.Users;
            _user.Interests.Remove(interest);
            await _userManager.UpdateAsync(_user);
        }

        public void SaveVisit(ProfilePageUser pageVisit)
        {
            var visit = _dbContext.VisitedPages.Find(pageVisit.ProfilePageID, pageVisit.UserId);
            if (visit == null)
            {
                _dbContext.VisitedPages.Add(pageVisit);
            }
            else
            {
                visit.Date = DateTime.Now;
                _dbContext.VisitedPages.Update(visit);
            }
            _dbContext.SaveChanges();
        }

        // This methods logic is used for "Passar vi ihop" and "Hitta passande parter", which we have given other english names
        // Returns a list of users that matches the searching users interests and preferences
        // I will only return matches for users that has similar intersts and matches the user preferences

        public async Task<List<User>> GetAllMatches(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);

            var users = AllUsers.Where(i => i.Id != user.Id).ToList();

            var currentUserInterests = user.Interests;
            double interestCounter = 0;
            double variables = 0;
            var matchingUsers = new List<User>();

            foreach (var aUser in users)
            {
                foreach (var interest in currentUserInterests)
                {
                    variables++;
                    foreach (var selectedInterest in aUser.Interests)
                    {
                        if (interest.Interest == selectedInterest.Interest)
                        {
                            interestCounter++;
                        }
                    }
                }

                double quota = (interestCounter / variables) * 100;   // Interest must have atleast a 30% match for it to be a match
                if (quota > 30)
                {

                    switch (user.Gender)   // After the method has checked if interests matches with atleas 30% then it will check the users preferences, it will check all combinations
                    {
                        case GenderType.FEMALE:
                            switch (user.Preference)
                            {
                                case MatchPreference.MALE:
                                    if (aUser.Gender == GenderType.MALE && (aUser.Preference == MatchPreference.FEMALE || aUser.Preference == MatchPreference.MALE_AND_FEMALE))
                                        matchingUsers.Add(aUser);
                                    break;
                                case MatchPreference.FEMALE:
                                    if (aUser.Gender == GenderType.FEMALE && (aUser.Preference == MatchPreference.FEMALE || aUser.Preference == MatchPreference.MALE_AND_FEMALE))
                                        matchingUsers.Add(aUser);
                                    break;
                                case MatchPreference.MALE_AND_FEMALE:
                                    if (aUser.Preference == MatchPreference.MALE_AND_FEMALE || aUser.Preference == MatchPreference.FEMALE)
                                        matchingUsers.Add(aUser);
                                    break;
                            }
                            break;
                        case GenderType.MALE:
                            switch (user.Preference)
                            {
                                case MatchPreference.MALE:
                                    if (aUser.Gender == GenderType.MALE && (aUser.Preference == MatchPreference.MALE || aUser.Preference == MatchPreference.MALE_AND_FEMALE))
                                        matchingUsers.Add(aUser);
                                    break;
                                case MatchPreference.FEMALE:
                                    if (aUser.Gender == GenderType.FEMALE && (aUser.Preference == MatchPreference.MALE || aUser.Preference == MatchPreference.MALE_AND_FEMALE))
                                        matchingUsers.Add(aUser);
                                    break;
                                case MatchPreference.MALE_AND_FEMALE:
                                    if (aUser.Preference == MatchPreference.MALE_AND_FEMALE || aUser.Preference == MatchPreference.MALE)
                                        matchingUsers.Add(aUser);
                                    break;
                            }
                            break;
                    }
                }
                interestCounter = 0; 
                variables = 0;  
            }
            return matchingUsers;
        }

        // Using Linq query syntax to get users from the database in a descending order i relation to the date one visited a specific page 
        public IQueryable<object> GetLatestVisitors(User user)
        {
            var profile = _dbContext.ProfilePage.Where(x => x.Owner == user.Id).FirstOrDefault();
            var latestVisitors = from us in _dbContext.User
                                 join visits in _dbContext.VisitedPages
                                 on us.Id equals visits.UserId
                                 join pageprof in _dbContext.ProfilePage
                                 on visits.UserId equals pageprof.Owner
                                 where visits.ProfilePageID == profile.ProfilePageID
                                 orderby visits.Date descending
                                 select new { us.Firstname, us.Id, Datetime = visits.Date.ToShortTimeString(), Date = visits.Date.ToShortDateString() };

            return latestVisitors;
        }

        public ProfilePage GetProfilePage(string id)
        {
            return _dbContext.ProfilePage.Where(x => x.Owner == id).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return _dbContext.User.Where(x => x.Email.Equals(email)).FirstOrDefault();
        }
    }
}
