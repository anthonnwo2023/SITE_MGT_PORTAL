﻿namespace Project.V1.Models;

[Table("TBL_RFACCEPT_SUMMERCONFIGS")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class SummerConfigModel : ObjectBase
{
}
