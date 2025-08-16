using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FintrakBanking.Common.CustomException;
using FintrakBanking.ViewModels.Setups.Credit;
using FintrakBanking.APICore.JWTAuth;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.Setups;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using FintrakBanking.APICore.core;
using System.Web;
using FintrakBanking.Common;
using FintrakBanking.Interfaces.Credit;
using FintrakBanking.Common.Enum;
using FintrakBanking.ViewModels.Credit;
using FintrakBanking.ViewModels.WorkFlow;
using FintrakBanking.Common;
namespace FintrakBanking.APICore.Controllers
{
    [RoutePrefix("api/v1/setups")]
    public class ChecklistController : ApiControllerBase
    {
        private IChecklistRepository repo;
        private ILoanApplicationRepository loanApplicationRepo;
        TokenDecryptionHelper token = new TokenDecryptionHelper();

        public ChecklistController(IChecklistRepository _repo, ILoanApplicationRepository _loanApplicationRepo)
        {
            repo = _repo;
            loanApplicationRepo = _loanApplicationRepo;
        }

        #region Checklist Definition
        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-definition")]
        public async Task<HttpResponseMessage> AddChecklistDefinition([FromBody] ChecklistDefinitionViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.AddChecklistDefinition(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-definition/multiple")]
        public async Task<HttpResponseMessage> AddMultipleChecklistDefinition([FromBody] List<ChecklistDefinitionViewModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    item.userBranchId = (short)token.GetBranchId;
                    item.userIPAddress = CommonHelpers.GetUserIP();
                    item.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    item.createdBy = token.GetStaffId;
                    item.companyId = token.GetCompanyId;
                }

                var recordId = await repo.AddMultipleChecklistDefinition(model);
                if (recordId)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
      new { success = true, result = recordId, message = TranslateHelper.get("Checklist Definitions has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Checklist Definition not created") });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
      new { success = false, message = $"{TranslateHelper.get("There was an error creating these records")} {e.Message}" });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-definition/multiple-items")]
        public async Task<HttpResponseMessage> AddMultipleChecklistDefinitionWithMultipleItems([FromBody] ChecklistDefinitionViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var recordId = await repo.AddMultipleChecklistDefinitionWithMultipleItems(model);

                if (recordId)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = recordId, message = TranslateHelper.get("Checklist Definitions have been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Checklist Definitions not created") });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("There was an error creating these records")} {e.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-definition")]
        public HttpResponseMessage GetAllChecklistDefinition()
        {
            try
            {
                var data = repo.GetAllChecklistDefinition();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
       
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-item-simulation/")]
        public async Task<HttpResponseMessage> GetChecklistItemSimulationDetails(int productId)
        {
            try
            {
                var data = await repo.GetChecklistItemSimulationDetails(productId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-definition-checklisttype/")]
        public HttpResponseMessage GetChecklistDefinitionByApprovalLevelCheckListType(int operationId, int checklistTypeId, int? productId, int loanTargetId,int? customerId=null)
        {
            try
            {
                var data = repo.GetChecklistDefinitionByApprovalLevelCheckListType(token.GetStaffId, productId, loanTargetId, operationId, checklistTypeId, customerId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-definition-checklisttype-view/")]
        public HttpResponseMessage GetChecklistDefinitionByApprovalLevelCheckListTypeView(int operationId, int checklistTypeId, int? productId, int loanTargetId,int customerId)
        {
            try
            {
                var data = repo.GetChecklistDefinitionByApprovalLevelCheckListType(token.GetStaffId, productId, loanTargetId, operationId, checklistTypeId, customerId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-type")]
        public async Task<HttpResponseMessage> GetAllChecklistType()
        {
            try
            {
                var data = await repo.GetAllChecklistType();
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-type-byapprovallevel")]
        public HttpResponseMessage GetChecklistTypeByApprovalLevel(int operationId, int productClassProcessId)
        {
            try
            {
                var data = repo.GetChecklistTypeByApprovalLevel(token.GetStaffId, token.GetCompanyId, operationId, productClassProcessId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-detail-valitation/")]
        public async Task<HttpResponseMessage> GetChecklistByCheckListTypeAndTargetId(int targetId, int checklistTypeId, bool isCamChecklist, int? customerId=null)
        {
            try
            {
                var data = await repo.GetChecklistByCheckListTypeAndTargetId(targetId, checklistTypeId, isCamChecklist, customerId);
                if (data == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("checklist-type-mapping/{checklistTypeMappingId}")]
        public async Task<HttpResponseMessage> DeleteChecklistTypeMapping(int checklistTypeMappingId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

               await repo.DeleteChecklistTypeMapping(checklistTypeMappingId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = true, result = checklistTypeMappingId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-definition/{CheckListDefinitionId}")]
        public async Task<HttpResponseMessage> GetAllChecklistDefinitionById(short CheckListDefinitionId)
        {
            try
            {
                var data = await repo.GetAllChecklistDefinitionById(CheckListDefinitionId);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), count = data.Count() });

            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    message = $"There was an error fetching the records {ex.Message}"
                });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-definition/mapped/approval-level/{approvalLevelId}/product/{productId}")]
        public async Task<HttpResponseMessage> GetAllChecklistDefinitionByApprovalLevelId(short approvalLevelId, short productId)
        {
            try
            {
                var data = await repo.GetAllMappedChecklistDefinitionByApprovalLevelAndProduct(approvalLevelId, productId);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data.ToList(), count = data.Count() });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), count = data.Count() });

            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    message = $"There was an error fetching the records {ex.Message}"
                });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-definition/unmapped/approval-level/{approvalLevelId}/product/{productId}")]
        public async Task<HttpResponseMessage> GetUnmappedChecklistDefintionToApprovalLevel(short approvalLevelId, short productId)
        {
            try
            {
                var data = await repo.GetAllUnmappedChecklistItemsToApprovalLevelAndProduct(approvalLevelId, productId);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        success = true,
                        result = data.ToList(),
                        count = data.Count()
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    result = data.ToList(),
                    count = data.Count()
                });

            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    message = $"{TranslateHelper.get("There was an error fetching the records")} {ex.Message}"
                });
            }

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("checklist-definition/{CheckListDefinitionId}")]
        public async Task<HttpResponseMessage> UpdateChecklistDefinition(short CheckListDefinitionId, [FromBody] ChecklistDefinitionViewModel model)
        {

            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.UpdateChecklistDefinition(CheckListDefinitionId, model);

                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                 new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("checklist-definition/{CheckListDefinitionId}")]
        public async Task<HttpResponseMessage> DeleteChecklistDefinition(short CheckListDefinitionId)
        {

            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

                await repo.DeleteChecklistDefinition(CheckListDefinitionId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = true, result = CheckListDefinitionId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        #endregion

        #region Checklist Detail
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-details-targetid/")]
        public async Task<HttpResponseMessage> GetChecklistDetailsById(int targetId)
        {
            try
            {
                var data = await repo.GetChecklistByTargetId(targetId);

                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, result = data.ToList(), count = data.Count() });

            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    success = false,
                    message = $"{TranslateHelper.get("There was an error fetching the records")} {ex.Message}"
                });
            }

        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-detail")]
        public async Task<HttpResponseMessage> AddChecklistDetail([FromBody] ChecklistDetailViewModel model)
        {
            try
            {
                string createUpdate = "";
                if (model.checklistId != 0 || model.checklistId > 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if (await repo.ValidateChecklistDetailEntry(model.checkListDefinitionId, model.targetId))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                                new { success = false, message = TranslateHelper.get("This checklist item is checked already") });
                    }
                }
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.AddChecklistDetail(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = true, result = data, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-detail-multiple")]
        public async Task<HttpResponseMessage> AddChecklistDetailMultiple([FromBody] List<ChecklistDetailViewModel> model)
        {
            try
            {
                if (model.Count <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("Please select a checklist to continue") });
                }
                var data = await repo.AddMultipleChecklistDetails(model, token.GetStaffId, (short)token.GetBranchId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = true, result = data, message = TranslateHelper.get("The Checklist has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = TranslateHelper.get("There was an error creating this Checklist") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }

        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-detail")]
        public async Task<HttpResponseMessage> GetAllChecklistDetail()
        {
            try
            {
                var data = await repo.GetAllChecklistDetail();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-detail/")]
        public async Task<HttpResponseMessage> GetAllChecklistDetailByProductId(int targetId)
        {
            try
            {
                // var targetTypeId = (int)CheckListTargetTypeEnum.Loan;
                var data = await repo.GetAllMappedChecklistDefinitionByProductId(targetId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-detail/target-type/{targetTypeId}/product/{productId}")]
        public async Task<HttpResponseMessage> GetAllChecklistDetailByProductAndTargetType(int targetTypeId, int productId)
        {
            try
            {
                var data = await repo.GetAllChecklistDetailByProductAndTargetId(targetTypeId, productId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-detail/checklist-definition/{checklistDefinitionId}")]
        public async Task<HttpResponseMessage> GetAllChecklistDetailByChecklistDefinition(int checklistDefinitionId)
        {
            try
            {
                var data = await repo.GetAllChecklistDetailByChecklistDefinitionId(checklistDefinitionId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-detail/{ChecklistId}")]
        public async Task<HttpResponseMessage> GetAllChecklistById(int ChecklistId)
        {
            try
            {
                var data = await repo.GetAllChecklistDetailById(ChecklistId);
                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
               new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("checklist-detail/{ChecklistId}")]
        public async Task<HttpResponseMessage> UpdateChecklistDetail(int ChecklistId, [FromBody] ChecklistDetailViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.UpdateChecklistDetail(ChecklistId, model);

                if (data)
                {

                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });

                }
                return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("checklist-detail/{ChecklistId}")]
        public async Task<HttpResponseMessage> DeleteLoanChecklist(int ChecklistId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

                 await repo.DeleteChecklistDetail(ChecklistId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = true, result = ChecklistId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        #endregion

        #region CheckList Items
        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-item")]
        public async Task<HttpResponseMessage> AddChecklistItem([FromBody] ChecklistItemViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.AddChecklistItem(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
             new { success = true, result = data, message = TranslateHelper.get("The record has been created successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }



        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-item/multiple")]
        public async Task<HttpResponseMessage> AddMultipleChecklistItem([FromBody] List<ChecklistItemViewModel> model)
        {
            try
            {
                var recordId = await repo.AddMultipleChecklistItem(model);
                if (recordId)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
              new { success = true, result = recordId, message = TranslateHelper.get("Checklist items has been created successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = TranslateHelper.get("Checklist items not created") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = TranslateHelper.get(ex.Message) });
            }
        }


        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-item")]
        public async Task<HttpResponseMessage> GetAllChecklistItem()
        {
            try
            {
                var data = await repo.GetAllChecklistItem();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-item/{ChecklistId}")]
        public async Task<HttpResponseMessage> GetAllChecklistItemById(int CheckListItemId)
        {
            try
            {
                var data =  await repo.GetAllChecklistItemById(CheckListItemId);
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = true, result = data, count = 1 });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("checklist-item/{CheckListItemId}")]
        public async Task<HttpResponseMessage> UpdateChecklistItem(int CheckListItemId, [FromBody] ChecklistItemViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.UpdateChecklistItem(CheckListItemId, model);

                if (data)
                {

                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });

                }
                return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("checklist-item/{CheckListItemId}")]
        public async Task<HttpResponseMessage> DeleteChecklistItem(int CheckListItemId)
        {
            try
            {

                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

                await repo.DeleteChecklistItem(CheckListItemId, user);

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = true, result = CheckListItemId, message = TranslateHelper.get("Record has been deleted successfully") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = TranslateHelper.get(ex.Message) });
            }


        }
        #endregion

        #region CheckList Select List
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-status")]
        public async Task<HttpResponseMessage> GetAllChecklistStatus()
        {
            try
            {
                var data = await repo.GetAllChecklistStatus();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-response-type")]
        public async Task<HttpResponseMessage> GetAllChecklistResponseType()
        {
            try
            {
                var data = await repo.GetAllChecklistResponseType();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-target-type")]
        public async Task<HttpResponseMessage> GetAllChecklistTargetType()
        {
            try
            {
                var data = await repo.GetAllChecklistTargetType();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        #endregion

        #region Loan Application CheckList
        [HttpGet]
        [ClaimsAuthorization]
        [Route("loan-application-checklist")]
        public async Task<HttpResponseMessage> GetLoanApplicationsAwaitingCheckList()
        {
            try
            {
                var data = await loanApplicationRepo.GetLoanApplicationsAwaitingCheckList(token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("validate-checklist-details")]
        public async Task<HttpResponseMessage> ValidateChecklistDetail([FromBody] List<ValidateChecklistDetailViewModel> model)
        {
            try
            {
                var data = await repo.ValidateChecklistDetail(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, result = data, message = TranslateHelper.get("The record has been updated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("There was an error updating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {e.Message}" });
            }
        }
        [HttpPut]
        [ClaimsAuthorization]
        [Route("validate-condition-precedence")]
        public async Task<HttpResponseMessage> ValidateConditionPrecedentDetail([FromBody] ConditionPrecedentViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = CommonHelpers.GetUserIP();
                if (await repo.ValidateChecklistForDefferalOrWaival(model))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = false, message = TranslateHelper.get("The request for deferral/waival of this item is still being processed. ") });
                }
                var data = await repo.ValidateConditionPrecedentDetail(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                   new { success = true, message = TranslateHelper.get("The record has been Validated successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("There was an error validating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("There was an error validating this record")  + " "+ e.Message });
            }

        }
        #endregion

        #region Condition Precedence Checklist
        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-prededence-checklist")]
        public async Task<HttpResponseMessage> GetConditionPrecedenceChecklist(int loanApplicationId, bool isAvailment)
        {
            try
            {
                var data = await repo.GetConditionPrecedenceChecklist(loanApplicationId, isAvailment);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("condition-prededence-checklist-status")]
        public async Task<HttpResponseMessage> GetConditionPrecedenceChecklistStatus(int loanApplicationId, bool isAvailment)
        {
            try
            {
                var data = await repo.GetConditionPrecedenceChecklistStatus(loanApplicationId, isAvailment, token.GetStaffId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("validate-precedence-checklist-completed")]
        public async Task<HttpResponseMessage> ValidatePrecedenceChecklistCompleted(int loanApplicationId)
        {
            try
            {
                var data = await repo.ValidatePrecedenceChecklistCompleted(loanApplicationId);
                if (data == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });     
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-validate-precedence-checklist-completed/{applicationId}")]
        public async Task<HttpResponseMessage> LMSValidatePrecedenceChecklistCompleted(int applicationId)
        {
            try
            {
                var data = await repo.LMSValidatePrecedenceChecklistCompleted(applicationId);
                if (data == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-deferred-documents-awaiting-approval")]
        public async Task<HttpResponseMessage> GetDeferralDocumentsAwaitingApproval()
        {
            try
            {
                var data = await repo.GetDeferralDocumentsAwaitingApproval(token.GetStaffId, token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("get-deferred-extensions-awaiting-approval")]
        public async Task<HttpResponseMessage> GetDeferralExtensionsAwaitingApproval()
        {
            try
            {
                var data = await repo.GetDeferralExtensionsAwaitingApproval(token.GetStaffId, token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-deferred-document-for-approval")]
        public HttpResponseMessage SubmitDeferralDocumentForApproval([FromBody] ConditionPrecedentViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var response = repo.SubmitDeferralDocumentForApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = response, message = repo.ResponseMessage(response, "DEFERRED DOCUMENT PROVISION") });
                //return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            { 
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error saving this record")  + " "+ ex.Message });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-deferred-extension-for-approval")]
        public async Task<HttpResponseMessage> SubmitDeferralExtensionForApproval([FromBody] ConditionPrecedentViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = HttpContext.Current.Request.UserHostAddress;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var res = await repo.SubmitDeferralExtensionForApproval(model);
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = res });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("There was an error saving this record")}. {TranslateHelper.get("Error")} - {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("deferred-checklist-awaiting-approval")]
        public async Task<HttpResponseMessage> GetChecklistAwaitingApproval()
        {
            try
            {
                var data = await repo.GetChecklistAwaitingApproval(token.GetStaffId, token.GetCompanyId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("deferred-checklist")]
        public async Task<HttpResponseMessage> GetAllDeferralChecklist()
        {
            try
            {
                var data = await repo.GetAllDeferralChecklist();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpGet]
        [Route("get-deferral-approval-trail/targetId/{targetId}/operationId/{operationId}")]
        public async Task<HttpResponseMessage> GetDeferralApprovalTrail(int targetId, int operationId)
        {
            var data = await repo.GetDeferralApprovalTrail(targetId, operationId);
            if (data.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = TranslateHelper.get("No Record Found") });
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("validate-checklist/{applicationId}")]
        public async Task<HttpResponseMessage> ValidateChecklist(int applicationId)
        {
                var data = await repo.ValidateChecklist(applicationId);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = "An error occurred" });
           
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("deferred-checklist-byContionId/")]
        public async Task<HttpResponseMessage> GetAllDeferralChecklist(int conditionId)
        {
            try
            {
                var data = await repo.GetDeferralChecklistByConditionId(conditionId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("update-loan-condition-precedence-status")]
        public async Task<HttpResponseMessage> UpdateLoanConditionPrecedenceStatus([FromBody] ConditionPrecedentViewModel model)
        {
            if (model.conditionId == 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get("Please select a checklist to continue") });
            }
            if (model.deferedDate < DateTime.Now)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get("Deferred date cannot be less than today's date") });
            }
            model.userBranchId = (short)token.GetBranchId;
            model.companyId = token.GetCompanyId;
            model.createdBy = token.GetStaffId;
            model.applicationUrl = HttpContext.Current.Request.Path;
            model.userIPAddress = CommonHelpers.GetUserIP();

            var data = await repo.UpdateLoanConditionPrecedenceStatus(model);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = true, message = TranslateHelper.get("The Checklist Status has been updated successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK,
        new { success = false, message = TranslateHelper.get("There was an error updating this Checklist") });

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("submit-loan-condition-precedence-status")]
        public async Task<HttpResponseMessage> ForwardChecklistForApproval([FromBody] List<ConditionPrecedentViewModel> models)
        {
            foreach (var model in models)
            {
                if (model.conditionId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("Please select a checklist to continue") });
                }
                if (model.deferedDate < DateTime.Now)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("Deferred date cannot be less than today's date") });
                }
                model.userBranchId = (short)token.GetBranchId;
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = CommonHelpers.GetUserIP();
            }
            

            var data = await repo.ForwardChecklistForApproval(models);
            if (data)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = true, message = TranslateHelper.get("The Checklist Status has been submitted successfully") });
            }

            return Request.CreateResponse(HttpStatusCode.OK,
        new { success = false, message = TranslateHelper.get("There was an error submitting this Checklist") });

        }


        [HttpPost]
        [ClaimsAuthorization]
        [Route("extend-checklist-deferral-date")]
        public async Task<HttpResponseMessage> ExtendChecklistDeferralDate([FromBody] ConditionPrecedentViewModel model)
        {
            try
            {
                if (model.conditionId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("Please select a checklist to continue") });
                }

                if ( await repo.ValidateDeferralDateExpiration((int)model.conditionId))
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("Extention terminated, deferral not expired") });
                }

                model.userBranchId = (short)token.GetBranchId;
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = CommonHelpers.GetUserIP();

                var data = await repo.ExtendChecklistDeferralDate(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = true, message = TranslateHelper.get("The Deferral Date has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = "There was an error extending checklist deferral" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("update-provided-checklist")]
        public async Task<HttpResponseMessage> UpdateProvidedChecklist([FromBody] ConditionPrecedentViewModel model)
        {
            try
            {
                if (model.conditionId == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = false, message = TranslateHelper.get("Please select a checklist to continue") });
                }
                model.userBranchId = (short)token.GetBranchId;
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = CommonHelpers.GetUserIP();

                var data = await repo.UpdateProvidedChecklist(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = true, message = TranslateHelper.get("The Deferral Date has been updated successfully") });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = TranslateHelper.get("There was an error extending checklist deferral") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-approval")]
        public HttpResponseMessage GoForApproval([FromBody]ApprovalViewModel entity)
        {
            try
            {
                entity.BranchId = token.GetBranchId;
                entity.companyId = token.GetCompanyId;
                entity.staffId = token.GetStaffId;
                entity.applicationUrl = HttpContext.Current.Request.Path;
                entity.userIPAddress = Request.RequestUri.Host;
                //entity.operationId = (int)OperationsEnum.DefferedChecklistApproval;
                var data = repo.GoForApproval(entity);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, message = data.responseMessage });

                //if (data == 1)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK,
                //        new { success = true, message = "Record has been approved successfully" });
                //}
                //else if (data == 2)
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK,
                //        new { success = true, message = "Record has been disapproved successfully." });
                //}
                //else
                //{
                //    return Request.CreateResponse(HttpStatusCode.OK,
                //    new { success = true, message = "Operation successful, request has been routed to the next approving office" });
                //}
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("There was an error submitting this record") + " "+ e.Message });
            }
        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("delete-loan-condition-checkstatus/{conditionId}/{isLMSChecklist}")]
        public async Task<HttpResponseMessage> DeleteLoanConditionPrecedenceStatus(int conditionId, bool isLMSChecklist)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

            var data =  await  repo.DeleteLoanConditionPrecedenceStatus(conditionId, isLMSChecklist, user);
                if(data == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                              new { success = true, result = conditionId, message = TranslateHelper.get("Record has been deleted successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false,  message = "Error deleting record" });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             new { success = false, message = TranslateHelper.get(ex.Message) });
            }

        }
        #endregion

        #region Checklist Type Mapping
        [HttpGet]
        [ClaimsAuthorization]
        [Route("mapped-checklist-type")]
        public async Task<HttpResponseMessage> GetAllChecklistTypeMapping()
        {
            try
            {
                var data = await repo.GetAllChecklistTypeMapping();
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("checklist-type-mapping")]
        public async Task<HttpResponseMessage> AddChecklistTypeMapping([FromBody] CheckListTypeMappingViewModel model)
        {
            try
            {
                string createUpdate = "";
                if (model.checklistTypeMappingId != 0 || model.checklistTypeMappingId < 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if (await repo.ValidateChecklistTypeMapping(model.checklistTypeId, model.approvalLevelId))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                     new { success = false, message = TranslateHelper.get("This Checklist Type is already mapped with the selected Approval Level") });
                    }
                }

                model.userBranchId = (short)token.GetBranchId;
                model.companyId = token.GetCompanyId;
                model.createdBy = token.GetStaffId;
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.userIPAddress = CommonHelpers.GetUserIP();

                var data = await repo.AddChecklistTypeMapping(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = true, message = $"{TranslateHelper.get("The record has been")} {createUpdate} {TranslateHelper.get("successfully")}" });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error ")}  {createUpdate}  {TranslateHelper.get(" this record.")}" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }
        #endregion

        #region EGS Checklist
        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-type")]
        public async Task<HttpResponseMessage> GetESGType()
        {
            try
            {
                var data =  await repo.GetESGType();
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-class")]
        public async Task<HttpResponseMessage> GetESGClass()
        {
            try
            {
                var data = await repo.GetESGClass();
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-categories")]
        public async Task<HttpResponseMessage> GetESGCategory()
        {
            try
            {
                var data = await repo.GetESGCategory();
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-sub-categories")]
        public async Task<HttpResponseMessage> GetESGSubCategory(int categoryId)
        {
            try
            {
                var data = await repo.GetESGSubCategory(categoryId);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-checklist-definition")]
        public async Task<HttpResponseMessage> GetESGChecklistDefinition()
        {
            try
            {
                var data = await repo.GetESGChecklistDefinition();
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-checklist-definition/delete/{esgChecklistDefinitionId}")]
        public async Task<HttpResponseMessage> DeleteESGChecklistDefinition(int esgChecklistDefinitionId)
        {
            try
            {
                var data = await repo.DeleteESGChecklistDefinition(esgChecklistDefinitionId, token.GetStaffId);
                if (data == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-checklist-status")]
        public async Task<HttpResponseMessage> GetESGChecklistDefinition(int loanApplicationId)
        {
            try
            {
                var data = await repo.GetESGChecklistStatus(loanApplicationId);
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            { 
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("esg-checklist-calculate")]
        public async Task<HttpResponseMessage> CalculateESGChecklistSummary([FromBody] List<ESGChecklistDetailViewModel> model)
        {
            try
            {
                var data = await repo.CalculateESGChecklistSummary(model);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error calculating the summary {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("green-rating-calculate")]
        public async Task<HttpResponseMessage> CalculateGreenRatingSummary([FromBody] List<ESGChecklistDetailViewModel> model)
        {
            try
            {
                var data = await repo.CalculateGreenRatingSummary(model);
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error calculating the summary {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("esg-checklist-detail")]
        public async Task<HttpResponseMessage> GetESGChecklistDetail(int loanApplicationId)
        {
            try
            {
                var data = await repo.GetESGChecklistDetail(loanApplicationId);
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }


        #region GreenRating
        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-scores/{checkListTypeId}")]
        public async Task<HttpResponseMessage> GetCheckListScores(int checkListTypeId)
        {
            try
            {
                var data = await repo.GetCheckListScores(checkListTypeId);
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-item/{checkListTypeId}")]
        public async Task<HttpResponseMessage> GetAllChecklistItemBycheckListTypeId(int checkListTypeId)
        {
            try
            {
                var data = await repo.GetAllChecklistItemBycheckListTypeId(checkListTypeId);
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"{TranslateHelper.get("There was an error updating this record")} {ex.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("green-rating-definition")]
        public async Task<HttpResponseMessage> GetGreenRatingDefinition()
        {
            try
            {
                var data = await repo.GetGreenRatingDefinition();
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("green-rating-definition")]
        public async Task<HttpResponseMessage> AddGreenRatingChecklistDefinition([FromBody] List<ESGChecklistDefinitionViewModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    item.userBranchId = (short)token.GetBranchId;
                    item.userIPAddress = CommonHelpers.GetUserIP();
                    item.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    item.createdBy = token.GetStaffId;
                    item.companyId = token.GetCompanyId;
                }

                var data = await repo.AddGreenRatingDefinition(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been created successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }

        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("green-rating-status")]
        public HttpResponseMessage GetGreenRatingDefinition(int loanApplicationId)
        {
            try
            {
                var data = repo.GetGreenRatingStatus(loanApplicationId);
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("green-rating-detail")]
        public async Task<HttpResponseMessage> GetGreenRatingDetail(int loanApplicationId)
        {
            try
            {
                var data = await repo.GetGreenRatingDetail(loanApplicationId);
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("green-rating-detail")]
        public async Task<HttpResponseMessage> AddGreenRatingDetail([FromBody] List<ESGChecklistDetailViewModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    item.userBranchId = (short)token.GetBranchId;
                    item.userIPAddress = CommonHelpers.GetUserIP();
                    item.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    item.createdBy = token.GetStaffId;
                    item.companyId = token.GetCompanyId;
                }

                var data = await repo.AddGreenRatingDetail(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been saved successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }

        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("green-rating-summary")]
        public async Task<HttpResponseMessage> AddGreenRatingSummary([FromBody] ESGChecklistSummaryViewModel model)
        {
            try
            {

                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.AddGreenRatingSummary(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been added successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }
        }

        [HttpGet]
        [ClaimsAuthorization]
        [Route("green-rating-definition/delete/{esgChecklistDefinitionId}")]
        public async Task<HttpResponseMessage> DeleteGreenRatingDefinition(int esgChecklistDefinitionId)
        {
            try
            {
                var data = await repo.DeleteGreenRatingDefinition(esgChecklistDefinitionId, token.GetStaffId);
                if (data == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {

                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }
        #endregion GreenRating

        //[HttpGet]
        //[ClaimsAuthorization]
        //[Route("esg-checklist-detail")]
        //public HttpResponseMessage GetESGChecklistSummary(int loanApplicationId)
        //{
        //    try
        //    {
        //        var data = repo.GetESGChecklistDetail(loanApplicationId);
        //        if (data.Count() > 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.OK,
        //                new { success = true, result = data });
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //          new { success = false, message = TranslateHelper.get("No Record Found") });
        //    }
        //    catch (SecureException ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK,
        //      new { success = false, message = $"There was an error fetching this record {ex.Message}" });
        //    }
        //}

        [HttpGet]
        [ClaimsAuthorization]
        [Route("checklist-facility-detail")]
        public async Task<HttpResponseMessage> GetAllFacilityDetails(int loanApplicationId)
        {
            try
            {
                var data = await repo.GetAllFacilityDetails(loanApplicationId, token.GetCompanyId);
                if (data.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, result = data });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                  new { success = false, message = TranslateHelper.get("No Record Found") });
            }
            catch (SecureException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
              new { success = false, message = $"There was an error fetching this record {ex.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("esg-category")]
        public async Task<HttpResponseMessage> AddESGCategory([FromBody] ESGChecklistDefinitionViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.AddESGCategory(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been created successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("esg-subcategory")]
        public async Task<HttpResponseMessage> AddESGSubCategory([FromBody] ESGChecklistDefinitionViewModel model)
        {
            try
            {
                    model.userBranchId = (short)token.GetBranchId;
                    model.userIPAddress = CommonHelpers.GetUserIP();
                    model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    model.createdBy = token.GetStaffId;
                    model.companyId = token.GetCompanyId;

                var data = await repo.AddESGSubCategory(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been created successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("esg-category")]
        public async Task<HttpResponseMessage> UpdateESGCategory([FromBody] ESGChecklistDefinitionViewModel model)
        {
            try
            {
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await
                    repo.UpdateESGCategory(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been updated successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error updating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error updating these records {e.Message}" });
            }

        }

        [HttpPut]
        [ClaimsAuthorization]
        [Route("esg-subcategory")]
        public async Task<HttpResponseMessage> UpdateESGSubCategory([FromBody] ESGChecklistDefinitionViewModel model)
        {
            try
            {
                    model.userBranchId = (short)token.GetBranchId;
                    model.userIPAddress = CommonHelpers.GetUserIP();
                    model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    model.createdBy = token.GetStaffId;
                    model.companyId = token.GetCompanyId;

                var data = await repo.UpdateESGSubCategory(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been updating successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error updating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error updating these records {e.Message}" });
            }

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("esg-category/{esgCategoryId}")]
        public async Task<HttpResponseMessage> DeleteESGCategory(int esgCategoryId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

                var data = await repo.DeleteESGCategory(esgCategoryId, user);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Record has been deleted successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error deleting record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error deleting these records {e.Message}" });
            }

        }

        [HttpDelete]
        [ClaimsAuthorization]
        [Route("esg-subcategory/{esgSubcategoryId}")]
        public async Task<HttpResponseMessage> DeleteESGSubcategory(int esgSubcategoryId)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    BranchId = token.GetBranchId,
                    companyId = token.GetCompanyId,
                    staffId = token.GetStaffId,
                    applicationUrl = HttpContext.Current.Request.Path,
                    userIPAddress = CommonHelpers.GetUserIP()
                };

                var data = await repo.DeleteESGSubcategory(esgSubcategoryId, user);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = TranslateHelper.get("Record has been deleted successfully") });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error deleting record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error deleting these records {e.Message}" });
            }

        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("esg-checklist-definition")]
        public async Task<HttpResponseMessage> AddESGChecklistDefinition([FromBody] List<ESGChecklistDefinitionViewModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    item.userBranchId = (short)token.GetBranchId;
                    item.userIPAddress = CommonHelpers.GetUserIP();
                    item.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    item.createdBy = token.GetStaffId;
                    item.companyId = token.GetCompanyId;
                }

                var data = await repo.AddESGChecklistDefinition(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been created successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }

        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("esg-checklist-detail")]
        public async Task<HttpResponseMessage> AddESGChecklistDetail([FromBody] List<ESGChecklistDetailViewModel> model)
        {
            try
            {
                foreach (var item in model)
                {
                    item.userBranchId = (short)token.GetBranchId;
                    item.userIPAddress = CommonHelpers.GetUserIP();
                    item.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                    item.createdBy = token.GetStaffId;
                    item.companyId = token.GetCompanyId;
                }

                var data = await repo.AddESGChecklistDetail(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been saved successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }

            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }

        }
        [HttpPost]
        [ClaimsAuthorization]
        [Route("esg-checklist-summary")]
        public async Task<HttpResponseMessage> AddESGChecklistSummary([FromBody] ESGChecklistSummaryViewModel model)
        {
            try
            {

                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.UserHostAddress;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;
             
                var data = await repo.AddESGChecklistSummary(model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = true, message = "Record has been added successfully" });
                }
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { success = false, message = "Error creating record" });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                        new { success = false, message = $"There was an error creating these records {e.Message}" });
            }
        }
        #endregion


        [HttpGet]
        [ClaimsAuthorization]
        [Route("regulatory-checklist-automapping")]
        public async Task<HttpResponseMessage> RegulatoryChecklistAutomapping(int customerId, int targetId)
        {
            try
            {
                ChecklistDetailViewModel model = new ChecklistDetailViewModel();
                model.targetId = targetId;
                string createUpdate = "";
                if (model.checklistId != 0 || model.checklistId > 0)
                {
                    createUpdate = "updated";
                }
                else
                {
                    createUpdate = "created";
                    if ( await repo.ValidateChecklistDetailEntry(model.checkListDefinitionId, model.targetId))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK,
                                new { success = false, message = TranslateHelper.get("This checklist item is checked already") });
                    }
                }
                model.userBranchId = (short)token.GetBranchId;
                model.userIPAddress = CommonHelpers.GetUserIP();
                model.applicationUrl = HttpContext.Current.Request.Path;
                model.createdBy = token.GetStaffId;
                model.companyId = token.GetCompanyId;

                var data = await repo.RegulatoryChecklistAutomapping(customerId, model);
                if (data)
                {
                    return Request.CreateResponse(HttpStatusCode.OK,
                 new { success = true, result = data, message = $"{TranslateHelper.get("The record has been ")} {createUpdate} {TranslateHelper.get(" successfully")}" });
                }

                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = TranslateHelper.get("There was an error creating this record") });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
            new { success = false, message = $"{TranslateHelper.get("There was an error creating this record")} {e.Message}" });
            }
        }

        #region LMS Condition Precedence Checklist
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-condition-prededence-checklist")]
        public async Task<HttpResponseMessage> GetLMSConditionPrecedenceChecklist(int loanReviewApplicationId)
        {
            try
            {
                var data = await repo.GetLMSConditionPrecedenceChecklist(loanReviewApplicationId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }
        [HttpGet]
        [ClaimsAuthorization]
        [Route("lms-condition-prededence-checklist-status")]
        public async Task<HttpResponseMessage> GetLMSConditionPrecedenceChecklistStatus(int loanReviewApplicationId)
        {
            try
            {
                var data = await repo.GetLMSConditionPrecedenceChecklistStatus(loanReviewApplicationId);
                if (!data.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("No Record Found") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data, count = data.Count() });
            }
            catch (SecureException e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            }
        }

        [HttpPost]
        [ClaimsAuthorization]
        [Route("populate-loanapplication-checklist")]
        public async Task<HttpResponseMessage> PopulateLoanApplicationChecklist(CheckListTargetTypeViewModel model)
        {
            //try
            //{   
                var data = await repo.PopulateLoanApplicationChecklist(model.loanApplicationId,token.GetStaffId,token.GetCompanyId,model.productClassProcessId);

                if (data == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = TranslateHelper.get("Error populating checklist") });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { success = true, result = data });
            //}
            //catch (SecureException e)
            //{
            //    return Request.CreateResponse(HttpStatusCode.OK, new { success = false, message = $"{TranslateHelper.get("Error")}: {e.Message}" });
            //}
        }
        #endregion
        
    }
}