using FintrakBanking.Entities.DocumentModels;
using FintrakBanking.ViewModels; 
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Setups.General
{
    public interface ICountryRepository
    {
        Task<IEnumerable<CityViewModel>> GetCity();

        Task<IEnumerable<CityViewModel>> GetCity(int Id);

        Task<IEnumerable<LookupViewModel>> GetAllCityClass();

        Task<CityViewModel> GetCityById(int cityId);

        IEnumerable<CityViewModel> GetCityByStateId(int stateId);
        IEnumerable<CityViewModel> GetCityByLGAId(int lgaId);
        Task<int> GetLgaCity(int cityId);

        IEnumerable<LocalGovtViewModel> GetLGAByStateId(int stateId);

        IEnumerable<Object> GetAllCitiesByContryId(int countryId);

        Task<bool> AddCity(CityViewModel entity);

        Task<bool> UpdateCity(CityViewModel entity, int id);

        IEnumerable<StateViewModel> GetState();

        IEnumerable<StateViewModel> GetStateByCountryId(int countryId);

        IEnumerable<CountryViewModel> GetCountry(int countryId);

        Task<IEnumerable<CountryViewModel>> GetCountry();

        IEnumerable<StateViewModel> GetStateByCompanyId(int companyId);

        Task<bool> UpdateState(StateViewModel entity, int stateId);

        Task<bool> AddLocalGovt(LocalGovtViewModel entity);

        Task<bool> UpdateLocalGovt(LocalGovtViewModel entity, int id);

        Task<IEnumerable<LocalGovtViewModel>> GetLocalGovt();

        Task<List<LocalGovtViewModel>> GetLocalGovtByStateId(int stateId);

        LocalGovtViewModel GetLocalGovtById(int id);
        IEnumerable<SubsidiariesViewModel> GetSubsidiaries();
        bool AddSubsidiaries(SubsidiariesViewModel entity);
        bool UpdateSubsidiaries(SubsidiariesViewModel entity, int id);
    }
}