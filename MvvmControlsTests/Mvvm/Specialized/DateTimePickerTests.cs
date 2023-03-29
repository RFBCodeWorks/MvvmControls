using Microsoft.VisualStudio.TestTools.UnitTesting;
using RFBCodeWorks.Mvvm.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Specialized.Tests
{
    [TestClass()]
    public class DateTimePickerTests
    {
        [TestMethod()]
        public void ValidateArrays()
        {
            var picker = new DateTimePicker();
            Assert.ThrowsException<NotImplementedException>(() => picker.Hours24[0] = 5); // Validates that the array is immutable.
            Assert.ThrowsException<NotImplementedException>(() => picker.Hours12.Add(5)); // Validates that the array is immutable.
            Assert.ThrowsException<NotImplementedException>(() => picker.MinutesSeconds.Clear()); // Validates that the array is immutable.
            Assert.AreEqual(24, picker.Hours24.Count);
            Assert.AreEqual(12, picker.Hours12.Count);
            Assert.AreEqual(60, picker.MinutesSeconds.Count);
            Assert.AreEqual(0, picker.Hours12[0]);
            Assert.AreEqual(0, picker.Hours24[0]);
            Assert.AreEqual(0, picker.MinutesSeconds[0]);
            Assert.AreEqual(11, picker.Hours12.Last());
            Assert.AreEqual(23, picker.Hours24.Last());
            Assert.AreEqual(59, picker.MinutesSeconds.Last());
        }

        [TestMethod()]
        public void TestUpdatingSelectedTime()
        {
            var picker = new DateTimePicker();
            picker.IsDateEnabled = false;
            picker.IsTimeEnabled = true;
            picker.Use24HourClock = true;
            picker.SelectedTimeOfDay = new TimeSpan(10, 20, 30);
            Assert.AreEqual(10, picker.SelectedHours);
            Assert.AreEqual(20, picker.SelectedMinutes);
            Assert.AreEqual(30, picker.SelectedSeconds);
            Assert.AreEqual("AM", picker.SelectedAmPm);

            picker.SelectedTimeOfDay = new TimeSpan(16, 25, 35);
            Assert.AreEqual(16, picker.SelectedHours);
            Assert.AreEqual(25, picker.SelectedMinutes);
            Assert.AreEqual(35, picker.SelectedSeconds);
            Assert.AreEqual("PM", picker.SelectedAmPm);

            picker.Use24HourClock = false;
            Assert.AreEqual(4, picker.SelectedHours);
            Assert.AreEqual("PM", picker.SelectedAmPm);

            picker.Use24HourClock = true;
            Assert.AreEqual(16, picker.SelectedHours);
            Assert.AreEqual("PM", picker.SelectedAmPm);
        }

        [TestMethod()]
        public void Convert24Hours()
        {
            var picker = new DateTimePicker();
            picker.IsDateEnabled = false;
            picker.IsTimeEnabled = true;
            picker.Use24HourClock = true;
            picker.SelectedTimeOfDay = new TimeSpan(0, 0, 0);
            Assert.AreEqual(0, picker.SelectedHours);
            Assert.AreEqual("AM", picker.SelectedAmPm);

            picker.Use24HourClock = false; // 0:00:00 becomes 12 AM
            Assert.AreEqual(12, picker.SelectedHours);
            Assert.AreEqual("AM", picker.SelectedAmPm);

            picker.Use24HourClock = true; //12AM becomes 0:00:00
            Assert.AreEqual(0, picker.SelectedHours);
            Assert.AreEqual("AM", picker.SelectedAmPm);

            picker.Use24HourClock = false; // Set false to use 12-hour clock, This updates it to 12AM. Then set the value to PM.
            picker.SelectedAmPm = picker.AmPm[1];
            Assert.AreEqual("PM", picker.SelectedAmPm);
            
            picker.Use24HourClock =true;  // Convert 12pm to 24-hour clock, which is still 12.
            Assert.AreEqual(12, picker.SelectedHours);

            picker.Use24HourClock = false;
            picker.SelectedTimeOfDay = new TimeSpan(13, 0, 0);
            Assert.AreEqual(1, picker.SelectedHours);
            Assert.AreEqual("PM", picker.SelectedAmPm);

            picker.Use24HourClock = true;
            picker.SelectedTimeOfDay = new TimeSpan(08, 0, 0);
            Assert.AreEqual(08, picker.SelectedHours);
            Assert.AreEqual("AM", picker.SelectedAmPm);
            picker.SelectedTimeOfDay = new TimeSpan(14, 0, 0);
            Assert.AreEqual(14, picker.SelectedHours);
            Assert.AreEqual("PM", picker.SelectedAmPm);

            picker.SelectedAmPm = "AM";
            Assert.AreEqual(2, picker.SelectedHours); // Check that converting from PM to AM subtracts 12 from 14
            picker.SelectedAmPm = "PM";
            Assert.AreEqual(14, picker.SelectedHours); // Check that converting from AM to PM adds 12

        }

        [TestMethod()]
        public void TestUpdatingSelectedDateTime()
        {
            var picker = new DateTimePicker();
            picker.IsDateEnabled = false;
            picker.IsTimeEnabled = true;
            picker.SelectedDateTime = new DateTime(2023, 03, 29, 5, 40, 25);
            
            Assert.AreEqual(5, picker.SelectedHours);
            Assert.AreEqual(40, picker.SelectedMinutes);
            Assert.AreEqual(25, picker.SelectedSeconds);
            Assert.AreEqual("AM", picker.SelectedAmPm);
            Assert.AreEqual(new TimeSpan(5, 40, 25).TotalSeconds, picker.SelectedTimeOfDay.TotalSeconds);
        }
    }
}