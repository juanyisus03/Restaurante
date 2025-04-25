using Restaurante.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Services
{
    class MesasService
    {
        private readonly SQLiteAsyncConnection _connection;
        private static MesasService? instance;


        private MesasService(string dbpath)
        {
            _connection = new SQLiteAsyncConnection(dbpath);
            _connection.CreateTableAsync<Mesa>().Wait();

        }


        public static MesasService GetInstance()
        {
            string dbpath = Path.Combine(FileSystem.AppDataDirectory, "restaurante.db");

            return instance ??= new MesasService(dbpath);
        }

        public async Task<bool> BaseDeDatosTieneRegistros()
        {
            var count = await _connection.Table<Mesa>().CountAsync();
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

        public async Task<int> CrearMesa(Mesa mesa)
        {
            return await _connection.InsertAsync(mesa);
        }

        public async Task<int> GuardarMesa(Mesa mesa)
        {
            return await _connection.InsertOrReplaceAsync(mesa);
        }

        public async Task<List<Mesa>> ObtenerMesaAsync(int numero)
        {
            return await _connection.Table<Mesa>().Where(u => u.Numero == numero).ToListAsync();
        }

        public async Task<List<Mesa>> ObtenerMesasAsync()
        {
            return await _connection.Table<Mesa>().ToListAsync();
        }

        public async Task<int> borrarMesa(Mesa em)
        {

            return await _connection.Table<Mesa>().Where(u => u.Numero == em.Numero).DeleteAsync();
        }


    }
}
