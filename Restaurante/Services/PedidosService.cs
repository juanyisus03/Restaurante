using Restaurante.Models;
using SQLite;

namespace Restaurante.Services;

// Servicio para gestionar los pedidos
class PedidosService
{
    private readonly SQLiteAsyncConnection _connection;
    private static PedidosService? instance;

    // Constructor privado para patrón Singleton
    private PedidosService(string dbpath)
    {
        _connection = new SQLiteAsyncConnection(dbpath);
        _connection.CreateTableAsync<Pedido>().Wait();
    }

    // Obtener instancia del servicio (Singleton)
    public static PedidosService GetInstance()
    {
        string dbpath = Path.Combine(FileSystem.AppDataDirectory, "restaurante.db");
        return instance ??= new PedidosService(dbpath);
    }

    // Verifica si hay pedidos en la base de datos
    public async Task<bool> BaseDeDatosTieneRegistros()
    {
        var count = await _connection.Table<Pedido>().CountAsync();
        return count > 0;
    }

    // Carga datos de ejemplo si no hay registros
    public async Task CargarDatos()
    {
        try
        {
            if (await BaseDeDatosTieneRegistros())
                return;

            List<Mesa> mesas = await _connection.Table<Mesa>().ToListAsync();
            List<ElementoMenu> elementos = await _connection.Table<ElementoMenu>().ToListAsync();
            

            Random randomMesa = new Random();
            Random randomElemento = new Random();

            for (int i = 0; i < 10; i++)
            {
                Pedido pedido = new Pedido
                {
                    MesaId = mesas[randomMesa.Next(mesas.Count)].Numero,
                    ElementoMenu = elementos[randomElemento.Next(elementos.Count)].Id,
                    Cantidad = randomMesa.Next(1, 6)
                };
                await CrearPedido(pedido);
            }

            
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al cargar datos: {ex.Message}", "OK");
        }
    }

    // Crea un nuevo pedido o actualiza uno existente si ya hay uno igual
    public async Task<int> CrearPedido(Pedido nuevoPedido)
    {
        var pedidoExistente = await _connection.Table<Pedido>()
            .Where(p => p.MesaId == nuevoPedido.MesaId && p.ElementoMenu == nuevoPedido.ElementoMenu)
            .FirstOrDefaultAsync();

        if (pedidoExistente != null)
        {
            pedidoExistente.Cantidad += nuevoPedido.Cantidad;
            return await _connection.UpdateAsync(pedidoExistente);
        }
        else
        {
            return await _connection.InsertAsync(nuevoPedido);
        }
    }

    // Actualiza un pedido existente
    public async Task<int> ActualizarPedido(Pedido pedido)
    {
        return await _connection.UpdateAsync(pedido);
    }

    // Obtiene los pedidos de una mesa específica
    public async Task<int> BorrarPedidosMesaAsync(int mesa)
    {
        return await _connection.Table<Pedido>()
            .Where(u => u.MesaId == mesa)
            .DeleteAsync();
    }

    // Obtiene los pedidos de un elementoMenu específic
    public async Task<int> BorrarPedidosElementoMenuAsync(int elementoMenu)
    {
        return await _connection.Table<Pedido>()
            .Where(u => u.ElementoMenu == elementoMenu)
            .DeleteAsync();
    }

    // Elimina un pedido por mesa 
    public async Task<List<Pedido>> ObtenerPedidosMesaAsync(int mesa)
    {
        return await _connection.Table<Pedido>()
            .Where(u => u.MesaId == mesa)
            .ToListAsync();
    }


    // Obtiene todos los pedidos
    public async Task<List<Pedido>> ObtenerPedidosAsync()
    {
        return await _connection.Table<Pedido>().ToListAsync();
    }

    // Elimina un pedido por mesa y elemento
    public async Task<int> borrarPedido(Pedido pedido)
    {
        int mesaId = pedido.MesaId;
        int elementoId = pedido.ElementoMenu;

        return await _connection.Table<Pedido>()
            .Where(u => u.MesaId == mesaId && u.ElementoMenu == elementoId)
            .DeleteAsync();
    }
}
