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
    /// View model class for the location information page
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class LocationWM : INotifyPropertyChanged
    {
        private ObservableCollection<Location> m_locations;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies controls and the collection the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #region Properties

        /// <summary>
        /// Gets or sets the location collection.
        /// </summary>
        /// <value>
        /// The location collection.
        /// </value>
        public ObservableCollection<Location> Locations 
        {
            get { return m_locations; }
            set 
            {
                m_locations = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
