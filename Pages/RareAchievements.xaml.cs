using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для RareAchievements.xaml
    /// </summary>
    public partial class RareAchievements : Page
    {
        ObservableCollection<Achievement> Achievemetns = new ObservableCollection<Achievement>();
        public RareAchievements()
        {
            InitializeComponent();
            foreach(Game g in Manager.gamesList.Games.Game)
            {
                foreach (Achievement a in g.Achievements.Achievement)
                {
                    Achievemetns.Add(a);
                }
            }
            AchievementTable.ItemsSource = Achievemetns;
        }
    }
}
