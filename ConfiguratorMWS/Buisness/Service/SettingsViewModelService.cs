using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Data.Repository;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Entity.MWSStructs;
using ConfiguratorMWS.Entity.MWSSubModels;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

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

        public async Task SendSettingsOnServerAsync(byte[] bufferFlashDataForWr, uint SerialNumberFullFormat, string prodType)
        {
            //отправка настроек на сервер
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Token) && !string.IsNullOrEmpty(Properties.Settings.Default.TokenAccountId))
            {
                DateTime tokenExpDate;

                if (DateTime.TryParse(Properties.Settings.Default.TokenExpDate, out tokenExpDate))
                {
                    if (tokenExpDate > DateTime.Now)
                    {

                        var url = Properties.Settings.Default.ApiUrl;
                        var tokenAccountId = Properties.Settings.Default.TokenAccountId;
                        var token = Properties.Settings.Default.Token;

                        string byteArrayAsString = string.Join(" ", bufferFlashDataForWr.Select(b => b.ToString()));

                        var requestBody = new MultipartFormDataContent
                        {
                            { new StringContent(tokenAccountId, Encoding.UTF8), "accountId" },
                            { new StringContent(SerialNumberFullFormat.ToString()), "serialNumber" },
                            { new StringContent(""), "MAC" },
                            { new StringContent(prodType), "sensorName" },
                            { new StringContent(byteArrayAsString, Encoding.UTF8), "file", $"{prodType}_{SerialNumberFullFormat}_{DateTime.Now:yyyyMMddHHmmss}.txt" }
                        };

                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                var response = await client.GetAsync(url + "/Ping");
                                if (response.IsSuccessStatusCode)
                                {
                                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                                    response = await client.PostAsync(url + "/api/APISettings", requestBody);
                                }
                                else
                                {
                                    SaveSettingsRequestForLater(bufferFlashDataForWr, SerialNumberFullFormat, prodType);
                                }
                            }
                        }
                        catch (Exception ex) {
                            SaveSettingsRequestForLater(bufferFlashDataForWr, SerialNumberFullFormat, prodType);
                        }

                    }
                }
            } 
        }

        public void SaveSettingsRequestForLater(byte[] bufferFlashDataForWr, uint SerialNumberFullFormat, string prodType)
        {
            try
            {
                string pendingRequestsFolder = Properties.Settings.Default.PendingRequestsFolder;
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string folderPath = Path.Combine(appData, pendingRequestsFolder);

                if (!Directory.Exists(folderPath) && !string.IsNullOrEmpty(appData))
                {
                    Directory.CreateDirectory(folderPath);
                }
                    
                if (Directory.Exists(folderPath))
                {
                    string byteArrayAsString = string.Join(" ", bufferFlashDataForWr.Select(b => b.ToString()));
                    var data = new
                    {
                        AccountId = Properties.Settings.Default.TokenAccountId,
                        SerialNumber = SerialNumberFullFormat.ToString(),
                        MAC = "",
                        SensorName = prodType,
                        FileName = $"{prodType}_{SerialNumberFullFormat.ToString()}_{DateTime.Now:yyyyMMddHHmmss}.txt",
                        FileContent = byteArrayAsString
                    };

                    // Сохранение данных в файл
                    string filePath = Path.Combine(folderPath, $"requestSettings_{DateTime.Now:yyyyMMddHHmmss}.json");
                    File.WriteAllText(filePath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
                }
                
            }
            catch (Exception ex)
            { 
            }

        }


    }
}
