using Microsoft.AspNetCore.Http;
using MiApp.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace MiApp.Web.Models
{
    public class ItemViewModel : ItemEntity
    {
        [Display(Name = "Logo")]
        public IFormFile LogoFile { get; set; }
    }
}