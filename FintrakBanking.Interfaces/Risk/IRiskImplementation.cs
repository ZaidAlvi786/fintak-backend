using FintrakBanking.ViewModels.Risk;
using FintrakBanking.ViewModels.Setups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Interfaces.Risk
{
    public interface IRiskImplementation
    {
        IEnumerable<AssessmentResultViewModel> GetAllAssessmentResult(int companyId);

        IEnumerable<AssessmentFormViewModel> GetRiskFormElements(int companyId, int titleId, int? targetId);

        Task<IEnumerable<AssessmentFormViewModel>> SaveFormElements(AssessmentFormSaveViewModel entity);
    }
}
