using CommunityToolkit.Maui.Views;
using Restaurante.Models;
using Restaurante.Services;
using Restaurante.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Restaurante.Views;

public partial class PopUpPedido : Popup, INotifyPropertyChanged
{
    private readonly PedidosService _pedidoService;
    private readonly ElementoMenuService _elementoMenuService;
    private readonly Mesa _mesa;
    private readonly PedidosViewModel _viewModel;

    public ObservableCollection<ElementoMenu> ElementosMenu { get; set; } = new();

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

    private string _cantidad;
    public string Cantidad
    {
        get => _cantidad;
        set
        {
            _cantidad = value;
            OnPropertyChanged(nameof(Cantidad));
        }
    }

    public PopUpPedido(Mesa mesa, PedidosViewModel viewModel)
    {
        InitializeComponent();
        _pedidoService = PedidosService.GetInstance();
        _elementoMenuService = ElementoMenuService.GetInstance();
        BindingContext = this;

        _mesa = mesa;
        _viewModel = viewModel;

        _ = CargarElementos();
    }

    private async Task CargarElementos()
    {
        var lista = await _elementoMenuService.ObtenerElementoMenusAsync();
        ElementosMenu.Clear();
        foreach (var item in lista)
            ElementosMenu.Add(item);
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        if (ElementoSeleccionado == null || string.IsNullOrWhiteSpace(Cantidad) || !int.TryParse(Cantidad, out int cantidadInt) || cantidadInt <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "Seleccione un elemento y cantidad válida", "OK");
            return;
        }

        var pedido = new Pedido
        {
            MesaId = _mesa.Numero,
            ElementoMenu = ElementoSeleccionado.Id,
            Cantidad = cantidadInt
        };

        await _pedidoService.CrearPedido(pedido);

        await _viewModel.RecargarPedidos();
        Close();
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
