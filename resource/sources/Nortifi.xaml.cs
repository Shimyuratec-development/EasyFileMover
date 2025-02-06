using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace EasyFileMover
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class NortifiWindow : Window
    {
        public event EventHandler<bool>? ResutlReceived;
        public string Message
        {
            get { return NortifiTextBox.Text; }
            set { NortifiTextBox.Text = value; }
        }

        public NortifiWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
         
            ResutlReceived?.Invoke(this, false);
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {

            ResutlReceived?.Invoke(this, false);

            base.OnClosing(e);
        }

    }
}
