using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using MOCSoftware.Utilities.PortInspector.Communication;
using MOCSoftware.Utilities.PortInspector.Model;
using MOCSoftware.Utilities.PortInspector.Properties;

namespace MOCSoftware.Utilities.PortInspector.Persistence
{
    internal sealed class PortDataPersistor : IPortDataPersistor
    {
        private const string _APPLICATION_DATA_FOLDER = "MOC.PortInspector";
        private const string _APPLICATION_DATA_FILE = "PortSettings.bin";

        private readonly string _applicationData =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         _APPLICATION_DATA_FOLDER, _APPLICATION_DATA_FILE);

        public List<PortInfo> PortInfoList { get; set; }

        public void SavePortData()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _APPLICATION_DATA_FOLDER)))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _APPLICATION_DATA_FOLDER));

            var formatter = new BinaryFormatter();
            using (var fileStream = new FileStream(_applicationData, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fileStream, PortInfoList);
            }
        }

        public void LoadPortData()
        {
            if (File.Exists(_applicationData))
            {
                LoadApplicationDataFromDataFile();
            }
            else
            {
                LoadApplicationDataFromServicesFile();
            }
        }

        public void LoadApplicationDataFromDataFile()
        {
            var formatter = new BinaryFormatter();
            try
            {
                using (var fileStream = new FileStream(_applicationData, FileMode.Open, FileAccess.Read, FileShare.Read)
                    )
                {
                    PortInfoList = (List<PortInfo>) formatter.Deserialize(fileStream);
                }
            }
            catch
            {
                LoadApplicationDataFromServicesFile();
            }
        }

        public void LoadApplicationDataFromServicesFile()
        {
            var lines = new StringCollection();

            if (File.Exists(string.Format(@"{0}\system32\drivers\etc\services", Environment.GetEnvironmentVariable("SystemRoot"))))
            {
                try
                {
                    using (
                        TextReader textReader =
                            File.OpenText(string.Format(@"{0}\system32\drivers\etc\services", Environment.GetEnvironmentVariable("SystemRoot"))))
                    {
                        using (var stringReader = new StringReader(textReader.ReadToEnd()))
                        {
                            string lineOfTextFromServicesFile;
                            while ((lineOfTextFromServicesFile = stringReader.ReadLine()) != null)
                                lines.Add(lineOfTextFromServicesFile);
                        }
                    }

                    ProcessServicesFileInMemory(lines);
                }
                catch (Exception ex)
                {
                    App.Mediator.Publish(App._EVENT_ERROR, new ErrorMessage{ ErrorTitle = Resources.Error_DataLoad_Title, ErrorText = ex.Message});
                }
            }
        }

        private void ProcessServicesFileInMemory(StringCollection lines)
        {
            foreach (var line in lines)
            {
                var portInfo = ProcessInputLine(line);
                if (portInfo != null)
                    PortInfoList.Add(portInfo);
            }
        }

        private PortInfo ProcessInputLine(string line)
        {
            PortInfo result = null;

            var commentParser = new Regex(@"^([^\W]+)\W+([0-9]+)/(tcp|udp)\W*([^#]*)?#?(.*)?$");
            
            var match = commentParser.Match(line);
            if (match.Success)
            {
                short portNumber;
                if (short.TryParse(match.Groups[2].Value, out portNumber))
                {
                    var alias = string.Empty;
                    var comment = string.Empty;
                    if (match.Groups.Count >= 4)
                        alias = match.Groups[4].Value.Trim();
                    if (match.Groups.Count >= 5)
                        comment = match.Groups[5].Value.Trim();
                    result = new PortInfo(match.Groups[1].Value.Trim(), match.Groups[3].Value.Trim(), portNumber, alias, comment, false);
                }
            }

            return result;
        }
    }
}
