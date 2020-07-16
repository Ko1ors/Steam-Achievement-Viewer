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
        private static Dictionary<string, Page> pagesDictionary = new Dictionary<string, Page>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public static Page GetPageObject(string type)
        {
            if (!pagesDictionary.TryGetValue(type, out Page page))
            {
                switch (type)
                {
                    case "MainPage":
                        page = new MainPage();
                        break;
                    case "LastAchievedPage":
                        page = new LastAchievedPage();
                        break;
                    default:
                        new Exception("Page type does not exist");
                        break;
                }
                pagesDictionary.Add(type, page);
            }
            return page;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = GetPageObject("MainPage");
        }
        private void LastAchievedButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = GetPageObject("LastAchievedPage");
        }
    }
}
