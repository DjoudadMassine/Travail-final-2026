using DAL;
using Models;
using System.Linq;
using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class AccountsController : Controller
    {
        public JsonResult EmailExist(string Email)
        {
            return Json(
                DB.Users.ToList().Any(u => u.Email == Email),
                JsonRequestBehavior.AllowGet
            );
        }



        public JsonResult EmailAvailable(string Email)
        {
            int currentId = Models.User.ConnectedUser != null
                ? Models.User.ConnectedUser.Id
                : 0;

            Models.User foundUser = DB.Users.ToList()
                .FirstOrDefault(u => u.Email == Email && u.Id != currentId);

            return Json(foundUser != null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login(string message = "", bool success = true)
        {
            Models.User.ConnectedUser = null;

            Session["LoginSuccess"] = success;
            Session["LoginMessage"] = message;

            return View(new LoginCredential());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginCredential credential)
        {
            if (credential == null)
                return View(new LoginCredential());

            string email = credential.Email != null
                ? credential.Email.Trim().ToLower()
                : "";

            string password = credential.Password != null
                ? credential.Password.Trim()
                : "";

            Models.User loginUser = DB.Users.ToList()
                .FirstOrDefault(u =>
                    u.Email.ToLower() == email &&
                    u.Password == password);

            if (loginUser == null)
            {
                Session["LoginSuccess"] = false;
                Session["LoginMessage"] = "Courriel ou mot de passe incorrect";

                return View(new LoginCredential
                {
                    Email = credential.Email
                });
            }

            if (loginUser.Blocked)
            {
                Session["LoginSuccess"] = false;
                Session["LoginMessage"] = "Compte bloqué";

                return View(new LoginCredential());
            }

            Models.User.ConnectedUser = loginUser;

            Session["UserId"] = loginUser.Id;
            Session["UserName"] = loginUser.Name;
            Session["AccessLevel"] = loginUser.Access.ToString();

            loginUser.Online = true;
            DB.Users.Update(loginUser);

            return RedirectToAction("Index", "Students");
        }

        public ActionResult Logout()
        {
            if (Models.User.ConnectedUser != null)
            {
                Models.User.ConnectedUser.Online = false;
                DB.Users.Update(Models.User.ConnectedUser);
            }

            Models.User.ConnectedUser = null;

            Session.Clear();

            return RedirectToAction("Login");
        }

        public ActionResult ExpiredSession()
        {
            Session.Clear();

            return Redirect(
                "/Accounts/Login?message=Session expirée&success=false"
            );
        }

        public ActionResult Subscribe()
        {
            return View(new Models.User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(Models.User user)
        {
            if (user == null)
                return View(new Models.User());

            user.Access = Access.View;
            user.Blocked = false;
            user.Verified = true;
            user.Online = false;

            DB.Users.Add(user);

            return Redirect(
                "/Accounts/Login?message=Compte créé avec succès&success=true"
            );
        }

        public ActionResult EditProfil()
        {
            if (Models.User.ConnectedUser == null)
                return RedirectToAction("Login");

            return View(Models.User.ConnectedUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfil(Models.User user)
        {
            if (Models.User.ConnectedUser == null)
                return RedirectToAction("Login");

            Models.User connectedUser = Models.User.ConnectedUser;

            user.Id = connectedUser.Id;
            user.Access = connectedUser.Access;
            user.Blocked = connectedUser.Blocked;
            user.Verified = connectedUser.Verified;
            user.Online = connectedUser.Online;

            DB.Users.Update(user);

            Models.User.ConnectedUser = DB.Users.Get(user.Id);

            Session["UserName"] = user.Name;
            Session["AccessLevel"] = user.Access.ToString();

            return RedirectToAction("Index", "Students");
        }

        public ActionResult DeleteProfil()
        {
            if (Models.User.ConnectedUser == null)
                return RedirectToAction("Login");

            DB.Users.Delete(Models.User.ConnectedUser.Id);

            Models.User.ConnectedUser = null;

            Session.Clear();

            return Redirect(
                "/Accounts/Login?message=Compte supprimé&success=true"
            );
        }

        public ActionResult ManageUsers()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            return View(
                DB.Users.ToList()
                    .OrderBy(u => u.Name)
                    .ToList()
            );
        }

        public ActionResult SetUserAccess(int userid, int access)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            Models.User user = DB.Users.Get(userid);

            if (user != null && user.Id != 1)
            {
                user.Access = (Access)access;
                DB.Users.Update(user);
            }

            return RedirectToAction("ManageUsers");
        }

        public ActionResult ToggleBlockUser(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            Models.User user = DB.Users.Get(id);

            if (user != null && user.Id != 1)
            {
                user.Blocked = !user.Blocked;
                user.Online = false;

                DB.Users.Update(user);
            }

            return RedirectToAction("ManageUsers");
        }


        public ActionResult DeleteUser(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            if (id != 1)
                DB.Users.Delete(id);

            return RedirectToAction("ManageUsers");
        }

        private bool IsAdmin()
        {
            return Models.User.ConnectedUser != null &&
                   Models.User.ConnectedUser.Access == Access.Admin;
        }


    }
}