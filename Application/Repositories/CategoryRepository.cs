using DataLayer.Data;
using DataLayer.Data.Models;
using System.Collections.Generic;
// Repository for Categories that can be used to categories a friend
namespace Application.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MatchBuddieContext _dbContext;

        public CategoryRepository(MatchBuddieContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<RelationType> AllCategories
        {
            get
            {
                return _dbContext.RelationType;
            }
        }
    }
}
