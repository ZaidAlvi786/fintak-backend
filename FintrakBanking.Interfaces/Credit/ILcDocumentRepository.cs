using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FintrakBanking.ViewModels.credit;
using FintrakBanking.ViewModels;

namespace FintrakBanking.Interfaces.credit
{
    public interface ILcDocumentRepository
    {
        Task<LcDocumentViewModel> GetLcDocument(int id);

        Task<IEnumerable<LcDocumentViewModel>> GetLcDocuments();

        Task<IEnumerable<LcDocumentViewModel>> GetLcDocumentsBylcIssuanceId(int lcIssuanceId);

        Task<bool> AddLcDocument(LcDocumentViewModel model);

        Task<bool> UpdateLcDocument(LcDocumentViewModel model, int id, UserInfo user);

        Task<bool> DeleteLcDocument(int id, UserInfo user);
    }
}
