namespace Project.V1.Models
{
    public class ADUserDomainModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public string Region { get; set; }
        public string ErrorMsg { get; set; }
        public virtual VendorModel Vendor { get; set; }
    }
}
