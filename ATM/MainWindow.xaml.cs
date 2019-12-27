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
using WpfAnimatedGif;
using System.Windows.Forms;
using System.Drawing;

namespace ATM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // navigate to the next screen on touch.
        // 17/09/2019
        private void Backgroundimg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CryptocurrencyOption cryptoopt = new CryptocurrencyOption();
            cryptoopt.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CryptocurrencyOption cryptoopt = new CryptocurrencyOption();
            cryptoopt.Show();
            this.Close();
        }
    }
}
