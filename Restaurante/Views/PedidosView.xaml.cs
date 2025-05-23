using Restaurante.Models;
using Restaurante.ViewModels;

namespace Restaurante.Views;

public partial class PedidosView : ContentPage
{

	
	public PedidosView(Mesa mesa)
	{
		
		InitializeComponent();
		BindingContext = new PedidosViewModel(mesa);
	}
}