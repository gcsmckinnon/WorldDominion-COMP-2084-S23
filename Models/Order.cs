namespace WorldDominion.Models
{
    public enum PaymentMethods
    {
        VISA,
        Mastercard,
        InteracDebit,
        PayPal,
        Stripe
    }

    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int CartId { get; set; }

        public decimal Total { get; set; }

        public string ShippingAddress { get; set; }

        public bool PaymentReceived { get; set; }

        public PaymentMethods PaymentMethod { get; set; }

        public User? User { get; set; }

        public Cart? Cart { get; set; }
    }
}
