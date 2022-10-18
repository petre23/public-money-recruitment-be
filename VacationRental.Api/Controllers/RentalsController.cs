using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.BusinessLogic.Bookings;
using VacationRental.Api.BusinessLogic.Rentals;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalsBL _rentalsBL;
        private readonly IBookingsBL _bookingsBL;

        public RentalsController(IRentalsBL rentalsBL, IBookingsBL bookingsBL)
        {
            _rentalsBL = rentalsBL;
            _bookingsBL = bookingsBL;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            var rentalToReturn = _rentalsBL.GetRentalById(rentalId);

            if (rentalToReturn == null)
                throw new ApplicationException("Rental not found");

            return rentalToReturn;
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel rentalToAdd)
        {
            return _rentalsBL.AddNewRental(rentalToAdd);
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public ResourceIdViewModel Put(int rentalId, RentalBindingModel rentalUpdateDetails)
        {
            if(!_bookingsBL.CanUpdateBookingForChangedRentalDetails(rentalId, _rentalsBL.GetRentalPreparationTimeInDays(rentalId), rentalUpdateDetails))
            {
                throw new ApplicationException("Cannot update existing bookings with new preparation time");
            }

            return _rentalsBL.UpdateRentalDetails(rentalId, rentalUpdateDetails);
        }
    }
}
