using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.BusinessLogic.Bookings;
using VacationRental.Api.BusinessLogic.Rentals;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingsBL _bookingsBL;
        private readonly IRentalsBL _rentalsBL;

        public BookingsController(
            IBookingsBL bookingsBL,
            IRentalsBL rentalsBL
            )
        {
            _bookingsBL = bookingsBL;
            _rentalsBL = rentalsBL;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            var bookingToReturn = _bookingsBL.GetBookingById(bookingId);

            if (bookingToReturn == null)
                throw new ApplicationException("Booking not found");

            return bookingToReturn;
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel bookingToAdd)
        {
            if (bookingToAdd.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");

            if (!_rentalsBL.RentalKeyExists(bookingToAdd.RentalId))
                throw new ApplicationException("Rental not found");

            for (var i = 0; i < bookingToAdd.Nights; i++)
            {
                var rentalPreparationTimeInDays = _rentalsBL.GetRentalPreparationTimeInDays(bookingToAdd.RentalId);
                var bookingAvailableUnits = _bookingsBL.GetBookingAvailableUnits(bookingToAdd, rentalPreparationTimeInDays);

                if (bookingAvailableUnits >= _rentalsBL.GetRentalNumberOfUnits(bookingToAdd.RentalId))
                    throw new ApplicationException("Not available");
            }

            return _bookingsBL.AddNewBooking(bookingToAdd);
        }
    }
}
