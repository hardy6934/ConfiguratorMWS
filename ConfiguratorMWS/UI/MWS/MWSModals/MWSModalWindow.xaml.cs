using System.Windows;

namespace ConfiguratorMWS.UI.MWS.MWSModals
{
    /// <summary>
    /// Логика взаимодействия для MWSModalWindow.xaml
    /// </summary>
    public partial class MWSModalWindow : Window
    {
        public MWSModalWindow()
        {
            InitializeComponent();
        }

        private void BackFromModal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
