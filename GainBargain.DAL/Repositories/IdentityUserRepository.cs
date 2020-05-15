using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Identity;
using GainBargain.DAL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GainBargain.DAL.Repositories
{
    public class IdentityUserRepository : IIdentityUserRepository
    {
        private AppUserManager userManager;
        private AppRoleManager roleManager;

        public IdentityUserRepository(AppUserManager userManager, AppRoleManager roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;

        }
        public async Task<IdentityResult> CreateAsync(AppUser newUser, string password, string role)
        {
            AppUser user = await userManager.FindByEmailAsync(newUser.Email);

            if (user != null)
            {
                return IdentityResult.Failed("This email is already taken.");
            }

            var result = await userManager.CreateAsync(newUser, password);


            if (result.Succeeded)
            {
                return await userManager.AddToRolesAsync(newUser.Id, role);
            }

            return result;
        }

        public async Task<ClaimsIdentity> Authenticate(string email, string password)
        {
            ClaimsIdentity claimsIdentity = null;
            AppUser user = await userManager.FindAsync(email, password);
            if (user != null)
                claimsIdentity = await userManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);
            return claimsIdentity;
        }

        public async Task<AppUser> GetAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<AppUser> FindByNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }
        public AppUser FindByName(string userName)
        {
            return userManager.FindByName(userName);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync(Expression<Func<AppUser, Boolean>> predicate)
        {
            return await userManager.Users.Where(predicate).OrderBy(x => x.DateOfRegistration).ToListAsync();
        }

        public async Task<IdentityResult> UpdateAsync(AppUser newUser, string newRole)
        {
            var result = await userManager.UpdateAsync(newUser);
            if (newRole != null)
            {
                await userManager.RemoveFromRoleAsync(newUser.Id, userManager.GetRoles(newUser.Id).FirstOrDefault());
                result = await userManager.AddToRolesAsync(newUser.Id, newRole);
            }
            return result;
        }

        public IEnumerable<string> GetRoles(string id)
        {
            return userManager.GetRoles(id);
        }

        public async Task<IdentityResult> DeleteAsync(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            return await userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<AppRole>> GetAllRolesAsync()
        {
            return await roleManager.Roles.ToListAsync();
        }
    }
}
