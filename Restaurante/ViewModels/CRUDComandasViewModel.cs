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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Restaurante.ViewModels
{
   public class CRUDComandasViewModel : INotifyPropertyChanged
    {
        private readonly ElementoMenuService _elementoMenuService;
        public ObservableCollection<ElementoMenu.TipoElementoMenu> Tipos { get; set; } = new() { 
            ElementoMenu.TipoElementoMenu.Plato,
            ElementoMenu.TipoElementoMenu.Postre,
            ElementoMenu.TipoElementoMenu.Bebida,
        };
        public ObservableCollection<ElementoMenu> Elementos { get; set; }
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

        public ICommand AgregarCommand { get; }
        public ICommand EliminarCommand { get; }

        public CRUDComandasViewModel()
        {

            _elementoMenuService = ElementoMenuService.GetInstance();


            AgregarCommand = new Command(async() => await AgregarElemento());
            EliminarCommand = new Command(async() => await EliminarElemento());
            Elementos = new ObservableCollection<ElementoMenu>();
            InicializarDatos();
            
        }

        private async void InicializarDatos()
        {
            await CargarDatos();
        }

        private async Task CargarDatos()
        {

            Elementos.Clear();
            var elementosMenus = await _elementoMenuService.ObtenerElementoMenusAsync();

            foreach (var elementosMenu in elementosMenus)
            {
                Elementos.Add(elementosMenu);
            }
        }

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

        private async Task EliminarElemento()
        {
            if (ElementoSeleccionado != null)
            {
                bool aceptar = await Shell.Current.DisplayAlert("Aviso",$"¿Seguro que desea borrar el elemento {ElementoSeleccionado.Nombre}?", "Okey", "Cancelar");
                if (aceptar)
                {
                    await _elementoMenuService.borrarElementoMenu(ElementoSeleccionado);
                    await CargarDatos();
                }
            }
                
        }


        private void LimpiarCampos()
        {
            NuevoNombre = string.Empty;
            NuevoTipo = ElementoMenu.TipoElementoMenu.Plato;
            NuevoPrecio = 0;
            OnPropertyChanged(nameof(NuevoNombre));
            OnPropertyChanged(nameof(NuevoTipo));
            OnPropertyChanged(nameof(NuevoPrecio));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
