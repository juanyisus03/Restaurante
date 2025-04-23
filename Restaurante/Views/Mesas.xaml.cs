using Restaurante.Models;
using Restaurante.ViewModels;

namespace Restaurante.Views;

public partial class Mesas : ContentPage
{
    public Mesas()
    {
        InitializeComponent();
        BindingContext = new MesasViewModel();
        GenerarCeldasGrid();
    }

    private void GenerarCeldasGrid()
    {
        for (int fila = 0; fila < 5; fila++)
        {
            for (int columna = 0; columna < 5; columna++)
            {
                var marco = new Frame
                {
                    BackgroundColor = Colors.Transparent,
                    BorderColor = Colors.LightGray,
                    Padding = 0,
                    CornerRadius = 0
                };

                var img = new Image
                {
                    Aspect = Aspect.AspectFill
                };

                marco.Content = img;
                var tapGesture = new TapGestureRecognizer();
                int f = fila, c = columna;

                tapGesture.Tapped += (s, e) =>
                {
                    var viewModel = (MesasViewModel)BindingContext;
                    viewModel.MesaTappedCommand.Execute((f, c, img));
                };
                marco.GestureRecognizers.Add(tapGesture);

                gridMesa.Add(marco, columna, fila);
            }
        }
    }

}

