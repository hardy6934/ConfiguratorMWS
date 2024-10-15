

using ConfiguratorMWS.Entity;

namespace ConfiguratorMWS.UI.MWS.MWSTabs.Information
{
    public class InformationViewModel : IInformationViewModel
    {
        public MWSEntity MWSEntity { get; set; }

        public InformationViewModel(MWSEntity mWSEntity) {

            MWSEntity = mWSEntity;

        }



    }
}
