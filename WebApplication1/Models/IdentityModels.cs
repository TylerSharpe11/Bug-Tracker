using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Ticket> TicketsOwned { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
        public virtual ICollection<Ticket> TicketsAssigned { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
           
        }
        

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<WebApplication1.Models.Ticket> Tickets { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketPriority> TicketPriorities { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketStatus> TicketStatus { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketType> TicketTypes { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.ProjectUser> ProjectUsers { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketAttachment> TicketAttachments { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketComment> TicketComments { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketNotification> TicketNotifications { get; set; }

        public System.Data.Entity.DbSet<WebApplication1.Models.TicketHistory> TicketHistories { get; set; }
    }
}