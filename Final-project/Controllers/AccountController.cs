using Final_project.Data;
using Final_project.Entities;
using Final_project.Models;
using Final_project.Services.Interfaces;
using Final_project.Utilities.Extensions;
using Final_project.Utilities.Roles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata.Ecma335;

namespace Final_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly MichaelDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public AccountController(MichaelDbContext context, IWebHostEnvironment env, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            TempData["Login"] = false;

            if (!ModelState.IsValid) return View();

            User user = await _userManager.FindByEmailAsync(login.Email);

            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }

            if (user.EmailConfirmed != true)
            {
                ModelState.AddModelError("", "Please, check your email");
                return View();
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Due to overtyring your account has been blocked for 5 minutes");
                    return View();
                }
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            TempData["Login"] = true;

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel account)
        {
            TempData["Register"] = false;
            if (!ModelState.IsValid) return View();
            if (!account.Terms)
            {
                ModelState.AddModelError("Terms", "Please, accept the terms!");
                return View();
            }

            if (await _userManager.FindByNameAsync(account.Username) != null)
            {
                ModelState.AddModelError("", "This username already in use");
                return View();
            }

            if (await _userManager.FindByEmailAsync(account.Email) != null)
            {
                ModelState.AddModelError("", "This Email address already in use");
                return View();
            }

            User user = new User
            {
                UserName = account.Username,
                Name = account.Firstname,
                Email = account.Email,
                SurName = account.Lastname
            };

            IdentityResult result = await _userManager.CreateAsync(user, account.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError message in result.Errors)
                {
                    ModelState.AddModelError("", message.Description);
                }
                return View();
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action(nameof(VerifyEmail), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("kamraneze@code.edu.az", "MichaelKors");
            mail.To.Add(new MailAddress(user.Email));

            mail.Subject = "Verify Email";
            mail.Body = string.Empty;
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/assets/template/verifyemail.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{userFullName}}", user.Name + " " + user.SurName);
            mail.Body = body.Replace("{{link}}", link);
            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("hellojob440@gmail.com", "eomddhluuxosvnoy");
            smtp.Send(mail);

            await _userManager.AddToRoleAsync(user, Roles.User.ToString());
            await _context.SaveChangesAsync();
            TempData["Register"] = true;
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }

        public async Task CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AccountDetail()
        {
            User user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new AccountDetailViewModel
            {
                FirstName = user.Name,
                LastName = user.SurName,
                EmailAddress = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AccountDetail(AccountDetailViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            User user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.Name = viewModel.FirstName;
            user.SurName = viewModel.LastName;
            user.Email = viewModel.EmailAddress;
            user.PhoneNumber = viewModel.PhoneNumber;

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to update user information.");
                return View(viewModel);
            }

            return View(viewModel);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
                return RedirectToAction("Login", "Account");
            }

            var currentPasswordIsValid = await _userManager.CheckPasswordAsync(currentUser, model.CurrentPassword);

            if (!currentPasswordIsValid)
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "The current password is incorrect.");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.IsSuccess = true;
                ModelState.Clear();
                TempData["SuccessMessage"] = "Password changed successfully!";
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> ForgotPasswordPage()
        {
            return View();
        }


        public async Task<IActionResult> ForgotPassword(AccountViewModel account)
        {
            TempData["ForgotPassword"] = false;
            if (account.User.Email is null) return Redirect(Request.Headers["Referer"].ToString());
            User user = await _userManager.FindByEmailAsync(account.User.Email);

            if (user is null) return Redirect(Request.Headers["Referer"].ToString());

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string? link = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());


            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("fidanji@code.edu.az", "MickaelKors");
            mail.To.Add(new MailAddress(user.Email));

            mail.Subject = "Reset Password";
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/assets/template/resetpassword.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{userFullName}}", user.Name + user.SurName);
            mail.Body = body.Replace("{{link}}", link);
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;


            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("fidanji@code.edu.az", "As1ad2af3");

            smtp.Send(mail);
            TempData["ForgotPassword"] = true;

            return Redirect(Request.Headers["Referer"].ToString());

        }



        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (email == null)
            {
                return BadRequest();
            }

            User user = await _userManager.FindByEmailAsync(email);
            if (user == null) BadRequest();

            AccountViewModel model = new()
            {
                User = user,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(AccountViewModel account)
        {
            TempData["Security"] = false;
            User user = await _userManager.FindByEmailAsync(account.User.Email);
            AccountViewModel model = new()
            {
                User = user,
                Token = account.Token,
                Password = account.Password,
                ConfirmPassword = account.ConfirmPassword
            };
            if (!ModelState.IsValid)
            {
                foreach (string message in ModelState.Values.SelectMany(v => v.Errors)
                                   .Select(e => e.ErrorMessage))
                {
                    ModelState.AddModelError("", message);
                }
                return View(account);
            };

            await _userManager.ResetPasswordAsync(user, account.Token, account.Password);
            TempData["Security"] = true;
            return RedirectToAction("Login", "Account");
        }
    }
}
