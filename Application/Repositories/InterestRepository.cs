using DataLayer.Data;
using DataLayer.Data.Models;
using System.Collections.Generic;
using System.Linq;
// Repository regarding the interest a user can have
namespace Application.Repositories
{
    public class InterestRepository : IInterestRepository
    {
        private readonly MatchBuddieContext _dbContext;

        public InterestRepository(MatchBuddieContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Interests> AllInterests
        {
            get
            {
                return _dbContext.Interests;
            }
        }

        public Interests GetInterest(string interest)
        {
            return _dbContext.Interests.FirstOrDefault(x => x.Interest == interest);
        }

        public ICollection<Interests> UsersInterest(string id)
        {
            return _dbContext.Users.Where(c => c.Id == id).SelectMany(c => c.Interests).ToList();
        }
    }
}
