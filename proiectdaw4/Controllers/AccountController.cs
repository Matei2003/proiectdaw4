using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proiectdaw4.Data;
using proiectdaw4.Model;
using proiectdaw4.Services;
using System.Numerics;

namespace proiectdaw4.Controllers
{

    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly AuthenticationService _authService;
        private readonly ValidationService _validationService;
        private readonly CreateAccountService _createAccountService;
        private readonly BdContext _context;

        public AccountController(AuthenticationService authService, ValidationService validationService, BdContext context, CreateAccountService createAccountService)
        {
            _authService = authService;
            _validationService = validationService;
            _context = context;
            _createAccountService = createAccountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _authService.Login(email, password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Email sau parola gresite.";
                return View();
            }

            HttpContext.Session.SetString("User", user.FirstName + " " + user.LastName);
            HttpContext.Session.SetString("Email", email);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string phone, string password)
        {
            var (success, errorMessage) = await _createAccountService.Register(firstName, lastName, phone, email, password);

            if (!success)
            {
                ViewBag.ErrorMessage = errorMessage;
                return View();
            }

            return RedirectToAction("Login", "Account");

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            Response.Cookies.Delete(".AspNetCore.Session");

            return RedirectToAction("Index", "Home");
        }
    }

}
