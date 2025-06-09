using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace EHealthApp.Converters
{
    public class CategoryTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentCategoryName = (value as EHealthApp.MedicalDocumentsPage.CategoryDTO)?.Name;
            var selectedCategory = (parameter as EHealthApp.MedicalDocumentsPage.CategoryDTO)?.Name;

            // Dacă e selectată => text alb, altfel text verde închis
            return (currentCategoryName == selectedCategory)
                ? Colors.White
                : Color.FromArgb("#1B5E20");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
