using Restaurante.Models;
using Restaurante.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Restaurante.ViewModels
{
    public class CRUDComandasViewModel : INotifyPropertyChanged
    {
        private readonly ElementoMenuService _elementoMenuService;

        // Lista de tipos de elementos del menú disponibles
        public ObservableCollection<ElementoMenu.TipoElementoMenu> Tipos { get; set; } = new()
        {
            ElementoMenu.TipoElementoMenu.Plato,
            ElementoMenu.TipoElementoMenu.Postre,
            ElementoMenu.TipoElementoMenu.Bebida,
        };

        // Lista de elementos actuales en el menú
        public ObservableCollection<ElementoMenu> Elementos { get; set; }

        // Propiedades para crear un nuevo elemento
        public string NuevoNombre { get; set; }
        public ElementoMenu.TipoElementoMenu NuevoTipo { get; set; }
        public float NuevoPrecio { get; set; }

        private ElementoMenu _elementoSeleccionado;
        public ElementoMenu ElementoSeleccionado
        {
            get => _elementoSeleccionado;
            set
            {
                _elementoSeleccionado = value;
                OnPropertyChanged(nameof(ElementoSeleccionado));
            }
        }

        // Comandos de agregar y eliminar
        public ICommand AgregarCommand { get; }
        public ICommand EliminarCommand { get; }

        // Constructor
        public CRUDComandasViewModel()
        {
            _elementoMenuService = ElementoMenuService.GetInstance();

            AgregarCommand = new Command(async () => await AgregarElemento());
            EliminarCommand = new Command(async () => await EliminarElemento());

            Elementos = new ObservableCollection<ElementoMenu>();
            InicializarDatos();
        }

        // Inicializar datos llamando a CargarDatos
        private async void InicializarDatos()
        {
            await CargarDatos();
        }

        // Cargar elementos del menú desde la base de datos
        private async Task CargarDatos()
        {
            Elementos.Clear();
            var elementosMenus = await _elementoMenuService.ObtenerElementoMenusAsync();

            foreach (var elementosMenu in elementosMenus)
            {
                Elementos.Add(elementosMenu);
            }
        }

        // Agregar un nuevo elemento al menú
        private async Task AgregarElemento()
        {
            if (string.IsNullOrEmpty(NuevoNombre))
            {
                await Shell.Current.DisplayAlert("Error", "Rellene todos los campos", "Okey");
            }
            else
            {
                var nuevo = new ElementoMenu
                {
                    Nombre = NuevoNombre,
                    Tipo = NuevoTipo,
                    Precio = NuevoPrecio
                };

                int comprobado = await _elementoMenuService.CrearElementoMenu(nuevo);
                if (comprobado == 0)
                {
                    await Shell.Current.DisplayAlert("Error", "Error a la hora de insertar los datos", "Okey");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Info", "Los datos se insertaron correctamente", "Okey");
                }
            }

            LimpiarCampos();
            await CargarDatos();
        }

        // Eliminar un elemento seleccionado del menú
        private async Task EliminarElemento()
        {
            if (ElementoSeleccionado != null)
            {
                bool aceptar = await Shell.Current.DisplayAlert("Aviso", $"¿Seguro que desea borrar el elemento {ElementoSeleccionado.Nombre}?", "Okey", "Cancelar");
                if (aceptar)
                {
                    await _elementoMenuService.borrarElementoMenu(ElementoSeleccionado);
                    await CargarDatos();
                }
            }
        }

        // Limpiar los campos del formulario de alta
        private void LimpiarCampos()
        {
            NuevoNombre = string.Empty;
            NuevoTipo = ElementoMenu.TipoElementoMenu.Plato;
            NuevoPrecio = 0;

            OnPropertyChanged(nameof(NuevoNombre));
            OnPropertyChanged(nameof(NuevoTipo));
            OnPropertyChanged(nameof(NuevoPrecio));
        }

        // Evento de cambio de propiedad
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
