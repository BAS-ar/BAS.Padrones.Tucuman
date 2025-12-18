using System;
using System.Globalization;
using System.Threading;

class Program
{
    public static void Main()
    {
        CultureInfo current = CultureInfo.CurrentCulture;
        CultureInfo ui = CultureInfo.CurrentUICulture;

        WriteLine("=== Culture Overview ===");
        PrintCulture("CurrentCulture", current);
        PrintCulture("CurrentUICulture", ui);

        WriteLine();
        WriteLine("=== Number & Currency Samples ===");
        ShowSamples(current);

        WriteLine();
        WriteLine("=== Invariant Culture (Reference) ===");
        PrintCulture("InvariantCulture", CultureInfo.InvariantCulture);
        ShowSamples(CultureInfo.InvariantCulture);
    }

    static void PrintCulture(string label, CultureInfo culture)
    {
        NumberFormatInfo nfi = culture.NumberFormat;
        DateTimeFormatInfo dfi = culture.DateTimeFormat;

        WriteLine($"[{label}]");
        WriteLine($"Name                : {culture.Name}");
        WriteLine($"Display Name        : {culture.DisplayName}");
        WriteLine($"English Name        : {culture.EnglishName}");
        WriteLine($"Is Neutral Culture  : {culture.IsNeutralCulture}");
        WriteLine();

        WriteLine("Number Formatting:");
        WriteLine($"  Decimal Separator : '{nfi.NumberDecimalSeparator}'");
        WriteLine($"  Group Separator   : '{nfi.NumberGroupSeparator}'");
        WriteLine($"  Decimal Digits    : {nfi.NumberDecimalDigits}");
        WriteLine();

        WriteLine("Currency Formatting:");
        WriteLine($"  Currency Symbol   : '{nfi.CurrencySymbol}'");
        WriteLine($"  Decimal Separator : '{nfi.CurrencyDecimalSeparator}'");
        WriteLine($"  Group Separator   : '{nfi.CurrencyGroupSeparator}'");
        WriteLine($"  Decimal Digits    : {nfi.CurrencyDecimalDigits}");
        WriteLine($"  Positive Pattern  : {nfi.CurrencyPositivePattern}");
        WriteLine($"  Negative Pattern  : {nfi.CurrencyNegativePattern}");
        WriteLine();

        WriteLine("Date Formatting:");
        WriteLine($"  Short Date        : {dfi.ShortDatePattern}");
        WriteLine($"  Long Date         : {dfi.LongDatePattern}");
        WriteLine($"  Date Separator    : '{dfi.DateSeparator}'");
        WriteLine();
    }

    static void ShowSamples(CultureInfo culture)
    {
        double number = 1234567.89;
        decimal money = 1234567.89m;

        WriteLine($"Culture: {culture.Name}");
        WriteLine($"  Number (N)   : {number.ToString("N", culture)}");
        WriteLine($"  Number (F)   : {number.ToString("F", culture)}");
        WriteLine($"  Currency (C): {money.ToString("C", culture)}");
    }

    static void WriteLine()
    {
        WriteLine("");
    }

    static void WriteLine(string text)
    {
        using(var sr = new StreamWriter("CultureInfo.txt", true))
        {
            sr.WriteLine(text);
        }

        Console.WriteLine(text);
    }
}