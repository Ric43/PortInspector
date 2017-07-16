using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MOCSoftware.Utilities.PortInspector.Model;
using System.Net.Sockets;

namespace MOCSoftware.Utilities.PortInspector.Utility
{
    internal class PortFilter
    {
        private ProtocolType _transportFilter;
        private readonly List<int> _portNumbers = new List<int>();
        private readonly List<string> _portNames = new List<string>();

        private const int _IP_PORT_MIN_VALUE = 0;
        private const int _IP_PORT_MAX_VALUE = 65535;
        private const string _PORT_NUMBER_PATTERN = "^([0-9]*)(-([0-9]*))?$";
        private const char _FILTER_STRING_SPLIT_CHAR = ',';

        public string CurrentFilter { get; set; }

        public PortInspectorProtocols CurrentProtocol { get; set; }

        internal List<PortInfo> FilterPorts(string filter, PortInspectorProtocols portInspectorProtocol, List<PortInfo> unfilteredList)
        {
            _transportFilter = MapPortInspectorProtocolToTransportFilter(portInspectorProtocol);
            SetFilter(filter);

            var filteredList = FilterList(unfilteredList);

            if (filteredList.Count > 0)
                return filteredList;

            return null;
        }

        private ProtocolType MapPortInspectorProtocolToTransportFilter(PortInspectorProtocols portInspectorProtocol)
        {
            CurrentProtocol = portInspectorProtocol;

            switch(CurrentProtocol)
            {
                case PortInspectorProtocols.TCP:
                    return ProtocolType.Tcp;
                case PortInspectorProtocols.UDP:
                    return ProtocolType.Udp;
                default:
                    return ProtocolType.Unspecified;
            }
        }

        private List<PortInfo> FilterList(List<PortInfo> unfilteredList)
        {
            var filteredList = unfilteredList;
            if (_transportFilter != ProtocolType.Unspecified)
                filteredList = unfilteredList.Where(e => e.Protocol == _transportFilter).ToList();

            filteredList = FilterListOnPortNumber(filteredList);
            filteredList = FilterListOnPortName(filteredList);

            var result = new List<PortInfo>();
            filteredList.ToList().ForEach(result.Add);
            return result;
        }

        private List<PortInfo> FilterListOnPortNumber(List<PortInfo> filteredList)
        {
            if (_portNumbers.Count == 0)
                return filteredList;

            var result = (from u in filteredList
                         join n in _portNumbers on u.PortNumber equals n
                         select u).ToList();

            return result;
        }

        private List<PortInfo> FilterListOnPortName(List<PortInfo> filteredList)
        {
            if (_portNames.Count == 0)
                return filteredList;

            var result = new List<PortInfo>();

            foreach (var entry in _portNames)
            {
                string portName = entry.ToLower();
                var filtered = filteredList.Where(e => e.PortName.Contains(portName) && !result.Contains(e));
                result.AddRange(filtered);
            }

            return result;
        }

        private void SetFilter(string filter)
        {
            CurrentFilter = filter;
            ClearFilterLists();
            SplitFilterStringAndSaveTokensInLists(filter);
        }

        private void ClearFilterLists()
        {
            _portNames.Clear();
            _portNumbers.Clear();
        }

        private void SplitFilterStringAndSaveTokensInLists(string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
                foreach (var entry in filter.Trim().Split(_FILTER_STRING_SPLIT_CHAR))
                {
                    ExtractFilterCriteriaFromCommaSeparatedEntry(entry);
                }
        }

        private void ExtractFilterCriteriaFromCommaSeparatedEntry(string entry)
        {
            var regEx = new Regex(_PORT_NUMBER_PATTERN);
            var match = regEx.Match(entry);
            if (match.Success)
            {
                ExtractPortNumbersFromEntry(match.Groups[1].Value, match.Groups[3].Value);
            }
            else
            {
                if (!_portNames.Contains(entry))
                    _portNames.Add(entry);
            }
        }

        private void ExtractPortNumbersFromEntry(string firstString, string secondString)
        {
            int first;
            if (int.TryParse(firstString, out first))
            {
                if (!_portNumbers.Contains(first) && IsIpPort(first))
                    _portNumbers.Add(first);

                int second;
                if (int.TryParse(secondString, out second))
                {
                    if (second > first)
                    {
                        var newNumbers =
                            Enumerable.Range(first + 1, second - first).Where(e => !_portNumbers.Contains(e) && IsIpPort(e));
                        _portNumbers.AddRange(newNumbers);
                    }
                }
            }
        }

        private bool IsIpPort(int portNumber)
        {
            if (portNumber < _IP_PORT_MIN_VALUE || portNumber > _IP_PORT_MAX_VALUE)
                return false;

            return true;
        }
    }
}
