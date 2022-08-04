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

namespace UI
{
    /// <summary>
    /// Entry and starting point for the UI project/application
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow : Window
    {
        public Client Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class initializing the data context and starting a client object
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Client = new Client();
            this.DataContext = this.Client;
            this.MainFrame.Content = new Pages.Login();
        }
    }
}
