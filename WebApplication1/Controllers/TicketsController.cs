using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication1.Models;
using System.Net.Mail;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public async Task<IList<ApplicationUser>> help(string userid)
        {
            var userId = userid;
            var helper = new UserProjectsHelper();
            var users = await helper.ProjectManagersOnSameProjects(userid);
            return users;
        }
        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Details()
        {
            if (!Request["commentT"].IsNullOrWhiteSpace() && Request["ticketid"] != null)
            {
                string ticketId = Request["ticketid"];
                int t = Convert.ToInt32(ticketId);
                string comment = Request["commentT"];
                CreateComment(t, comment);
                return RedirectToAction("Details", new { id = ticketId });
            }

            return RedirectToAction("MyTickets");
        }

        // GET: Tickets/Create
        [Authorize]
        public async Task <ActionResult> Create()
        {
            
            var userid=User.Identity.GetUserId();
            var users = await this.help(userid);
            var helper = new UserProjectsHelper();

            var projects = await helper.ListProjectsForUser(userid);
            if (users.Count == 0)
            {
                users.Add(db.Users.Find(userid));
            }

            ViewBag.AssignedUserId = new SelectList(users, "Id", "Email");
            
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,TicketStatusId,TicketPriorityId,TicketTypeId,ProjectId,OwnerUserId,AssignedUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                
                ticket.Created = new DateTimeOffset(DateTime.Now);
                ticket.OwnerUserId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.AssignedUserId = new SelectList(db.Users,"Id", "Email", ticket.AssignedUserId);
            ViewBag.OwnerUserId = User.Identity.GetUserId();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            var userid = User.Identity.GetUserId();
            var users = await this.help(userid);
            var helper = new UserProjectsHelper();
            IList<ApplicationUser> userlist=await helper.UsersOnProject(ticket.ProjectId);
            IList<ApplicationUser> owner = await helper.FindTicketOwner(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedUserId = new SelectList(userlist, "Id", "Email", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(owner, "Id", "Email", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,TicketStatusId,TicketPriorityId,TicketTypeId,ProjectId,OwnerUserId,AssignedUserId")] Ticket ticket)
        {

            var u=User.Identity.GetUserId();
            ticket.Updated =new DateTimeOffset(DateTime.Now);
            var tempticket = ticket;
            if (ModelState.IsValid)
            {
                SaveTicketHistory(u, tempticket);

                db.Entry(ticket).State = EntityState.Modified;
                
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Email", ticket.AssignedUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Email", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }


        public void SaveTicketHistory(string userid, Ticket ticket)
        {
            //old ticket values
            Ticket t = db.Tickets.AsNoTracking().Single(tick => tick.Id == ticket.Id);

            TicketHistory tickethistory = new TicketHistory();
            tickethistory.UserId = userid;
            tickethistory.TicketId = ticket.Id;
            string time=new DateTimeOffset(DateTime.Now).ToString();
            if (ticket.Equals(t))
            {
                return;
            }
            if (!ticket.Title.Equals(t.Title))
            {
                tickethistory.Changed = time;
                tickethistory.OldValue = t.Title;
                tickethistory.NewValue = ticket.Title;
                tickethistory.Property = "Title";
                db.TicketHistories.Add(tickethistory);
                db.SaveChanges();

            }
            if (!ticket.Description.Equals(t.Description))
            {
                tickethistory.Changed = time;
                tickethistory.OldValue = t.Description;
                tickethistory.NewValue = ticket.Description;
                tickethistory.Property = "Description";
                db.TicketHistories.Add(tickethistory);
                db.SaveChanges();

            }
            if (!ticket.TicketTypeId.Equals(t.TicketTypeId))
            {
                tickethistory.Changed = time;
                tickethistory.OldValue = t.TicketType.Name;
                tickethistory.NewValue = ticket.TicketType.Name;
                tickethistory.Property = "TicketType";
                db.TicketHistories.Add(tickethistory);
                db.SaveChanges();

            }
            if (!ticket.TicketTypeId.Equals(t.TicketPriorityId))
            {
                tickethistory.Changed = time;
                tickethistory.OldValue = t.TicketPriority.Name;
                tickethistory.NewValue = ticket.TicketPriority.Name;
                tickethistory.Property = "TicketPriority";
                db.TicketHistories.Add(tickethistory);
                db.SaveChanges();
            }
            if (!ticket.AssignedUserId.Equals(t.AssignedUserId))
            {
                tickethistory.Changed = time;
                tickethistory.OldValue = t.AssignedUserId;
                tickethistory.NewValue = ticket.AssignedUserId;
                tickethistory.Property = "AssignedUserId";
                db.TicketHistories.Add(tickethistory);
                string assigneduserid=ticket.AssignedUserId;
                notify(assigneduserid, ticket);
                db.SaveChanges();
            }

        }

        public void notify(string userid, Ticket t)
        {
            try
            {
                var u= db.Users.Find(userid);
                string email=u.Email;
                //Replace this with your own correct Gmail Address

                string to = email;
                //Replace this with the Email Address 
                //to whom you want to send the mail

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.To.Add(to);
                mail.From = new MailAddress(from, "Admin", System.Text.Encoding.UTF8);
                mail.Subject = "You've been assigned a ticket";
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.Body = "You've been assigned this ticket"+t.Title+t.Description;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

                //Add the Creddentials- use your own email id and password
                System.Net.NetworkCredential nt =
                new System.Net.NetworkCredential(from, "SuperSecret");

                client.Port = 587; // Gmail works on this port
                client.EnableSsl = true; //Gmail works on Server Secured Layer
                client.UseDefaultCredentials = false;
                client.Credentials = nt;
                client.Send(mail);

            }
            catch (Exception ex)
            {
                Response.Write("Could not send the e-mail - error: " + ex.Message);
            }
        }


        // GET: Tickets/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult MyTickets(int? page)
        {
            var userid=User.Identity.GetUserId();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var tickets = db.Tickets.Where(t => t.OwnerUserId == userid);
            var ticketList = new List<Ticket>();
            var qlist = tickets.AsQueryable();

            return View(qlist.OrderByDescending(i => i.Created).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        [Authorize]
        public ActionResult MyTickets(HttpPostedFileBase uploadFile)
        {
            var file = uploadFile;
            if (file != null && file.ContentLength > 0 && Request["description"]!=null)
            {

                var ticketAttachment=new TicketAttachment();
                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);

                ticketAttachment.TicketId = Convert.ToInt32(Request["ticketid"]);
                // then save on the server...
                var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
                file.SaveAs(path);
                ticketAttachment.FilePath = path;
                ticketAttachment.FileUrl = "~/uploads/"+fileName;
                ticketAttachment.Created = new DateTimeOffset(DateTime.Now);
                var userid = User.Identity.GetUserId();
                ticketAttachment.UserId = userid;

                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            if (!Request["commentT"].IsNullOrWhiteSpace() && Request["ticketid"]!= null)
            {
                string ticketId = Request["ticketid"];
                int t = Convert.ToInt32(ticketId);
                string comment = Request["commentT"];
                CreateComment(t, comment);
                return RedirectToAction("Details", new { id = ticketId });
            }

            return RedirectToAction("Index", "Home");
        }
        public ActionResult AssignedTickets()
        {
            var userid = User.Identity.GetUserId();
            var tickets = db.Tickets.Where(t => t.AssignedUserId == userid);
            return View(tickets.ToList());
        }

        public void CreateComment(int ticketid, string comment)
        {
            var ticketComment = new TicketComment();
            var userId = User.Identity.GetUserId();
            ticketComment.UserId = userId;
            ticketComment.Created = new DateTimeOffset(DateTime.Now);
            ticketComment.TicketId = ticketid;
            ticketComment.Comment = comment;

            if (ticketid!=null && !string.IsNullOrEmpty(comment))
            {
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
            }

        }
    }
}


