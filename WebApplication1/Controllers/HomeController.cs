using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PagedList;
using WebApplication1.Models;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> manager =
            new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(
            new ApplicationDbContext()));
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!User.Identity.IsAuthenticated)
            {
                 return RedirectToAction("LandingPage");
            }
            ViewBag.CurrentSort = sortOrder;

            var db = new ApplicationDbContext();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.CreatedSortParm = sortOrder == "Created" ? "created_desc" : "Created";
            ViewBag.OwnerSortParm = sortOrder == "Owner" ? "owner_desc" : "Owner";
            ViewBag.AssignmentSortParm = sortOrder == "Assignment" ? "assignment_desc" : "Assignment";
            ViewBag.UpdatedSortParm = sortOrder == "Updated" ? "updated_desc" : "Updated";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";
            ViewBag.PrioritySortParm = sortOrder == "Priority" ? "priority_desc" : "Priority";
            ViewBag.ProjectSortParm = sortOrder == "Project" ? "project_desc" : "Project";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";
            
            var tickets = from m in db.Tickets
                        select m;
            var user = manager.FindById(User.Identity.GetUserId());

            var ticketList = new List<Ticket>();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.Title.Contains(searchString) || t.Description.Contains(searchString) || t.Project.Name.Contains(searchString) || t.TicketType.Name.Contains(searchString) || t.TicketPriority.Name.Contains(searchString) || t.OwnerUser.Email.Contains(searchString) || t.OwnerUser.UserName.Contains(searchString) || t.AssignedUser.Email.Contains(searchString) || t.AssignedUser.UserName.Contains(searchString));
            }
            if (manager.IsInRole(user.Id,"Administrator"))
            {
                ticketList.AddRange(tickets);
            }
            if (manager.IsInRole(user.Id, "Submitter") && !manager.IsInRole(user.Id, "Administrator"))
            {
                var t= tickets.Where(s => s.OwnerUserId.Equals(user.Id));
                ticketList.AddRange(t);
            }
            if (manager.IsInRole(user.Id, "ProjectManager") && !manager.IsInRole(user.Id, "Administrator"))
            {
                //assumes first projectuser in project is project manager
                var helper = new UserProjectsHelper();
                var t = await helper.TicketsFromProjects(User.Identity.GetUserId());
                ticketList.AddRange(t);
            }
            if (manager.IsInRole(user.Id, "Developer") && !manager.IsInRole(user.Id, "Administrator"))
            {
                var t = tickets.Where(s => s.AssignedUserId.Equals(user.Id));
                ticketList.AddRange(t);
            }
            var qlist = ticketList.AsQueryable();

            switch (sortOrder)
            {
                case "title_desc":
                    qlist = qlist.OrderByDescending(s => s.Title);
                    break;
                case "Created":
                    qlist = qlist.OrderBy(s => s.Created);
                    break;
                case "created_desc":
                    qlist = qlist.OrderByDescending(s => s.Created);
                    break;
                case "Status":
                    qlist = qlist.OrderBy(s => s.TicketStatusId);
                    break;
                case "status_desc":
                    qlist = qlist.OrderByDescending(s => s.TicketStatusId);
                    break;
                case "Owner":
                    qlist = qlist.OrderBy(s => s.OwnerUser.Email);
                    break;
                case "owner_desc":
                    qlist = qlist.OrderByDescending(s => s.OwnerUser.Email);
                    break;
                case "Assignment":
                    qlist = qlist.OrderBy(s => s.AssignedUser.Email);
                    break;
                case "assignment_desc":
                    qlist = qlist.OrderByDescending(s => s.AssignedUser.Email);
                    break;
                case "Updated":
                    qlist = qlist.OrderBy(s => s.Updated);
                    break;
                case "updated_desc":
                    qlist = qlist.OrderByDescending(s => s.Updated);
                    break;
                case "Type":
                    qlist = qlist.OrderBy(s => s.TicketTypeId);
                    break;
                case "type_desc":
                    qlist = qlist.OrderByDescending(s => s.TicketTypeId);
                    break;
                case "Priority":
                    qlist = qlist.OrderBy(s => s.TicketPriorityId);
                    break;
                case "priority_desc":
                    qlist = qlist.OrderByDescending(s => s.TicketPriorityId);
                    break;
                case "Project":
                    qlist = qlist.OrderBy(s => s.Project.Name);
                    break;
                case "project_desc":
                    qlist = qlist.OrderByDescending(s => s.Project.Name);
                    break;
                default:
                    qlist = qlist.OrderByDescending(s => s.Created);
                    break;

            }
            
            return View(qlist.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult LandingPage()
        {
            
            return View();
        }
    }
}