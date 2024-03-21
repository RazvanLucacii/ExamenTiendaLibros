using ExamenTiendaLibros.Data;
using ExamenTiendaLibros.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#region Procedimientos almacenados

//create procedure SP_GENEROS_LIBROS
//(@idgenero int)
//as
//	select * from LIBROS
//	where LIBROS.IdGenero=@idgenero
//go

#endregion

namespace ExamenTiendaLibros.Repositories
{
    public class RepositoryLibros
    {
        private LibrosContext context;

        public RepositoryLibros(LibrosContext context)
        {
            this.context = context;
        }

        public async Task<List<Libro>> GetLibrosAsync()
        {
            return await this.context.Libros.ToListAsync();
        }

        public async Task<Libro> FindLibroAsync(int idLibro)
        {
            return await this.context.Libros.FirstOrDefaultAsync(x => x.IdLibro == idLibro);
        }

        public async Task<Usuario> LoginUsuariosAsync(string email, string password)
        {
            Usuario usuario = await this.context.Usuarios.Where(z => z.Email == email && z.Password == password).FirstOrDefaultAsync();
            return usuario;
        }

        public async Task<List<Genero>> GetGenerosAsync()
        {
            return await this.context.Generos.ToListAsync();
        }

        public List<Libro> GetLibrosGeneros(int idGenero)
        {
            string sql = "SP_GENEROS_LIBROS @idgenero";
            SqlParameter pamId = new SqlParameter("@idgenero", idGenero);
            var consulta = this.context.Libros.FromSqlRaw(sql, pamId);
            List<Libro> libros = consulta.ToList();
            return libros;
        }

        public async Task<List<Libro>> GetLibrosSessionAsync(List<int> libros)
        {
            return await this.context.Libros.Where(z => libros.Contains(z.IdLibro)).ToListAsync();
        }

        public async Task<int> GetMaxIdPedidoAsync()
        {
            if (this.context.Pedidos.Count() == 0)
            {
                return 1;
            };
            return await this.context.Pedidos.MaxAsync(x => x.IdPedido) + 1;
        }

        public async Task<int> GetMaxIdFacturaAsync()
        {
            if (this.context.Pedidos.Count() == 0)
            {
                return 1;
            };
            return await this.context.Pedidos.MaxAsync(x => x.IdFactura) + 1;
        }

        public async Task<List<Libro>> GetLibrosCestaAsync(List<int> idsLibros)
        {
            List<Libro> librosCesta = await this.context.Libros.Where(l => idsLibros.Contains(l.IdLibro)).ToListAsync();
            return librosCesta;
        }

        public async Task<Pedido> CrearPedidoAsync(int idUsuario, List<Libro> carrito)
        {

            var idlibro = 0;

            foreach (Libro libro in carrito)
            {
                idlibro = libro.IdLibro;
            }

            Pedido pedido = new Pedido
            {
                IdPedido = await GetMaxIdPedidoAsync(),
                IdFactura = await GetMaxIdFacturaAsync(),
                Fecha = DateTime.Now,
                IdLibro = idlibro,
                IdUsuario = idUsuario,
                Cantidad = 1
            };
            await this.context.Pedidos.AddAsync(pedido);
            await this.context.SaveChangesAsync();

            return pedido;
        }

        public async Task<List<VistaPedidosModel>> GetLibrosPedidoUsuarioAsync(int idUsuario)
        {
            return await context.VistaPedidos.Where(d => d.IdUsuario == idUsuario).ToListAsync();
        }
    }
}
