using DAL;
using EmailHandling;
using Models;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

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
        public ActionResult Subscribe(Models.User user, string NotifyCB = "off")
        {
            if (user == null)
                return View(new Models.User());

            user.Notify = NotifyCB == "on";
            user.Access = Access.View;
            user.Blocked = false;
            user.Verified = false;
            user.Online = false;

            Models.User.ConnectedUser = user;
            DB.Users.Add(user);
            Models.User.ConnectedUser = null;

            AccountsEmailing.SendEmailVerification(
                Url.Action("VerifyUser", "Accounts", null, Request.Url.Scheme),
                user
            );

            return Redirect("/Accounts/Login?message=Création de compte effectuée avec succès! Un courriel de confirmation vous a été envoyé.&success=true");
        }
        public ActionResult VerifyUser(string code)
        {
            UnverifiedEmail unverifiedEmail = DB.UnverifiedEmails.ToList()
                .FirstOrDefault(u => u.VerificationCode == code);

            if (unverifiedEmail != null)
            {
                Models.User user = DB.Users.Get(unverifiedEmail.UserId);

                DB.UnverifiedEmails.Delete(unverifiedEmail.Id);

                if (user != null)
                {
                    user.Verified = true;
                    Session["CurrentLoginEmail"] = user.Email;
                    DB.Users.Update(user);

                    AccountsEmailing.SendEmailUserStatusChanged(
                        "Votre adresse de courriel a été confirmée.",
                        user
                    );

                    return Redirect("/Accounts/Login?message=Votre adresse de courriel a été vérifiée avec succès!&success=true");
                }
            }

            return Redirect("/Accounts/Login?message=Erreur de vérification de courriel!&success=false");
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
        [UserAccess(Access.Admin)]
        public ActionResult ManageUsers()
        {
            ViewBag.PageTitle = "Gestion des usagers";
            return View();
        }

        [UserAccess(Access.Admin)]
        public ActionResult GetUsers(bool forceRefresh = false)
        {
            var users = DB.Users.ToList()
                .Where(u => Models.User.ConnectedUser == null || u.Id != Models.User.ConnectedUser.Id)
                .OrderBy(u => u.Name)
                .ToList();

            return PartialView(users);
        }

        [UserAccess(Access.Admin)]
        public ActionResult SetUserAccess(int userid, int access)
        {
            Models.User user = DB.Users.Get(userid);

            if (user != null && user.Id != 1)
            {
                user.Access = (Access)access;
                DB.Users.Update(user);
            }

            return null;
        }
        [UserAccess(Access.Admin)]
        public ActionResult ForceVerifyUser(int id)
        {
            Models.User user = DB.Users.Get(id);

            if (user != null && user.Id != 1)
            {
                user.Verified = true;
                DB.Users.Update(user);
            }

            return null;
        }


        [UserAccess(Access.Admin)]
        public ActionResult ToggleBlockUser(int id)
        {
            Models.User user = DB.Users.Get(id);

            if (user != null && user.Id != 1)
            {
                user.Blocked = !user.Blocked;
                user.Online = false;
                DB.Users.Update(user);
            }

            return null;
        }



        [UserAccess(Access.Admin)]
        public ActionResult DeleteUser(int id)
        {
            if (id != 1)
                DB.Users.Delete(id);

            return null;
        }

        private bool IsAdmin()
        {
            return Models.User.ConnectedUser != null &&
                   Models.User.ConnectedUser.Access == Access.Admin;
        }


    }
}