

using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using System.IO;
using System.Net.Http;
using System.Text.Json;

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
        public Task SendSettingsOnServerAsync(byte[] bufferFlashDataForWr, uint SerialNumberFullFormat, string prodType);
        public void SaveSettingsRequestForLater(byte[] bufferFlashDataForWr, uint SerialNumberFullFormat, string prodType);


    }
}
