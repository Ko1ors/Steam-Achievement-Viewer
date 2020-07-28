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
