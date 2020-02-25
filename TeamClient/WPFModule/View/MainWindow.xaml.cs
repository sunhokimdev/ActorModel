using System.Windows;

namespace TeamClient
{
    public partial class MainWindow : Window
    {       
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainApplication._mainView;            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            MainApplication._mainView.WindowClosing();            
        }
    }
}
