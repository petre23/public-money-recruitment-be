using System;
using System.Collections;
using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Calendar
{
    public class CalendarBL : ICalendarBL
    {
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public CalendarBL(IDictionary<int, BookingViewModel> bookings)
        {
            _bookings = bookings;
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

            foreach (var booking in _bookings.Values)
            {
                if (booking.RentalId == rentalId
                    && booking.Start <= calendarDate.Date && booking.Start.AddDays(booking.Nights) > calendarDate.Date)
                {
                    calendarDate.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id });
                }
            }

            return calendarDate;
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
