using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPograAvanzada.Models
{
    public class Reseñas
    {
        [Key]
        public int IdReseña { get; set; }
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }
        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; }

    }
}