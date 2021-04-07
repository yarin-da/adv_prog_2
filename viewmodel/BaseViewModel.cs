using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Adv_Prog_2.viewmodel
{
    class BaseViewModel : INotifyPropertyChanged
    {
        protected IModel model;

        #region ctor

        public BaseViewModel()
        {
            model = FlightModel.GetInstance();
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e)
                {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        #endregion

        #region property_changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
