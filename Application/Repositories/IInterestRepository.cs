
using DataLayer.Data.Models;
using System.Collections.Generic;

namespace Application.Repositories
{
    public interface IInterestRepository
    {
        IEnumerable<Interests> AllInterests { get; }
        ICollection<Interests> UsersInterest(string id);
        Interests GetInterest(string dto);
    }
}
