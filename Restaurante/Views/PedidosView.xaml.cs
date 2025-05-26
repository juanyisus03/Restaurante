using Restaurante.Models;
using Restaurante.ViewModels;

namespace Restaurante.Views;

public partial class PedidosView : ContentPage
{

	
	public PedidosView(Mesa mesa)
	{
		
		InitializeComponent();
		BindingContext = new PedidosViewModel(mesa);
		Appearing += CargarDatosViewModel;
	}

    private async void CargarDatosViewModel(object? sender, EventArgs e)
    {
		await ((PedidosViewModel) BindingContext).CargarDatos();
    }
}