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
        private static MesasService? instance; // Singleton para acceder a una sola instancia del servicio

        // Constructor privado para inicializar la conexión a la base de datos
        private MesasService(string dbpath)
        {
            _connection = new SQLiteAsyncConnection(dbpath);
            _connection.CreateTableAsync<Mesa>().Wait(); // Crear la tabla Mesa si no existe
        }

        // Método para obtener la única instancia del servicio (Singleton Pattern)
        public static MesasService GetInstance()
        {
            string dbpath = Path.Combine(FileSystem.AppDataDirectory, "restaurante.db");

            return instance ??= new MesasService(dbpath);
        }

        // Verifica si hay registros en la base de datos
        public async Task<bool> BaseDeDatosTieneRegistros()
        {
            var count = await _connection.Table<Mesa>().CountAsync();
            return count > 0;
        }

        // Método para cargar datos iniciales (en caso de ser necesario)
        public async Task CargarDatos()
        {
            try
            {
                if (await BaseDeDatosTieneRegistros())
                {
                    return;
                }
                // Aquí podrías insertar datos iniciales si la tabla está vacía
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al cargar datos: {ex.Message}", "OK");
            }
        }

        // Crear una nueva mesa en la base de datos
        public async Task<int> CrearMesa(Mesa mesa)
        {
            return await _connection.InsertAsync(mesa);
        }

        // Guardar o actualizar una mesa
        public async Task<int> GuardarMesa(Mesa mesa)
        {
            return await _connection.InsertOrReplaceAsync(mesa);
        }

        // Obtener una lista de mesas filtrada por número
        public async Task<List<Mesa>> ObtenerMesaAsync(int numero)
        {
            return await _connection.Table<Mesa>().Where(u => u.Numero == numero).ToListAsync();
        }

        // Obtener todas las mesas
        public async Task<List<Mesa>> ObtenerMesasAsync()
        {
            return await _connection.Table<Mesa>().ToListAsync();
        }

        // Borrar una mesa de la base de datos
        public async Task<int> borrarMesa(Mesa em)
        {
            return await _connection.Table<Mesa>().Where(u => u.Numero == em.Numero).DeleteAsync();
        }
    }
}
