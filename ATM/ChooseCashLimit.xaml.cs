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
using System.Windows.Shapes;

namespace ATM
{
    /// <summary>
    /// Interaction logic for ChooseCashLimit.xaml
    /// </summary>
    public partial class ChooseCashLimit : Window
    {
        public ChooseCashLimit()
        {
            InitializeComponent();
            GetCurrentPrice();
        }

        // Date: 17/09/2019
        // Get current price of selected cryptocurrency and show.
        private void GetCurrentPrice()
        {
            dynamic selectedcyptocurrency = Application.Current.Resources["BTNclicked"];
            dynamic apiresponse = Application.Current.Resources["APIresponse"];
            foreach (var item in apiresponse)
            {
                String coin = item.coin;
                if (selectedcyptocurrency == coin.ToUpper())
                {
                    Decimal Price = item.buy["twd"];
                    lbl_cryptoprice.Text = "1" + selectedcyptocurrency + " =";
                    lbl_twdprice.Text = Price.ToString() + " TWD";
                    break;
                }
            }
        }

        // Date: 10/09/2019
        // Cancel functionality to go back page.
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            BuySelloption cryptoopt = new BuySelloption();
            cryptoopt.Show();
            this.Close();
        }

        // Date: 10/09/2019
        // Functionality after choosing cash limit option.
        private void BtnChoosecash_Click(object sender, RoutedEventArgs e)
        {
            ScanQRCode cryptoopt = new ScanQRCode();
            cryptoopt.Show();
            this.Close();
        }
    }
}
