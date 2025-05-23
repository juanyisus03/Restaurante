using Restaurante.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    class PedidosService
    {
        private readonly SQLiteAsyncConnection _connection;
        private static PedidosService? instance;

        private PedidosService(string dbpath)
        {
            _connection = new SQLiteAsyncConnection(dbpath);
            _connection.CreateTableAsync<Pedido>().Wait();
        }

        public static PedidosService GetInstance()
        {
            string dbpath = Path.Combine(FileSystem.AppDataDirectory, "restaurante.db");
            return instance ??= new PedidosService(dbpath);
        }

        public async Task<bool> BaseDeDatosTieneRegistros()
        {
            var count = await _connection.Table<Pedido>().CountAsync();
            return count > 0;
        }

        public async Task CargarDatos()
        {
            try
            {
                if (await BaseDeDatosTieneRegistros())
                {
                    return;
                }

                List<Mesa> mesas = await _connection.Table<Mesa>().ToListAsync();
                List<ElementoMenu> elementos = await _connection.Table<ElementoMenu>().ToListAsync();
                var pedidosIniciales = new List<Pedido>();

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
                    pedidosIniciales.Add(pedido);
                }

                await _connection.InsertAllAsync(pedidosIniciales);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al cargar datos: {ex.Message}", "OK");
            }
        }

        public async Task<int> CrearPedido(Pedido nuevoPedido)
        {
            // Buscar si ya existe un pedido para esa mesa y ese elemento
            var pedidoExistente = await _connection.Table<Pedido>()
                .Where(p => p.MesaId == nuevoPedido.MesaId && p.ElementoMenu == nuevoPedido.ElementoMenu)
                .FirstOrDefaultAsync();

            if (pedidoExistente != null)
            {
                // Ya existe → sumamos cantidad y actualizamos
                pedidoExistente.Cantidad += nuevoPedido.Cantidad;
                return await _connection.UpdateAsync(pedidoExistente);
            }
            else
            {
                // No existe → lo insertamos como nuevo
                return await _connection.InsertAsync(nuevoPedido);
            }
        }


        public async Task<int> ActualizarPedido(Pedido pedido)
        {
            return await _connection.UpdateAsync(pedido);
        }

        public async Task<List<Pedido>> ObtenerPedidosMesaAsync(int mesa)
        {
            return await _connection.Table<Pedido>().Where(u => u.MesaId == mesa).ToListAsync();
        }

        public async Task<List<Pedido>> ObtenerPedidosAsync()
        {
            return await _connection.Table<Pedido>().ToListAsync();
        }

        public async Task<int> borrarPedido(Pedido pedido)
        {
            int mesaId = pedido.MesaId;
            int elementoId = pedido.ElementoMenu;
            return await _connection.Table<Pedido>()
                .Where(u => u.MesaId == mesaId && u.ElementoMenu == elementoId)
                .DeleteAsync();
        }
    }
}
