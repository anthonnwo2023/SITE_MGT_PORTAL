﻿using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_PROJECTTYPES")]
    [Index(new string[] { nameof(Name) }, IsUnique = true)]
    public class ProjectTypeModel : ObjectBase
    {
    }
}
