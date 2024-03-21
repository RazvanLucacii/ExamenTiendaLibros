using ExamenTiendaLibros.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExamenTiendaLibros.Models;

namespace ExamenTiendaLibros.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryLibros repo;

        public ManagedController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login
            (string email, string password)
        {
            Usuario usuario = await
                this.repo.LoginUsuariosAsync(email, password);
            if (usuario != null)
            {
                //SEGURIDAD
                ClaimsIdentity identity =
                    new ClaimsIdentity(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name, "Pass");

                Claim claimName =
                    new Claim(ClaimTypes.Name, usuario.Nombre);
                identity.AddClaim(claimName);
                Claim claimId =
                    new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString());
                identity.AddClaim(claimId);
                Claim claimSurname =
                    new Claim(ClaimTypes.Surname, usuario.Apellidos);
                identity.AddClaim(claimSurname);
                Claim claimEmail =
                    new Claim(ClaimTypes.Email, usuario.Email);
                identity.AddClaim(claimEmail);
                Claim claimPassword =
                    new Claim("Pass", usuario.Password);
                identity.AddClaim(claimPassword);
                Claim claimFoto =
                    new Claim("Foto", usuario.Foto);
                identity.AddClaim(claimFoto);

                ClaimsPrincipal userPrincipal =
                    new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal);

                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();
                return RedirectToAction(action, controller);
            }
            else
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
