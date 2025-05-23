using Restaurante.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    class ElementoMenuService
    {
        private readonly SQLiteAsyncConnection _connection;
        private static ElementoMenuService? instance;

        private ElementoMenuService(string dbpath)
        {
            _connection = new SQLiteAsyncConnection(dbpath);
            _connection.CreateTableAsync<ElementoMenu>().Wait();
        }

        public static ElementoMenuService GetInstance()
        {
            string dbpath = Path.Combine(FileSystem.AppDataDirectory, "restaurante.db");
            return instance ??= new ElementoMenuService(dbpath);
        }

        public async Task<bool> BaseDeDatosTieneRegistros()
        {
            var count = await _connection.Table<ElementoMenu>().CountAsync();

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

                var elementosIniciales = new List<ElementoMenu>
                {
                    new ElementoMenu { Nombre = "Hamburguesa", Precio = 120.50f, Tipo = ElementoMenu.TipoElementoMenu.Plato },
                    new ElementoMenu { Nombre = "Pizza Margarita", Precio = 150.00f, Tipo = ElementoMenu.TipoElementoMenu.Plato },
                    new ElementoMenu { Nombre = "Ensalada César", Precio = 95.75f, Tipo = ElementoMenu.TipoElementoMenu.Plato },
                    new ElementoMenu { Nombre = "Coca Cola", Precio = 35.00f, Tipo = ElementoMenu.TipoElementoMenu.Bebida },
                    new ElementoMenu { Nombre = "Jugo de Naranja", Precio = 40.00f, Tipo = ElementoMenu.TipoElementoMenu.Bebida },
                    new ElementoMenu { Nombre = "Agua Mineral", Precio = 25.00f, Tipo = ElementoMenu.TipoElementoMenu.Bebida },
                    new ElementoMenu { Nombre = "Pastel de Chocolate", Precio = 80.00f, Tipo = ElementoMenu.TipoElementoMenu.Postre },
                    new ElementoMenu { Nombre = "Helado de Vainilla", Precio = 60.00f, Tipo = ElementoMenu.TipoElementoMenu.Postre }
                };

                await _connection.InsertAllAsync(elementosIniciales);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al cargar datos: {ex.Message}", "OK");
            }
        }

        public async Task<int> CrearElementoMenu(ElementoMenu elementoMenu)
        {
            return await _connection.InsertAsync(elementoMenu);
        }

        public async Task<ElementoMenu> ObtenerElementoMenuAsync(int id)
        {
            return await _connection.Table<ElementoMenu>().Where(u => u.Id == id).FirstAsync();
        }

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

        public async Task<List<ElementoMenu>> ObtenerElementoMenusAsync()
        {
            return await _connection.Table<ElementoMenu>().ToListAsync();
        }

        public async Task<int> borrarElementoMenu(ElementoMenu em)
        {
            return await _connection.Table<ElementoMenu>().Where(u => u.Id == em.Id).DeleteAsync();
        }

        public async Task<int> ActualizarElementoMenu(ElementoMenu editado)
        {
            return await _connection.UpdateAsync(editado);
        }
    }
}
