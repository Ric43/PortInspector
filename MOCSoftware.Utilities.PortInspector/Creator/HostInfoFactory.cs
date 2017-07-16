using MOCSoftware.Utilities.PortInspector.Model;

namespace MOCSoftware.Utilities.PortInspector.Creator
{
    internal sealed class HostInfoFactory
    {
        internal HostInfo CreateHostInfo(string hostName)
        {
            if (!string.IsNullOrWhiteSpace(hostName))
            {
                var hostInfo = new HostInfo {HostEntry = hostName};
                return hostInfo;
            }

            return null;
        }
    }
}
