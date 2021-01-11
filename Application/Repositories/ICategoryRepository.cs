
using DataLayer.Data.Models;
using System.Collections.Generic;

namespace Application.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<RelationType> AllCategories { get; }
    }
}
