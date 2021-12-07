﻿using Project.V1.Data;
using Project.V1.Lib.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Services
{
    public class Projects : GenericRepo<ProjectModel>, IProjects, IDisposable
    {
        public Projects(ApplicationDbContext context)
            : base(context, "")
        {
        }
    }
}