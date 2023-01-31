using System.ComponentModel;

namespace uph.dotnet.pz.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class Customer
    {
        [DisplayName("Lp.")]
        public int Customer_Id { get; set; }
        [DisplayName("Imię")]
        public string Firstname { get; set; }
        [DisplayName("Nazwisko")]
        public string Lastname { get; set; }
        [DisplayName("E-mail")]
        public string Email { get; set; }
    }
}
