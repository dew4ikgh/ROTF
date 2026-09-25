namespace ROTF.Server.DTOs
{
    public class AddItemRequest
    {
        public int PlayerProgressId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
    }
}