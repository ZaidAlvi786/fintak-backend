using FintrakBanking.Common.Enum;
using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using FintrakBanking.ViewModels.Setups.Finance;
using FintrakBanking.Common;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FintrakBanking.Repositories.Setups.General
{

    public class ProductFeeRepository : IProductFeeRepository
    {
        private FinTrakBankingContext context;
        private IGeneralSetupRepository genSetup;
        private IAuditTrailRepository auditTrail;

        public ProductFeeRepository(FinTrakBankingContext _context,
                                        IGeneralSetupRepository genSetup, IGeneralSetupRepository _genSetup,
                                        IAuditTrailRepository _auditTrail)
        {
            this.context = _context;
            this.genSetup = _genSetup;
            auditTrail = _auditTrail;
        }

        private bool SaveAll()
        {
            return this.context.SaveChanges() > 0;
        }

        public async Task<int> AddMultipleProductFee(List<ProductFeeViewModel> productFees)
        {
            if (productFees.Count <= 0)
                return -1;

            foreach (ProductFeeViewModel item in productFees)
            {
                await AddTempProductFee(item);
            }

            return 1;
        }

        public async Task<int> AddProductFee(ProductFeeViewModel productFee)
        {
            var existingApprovedProduct = context.TBL_PRODUCT.Where(x => x.PRODUCTID == productFee.productId);
            var dataExist = await this.context.TBL_PRODUCT_CHARGE_FEE.FirstOrDefaultAsync(x => x.PRODUCTID == productFee.productId && x.CHARGEFEEID == productFee.feeId && x.DELETED == true); // .Find(accountId);

            var productFeeEntity = dataExist;

            if (dataExist == null)
            {
                productFeeEntity = new TBL_PRODUCT_CHARGE_FEE()
                {
                    PRODUCTID = productFee.productId,
                    CHARGEFEEID = productFee.feeId,
                    COMPANYID = productFee.companyId,

                    RATEVALUE = productFee.rateValue,
                    DEPENDENTAMOUNT = productFee.dependentAmount,

                    CREATEDBY = productFee.createdBy,
                    DATETIMECREATED = genSetup.GetApplicationDate(),
                    DELETED = false
                };

                this.context.TBL_PRODUCT_CHARGE_FEE.Add(productFeeEntity);
                // Audit Section ---------------------------
                var product =  context.TBL_PRODUCT.FirstOrDefault(x => x.PRODUCTID == productFee.productId)?.PRODUCTNAME;
                var audit = new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.ProductFeeAdded,
                    STAFFID = productFee.createdBy,
                    BRANCHID = (short)productFee.userBranchId,
                    DETAIL = $"{TranslateHelper.get("Added Product Fee:")} { productFee.feeName } {TranslateHelper.get("to product")} {product} {TranslateHelper.get("with amount")} {productFee.rateValue} ",
                    IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                    URL = productFee.applicationUrl,
                    APPLICATIONDATE = genSetup.GetApplicationDate(),
                    SYSTEMDATETIME = DateTime.Now,
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName(),
                };

                this.auditTrail.AddAuditTrail(audit);

                //end of Audit section -------------------------------
            }
            else
            {
                productFeeEntity.RATEVALUE = productFee.rateValue;
                productFeeEntity.DEPENDENTAMOUNT = productFee.dependentAmount;

                productFeeEntity.DELETED = false;
            }

            var status = this.SaveAll();

            if (status)
                return productFeeEntity.PRODUCTFEEID;
            return -1;
        }

        public async Task<int> AddTempProductFee(ProductFeeViewModel productFee)
        {
            var dataExist = await this.context.TBL_TEMP_PRODUCT_CHARGE_FEE.FirstOrDefaultAsync(x => x.TEMP_PRODUCTID == productFee.productId
                                                                && x.PRODUCTFEEID == productFee.feeId
                                                                && x.DELETED == true); // .Find(accountId);

            var tempProductFeeEntity = dataExist;

            if (dataExist == null)
            {
                tempProductFeeEntity = new TBL_TEMP_PRODUCT_CHARGE_FEE()
                {
                    //ProductId = productFee.productId,
                    CHARGEFEEID = productFee.feeId,
                    COMPANYID = productFee.companyId,

                    RATEVALUE = productFee.rateValue,
                    DEPENDENTAMOUNT = productFee.dependentAmount,

                    CREATEDBY = productFee.createdBy,
                    DATETIMECREATED = genSetup.GetApplicationDate(),
                    DELETED = false,
                };

                var existingProductApprovalLog = await context.TBL_TEMP_PRODUCT.FindAsync(productFee.productId);
                var productData = await context.TBL_TEMP_PRODUCT.FindAsync(productFee.productId);

                if (existingProductApprovalLog != null)
                {
                    if (productData != null) productData.ISCURRENT = true;
                }

                this.context.TBL_TEMP_PRODUCT_CHARGE_FEE.Add(tempProductFeeEntity);
                // Audit Section ---------------------------
                var productName =  this.context.TBL_TEMP_PRODUCT.FirstOrDefault(x => x.TEMP_PRODUCTID == productFee.productId)?.PRODUCTNAME;
                var audit = new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.ProductFeeAdded,
                    STAFFID = productFee.createdBy,
                    BRANCHID = (short)productFee.userBranchId,
                    DETAIL = $"{TranslateHelper.get("Initiated adding Fee:")} { productFee.feeName } {TranslateHelper.get("for product")} {productName} {TranslateHelper.get("with amount")} {productFee.rateValue} ",
                    IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                    URL = productFee.applicationUrl,
                    APPLICATIONDATE = genSetup.GetApplicationDate(),
                    SYSTEMDATETIME = DateTime.Now,
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName(),
                };

                this.auditTrail.AddAuditTrail(audit);

                //end of Audit section -------------------------------
            }
            else
            {
                tempProductFeeEntity.RATEVALUE = productFee.rateValue;
                tempProductFeeEntity.DEPENDENTAMOUNT = productFee.dependentAmount;

                tempProductFeeEntity.DELETED = false;
            }

            var status = context.SaveChanges() > 0;

            if (status)
                return tempProductFeeEntity.PRODUCTFEEID;
            else
                return -1;
        }

        public void ApproveProductFee(int productId, UserInfo user)
        {
            var productFeeModel = context.TBL_TEMP_PRODUCT_CHARGE_FEE.Where(x => x.TEMP_PRODUCTID == productId
                                                                        && x.DELETED == false);
                                                                       
            var productToUpdate = context.TBL_PRODUCT.Find(productId);

            foreach (var p in productFeeModel)
            {
                var product = new TBL_PRODUCT_CHARGE_FEE()
                {
                    PRODUCTID = p.TEMP_PRODUCTID,
                    CHARGEFEEID = p.CHARGEFEEID,
                    COMPANYID = p.COMPANYID,

                    RATEVALUE = p.RATEVALUE,
                    DEPENDENTAMOUNT = p.DEPENDENTAMOUNT,

                    CREATEDBY = p.CREATEDBY,
                    DATETIMECREATED = genSetup.GetApplicationDate(),
                    DELETED = false
                };
                context.TBL_PRODUCT_CHARGE_FEE.Add(product);
                //context.tbl_Temp_Product_Fee.Remove(p);
            }

            foreach (var item in productFeeModel)
            {
                item.DATETIMEUPDATED = DateTime.Now;
            }

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductFeeAdded,
                STAFFID = user.staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Added Fee for product")} '{productToUpdate?.PRODUCTNAME}' {TranslateHelper.get("with product code")} '{productToUpdate?.PRODUCTCODE}'",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = user.applicationUrl,
                APPLICATIONDATE = genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            this.auditTrail.AddAuditTrail(audit);
            // Audit Section ---------------------------

            //return this.SaveAll();
        }

        public async Task<bool> DeleteMultipleProductFee(List<int> productFeeIds)
        {
            if (productFeeIds.Count <= 0)
                return false;

            var dataList =  await (from a in context.TBL_PRODUCT_CHARGE_FEE
                            where productFeeIds.ToList().Contains(a.PRODUCTFEEID)
                            select a).ToListAsync();

            foreach (TBL_PRODUCT_CHARGE_FEE data in dataList)
            {
                data.DELETED = true;
                data.DATETIMEDELETED = DateTime.Now;
            }

            return this.SaveAll();
        }

        public async Task<bool> DeleteProductFee(int productFeeId, UserInfo user)
        {
            var data =await this.context.TBL_PRODUCT_CHARGE_FEE.FindAsync(productFeeId);

            if (data == null)
                return false;

            data.DELETED = true;
            //accountModel.DeletedBy = ;
            data.DATETIMEDELETED = genSetup.GetApplicationDate();
            // Audit Section ---------------------------
            var productFee = await this.context.TBL_CHARGE_FEE.FirstOrDefaultAsync(x => x.CHARGEFEEID == data.CHARGEFEEID);
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductFeeDeleted,
                STAFFID = user.staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Deleted TBL_PRODUCT Fee:")} {productFee?.CHARGEFEENAME} {TranslateHelper.get("to product")} {data.TBL_PRODUCT?.PRODUCTNAME} ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = user.applicationUrl,
                APPLICATIONDATE = genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------
            return this.SaveAll();
        }

        public async Task<IEnumerable<ProductFeeViewModel>> GetAllMappedFeeByProduct(int productId)
        {
            return await (from data in context.TBL_PRODUCT_CHARGE_FEE
                    where data.PRODUCTID == productId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                    select new ProductFeeViewModel()
                    {
                        productFeeId = data.PRODUCTFEEID,
                        productId = data.PRODUCTID,
                        feeId = data.CHARGEFEEID,
                        feeName = data.TBL_CHARGE_FEE.CHARGEFEENAME,
                        feeTypeId = data.TBL_CHARGE_FEE.FEETYPEID,
                        feeIntervalName = data.TBL_CHARGE_FEE.TBL_FEE_INTERVAL.FEEINTERVALNAME,
                        feeTargetName = data.TBL_CHARGE_FEE.TBL_FEE_TARGET.FEETARGETNAME,
                        feeTypeName = data.TBL_CHARGE_FEE.TBL_FEE_TYPE.FEETYPENAME,
                        //glAccountCode = data.TBL_CHARGE_FEE.TBL_CHART_OF_ACCOUNT.ACCOUNTCODE,
                        //glAccountName = data.TBL_CHARGE_FEE.TBL_CHART_OF_ACCOUNT.ACCOUNTNAME,
                        companyId = data.COMPANYID,

                        rateValue = data.RATEVALUE,
                        dependentAmount = data.DEPENDENTAMOUNT,

                        createdBy = data.CREATEDBY,
                        dateTimeCreated = data.DATETIMECREATED,
                    }).ToListAsync();
        }

        public async Task<IEnumerable<ProductFeeViewModel>> GetAllMappedFeeByTempProduct(int productId)
        {
            return await(from data in context.TBL_TEMP_PRODUCT_CHARGE_FEE
                    where data.TEMP_PRODUCTID == productId && data.DELETED == false
                    select new ProductFeeViewModel()
                    {
                        productFeeId = data.PRODUCTFEEID,
                        productId = data.TEMP_PRODUCTID,
                        feeId = data.CHARGEFEEID,
                        feeName = data.TBL_CHARGE_FEE.CHARGEFEENAME,
                        feeIntervalName = data.TBL_CHARGE_FEE.TBL_FEE_INTERVAL.FEEINTERVALNAME,
                        feeTargetName = data.TBL_CHARGE_FEE.TBL_FEE_TARGET.FEETARGETNAME,
                        feeTypeName = data.TBL_CHARGE_FEE.TBL_FEE_TYPE.FEETYPENAME,
                      // glAccountCode = data.TBL_CHARGE_FEE.TBL_CHARGE_FEE_DETAIL.ACCOUNTCODE,
                      //glAccountName = data.TBL_CHARGE_FEE.TBL_CHART_OF_ACCOUNT.ACCOUNTNAME,
                        companyId = data.COMPANYID,

                        rateValue = data.RATEVALUE,
                        dependentAmount = data.DEPENDENTAMOUNT,

                        createdBy = data.CREATEDBY,
                        dateTimeCreated = data.DATETIMECREATED,
                    }).ToListAsync();
        }

        public bool DoesProductFeeExist(int productFeeId)
        {
            return context.TBL_PRODUCT_CHARGE_FEE.Any(x => x.PRODUCTFEEID == productFeeId);
        }

        public async Task<List<ProductFeeViewModel>> GetProductFeeAwaitingApprovals(int tempProductId)
        {
            return await (from data in context.TBL_TEMP_PRODUCT_CHARGE_FEE
                    join p in context.TBL_TEMP_PRODUCT on data.TEMP_PRODUCTID equals p.TEMP_PRODUCTID
                    where data.TEMP_PRODUCTID == tempProductId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                    select new ProductFeeViewModel()
                    {
                        productFeeId = data.PRODUCTFEEID,
                        productName = p.PRODUCTNAME,
                        productId = data.TEMP_PRODUCTID,
                        feeId = data.PRODUCTFEEID,
                        feeName = data.TBL_CHARGE_FEE.CHARGEFEENAME,
                        companyId = data.COMPANYID,

                        rateValue = data.RATEVALUE,
                        dependentAmount = data.DEPENDENTAMOUNT,

                        createdBy = data.CREATEDBY,
                        dateTimeCreated = data.DATETIMECREATED,
                    }).ToListAsync();
        }

        public async Task<ProductFeeViewModel> GetProductFee(int productFeeId)
        {
            return await (from data in context.TBL_PRODUCT_CHARGE_FEE
                    where data.PRODUCTFEEID == productFeeId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                    select new ProductFeeViewModel()
                    {
                        productFeeId = data.PRODUCTFEEID,
                        productId = (short)data.PRODUCTID,
                        feeId = data.CHARGEFEEID,
                        feeName = data.TBL_CHARGE_FEE.CHARGEFEENAME,
                        companyId = data.COMPANYID,

                        rateValue = data.RATEVALUE,
                        dependentAmount = data.DEPENDENTAMOUNT,

                        createdBy = data.CREATEDBY,
                        dateTimeCreated = data.DATETIMECREATED,
                    }).FirstOrDefaultAsync();
        }

        public async Task<List<ProductFeeViewModel>> GetTempProductFee(int productProductFeeId)
        {
            return await (from data in context.TBL_TEMP_PRODUCT_CHARGE_FEE
                    where data.PRODUCTFEEID == productProductFeeId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                    select new ProductFeeViewModel()
                    {
                        productFeeId = data.PRODUCTFEEID,
                        productId = (short)data.TEMP_PRODUCTID,
                        feeId = data.PRODUCTFEEID,
                        feeName = data.TBL_CHARGE_FEE.CHARGEFEENAME,
                        companyId = data.COMPANYID,

                        rateValue = data.RATEVALUE,
                        dependentAmount = data.DEPENDENTAMOUNT,

                        createdBy = data.CREATEDBY,
                        dateTimeCreated = data.DATETIMECREATED,
                    }).ToListAsync();
        }

        public async Task<IEnumerable<ChargeFeeViewModel>> GetUnmappedFeeToProduct(int productId)
        {
            var dataList = await (from data in context.TBL_PRODUCT_CHARGE_FEE
                            where data.PRODUCTID == productId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                            select data.CHARGEFEEID).ToListAsync();

            var chargeFee =  (from data in context.TBL_CHARGE_FEE
                             where data.DELETED == false
                             select new ChargeFeeViewModel
                             {
                                 chargeFeeId = data.CHARGEFEEID,
                                 chargeName = data.CHARGEFEENAME,
                                 productTypeId = data.PRODUCTTYPEID,
                                 amount = data.AMOUNT,
                                 rate = data.RATE,
                                 feeTypeId = data.FEETYPEID,
                                 feeTypeName = data.TBL_FEE_TYPE.FEETYPENAME,
                                 //ledgerAccountId = data.GLACCOUNTID,
                                 //ledgerAccountName = data.TBL_CHART_OF_ACCOUNT.ACCOUNTNAME,
                                 //ledgerAccountCode = data.TBL_CHART_OF_ACCOUNT.ACCOUNTCODE,
                                 //accountCategoryId = data.ACCOUNTCATEGORYID,
                                 //accountCategoryName = data.TBL_ACCOUNT_CATEGORY.ACCOUNTCATEGORYNAME,
                                 targetId = data.FEETARGETID,
                                 targetName = data.TBL_FEE_TARGET.FEETARGETNAME,
                                 amortisationTypeId = data.FEEAMORTISATIONTYPEID,
                                 amortisationTypeName = data.TBL_FEE_AMORTISATION_TYPE.FEEAMORTISATIONTYPENAME,
                                 frequencyTypeId = data.FEEINTERVALID,
                                 frequencyTypeName = data.TBL_FEE_INTERVAL.FEEINTERVALNAME
                             });

            if (dataList.Any())
            {
                chargeFee = chargeFee.Where(x => !dataList.Contains(x.chargeFeeId));
            }

            return chargeFee;
        }

        public async Task<bool> UpdateProductFee(int productFeeId, ProductFeeViewModel productFee)
        {
            var productFeeEntity =await  this.context.TBL_PRODUCT_CHARGE_FEE.FindAsync(productFeeId);

            if (productFeeEntity == null)
                return false;

            productFeeEntity.RATEVALUE = productFee.rateValue;
            productFeeEntity.DEPENDENTAMOUNT = productFee.dependentAmount;

            productFeeEntity.LASTUPDATEDBY = productFee.lastUpdatedBy;
            productFeeEntity.DATETIMEUPDATED = genSetup.GetApplicationDate();

            // Audit Section ---------------------------
            var productName = this.context.TBL_PRODUCT.FirstOrDefault(x => x.PRODUCTID == productFee.productId)?.PRODUCTNAME;
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductFeeUpdated,
                STAFFID = productFee.createdBy,
                BRANCHID = (short)productFee.userBranchId,
                DETAIL = $"{TranslateHelper.get("Updated TBL_PRODUCT Fee:")} { productFee.feeName } {TranslateHelper.get("to product")} {productName} {TranslateHelper.get("with amount")} {productFee.rateValue} ",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = productFee.applicationUrl,
                APPLICATIONDATE = genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------

            return this.SaveAll();
        }


        public async Task<IEnumerable<dynamic>> GetFeesByProductId(int productId)
        {
            var mappedFeeByProd = await GetAllMappedFeeByProduct(productId);
            return mappedFeeByProd.Where((c) => c.productId == productId)
                .Select(c => new
                {
                    feeId = c.feeId,
                    feeName = c.feeName,
                    rate = c.rateValue,
                    feeTypeId= c.feeTypeId
                });
        }
        
        public async Task<IEnumerable<dynamic>> GetSavedFee(int loanApplicationDetailId, bool forModifyFacility)
        {
            if (forModifyFacility)
            {
                var savedFees =await context.TBL_FACILITY_MOD_DETL_FEE.Where(f => f.FACILITYMODIFICATIONID == loanApplicationDetailId).ToListAsync();
                return savedFees.Select(c => new
                {
                    feeId = c.CHARGEFEEID,
                    feeName = context.TBL_CHARGE_FEE.FirstOrDefault(f => f.CHARGEFEEID == c.CHARGEFEEID).CHARGEFEENAME,
                    rate = c.RECOMMENDED_FEERATEVALUE,
                    feeTypeId = context.TBL_CHARGE_FEE.FirstOrDefault(f => f.CHARGEFEEID == c.CHARGEFEEID).FEETYPEID
                });
            }
            else
            {
                var savedFees = await context.TBL_LOAN_APPLICATION_DETL_FEE.Where(f => f.LOANAPPLICATIONDETAILID == loanApplicationDetailId && f.DELETED == false).ToListAsync();
                return savedFees.Select(c => new
                {
                    feeId = c.CHARGEFEEID,
                    feeName = c.TBL_CHARGE_FEE.CHARGEFEENAME,
                    rate = c.RECOMMENDED_FEERATEVALUE,
                    feeTypeId = c.TBL_CHARGE_FEE.FEETYPEID
                });
            }
        }

    }
}

<!-- Auto-push timestamp: 2026-03-28 18:22:44 -->