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
using System.IO.Ports;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ATM
{
    /// <summary>
    /// Interaction logic for InsertCash.xaml
    /// </summary>
    public partial class InsertCash : Window
    {
        public InsertCash()
        {
            InitializeComponent();
            getDestinationAddress();
            getCalculation();
        }

        //private SerialPort mySerialPort = new SerialPort("COM6", 9600, Parity.Even, 8, StopBits.One);
        Boolean istrue = false;
        public static dynamic Publicaddress;
        public static dynamic Publiccoin;
        public static dynamic Publicside;
        public static dynamic notecount;
        public static dynamic apiresponse;

        // Date: 25/11/2019
        // Get all stored session values.
        private void getDestinationAddress()
        {
            Publicaddress = Application.Current.Resources["Buy_ScannedQR"];
            txt_scannedaddress.Text = Publicaddress.ToString();
            Publiccoin = Application.Current.Resources["SelectedCoin"];
            Publicside = Application.Current.Resources["SelectedSide"];
            notecount = Application.Current.Resources["NoteCount"];
            apiresponse = Application.Current.Resources["APIresponse"];
        }

        // Date: 25/11/2019
        // Calculate count of notes into NTD and selected coin.
        private void getCalculation()
        {
            try
            {
                GetAPIResponseCoinlist();
                Decimal Price = 0;
                int notes = Convert.ToInt32(notecount) * 1000;
                txt_insertedcash.Text = notes.ToString() + " " + "NTD";
                foreach (var item in apiresponse)
                {
                    String coin = item.coin;
                    if (Publiccoin == coin.ToUpper())
                    {
                        Price = item.buy["twd"];
                        break;
                    }
                }
                Decimal btc = 1 / Price;
                Decimal getBTC = btc * notes;
                getBTC = Math.Round(getBTC, 4);
                txt_exchangedrate.Text = getBTC.ToString() + " " + Publiccoin;
                Application.Current.Resources["Exchangedrate"] = getBTC;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DispenseCash d = new DispenseCash();
            d.Show();
            this.Close();
        }

        

        private bool GetAPIResponse(string coin, string side, Decimal price, string address, int cash)
        {
            try
            {
                string url = Constants.SALE_RESULT_API;
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                //string postData = "coin=usdt&side=buy&price=1000&address=0x123";
                string postData = "coin=" + coin + "&side=" + side + "&price=" + price + "&address=" + address + "&cash=" +cash + "";
                var data = Encoding.ASCII.GetBytes(postData);
                request.ContentLength = data.Length;

                var content = string.Empty;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    dynamic dynJson = JsonConvert.DeserializeObject(responseString);
                    var a = dynJson.result;
                    return a;
                }
                //var response = (HttpWebResponse)request.GetResponse();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); return false;
            }
        }

        

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cash = notecount * 1000;
                var coin = Publiccoin.ToLower();
                var side = Publicside;
                var scannedaddress = Publicaddress;
                Decimal price = 0;
                foreach (var item in apiresponse)
                {
                    String apicoin = item.coin;
                    if (Publiccoin == apicoin.ToUpper())
                    {
                        price = item.buy["twd"];
                        break;
                    }
                }
                istrue = GetAPIResponse(coin, side, price, scannedaddress, cash);
                if (istrue == false)
                {
                    MessageBox.Show(Constants.CONFIRM_EXCHANGED_RATE);
                    getCalculation();
                }
                else if (istrue == true)
                {
                    TransctionConfirmation tr = new TransctionConfirmation();
                    tr.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void GetAPIResponseCoinlist()
        {
            try
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
                dynamic dynJson = JsonConvert.DeserializeObject(content);
                apiresponse = dynJson.result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

