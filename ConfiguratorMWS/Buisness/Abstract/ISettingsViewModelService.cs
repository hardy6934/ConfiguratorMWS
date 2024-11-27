

using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;

namespace ConfiguratorMWS.Buisness.Abstract
{
    public interface ISettingsViewModelService
    {
        public byte[] RawSerialize(object anything);
        public MwsTable ConvertMwsTableClassToStruct(MwsTableClass mwsTableClass);
        public MwsUserSettings ConvertUserSettingsClassToStruct(MwsUserSettingsClass settings);
        public MwsProdSettings ConvertMwsProdSettingsClassToStruct(MwsProdSettingsClass settings);

        public void ChangeTimerWorkInterval(int interval);
        public MWSEntity GetEntity();
        public void ChangeProgressBarValue(int value);

    }
}
