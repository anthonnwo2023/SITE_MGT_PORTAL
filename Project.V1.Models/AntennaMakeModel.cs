﻿namespace Project.V1.Models;

[Table("TBL_RFACCEPT_ANTENNA_MAKES")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class AntennaMakeModel : ObjectBase
{
}
