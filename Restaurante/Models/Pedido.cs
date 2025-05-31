using SQLite;

namespace Restaurante.Models
{
    public class Pedido
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int MesaId { get; set; }
        public int ElementoMenu { get; set; }

        public int Cantidad { get; set; }

        [Ignore]
        public ElementoMenu ElementoMenuPedido { get; set; }

    }
}
