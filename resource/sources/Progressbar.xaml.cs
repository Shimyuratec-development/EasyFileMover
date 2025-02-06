using System.Windows;


namespace EasyFileMover
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Progressbar : Window
    {
        public event EventHandler<bool>? ResutlReceived;
        public Progressbar(MainViewModel v)
        {
            InitializeComponent();
            DataContext = v;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResutlReceived?.Invoke(this, true);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ResutlReceived?.Invoke(this, false);
            this.Close();
        }
    }
}
