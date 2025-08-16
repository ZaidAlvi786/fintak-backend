using System.Threading.Tasks;
using System.Web;

namespace FintrakBanking.AccessSubsediary
{
    public interface ISubsediaryParentController
    {
        Task<object> delete(string countryCode, string url);
        Task<object> get(string countryCode, string url);
        string getCountryCode();
        Task<object> post(string countryCode, string url, object body);
        Task<object> put(string countryCode, string url, object body);
    }
}