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
    public class MesasViewModel : INotifyPropertyChanged
    {
        private OpcionMesa? _opcionSeleccionada;
        private Dictionary<(int fila, int columna), Mesa> _mapaMesas;

        public ObservableCollection<OpcionMesa> OpcionesMesa { get; set; }
        public ICommand MesaTappedCommand { get; }

        private bool _isEditionMode;

        public bool isEditionMode
        {
            get => _isEditionMode;
            set
            {
                _isEditionMode = value;
                selectionMode = _isEditionMode ? SelectionMode.Single : SelectionMode.None;
                OnPropertyChanged(nameof(isEditionMode));
            }
        }

        private SelectionMode _selectionMode;


        public SelectionMode selectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                OnPropertyChanged(nameof(selectionMode));
            }
        }

        public MesasViewModel()
        {
            _mapaMesas = new Dictionary<(int fila, int columna), Mesa>();
            OpcionesMesa = new ObservableCollection<OpcionMesa>
            {
                new OpcionMesa { Nombre = "Tipo 1", Imagen = "mesalargav.png" },
                new OpcionMesa { Nombre = "Tipo 2", Imagen = "mesalargah.png" },
                new OpcionMesa { Nombre = "Tipo 3", Imagen = "mesa1.png" },
                new OpcionMesa { Nombre = "Borrar", Imagen = "borrar.png" }
            };

            _isEditionMode = false;
            MesaTappedCommand = new Command<(int fila, int columna, Image img)>(ManejarClickCelda);

        }

        public OpcionMesa? OpcionSeleccionada
        {
            get => _opcionSeleccionada;
            set
            {
                _opcionSeleccionada = value;
                OnPropertyChanged(nameof(OpcionSeleccionada));
            }
        }



        private async void ManejarClickCelda((int fila, int columna, Image img) args)
        {


            if (OpcionSeleccionada == null) return;

            if (isEditionMode)
            {
                cambiarUI(args);
            }
            else
            {
                if (_mapaMesas.ContainsKey((args.fila, args.columna)))
                {
                    await App.Current.MainPage.DisplayAlert("Probando", "Esto es una prueba de que funciona mas o menos bien", "Estoy cansado jefe");
                }
            }

        }

        private void cambiarUI((int fila, int columna, Image img) args)
        {
            var (fila, columna, imagenControl) = args;
            if (OpcionSeleccionada.Nombre == "Borrar")
            {
                if (_mapaMesas.ContainsKey((fila, columna)))
                {
                    _mapaMesas.Remove((fila, columna));
                    imagenControl.Source = null;
                }
            }
            else
            {
                var mesa = new Mesa(numero: fila * 5 + columna, fila:fila, columna:columna, imagen: OpcionSeleccionada.Imagen);
                _mapaMesas[(fila, columna)] = mesa;
                imagenControl.Source = mesa.Imagen;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}