
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;


namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для LastAchievedPage.xaml
    /// </summary>
    public partial class LastAchievedPage : Page
    {
        public LastAchievedPage()
        {
            InitializeComponent();
            
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if(Manager.IsLogged())
                AchievementTable.ItemsSource = Manager.GetLatestAchievements(100);
        }
    }
}
 