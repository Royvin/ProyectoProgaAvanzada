using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPograAvanzada.Models
{
    public class DetallePedidos
    {
        [Key]
        public int IdDetalle { get; set; }
        [Required]
        public int IdPedido { get; set; }
        [Required]
        public int IdItem { get; set; }
    }
}