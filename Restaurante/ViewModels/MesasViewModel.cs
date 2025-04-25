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
    public class MesasViewModel : INotifyPropertyChanged
    {
        private OpcionMesa? _opcionSeleccionada;
        private Dictionary<(int fila, int columna), Mesa> _mapaMesas;

        public ObservableCollection<OpcionMesa> OpcionesMesa { get; set; }
        public ICommand MesaTappedCommand { get; }


        private Color _colorFondo = Colors.White;
        public Color ColorFondo
        {
            get => _colorFondo;
            set
            {
                _colorFondo = value;
                OnPropertyChanged("ColorFondo");
            }
        }

        private bool _isEditionMode;
        public bool isEditionMode
        {
            get => _isEditionMode;
            set
            {
                _isEditionMode = value;
                ColorFondo = value ? Colors.LightSalmon : Colors.White; 
                selectionMode = _isEditionMode ? SelectionMode.Single : SelectionMode.None;
                OnPropertyChanged("isEditionMode");
            }
        }

        private SelectionMode _selectionMode;
        public SelectionMode selectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                OnPropertyChanged("selectionMode");
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
                OnPropertyChanged("OpcionSeleccionada");
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
                if (_mapaMesas.TryGetValue((args.fila, args.columna), out var mesa))
                {
                    string mensaje = $"Número: {mesa.Numero}\n" +
                                     $"Fila: {mesa.Fila}\n" +
                                     $"Columna: {mesa.Columna}\n" +
                                     $"Imagen: {mesa.Imagen}\n" +
                                     $"Estado: {mesa.Estado}";

                    await App.Current.MainPage.DisplayAlert("Información de Mesa", mensaje, "Cerrar");
                }
            }

        }

        public async Task CargarMesasDesdeBaseDeDatos(Grid grid)
        {
            var service = MesasService.GetInstance();
            var mesas = await service.ObtenerMesasAsync();

            foreach (var mesa in mesas)
            {
                _mapaMesas[(mesa.Fila, mesa.Columna)] = mesa;

                
                foreach (var child in grid.Children)
                {
                    if (grid.GetRow(child) == mesa.Fila && grid.GetColumn(child) == mesa.Columna && child is Frame frame)
                    {
                        if (frame.Content is Image img)
                        {
                            img.Source = mesa.Imagen;
                        }
                    }
                }
            }
        }


        private async void cambiarUI((int fila, int columna, Image img) args)
        {
            var (fila, columna, imagenControl) = args;
            var service = MesasService.GetInstance();

            if (OpcionSeleccionada?.Nombre == "Borrar")
            {
                if (_mapaMesas.ContainsKey((fila, columna)))
                {
                    var mesa = _mapaMesas[(fila, columna)];
                    _mapaMesas.Remove((fila, columna));
                    imagenControl.Source = null;

                    
                    await service.borrarMesa(mesa);
                }
            }
            else
            {
                var mesa = new Mesa(numero: fila * 5 + columna, fila: fila, columna: columna, imagen: OpcionSeleccionada.Imagen);
                _mapaMesas[(fila, columna)] = mesa;
                imagenControl.Source = mesa.Imagen;

                await service.GuardarMesa(mesa); 
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}