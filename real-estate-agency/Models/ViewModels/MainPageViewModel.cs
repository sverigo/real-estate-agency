namespace real_estate_agency.Models.ViewModels
{
    public class MainPageViewModel
    {
        public PagedList.IPagedList<Ad> PagedListModel { get; set; }

        public int Page { get; set; }

        //filter's options
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public string Currency { get; set; }
        public int? MinRoomsCount { get; set; }
        public int? MaxRoomsCount { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public int? MinFloor { get; set; }
        public int? MaxFloor { get; set; }
        public string SortType { get; set; }
        public string RentType { get; set; }
    }
}