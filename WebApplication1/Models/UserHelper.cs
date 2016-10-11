using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication1.Models
{
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> manager = 
            new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(
            new ApplicationDbContext()));

        public bool IsUserInRole(string userId, string roleName)
        {
            var result =manager.IsInRole(userId, roleName);
            return result;
        }
        public string findIdFromEmail(string email)
        {
            var result = manager.FindByEmail(email);
            return result.Id;
        }
        public IList<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }
        public bool AddUserRole(string userId, string roleName)
        {
            var result =manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }
        public IList<string> getRoles()
        {
            var roleList = new List<string>();
            roleList.Add("Developer");
            roleList.Add("Submitter");
            roleList.Add("ProjectManager");
            roleList.Add("Administrator");
            return roleList;
        }
        public IList<ApplicationUser> UsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            foreach( var user in manager.Users)
            {
                if(IsUserInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }
    public class UserProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private UserManager<ApplicationUser> manager =
            new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(
            new ApplicationDbContext()));
        public async Task<bool> IsOnProject(string userId, int projectId)
        {

            if(await db.ProjectUsers.AnyAsync(p => p.ProjectId==projectId && p.UserId == userId))
            {
                return true;
            }
            return false;
        }
        public async Task AddUserToProject(string userId, int projectId)
        {
            var pu = new ProjectUser { ProjectId = projectId, UserId = userId };
            if(!await this.IsOnProject(userId, projectId))
            {
                db.ProjectUsers.Add(pu);
                db.SaveChanges();
            }
        }
        public async Task RemoveUserFromProject(string userId, int projectId)
        {
            if (await this.IsOnProject(userId, projectId))
            {
                var pu=db.ProjectUsers.SingleAsync(p => p.ProjectId == projectId && p.UserId ==userId);
                db.ProjectUsers.Remove(pu.Result);
                db.SaveChanges();
            }
        }

        public async Task<IList<Ticket>> TicketsFromProjects(string userId)
        {
            var t = new List<Ticket>();
            foreach (var project in db.Projects)
            {
                if (await this.IsOnProject(userId, project.Id))
                {
                    foreach (var ticket in db.Tickets)
                    {
                        if (ticket.ProjectId == project.Id)
                        {
                            t.Add(ticket);
                        }
                    }
                }
            }
            return t;
        }

        public async Task<IList<Project>> ListProjectsForUser(string userId)
        {
            var projectList = new List<Project>();

            foreach (var project in db.Projects)
            {
                if(await this.IsOnProject(userId, project.Id))
                {
                    projectList.Add(project);
                }
            }
            return projectList;
        }
        public async Task<IList<ApplicationUser>> UsersOnProject(int projectId)
        {
            var userList = new List<ApplicationUser>();
            foreach(var user in db.Users)
            {
                if(await this.IsOnProject(user.Id, projectId))
                {
                    userList.Add(user);
                }
                
            }
            return userList;
        }
        public async Task<IList<ApplicationUser>> FindTicketOwner(int? ticketid)
        {
            var userList = new List<ApplicationUser>();
            foreach (var tick in db.Tickets)
            {
                if (tick.Id==ticketid)
                {
                    userList.Add(tick.OwnerUser);
                }

            }
            return userList;
        }

        public async Task<IList<ApplicationUser>> UsersOnProject(int projectId, string rolename)
        {
            var userList = new List<ApplicationUser>();
            var rolesHelper = new UserRolesHelper();
            foreach (var user in db.Users)
            {
                if ((await this.IsOnProject(user.Id, projectId)) && (rolesHelper.IsUserInRole(user.Id,rolename)))
                {
                    userList.Add(user);
                }
                    
            }
            return userList;
        }

        public async Task<IList<ApplicationUser>> UsersNotOnProject(int projectId, string rolename)
        {
            var userList = new List<ApplicationUser>();
            var rolesHelper = new UserRolesHelper();
            foreach (var user in db.Users)
            {
                if ((!await this.IsOnProject(user.Id, projectId)) && (rolesHelper.IsUserInRole(user.Id, rolename)))
                {
                    userList.Add(user);
                }

            }
            return userList;
        }
        public async Task<IList<ApplicationUser>> DevelopersOnSameProjects(string userId)
        {
            var userList = new List<ApplicationUser>();
            var rolesHelper = new UserRolesHelper();
            foreach (var user in db.Users)
            {
                foreach(var project in db.Projects)
                {
                    if ((await this.IsOnProject(user.Id, project.Id)))
                    {
                        if (await this.IsOnProject(userId, project.Id))
                        {
                            if (rolesHelper.IsUserInRole(user.Id, "Developer"))
                            {
                                userList.Add(user);
                            }
                        }
                        
                    }
                }
               

            }

            return userList;
        }
        public async Task<IList<ApplicationUser>> ProjectManagersOnSameProjects(string userId)
        {
            var userList = new List<ApplicationUser>();
            var rolesHelper = new UserRolesHelper();
            foreach (var user in db.Users)
            {
                foreach (var project in db.Projects)
                {
                    if ((await this.IsOnProject(user.Id, project.Id)))
                    {
                        if (await this.IsOnProject(userId, project.Id))
                        {
                            if (rolesHelper.IsUserInRole(user.Id, "ProjectManager"))
                            {
                                userList.Add(user);
                            }
                        }

                    }
                }


            }

            return userList;
        }
        public async Task<IList<ApplicationUser>> UsersOnSameProjects(string userId)
        {
            var userList = new List<ApplicationUser>();
            var rolesHelper = new UserRolesHelper();
            foreach (var user in db.Users)
            {
                foreach (var project in db.Projects)
                {
                    if ((await this.IsOnProject(user.Id, project.Id)))
                    {
                        if (await this.IsOnProject(userId, project.Id))
                        {
                            userList.Add(user);
                        }

                    }
                }


            }
            
            return userList;
        }
        public async Task<IList<ApplicationUser>> SubmittersOnSameProjects(string userId)
        {
            var userList = new List<ApplicationUser>();
            var rolesHelper = new UserRolesHelper();
            foreach (var user in db.Users)
            {
                foreach (var project in db.Projects)
                {
                    if ((await this.IsOnProject(user.Id, project.Id)))
                    {
                        if (await this.IsOnProject(userId, project.Id))
                        {
                            if (rolesHelper.IsUserInRole(user.Id, "Submitter"))
                            {
                                userList.Add(user);
                            }
                        }

                    }
                }


            }
            return userList;
        }
        public async Task<ApplicationUser> UserWithId(string email)
        {
            var u = new ApplicationUser();
            u= manager.FindByEmail(email);
            var userId = u.Id;

            foreach (var ticket in db.Tickets)
            {
                if (ticket.AssignedUserId == userId)
                {
                    u.TicketsAssigned.Add(ticket);
                }
                if (ticket.OwnerUserId == userId)
                {
                    u.TicketsOwned.Add(ticket);
                }
            }
            foreach (var Projectuser in db.ProjectUsers)
            {
                if (Projectuser.UserId == userId)
                {
                    u.ProjectUsers.Add(Projectuser);
                }
            }
            foreach (var ta in db.TicketAttachments)
            {
                if (ta.UserId == userId)
                {
                    u.TicketAttachments.Add(ta);
                }
            }
            foreach (var c in db.TicketComments)
            {
                if (c.UserId == userId)
                {
                    u.TicketComments.Add(c);
                }
            }
            foreach (var th in db.TicketHistories)
            {
                if (th.UserId == userId)
                {
                    u.TicketHistories.Add(th);
                }
            }
            return u;

        }
        public void CreateComment(int ticketid, string userId, string comment)
        {
            var ticketComment = new TicketComment();
            ticketComment.UserId = userId;
            ticketComment.Created = new DateTimeOffset(DateTime.Now);

            ticketComment.Comment = comment;
            ticketComment.TicketId = ticketid;

            if (ticketid != null && !string.IsNullOrEmpty(comment))
            {
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
            }

        }
        
    }

}