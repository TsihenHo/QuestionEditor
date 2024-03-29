﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace QuestionEditor.Pages.Converters;

public class Enum2BoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        => value?.Equals(parameter);

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        => value?.Equals(true) == true ? parameter : Binding.DoNothing;
}