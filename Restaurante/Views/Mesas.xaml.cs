using Restaurante.Models;
using Restaurante.ViewModels;

namespace Restaurante.Views;

// Clase parcial para la vista de Mesas
public partial class Mesas : ContentPage
{
    public Mesas()
    {
        InitializeComponent();
        MostrarDialogoAyuda(); // Método que genera un DisplayAlert para ayuda
        GenerarCeldasGrid();   // Método que crea la estructura visual del grid
        CargarDesdeBD();       // Método que carga las mesas almacenadas
    }

    // Genera las celdas del grid dinámicamente
    private void GenerarCeldasGrid()
    {
        for (int fila = 0; fila < 5; fila++)
        {
            for (int columna = 0; columna < 5; columna++)
            {
                // Crear un Frame como contenedor
                var marco = new Frame
                {
                    BackgroundColor = Colors.Transparent,
                    BorderColor = Colors.LightGray,
                    Padding = 0,
                    CornerRadius = 0
                };

                // Crear imagen que estará dentro del Frame
                var img = new Image
                {
                    Aspect = Aspect.AspectFill
                };

                marco.Content = img;

                // Configurar un evento de tap para cada celda
                var tapGesture = new TapGestureRecognizer();
                int f = fila, c = columna;

                tapGesture.Tapped += (s, e) =>
                {
                    var viewModel = (MesasViewModel)BindingContext;
                    viewModel.MesaTappedCommand.Execute((f, c, img));
                };

                marco.GestureRecognizers.Add(tapGesture);

                // Agregar el frame al grid
                gridMesa.Add(marco, columna, fila);
            }
        }
    }

    // Método que carga las mesas desde la base de datos
    private async void CargarDesdeBD()
    {
        if (BindingContext is MesasViewModel vm)
        {
            await vm.CargarMesasDesdeBaseDeDatos(gridMesa);
        }
    }

    private async void MostrarDialogoAyuda()
    {
        bool yaMostrado = Preferences.Get("Mesas_AyudaMostrada", false);
        if (yaMostrado) return;

        bool noMostrarMas = await Application.Current.MainPage.DisplayAlert(
            "Bienvenido",
            "Aquí puedes gestionar todas las mesas del restaurante.\n \u25CF Haz click en el boton para activar modo edición y gestionar el layout de mesas. \n\n \u25CF Toca una celda con mesa sin el modo edicion para empezar a asignarle pedidos a la mesa.",
            "No volver a mostrar",
            "OK"
        );

        if (noMostrarMas)
        {
            Preferences.Set("Mesas_AyudaMostrada", true);
        }
    }


}
