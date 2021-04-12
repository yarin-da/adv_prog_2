using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Adv_Prog_2.viewmodel;
using Microsoft.Win32;

namespace Adv_Prog_2
{
    public partial class SessionSetupPanel : UserControl
    {
        private SessionSetupViewModel vm;
        SolidColorBrush green = new SolidColorBrush(Colors.LightGreen);

        public SessionSetupPanel()
        {
            InitializeComponent();
            vm = new SessionSetupViewModel();
            DataContext = vm;
        }
    }
}
