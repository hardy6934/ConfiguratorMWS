﻿
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using ConfiguratorMWS.Buisness.Abstract;

namespace ConfiguratorMWS.UI.MWS.MWSTabs
{
    /// <summary>
    /// Interaction logic for MWSWindow.xaml
    /// </summary>
    public partial class MWSWindow : Window
    {
        private readonly IMWSViewModel mWSViewModel; 
        private readonly IMWSService mWSService; 
        public MWSWindow(IMWSViewModel mWSViewModel, IMWSService mWSService)
        {
            InitializeComponent();
            DataContext = mWSViewModel;
            this.mWSService = mWSService;
        }

        private void CommPortCombobox_DropDownOpened(object sender, EventArgs e)
        {
            // Принудительно обновляем список портов при открытии выпадающего списка
            var viewModel = DataContext as MWSViewModel;
            if (viewModel != null)
            {
                viewModel.RefreshComPorts();
            }
        }
    }
     
}