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

using UI.Models;
using UI;
using UI.Models.Constants;
using UI.WindowModel.Constants;

namespace UI.Pages
{
    /// <summary>
    /// Code behind class for the login page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class Login : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> code behind class.
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the loginBtn control to log the user into the system
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ErrorMsgTextBlock.Visibility = Visibility.Visible;
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

            this.ErrorMsgTextBlock.Text = mainWindow.Client.CheckUserID(this.UserIDTextBox.Text);
            if(this.ErrorMsgTextBlock.Text == ServerResponses.UserIDNotNumeric)
            {
                this.UserIDTextBox.Text = string.Empty;
                this.PasswordTextBox.Text = string.Empty;
                return;
            }

            this.ErrorMsgTextBlock.Text = mainWindow.Client.CheckPassword(this.PasswordTextBox.Text);
            if (this.ErrorMsgTextBlock.Text == ServerResponses.PasswordEmpty)
            {
                this.UserIDTextBox.Text = string.Empty;
                this.PasswordTextBox.Text = string.Empty;
                return;
            }



            try
            {
                mainWindow.Client.Connect(Int32.Parse(this.UserIDTextBox.Text), this.PasswordTextBox.Text);
            }
            catch (Exception ex)
            {
                this.ErrorMsgTextBlock.Text = ex.Message;
                return;
            }

            while (true)
            {
                if (mainWindow.Client.ResponseReceived == true)
                {
                    this.ErrorMsgTextBlock.Text = mainWindow.Client.CheckResponseCode();
                    break;
                }
                Thread.Sleep(10);
            }

            if (this.ErrorMsgTextBlock.Text == ServerResponses.LogInSuccess)
            {
                this.ErrorMsgTextBlock.Visibility = Visibility.Hidden;
                mainWindow.MainFrame.Content = new Pages.Home();
            }
            this.UserIDTextBox.Text = string.Empty;
            this.PasswordTextBox.Text = string.Empty;
            return;
        }
    }
}
