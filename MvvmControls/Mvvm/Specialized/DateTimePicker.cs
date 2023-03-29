using System;
using System.Collections.Generic;
using System.Linq;

namespace RFBCodeWorks.Mvvm.Specialized
{
    /// <summary>
    /// ViewModel used to select a Date/Time
    /// </summary>
    public class DateTimePicker : ViewModelBase
    {

        static DateTimePicker()
        {
            Static12Hours = new Helpers.NetStandardImmutableArray<int>(IntegerPicker.GenerateArray(12));
            Static24Hours = new Helpers.NetStandardImmutableArray<int>(IntegerPicker.GenerateArray(24));
            StaticMinutesSeconds = new Helpers.NetStandardImmutableArray<int>(IntegerPicker.GenerateArray(60));
            StaticAMPM = new Helpers.NetStandardImmutableArray<string>(new string[] { "AM", "PM" });
        }

        private static IList<int> Static24Hours { get; }
        private static IList<int> Static12Hours { get; }
        private static IList<string> StaticAMPM { get; }
        private static IList<int> StaticMinutesSeconds { get; }

        private static INotifyArgs SelectedHoursChangingArgs { get; } = new(nameof(SelectedHours));
        private static INotifyArgs SelectedTimeOfDayChangingArgs { get; } = new(nameof(SelectedTimeOfDay));
        private static INotifyArgs SelectedDateTimeChangingArgs { get; } = new(nameof(SelectedDateTime));


        /// <summary> </summary>
        public DateTimePicker() : this(null) { }

        /// <summary> </summary>
        public DateTimePicker(IViewModel parent) : base(parent) { }

        /// <summary> Flag to tell the <see cref="SelectedDateTime"/> property to recalculate itself on next get</summary>
        private bool recalculateSelectedDateTime;

        /// <summary> Flag to suppress <see cref="DateTimeUpdated"/> event until all operations are complete </summary>
        private bool suppressEvents;

        /// <summary>
        /// Immutable array for the Hours of the day: 0-23
        /// </summary>
        public IList<int> Hours24 => Static24Hours;


        /// <summary>
        /// Immutable array for the Hours of the day: 0-12 -- For use with <see cref="AmPm"/>
        /// </summary>
        public IList<int> Hours12 => Static12Hours;

        /// <summary>
        /// Immutable array for Minutes / Seconds menus. 0-59
        /// </summary>
        public IList<int> MinutesSeconds => StaticMinutesSeconds;

        /// <summary>
        /// Immutable array Containing "AM" &amp; "PM"
        /// </summary>
        public IList<string> AmPm => StaticAMPM;


        /// <summary>
        /// DateTime field for use with a DatePicker control - Only stores the DATE component of the value
        /// </summary>
        public DateTime SelectedDate
        {
            get { return DateField; }
            set {
                if (SetProperty(ref DateField, value.Date, nameof(SelectedDate)))
                    OnDateTimeUpdated();
            }
        }
        private DateTime DateField = DateTime.Today;

        /// <summary>
        /// Bound value to set the HOURS component of the <see cref="SelectedDateTime"/>
        /// </summary>
        public virtual int SelectedHours
        {
            get { return SelectedHoursField; }
            set
            {
                if (SelectedHoursField == value) return;
                if (value >= 0 && value < 24)
                {
                    OnPropertyChanging(SelectedHoursChangingArgs);
                    bool eventsalreadysuppressed = suppressEvents;
                    suppressEvents = true;
                    if (!Use24HourClock) //Using 12 hour clock
                    {
                       if (value == 0 | value == 12) // 12am
                        {
                            SelectedHoursField = 12;
                            SelectedAmPm = AmPm[0];
                        }
                       else if (value < 12)
                        {
                            SelectedHoursField = value;
                        }
                        else if (value > 12)
                        {
                            SelectedHoursField = value - 12;
                            SelectedAmPm = AmPm[1];
                        }
                    }
                    else if (Use24HourClock)
                    {
                        if (value < 12)
                            SelectedAmPm = AmPm[0];
                        else
                            SelectedAmPm = AmPm[1];
                        SelectedHoursField = value;
                    }
                    OnPropertyChanged(SelectedHoursChangingArgs);
                    suppressEvents = eventsalreadysuppressed;
                    OnDateTimeUpdated();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Expected value between 0-23");
                }
            }
        }
        private int SelectedHoursField;


        /// <summary>
        /// Bound value to set the Minutes component of the <see cref="SelectedDateTime"/>
        /// </summary>
        public virtual int SelectedMinutes
        {
            get { return SelectedMinutesField; }
            set
            {
                if (value >= 0 && value < 60)
                {
                    if (SetProperty(ref SelectedMinutesField, value, nameof(SelectedMinutes)))
                        OnDateTimeUpdated();
                }
            }
        }
        private int SelectedMinutesField;


        /// <summary>
        /// Bound value to set the Seconds component of the <see cref="SelectedDateTime"/>
        /// </summary>
        public virtual int SelectedSeconds
        {
            get { return SelectedSecondsField; }
            set
            {
                if (value >= 0 && value < 60)
                {
                    if (SetProperty(ref SelectedSecondsField, value, nameof(SelectedSeconds)))
                        OnDateTimeUpdated();
                }
            }
        }
        private int SelectedSecondsField;


        /// <summary>
        /// Bound value to set the AM/PM component of the <see cref="SelectedDateTime"/>
        /// </summary>
        public string SelectedAmPm
        {
            get { return SelectedAmPmField; }
            set
            {
                
                if (AmPm.Contains(value))
                {

                    if (SetProperty(ref SelectedAmPmField, value, nameof(SelectedAmPm)))
                    {
                        if (Use24HourClock) // Add/Subtract as needed if using 24-hour clock
                        {
                            bool alreadySuppressed = suppressEvents;
                            suppressEvents = true;
                            if (SelectedHours > 12)
                                SelectedHours -= 12;
                            else if (SelectedHours < 12)
                                SelectedHours += 12;
                            suppressEvents = alreadySuppressed;
                        }
                        OnDateTimeUpdated();
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Expected either 'AM' or 'PM' as the value");
                }
            }
        }
        private string SelectedAmPmField = StaticAMPM[0];


        /// <summary>
        /// Flag used to determine how this object reacts to setting the <see cref="SelectedHours"/> property. 
        /// <br/> If set false, uses the 12-hour clock. (Default is false)
        /// </summary>
        public bool Use24HourClock
        {
            get { return Use24HourClockField; }
            set { 
                if (Use24HourClock != value)
                {
                    SetProperty(ref Use24HourClockField, value, nameof(Use24HourClock));
                    suppressEvents = true;
                    if (value) // using 24-hours
                    {
                        bool isPM = SelectedAmPm == AmPm[1];
                        if (isPM && SelectedHours < 12)
                        {
                            SelectedHours += 12;
                        }
                        else if (!isPM && SelectedHours == 12) //12am
                        {
                            SelectedHours = 0;
                        }
                    }
                    else // using 12-hours
                    {
                        if (SelectedHours > 12)
                        {
                            SelectedHours -= 12;
                            SelectedAmPm = AmPm[1];
                        }
                        else
                        {
                            SelectedAmPm = AmPm[0];
                            if (SelectedHours == 0)
                            {
                                SelectedHours = 12;
                            }
                        }
                    }
                    suppressEvents = false;
                }
            }
        }
        private bool Use24HourClockField;


        /// <summary>
        /// Returns a new <see cref="DateTime"/> object that is the result of all the selections current status
        /// </summary>
        public DateTime SelectedDateTime
        {
            get
            {
                if (recalculateSelectedDateTime)
                {
                    int hours = SelectedHours;
                    if (!Use24HourClock && SelectedAmPm == AmPm[1]) // add 12 hours if using 12-hour clock and set to PM
                    {
                        hours += 12;
                    }
                    SelectedDateTimeField = DateTime.Parse($@"{SelectedDate.Date.ToShortDateString()} {hours}:{SelectedMinutes}:{SelectedSeconds}");
                    recalculateSelectedDateTime = false;
                }
                return SelectedDateTimeField;
            }

            set
            {
                suppressEvents = true;
                SelectedDateTimeField = value;
                SelectedDate = value;
                SelectedTimeOfDay = value.TimeOfDay; // updates all the hours/minutes/seconds, etc
                recalculateSelectedDateTime = false;
                suppressEvents = false;
                OnDateTimeUpdated();
            }
        }
        private DateTime SelectedDateTimeField;

        /// <summary>
        /// Returns a new <see cref="TimeSpan"/> object that is the represents the selected time of day
        /// </summary>
        public TimeSpan SelectedTimeOfDay
        {
            get
            {
                return SelectedDateTime.TimeOfDay;
            }
            set
            {
                bool isAlreadySuppressingEvents = suppressEvents;
                suppressEvents = true;
                this.SelectedHours = value.Hours;
                //this.SelectedAmPm = value.Hours >= 12 ? AmPm[0] : AmPm[1]; // AM/PM is set via the SelectedHours property!
                this.SelectedMinutes = value.Minutes;
                this.SelectedSeconds = value.Seconds;
                suppressEvents = isAlreadySuppressingEvents;
                OnDateTimeUpdated();
            }
        }


        /// <summary>
        /// Boolean value to enable/disable a DatePicker control
        /// </summary>
        public bool IsDateEnabled
        {
            get { return IsDateEnabledField; }
            set { SetProperty(ref IsDateEnabledField, value, nameof(IsDateEnabled)); }
        }
        private bool IsDateEnabledField;

        /// <summary>
        /// Boolean value to enable/disable time-picker controls
        /// </summary>
        public bool IsTimeEnabled
        {
            get { return IsTimeEnabledField; }
            set { SetProperty(ref IsTimeEnabledField, value, nameof(IsTimeEnabled)); }
        }
        private bool IsTimeEnabledField;


        #region < DateTimeUpdated >

        /// <summary>
        /// Occurs when the <see cref="SelectedDateTime"/> property changes.
        /// </summary>
        public event EventHandler DateTimeUpdated;

        /// <summary> Raises the DateTimeUpdated event </summary>
        protected void OnDateTimeUpdated()
        {
            OnDateTimeUpdated(EventArgs.Empty);
        }

        /// <summary> Raises the DateTimeUpdated event </summary>
        protected virtual void OnDateTimeUpdated(EventArgs e)
        {
            if (suppressEvents) return; 
            recalculateSelectedDateTime = true;
            OnPropertyChanged(SelectedDateTimeChangingArgs);
            OnPropertyChanged(SelectedTimeOfDayChangingArgs);
            DateTimeUpdated?.Invoke(this, e);
        }

        #endregion



    }
}
