using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurante.Models
{
    class Pedido
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int MesaId { get; set; }
        public int ElementoMenu {  get; set; }
    }
}
