using SteamAchievementViewer.ViewModels;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage(AuthPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}