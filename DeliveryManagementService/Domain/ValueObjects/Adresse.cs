namespace DeliveryManagementService.Domain.ValueObjects
{
    public class Adresse
    {
        public string Street { get; private set; }
        public string PostalCode { get; private set; }
        public string City { get; private set; }

        public Adresse(string street, string postalCode, string city)
        {
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street cannot be empty");
            if (string.IsNullOrWhiteSpace(postalCode)) throw new ArgumentException("Postal code cannot be empty");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty");

            Street = street;
            PostalCode = postalCode;
            City = city;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Adresse other) return false;

            return Street == other.Street &&
                   PostalCode == other.PostalCode &&
                   City == other.City;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Street, PostalCode, City);
        }
    }
}
