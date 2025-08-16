using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;

namespace FintrakBanking.Interfaces.Media
{
    public interface IDocumentCategoryTypeRepository
    {
        DocumentCategoryTypeViewModel GetDocumentCategoryType(int id);

        IEnumerable<DocumentCategoryTypeViewModel> GetDocumentCategoryTypes();

        Task<bool> AddDocumentCategoryType(DocumentCategoryTypeViewModel model);

        Task<bool> UpdateDocumentCategoryType(DocumentCategoryTypeViewModel model, int id, UserInfo user);

        Task<bool> DeleteDocumentCategoryType(int id, UserInfo user);
    }
}
