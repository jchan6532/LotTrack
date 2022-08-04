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

using UI.Models;
using UI.WindowModel;
using UI.WindowModel.Constants;

namespace UI.Pages
{
    /// <summary>
    /// Code behind class for the location information page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class LocationInfo : Page
    {
        public LocationWM LocationModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationInfo"/> code behind class.
        /// </summary>
        public LocationInfo()
        {
            InitializeComponent();
            this.LocationModel = (LocationWM)this.DataContext;
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
        /// Handles the Loaded event of the Page control populating the data grid and the view model's collection with location information
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            this.LocationModel.Locations = ((Client)mainWindow.DataContext).Locations;
            this.LocationDataGrid.ItemsSource = ((Client)mainWindow.DataContext).Locations;
        }
    }
}
