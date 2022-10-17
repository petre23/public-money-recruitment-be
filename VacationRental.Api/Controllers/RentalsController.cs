using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.BusinessLogic.Rentals;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalsBL _rentalsBL;

        public RentalsController(IRentalsBL rentalsBL)
        {
            _rentalsBL = rentalsBL;
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
    }
}
