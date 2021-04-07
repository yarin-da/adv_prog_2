using System.Windows.Controls;
using Adv_Prog_2.viewmodel;

namespace Adv_Prog_2
{

    public partial class DashboardPanel : UserControl
    {
        private DashboardViewModel vm;
        public DashboardPanel()
        {
            InitializeComponent();
            vm = new DashboardViewModel();
            DataContext = vm;
        }
    }
}
