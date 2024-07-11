using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SendMoneyService.Helpers.Pagins
{
    public static class PaginationHeader
    {
        public static void Add(this HttpResponse response,
           int currentPage, int itemsPerPage, int totalPages, int totalItems)
        {
            var paginationHeader = new
            {
                CurrentPage = currentPage,
                ItemsPerPage = itemsPerPage,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination",
                JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
