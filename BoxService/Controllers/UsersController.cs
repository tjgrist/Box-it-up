using BoxService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
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
            FoodBox box = db.FoodBoxes.First(x => x.ID == currentUser.FoodBoxID);
            currentUser.Dues = box.Price;
            db.SaveChanges();
            return View(currentUser);

        }
        [Authorize]
        public ActionResult Admin()
        {
            GroupedAdminViewModel adminViewModel = new GroupedAdminViewModel();
            var allUsers = db.Users.ToList();
            adminViewModel.Users = allUsers.Select(u => new AdminViewModel { UserName = u.UserName, Box = u.UserBox, InvoiceBox = db.FoodBoxes.First(x => x.ID == u.FoodBoxID) }).ToList();
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
        public ActionResult ChangeBox(int? id)
        {
            ViewBag.Boxes = new SelectList(db.FoodBoxes.Distinct().ToList(), "Name", "Name");
            var currentUserID = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(currentUserID);
            var boxupdate = db.FoodBoxes.Where(x => x.ID == currentUser.FoodBoxID);
            UpdateModel(currentUser);
            db.SaveChanges();
            db.SaveChanges();
            return View(currentUser);
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