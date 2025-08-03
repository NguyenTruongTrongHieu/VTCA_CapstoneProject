using System.Globalization;
using UnityEngine;

public static class NumberFomatter
{
    public static float RoundFloatToTwoDecimalPlaces(float number)
    {
        // Làm tròn số về 2 chữ số thập phân
        return Mathf.Round(number * 100f) / 100f;
    }

    public static string FormatFloatToString(float number, int decimalPlaces)
    {
        // Format string theo số chữ số mong muốn khi hiển thị
        string format = "0." + new string('0', decimalPlaces);

        // Nếu số nhỏ hơn 1000
        if (number < 1000f)
        {
            if (Mathf.Approximately(number, Mathf.Round(number)))
                return number.ToString("0");
            else
                return number.ToString(format, CultureInfo.InvariantCulture);
        }

        // Nếu số lớn hơn hoặc bằng 1000 → hiển thị K, M, B
        if (number >= 1_000_000_000f)
            return (number / 1_000_000_000f).ToString(format, CultureInfo.InvariantCulture) + "B";
        else if (number >= 1_000_000f)
            return (number / 1_000_000f).ToString(format, CultureInfo.InvariantCulture) + "M";
        else // >= 1_000
            return (number / 1_000f).ToString(format, CultureInfo.InvariantCulture) + "K";
    }

    public static string FormatIntToString(int number, int decimalPlaces)
    {
        // Format string theo số chữ số mong muốn khi hiển thị
        string format = "0." + new string('0', decimalPlaces);


        if (number >= 1_000_000_000)
            return (number / 1_000_000_000f).ToString(format, CultureInfo.InvariantCulture) + "B";
        else if (number >= 1_000_000)
            return (number / 1_000_000f).ToString(format, CultureInfo.InvariantCulture) + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString(format, CultureInfo.InvariantCulture) + "K";
        else
            return number.ToString("0");
    }
}
