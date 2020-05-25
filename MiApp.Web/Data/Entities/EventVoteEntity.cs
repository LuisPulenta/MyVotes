using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MiApp.Web.Data.Entities
{
    public class EventVoteEntity
    {
        public int Id { get; set; }

        [Display(Name = "Evento")]
        [MaxLength(50, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(250, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Description { get; set; }

        [Display(Name = "Logo")]
        public string LogoPath { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Fecha Fin")]
        public DateTime EndDate { get; set; }

        public string LogoFullPath => string.IsNullOrEmpty(LogoPath)
           ? "noimage"//null
           : $"http://keypress.serveftp.net:88/MiAppApi{LogoPath.Substring(1)}";

        public ICollection<EventVoteOptionEntity> EventVoteOptions { get; set; }

        public ICollection<AppUserEntity> AppUsers { get; set; }
    }
}