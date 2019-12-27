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
    /// Interaction logic for TransctionConfirmation.xaml
    /// </summary>
    public partial class TransctionConfirmation : Window
    {
        public TransctionConfirmation()
        {
            InitializeComponent();
            getDestinationAddress();
        }

        public static dynamic notecount;
        public static dynamic exchangedrate;
        public static dynamic Publiccoin;
        private void getDestinationAddress()
        {
            notecount = Application.Current.Resources["NoteCount"];
            exchangedrate = Application.Current.Resources["Exchangedrate"];
            Publiccoin = Application.Current.Resources["SelectedCoin"];
            lblexchangeamount.Text = exchangedrate + " " + Publiccoin;
            int notes = Convert.ToInt32(notecount) * 1000;
            lblinsertedamount.Text = notes.ToString() + " " + "NTD";
        }
    }
}
