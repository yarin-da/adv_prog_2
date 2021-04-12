using System;
using System.Windows;
using Adv_Prog_2.viewmodel;

namespace Adv_Prog_2
{
    public partial class MainWindow : Window
    {
        FlightViewModel vm = new FlightViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }
        public void OnWindowClose(object sender, EventArgs e)
        {
            vm.CloseApplication();
        }
    }
}
