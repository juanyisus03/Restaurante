using CommunityToolkit.Maui.Views;
namespace Restaurante.Views;
using Restaurante.Models;
using System.ComponentModel;

public partial class PopUpElementoMenu : Popup, INotifyPropertyChanged
{


    private ElementoMenu _elementoMenu;
    public ElementoMenu Elemento
    {
        get => _elementoMenu;
        set
        {
            _elementoMenu = value;
            OnPropertyChanged(nameof(Elemento));
        }
    }
    public List<ElementoMenu.TipoElementoMenu> Tipos { get; set; } = Enum.GetValues(typeof(ElementoMenu.TipoElementoMenu)).Cast<ElementoMenu.TipoElementoMenu>().ToList();

    public Action<ElementoMenu> OnEditar;
    public Action<ElementoMenu> OnEliminar;



    public PopUpElementoMenu(ElementoMenu elemento)
    {
        InitializeComponent();
        BindingContext = this;
        Elemento = new ElementoMenu
        {
            Id = elemento.Id,
            Nombre = elemento.Nombre,
            Precio = elemento.Precio,
            Tipo = elemento.Tipo
        };
    }

    private void OnGuardarClicked(object sender, EventArgs e)
    {
      

        OnEditar?.Invoke(Elemento);
        Close();
    }

    private void OnEliminarClicked(object sender, EventArgs e)
    {
        OnEliminar?.Invoke(Elemento);
        Close();
    }
   public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string prop) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
