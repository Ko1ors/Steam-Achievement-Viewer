﻿
using System.Collections.ObjectModel;
using System.Windows.Controls;


namespace AchievementTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для LastAchievedPage.xaml
    /// </summary>
    public partial class LastAchievedPage : Page
    {
        ObservableCollection<Achievement> Achievements = new ObservableCollection<Achievement>();
        public LastAchievedPage()
        {
            InitializeComponent();
        }
    }
}
