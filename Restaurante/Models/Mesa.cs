using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Models
{
    public class Mesa
    {
        [PrimaryKey]
        public int Numero { get; set; }
        public string Imagen{ get; set; }
        public int Fila {  get; set; }
        public int Columna { get; set; }
        public EstadoMesa Estado { get; set; }

        public Mesa() { }
        public Mesa(int numero, string imagen,int fila, int columna, EstadoMesa estado = EstadoMesa.Libre)
        {
            Numero = numero;
            Imagen = imagen;
            Estado = estado;
            Fila = fila;
            Columna = columna;
        }
    }

    public enum EstadoMesa
    {
        Libre,
        Ocupada,
        Reservada
    }


}
