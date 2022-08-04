using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;


namespace UI.Models
{
    /// <summary>
    /// Model for storing 1 wafer entry sent from the server
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Wafer : INotifyPropertyChanged
    {
        private int m_waferID;
        private string m_belongsToLotID;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies controls and the collection the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #region Properties

        /// <summary>
        /// Gets or sets the wafer identifier.
        /// </summary>
        /// <value>
        /// The wafer identifier.
        /// </value>
        public int WaferID 
        {
            get { return m_waferID; }
            set { m_waferID = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the lot identifier the wafer is currently in.
        /// </summary>
        /// <value>
        /// The lot identifier the wafer is currently in.
        /// </value>
        public string BelongsToLotID 
        {
            get { return m_belongsToLotID; }
            set { m_belongsToLotID = value; NotifyPropertyChanged(); }
        }

        #endregion
    }
}
