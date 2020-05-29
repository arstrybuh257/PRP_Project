using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.WEB.Models;

namespace GainBargain.WEB.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult RegisterPartial()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult LoginPartial()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult LoginPartial(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (GainBargainContext db = new GainBargainContext())
            {
                bool isValid = db.Users.Any(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password));

                if (!isValid)
                {
                    ModelState.AddModelError("", "Credentials are wrong");
                    return View(model);
                }

                User user = db.Users.FirstOrDefault(x => x.Email == model.Email);

                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Email, model.RememberMe));
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords are not confirmed");
                return View(model);
            }

            using (GainBargainContext db = new GainBargainContext())
            {
                if (db.Users.Any(x=>x.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already taken");
                    model.Email = "";
                    return View(model);
                }

                User user = new User()
                {
                    Email = model.Email,
                    Password = model.Password
                };

                db.Users.Add(user);
                db.SaveChanges();

                UserRole userRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRole);
                db.SaveChanges();
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult Login()
        {
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("UserProfile");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (GainBargainContext db = new GainBargainContext())
            {
                bool isValid = db.Users.Any(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password));

                if (!isValid)
                {
                    ModelState.AddModelError("", "Credentials are wrong");
                    return View(model);
                }

                User user = db.Users.FirstOrDefault(x => x.Email == model.Email);

                FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                return Redirect(FormsAuthentication.GetRedirectUrl(model.Email, model.RememberMe));
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}