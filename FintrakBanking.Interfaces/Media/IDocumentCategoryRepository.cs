using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Credit;

namespace FintrakBanking.Interfaces.Media
{
    public interface IDocumentCategoryRepository
    {
        DocumentCategoryViewModel GetDocumentCategory(int id);
        Task<IEnumerable<DocumentCategoryViewModel>> GetDocumentCategorys();

        Task<bool> AddDocumentCategory(DocumentCategoryViewModel model);

        Task<bool> UpdateDocumentCategory(DocumentCategoryViewModel model, int id, UserInfo user);

        Task<bool> DeleteDocumentCategory(int id, UserInfo user);
    }
}
