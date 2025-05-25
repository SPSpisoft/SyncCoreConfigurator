using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SyncCore.Helpers
{
    /// <summary>
    /// Converts boolean to brush
    /// </summary>
    /// <remarks>
    /// تبدیل بولین به براش
    /// </remarks>
    public class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string colors)
                {
                    var colorArray = colors.Split(';');
                    if (colorArray.Length == 2)
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(boolValue ? colorArray[0] : colorArray[1]));
                    }
                }
                return new SolidColorBrush(boolValue ? Colors.Blue : Colors.Gray);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts boolean to visibility with inverse logic
    /// </summary>
    /// <remarks>
    /// تبدیل بولین به وضعیت نمایش با منطق معکوس
    /// </remarks>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    /// <summary>
    /// Inverts boolean value
    /// </summary>
    /// <remarks>
    /// معکوس کردن مقدار بولین
    /// </remarks>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }
    }

    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility != Visibility.Visible;
        }
    }

    public class StepIndexToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stepIndex)
            {
                return stepIndex == 3 ? "Finish" : "Next";
            }
            return "Next";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 