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
        public ActionResult ChangeBox()
        {
            ViewBag.Boxes = new SelectList(db.FoodBoxes.Distinct().ToList(), "Name", "Name");
            //user.FoodBoxID = db.FoodBoxes.Where(x => x.Name == )

            var currentUserID = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserID);
            // currentUser.UserBox = model.UserBox;
            // FoodBox box = db.FoodBoxes.First(x => x.Name == currentUser.UserBox);
            // currentUser.FoodBoxID = box.ID;
            // //db.Entry(boxupdate).State = EntityState.Modified;
            // db.SaveChanges();

            return View(currentUser);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangeBox(ApplicationUser model)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (ModelState.IsValid)
            {
                var currentUserID = User.Identity.GetUserId();
                var currentUser = db.Users.Find(currentUserID);
                currentUser.UserBox = model.UserBox;
                currentUser.FoodBoxID = db.FoodBoxes.Where(x => x.Name == model.UserBox).SingleOrDefault().ID;
                FoodBox box = db.FoodBoxes.First(x => x.ID == currentUser.FoodBoxID);
                currentUser.Dues = box.Price;
                return RedirectToAction("UserProfile", "Users");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
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