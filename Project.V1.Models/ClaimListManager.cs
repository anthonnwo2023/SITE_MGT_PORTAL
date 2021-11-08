using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Models
{
    public class ClaimListManager
    {
        public string Category { get; set; }

        public List<ClaimViewModel> Claims { get; set; }
    }
}
