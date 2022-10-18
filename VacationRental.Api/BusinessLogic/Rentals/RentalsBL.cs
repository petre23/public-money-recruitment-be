using System.Collections.Generic;
using VacationRental.Api.Models;

namespace VacationRental.Api.BusinessLogic.Rentals
{
    public class RentalsBL : IRentalsBL
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        public RentalsBL(IDictionary<int, RentalViewModel> rentals)
        {
            _rentals = rentals;
        }

        public bool RentalKeyExists(int rentalIdToCheck)
        {
            return _rentals != null 
                && _rentals.Count > 0 
                && _rentals.ContainsKey(rentalIdToCheck);
        }

        public int GetRentalNumberOfUnits(int rentalId)
        {
            return RentalKeyExists(rentalId) ? _rentals[rentalId].Units : 0;
        }

        public RentalViewModel GetRentalById(int rentalId)
        {
            return (_rentals != null
                    && _rentals.Count > 0
                    && _rentals.ContainsKey(rentalId))
                    ? _rentals[rentalId]
                    : null;
        }

        public ResourceIdViewModel AddNewRental(RentalBindingModel rentalToAdd)
        {
            var newRentalId = _rentals.Keys.Count + 1 ;

            _rentals.Add(newRentalId, new RentalViewModel
            {
                Id = newRentalId,
                Units = rentalToAdd.Units,
                PreparationTimeInDays = rentalToAdd.PreparationTimeInDays
            });

            return new ResourceIdViewModel { Id = newRentalId };
        }

        public int GetRentalPreparationTimeInDays(int rentalId)
        {
            var rental = GetRentalById(rentalId);
            return rental != null ? rental.PreparationTimeInDays : 0;
        }

        public ResourceIdViewModel UpdateRentalDetails(int rentalId, RentalBindingModel rentalToAdd)
        {
            var rentalToUpdate = GetRentalById(rentalId);
            if(rentalToUpdate != null)
            {
                rentalToUpdate.Units = rentalToAdd.Units;
                rentalToUpdate.PreparationTimeInDays = rentalToAdd.PreparationTimeInDays;

                _rentals[rentalId] = rentalToUpdate;
            }

            return new ResourceIdViewModel { Id = rentalId };
        }
    }
}
