using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PutRentalTests
    {
        private readonly HttpClient _client;

        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 3
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var rentalUpdateRequest = new RentalBindingModel
            {
                Units = 20,
                PreparationTimeInDays = 2
            };

            ResourceIdViewModel putResult;
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", rentalUpdateRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                putResult = await putResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(rentalUpdateRequest.Units, getResult.Units);
                Assert.Equal(rentalUpdateRequest.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAPostReturnsErrorWhenTheUnitsNumberDecreases()
        {
            var request = new RentalBindingModel
            {
                Units = 3,
                PreparationTimeInDays = 3
            };

            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2022, 10, 21),
                Unit = 1
            };

            ResourceIdViewModel postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2022, 10, 21),
                Unit = 2
            };

            ResourceIdViewModel postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking3Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 1,
                Start = new DateTime(2022, 10, 21),
                Unit = 3
            };

            ResourceIdViewModel postBooking3Result;
            using (var postBooking3Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking3Request))
            {
                Assert.True(postBooking3Response.IsSuccessStatusCode);
                postBooking3Result = await postBooking3Response.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var rentalUpdateRequest = new RentalBindingModel
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", rentalUpdateRequest))
                {
                }
            });
        }
    }
}
