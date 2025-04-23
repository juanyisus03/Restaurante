using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Models
{
    public class ElementoMenu
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }        
        public float Precio { get; set; }
        public TipoElementoMenu Tipo {  get; set; }

        public ElementoMenu() { }
        public ElementoMenu(int id, string nombre, float precio, TipoElementoMenu tipo)
        {
            Id = id;
            Nombre = nombre;
            Precio = precio;
            Tipo = tipo;
        }

        public enum TipoElementoMenu
        {
            Plato,
            Bebida,
            Postre
        }
    }
}
