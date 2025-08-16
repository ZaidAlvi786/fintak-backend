using FintrakBanking.Entities.Models;
using FintrakBanking.Interfaces.Setups.General;
using FintrakBanking.ViewModels.Setups.General;
using System;
using System.Collections.Generic;
using System.Linq;
using FintrakBanking.ViewModels;
using FintrakBanking.Interfaces.Admin;
using FintrakBanking.Common.Enum;
using FintrakBanking.ViewModels.Credit;
using System.ComponentModel.Composition;
using FintrakBanking.Common;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FintrakBanking.Repositories.Setups.Finance
{
    /// <summary>
    /// TODO: Implement audit trails in these methods
    /// </summary>
    [Export(typeof(IProductCollateralTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductCollateralTypeRepository : IProductCollateralTypeRepository
    {
        private FinTrakBankingContext context;
        private IAuditTrailRepository auditTrail;
        IGeneralSetupRepository _genSetup;
        public ProductCollateralTypeRepository(FinTrakBankingContext _context, 
                                                IAuditTrailRepository _auditTrail,
                                                IGeneralSetupRepository genSetup)
        {
            this.context = _context;
            this._genSetup = genSetup;
            auditTrail = _auditTrail;
        }

        private bool SaveAll()
        {
            return this.context.SaveChanges() > 0;
        }

        public async Task<int> AddProductCollateralType(ProductCollateralTypeViewModel productCollateral)
        {
            var dataExist = this.context.TBL_PRODUCT_COLLATERALTYPE.FirstOrDefault(x => x.PRODUCTID == productCollateral.productId && x.COLLATERALTYPEID == productCollateral.collateralTypeId && x.DELETED == true); // .Find(accountId);

            var productCollateralEntity = dataExist;

            if (dataExist == null)
            {
                productCollateralEntity = new TBL_PRODUCT_COLLATERALTYPE()
                {
                    PRODUCTID = productCollateral.productId,
                    COLLATERALTYPEID = productCollateral.collateralTypeId,
                    COMPANYID = productCollateral.companyId,
                    CREATEDBY = productCollateral.createdBy,
                    DATETIMECREATED = _genSetup.GetApplicationDate()
                };

                this.context.TBL_PRODUCT_COLLATERALTYPE.Add(productCollateralEntity);
            }
            else
            {
                productCollateralEntity.DELETED = false;
            }
            var status = this.SaveAll();

            // Audit Section ---------------------------
            var product = this.context.TBL_PRODUCT.FirstOrDefault(x => x.PRODUCTID == productCollateral.productId);
            var collateralInfo = await this.context.TBL_COLLATERAL_TYPE.FirstOrDefaultAsync(x => x.COLLATERALTYPEID == productCollateral.collateralTypeId);
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductCollateralAdded,
                STAFFID = productCollateral.createdBy,
                BRANCHID = (short)productCollateral.userBranchId,
                DETAIL = $"{TranslateHelper.get("Added product collateral type:")} {collateralInfo.COLLATERALTYPENAME} {TranslateHelper.get("to product")} {product.PRODUCTCODE}  ({product.PRODUCTNAME})",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = productCollateral.applicationUrl,
                SYSTEMDATETIME = DateTime.Now,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),


            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------

            if (status)
                return productCollateralEntity.PRODUCTCOLLATERALTYPEID;
            else
                return -1;
        }

        public async Task<int> AddTempProductCollateralType(ProductCollateralTypeViewModel productCollateral)
        {
            var dataExist = this.context.TBL_PRODUCT_COLLATERALTYPE.FirstOrDefault(x => x.PRODUCTID == productCollateral.productId
                                                                && x.COLLATERALTYPEID == productCollateral.collateralTypeId
                                                                && x.DELETED == true); // .Find(accountId);

            var tempProductCollateralEntity = dataExist;

            if (dataExist == null)
            {
                tempProductCollateralEntity = new TBL_PRODUCT_COLLATERALTYPE()
                {
                    PRODUCTID = productCollateral.productId,
                    COMPANYID = productCollateral.companyId,
                    COLLATERALTYPEID = productCollateral.collateralTypeId,
                    CREATEDBY = productCollateral.createdBy,
                    DATETIMECREATED = _genSetup.GetApplicationDate(),
                    DELETED = false,
                    //IsCurrent = true
                };

                var existingProductApprovalLog = await context.TBL_TEMP_PRODUCT.FindAsync(productCollateral.productId);
                var ProductData = await context .TBL_PRODUCT.FindAsync(productCollateral.productId);

                //if (existingProductApprovalLog != null)
                //{
                //    ProductData.IsCurrent = true;
                //}

                this.context.TBL_PRODUCT_COLLATERALTYPE.Add(tempProductCollateralEntity);
                // Audit Section ---------------------------
                var product = await this.context.TBL_TEMP_PRODUCT.FirstOrDefaultAsync(x => x.TEMP_PRODUCTID == productCollateral.productId);
                var collateralInfo = await this.context.TBL_COLLATERAL_TYPE.FirstOrDefaultAsync(x => x.COLLATERALTYPEID == productCollateral.collateralTypeId);
                var audit = new TBL_AUDIT
                {
                    AUDITTYPEID = (short)AuditTypeEnum.ProductCollateralAdded,
                    STAFFID = productCollateral.createdBy,
                    BRANCHID = (short)productCollateral.userBranchId,
                    DETAIL = $"{TranslateHelper.get("Added product collateral type:")} {collateralInfo?.COLLATERALTYPENAME} {TranslateHelper.get("to product")} {product.PRODUCTCODE}  ({product.PRODUCTNAME})",
                    IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                    URL = productCollateral.applicationUrl,
                    SYSTEMDATETIME = DateTime.Now,
                    APPLICATIONDATE = _genSetup.GetApplicationDate(),
                    DEVICENAME = CommonHelpers.GetDeviceName(),
                    OSNAME = CommonHelpers.FriendlyName(),
                };

                this.auditTrail.AddAuditTrail(audit);

                //end of Audit section -------------------------------
            }
            else
            {
                tempProductCollateralEntity.PRODUCTID = productCollateral.productId;
                tempProductCollateralEntity.COLLATERALTYPEID = productCollateral.collateralTypeId;

                tempProductCollateralEntity.DELETED = false;
            }

            var status = this.SaveAll();

            if (status)
                return tempProductCollateralEntity.PRODUCTCOLLATERALTYPEID;

            return -1;
        }

        public void ApproveProductCollateral(int productId, UserInfo user)
        {
            var productCollateralTypeModel = context.TBL_TEMP_PRODUCT_COLLATERALTYP.Where(x => x.TEMP_PRODUCTID == productId
                                                                        && x.DELETED == false
                                                                        && x.ISCURRENT == true);
            var productToUpdate = context.TBL_PRODUCT.Find(productId);

            foreach (var p in productCollateralTypeModel)
            {
                var productCollateralType = new TBL_PRODUCT_COLLATERALTYPE()
                {
                    PRODUCTID = p.TEMP_PRODUCTID,
                    COMPANYID = p.COMPANYID,
                    CREATEDBY = p.CREATEDBY,
                    DATETIMECREATED = _genSetup.GetApplicationDate(),
                    DELETED = false
                };
                context.TBL_PRODUCT_COLLATERALTYPE.Add(productCollateralType);
                context.TBL_TEMP_PRODUCT_COLLATERALTYP.Remove(p);
            }

            // Audit Section ---------------------------
            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductFeeAdded,
                STAFFID = user.staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Added CollateralType for product")} '{productToUpdate.PRODUCTNAME}' {TranslateHelper.get("with product code")}'{productToUpdate.PRODUCTCODE}'",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = user.applicationUrl,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            this.auditTrail.AddAuditTrail(audit);
            // Audit Section ---------------------------

            //return this.SaveAll();
        }

        public async Task<int> AddMultipleProductCollateralType(List<ProductCollateralTypeViewModel> collateralTypes)
        {
            if (collateralTypes.Count <= 0)
                return -1;

            foreach (ProductCollateralTypeViewModel item in collateralTypes)
            {
                await AddProductCollateralType(item );
            }

            return 1;
        }

        public bool DoesProductCollateralExist(int productCollateralTypeId)
        {
            return context.TBL_PRODUCT_COLLATERALTYPE.Any(x => x.PRODUCTCOLLATERALTYPEID == productCollateralTypeId);
        }

        public async Task<IEnumerable<ProductCollateralTypeViewModel>> GetCollateralTypeByProduct(int productId)
        {
            return await (from data in context.TBL_PRODUCT_COLLATERALTYPE
                    where data.PRODUCTID == productId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                    select new ProductCollateralTypeViewModel()
                    {
                        productCollateralId = data.PRODUCTCOLLATERALTYPEID,
                        productId = (short)data.PRODUCTID,
                        collateralTypeId = data.COLLATERALTYPEID,
                        collateralTypeName = data.TBL_COLLATERAL_TYPE.COLLATERALTYPENAME,
                        companyId = data.COMPANYID,
                        createdBy = data.CREATEDBY,
                        deleted = data.DELETED,
                    }).ToListAsync();
            
        }

        public async Task<IEnumerable<ProductCollateralTypeViewModel>> GetMappedCollateralTypeByProduct(int productId)
        {
            var response = await (from data in context.TBL_TEMP_PRODUCT_COLLATERALTYP
                            where data.TEMP_PRODUCTID == productId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                            select new ProductCollateralTypeViewModel()
                            {
                                productCollateralId = data.PRODUCTCOLLATERALTYPEID,
                                productId = (short)data.TEMP_PRODUCTID,
                                collateralTypeId = data.COLLATERALTYPEID,
                                collateralTypeName = data.TBL_COLLATERAL_TYPE.COLLATERALTYPENAME,
                                companyId = data.COMPANYID,
                                createdBy = data.CREATEDBY,
                                deleted = data.DELETED,
                            }).ToListAsync();

            return response;
        }

        public async Task<IEnumerable<CollateralTypeViewModel>> GetUnmappedCollateralToProduct(int productId)
        {
            IEnumerable<CollateralTypeViewModel> collaterals = null;

            var dataList =await (from data in context.TBL_PRODUCT_COLLATERALTYPE
                            where data.PRODUCTID == productId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                            select data.COLLATERALTYPEID).ToListAsync();

            collaterals = await (from a in context.TBL_COLLATERAL_TYPE
                           where a.DELETED == false
                           select new CollateralTypeViewModel
                           {
                               collateralTypeName = a.COLLATERALTYPENAME,
                               collateralTypeId = a.COLLATERALTYPEID,
                               companyId = a.COMPANYID,
                               deleted = a.DELETED,
                               details = a.DETAILS,
                               createdBy = a.CREATEDBY,
                               dateTimeCreated = a.DATETIMECREATED 
                           }).ToListAsync();

            if (dataList.Any())
            {
                collaterals = collaterals.Where(x => !dataList.Contains(x.collateralTypeId));
            }

            return collaterals;
        }

        public async Task<ProductCollateralTypeViewModel> GetProductCollateralTypeViewModel(int productCollateralTypeId)
        {
            return await (from data in context.TBL_PRODUCT_COLLATERALTYPE
                    where data.PRODUCTCOLLATERALTYPEID == productCollateralTypeId && data.DELETED == false //orderby account.AccountCode ascending, account.AccountName ascending
                    select new ProductCollateralTypeViewModel()
                    {
                        productCollateralId = data.PRODUCTCOLLATERALTYPEID,
                        productId =(short)data.PRODUCTID,
                        collateralTypeId = data.COLLATERALTYPEID,
                        collateralTypeName = data.TBL_COLLATERAL_TYPE.COLLATERALTYPENAME,
                        companyId = data.COMPANYID,

                        createdBy = data.CREATEDBY,
                        //dateTimeCreated = data.DateTimeCreated,

                        //lastUpdatedBy = data.LastUpdatedBy.Value,
                       // dateTimeUpdated = data.DateTimeUpdated,

                        deleted = data.DELETED,
                       // deletedBy = data.DeletedBy,
                       // dateTimeDeleted = data.DateTimeDeleted
                    }).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteProductCollateralType(int productCollateralTypeId, UserInfo user)

        {
            var data = await this.context.TBL_PRODUCT_COLLATERALTYPE.FindAsync(productCollateralTypeId);

            if (data == null)
                return false;

            data.DELETED = true;
            data.DATETIMEDELETED = _genSetup.GetApplicationDate();

            // Audit Section ---------------------------

            
            var collateralInfo =await (from x in context.TBL_PRODUCT_COLLATERALTYPE
                                  where x.PRODUCTCOLLATERALTYPEID == productCollateralTypeId  //orderby account.AccountCode ascending, account.AccountName ascending
                                  select new
                                  {

                                      collateralTypeName = x.TBL_COLLATERAL_TYPE.COLLATERALTYPENAME,
                                      productCode = x.TBL_PRODUCT.PRODUCTCODE,
                                      productName = x.TBL_PRODUCT.PRODUCTNAME

                                  }).FirstOrDefaultAsync();

            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductCollateralDeleted,
                STAFFID = user.staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Deleted product collateral type:")} {collateralInfo.collateralTypeName}  {TranslateHelper.get("to product")} {collateralInfo.productCode} ({collateralInfo.productName})",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = user.applicationUrl,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            this.auditTrail.AddAuditTrail(audit);
            //AuditTrail
            
            return this.SaveAll();

            
        }

        public async Task<bool> DeleteMultipleProductCollateralType(List<int> productCollateralTypeIds, UserInfo user)
        {
            if (productCollateralTypeIds.Count <= 0)
                return false;

            var dataList =  (from a in context.TBL_PRODUCT_COLLATERALTYPE
                            where productCollateralTypeIds.ToList().Contains(a.PRODUCTCOLLATERALTYPEID)
                            select a);

            foreach (TBL_PRODUCT_COLLATERALTYPE data in dataList)
            {
                data.DELETED = true;
                data.DATETIMEDELETED = DateTime.Now;   
                // Audit Section ---------------------------

            var collateralType = await this.context.TBL_PRODUCT_COLLATERALTYPE.FirstOrDefaultAsync(x => x.PRODUCTCOLLATERALTYPEID == data.PRODUCTCOLLATERALTYPEID);

            var audit = new TBL_AUDIT
            {
                AUDITTYPEID = (short)AuditTypeEnum.ProductCollateralDeleted,
                STAFFID = user.staffId,
                BRANCHID = (short)user.BranchId,
                DETAIL = $"{TranslateHelper.get("Deleted product collateral type:")} {collateralType.TBL_COLLATERAL_TYPE.COLLATERALTYPENAME}  {TranslateHelper.get("to product")} {collateralType.TBL_PRODUCT.PRODUCTCODE} ({collateralType.TBL_PRODUCT.PRODUCTNAME})",
                IPADDRESS = CommonHelpers.GetLocalIpAddress(),
                URL = user.applicationUrl,
                APPLICATIONDATE = _genSetup.GetApplicationDate(),
                SYSTEMDATETIME = DateTime.Now,
                DEVICENAME = CommonHelpers.GetDeviceName(),
                OSNAME = CommonHelpers.FriendlyName(),
            };

            this.auditTrail.AddAuditTrail(audit);

            //end of Audit section -------------------------------
            }
     

            return this.SaveAll();
        }
    }
}