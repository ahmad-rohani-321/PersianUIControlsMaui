using MauiPersianToolkit.Enums;
using MauiPersianToolkit.Models;
using MauiPersianToolkit.Services.Calendar;
using MauiPersianToolkit.ViewModels;
using Xunit;

namespace MauiPersianToolkit.Test;

/// <summary>
/// Test cases for calendar week layout - verifying days are displayed in correct columns
/// Tests that days of month appear in the correct grid columns based on day of week
/// Example: 1403/05/02 (if Friday) should appear in Friday column, not Sunday column
/// </summary>
public class CalendarWeekLayoutTests
{
    private readonly ICalendarService _persianService = CalendarServiceFactory.GetService(CalendarType.Persian);
    private readonly ICalendarService _gregorianService = CalendarServiceFactory.GetService(CalendarType.Gregorian);
    private readonly ICalendarService _hijriService = CalendarServiceFactory.GetService(CalendarType.Hijri);

    /// <summary>
    /// Test that consecutive days appear in consecutive columns
    /// </summary>
    [Fact]
    public void TestConsecutiveDaysInConsecutiveColumns()
    {
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // Get first few days of month
        var firstDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 1);
        var secondDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 2);
        var thirdDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 3);

        Assert.NotNull(firstDay);
        Assert.NotNull(secondDay);
        Assert.NotNull(thirdDay);

        int firstDayIndex = viewModel.DaysOfMonth.IndexOf(firstDay);
        int secondDayIndex = viewModel.DaysOfMonth.IndexOf(secondDay);
        int thirdDayIndex = viewModel.DaysOfMonth.IndexOf(thirdDay);

        // Consecutive days should be consecutive in the list
        Assert.Equal(secondDayIndex, firstDayIndex + 1);
        Assert.Equal(thirdDayIndex, secondDayIndex + 1);
    }

    /// <summary>
    /// Test that each week row has exactly 7 columns
    /// </summary>
    [Fact]
    public void TestWeekRowHasSevenColumns()
    {
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // Total days in the grid should be divisible by 7
        Assert.True(viewModel.DaysOfMonth.Count % 7 == 0,
            $"Days count {viewModel.DaysOfMonth.Count} should be divisible by 7");
    }

    /// <summary>
    /// Test Persian calendar - each day should be in the correct day-of-week column
    /// </summary>
    [Fact]
    public void TestPersianCalendarDayColumnAlignment()
    {
        var testDate = new DateTime(2024, 9, 22); // Around Persian month 6-7

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Persian
        };

        var viewModel = new DatePickerViewModel(options);

        // Check each day
        for (int i = 0; i < viewModel.DaysOfMonth.Count; i++)
        {
            var day = viewModel.DaysOfMonth[i];

            if (!day.IsInCurrentMonth)
                continue;

            // Get the column position (0-6)
            int columnPosition = i % 7;

            // Get the actual day of week
            var dayOfWeek = _persianService.GetDayOfWeek(day.GregorianDate);
            int expectedColumn = (int)dayOfWeek;

            // Verify the day is in the correct column
            Assert.Equal(expectedColumn, columnPosition);
        }
    }

    /// <summary>
    /// Test Gregorian calendar - each day should be in the correct day-of-week column
    /// </summary>
    [Fact]
    public void TestGregorianCalendarDayColumnAlignment()
    {
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // Check each day
        for (int i = 0; i < viewModel.DaysOfMonth.Count; i++)
        {
            var day = viewModel.DaysOfMonth[i];

            if (!day.IsInCurrentMonth)
                continue;

            // Get the column position (0-6)
            int columnPosition = i % 7;

            // Get the actual day of week
            var dayOfWeek = _gregorianService.GetDayOfWeek(day.GregorianDate);
            int expectedColumn = (int)dayOfWeek;

            // Verify the day is in the correct column
            Assert.Equal(expectedColumn, columnPosition);
        }
    }

    /// <summary>
    /// Test that empty cells at the beginning of month match the first day's position
    /// </summary>
    [Fact]
    public void TestEmptyCellsBeforeFirstDay()
    {
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // Get first day of month
        var firstDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 1);
        Assert.NotNull(firstDay);

        int firstDayIndex = viewModel.DaysOfMonth.IndexOf(firstDay);
        int columnsBeforeFirstDay = firstDayIndex % 7;

        // The number of empty cells should match the day of week of the first day
        var firstDayOfWeek = _gregorianService.GetDayOfWeek(firstDay.GregorianDate);
        Assert.Equal((int)firstDayOfWeek, columnsBeforeFirstDay);
    }

    /// <summary>
    /// Test end of month - last day should be in correct position
    /// </summary>
    [Fact]
    public void TestEndOfMonthDayPosition()
    {
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // August 2024 has 31 days
        var lastDay = viewModel.DaysOfMonth.Where(d => d.IsInCurrentMonth).LastOrDefault();
        Assert.NotNull(lastDay);
        Assert.Equal(31, lastDay.DayNum);

        int lastDayIndex = viewModel.DaysOfMonth.IndexOf(lastDay);
        int columnPosition = lastDayIndex % 7;

        var lastDayOfWeek = _gregorianService.GetDayOfWeek(lastDay.GregorianDate);
        Assert.Equal((int)lastDayOfWeek, columnPosition);
    }

    /// <summary>
    /// Test that all days of month are consecutive in the grid
    /// </summary>
    [Fact]
    public void TestAllDaysAreConsecutive()
    {
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        var currentMonthDays = viewModel.DaysOfMonth.Where(d => d.IsInCurrentMonth).ToList();

        // Check that days are numbered 1, 2, 3, ..., lastDay
        for (int i = 0; i < currentMonthDays.Count; i++)
        {
            Assert.Equal(i + 1, currentMonthDays[i].DayNum);
        }
    }

    /// <summary>
    /// Test multiple months to ensure consistent week layout
    /// </summary>
    [Theory]
    [InlineData(2024, 1)]
    [InlineData(2024, 6)]
    [InlineData(2024, 12)]
    public void TestMultipleMonthsWeekLayout(int year, int month)
    {
        var testDate = new DateTime(year, month, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // Verify grid has 7 columns
        Assert.True(viewModel.DaysOfMonth.Count % 7 == 0);

        // Verify first day is in correct position
        var firstDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 1);
        Assert.NotNull(firstDay);

        int firstDayIndex = viewModel.DaysOfMonth.IndexOf(firstDay);
        int columnPosition = firstDayIndex % 7;

        var expectedDayOfWeek = _gregorianService.GetDayOfWeek(firstDay.GregorianDate);
        Assert.Equal((int)expectedDayOfWeek, columnPosition);
    }

    /// <summary>
    /// Test Persian calendar month layout with different months
    /// </summary>
    [Theory]
    [InlineData("1403/01/15")] // Farvardin
    [InlineData("1403/06/15")] // Shahrivar
    [InlineData("1403/12/15")] // Esfand
    public void TestPersianCalendarMultipleMonths(string persianDate)
    {
        var testDate = _persianService.ToGregorianDate(persianDate);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Persian
        };

        var viewModel = new DatePickerViewModel(options);

        // Verify grid structure
        Assert.True(viewModel.DaysOfMonth.Count % 7 == 0,
            $"Days count should be divisible by 7 for {persianDate}");

        // Verify all consecutive days are present
        var monthDays = viewModel.DaysOfMonth.Where(d => d.IsInCurrentMonth).ToList();
        for (int i = 0; i < monthDays.Count; i++)
        {
            Assert.Equal(i + 1, monthDays[i].DayNum);
        }
    }

    /// <summary>
    /// Test that holiday days appear in correct columns
    /// For Gregorian: Saturday is last day (column 6)
    /// For Persian: Friday is last day (column 6)
    /// </summary>
    [Fact]
    public void TestHolidayDayInCorrectColumn_Gregorian()
    {
        // October 2024: 5th is Saturday (first Saturday of month)
        var testDate = new DateTime(2024, 10, 5);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        // Find Saturday the 5th
        var saturdayDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 5);
        Assert.NotNull(saturdayDay);

        var dayOfWeek = _gregorianService.GetDayOfWeek(saturdayDay.GregorianDate);
        Assert.Equal(DayOfWeek.Saturday, dayOfWeek);

        int saturdayIndex = viewModel.DaysOfMonth.IndexOf(saturdayDay);
        int columnPosition = saturdayIndex % 7;

        // Saturday should be in column 6 (last column)
        Assert.Equal(6, columnPosition);
    }

    /// <summary>
    /// Test specific Persian calendar date alignment
    /// Verify dates appear in their correct day-of-week columns
    /// </summary>
    [Fact]
    public void TestSpecificPersianDateColumnPlacement()
    {
        // Test 1403/05/02
        var persianDate = "1403/05/02";
        var gregorianDate = _persianService.ToGregorianDate(persianDate);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Persian
        };

        var viewModel = new DatePickerViewModel(options);

        var targetDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 2);
        Assert.NotNull(targetDay);

        // Get actual day of week
        var dayOfWeek = _persianService.GetDayOfWeek(targetDay.GregorianDate);

        // Find position in grid
        int dayIndex = viewModel.DaysOfMonth.IndexOf(targetDay);
        int columnPosition = dayIndex % 7;

        // Verify position matches day of week
        Assert.Equal((int)dayOfWeek, columnPosition);
    }

    /// <summary>
    /// Test Gregorian calendar specific date alignment
    /// </summary>
    [Fact]
    public void TestSpecificGregorianDateColumnPlacement()
    {
        // Test August 15, 2024 (Thursday)
        var testDate = new DateTime(2024, 8, 15);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        var targetDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 15);
        Assert.NotNull(targetDay);

        var dayOfWeek = _gregorianService.GetDayOfWeek(targetDay.GregorianDate);
        Assert.Equal(DayOfWeek.Thursday, dayOfWeek);

        int dayIndex = viewModel.DaysOfMonth.IndexOf(targetDay);
        int columnPosition = dayIndex % 7;

        // Thursday = column 4
        Assert.Equal(4, columnPosition);
    }

    /// <summary>
    /// Test that month with different starting days aligns correctly
    /// </summary>
    [Theory]
    [InlineData(2024, 2)] // February starts on Thursday
    [InlineData(2024, 3)] // March starts on Friday
    [InlineData(2024, 4)] // April starts on Monday
    public void TestVariousMonthStartAlignment(int year, int month)
    {
        var testDate = new DateTime(year, month, 1);

        var options = new CalendarOptions
        {
            CalendarType = CalendarType.Gregorian
        };

        var viewModel = new DatePickerViewModel(options);

        var firstDay = viewModel.DaysOfMonth.FirstOrDefault(d => d.DayNum == 1);
        Assert.NotNull(firstDay);

        var expectedDayOfWeek = _gregorianService.GetDayOfWeek(firstDay.GregorianDate);
        int firstDayIndex = viewModel.DaysOfMonth.IndexOf(firstDay);
        int columnPosition = firstDayIndex % 7;

        Assert.Equal((int)expectedDayOfWeek, columnPosition);
    }
}
