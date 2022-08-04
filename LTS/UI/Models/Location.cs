using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

using UI.WindowModel.Constants;
using UI.Models.Constants;

namespace UI.Models
{
    /// <summary>
    /// Model for storing 1 location entry sent from the server
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Location : INotifyPropertyChanged
    {
        private int m_locationID;
        private int m_locationNumber;
        private int m_locationState;
        private string m_locationStateStr;
        private string m_hasLotID;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies controls and the collection the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #region Properties

        /// <summary>
        /// Gets or sets the location identifier.
        /// </summary>
        /// <value>
        /// The location identifier.
        /// </value>
        public int LocationID 
        {
            get { return m_locationID; }
            set { m_locationID = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the location number.
        /// </summary>
        /// <value>
        /// The location number.
        /// </value>
        public int LocationNumber 
        { 
            get { return m_locationNumber; } 
            set { m_locationNumber = value; NotifyPropertyChanged(); } 
        }

        /// <summary>
        /// Gets or sets the location state.
        /// </summary>
        /// <value>
        /// The location state.
        /// </value>
        public int LocationState 
        {
            get { return m_locationState; }
            set { m_locationState = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the string representation of the location state.
        /// </summary>
        /// <value>
        /// The string representation of the location state.
        /// </value>
        public string LocationStateStr 
        {
            get { return m_locationStateStr; }
            set { m_locationStateStr = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the lot identifier currently at the location.
        /// </summary>
        /// <value>
        /// The lot identifier currently at the location.
        /// </value>
        public string HasLotID 
        {
            get { return m_hasLotID; }
            set { m_hasLotID = value; NotifyPropertyChanged(); }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        public Location()
        {
            this.m_locationState = ServerLocationState.Vacant;
            this.m_locationStateStr = "VACANT";
            this.m_hasLotID = "UNDEFINED";
        }

        #endregion
    }
}
