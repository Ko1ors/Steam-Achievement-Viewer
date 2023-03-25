using SteamAchievementViewer.ViewModels;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Interaction logic for EasiestGamesToCompletePage.xaml
    /// </summary>
    public partial class EasiestGamesToCompletePage : Page
    {
        public EasiestGamesToCompletePage(EasiestGamesToCompleteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
