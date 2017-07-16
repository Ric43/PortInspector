using MOCSoftware.Utilities.PortInspector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MOCSoftware.Utilities.PortInspector.Persistence
{
    internal interface IHostDataPersistor
    {
        void LoadHostData();
        void SaveHostData(List<string> hostInfo);
    }
}
