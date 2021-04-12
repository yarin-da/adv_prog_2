using System.Collections.Generic;
using OxyPlot;

namespace Adv_Prog_2.viewmodel
{
    class GraphViewModel : BaseViewModel
    {
        #region properties
        public PlotModel VM_SelectedPlot { get { return model.SelectedPlot; } }
        public PlotModel VM_CorrelatedPlot { get { return model.CorrelatedPlot; } }
        public PlotModel VM_AnomalyPlot { get { return model.AnomalyPlot; } }
        public List<string> VM_ColumnList { get { return model.ColumnList; } }
        public string VM_SelectedColumn
        {
            get { return model.SelectedColumn; }
            set { model.SelectedColumn = value; }
        }
        #endregion
    }
}
