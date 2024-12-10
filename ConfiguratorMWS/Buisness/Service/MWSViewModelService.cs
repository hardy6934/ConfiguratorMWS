using ConfiguratorMWS.Buisness.Abstract;
using ConfiguratorMWS.Data.Abstract;
using ConfiguratorMWS.Entity;
using ConfiguratorMWS.Resources;
using ConfiguratorMWS.UI.MWS.MWSModals;
using ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NPOI.HPSF;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace ConfiguratorMWS.Buisness.Service
{
    public class MWSViewModelService : IMWSViewModelService
    {
        private readonly IMWSRepository mwsRepository;
        private readonly IServiceProvider serviceProvider;
        public LocalizedStrings localizedStrings { get; set; }

        public MWSViewModelService(IMWSRepository mwsRepository, IServiceProvider serviceProvider)
        {
            this.mwsRepository = mwsRepository;
            this.serviceProvider = serviceProvider;

            localizedStrings = (LocalizedStrings) Application.Current.Resources["LocalizedStrings"];
        }

        public void ChangeProgressBarValue(int value)
        {
            mwsRepository.ChangeProgressBarValue(value);
        }

        public MWSEntity GetEntity()
        {
            return mwsRepository.GetEntity();
        }

        public void ClearToken()
        {
            Properties.Settings.Default.Token = "";
            Properties.Settings.Default.TokenAccountId = "";
            Properties.Settings.Default.TokenExpDate = "";
            Properties.Settings.Default.TokenRole = "";
            Properties.Settings.Default.Save();
        }

        

        public void ShowUpdateFirmwareWindow(Window parentWindow)
        {
            var updateFirmwareWindow = serviceProvider.GetRequiredService<UpdateFirmwareWindow>();
            updateFirmwareWindow.Owner = parentWindow;
            updateFirmwareWindow.Closed += (sender, e) =>
            {
                // Показываем родительское окно при закрытии
                parentWindow.Show();
            };

            parentWindow.Hide(); // Скрываем родительское окно
            updateFirmwareWindow.Show();
        }

        public void ShowAuthorizationModal(Window parentWindow)
        {
            var authorizationModalWindow = serviceProvider.GetRequiredService<AuthorizationModalWindow>();
            authorizationModalWindow.Owner = parentWindow;
            authorizationModalWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            authorizationModalWindow.ShowDialog();
        }
         
        public void EnsurePendingRequestsFolderExists()
        {
            string pendingRequestsFolder = Properties.Settings.Default.PendingRequestsFolder;
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(appData, pendingRequestsFolder);
               
            try 
            {
                if (!Directory.Exists(folderPath) && !string.IsNullOrEmpty(appData))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch {
            }
            
        }

        public async Task RetryPendingRequestsAsync()
        {
            string pendingRequestsFolder = Properties.Settings.Default.PendingRequestsFolder;
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pendingRequestsFolder);

            var url = Properties.Settings.Default.ApiUrl; 
            var token = Properties.Settings.Default.Token;
            int tokenAccountId;

            if (!string.IsNullOrEmpty(token) && int.TryParse(Properties.Settings.Default.TokenAccountId, out tokenAccountId) && Directory.Exists(folderPath))
            {
                DateTime tokenExpDate;

                if (DateTime.TryParse(Properties.Settings.Default.TokenExpDate, out tokenExpDate))
                {
                    if (tokenExpDate > DateTime.Now)
                    {
                        try
                        {
                            foreach (var file in Directory.GetFiles(folderPath, "*.json"))
                            {
                                if (file.Contains("requestConHistory"))
                                {
                                    var json = File.ReadAllText(file);
                                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                                    using (HttpClient client = new HttpClient())
                                    {
                                        var response = await client.GetAsync(url + "/Ping");
                                        if (response.IsSuccessStatusCode)
                                        {
                                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                                            response = await client.PostAsync(url + "/api/APIConnectionsHistory", jsonContent);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                File.Delete(file);
                                            }
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                }
                                else if (file.Contains("requestSettings_"))
                                {
                                    string jsonData = File.ReadAllText(file);
                                    using (JsonDocument document = JsonDocument.Parse(jsonData))
                                    {
                                        JsonElement root = document.RootElement; 
                                        // Извлечение данных
                                        string accountId = root.GetProperty("AccountId").GetString();
                                        string serialNumber = root.GetProperty("SerialNumber").GetString();
                                        string mac = root.GetProperty("MAC").GetString();
                                        string sensorName = root.GetProperty("SensorName").GetString();
                                        string fileName = root.GetProperty("FileName").GetString();
                                        string fileContentString = root.GetProperty("FileContent").GetString();

                                        if (int.TryParse(accountId, out _) && !string.IsNullOrEmpty(serialNumber) && !string.IsNullOrEmpty(sensorName) 
                                            && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(fileContentString) )
                                        {
                                            var requestBody = new MultipartFormDataContent
                                            {
                                                { new StringContent(accountId, Encoding.UTF8), "accountId" },
                                                { new StringContent(serialNumber), "serialNumber" },
                                                { new StringContent(""), "MAC" },
                                                { new StringContent(sensorName), "sensorName" },
                                                { new StringContent(fileContentString, Encoding.UTF8), "file", $"{fileName}" }
                                            };

                                            using (HttpClient client = new HttpClient())
                                            {
                                                var response = await client.GetAsync(url + "/Ping");
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    client.DefaultRequestHeaders.Authorization =
                                                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                                                    response = await client.PostAsync(url + "/api/APISettings", requestBody);

                                                    if (response.IsSuccessStatusCode)
                                                    {
                                                        File.Delete(file);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            File.Delete(file);
                                        }

                                    }  
                                     
                                }
                                else
                                {
                                    File.Delete(file);
                                }

                            }
                        }
                        catch {
                            return;
                        } 

                    }
                }
            } 

        }





        private string ExtractVersion(string fileName)
        { 
            // Регулярное выражение для поиска версии в формате x.y.z 
            var regex = new Regex(@"\d+\.\d+\.\d+");
            var match = regex.Match(fileName);
            return match.Success ? match.Value : string.Empty; // Возвращаем версию или пустую строку 
        }  
        public async Task DownloadInstallerFolderAsync()
        {  
            var setuperUrl = Properties.Settings.Default.SetuperUrl; 
            string currentConfigVersion = Properties.Settings.Default.AppVersion;

            try
            { 
                using (HttpClient client = new HttpClient())
                {
                    // Отправляем запрос с методом HEAD
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, setuperUrl);
                    HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                    if (response.IsSuccessStatusCode)
                    {
                        // Ищем заголовок Content-Disposition
                        if (response.Content.Headers.ContentDisposition != null)
                        {
                            var fileName = response.Content.Headers.ContentDisposition.FileName?.Trim('"');

                            // Сравниваем версии
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                string configuratorVersion = ExtractVersion(fileName);

                                if (!string.IsNullOrEmpty(configuratorVersion) && configuratorVersion != currentConfigVersion)
                                {
                                    var result = MessageBox.Show(localizedStrings["NewVersionAvailable"] + $"{configuratorVersion}." + localizedStrings["InstallTheUpdate"],
                                   localizedStrings["Update"],
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Question);

                                    if (result == MessageBoxResult.Yes)
                                    {
                                        OpenUpdateLinkInBrowser();
                                    }
                                }    
                            }
                        }
                    }
                }
 
            }
            catch (Exception ex)
            {
            } 
        }

        public void OpenUpdateLinkInBrowser()
        {
            try
            {
                string folderArchiveUrl = Properties.Settings.Default.SetupZipUrl;

                // Открываем ссылку в браузере
                Process.Start(new ProcessStartInfo
                {
                    FileName = folderArchiveUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            { 
            }

        }


      


    }
}
