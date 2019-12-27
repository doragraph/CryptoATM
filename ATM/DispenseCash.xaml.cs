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

namespace ATM
{
    /// <summary>
    /// Interaction logic for DispenseCash.xaml
    /// </summary>
    public partial class DispenseCash : Window
    {
        public DispenseCash()
        {
            InitializeComponent();
            SettingRS232();
        }

        private SerialPort mySerialPort = new SerialPort();
        public void SettingRS232()
        {
            try
            {
                //mySerialPort.BaudRate = 9600;
                //mySerialPort.Parity = Parity.Even;
                //mySerialPort.StopBits = StopBits.One;
                //mySerialPort.DataBits = 8;
                //mySerialPort.Handshake = Handshake.None;
                //mySerialPort.ReadTimeout = 2000;
                //mySerialPort.WriteTimeout = 500;

                mySerialPort.PortName = Constants.PORT_NAME;
                mySerialPort.BaudRate = Constants.BAUDRATE;
                mySerialPort.Parity = Constants.PARITY;
                mySerialPort.StopBits = Constants.STOPBITS;
                mySerialPort.DataBits = Constants.DATABITS;
                mySerialPort.Handshake = Constants.HANDSHAKE;
                mySerialPort.ReadTimeout = Constants.READTIMEOUT;
                mySerialPort.WriteTimeout = Constants.WRITETIMEOUT;

                mySerialPort.DtrEnable = true;
                mySerialPort.RtsEnable = true;

                var check = mySerialPort.IsOpen;
                if (check == true)
                {
                    mySerialPort.Close();
                }

                mySerialPort.Open();

                Get_RecyclerBox_Count();

                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ScanQRCode sr = new ScanQRCode();
                sr.Show();
                this.Close();
            }
        }


        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                System.Threading.Thread.Sleep(1000);
                //string indata = sp.ReadExisting();

                int nb = sp.BytesToRead;
                if (nb > 0)
                {
                    byte[] buffer = new byte[nb];
                    sp.Read(buffer, 0, nb);

                    var bytestostring = BitConverter.ToString(buffer, 0);

                    MessageBox.Show(bytestostring);


                    if (bytestostring.Contains("01-01-03-00-00"))
                    {
                        string toBeSearched = "01-01-03-00-00";
                        int ix = bytestostring.IndexOf(toBeSearched);

                        String[] splitedresponse = new string[] { };

                        if (ix != -1)
                        {
                            string code = bytestostring.Substring(ix + toBeSearched.Length);
                            splitedresponse = code.Split('-');
                        }

                        string count = splitedresponse[1].Trim();

                        //string response = bytestostring.Trim();
                        //String[] splitedresponse = response.Split('-');
                        //string count = splitedresponse[5].Trim();


                        int notecount = Convert.ToInt32(count);
                        Application.Current.Resources["NoteCount"] = notecount;
                        Dispense_Cash(notecount);

                        System.Threading.Thread.Sleep(500);
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblmsg.Text = Constants.CASH_DISPENSE_MESSAGE;
                        }));

                        System.Threading.Thread.Sleep(500);
                        Get_Dispense_Monitor();

                    }
                    if (bytestostring.Contains("01-07-03-00-00"))
                    {
                        string toBeSearched = "01-07-03-00-00";
                        int ix = bytestostring.IndexOf(toBeSearched);

                        if (ix != -1)
                        {
                            string code = bytestostring.Substring(ix + toBeSearched.Length);
                            String[] splitedresponse = code.Split('-');
                            string count = splitedresponse[2].Trim();
                            if (count == "11")
                            {
                                Get_Dispense_Monitor();
                            }
                            else if (count == "22")
                            {
                                ReSet_BillTypeParameter();
                                mySerialPort.Close();
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    //Resources.MergedDictionaries.Clear();
                                    CryptocurrencyOption c = new CryptocurrencyOption();
                                    c.Show();
                                    this.Close();
                                }));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Date: 26/11/2019
        // Dispense cash from recycler box.
        private void Dispense_Cash(int notecount)
        {
            try
            {
                //get no. of notes to be dispense.
                string count = "0" + notecount.ToString();
                byte[] forchecksum = new byte[6];
                forchecksum[0] = 0x03;
                forchecksum[1] = 0x01;
                forchecksum[2] = 0x01;
                forchecksum[3] = 0x00;
                forchecksum[4] = 0x1C;
                forchecksum[5] = Convert.ToByte(count);

                byte getbyte = CalculateChecksum(forchecksum, forchecksum.Length);

                string gethex = getbyte.ToString("X2");

                //dispense cash.
                byte[] sendbuffer1 = new byte[7];
                sendbuffer1[0] = 0x03;
                sendbuffer1[1] = 0x01;
                sendbuffer1[2] = 0x01;
                sendbuffer1[3] = 0x00;
                sendbuffer1[4] = 0x1C;
                sendbuffer1[5] = Convert.ToByte(count);
                sendbuffer1[6] = Convert.ToByte(gethex, 16);

                mySerialPort.Write(sendbuffer1, 0, 7);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Date: 26/11/2019
        // calculate checksum using CheckSum8 Xor.
        private byte CalculateChecksum(byte[] _PacketData, int PacketLength)
        {
            Byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketLength; i++)
                _CheckSumByte ^= _PacketData[i];
            return _CheckSumByte;
        }

        // Date: 28/11/2019
        // Get number of recycling bill count means how many notes are in recycler box.
        private void Get_Dispense_Monitor()
        {
            try
            {
                byte[] sendbuffer1 = new byte[6];
                sendbuffer1[0] = 0x03;
                sendbuffer1[1] = 0x00;
                sendbuffer1[2] = 0x01;
                sendbuffer1[3] = 0x00;
                sendbuffer1[4] = 0x1D;
                sendbuffer1[5] = 0x1F;

                mySerialPort.Write(sendbuffer1, 0, 6);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Date: 28/11/2019
        // Set Bill type Parameter to nothing. //it should not accept note.
        private void ReSet_BillTypeParameter()
        {
            try
            {
                byte[] sendbuffer = new byte[8];
                sendbuffer[0] = 0x03;
                sendbuffer[1] = 0x02;
                sendbuffer[2] = 0x01;
                sendbuffer[3] = 0x00;
                sendbuffer[4] = 0x15;
                sendbuffer[5] = 0x00;
                sendbuffer[6] = 0x00;
                sendbuffer[7] = 0x15;

                mySerialPort.Write(sendbuffer, 0, 8);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        // Date: 26/11/2019
        // Get number of recycling bill count means how many notes are in recycler box.
        private void Get_RecyclerBox_Count()
        {
            try
            {
                byte[] sendbuffer1 = new byte[6];
                sendbuffer1[0] = 0x03;
                sendbuffer1[1] = 0x00;
                sendbuffer1[2] = 0x01;
                sendbuffer1[3] = 0x00;
                sendbuffer1[4] = 0x1B;
                sendbuffer1[5] = 0x19;

                mySerialPort.Write(sendbuffer1, 0, 6);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
