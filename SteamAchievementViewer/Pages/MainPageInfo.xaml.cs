using SteamAchievementViewer.Services;
using SteamAchievementViewer.ViewModels;
using System.Windows.Controls;

namespace SteamAchievementViewer.Pages
{

    public partial class MainPageInfo : Page
    {
        public MainPageInfo(MainInfoViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
