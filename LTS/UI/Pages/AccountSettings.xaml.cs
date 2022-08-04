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

namespace UI.Pages
{
    /// <summary>
    /// Code behind class for the account settings page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class AccountSettings : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountSettings"/> code behind class.
        /// </summary>
        public AccountSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the Page control and populates the account settings with user information
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            this.UserIDTextBlock.Text = mainWindow.Client.User.UserID.ToString();
            this.PasswordTextBlock.Text = mainWindow.Client.User.Password;
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
    }
}
