namespace LaptopStore.Data.Entities
{
    public class Review
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public string UserID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}
