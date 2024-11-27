using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ConfiguratorMWS.UI.MWS.MWSWindowUpdateFirmmware
{
    /// <summary>
    /// Логика взаимодействия для UpdateFirmwareWindow.xaml
    /// </summary>
    public partial class UpdateFirmwareWindow : Window
    {
        private readonly IUpdateFirmwareViewModel viewModel;
        public UpdateFirmwareWindow(IUpdateFirmwareViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;

            viewModel.UpdateFirmwareFromRemoteServerCommand.Execute(this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Открыть браузер и перейти по ссылке
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) 
        { 
            if (e.LeftButton == MouseButtonState.Pressed) 
            { 
                this.DragMove(); 
            } 
        }



        //// Импортируем функции WinAPI
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        //[DllImport("user32.dll")]
        //private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        //private const uint SC_CLOSE = 0xF060;
        //private const uint MF_BYCOMMAND = 0x00000000;
        //private const uint MF_GRAYED = 0x00000001;

        //private void Window_SourceInitialized(object sender, EventArgs e)
        //{
        //    // Получаем хэндл окна
        //    IntPtr hWnd = new WindowInteropHelper(this).Handle;
        //    IntPtr hMenu = GetSystemMenu(hWnd, false);

        //    // Отключаем кнопку закрытия
        //    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        //}



    }
}
