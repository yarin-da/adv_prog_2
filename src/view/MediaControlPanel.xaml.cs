using System;
using System.Windows;
using System.Windows.Controls;
using Adv_Prog_2.viewmodel;

namespace Adv_Prog_2
{
    public partial class MediaControlPanel : UserControl
    {
        private MediaControlViewModel vm;
        public MediaControlPanel()
        {
            InitializeComponent();
            vm = new MediaControlViewModel();
            DataContext = vm;
        }
    }
}
