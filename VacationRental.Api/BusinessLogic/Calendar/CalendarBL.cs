using System;
using System.Collections;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Calendar
{
    public class CalendarBL : ICalendarBL
    {
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly IDictionary<int, PreparationTimeViewModel> _preparationTimes;

        public CalendarBL(
            IDictionary<int, BookingViewModel> bookings
            , IDictionary<int, PreparationTimeViewModel> preparationTimes)
        {
            _bookings = bookings;
            _preparationTimes = preparationTimes;
        }

        public CalendarViewModel GetCalendarViewModelForBookingDetails(int rentalId, DateTime start, int nights)
        {
            var newCalendarViewModel = GetNewCalendarViewModel(rentalId);

            for (var i = 0; i < nights; i++)
            {
                var calendarDate = GetNewCalendarDatesWithAddedBookings(start, i, rentalId);
                newCalendarViewModel.Dates.Add(calendarDate);
            }

            return newCalendarViewModel;
        }

        private CalendarViewModel GetNewCalendarViewModel(int rentalId)
        {
            return new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };
        }

        private CalendarDateViewModel GetNewCalendarDatesWithAddedBookings(DateTime startDate, int nightsToAdd, int rentalId)
        {
            var calendarDate = GetNewCalendarDateViewModelForStartDate(startDate, nightsToAdd);
            calendarDate.Bookings = GetCalendarBookings(rentalId, calendarDate.Date);
            calendarDate.PreparationTimes = GetCalendarPreparationTimes(rentalId);

            return calendarDate;
        }

        private List<CalendarBookingViewModel> GetCalendarBookings(int rentalId, DateTime calendarDate)
        {
            var bookingsForCalendar = new List<CalendarBookingViewModel>();
            foreach (var booking in _bookings.Values)
            {
                if (booking.RentalId == rentalId
                    && booking.Start <= calendarDate.Date && booking.Start.AddDays(booking.Nights) > calendarDate.Date)
                {
                    bookingsForCalendar.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = booking.Unit });
                }
            }

            return bookingsForCalendar;
        }

        private List<CalendarPreparationTimeViewModel> GetCalendarPreparationTimes(int rentalId)
        {
            var preparationTimesForCalendar = new List<CalendarPreparationTimeViewModel>();
            foreach (var preparationTime in _preparationTimes.Values)
            {
                if (preparationTime.RentalId == rentalId)
                {
                    preparationTimesForCalendar.Add(new CalendarPreparationTimeViewModel { Unit = preparationTime.Unit });
                }
            }

            return preparationTimesForCalendar;
        }

        private CalendarDateViewModel GetNewCalendarDateViewModelForStartDate(DateTime startDate, int nightsToAdd)
        {
            var calendarDate = new CalendarDateViewModel
            {
                Date = startDate.Date.AddDays(nightsToAdd),
                Bookings = new List<CalendarBookingViewModel>()
            };

            return calendarDate;
        }
    }
}
