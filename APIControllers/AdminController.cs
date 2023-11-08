using ClaimAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimAPI.Controllers
{
    public class AdminController : Controller
    {

        REALESTATEEntities db = new REALESTATEEntities();
        // GET: Admin
        public ActionResult Allerror()
        {

            var allpost = (from e in db.RealAllErrors orderby e.id descending select e).ToList();


            return View(allpost);
        }

        public ActionResult Allpost()
        {

            var allpost = (from e in db.RealPosts orderby e.id descending select e).ToList();


            return View(allpost);
        }


        public ActionResult Allcomment()
        {

            var allpost = (from e in db.RealComments orderby e.id descending select e).ToList();


            return View(allpost);
        }

        public ActionResult Alluser()
        {

            var allpost = (from e in db.RealREGUSERs orderby e.id descending select e).ToList();


            return View(allpost);
        }


        public ActionResult Allsavedpost()
        {

            var allpost = (from e in db.RealSavedposts orderby e.id descending select e).ToList();


            return View(allpost);
        }

        public ActionResult Allrequest()
        {

            var allpost = (from e in db.Realrequests orderby e.id descending select e).ToList();


            ViewBag.all = allpost;
            return View();
        }

        public ActionResult Delete(int? id,string name)
        {

            if(name == "error")
            {

                var allpost = (from e in db.RealAllErrors where e.id == id select e).FirstOrDefault();

                db.RealAllErrors.Remove(allpost);
                db.SaveChanges();

                return RedirectToAction("Allerror");
            }


            if (name == "post")
            {

                var allpost = (from e in db.RealPosts where e.id == id select e).FirstOrDefault();

                db.RealPosts.Remove(allpost);
                db.SaveChanges();


                return RedirectToAction("Allpost");
            }


            if (name == "comment")
            {

                var allpost = (from e in db.RealComments where e.id == id select e).FirstOrDefault();

                db.RealComments.Remove(allpost);
                db.SaveChanges();


                return RedirectToAction("Allcomment");
            }


            if (name == "user")
            {

                var allpost = (from e in db.RealREGUSERs where e.id == id select e).FirstOrDefault();

                db.RealREGUSERs.Remove(allpost);
                db.SaveChanges();


                return RedirectToAction("Alluser");
            }

            if (name == "spost")
            {

                var allpost = (from e in db.RealSavedposts where e.id == id select e).FirstOrDefault();

                db.RealSavedposts.Remove(allpost);
                db.SaveChanges();


                return RedirectToAction("Allsavedpost");
            }

            if (name == "request")
            {

                var allpost = (from e in db.Realrequests where e.id == id select e).FirstOrDefault();

                db.Realrequests.Remove(allpost);
                db.SaveChanges();

                return RedirectToAction("Allrequest");
            }


            return View();
        }
    }
}