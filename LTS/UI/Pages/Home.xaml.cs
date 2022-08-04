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

using UI.WindowModel.Constants;
using UI.WindowModel;

namespace UI.Pages
{
    /// <summary>
    /// Code behind class for the home page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class Home : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Home"/> code behind class.
        /// </summary>
        public Home()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the ModelBtn control directing the user to the wafer information page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ModelBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Content = new Pages.PlantModel();
        }

        /// <summary>
        /// Handles the Click event of the SettingsBtn control directing the user to the account settings information page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Content = new Pages.AccountSettings();
        }

        /// <summary>
        /// Handles the Click event of the LotInfoBtn control directing the user to the lot information page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LotInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Content = new Pages.LotInfo();
        }

        /// <summary>
        /// Handles the Click event of the LocationInfoBtn control directing the user to the location information page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LocationInfoBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Content = new Pages.LocationInfo();
        }

        /// <summary>
        /// Handles the Click event of the LogoutBtn control directing the user to the login page
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            string response = string.Empty;

            mainWindow.Client.Disconnect();
            while (true)
            {
                if (mainWindow.Client.ResponseReceived == true)
                {
                    response = mainWindow.Client.CheckResponseCode();
                    break;
                }
                Thread.Sleep(10);
            }

            if (response == ServerResponses.LogOutSuccess)
            {
                mainWindow.MainFrame.Content = new Pages.Login();
            }
            else if (response == ServerResponses.LogOutFailed)
            {
                MessageBox.Show(response);
            }
        }

        /// <summary>
        /// Handles the Click event of the NewlotBtn control directing the user to the page for creating a new lot
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NewlotBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Content = new Pages.NewLot();
        }
    }
}
