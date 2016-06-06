using BoxService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BoxService.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        // GET: Users
        public ActionResult Index()
        {          
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewBag.isAdmin = "No";

                if (isAdminUser())
                {
                    ViewBag.isAdmin = "Yes";
                    return View();
                }
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();
        }
        [Authorize]
        public ActionResult UserProfile()
        {
            if (isAdminUser())
            {
                ViewBag.isAdmin = "Yes";
            }
            var currentUserID = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(currentUserID);
            return View(currentUser);

        }
        [Authorize]
        public ActionResult Admin()
        {
            GroupedAdminViewModel adminViewModel = new GroupedAdminViewModel();
            var allUsers = db.Users.ToList();
            adminViewModel.Users = allUsers.Select(u => new AdminViewModel { UserName = u.UserName, BoxName = u.UserBox, InvoiceBox = db.FoodBoxes.First(x => x.ID == u.FoodBoxID) }).ToList();
            foreach (var u in adminViewModel.Users)
            {
                adminViewModel.TotalSales += u.InvoiceBox.Price;
            }
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
                ViewBag.isAdmin = "No";

                if (isAdminUser())
                {
                    ViewBag.isAdmin = "Yes";
                    return View(adminViewModel);
                }
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangeBox()
        {
            ViewBag.Boxes = new SelectList(db.FoodBoxes.ToList(), "Name", "Name");
            var currentUserID = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserID);
            return View(currentUser);
        }
        [HttpPost]
        public ActionResult ChangeBox(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.FoodBoxID = db.FoodBoxes.First(x => x.Name == user.UserBox).ID;
                user.Dues = db.FoodBoxes.First(x => x.ID == user.FoodBoxID).Price;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserProfile", "Users");
            }
            return View(user);
        }
        [Authorize]
        [HttpGet]
        public ActionResult SuggestBox()
        {
            var currentUserID = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserID);
            if (user.Likes.ToLower().Replace(" ", "") == "yes")
            {
                ViewBag.SuggestedBoxes = new SelectList(db.FoodBoxes.Where(x => x.FlavorProfile >= 5).ToList(), "Name", "Name");
            }
            else
            {
                ViewBag.SuggestedBoxes = new SelectList(db.FoodBoxes.Where(x => x.FlavorProfile <= 5).ToList(), "Name", "Name");
            }
            return View(user);
        }
        [HttpPost]
        public ActionResult SuggestBox(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.FoodBoxID = db.FoodBoxes.First(x => x.Name == user.UserBox).ID;
                user.Dues = db.FoodBoxes.First(x => x.ID == user.FoodBoxID).Price;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserProfile", "Users");
            }
            return View(user);
        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}