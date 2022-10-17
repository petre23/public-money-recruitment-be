using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Bookings
{
    public class BookingsBL : IBookingsBL
    {
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public BookingsBL(IDictionary<int, BookingViewModel> bookings)
        {
            _bookings = bookings;
        }

        public ResourceIdViewModel AddNewBooking(BookingBindingModel bookingDetailsToAdd)
        {
            var newBookingId = _bookings.Keys.Count + 1;

            _bookings.Add(newBookingId, new BookingViewModel
            {
                Id = newBookingId,
                Nights = bookingDetailsToAdd.Nights,
                RentalId = bookingDetailsToAdd.RentalId,
                Start = bookingDetailsToAdd.Start.Date
            });

            return new ResourceIdViewModel { Id = newBookingId };
        }

        public int GetBookingAvailableUnits(BookingBindingModel bookingDetailsToCheck)
        {
            var numberOfAvailableBookingUnitsForDateRange = 0;

            foreach (var booking in _bookings.Values)
            {
                var newBookingStartDate = bookingDetailsToCheck.Start;
                var newBookingStartDateWithAddedNights = newBookingStartDate.AddDays(bookingDetailsToCheck.Nights);
                var bookingStartDate = booking.Start;
                var bookingStartDateWithAddedNights = bookingStartDate.AddDays(booking.Nights);

                if (booking.RentalId == bookingDetailsToCheck.RentalId
                    && (bookingStartDate <= newBookingStartDate.Date && bookingStartDateWithAddedNights > newBookingStartDate.Date)
                    || (bookingStartDate < newBookingStartDateWithAddedNights && bookingStartDateWithAddedNights >= newBookingStartDateWithAddedNights)
                    || (bookingStartDate > newBookingStartDate && bookingStartDateWithAddedNights < newBookingStartDateWithAddedNights))
                {
                    numberOfAvailableBookingUnitsForDateRange++;
                }
            }            

            return numberOfAvailableBookingUnitsForDateRange;
        }

        public BookingViewModel GetBookingById(int bookingId)
        {
            return (_bookings != null 
                    && _bookings.Count > 0 
                    && _bookings.ContainsKey(bookingId)) 
                    ? _bookings[bookingId] 
                    : null;
        }
    }
}
