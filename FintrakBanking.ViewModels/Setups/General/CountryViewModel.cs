namespace FintrakBanking.ViewModels.Setups.General
{
    public class CountryViewModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
    public class SubsidiariesViewModel
    {
        public int SubsidiaryId { get; set; }
        public string SubsidiaryName { get; set; }
        public int CountryId { get; set; }
        public string Location { get; set; }
        public string UrlLink { get; set; }
        public string CountryName { get; set; }
        public bool isActive { get; set; }
    }
}