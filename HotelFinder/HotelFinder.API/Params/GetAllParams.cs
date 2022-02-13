using HotelFinder.API.Enums;

namespace HotelFinder.API.Params
{
    public class GetAllParams
    {
        public string SearchVal { get; set; }
        public string[] FilterFields { get; set; }
        public string SortField { get; set; }
        public SortType SortType { get; set; }

    }
}
