using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.BusinessLogic.Calendar;
using VacationRental.Api.BusinessLogic.Rentals;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IRentalsBL _rentalsBL;
        private readonly ICalendarBL _calendarBL;

        public CalendarController(
            IRentalsBL rentalsBL,
            ICalendarBL calendarBL)
        {
            _rentalsBL = rentalsBL;
            _calendarBL = calendarBL;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");

            if (!_rentalsBL.RentalKeyExists(rentalId))
                throw new ApplicationException("Rental not found");

            return _calendarBL.GetCalendarViewModelForBookingDetails(rentalId, start, nights);
        }
    }
}
