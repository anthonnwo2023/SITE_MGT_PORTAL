﻿global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.OData.Routing.Controllers;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Project.V1.Data;
global using Project.V1.DLL.Services.Interfaces;
global using Project.V1.Lib.Helpers;
global using Project.V1.Lib.Services;
global using Project.V1.Models;
global using Project.V1.Models.SiteHalt;
global using Serilog;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Security.Claims;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.WebUtilities;
global using System.Text;
global using System.Text.Json;
global using Project.V1.Lib.Interfaces;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Project.V1.DLL.Interface;
global using Project.V1.DLL.Services.Interfaces.FormSetup;
global using Project.V1.DLL.Services.Interfaces.SiteHalt;