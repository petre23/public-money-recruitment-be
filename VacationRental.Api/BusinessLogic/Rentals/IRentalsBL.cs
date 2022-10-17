using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Rentals
{
    public interface IRentalsBL
    {
        RentalViewModel GetRentalById(int rentalId);
        ResourceIdViewModel AddNewRental(RentalBindingModel rentalToAdd);
        bool RentalKeyExists(int rentalIdToCheck);
        int GetRentalNumberOfUnits(int rentalId);
    }
}
