using System;
using System.ComponentModel.DataAnnotations;

namespace MiApp.Web.Data.Entities
{
    public class VersionEntity
    {
        [Key]
        public int Id { get; set; }
        public string NroVersion { get; set; }
        public DateTime Fecha { get; set; }
    }
}