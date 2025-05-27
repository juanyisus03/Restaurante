using CommunityToolkit.Maui.Views;
using Restaurante.Models;
using Restaurante.Services;
using Restaurante.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Restaurante.ViewModels;

// ViewModel para la gestión de pedidos de una mesa
public class PedidosViewModel : INotifyPropertyChanged
{
    private readonly PedidosService _pedidoService;
    private readonly ElementoMenuService _elementoMenuService;

    public ObservableCollection<Pedido> Pedidos { get; set; } = new();
    public Mesa Mesa { get; }


    private string _totalTexto = "Pagar 0 €";
    public string TotalTexto
    {
        get => _totalTexto;
        set
        {
            _totalTexto = value;
            OnPropertyChanged(nameof(TotalTexto));
        }
    }

    private bool _puedePagar;
    public bool PuedePagar
    {
        get => _puedePagar;
        set
        {
            _puedePagar = value;
            OnPropertyChanged(nameof(PuedePagar));
        }
    }

    // Comandos para la interfaz
    public ICommand IncrementarCantidadCommand { get; }
    public ICommand DecrementarCantidadCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand BorrarTodosCommand { get; }
    public ICommand AgregarCommand { get; }
    public ICommand PagarCommand { get; }

    public PedidosViewModel(Mesa mesa)
    {
        Mesa = mesa;
        _pedidoService = PedidosService.GetInstance();
        _elementoMenuService = ElementoMenuService.GetInstance();
        InicializarDatos();

        // Inicialización de comandos
        AgregarCommand = new Command(async () => await AgregarPedido());
        EliminarCommand = new Command<Pedido>(async (pedido) => await EliminarPedido(pedido));
        DecrementarCantidadCommand = new Command<Pedido>(async (pedido) => await CambiarCantidad(pedido, -1));
        IncrementarCantidadCommand = new Command<Pedido>(async (pedido) => await CambiarCantidad(pedido, 1));
        BorrarTodosCommand = new Command(async () => await BorrarTodosPedidoMesa());
        PagarCommand = new Command(async () => await GenerarTicketPDF());
    }

    private async Task BorrarTodosPedidoMesa()
    {
        bool borrar = await Application.Current.MainPage.DisplayAlert("¿Seguro desea Borrar?", "¿Se ha asegurado de pagar antes el Pedido?", "Si", "No");
        if (borrar == true)
        {
            await _pedidoService.BorrarPedidosMesaAsync(Mesa.Numero);
            await CargarDatos();
        }
    }

    // Genera y guarda un ticket en PDF
    private async Task GenerarTicketPDF()
    {
        try
        {
            var total = Pedidos.Sum(p => p.Cantidad * (p.ElementoMenuPedido?.Precio ?? 0));
            string nombreArchivo = $"Ticket_Mesa_{Mesa.Numero}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            string ruta = Path.Combine(
                DeviceInfo.Platform == DevicePlatform.Android
                    ? FileSystem.AppDataDirectory
                    : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                nombreArchivo);

            using (var pdfWriter = new PdfWriter(ruta))
            using (var pdfDocument = new PdfDocument(pdfWriter))
            using (var document = new Document(pdfDocument))
            {
                document.Add(new Paragraph($"Ticket - Mesa {Mesa.Numero}").SetFontSize(18).SimulateBold());
                document.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                document.Add(new Paragraph("--------------------------------------"));

                foreach (var pedido in Pedidos)
                {
                    var nombre = pedido.ElementoMenuPedido?.Nombre ?? "Desconocido";
                    var precio = pedido.ElementoMenuPedido?.Precio ?? 0;
                    document.Add(new Paragraph($"{pedido.Cantidad} x {nombre} - {(precio * pedido.Cantidad):0.00} €"));
                }

                document.Add(new Paragraph("--------------------------------------"));
                document.Add(new Paragraph($"Total: {total:0.00} €").SimulateBold());
            }

            await Application.Current.MainPage.DisplayAlert("PDF generado", $"Se ha guardado el ticket en:\n{ruta}", "OK");

            // Abre el archivo generado
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(ruta)
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // Cambia la cantidad de un pedido
    private async Task CambiarCantidad(Pedido pedido, int num)
    {
        if (pedido == null) return;

        pedido.Cantidad += num;

        if (pedido.Cantidad <= 0)
        {
            await _pedidoService.borrarPedido(pedido);
        }
        else
        {
            await _pedidoService.ActualizarPedido(pedido);
        }

        await CargarDatos();
    }

    // Actualiza el total y el estado de "PuedePagar"
    private void ActualizarTotal()
    {
        var total = Pedidos.Sum(p => p.Cantidad * (p.ElementoMenuPedido?.Precio ?? 0));
        TotalTexto = $"Pagar {total:0.00} €";
        PuedePagar = Pedidos.Any();
    }

    // Inicializa los datos al construir el ViewModel
    private async void InicializarDatos()
    {
        await _pedidoService.CargarDatos();
        await CargarDatos();
    }

    private bool _isLoading = false;

    // Carga los pedidos de la mesa y sus datos relacionados
    public async Task CargarDatos()
    {
        if (_isLoading) return;

        _isLoading = true;

        try
        {
            Pedidos.Clear();
            var pedidosMesa = await _pedidoService.ObtenerPedidosMesaAsync(Mesa.Numero);

            foreach (var pedido in pedidosMesa)
            {
                var elementoMenu = await _elementoMenuService.ObtenerElementoMenuAsync(pedido.ElementoMenu);
                pedido.ElementoMenuPedido = elementoMenu;
                Pedidos.Add(pedido);
            }

            ActualizarTotal();
        }
        finally
        {
            _isLoading = false;
        }
    }

    // Muestra el popup para agregar pedido
    private async Task AgregarPedido()
    {
        var popup = new PopUpPedido(Mesa, this);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
    }

    // Elimina un pedido
    private async Task EliminarPedido(Pedido pedido)
    {
        await _pedidoService.borrarPedido(pedido);
        await CargarDatos();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Método público para refrescar los pedidos
    public async Task RecargarPedidos()
    {
        await CargarDatos();
    }
}
