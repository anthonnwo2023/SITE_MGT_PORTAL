﻿using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_ANTENNA_TYPES")]
    [Index(new string[] { nameof(Name) }, IsUnique = true)]
    public class AntennaTypeModel : ObjectBase
    {
    }
}
