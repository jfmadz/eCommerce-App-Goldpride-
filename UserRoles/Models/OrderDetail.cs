namespace UserRoles.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderID { get; set; }
        //public string Email { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Deposit { get; set; }
        public string Distance { get; set; }
        public int QuantReturned { get; set; }
        public virtual ItemsHire ItemsHire { get; set; }
        public virtual Order Order { get; set; }
       // public virtual InvoiceVM InvoiceVM { get; set; }
    }
}