using ExamenTiendaLibros.Data;
using ExamenTiendaLibros.Extensions;
using ExamenTiendaLibros.Filters;
using ExamenTiendaLibros.Models;
using ExamenTiendaLibros.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamenTiendaLibros.Controllers
{
    public class LibrosController : Controller
    {
        private RepositoryLibros repo;
        private LibrosContext context;

        public LibrosController(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Libro> libros = await this.repo.GetLibrosAsync();
            return View(libros);
        }

        public async Task<IActionResult> Details(int idLibro)
        {
            Libro libro = await this.repo.FindLibroAsync(idLibro);
            return View(libro);
        }

        public async Task<IActionResult> ListarLibrosGeneros(int idGenero)
        {
            List<Libro> librosGeneros = this.repo.GetLibrosGeneros(idGenero);
            return View(librosGeneros);
        }

        public async Task<IActionResult> Carrito()
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (carrito != null)
            {
                List<Libro> libros = await this.repo.GetLibrosSessionAsync(carrito);
                return View(libros);
            }
            return View();
        }

        public IActionResult AñadirLibroCarrito(int? idLibro)
        {
            if (idLibro != null)
            {
                List<int> carrito;
                if(HttpContext.Session.GetString("CARRITO") == null)
                {
                    carrito = new List<int>();
                }
                else
                {
                    carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
                }
                carrito.Add(idLibro.Value);
                ViewData["NUMLIBROS"] = carrito;
                HttpContext.Session.SetObject("CARRITO", carrito);
            }
            return RedirectToAction("Carrito", "Libros");
        }

        public async Task<IActionResult> EliminarLibroCarrito(int? idLibro)
        {
            if(idLibro != null)
            {
                List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
                carrito.Remove(idLibro.Value);
                if(carrito.Count() == 0)
                {
                    HttpContext.Session.Remove("CARRITO");
                }
                else
                {
                    HttpContext.Session.SetObject("CARRITO", carrito);
                }

            }
            return RedirectToAction("Carrito", "Libros");
        }

        [AuthorizeLibros]
        public async Task<IActionResult> FinalizarCompra()
        {
            int usuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value); 

            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");

            List<Libro> librosCarrito = await this.repo.GetLibrosCestaAsync(carrito);

            await this.repo.CrearPedidoAsync(usuario, librosCarrito);

            HttpContext.Session.Remove("CARRITO");

            return RedirectToAction("Index", "Libros");
        }

        public async Task<IActionResult> PedidosUsuario()
        {
            int usuario = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            List<VistaPedidosModel> pedidosUsuario = await this.repo.GetLibrosPedidoUsuarioAsync(usuario);
            return View(pedidosUsuario);
        }
    }
}
