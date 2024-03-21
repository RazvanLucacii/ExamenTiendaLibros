using ExamenTiendaLibros.Models;
using ExamenTiendaLibros.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExamenTiendaLibros.ViewComponents
{
    public class MenuGenerosViewComponent : ViewComponent
    {
        private RepositoryLibros repo;

        public MenuGenerosViewComponent(RepositoryLibros repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Genero> generos = await this.repo.GetGenerosAsync();
            return View(generos);
        }
    }
}
