using System;

namespace MiApp.Common.Models
{
    public class ItemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public byte[] PictureArray { get; set; }
        public bool Active { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}