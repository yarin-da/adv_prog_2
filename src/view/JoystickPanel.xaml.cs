using System.Windows.Controls;
using Adv_Prog_2.viewmodel;

namespace Adv_Prog_2
{
    public partial class JoystickPanel : UserControl
    {
        private JoystickViewModel vm;
        public JoystickPanel()
        {
            InitializeComponent();
            vm = new JoystickViewModel();
            DataContext = vm;
        }
    }
}
