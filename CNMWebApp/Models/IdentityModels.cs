using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CNMWebApp.Models
{
    [Table("Estados")]
    public class Estado
    {
        public int EstadoId { get; set; }
        public string Nombre { get; set; }
    }

    [Table("SolicitudesVacaciones")]
    public class SolicitudVacaciones
    {
        public int SolicitudVacacionesId { get; set; }
        public string UsuarioId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Comentario { get; set; }
        public int EstadoId { get; set; }

        // Virtual properties mapping to ApplicationUser and Estado
        public virtual ApplicationUser Usuario { get; set; }
        public virtual Estado Estado { get; set; }
    }

    // Unidades Tecnicas del empleado va a ir mapeada a la tabla Unidades Tecnicas
    [Table("UnidadesTecnicas")]
    public class UnidadTecnica
    {
        public int UnidadTecnicaId { get; set; }
        public string Nombre { get; set; }
    }

    // Categorias del empleado va a ir mapeada a la tabla Categorias
    [Table("Categorias")]
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
    }


    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo Primer Apellido es requerido")]
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string FotoRuta { get; set; }
        public int CategoriaId { get; set; }
        public int UnidadTecnicaId { get; set; }
        public string JefeCedula { get; set; }

        // Virtual properties, foregin keys to Categoria and UnidadTecnica
        public virtual Categoria Categoria { get; set; }
        public virtual UnidadTecnica UnidadTecnica { get; set; }
        public virtual ICollection<SolicitudVacaciones> VacacionesSolicitadas { get; set; }

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
            AgregarEstados(context);
            AgregarRoles(context);
            AgregarUnidadesTecnicas(context);
            AgregarCategorias(context);
            CrearUsuarioAsignarRoles(context);
        }

        private void AgregarEstados(ApplicationDbContext context)
        {
            context.Estados.AddRange(new List<Estado>()
            {
                new Estado()
                {
                    EstadoId = 1,
                    Nombre = "Por revisar"
                },
                new Estado()
                {
                    EstadoId = 2,
                    Nombre = "Aprobado"
                },
                new Estado()
                {
                    EstadoId = 3,
                    Nombre = "Rechazado"
                }
            });
            context.SaveChanges();
        }

        private void AgregarUnidadesTecnicas(ApplicationDbContext context)
        {
            context.UnidadesTecnicas.AddRange(new List<UnidadTecnica>()
            {
                new UnidadTecnica()
                {
                    UnidadTecnicaId = 1,
                    Nombre = "Centro Nacional de la Musica (CNM)"
                },
                new UnidadTecnica()
                {
                    UnidadTecnicaId = 2,
                    Nombre = "Instituto Nacional de la Musica (INM)"
                },
                new UnidadTecnica()
                {
                    UnidadTecnicaId = 3,
                    Nombre = "Coro Sinfonico Nacional"
                },
                new UnidadTecnica()
                {
                    UnidadTecnicaId = 4,
                    Nombre = "Compañia de Lirica Nacional"
                },
                new UnidadTecnica()
                {
                    UnidadTecnicaId = 5,
                    Nombre = "Profesores INM"
                },
                new UnidadTecnica()
                {
                    UnidadTecnicaId = 6,
                    Nombre = "Manager"
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
                    CategoriaId = 1,
                    Nombre = "Oficinistas"
                },
                new Categoria()
                {
                    CategoriaId = 2,
                    Nombre = "Tecnicos"
                },
                new Categoria()
                {
                    CategoriaId = 3,
                    Nombre = "Profesional"
                },
                new Categoria()
                {
                    CategoriaId = 4,
                    Nombre = "Jefatura"
                },
                new Categoria()
                {
                    CategoriaId = 5,
                    Nombre = "Director Administrativo"
                },
                new Categoria()
                {
                    CategoriaId = 6,
                    Nombre = "Director General"
                },
                new Categoria()
                {
                    CategoriaId = 7,
                    Nombre = "Miscelaneos"
                },
                new Categoria()
                {
                    CategoriaId = 8,
                    Nombre = "Seguridad"
                },
                new Categoria()
                {
                    CategoriaId = 9,
                    Nombre = "Manager"
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
                    Id = "00000000",
                    Nombre = "Manager",
                    PrimerApellido = "Manager",
                    SegundoApellido = "Manager",
                    UserName = "manager@manager.com",
                    Email = "manager@manager.com",
                    FechaIngreso = DateTime.Now,
                    UnidadTecnicaId = 1,
                    CategoriaId = 1,
                    JefeCedula = "00000000"
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
        public DbSet<Estado> Estados { get; set; }
        public DbSet<SolicitudVacaciones> SolicitudesVacaciones { get; set; }

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
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Email).HasColumnName("Correo").IsRequired();
            modelBuilder.Entity<ApplicationUser>().Property(p => p.EmailConfirmed).HasColumnName("CorreoConfirmado");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.LockoutEnabled).HasColumnName("BloqueoActivado");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.LockoutEndDateUtc).HasColumnName("FechaFinBloqueoUtc");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.PasswordHash).HasColumnName("ContrasenaHash");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.PhoneNumber).HasColumnName("Telefono");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.PhoneNumberConfirmed).HasColumnName("TelefonoConfirmado");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.SecurityStamp).HasColumnName("SelloSeguridad");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.TwoFactorEnabled).HasColumnName("AutenticacionDosFactoresActivada");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.UserName).HasColumnName("NombreUsuario");
            modelBuilder.Entity<ApplicationUser>().Property(p => p.JefeCedula).IsRequired();


            modelBuilder.Entity<SolicitudVacaciones>().Property(p => p.UsuarioId).HasColumnName("Cedula");

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