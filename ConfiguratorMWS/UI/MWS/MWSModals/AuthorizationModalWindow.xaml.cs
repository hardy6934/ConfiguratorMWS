using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Net.Http;
using ConfiguratorMWS.Entity;
using System.Security.Policy;
using ConfiguratorMWS.Resources;
using System.Text;
using System.Text.Json;

namespace ConfiguratorMWS.UI.MWS.MWSModals
{
    /// <summary>
    /// Interaction logic for AuthorizationModalWindow.xaml
    /// </summary>
    public partial class AuthorizationModalWindow : Window
    {
        private readonly LocalizedStrings localizedStrings;
        public AuthorizationModalWindow()
        {
            InitializeComponent();
            localizedStrings = (LocalizedStrings)Application.Current.Resources["LocalizedStrings"];
        }

        public class MyResponseModel
        {
            public string accesToken { get; set; }
            public DateTime tokenExp { get; set; }
            public string role { get; set; }
            public int accountId { get; set; }
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Открыть браузер и перейти по ссылке
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void BackFromModal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void LogInButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Отключить кнопку, чтобы предотвратить повторные нажатия
            LogInButton.IsEnabled = false;

            try
            {
                // Запуск асинхронной операции
                bool success = await LogInAsync();

                if (success)
                {
                    this.DialogResult = true; // Закрытие окна с успешным результатом
                }
                else
                {
                    ErrorMessage.Text = localizedStrings["InvalidLoginOrPassword"]; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(localizedStrings["CheckInternetConnection"], localizedStrings["strError"]);
            }
            finally
            {
                // Включить кнопку после завершения операции
                LogInButton.IsEnabled = true;
            }
        }  
         

        private void EmailField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ErrorMessage != null)
            {
                ErrorMessage.Text = "";
            }
        }

        private void PasswordField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ErrorMessage != null)
            {
                ErrorMessage.Text = "";
            }
        }



        private async Task<bool> LogInAsync()
        {
            try
            {
                var url = Properties.Settings.Default.ApiUrl;

                var requestBody = new
                {
                    login = EmailField.Text,
                    password = PasswordField.Text,
                    loginKeyAccessId = ""
                };
                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    // Проверка доступности сервера
                    var response = await client.GetAsync(url + "/Ping");
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Cannot reach server. Check your internet connection.");
                    }

                    // Отправка запроса на авторизацию
                    response = await client.PostAsync(url + "/api/APIAccount", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<MyResponseModel>(responseBody);

                        // Проверка ответа
                        if (!string.IsNullOrEmpty(responseObject.accesToken))
                        {
                            // Сохранение токена
                            Properties.Settings.Default.Token = responseObject.accesToken;
                            Properties.Settings.Default.TokenAccountId = responseObject.accountId.ToString();
                            Properties.Settings.Default.TokenExpDate = responseObject.tokenExp.ToString();
                            Properties.Settings.Default.TokenRole = responseObject.role;
                            Properties.Settings.Default.Save();

                            return true;
                        }
                    }

                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

    }


}
