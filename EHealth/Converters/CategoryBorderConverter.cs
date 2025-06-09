using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace EHealthApp.Converters
{
    public class CategoryBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentCategoryName = (value as EHealthApp.MedicalDocumentsPage.CategoryDTO)?.Name;
            var selectedCategory = (parameter as EHealthApp.MedicalDocumentsPage.CategoryDTO)?.Name;

            // Dacă e selectată => border verde închis, altfel border verde principal
            return (currentCategoryName == selectedCategory)
                ? Color.FromArgb("#1B5E20")
                : Color.FromArgb("#388E3C");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
