using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Bookings
{
    public interface IBookingsBL
    {
        BookingViewModel GetBookingById(int bookingId);
        ResourceIdViewModel AddNewBooking(BookingBindingModel bookingDetailsToAdd);
        int GetBookingAvailableUnits(BookingBindingModel bookingDetailsToCheck, int rentalPreparationTimeInDays = 0);
        bool CanUpdateBookingForChangedRentalDetails(int rentalId, int previousPreparationTimeInDays, RentalBindingModel updatedRentalDetails);
    }
}
