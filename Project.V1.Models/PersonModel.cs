using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_PERSON_APPROVER")]
    public class PersonModel
    {
        [Key]
        public string Id { get; set; }

        public string Username { get; set; }

        public string Fullname { get; set; }

        [Phone]
        public string PhoneNo { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }
    }
}
