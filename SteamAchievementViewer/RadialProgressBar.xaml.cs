using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SteamAchievementViewer
{
    /// <summary>
    /// Логика взаимодействия для RadialProgressBar.xaml
    /// </summary>
    public partial class RadialProgressBar : UserControl
    {
        public static readonly DependencyProperty IndicatorBrushProperty = DependencyProperty.Register("IndicatorBrush", typeof(Brush), typeof(RadialProgressBar));
        public Brush IndicatorBrush
        {
            get { return (Brush)this.GetValue(IndicatorBrushProperty); }
            set { this.SetValue(IndicatorBrushProperty, value); }
        }
        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(RadialProgressBar));
        public Brush BackgroundBrush
        {
            get { return (Brush)this.GetValue(BackgroundBrushProperty); }
            set { this.SetValue(BackgroundBrushProperty, value); }
        }
        public static readonly DependencyProperty ProgressBorderBrushProperty = DependencyProperty.Register("ProgressBorderBrush", typeof(Brush), typeof(RadialProgressBar));
        public Brush ProgressBorderBrush
        {
            get { return (Brush)this.GetValue(ProgressBorderBrushProperty); }
            set { this.SetValue(ProgressBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(RadialProgressBar));
        public int Value
        {
            get { return (int)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }
        public RadialProgressBar()
        {
            InitializeComponent();
        }
    }
    [ValueConversion(typeof(int), typeof(double))]
    public class ValueToAngle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)(((int)value * 0.01) * 360);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)((double)value / 360) * 100;

        }
    }
}
