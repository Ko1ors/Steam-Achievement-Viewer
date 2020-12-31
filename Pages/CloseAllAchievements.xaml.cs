using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamAchievementViewer.Pages
{
    /// <summary>
    /// Логика взаимодействия для CloseAllAchievements.xaml
    /// </summary>
    public partial class CloseAllAchievements : Page
    {
        public CloseAllAchievements()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Manager.IsLogged())
                AchievementTable.ItemsSource = Manager.GetClosestAchievements(100);
        }
    }
}
