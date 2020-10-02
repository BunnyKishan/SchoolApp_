using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolProj.Common;
using System.Linq.Dynamic;
using SchoolProj.Models;

namespace SchoolProj.Controllers
{
    public class TeachersBoostrapTableController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TeachersBoostrapTable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetTeachers(FilterInfor filtersInfor)
        {
            // handle filter
            var query = from t in db.Teachers
                        select new
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Gender = t.Gender.ToString(),
                            Department = t.Department.ToString(),
                            PhoneNo = t.PhoneNo,
                            Email = t.Email,
                            Address = t.Address
                        };

            var filtered = query.ApplyFilter(filtersInfor);
            return Content(CommonMethods.SerializeToJson(filtered, filtersInfor), "application/json");
        }





        // GET: TeachersBoostrapTable/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: TeachersBoostrapTable/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TeachersBoostrapTable/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Gender,Department,PhoneNo,Email,Address")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Teachers.Add(teacher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        // GET: TeachersBoostrapTable/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: TeachersBoostrapTable/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Gender,Department,PhoneNo,Email,Address")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // POST: TeachersBoostrapTable/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            Teacher teacher = db.Teachers.Find(id);
            db.Teachers.Remove(teacher);
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
    }
}
