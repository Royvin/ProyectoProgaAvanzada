using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPograAvanzada.Models
{
    public class Carrito
    {
        [Key]
        public int Id { get; set; }
        //public string UsuarioId { get; set; } // ID del usuario (puede depender del sistema de autenticación)

        // Propiedad de navegación
        public virtual List<CarritoItem> Items { get; set; } = new List<CarritoItem>();
    }

    public class CarritoItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total => Precio * Cantidad;

        // Propiedades para la relación con Carrito
        public int CarritoId { get; set; }
        public virtual Carrito Carrito { get; set; }
    }
}