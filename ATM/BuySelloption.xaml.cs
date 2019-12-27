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
    /// Interaction logic for BuySelloption.xaml
    /// </summary>
    public partial class BuySelloption : Window
    {
        public BuySelloption()
        {
            InitializeComponent();
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/Resources/RoundedButton.xaml", UriKind.RelativeOrAbsolute)
            });
            GetCurrentPrice();
            GetButtons();
        }

        // Date: 06/09/2019
        // Get current price of selected cryptocurrency and show.
        private void GetCurrentPrice()
        {
            dynamic selectedcyptocurrency = Application.Current.Resources["SelectedCoin"];
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

        // Date: 06/09/2019
        // Check status of selected cryptocurrency and show buttons accordingly.
        private void GetButtons()
        {
            dynamic selectedcyptocurrency = Application.Current.Resources["SelectedCoin"];
            dynamic apiresponse = Application.Current.Resources["APIresponse"];
            var i = 0;
            foreach (var item in apiresponse)
            {
                String coin = item.coin;
                if (selectedcyptocurrency == coin.ToUpper())
                {
                    if(item.buy["status"] == true)
                    {
                        System.Windows.Controls.Button newBtn = new Button();

                        newBtn.Content = "Buy" +" "+ coin.ToString().ToUpper();
                        newBtn.Name = "Buy" + coin.ToString();
                        newBtn.Background = Brushes.Black;
                        newBtn.Foreground = Brushes.White;
                        Thickness margin = newBtn.Margin;
                        margin.Top = 10;
                        margin.Left = 10;
                        newBtn.Margin = margin;
                        newBtn.Height = 95;
                        newBtn.Width = 250;
                        //newBtn.FontFamily = new FontFamily("Comic Sans MS");
                        newBtn.FontSize = 20;

                        var style = FindResource("RoundedButton") as Style;  //This is only added 
                        newBtn.Style = style;  //This is only added 


                        newBtn.Click += new RoutedEventHandler(Buybutton_Click);

                        Grid.SetColumn(newBtn, i);
                        i++;
                        grd_buysellbuttons.Children.Add(newBtn);

                    }
                    if (item.sell["status"] == true)
                    {
                        System.Windows.Controls.Button newBtn = new Button();

                        newBtn.Content = "Sell" + " " + coin.ToString().ToUpper();
                        newBtn.Name = "Sell" + coin.ToString();
                        newBtn.Background = Brushes.Black;
                        newBtn.Foreground = Brushes.White;
                        Thickness margin = newBtn.Margin;
                        margin.Top = 10;
                        margin.Left = 10;
                        newBtn.Margin = margin;
                        newBtn.Height = 95;
                        newBtn.Width = 250;
                        //newBtn.FontFamily = new FontFamily("Comic Sans MS");
                        newBtn.FontSize = 20;

                        var style = FindResource("RoundedButton") as Style;
                        newBtn.Style = style;

                        Grid.SetColumn(newBtn, i);

                        grd_buysellbuttons.Children.Add(newBtn);

                    }
                    break;
                }
            }
        }

        // Date: 09/09/2019
        // Proceed to buy selected cryptocurrency.
        private void Buybutton_Click(object sender, RoutedEventArgs e)
        {
            //ChooseCashLimit limit = new ChooseCashLimit();

            Application.Current.Resources["SelectedSide"] = Constants.SELECTED_SIDE_BUY;
            ScanQRCode limit = new ScanQRCode();
            limit.Show();
            this.Close();
        }

        // Date: 09/09/2019
        // Cancel functionality to go back page.
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedcyptocurrency = Application.Current.Resources["SelectedCoin"];
            Application.Current.Resources.Remove("SelectedCoin");
            dynamic selectedcyptocurrency1 = Application.Current.Resources["SelectedCoin"];
            CryptocurrencyOption cryptoopt = new CryptocurrencyOption();
            cryptoopt.Show();
            this.Close();
        }
    }
}
