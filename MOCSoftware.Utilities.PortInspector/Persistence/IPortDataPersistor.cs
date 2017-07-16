using System.Collections.Generic;
using MOCSoftware.Utilities.PortInspector.Model;

namespace MOCSoftware.Utilities.PortInspector.Persistence
{
    internal interface IPortDataPersistor
    {
        List<PortInfo> PortInfoList { get; set; }
        void LoadPortData();
        void LoadApplicationDataFromDataFile();
        void LoadApplicationDataFromServicesFile();
        void SavePortData();
    }
}
