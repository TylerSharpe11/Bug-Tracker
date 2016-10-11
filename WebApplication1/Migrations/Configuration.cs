namespace WebApplication1.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApplication1.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication1.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "WebApplication1.Models.ApplicationDbContext";
        }

        protected override void Seed(WebApplication1.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            if(!context.Roles.Any(r => r.Name == "Administrator"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Administrator" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "ProjectManager" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Developer" };

                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Submitter" };

                manager.Create(role);
            }
            if (!context.Users.Any(u => u.Email == "sharpety12@ecualumni.ecu.edu"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser
                {
                    UserName = "sharpety",
                    Email = "sharpety12@ecualumni.ecu.edu",
                };

                manager.Create(user, "Password2");
                manager.AddToRoles(user.Id, new string[] { "Administrator", "Developer" });
            }

            var st = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(st);
            var userId = userManager.FindByEmail("sharpety12@ecualumni.ecu.edu").Id;
            var projectmanagerid = userManager.FindByEmail("jake17@email.com").Id;
            var developerid = userManager.FindByEmail("deangelo34@email.com").Id;
            var submitterid = userManager.FindByEmail("greg81@email.com").Id;
            var a = new ProjectUser {UserId = userId};
            var pm = new ProjectUser {UserId = projectmanagerid};
            var d = new ProjectUser {UserId = developerid};
            var sid = new ProjectUser {UserId = submitterid};
            var uset = new HashSet<ProjectUser> {a, pm, d, sid};
            var projects = new List<Project>
                {
                    new Project { Name = "The Big Lebowski2", ProjectUsers = uset},
                    new Project { Name = "Peterkin2" , ProjectUsers = uset },
                    new Project { Name = "Ferris2" , ProjectUsers = uset},
                    new Project { Name = "Newman2" , ProjectUsers = uset},
                    new Project { Name = "Kramer2", ProjectUsers = uset }
                };
                if (!context.Projects.Any(r => r.Name == "The Big Lebowski2"))
                {
                 
                    projects.ForEach(p => context.Projects.Add(p));
                    context.SaveChanges();
                }

                var priorities = new List<TicketPriority>
                    {
                        new TicketPriority { Name = "High" },
                        new TicketPriority { Name = "Medium" },
                        new TicketPriority { Name = "Low" }
                    };
                if (!context.TicketPriorities.Any(r => r.Name == "High"))
                    {
                        priorities.ForEach(t => context.TicketPriorities.Add(t));
                        context.SaveChanges();
                    }
            

                var types = new List<TicketType>
                {
                    new TicketType { Name = "Bug" },
                    new TicketType { Name = "Featue request" },
                    new TicketType { Name = "Improvement" }
                };
                
                if (!context.TicketTypes.Any(r => r.Name == "Bug"))
                {
                    types.ForEach(t => context.TicketTypes.Add(t));
                    context.SaveChanges();
                }
            
                var status = new List<TicketStatus>
                {
                    new TicketStatus { Name = "In progress" },
                    new TicketStatus { Name = "Not assigned" },
                    new TicketStatus { Name = "Closed" }
                };
                if (!context.TicketStatus.Any(r => r.Name == "Not assigned"))
                {
                    status.ForEach(s => context.TicketStatus.Add(s));
                    context.SaveChanges();
                }
                var typeId = context.TicketTypes.First(p => p.Name == "Bug").Id;
                var priorityid = context.TicketPriorities.First(p => p.Name == "Low").Id;
                var statusId = context.TicketStatus.First(p => p.Name == "In progress").Id;
                var project = context.Projects.Single(p => p.Name == "The Big Lebowski").Id;
                var project2 = context.Projects.Single(p => p.Name == "Peterkin").Id;
                var project3 = context.Projects.Single(p => p.Name == "Ferris").Id;
                var project4 = context.Projects.Single(p => p.Name == "Newman").Id;
                var project5 = context.Projects.Single(p => p.Name == "Kramer").Id;
                if (!context.ProjectUsers.Any(r => r.UserId == userId))
                {
                    context.ProjectUsers.Add(new ProjectUser()
                    {
                        ProjectId = project,
                        UserId = userId
                    });
                }
                var tickets = new List<Ticket>
                {

                    new Ticket {
                    Title = "Search is broken",
                    Description = "The search never returns results",
                    Created = System.DateTimeOffset.Now,
                    TicketPriorityId=priorityid,
                    ProjectId = project,
                    TicketTypeId = typeId,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Can't attach a file to a ticket",
                    Description = "I get an error undefined everytinme",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project2,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Can't reassign a ticket",
                    Description = "The drop down of users doesn't populate",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project3,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Can't change status of a ticket",
                    Description = "Error every time",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project4,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Can't create a new project",
                    Description = "Validation error",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project5,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Can't assign users to a ticket",
                    Description = "Drop down list doesn't populate",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project5,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                    new Ticket {
                    Title = "Sorting of rows not working",
                    Description = "When you click on a row nothing happens",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project4,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Create new ticket",
                    Description = "Need a textarea for description",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project3,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Timestamps are editable",
                    Description = "Really? How convenient",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project2,
                    TicketTypeId = typeId,
                    TicketStatusId = statusId,
                    TicketPriorityId=priorityid,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                new Ticket {
                    Title = "Save after editing broken",
                    Description = "More validation errors",
                    Created = System.DateTimeOffset.Now,
                    ProjectId = project,
                    TicketTypeId = typeId,
                    TicketPriorityId=priorityid,
                    TicketStatusId = statusId,
                    OwnerUserId = userId,
                    AssignedUserId = userId
                },
                };
                if (!context.Tickets.Any(r => r.ProjectId == project))
                {
                    tickets.ForEach(t => context.Tickets.Add(t));
                    context.SaveChanges();
                }
            if(context.Users.Count()<10)
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var p = "password";
                var user = new ApplicationUser
                {
                    UserName = "bill5",
                    Email = "bill5@email.com",
                };

                manager.Create(user, p);
                manager.AddToRoles(user.Id, "Developer");


                var user2 = new ApplicationUser
                {
                    UserName = "bob33",
                    Email = "bob33@email.com",
                };

                manager.Create(user2, p);
                manager.AddToRoles(user2.Id, "Developer");


                var user3 = new ApplicationUser
                {
                    UserName = "jane7",
                    Email = "jane7@email.com",
                };

                manager.Create(user3, p);
                manager.AddToRoles(user3.Id, "Developer");

                var user4 = new ApplicationUser
                {
                    UserName = "john21",
                    Email = "john21@email.com",
                };
                manager.Create(user4, p);
                manager.AddToRoles(user4.Id, "Developer");

                var user5 = new ApplicationUser
                {
                    UserName = "tyler87",
                    Email = "tyler87@email.com",
                };
                manager.Create(user5, p);
                manager.AddToRoles(user5.Id, "Developer");


                var user6 = new ApplicationUser
                {
                    UserName = "evan67",
                    Email = "evan67@email.com",
                };
                manager.Create(user6, p);
                manager.AddToRoles(user6.Id, "Developer");

                var user7 = new ApplicationUser
                {
                    UserName = "jill34",
                    Email = "jill34@email.com",
                };
                manager.Create(user7, p);
                manager.AddToRoles(user7.Id, "Developer");

                var user8 = new ApplicationUser
                {
                    UserName = "deangelo34",
                    Email = "deangelo34@email.com",
                };
                manager.Create(user8, p);
                manager.AddToRoles(user8.Id, "Developer");

                var user9 = new ApplicationUser
                {
                    UserName = "steve89",
                    Email = "steve89@email.com",
                };
                manager.Create(user9, p);
                manager.AddToRoles(user9.Id, "Submitter");


                var user10 = new ApplicationUser
                {
                    UserName = "jake17",
                    Email = "jake17@email.com",
                };
                manager.Create(user10, p);
                manager.AddToRoles(user10.Id, "ProjectManager");


                var user11 = new ApplicationUser
                {
                    UserName = "cam1",
                    Email = "cam1@email.com",
                };
                manager.Create(user11, p);
                manager.AddToRoles(user11.Id, "ProjectManager");


                var user12 = new ApplicationUser
                {
                    UserName = "greg81",
                    Email = "greg81@email.com",
                };
                manager.Create(user12, p);
                manager.AddToRoles(user12.Id, "Submitter");
            }
        }
    }
}
