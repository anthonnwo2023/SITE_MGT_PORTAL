using Project.V1.Models;
using System.Collections.Generic;

namespace Project.V1.Web.Pages.Live
{
    public partial class Dashboard
    {
        public List<PathInfo> Paths { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Live Sites", Link = "live/dashboard" },
            };
        }
    }
}
