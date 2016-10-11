using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProjectUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: ProjectUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            if (projectUser == null)
            {
                return HttpNotFound();
            }
            return View(projectUser);
        }

        // GET: ProjectUsers/Create
        public async Task<ActionResult> Create()
        {
            var userid = User.Identity.GetUserId();
            var helper = new UserProjectsHelper();
            if (!User.IsInRole("Administrator"))
            {
                var projects = await helper.ListProjectsForUser(userid);
                ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            }
            else
            {
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");

            }

            
            return View();
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProjectId,UserId")] ProjectUser projectUser)
        {
            var uhelper = new UserRolesHelper();
            if (ModelState.IsValid)
            {
                projectUser.UserId = uhelper.findIdFromEmail(projectUser.UserId);
                db.ProjectUsers.Add(projectUser);
                db.SaveChanges();
                return RedirectToAction("MyProjects", "Projects");
            }

            return View(projectUser);
        }

        // GET: ProjectUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUser projectUser = db.ProjectUsers.Find(id);
            if (projectUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectUser.ProjectId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", projectUser.UserId);
            return View(projectUser);
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,UserId")] ProjectUser projectUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", projectUser.ProjectId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", projectUser.UserId);
            return View(projectUser);
        }

        // GET: ProjectUsers/Delete/5
        public async Task< ActionResult> Delete()
        {
 
            
            var userid = User.Identity.GetUserId();
            var helper = new UserProjectsHelper();
            var users = await helper.UsersOnSameProjects(userid);
            var uniqueCollection = users.GroupBy(x => x.Id).Select(y => y.First());
            ViewBag.UserId = new SelectList(uniqueCollection, "Id", "Email");
            var projects = await helper.ListProjectsForUser(userid);
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            return View();
        }

        // POST: ProjectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([Bind(Include = "Id,ProjectId,UserId")] ProjectUser projectUser)
        {

            var pid = projectUser.ProjectId;
            var uid = projectUser.UserId;
            var pu=db.ProjectUsers.Single(p => p.ProjectId == pid && p.UserId == uid);
            db.ProjectUsers.Remove(pu);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
