using Microsoft.AspNetCore.Components;
using System;

namespace BlazorTest.Client.Components
{
    public partial class GeckosTimePicker
    {
        [Parameter]
        public TimeSpan? Min { get; set; }
        [Parameter]
        public TimeSpan? Max { get; set; }
        [Parameter]
        public DateTime Value { get; set; }
        [Parameter]
        public EventCallback<DateTime> ValueChanged { get; set; }
        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public int? Step { get; set; }

        protected Guid Id { get; private set; } = Guid.NewGuid();

        [Parameter]
        public string Label { get; set; }

        private DateTime _internalValue;
        public DateTime InternalValue
        {
            get
            {
                return _internalValue;
            }
            set
            {
                if (_internalValue.Hour != value.Hour || _internalValue.Minute != value.Minute)
                {
                    _internalValue = _internalValue.Date.AddHours(value.Hour).AddMinutes(value.Minute);
                    this.Value = _internalValue;
                    this.ValueChanged.InvokeAsync(this.Value);
                }
            }
        }

        protected override void OnParametersSet()
        {
            _internalValue = this.Value;

            if (this.Min.HasValue && this.Value.ToDayTimeSpan() < this.Min)
            {
                this.InternalValue = this.Value.ToTime(this.Min.Value);
            }
            else if (this.Max.HasValue && this.Value.ToDayTimeSpan() > this.Max)
            {
                this.InternalValue = this.Value.ToTime(this.Max.Value);
            }


            base.OnParametersSet();
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToUltraShordDateString(this DateTime? source)
        {
            return ToUltraShordDateString(source ?? DateTime.MinValue);
        }

        /// <summary>
        /// repris de DateToStringConverter
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUltraShordDateString(this DateTime source)
        {
            return source == DateTime.MinValue ? null : source.ToString("dd/MM/yy");
        }

        public static string ToLongDay(this DateTime? source)
        {
            return ToLongDay(source ?? DateTime.MinValue);
        }

        public static string ToLongDay(this DateTime source)
        {
            return source == DateTime.MinValue ? null : source.ToString("dddd");
        }

        /// <summary>
        /// renvoie le timespan correspondant à la partie journée de la date
        /// exemple
        /// 12/03/2018 15:22:07
        /// => 15:22:07
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TimeSpan ToDayTimeSpan(this DateTime source)
        {
            return new TimeSpan(source.Hour, source.Minute, source.Second);
        }

        public static bool IsBetween(this DateTime source, DateTime? start, DateTime? end)
        {
            return IsBetween(source, start ?? DateTime.MinValue, end ?? DateTime.MaxValue);
        }

        public static bool IsBetween(this DateTime source, DateTime start, DateTime? end)
        {
            return IsBetween(source, start, end ?? DateTime.MaxValue);
        }

        public static bool IsBetween(this DateTime source, DateTime? start, DateTime end)
        {
            return IsBetween(source, start ?? DateTime.MinValue, end);
        }

        public static bool IsBetween(this DateTime source, DateTime start, DateTime end)
        {
            return source >= start && source <= end;
        }

        public static DateTime ToEndOfDay(this DateTime source)
        {
            return source.Date.AddDays(1).AddMilliseconds(-1);
        }

        public static DateTime ToEndOfMonth(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, DateTime.DaysInMonth(source.Year, source.Month));
        }

        public static DateTime ToTime(this DateTime source, TimeSpan hours)
        {
            return source.ToTime(hours.Hours, hours.Minutes, hours.Seconds);
        }

        public static DateTime ToTime(this DateTime source, int hours, int minutes, int seconds)
        {
            return source.Date.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
        }
    }
}
