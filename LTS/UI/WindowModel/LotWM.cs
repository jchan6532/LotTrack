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
    /// View model class for the lot information page
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class LotWM : INotifyPropertyChanged
    {
        private ObservableCollection<Lot> m_lots;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies controls and the collection the property changed.
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
        /// Gets or sets the lot collection.
        /// </summary>
        /// <value>
        /// The lot collection.
        /// </value>
        public ObservableCollection<Lot> Lots 
        { 
            get { return m_lots; }
            set 
            { 
                m_lots = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
