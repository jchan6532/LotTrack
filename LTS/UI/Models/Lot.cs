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
    /// Model for storing 1 lot entry sent from the server
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Lot : INotifyPropertyChanged
    {
        private int m_lotID;
        private int m_waferAmount;
        private int m_lotState;
        private string m_lotStateStr;
        private string m_atLocationNumber;
        public List<int> WaferIDs;

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
        /// Gets or sets the lot identifier.
        /// </summary>
        /// <value>
        /// The lot identifier.
        /// </value>
        public int LotID 
        {
            get { return m_lotID; }
            set { m_lotID = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the wafer amount.
        /// </summary>
        /// <value>
        /// The wafer amount.
        /// </value>
        public int WaferAmount 
        {
            get { return m_waferAmount; }
            set { m_waferAmount = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the lot state.
        /// </summary>
        /// <value>
        /// The lot state.
        /// </value>
        public int LotState 
        {
            get { return m_lotState; }
            set { m_lotState = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the string representation of the lot state.
        /// </summary>
        /// <value>
        /// The string representation of the lot state.
        /// </value>
        public string LotStateStr
        {
            get { return m_lotStateStr; }
            set { m_lotStateStr = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the location number the lot is currently at.
        /// </summary>
        /// <value>
        /// The location number the lot is currently at.
        /// </value>
        public string AtLocationNumber 
        {
            get { return m_atLocationNumber; }
            set { m_atLocationNumber = value; NotifyPropertyChanged(); }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Lot"/> class.
        /// </summary>
        public Lot()
        {
            this.WaferIDs = new List<int>();
        }

        #endregion
    }
}
