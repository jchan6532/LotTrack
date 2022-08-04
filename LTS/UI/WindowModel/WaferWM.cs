using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

using UI.Models;

namespace UI.WindowModel
{
    /// <summary>
    /// View model class for the wafer information page
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class WaferWM : INotifyPropertyChanged
    {
        private ObservableCollection<Wafer> m_wafers;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies controls and the collection the property changed
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #region Properties

        /// <summary>
        /// Gets or sets the wafer collection.
        /// </summary>
        /// <value>
        /// The wafer collection.
        /// </value>
        public ObservableCollection<Wafer> Wafers 
        {
            get { return m_wafers; }
            set 
            { 
                m_wafers = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
