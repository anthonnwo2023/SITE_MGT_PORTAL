using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Asn1.Ocsp;
using Project.V1.DLL.Interface;
using Project.V1.Lib.Services;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.DLL.Helpers
{
    public class AppState
    {
        public event Action OnChange;

        public AppState()
        {
            LoginObject.InitObjects();
        }

        public void TriggerRequestRecount()
        {
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange.Invoke();
    }
}
