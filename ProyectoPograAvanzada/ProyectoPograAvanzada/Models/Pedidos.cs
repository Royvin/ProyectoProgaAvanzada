using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPograAvanzada.Models
{
    public class Pedidos
    {
        [Key]
        public int IdPedido { get; set; }
        [Required]
        public int idCarrito { get; set; }
        [Required]
        public DateTime FechaCompra { get; set; }
        [Required]
        public string Estado { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; }

    }
    public class PedidoItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total => Precio * Cantidad;

        // Propiedades para la relación con Pedido
        public int IdPedido { get; set; }
        public virtual Pedidos Pedidos { get; set; }
    }
}