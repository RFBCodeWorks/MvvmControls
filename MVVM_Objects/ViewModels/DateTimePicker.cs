using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFBCodeWorks.MVVMObjects.ViewModels
{
    /// <summary>
    /// ViewModel
    /// </summary>
    public class DateTimePicker : ViewModelBase
    {

        static DateTimePicker()
        {
            Static12Hours = new int[] { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11 };
            Static24Hours = new int[] { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
            StaticAMPM = new string[] { "AM", "PM" };
            StaticMinutesSeconds = MVVMObjects.ViewModels.IntegerPicker.GenerateArray(60);
        }

        private static int[] Static24Hours { get; }
        private static int[] Static12Hours { get; }
        private static string[] StaticAMPM{ get; }
        private static int[] StaticMinutesSeconds{ get; }

        /// <summary> </summary>
        public DateTimePicker() :base() { }
        
        /// <summary> </summary>
        public DateTimePicker(ViewModelBase parent) : base(parent) { }

        /// <summary>
        /// 
        /// </summary>
        public int[] Hours24 => Static24Hours;


        /// <summary>
        /// 
        /// </summary>
        public int[] Hours12 => Static12Hours;

        /// <summary>
        /// 
        /// </summary>
        public int[] MinutesSeconds => StaticMinutesSeconds;

        /// <summary>
        /// 
        /// </summary>
        public string[] AmPm => StaticAMPM;


        ///// <summary>
        ///// First date for the DatePicker control
        ///// </summary>
        //public DateTime StartDate
        //{
        //    get { return StartDateField; }
        //    set { SetProperty(ref StartDateField, value, nameof(StartDate)); }
        //}
        //private DateTime StartDateField = DateTime.MinValue;


        ///// <summary>
        ///// Last date for the DatePicker control
        ///// </summary>
        //public DateTime EndDate
        //{
        //    get { return EndDateField; }
        //    set { SetProperty(ref EndDateField, value, nameof(EndDate)); }
        //}
        //private DateTime EndDateField = DateTime.Today;


        /// <summary>
        /// DateTime field for use with a DatePicker control
        /// </summary>
        public DateTime SelectedDate
        {
            get { return DateField; }
            set { SetProperty(ref DateField, value, nameof(SelectedDate)); }
        }
        private DateTime DateField = DateTime.Today;

        /// <summary>
        /// 
        /// </summary>
        public int SelectedHours
        {
            get { return SelectedHoursField; }
            set {
                if (value >= 0 && value < 24)
                {
                    SetProperty(ref SelectedHoursField, value, nameof(SelectedHours));
                    OnDateTimeUpdated();
                }
            }
        }
        private int SelectedHoursField;


        /// <summary>
        /// 
        /// </summary>
        public int SelectedMinutes
        {
            get { return SelectedMinutesField; }
            set {
                if (value >= 0 && value < 60)
                {
                    SetProperty(ref SelectedMinutesField, value, nameof(SelectedMinutes));
                    OnDateTimeUpdated();
                }
            }
        }
        private int SelectedMinutesField;


        /// <summary>
        /// 
        /// </summary>
        public int SelectedSeconds
        {
            get { return SelectedSecondsField; }
            set {
                if (value >= 0 && value < 60)
                {
                    SetProperty(ref SelectedSecondsField, value, nameof(SelectedSeconds));
                    OnDateTimeUpdated();
                }
            }
        }
        private int SelectedSecondsField;


        /// <summary>
        /// 
        /// </summary>
        public string SelectedAmPm
        {
            get { return SelectedAmPmField; }
            set {
                if (string.IsNullOrWhiteSpace(value) | AmPm.Contains(value))
                {
                    SetProperty(ref SelectedAmPmField, value, nameof(SelectedAmPm));
                    OnDateTimeUpdated();
                }
            }
        }
        private string SelectedAmPmField;


        /// <summary>
        /// Returns a new <see cref="DateTime"/> object that is the result of all the selections current status
        /// </summary>
        public DateTime SelectedDateTime
        { 
            get
            {
                int hours = SelectedHours;
                if (!String.IsNullOrWhiteSpace(SelectedAmPm))
                {
                    if (SelectedAmPm == AmPm[1])
                        hours += 12;
                }
                return DateTime.Parse($@"{SelectedDate.Date.ToShortDateString()} {hours}:{SelectedMinutes}:{SelectedSeconds}");

            }

            set
            {
                this.SelectedDate = value;
                this.SelectedTimeOfDay = value.TimeOfDay;
                OnPropertyChanged(nameof(SelectedDateTime));
            }
        }

        /// <summary>
        /// Returns a new <see cref="TimeSpan"/> object that is the represents the selected time of day
        /// </summary>
        public TimeSpan SelectedTimeOfDay
        {
            get
            {
                int hours = SelectedHours;
                if (!String.IsNullOrWhiteSpace(SelectedAmPm))
                {
                    if (SelectedAmPm == AmPm[1])
                        hours += 12;
                }
                return TimeSpan.Parse($@"{hours}:{SelectedMinutes}:{SelectedSeconds}");
            }
            set
            {
                this.SelectedHours = value.Hours;
                this.SelectedAmPm = value.Hours >= 12 ? AmPm[0] : AmPm[1];
                this.SelectedMinutes = value.Minutes;
                this.SelectedSeconds = value.Seconds;
                OnPropertyChanged(nameof(SelectedTimeOfDay));
            }
        }


        /// <summary>
        /// Set the value for if the <see cref="Date"/> is in use by the View
        /// </summary>
        public bool IsDateEnabled
        {
            get { return IsDateEnabledField; }
            set { SetProperty(ref IsDateEnabledField, value, nameof(IsDateEnabled)); }
        }
        private bool IsDateEnabledField;

        /// <summary>
        /// Set the value for if the Time properties are in use by the View
        /// </summary>
        public bool IsTimeEnabled
        {
            get { return IsTimeEnabledField; }
            set { SetProperty(ref IsTimeEnabledField, value, nameof(IsTimeEnabled)); }
        }
        private bool IsTimeEnabledField;


        #region < DateTimeUpdated >

        /// <summary>
        /// Delegate for the DateTimeUpdated event
        /// </summary>
        public delegate void DateTimeUpdatedHandler(DateTimePicker sender, EventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event DateTimeUpdatedHandler DateTimeUpdated;

        /// <summary> Raises the DateTimeUpdated event </summary>
        protected void OnDateTimeUpdated()
        {
            OnDateTimeUpdated(new EventArgs());
        }

        /// <summary> Raises the DateTimeUpdated event </summary>
        protected virtual void OnDateTimeUpdated(EventArgs e)
        {
            OnPropertyChanged(nameof(SelectedDateTime));
            DateTimeUpdated?.Invoke(this, e);
        }

        #endregion



    }
}
