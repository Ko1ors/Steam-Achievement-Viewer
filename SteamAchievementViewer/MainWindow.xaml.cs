using SteamAchievementViewer.Services;
using SteamAchievementViewer.ViewModels;
using System.Windows;

namespace SteamAchievementViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(MainWindowViewModel viewModel, ISteamService steamService)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
