using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SchoolProj.Enums;
using SchoolProj.Models;

namespace SchoolProj.Controllers
{
    public class StudentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetStudents()
        {
            var raw_query = db.Students.ToList();
            var students = raw_query.Select(row => new
            {
                
                Name = row.Name,
                Gender = row.Gender == Gender.Female ? "Female" : "Male",
                RollNo = row.RollNo,
                Age = row.Age,
                PhoneNo = row.PhoneNo,
                Email = row.Email,
                Address = row.Address,
                Id = row.Id
            });
            return Json(new { success = true, data = students }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentById(long? id)
        {
            if (!id.HasValue)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var student = db.Students.Find(id);
            if (student != null)
                return Json(new { success = true, data = student }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Student/Details/5
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

        // GET: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Id,Name,Gender,RollNo,Age,PhoneNo,Email,Address")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }

                var errorList = ModelState.Where(y => y.Value.Errors.Count > 0).Select(x => new { x.Value.Errors, x.Key }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit([Bind(Include = "Id,Name,Gender,RollNo,Age,PhoneNo,Email,Address")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Student/Delete/5
        [HttpPost]
        public JsonResult Delete(long? id)
        {
            if (id == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            Student student = db.Students.Find(id);
            if (student != null)
            {
                db.Students.Remove(student);
                db.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
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
