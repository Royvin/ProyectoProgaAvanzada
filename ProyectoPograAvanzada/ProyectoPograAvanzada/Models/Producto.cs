using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPograAvanzada.Models
{
	public class Producto
	{
        [Key]
        public int IdProducto { get; set; }
        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public int CantidadaDisponible { get; set; }


	}
}