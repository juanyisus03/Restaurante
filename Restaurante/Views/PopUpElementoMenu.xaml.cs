using CommunityToolkit.Maui.Views;
namespace Restaurante.Views;
using Restaurante.Models;
using System.ComponentModel;

// Popup para editar o eliminar un elemento del menú
public partial class PopUpElementoMenu : Popup, INotifyPropertyChanged
{
    private ElementoMenu _elementoMenu;

    // Elemento que se edita dentro del popup
    public ElementoMenu Elemento
    {
        get => _elementoMenu;
        set
        {
            _elementoMenu = value;
            OnPropertyChanged(nameof(Elemento));
        }
    }

    // Lista de tipos disponibles para el elemento
    public List<ElementoMenu.TipoElementoMenu> Tipos { get; set; } =
        Enum.GetValues(typeof(ElementoMenu.TipoElementoMenu))
            .Cast<ElementoMenu.TipoElementoMenu>()
            .ToList();

    public Action<ElementoMenu> OnEditar;
    public Action<ElementoMenu> OnEliminar;

    public PopUpElementoMenu(ElementoMenu elemento)
    {
        InitializeComponent();
        BindingContext = this;

        // Se clona el elemento recibido para edición
        Elemento = new ElementoMenu
        {
            Id = elemento.Id,
            Nombre = elemento.Nombre,
            Precio = elemento.Precio,
            Tipo = elemento.Tipo
        };
    }

    // Acción al guardar cambios
    private void OnGuardarClicked(object sender, EventArgs e)
    {
        OnEditar?.Invoke(Elemento);
        Close();
    }

    // Acción al eliminar el elemento
    private void OnEliminarClicked(object sender, EventArgs e)
    {
        OnEliminar?.Invoke(Elemento);
        Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Notifica cambios en propiedades
    private void OnPropertyChanged(string prop) { 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
