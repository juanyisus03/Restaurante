using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Models
{
    public class Mesa
    {

        public int Numero { get; set; }
        public string Imagen{ get; set; }
        public EstadoMesa Estado { get; set; }

        public Mesa(int numero, string imagen, EstadoMesa estado = EstadoMesa.Libre)
        {
            Numero = numero;
            Imagen = imagen;
            Estado = estado;
        }
    }

    public enum EstadoMesa
    {
        Libre,
        Ocupada,
        Reservada
    }


}
