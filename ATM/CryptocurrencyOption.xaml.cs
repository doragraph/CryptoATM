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
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ATM
{
    /// <summary>
    /// Interaction logic for CryptocurrencyOption.xaml
    /// </summary>
    public partial class CryptocurrencyOption : Window
    {
        public CryptocurrencyOption()
        {
            InitializeComponent();
            
            foreach(var res in Application.Current.Resources.Keys)
            {
                Application.Current.Resources.Remove(res);
            }

            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/Resources/RoundedButton.xaml", UriKind.RelativeOrAbsolute)
            });
            GetAPIResponse();
        }

        // Date: 06/09/2019
        // Get API response(coin list and their properties.)
        private void GetAPIResponse()
        {
            string url = Constants.COIN_LIST_API;
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            //request.UserAgent = RequestConstants.UserAgentValue;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            //var content = @"{ ""url"":""/coinList"",""result"":[{""_id"":""5d7220264295905fd35b2520"",""coin"":""btc"",""buy"":{""twd"":""325911.0622"",""status"":true},""sell"":{""status"":false},""time"":1567999415550.3032},{""_id"":""5d7220314295905fd35b2521"",""coin"":""eth"",""buy"":{""twd"":""5648.92697"",""status"":true},""sell"":{""status"":false},""time"":1567999414869.6516},{""_id"":""5d722bf34295905fd35b2522"",""coin"":""usdt"",""buy"":{""twd"":""31.2898"",""status"":true},""sell"":{""twd"":""30.6306"",""status"":true},""time"":1567999415284.331}],""status"":0}";

            var content = "";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            var i = 0;
            dynamic dynJson = JsonConvert.DeserializeObject(content);
            Application.Current.Resources["APIresponse"] = dynJson.result;
            foreach (var item in dynJson.result)
            {
                
                String coin = item.coin;
                
                //System.Windows.Controls.Grid btngrid = new Grid();
                ColumnDefinition c1 = new ColumnDefinition();
                c1.Width = new GridLength(1, GridUnitType.Auto);
                grdbtns.ColumnDefinitions.Add(c1);
                

                System.Windows.Controls.Button newBtn = new Button();

                newBtn.Content = coin.ToString().ToUpper();
                newBtn.Name = "Button" + coin.ToString();
                //newBtn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x1D, 0x5B, 0xBA)); //"#FF1D5BBA";
                //newBtn.Background = Brushes.Red; //"#FF1D5BBA";
                //newBtn.Foreground = Brushes.White;
                Thickness margin = newBtn.Margin;
                margin.Top = 50;
                margin.Left = 50;
                newBtn.Margin = margin;
                newBtn.Height = 95;
                newBtn.Width = 250;
                
                //newBtn.FontFamily = new FontFamily("Comic Sans MS");
                newBtn.FontSize = 20;
                newBtn.FontWeight = FontWeights.Bold;//
                var style = FindResource("RoundedButton") as Style;
                newBtn.Style = style;

                newBtn.Click += new RoutedEventHandler(button_Click);

                Grid.SetColumn(newBtn, i);
                i++;

                //stk_panel_cryptobuttons.Children.Add(btngrid);
                grdbtns.Children.Add(newBtn);
            }
        }

        // Date: 06/09/2019
        // selected cryptocurrency button click.
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if(clickedButton == null)
            {
                return;
            }
            Application.Current.Resources["SelectedCoin"] = clickedButton.Content;
            BuySelloption cryptoopt = new BuySelloption();
            cryptoopt.Show();
            this.Close();
        }
    }
}
