using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace EHealthApp.Converters
{
    public class CategoryBgConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentCategoryName = (value as EHealthApp.MedicalDocumentsPage.CategoryDTO)?.Name;
            var selectedCategory = (parameter as EHealthApp.MedicalDocumentsPage.CategoryDTO)?.Name;

            // Dacă e selectată => verde închis, altfel verde pal
            return (currentCategoryName == selectedCategory)
                ? Color.FromArgb("#388E3C")
                : Color.FromArgb("#E8F5E9");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
