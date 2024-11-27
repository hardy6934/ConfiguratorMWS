using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using System.Runtime.InteropServices;

namespace ConfiguratorMWS.Buisness.Service
{
    public class SettingsViewModelService : ISettingsViewModelService
    {
        private readonly IMWSRepository mWSRepository; 
        private readonly ITimerRepository timerRepository;

        public SettingsViewModelService(IMWSRepository mWSRepository, ITimerRepository timerRepository) { 
            this.timerRepository = timerRepository;
            this.mWSRepository = mWSRepository;
        }

        public MwsTable ConvertMwsTableClassToStruct(MwsTableClass mwsTableClass)
        {
            var mwsTableStruct = new MwsTable
            {
                rows = new MwsRow[256]
            };

            for (int i = 0; i < mwsTableClass.Rows.Count; i++)
            {
                mwsTableStruct.rows[i] = new MwsRow
                {
                    distance = (float)Math.Round(mwsTableClass.Rows[i].Distance / 1000, 3),
                    volume = mwsTableClass.Rows[i].Volume
                };
            }
            mwsTableStruct.rows[mwsTableClass.Rows.Count] = new MwsRow { volume = 100000.0f, distance = 100000.0f };

            return mwsTableStruct;
        }

        public MwsUserSettings ConvertUserSettingsClassToStruct(MwsUserSettingsClass settings)
        {
            return new MwsUserSettings
            {
                flashUserSetYear = settings.FlashUserSetYear,
                flashUserSetMonth = settings.FlashUserSetMonth,
                flashUserSetDay = settings.FlashUserSetDay,
                flashUserSerialNumber = settings.FlashUserSerialNumber,
                flashUserpassword = settings.FlashUserpassword,
                flashUserConfigFlags = settings.FlashUserConfigFlags,
                flashUserBaudRateRs = settings.FlashUserBaudRateRs,
                flashUserSpeedCan = settings.FlashUserSpeedCan,
                flashUserAdrSensor = settings.FlashUserAdrSensor,
                flashUserLlsMinN = settings.FlashUserLlsMinN,
                flashUserLlsMaxN = settings.FlashUserLlsMaxN,
                flashUserSaCan = settings.FlashUserSaCan,
                flashUserTankHeight = settings.FlashUserTankHeight,
                flashUserAverageS = settings.FlashUserAverageS,
                flashUserThreshold = settings.FlashUserThreshold
            };
        }
        public MwsProdSettings ConvertMwsProdSettingsClassToStruct(MwsProdSettingsClass settings)
        {
            return new MwsProdSettings
            {
                prodYear = settings.ProdYear,
                prodMonth = settings.ProdMonth,
                prodDay = settings.ProdDay,
                serialNumber = settings.SerialNumber,
                deviceType = settings.DeviceType,
                password = settings.Password,  
            };

    }

        public byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(anything, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return rawdata;
        }

        public void ChangeTimerWorkInterval(int interval)
        {
            timerRepository.ChangeTimerWorkInterval(interval);

        }

        public MWSEntity GetEntity() {
            return mWSRepository.GetEntity();
        }
        public void ChangeProgressBarValue(int value)
        {
            mWSRepository.ChangeProgressBarValue(value);
        }


    }
}
