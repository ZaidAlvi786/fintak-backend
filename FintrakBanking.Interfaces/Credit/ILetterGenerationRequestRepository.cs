using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;

namespace FintrakBanking.Interfaces.Credit
{
    public interface ILetterGenerationRequestRepository
    {
        Task<LetterGenerationRequestViewModel> GetLetterGenerationRequest(int id);
        Task<IEnumerable<LetterGenerationRequestViewModel>> GetLetterGenerationRequests(int staffId);
        Task<IEnumerable<AuthorisedSignatoryViewModel>> GetLetterGenerationSignatory(int requestId);
        Task<IEnumerable<LetterGenerationRequestViewModel>> GetLetterGenerationRequestsForApproval(int staffId);
        Task<LetterGenerationRequestViewModel> AddLetterGenerationRequest(LetterGenerationRequestViewModel model);
        LetterGenerationRequestViewModel UpdateLetterGenerationRequest(LetterGenerationRequestViewModel model, int id, UserInfo user);
        bool DeleteLetterGenerationRequest(int id, UserInfo user);
        Task<List<CamsolLoanDocumentViewModel>> GetCamsolLoansByCustomerCode(string customerName, string customerCode);
        Task<string> GetCamsolLoanDocument(int typeId, LetterGenerationRequestViewModel model);
        IEnumerable<LetterGenerationRequestViewModel> GetLetterGenerationCompleted();
        IEnumerable<LetterGenerationRequestViewModel> Search(string searchString);
    
    }
}
