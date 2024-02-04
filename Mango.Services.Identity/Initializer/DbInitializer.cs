using IdentityModel;
using Mango.Services.Identity.DbContext;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }
            ApplicationUser adminUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111111111",
                FirstName = "Jon",
                LastName = "Admin"
            };
            _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1 = _userManager.AddClaimsAsync(adminUser, new System.Security.Claims.Claim[] {
                new System.Security.Claims.Claim(JwtClaimTypes.Name , $"{adminUser.FirstName} {adminUser.LastName}"),
                new System.Security.Claims.Claim (JwtClaimTypes.FamilyName, adminUser.LastName),
                new System.Security.Claims.Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                new System.Security.Claims.Claim(JwtClaimTypes.Role, SD.Admin)
            }).Result;

            
            ApplicationUser customer = new ApplicationUser
            {
                UserName = "customer1@gmail.com",
                Email = "customer1@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "111111111111111",
                FirstName = "Jon",
                LastName = "Cust"
            };
            _userManager.CreateAsync(customer, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customer, SD.Customer).GetAwaiter().GetResult();

            var temp2= _userManager.AddClaimsAsync(customer, new System.Security.Claims.Claim[] {
                new System.Security.Claims.Claim(JwtClaimTypes.Name , $"{customer.FirstName} {customer.LastName}"),
                new System.Security.Claims.Claim (JwtClaimTypes.FamilyName, customer.LastName),
                new System.Security.Claims.Claim(JwtClaimTypes.GivenName, customer.FirstName),
                new System.Security.Claims.Claim(JwtClaimTypes.Role, SD.Admin)
            }).Result;
        }
    

    }
}
