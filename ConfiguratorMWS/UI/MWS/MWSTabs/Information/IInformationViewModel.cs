

using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MwsModels;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Information
{
    public interface IInformationViewModel
    {
        public MWSEntity mWSEntity { get; set; }

        public List<string> PortList { get; set; }
        public string SelectedPort { get; set; }
        public double HeightOfFuelInTheTank { get; }

        public void RefreshComPorts();


        /// <summary>
        /// ////////
        /// </summary>
         
        //public double FuelLevel { get; set; }
        //public string FuelLevelPercent { get; set; }
    }
}
