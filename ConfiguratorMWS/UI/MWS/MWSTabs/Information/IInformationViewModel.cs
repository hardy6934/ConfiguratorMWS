

using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Information
{
    public interface IInformationViewModel
    {
        MWSEntity mWSEntity { get; set; }

        public List<string> PortList { get; set; }
        public string SelectedPort { get; set; }

        public void RefreshComPorts();
    }
}
