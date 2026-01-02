using MauiPersianToolkit.Enums;
using MauiPersianToolkit.Models;
using MauiPersianToolkit.Services.Calendar;
using MauiPersianToolkit.ViewModels;

namespace MauiPersianToolkit.Tests;

/// <summary>
/// Test cases for calendar services with different calendar types
/// </summary>
public class CalendarServiceTests
{
    private readonly ICalendarService _persianService = CalendarServiceFactory.GetService(CalendarType.Persian);
    private readonly ICalendarService _gregorianService = CalendarServiceFactory.GetService(CalendarType.Gregorian);
    private readonly ICalendarService _hijriService = CalendarServiceFactory.GetService(CalendarType.Hijri);

    /// <summary>
    /// Test date conversion roundtrip (DateTime -> String -> DateTime)
    /// </summary>
    public void TestDateConversionRoundtrip()
    {
        var testDate = new DateTime(2024, 8, 15);

        // Persian
        var persianStr = _persianService.ToCalendarDate(testDate);
        var persianConverted = _persianService.ToGregorianDate(persianStr);
        Assert.AreEqual(testDate.Date, persianConverted.Date);

        // Gregorian
        var gregorianStr = _gregorianService.ToCalendarDate(testDate);
        var gregorianConverted = _gregorianService.ToGregorianDate(gregorianStr);
        Assert.AreEqual(testDate.Date, gregorianConverted.Date);

        // Hijri
        var hijriStr = _hijriService.ToCalendarDate(testDate);
        var hijriConverted = _hijriService.ToGregorianDate(hijriStr);
        Assert.AreEqual(testDate.Date, hijriConverted.Date);
    }

    /// <summary>
    /// Test month boundaries
    /// </summary>
    public void TestMonthBoundaries()
    {
        var testDate = new DateTime(2024, 8, 15);

        // Persian
        var persianStart = _persianService.GetMonthBeginning(testDate);
        var persianEnd = _persianService.GetMonthEnding(testDate);
        Assert.IsNotNull(persianStart);
        Assert.IsNotNull(persianEnd);

        // Gregorian
        var gregorianStart = _gregorianService.GetMonthBeginning(testDate);
        var gregorianEnd = _gregorianService.GetMonthEnding(testDate);
        Assert.IsNotNull(gregorianStart);
        Assert.IsNotNull(gregorianEnd);

        // Hijri
        var hijriStart = _hijriService.GetMonthBeginning(testDate);
        var hijriEnd = _hijriService.GetMonthEnding(testDate);
        Assert.IsNotNull(hijriStart);
        Assert.IsNotNull(hijriEnd);
    }

    /// <summary>
    /// Test holiday detection
    /// </summary>
    public void TestHolidayDetection()
    {
        // Persian calendar: Friday
        var persianHoliday = _persianService.GetLastDayOfWeek();
        Assert.AreEqual(DayOfWeek.Friday, persianHoliday);

        // Gregorian calendar: Sunday
        var gregorianHoliday = _gregorianService.GetLastDayOfWeek();
        Assert.AreEqual(DayOfWeek.Sunday, gregorianHoliday);

        // Hijri calendar: Friday
        var hijriHoliday = _hijriService.GetLastDayOfWeek();
        Assert.AreEqual(DayOfWeek.Friday, hijriHoliday);
    }

    /// <summary>
    /// Test month names
    /// </summary>
    public void TestMonthNames()
    {
        // Persian
        var persianMonths = _persianService.GetAllMonthNames();
        Assert.AreEqual(12, persianMonths.Count());

        // Gregorian
        var gregorianMonths = _gregorianService.GetAllMonthNames();
        Assert.AreEqual(12, gregorianMonths.Count());

        // Hijri
        var hijriMonths = _hijriService.GetAllMonthNames();
        Assert.AreEqual(12, hijriMonths.Count());
    }

    /// <summary>
    /// Test leap year
    /// </summary>
    public void TestLeapYear()
    {
        // Persian calendar leap years
        var persianLeap = _persianService.IsLeapYear(1403);
        
        // Gregorian calendar leap year
        var gregorianLeap = _gregorianService.IsLeapYear(2024);
        Assert.IsTrue(gregorianLeap);

        // Hijri calendar leap year
        var hijriLeap = _hijriService.IsLeapYear(1446);
    }

    /// <summary>
    /// Test DatePickerViewModel with different calendar types
    /// </summary>
    public void TestDatePickerViewModelWithDifferentCalendars()
    {
        // Persian DatePicker
        var persianOptions = new CalendarOptions 
        { 
            CalendarType = CalendarType.Persian 
        };
        var persianViewModel = new DatePickerViewModel(persianOptions);
        Assert.IsNotNull(persianViewModel.CurrentMonth);
        Assert.IsGreater(persianViewModel.CurrentYear, 1000);

        // Gregorian DatePicker
        var gregorianOptions = new CalendarOptions 
        { 
            CalendarType = CalendarType.Gregorian 
        };
        var gregorianViewModel = new DatePickerViewModel(gregorianOptions);
        Assert.IsNotNull(gregorianViewModel.CurrentMonth);
        Assert.IsGreater(gregorianViewModel.CurrentYear, 2000);

        // Hijri DatePicker
        var hijriOptions = new CalendarOptions 
        { 
            CalendarType = CalendarType.Hijri 
        };
        var hijriViewModel = new DatePickerViewModel(hijriOptions);
        Assert.IsNotNull(hijriViewModel.CurrentMonth);
        Assert.IsGreater(hijriViewModel.CurrentYear, 1000);
    }

    /// <summary>
    /// Test date formatting with different calendars
    /// </summary>
    public void TestDateFormatting()
    {
        var testDate = new DateTime(2024, 8, 15);

        // Persian
        var persianDate = _persianService.ToCalendarDate(testDate);
        Assert.IsTrue(persianDate.Contains("/"));
        Assert.AreEqual(3, persianDate.Split('/').Length);

        // Gregorian
        var gregorianDate = _gregorianService.ToCalendarDate(testDate);
        Assert.IsTrue(gregorianDate.Contains("/"));
        Assert.AreEqual(3, gregorianDate.Split('/').Length);

        // Hijri
        var hijriDate = _hijriService.ToCalendarDate(testDate);
        Assert.IsTrue(hijriDate.Contains("/"));
        Assert.AreEqual(3, hijriDate.Split('/').Length);
    }
}

// Simple Assert helper (replace with your test framework)
internal static class Assert
{
    public static void AreEqual<T>(T expected, T actual)
    {
        if (!Equals(expected, actual))
            throw new Exception($"Expected {expected}, but got {actual}");
    }

    public static void IsNotNull(object obj)
    {
        if (obj == null)
            throw new Exception("Expected non-null object");
    }

    public static void IsTrue(bool condition)
    {
        if (!condition)
            throw new Exception("Expected true condition");
    }

    public static void IsGreater(int actual, int expectedMin)
    {
        if (actual <= expectedMin)
            throw new Exception($"Expected {actual} > {expectedMin}");
    }
}
