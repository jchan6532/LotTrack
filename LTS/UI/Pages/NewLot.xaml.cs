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
using System.Windows.Threading;
using System.Threading;

using UI.WindowModel.Constants;

namespace UI.Pages
{
    /// <summary>
    /// Code behind class for the new lot page
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Page" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class NewLot : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewLot"/> code behind class.
        /// </summary>
        public NewLot()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the CreateLotBtn control and sends to the server a create lot action
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CreateLotBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ErrorMsgTextBlock.Visibility = Visibility.Hidden;
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);

            this.ErrorMsgTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            string msg = mainWindow.Client.CheckWaferAmount(this.WaferAmtTBox.Text);
            if (msg != ServerResponses.WaferAmtClientSideChecked)
            {
                this.WaferAmtTBox.Text = string.Empty;
                this.ErrorMsgTextBlock.Text = msg;
                this.ErrorMsgTextBlock.Visibility = Visibility.Visible;
                return;
            }

            mainWindow.Client.SendNewLotInfo(this.WaferAmtTBox.Text);
            while (true)
            {
                if (mainWindow.Client.ResponseReceived == true)
                {
                    msg = mainWindow.Client.CheckResponseCode();
                    break;
                }
                Thread.Sleep(10);
            }
            this.ErrorMsgTextBlock.Text = msg;

            if (msg == ServerResponses.AddLotSuccess)
            {
                this.ErrorMsgTextBlock.Visibility = Visibility.Visible;
                this.ErrorMsgTextBlock.Foreground = new SolidColorBrush(Colors.Green);
            }
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
