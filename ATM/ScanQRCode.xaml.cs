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
using WebCam_Capture;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace ATM
{
    /// <summary>
    /// Interaction logic for ScanQRCode.xaml
    /// </summary>
    public partial class ScanQRCode : Window
    {
        public ScanQRCode()
        {
            InitializeComponent();
            GetCurrentPrice();
        }

        // Date: 17/09/2019
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

        WebCam webcam;

        // Date: 11/09/2019
        // when window load first, initialize webcam.
        private void mainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
            webcam.Start();
        }

        private void bntStart_Click(object sender, RoutedEventArgs e)
        {
            webcam.Start();
        }

        private void bntStop_Click(object sender, RoutedEventArgs e)
        {
            webcam.Stop();
        }

        private void bntContinue_Click(object sender, RoutedEventArgs e)
        {
            webcam.Continue();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            webcam.Stop();
            //ChooseCashLimit cryptoopt = new ChooseCashLimit();
            BuySelloption cryptoopt = new BuySelloption();
            cryptoopt.Show();
            this.Close();
        }
    }

    class WebCam 
     {
        private WebCamCapture webcam;
        private System.Windows.Controls.Image _FrameImage;
        private int FrameNumber = 30;
        public void InitializeWebCam(ref System.Windows.Controls.Image FingerImage)
        {
            webcam = new WebCamCapture();
            webcam.FrameNumber = ((ulong)(0ul));
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.ImageCaptured += new WebCamCapture.WebCamEventHandler(webcam_ImageCaptured);
            _FrameImage = FingerImage;
        }

        void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            _FrameImage.Source = Helper.LoadBitmap((System.Drawing.Bitmap)e.WebCamImage);

            try
            {
                Bitmap bitmap = new Bitmap(e.WebCamImage);
                BarcodeReader reader = new BarcodeReader { AutoRotate = true, TryInverted = true };
                Result result = reader.Decode(bitmap);
                if (result != null)
                {
                    bool IsValid = false;
                    string decoded = result.ToString().Trim();
                    if (decoded.Contains(":"))
                    {
                        string stringBeforeChar = decoded.Substring(0, decoded.IndexOf("?"));
                        string stringAfterChar = stringBeforeChar.Substring(stringBeforeChar.IndexOf(":") + 1);
                        string address = stringAfterChar.Trim();
                        Application.Current.Resources["Buy_ScannedQR"] = address.Trim();
                        IsValid =  ValidateAddress(address);
                    }
                    else
                    {
                        Application.Current.Resources["Buy_ScannedQR"] = decoded.Trim();
                        IsValid = ValidateAddress(decoded);
                    }

                    if(IsValid == true) {
                        webcam.Stop();
                        foreach (Window w in Application.Current.Windows)
                        {
                            if (w.Title == "ScanQRCode")
                            {
                                InsertCashConfirm insertcash = new InsertCashConfirm();
                                w.Close();
                                insertcash.Show();
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid wallet address.");
                    }
                    
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("No QRCode");
                Console.WriteLine("No QRCode");
            }
        }

        public void Start()
        {
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.Start(0);
        }

        public void Stop()
        {
            webcam.Stop();
        }

        public void Continue()
        {
            // change the capture time frame
            webcam.TimeToCapture_milliseconds = FrameNumber;

            // resume the video capture from the stop
            webcam.Start(this.webcam.FrameNumber);
        }

        public void ResolutionSetting()
        {
            webcam.Config();
        }

        public void AdvanceSetting()
        {
            webcam.Config2();
        }

        public bool ValidateAddress(String address)
        {
            try
            {
                Boolean IsValid = false;
                String addresstovalidate = address.Trim();
                dynamic selectedcyptocurrency = Application.Current.Resources["SelectedCoin"];
                if((selectedcyptocurrency == Constants.ETH_COIN) || (selectedcyptocurrency == Constants.USDT_COIN))
                {
                    IsValid = ValidateEthereumAddressTest(addresstovalidate);
                    return IsValid;
                }
                else if(selectedcyptocurrency == Constants.BTC_COIN)
                {
                    IsValid = ValidateBitcoinAddress(addresstovalidate);
                    return IsValid;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ValidateEthereumAddressTest(string address)
        {
            Regex rx = new Regex(@"^0x[a-fA-F0-9]{40}$");
            if (rx.IsMatch(address))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateBitcoinAddress(string address)
        {
            if (address.Length < 26 || address.Length > 35) { return false; } //throw new Exception("wrong length");
            else
            {
                var decoded = DecodeBase58(address);
                var d1 = Hash(decoded.SubArray(0, 21));
                var d2 = Hash(d1);
                if (!decoded.SubArray(21, 4).SequenceEqual(d2.SubArray(0, 4))) { return false; } //throw new Exception("bad digest");
                else { return true; }
            }
        }

        const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        const int Size = 25;
        private static byte[] DecodeBase58(string input)
        {
            var output = new byte[Size];
            foreach (var t in input)
            {
                var p = Alphabet.IndexOf(t);
                if (p == -1) { break; } //throw new Exception("invalid character found");
                var j = Size;
                while (--j > 0)
                {
                    p += 58 * output[j];
                    output[j] = (byte)(p % 256);
                    p /= 256;
                }
                if (p != 0) { break; } //throw new Exception("address too long");

            }
            return output;
        }
        private static byte[] Hash(byte[] bytes)
        {
            var hasher = new SHA256Managed();
            return hasher.ComputeHash(bytes);
        }
    }

    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    class Helper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);
        public static BitmapSource bs;
        public static IntPtr ip;
        public static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {

            ip = source.GetHbitmap();

            bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,

                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ip);

            return bs;
        }
        public static void SaveImageCapture(BitmapSource bitmap)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.QualityLevel = 100;

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Image"; // Default file name
            dlg.DefaultExt = ".Jpg"; // Default file extension
            dlg.Filter = "Image (.jpg)|*.jpg"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                FileStream fstream = new FileStream(filename, FileMode.Create);
                encoder.Save(fstream);
                fstream.Close();
            }
        }
    }
}
