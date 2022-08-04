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
    /// Code behind class for the wafer information page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class PlantModel : Page
    {
        public WaferWM WaferModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantModel"/> code behind class.
        /// </summary>
        public PlantModel()
        {
            InitializeComponent();
            this.WaferModel = (WaferWM)this.DataContext;
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
        /// Handles the Loaded event of the Page control populating the data grid and the view model's collection with wafer information
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            this.WaferModel.Wafers = ((Client)mainWindow.DataContext).Wafers;
            this.WaferDataGrid.ItemsSource = ((Client)mainWindow.DataContext).Wafers;
        }
    }
}
