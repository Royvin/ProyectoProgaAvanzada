using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    }
}