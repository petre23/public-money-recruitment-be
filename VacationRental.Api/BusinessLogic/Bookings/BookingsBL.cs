using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Bookings
{
    public class BookingsBL : IBookingsBL
    {
        private readonly IDictionary<int, BookingViewModel> _bookings;
        private readonly IDictionary<int, PreparationTimeViewModel> _preparationTimes;

        public BookingsBL(IDictionary<int, BookingViewModel> bookings, IDictionary<int, PreparationTimeViewModel> preparationTimes)
        {
            _bookings = bookings;
            _preparationTimes = preparationTimes;
        }

        public ResourceIdViewModel AddNewBooking(BookingBindingModel bookingDetailsToAdd)
        {
            var newBookingId = _bookings.Keys.Count + 1;

            _bookings.Add(newBookingId, new BookingViewModel
            {
                Id = newBookingId,
                Nights = bookingDetailsToAdd.Nights,
                RentalId = bookingDetailsToAdd.RentalId,
                Start = bookingDetailsToAdd.Start.Date,
                Unit = bookingDetailsToAdd.Unit
            });

            if(bookingDetailsToAdd.Unit > 0)
            {
                var newPreparationTimeId = _preparationTimes.Keys.Count + 1;
                _preparationTimes.Add(_preparationTimes.Keys.Count + 1, new PreparationTimeViewModel
                {
                    Id = newPreparationTimeId,
                    Unit = bookingDetailsToAdd.Unit,
                    RentalId = bookingDetailsToAdd.RentalId
                });
            }

            return new ResourceIdViewModel { Id = newBookingId };
        }

        public int GetBookingAvailableUnits(BookingBindingModel bookingDetailsToCheck, int rentalPreparationTimeInDays = 0)
        {
            var numberOfAvailableBookingUnitsForDateRange = 0;

            foreach (var booking in _bookings.Values.Where(b => b.RentalId == bookingDetailsToCheck.RentalId))
            {
                var newBookingStartDate = bookingDetailsToCheck.Start;
                var newBookingStartDateWithAddedNights = newBookingStartDate.AddDays(bookingDetailsToCheck.Nights);
                var bookingStartDate = booking.Start;
                var bookingStartDateWithAddedNights = bookingStartDate.AddDays(booking.Nights);

                var preparationTimeForBookingUnit = _preparationTimes.Values.Any(p => p.RentalId == bookingDetailsToCheck.RentalId && p.Unit == booking.Unit);
                if(preparationTimeForBookingUnit && rentalPreparationTimeInDays > 0)
                {
                    bookingStartDateWithAddedNights.AddDays(rentalPreparationTimeInDays);
                }

                if ((bookingStartDate <= newBookingStartDate.Date && bookingStartDateWithAddedNights > newBookingStartDate.Date)
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

        public bool CanUpdateBookingForChangedRentalDetails(int rentalId, int previousPreparationTimeInDays, RentalBindingModel updatedRentalDetails)
        {
            return HasValidNumberOfBookedUnitsForUpdatedRental(rentalId, updatedRentalDetails.Units)
                && HasValidPreparationTimesForUpdatedRental(rentalId, previousPreparationTimeInDays, updatedRentalDetails.PreparationTimeInDays);
        }

        private bool HasValidNumberOfBookedUnitsForUpdatedRental(int rentalId, int rentalUpdatedUnitsNumber)
        {
            var bookedUnits = _bookings.Values.Where(booking => booking.RentalId == rentalId).Select(b => b.Unit).Distinct();
            var preparationTimeUnits = _preparationTimes.Values.Where(preparationTime => preparationTime.RentalId == rentalId).Select(p => p.Unit).Distinct();

            var aldreadyBookedUnits = bookedUnits.Intersect(preparationTimeUnits);

            return aldreadyBookedUnits != null && aldreadyBookedUnits.Count() <= rentalUpdatedUnitsNumber;
        }

        private bool HasValidPreparationTimesForUpdatedRental(int rentalId, int previousPreparationTimeInDays, int rentalUpdatedUnitsNumber)
        {
            if (previousPreparationTimeInDays == rentalUpdatedUnitsNumber)
                return true;

            var bookedUnitsForRental = _bookings.Values.Where(booking => booking.RentalId == rentalId);
            var preparationTimeUnitsForRental = _preparationTimes.Values.Where(preparationTime => preparationTime.RentalId == rentalId);

            var distinctBookedUnits = bookedUnitsForRental.Select(b => b.Unit).Distinct();
            var distinctPreparationTimeUnits = preparationTimeUnitsForRental.Select(p => p.Unit).Distinct();

            var allBookedUnitsWithPreparationTimes = distinctBookedUnits.Intersect(distinctPreparationTimeUnits);

            foreach(var preparationTimeUnit in allBookedUnitsWithPreparationTimes)
            {
                var bookedUnitsWithPreparationTimeUnit = bookedUnitsForRental.Where(b => b.Unit == preparationTimeUnit).OrderByDescending(b => b.Start);

                for (int i = 0; i < bookedUnitsWithPreparationTimeUnit.Count() - 2; i++)
                {
                    var booking = bookedUnitsWithPreparationTimeUnit.ElementAt(i);
                    var bookingStartDate = booking.Start;
                    var bookingStartDateWithAddedNights = bookingStartDate.AddDays(booking.Nights);

                    var nextBookingToOverlap = bookedUnitsWithPreparationTimeUnit.ElementAt(i + 1);
                    var nextBookingStartDateToOverlap = nextBookingToOverlap.Start;
                    var nextBookingToOverlapStartDateWithAddedNights = nextBookingStartDateToOverlap.AddDays(booking.Nights);

                    if (bookingStartDateWithAddedNights.AddDays(previousPreparationTimeInDays) > bookingStartDateWithAddedNights.AddDays(rentalUpdatedUnitsNumber)
                        || bookingStartDateWithAddedNights.AddDays(rentalUpdatedUnitsNumber) >= nextBookingToOverlapStartDateWithAddedNights.AddDays(previousPreparationTimeInDays))
                    {
                        return false;
                    }
                    
                }
            }

            return true;
        }
    }
}
