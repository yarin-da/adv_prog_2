using System.Windows.Controls;
using Adv_Prog_2.viewmodel;

namespace Adv_Prog_2
{
    public partial class GraphPanel : UserControl
    {
        private GraphViewModel vm;

        public GraphPanel()
        {
            InitializeComponent();
            vm = new GraphViewModel();
            DataContext = vm;
        }
    }
}
