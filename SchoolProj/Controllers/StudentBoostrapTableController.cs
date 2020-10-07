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
    public class StudentBoostrapTableController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StudentBoostrapTable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetStudents(FilterInfor filtersInfor)
        {
            // handle filter
            var query = from t in db.Students
                        select new
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Gender = t.Gender.ToString(),
                            RollNo = t.RollNo,
                            Age = t.Age,
                            PhoneNo = t.PhoneNo,
                            Email = t.Email,
                            Address = t.Address
                        };

            var filtered = query.ApplyFilter(filtersInfor);
            return Content(CommonMethods.SerializeToJson(filtered, filtersInfor), "application/json");
        }





        // GET: StudentBoostrapTable/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: StudentBoostrapTable/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentBoostrapTable/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Gender,RollNo,Age,PhoneNo,Email,Address")] Student student)
        {
            object jsonObj = null;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();

                    jsonObj = new { success = true, id = student.Id };
                }
                else
                {
                    jsonObj = new { success = false, message = "Some fields are missing or invalid." };
                }
            }
            catch (Exception)
            {
                jsonObj = new { success = true, message = "Could not udpate, please try again." };
            }

            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }

        // GET: StudentBoostrapTable/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: StudentBoostrapTable/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Gender,RollNo,Age,PhoneNo,Email,Address")] Student student)
        {
            object jsonObj = null;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();

                    jsonObj = new { success = true, id = student.Id };
                }
                else
                {
                    jsonObj = new { success = false, message = "Some fields are missing or invalid." };
                }
            }
            catch (Exception ex)
            {
                jsonObj = new { success = true, message = "Could not udpate, please try again." };
            }

            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }

        // POST: StudentBoostrapTable/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            bool isSuccess = false;
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return Json(new { success = isSuccess }, JsonRequestBehavior.AllowGet);
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
