using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public partial class Ticket
    {
        public Ticket()
        {
            this.TicketAttachments=new HashSet<TicketAttachment>();
            this.TicketComments=new HashSet<TicketComment>();
            this.TicketHistories=new HashSet<TicketHistory>();
            this.TicketNotifications=new HashSet<TicketNotification>();
            this.TicketPriorities = new HashSet<TicketPriority>();

        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public Nullable<DateTimeOffset> Updated { get; set; }       
        public int TicketStatusId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketTypeId { get; set; }
        public int ProjectId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedUserId { get; set; }

        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<TicketPriority> TicketPriorities { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual Project Project { get; set; }

    }
    public partial class TicketAttachment
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public Nullable<DateTimeOffset> Created { get; set; }
        public string FileUrl { get; set; }
        public string UserId { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
    public partial class TicketComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTimeOffset> Created { get; set; }
        public string UserId { get; set; }
        public int TicketId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
    public partial class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Changed { get; set; }
        public string UserId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
    public partial class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
    public partial class TicketType
    {
        public TicketType()
        {
            this.Tickets=new HashSet<Ticket>();
        }
           
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
    public partial class TicketStatus
    {
        public TicketStatus()
        {
            this.Tickets=new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
    public partial class TicketPriority
    {
        public TicketPriority()
        {
            this.Tickets=new HashSet<Ticket>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
    public partial class Project
    {
        public Project()
        {
            this.ProjectUsers=new HashSet<ProjectUser>();
            this.Tickets= new HashSet<Ticket>();

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
    public partial class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser User { get; set; }
       
    }
}