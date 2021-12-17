﻿using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_VENDORS")]
    [Index(new string[] { nameof(Name) }, IsUnique = true)]
    public class VendorModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string MailList { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
