using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;

using UI.Models;
using UI.WindowModel;
using UI.WindowModel.Constants;

namespace UI.Pages
{
    /// <summary>
    /// Code behind class for the lot information page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class LotInfo : Page
    {
        public LotWM LotModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LotInfo"/> code behind class.
        /// </summary>
        public LotInfo()
        {
            InitializeComponent();
            this.LotModel = (LotWM)this.DataContext;
        }

        /// <summary>
        /// Handles the Click event of the MoveLotBtn control sending to the server a move lot command for the specified lot
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MoveLotBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            bool validLotID = false;
            int lotID = 0;
            if(Int32.TryParse(this.LotIDTxtBox.Text, out lotID) == false)
            {
                this.ErrorMsgTextBlock.Text = ServerResponses.LotIDNotNumeric;
                this.ErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }

            this.ErrorMsgTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            foreach (Lot lot in mainWindow.Client.Lots)
            {
                if(this.LotIDTxtBox.Text == lot.LotID.ToString())
                {
                    validLotID = true;
                    break;
                }
            }
            if(validLotID == false)
            {
                this.ErrorMsgTextBlock.Text = ServerResponses.InvalidLotID;
                this.ErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }

            string msg = string.Empty;
            mainWindow.Client.SendMoveLotInfo(LotIDTxtBox.Text);
            while (true)
            {
                if(mainWindow.Client.ResponseReceived == true)
                {
                    msg = mainWindow.Client.CheckResponseCode();
                    break;
                }
                Thread.Sleep(10);
            }
            this.ErrorMsgTextBlock.Text = msg;

            if ((msg == ServerResponses.MoveLotSuccess))
            {
                this.ErrorMsgTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                this.ErrorMsgTextBlock.Visibility = Visibility.Visible;
            }
        }


        /// <summary>
        /// Handles the Click event of the Datagrid_SelectedBtn control and to populate the text box with the lot identifier of the row clicked
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Datagrid_SelectedBtn_Click(object sender, EventArgs e)            // FIXXXX
        {
            //DataGrid grid = sender as DataGrid;
            //Lot selectedLot = grid.SelectedItems[0] as Lot;
            //string lotID = selectedLot.LOTID.ToString();
            //string lotState = selectedLot.LOTSTATESTR.ToString();

            //if(lotState == "STANDBY")
            //{
            //    this.LotIDTxtBox.Text = lotID;
            //}
            //else
            //{
            //    this.LotIDTxtBox.Text = string.Empty;
            //    errorMsgBlck.Text = "lot is not ready yet";
            //    grid.SelectedItem = null;
            //}
        }

        /// <summary>
        /// Handles the Click event of the BackBtn control directing the user to the previous page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {

            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Content = new Pages.Home();
        }

        /// <summary>
        /// Handles the Loaded event of the Page control populating the data grid and the view model's collection with lot information
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            this.LotModel.Lots = ((Client)mainWindow.DataContext).Lots;
            this.LotDataGrid.ItemsSource = ((Client)mainWindow.DataContext).Lots;
        }
    }
}
