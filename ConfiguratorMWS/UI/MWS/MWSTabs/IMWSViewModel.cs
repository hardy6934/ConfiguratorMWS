

using ConfiguratorMWS.Commands;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    public interface IMWSViewModel
    {
        RelayCommand SwitchTabCommand { get; }

        object CurrentView { get; set; }
        void SwitchTab(object tab);
         

    }
}
