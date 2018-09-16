using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CNMWebApp.Models
{
    // Unidades Tecnicas del empleado va a ir mapeada a la tabla Unidades Tecnicas
    [Table("UnidadesTecnicas")]
    public class UnidadTecnica
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    // Categorias del empleado va a ir mapeada a la tabla Categorias
    [Table("Categorias")]
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }


    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public DateTime FechaIngreso { get; set; }
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
            AgregarRoles(context);
            AgregarUnidadesTecnicas(context);
            AgregarCategorias(context);
            CrearUsuarioAsignarRoles(context);
        }

        private void AgregarUnidadesTecnicas(ApplicationDbContext context)
        {
            context.UnidadesTecnicas.AddRange(new List<UnidadTecnica>()
            {
                new UnidadTecnica()
                {
                    Id = 1,
                    Nombre = "Centro Nacional de la Musica (CNM)"
                },
                new UnidadTecnica()
                {
                    Id = 2,
                    Nombre = "Instituto Nacional de la Musica (INM)"
                },
                new UnidadTecnica()
                {
                    Id = 3,
                    Nombre = "Coro Sinfonico Nacional"
                },
                new UnidadTecnica()
                {
                    Id = 4,
                    Nombre = "Compañia de Lirica Nacional"
                },
                new UnidadTecnica()
                {
                    Id = 5,
                    Nombre = "Profesores INM"
                }
            });
            context.SaveChanges();
        }

        private void AgregarCategorias(ApplicationDbContext context)
        {
            context.Categorias.AddRange(new List<Categoria>()
            {
                new Categoria()
                {
                    Id = 1,
                    Nombre = "Oficinistas"
                },
                new Categoria()
                {
                    Id = 2,
                    Nombre = "Tecnicos"
                },
                new Categoria()
                {
                    Id = 3,
                    Nombre = "Profesional"
                },
                new Categoria()
                {
                    Id = 4,
                    Nombre = "Jefatura"
                },
                new Categoria()
                {
                    Id = 5,
                    Nombre = "Director Administrativo"
                },
                new Categoria()
                {
                    Id = 6,
                    Nombre = "Director General"
                },
                new Categoria()
                {
                    Id = 7,
                    Nombre = "Miscelaneos"
                },
                new Categoria()
                {
                    Id = 8,
                    Nombre = "Seguridad"
                }
            });
            context.SaveChanges();
        }

        private void AgregarRoles(ApplicationDbContext context)
        {
            context.Roles.Add(new IdentityRole()
            {
                Id = "1",
                Name = "Funcionario"
            });
            context.Roles.Add(new IdentityRole()
            {
                Id = "2",
                Name = "Jefatura"
            });
            context.Roles.Add(new IdentityRole()
            {
                Id = "3",
                Name = "Manager"
            });
            context.Roles.Add(new IdentityRole()
            {
                Id = "4",
                Name = "RecursosHumanos"
            });
            context.SaveChanges();
        }

        private void CrearUsuarioAsignarRoles(DbContext context)
        {
            try
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var user = new ApplicationUser()
                {
                    UserName = "manager@manager.com",
                    Email = "manager@manager.com",
                    FechaIngreso = DateTime.Now
                };

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
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<UnidadTecnica> UnidadesTecnicas { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new Initializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<ApplicationUser>().ToTable("Usuarios");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Id).HasColumnName("Cedula");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.AccessFailedCount).HasColumnName("AccesoFallidoCantidad");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Email).HasColumnName("Correo");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.EmailConfirmed).HasColumnName("CorreoConfirmado");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.LockoutEnabled).HasColumnName("BloqueoActivado");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.LockoutEndDateUtc).HasColumnName("FechaFinBloqueoUtc");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.PasswordHash).HasColumnName("ContrasenaHash");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.PhoneNumber).HasColumnName("Telefono");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.PhoneNumberConfirmed).HasColumnName("TelefonoConfirmado");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.SecurityStamp).HasColumnName("SelloSeguridad");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.TwoFactorEnabled).HasColumnName("AutenticacionDosFactoresActivada");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.UserName).HasColumnName("NombreUsuario");

            modelBuilder.Entity<IdentityUserRole>().ToTable("UsuarioRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UsuarioLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UsuarioClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}