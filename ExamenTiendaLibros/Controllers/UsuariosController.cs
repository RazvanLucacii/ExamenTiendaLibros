using ExamenTiendaLibros.Filters;
using ExamenTiendaLibros.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExamenTiendaLibros.Controllers
{
    public class UsuariosController : Controller
    {
        private RepositoryLibros repo;

        public UsuariosController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        [AuthorizeLibros]
        public async Task<IActionResult> PerfilUsuario()
        {
            return View();
        }
    }
}
