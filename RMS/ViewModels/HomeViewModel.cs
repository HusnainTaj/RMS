using RMS.Models;

namespace RMS.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        public Category? Category { get; set; }
    }
}
