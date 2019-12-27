using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ATM
{
    class Constants
    {
        public const string CASH_DISPENSE_MESSAGE = "Please wait cash is getting dispensing...";
        public const string CONFIRM_EXCHANGED_RATE = "The price has been changed. Please confirm the exchanged rate again.";
        public const string COIN_LIST_API = "http://192.168.51.203:2222/casigo/coinList";
        public const string SALE_RESULT_API = "http://192.168.51.203:2222/casigo/saleResult";
        public const string SELECTED_SIDE_BUY = "buy";
        public const string NO_CASH = "Please insert cash.";
        public const string PORT_NAME = "COM6";
        public const Int32 BAUDRATE = 9600;
        public const Parity PARITY = Parity.Even;
        public const StopBits STOPBITS = StopBits.One;
        public const Int32 DATABITS = 8;
        public const Handshake HANDSHAKE = Handshake.None;
        public const Int32 READTIMEOUT = 2000;
        public const Int32 WRITETIMEOUT = 500;
        public const string USDT_COIN = "USDT";
        public const string BTC_COIN = "BTC";
        public const string ETH_COIN = "ETH";
    }
}
