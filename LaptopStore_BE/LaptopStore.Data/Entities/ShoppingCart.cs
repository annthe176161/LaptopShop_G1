namespace LaptopStore.Data.Entities
{
    public class ShoppingCart
    {
        public int ShoppingCartID { get; set; }
        public string UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}
