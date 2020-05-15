using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GainBargain.DAL.Entities;
using Microsoft.AspNet.Identity;

namespace GainBargain.DAL.Interfaces
{
    public interface IIdentityUserRepository
    {
        Task<IdentityResult> CreateAsync(AppUser user, string password, string role);
        Task<IdentityResult> UpdateAsync(AppUser user, string newRole);
        Task<AppUser> FindByNameAsync(string userName);
        Task<ClaimsIdentity> Authenticate(string email, string password);
        Task<IEnumerable<AppUser>> GetUsersAsync(Expression<Func<AppUser, Boolean>> predicate);
        IEnumerable<string> GetRoles(string id);
        Task<AppUser> GetAsync(string id);
        AppUser FindByName(string userName);
        Task<IdentityResult> DeleteAsync(string id);
        Task<IEnumerable<AppRole>> GetAllRolesAsync();
    }
}
