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
        public RareAchievements()
        {
            InitializeComponent();
            AchievementTable.ItemsSource = Manager.GetRarestAchievements(100);
        }
    }
}
