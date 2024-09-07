namespace OjtPortal.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public Address Address { get; set; } = new();

        public Company()
        {
        }

        public Company(string companyName, string contactNo, string contactEmail, Address address)
        {
            CompanyName = companyName;
            ContactNo = contactNo;
            ContactEmail = contactEmail;
            Address = address;
        }
    }
}
