using Microsoft.AspNetCore.Mvc.RazorPages;
using FruitWebApp.Models;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace FruitWebApp.Pages
{
    public class IndexModel : PageModel
    {
        // IHttpClientFactory set using dependency injection 
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Add the data model and bind the form data to the page model properties
        // Enumerable since an array is expected as a response
        [BindProperty]
        public IEnumerable<FruitModel> FruitModels { get; set; }

        // Begin GET operation code
        public async Task OnGet()
        {
            try
            {
                // Create the HTTP client using the FruitAPI named factory
                var httpClient = _httpClientFactory.CreateClient("FruitAPI");

                // Perform the GET request and store the response. The empty parameter
                // in GetAsync doesn't modify the base address set in the client factory 
                using HttpResponseMessage response = await httpClient.GetAsync("");

                // If the request is successful deserialize the results into the data model
                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    FruitModels = await JsonSerializer.DeserializeAsync<IEnumerable<FruitModel>>(contentStream);

                }

            }
            catch (HttpRequestException ex)
            {
                // Log the exception
                // _logger.LogError(ex, "Error making HTTP request to FruitAPI");

                // Set a user-friendly error message
                TempData["failure"] = "There was an error connecting to the FruitAPI. Please try again later.";

                // Set a default value or take appropriate action
                if (FruitModels == null)
                    FruitModels = Enumerable.Empty<FruitModel>();

                // Return a specific HTTP status code if needed if return type is IActionResult
                // return StatusCode(500, "Internal Server Error");

                // Optionally, re-throw the exception to allow it to propagate up
                // throw;
            }
        }
        // End GET operation code
    }
}

