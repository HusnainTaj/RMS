﻿namespace RMS.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImgURL { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
