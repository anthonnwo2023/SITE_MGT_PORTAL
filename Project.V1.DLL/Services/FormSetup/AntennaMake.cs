﻿using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class AntennaMake : GenericRepo<AntennaMakeModel>, IAntennaMake, IDisposable
{
    public AntennaMake(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}
