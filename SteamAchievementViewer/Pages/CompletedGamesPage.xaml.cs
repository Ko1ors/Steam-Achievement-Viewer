using SteamAchievementViewer.ViewModels;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Interaction logic for CompletedGamesPage.xaml
    /// </summary>
    public partial class CompletedGamesPage : Page
    {
        public CompletedGamesPage(CompletedGamesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
