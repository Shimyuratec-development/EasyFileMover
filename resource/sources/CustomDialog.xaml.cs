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
    /// CustomDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class CustomDialog : Window
    {
        //public event EventHandler<bool>? ResutlReceived;
        public string Message
        {
            get { return MessageTextBlock.Text; }
            set { MessageTextBlock.Text = value; }
        }
        public bool Result
        {
            get;private set;
        }
        public event Action<bool>? ResultSubmitted;

        private bool _resultSubmitted = false;

        public CustomDialog()
        {
            InitializeComponent();
        }

        

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;

            Result = true;
            _resultSubmitted = true;
            ResultSubmitted?.Invoke(Result);

            //ResutlReceived?.Invoke(this, true);
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            _resultSubmitted = true;
            ResultSubmitted?.Invoke(Result);
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_resultSubmitted)
            {
                Result = false;
                _resultSubmitted = true;
                ResultSubmitted?.Invoke(Result);

            }
            base.OnClosing(e);
        }
    }
}
