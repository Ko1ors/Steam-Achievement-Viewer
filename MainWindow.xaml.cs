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
        private static Dictionary<Type, Page> pagesDictionary = new Dictionary<Type, Page>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public static Page GetPageObject<T>() where T : Page, new()
        {
            if (!pagesDictionary.TryGetValue(typeof(T), out Page page))
            {
                page = new T();
                pagesDictionary.Add(typeof(T), page);
            }
            return page;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = GetPageObject<MainPage>();
        }
        private void LastAchievedButton_clicked(object sender, RoutedEventArgs e)
        {
            Information.Content = GetPageObject<LastAchievedPage>();
        }
    }
}
