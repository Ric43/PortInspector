using MOCSoftware.Utilities.PortInspector.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MOCSoftware.Utilities.PortInspector.Communication;

namespace MOCSoftware.Utilities.PortInspector.Persistence
{
    internal class HostDataPersistor: IHostDataPersistor
    {
        public void LoadHostData()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            SetDialogOptions(dlg, Resources.HostDataPersistor_FileLoadDialogTitle);
            if (dlg.ShowDialog() ?? false)
            {
                App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = true, StatusMessage = Resources.StatusMessageLoadingHosts,
                    ProgressValue = App._PROGRESS_BAR_HIDE });
                var task = Task.Factory.StartNew(() => Load(dlg.FileName));
            }
        }

        public void SaveHostData(List<string> hostInfo)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            SetDialogOptions(dlg, Resources.HostDataPersistor_FileSaveDialogTitle);
            if (dlg.ShowDialog() ?? false)
            {
                App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = true, StatusMessage = Resources.StatusMessageLoadingHosts,
                    ProgressValue = App._PROGRESS_BAR_HIDE });
                var task = Task.Factory.StartNew(() => Save(hostInfo, dlg.FileName));
            }
        }

        private void SetDialogOptions(Microsoft.Win32.FileDialog dialog, string title)
        {
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = title;
            dialog.ValidateNames = true;
        }

        private void Save(List<string> hostList, string fileName)
        {
            try
            {
                using (var writer = File.CreateText(fileName))
                {
                    for (int hostListIndex = 0; hostListIndex < hostList.Count; hostListIndex++)
                    {
                        writer.WriteLine(hostList[hostListIndex]);
                        App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage
                        {
                            ProgressValue = (int)Math.Round((decimal)hostListIndex / (decimal)(hostList.Count * 100), 0)
                        });
                    }
                    App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { StatusMessage = string.Empty, ShowBusy = false });
                }
            }
            catch (Exception ex)
            {
                App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = false, StatusMessage = Resources.StatusMessageError });
                App.Mediator.Publish(App._EVENT_ERROR, new ErrorMessage
                {
                    ErrorTitle = Resources.ErrorMessageHostSaveTitle,
                    ErrorText = string.Format(Resources.ErrorMessageHostSaveText, ex.Message)
                });
            }
        }

        private void Load(string fileName)
        {
            var loadResult = new List<string>();
            try
            {
                using (var writer = File.OpenText(fileName))
                {
                    string line;
                    while ((line = writer.ReadLine()) != null)
                    {
                        loadResult.Add(line);
                    }
                }
                App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = false, StatusMessage = string.Empty });
                App.Mediator.Publish(App._EVENT_HOSTDATA_LOADED, new HostFileLoadedMessage { HostData = loadResult});
            }
            catch (Exception ex)
            {
                App.Mediator.Publish(App._EVENT_STATUSUPDATE, new StatusUpdateMessage { ShowBusy = false, StatusMessage = Resources.StatusMessageError });
                App.Mediator.Publish(App._EVENT_ERROR, new ErrorMessage
                {
                    ErrorTitle = Resources.ErrorMessageHostLoadTitle,
                    ErrorText = string.Format(Resources.ErrorMessageHostLoadText, ex.Message)
                });
            }
        }
    }
}
