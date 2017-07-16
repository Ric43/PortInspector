namespace MOCSoftware.Utilities.PortInspector.Communication
{
    internal class StatusUpdateMessage : Message
    {
        internal int ProgressValue { get; set; }
        internal bool ShowBusy { get; set; }
        internal string StatusMessage { get; set; }
    }
}
