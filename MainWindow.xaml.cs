using AchievementTest.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace AchievementTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            Manager.Start();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AuthorizationButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<MainPage>();
        }
        private void LastAchievedButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<LastAchievedPage>();
        }

        private void CloseAchievementButton_clicked(object sender, RoutedEventArgs e)
        {
            
            Information.Content = Manager.GetPageObject<CloseAchievements>();
        }

        private void RareAchievementButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<RareAchievements>();
        }

        private void MainInfoButton_cliked(object sender, RoutedEventArgs e)
        {
            Information.Content = Manager.GetPageObject<MainPageInfo>();
        }
    }
}
