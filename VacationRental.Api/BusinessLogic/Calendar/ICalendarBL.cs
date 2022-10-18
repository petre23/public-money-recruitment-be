using System;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Calendar
{
    public interface ICalendarBL
    {
        CalendarViewModel GetCalendarViewModelForBookingDetails(int rentalId, DateTime start, int nights);
    }
}
