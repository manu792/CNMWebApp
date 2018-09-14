using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CNMWebApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class Initializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        public Initializer()
        {
            
        }
        protected override void Seed(ApplicationDbContext context)
        {
            context.Roles.Add(new IdentityRole()
            {
                Id = "1",
                Name = "Manager"
            });
            context.Roles.Add(new IdentityRole()
            {
                Id = "2",
                Name = "Empleado"
            });
            context.SaveChanges();

            CreateUserAndAssignRole(context);
        }

        private void CreateUserAndAssignRole(DbContext context)
        {
            try
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var user = new ApplicationUser() { UserName = "manager@manager.com" };

                var task = userManager.CreateAsync(user, "Manager123.");
                var result = task.Result;
                var currentUser = userManager.FindByName(user.UserName);
                var roleresult = userManager.AddToRole(currentUser.Id, "Manager");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new Initializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}