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
    public class TeachersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Teachers
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTeachers()
        {
            var raw_query = db.Teachers.ToList();
            var teachers = raw_query.Select(row => new
            {
                Name = row.Name,
                Gender = row.Gender.ToString(),
                Department = row.Department.ToString(),
                PhoneNo = row.PhoneNo,
                Email = row.Email,
                Address = row.Address,
                Id = row.Id
            });
            return Json(new { success = true, data = teachers }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTeacherById(long? id)
        {
            if (!id.HasValue)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var teacher = db.Teachers.Find(id);
            if (teacher != null)
                return Json(new { success = true, data = teacher }, JsonRequestBehavior.AllowGet);

            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        // GET: Teachers/Details/5
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "Id,Name,Gender,Department,PhoneNo,Email,Address")] Teacher teacher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Teachers.Add(teacher);
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
        public JsonResult Edit([Bind(Include = "Id,Name,Gender,Department,PhoneNo,Email,Address")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long? id)
        {
            if (id == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            Teacher teacher = db.Teachers.Find(id);
            if (teacher != null)
            {
                db.Teachers.Remove(teacher);
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
