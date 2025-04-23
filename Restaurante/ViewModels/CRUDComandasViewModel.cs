using Restaurante.Models;
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
        public ObservableCollection<ElementoMenu.TipoElementoMenu> Tipos { get; set; } = new() { 
            ElementoMenu.TipoElementoMenu.Plato,
            ElementoMenu.TipoElementoMenu.Postre,
            ElementoMenu.TipoElementoMenu.Bebida,
        };
        public ObservableCollection<ElementoMenu> Elementos { get; set; } = new();
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
            AgregarCommand = new Command(AgregarElemento);
            EliminarCommand = new Command(EliminarElemento);
            
        }

        private void AgregarElemento()
        {
            var nuevo = new ElementoMenu
            {
                Nombre = NuevoNombre,
                Tipo = NuevoTipo,
                Precio = NuevoPrecio
            };

            Elementos.Add(nuevo);
            LimpiarCampos();
        }

        private void EliminarElemento()
        {
            if (ElementoSeleccionado != null)
                Elementos.Remove(ElementoSeleccionado);
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
