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

        public async Task<List<ElementoMenu>> ObtenerElementoMenuAsync(int id)
        {
            return await _connection.Table<ElementoMenu>().Where(u => u.Id == id).ToListAsync();
        }

        public async Task<List<ElementoMenu>> ObtenerElementoMenusAsync()
        {
            return await _connection.Table<ElementoMenu>().ToListAsync();
        }

        public async Task<int> borrarElementoMenu(ElementoMenu em)
        {

            return await _connection.Table<ElementoMenu>().Where(u => u.Id == em.Id).DeleteAsync();
        }


    }
}
