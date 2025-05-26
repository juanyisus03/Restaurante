using Restaurante.Models;
using SQLite;

namespace Restaurante.Services;

// Servicio para gestionar los elementos del menú
class ElementoMenuService
{
    private readonly SQLiteAsyncConnection _connection;
    private static ElementoMenuService? instance;

    // Constructor privado para Singleton
    private ElementoMenuService(string dbpath)
    {
        _connection = new SQLiteAsyncConnection(dbpath);
        _connection.CreateTableAsync<ElementoMenu>().Wait();
    }

    // Obtener instancia del servicio (Singleton)
    public static ElementoMenuService GetInstance()
    {
        string dbpath = Path.Combine(FileSystem.AppDataDirectory, "restaurante.db");
        return instance ??= new ElementoMenuService(dbpath);
    }

    // Verifica si hay registros en la base de datos
    public async Task<bool> BaseDeDatosTieneRegistros()
    {
        var count = await _connection.Table<ElementoMenu>().CountAsync();
        return count > 0;
    }

    // Carga elementos iniciales si no hay registros
    public async Task CargarDatos()
    {
        try
        {
            if (await BaseDeDatosTieneRegistros())
                return;

            var elementosIniciales = new List<ElementoMenu>
            {
                new ElementoMenu { Nombre = "Hamburguesa", Precio = 5.50f, Tipo = ElementoMenu.TipoElementoMenu.Plato },
                new ElementoMenu { Nombre = "Pizza Margarita", Precio = 10.00f, Tipo = ElementoMenu.TipoElementoMenu.Plato },
                new ElementoMenu { Nombre = "Ensalada César", Precio = 5.50f, Tipo = ElementoMenu.TipoElementoMenu.Plato },
                new ElementoMenu { Nombre = "Coca Cola", Precio = 1.50f, Tipo = ElementoMenu.TipoElementoMenu.Bebida },
                new ElementoMenu { Nombre = "Jugo de Naranja", Precio = 1.40f, Tipo = ElementoMenu.TipoElementoMenu.Bebida },
                new ElementoMenu { Nombre = "Agua Mineral", Precio = 1.00f, Tipo = ElementoMenu.TipoElementoMenu.Bebida },
                new ElementoMenu { Nombre = "Pastel de Chocolate", Precio = 6.00f, Tipo = ElementoMenu.TipoElementoMenu.Postre },
                new ElementoMenu { Nombre = "Helado de Vainilla", Precio = 4.00f, Tipo = ElementoMenu.TipoElementoMenu.Postre }
            };

            await _connection.InsertAllAsync(elementosIniciales);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al cargar datos: {ex.Message}", "OK");
        }
    }

    // Crear un nuevo elemento en el menú
    public async Task<int> CrearElementoMenu(ElementoMenu elementoMenu)
    {
        return await _connection.InsertAsync(elementoMenu);
    }

    // Obtener un elemento del menú por ID
    public async Task<ElementoMenu> ObtenerElementoMenuAsync(int id)
    {
        return await _connection.Table<ElementoMenu>().Where(u => u.Id == id).FirstAsync();
    }

    // Obtener elementos del menú pedidos por una mesa
    public async Task<List<ElementoMenu>> ObtenerElementoMenuFromMesaAsync(int id)
    {
        List<Pedido> pedidos = await _connection.Table<Pedido>().Where(u => u.MesaId == id).ToListAsync();
        var elementos = new List<ElementoMenu>();

        for (int i = 0; i < pedidos.Count; i++)
        {
            var pedido = pedidos[i];
            var elemento = await _connection.Table<ElementoMenu>().Where(u => u.Id == pedido.ElementoMenu).FirstAsync();
            elementos.Add(elemento);
        }

        return elementos;
    }

    // Obtener todos los elementos del menú
    public async Task<List<ElementoMenu>> ObtenerElementoMenusAsync()
    {
        return await _connection.Table<ElementoMenu>().ToListAsync();
    }

    // Borrar un elemento del menú
    public async Task<int> borrarElementoMenu(ElementoMenu em)
    {
        await PedidosService.GetInstance().BorrarPedidosElementoMenuAsync(em.Id);
        return await _connection.Table<ElementoMenu>().Where(u => u.Id == em.Id).DeleteAsync();
    }

    // Actualizar un elemento del menú
    public async Task<int> ActualizarElementoMenu(ElementoMenu editado)
    {
        return await _connection.UpdateAsync(editado);
    }
}
