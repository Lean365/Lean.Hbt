//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtDbSeedSalesDictType.cs
// 创建者 : Claude
// 创建时间: 2024-02-19
// 版本号 : V0.0.1
// 描述   : 销售相关字典类型种子数据初始化类
//===================================================================

using Lean.Hbt.Domain.Entities.Core;

namespace Lean.Hbt.Infrastructure.Data.Seeds;

/// <summary>
/// 销售相关字典类型种子数据初始化类
/// </summary>
public class HbtDbSeedSalesDictType
{
    private readonly IHbtRepository<HbtDictType> _dictTypeRepository;
    private readonly IHbtLogger _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dictTypeRepository">字典类型仓储</param>
    /// <param name="logger">日志记录器</param>
    public HbtDbSeedSalesDictType(IHbtRepository<HbtDictType> dictTypeRepository, IHbtLogger logger)
    {
        _dictTypeRepository = dictTypeRepository;
        _logger = logger;
    }

    /// <summary>
    /// 初始化销售相关字典类型数据
    /// </summary>
    public async Task<(int, int)> InitializeSalesDictTypeAsync()
    {
        int insertCount = 0;
        int updateCount = 0;

        var defaultDictTypes = new List<HbtDictType>
        {
            new HbtDictType
            {
                DictName = "销售订单类型",
                DictType = "sys_sales_order_type",
                OrderNum = 1,
                Status = 0,

                Remark = "销售订单类型字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "销售订单状态",
                DictType = "sys_sales_order_status",
                OrderNum = 2,
                Status = 0,

                Remark = "销售订单状态字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "销售组织",
                DictType = "sys_sales_org",
                OrderNum = 3,
                Status = 0,

                Remark = "销售组织字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "分销渠道",
                DictType = "sys_distribution_channel",
                OrderNum = 4,
                Status = 0,

                Remark = "分销渠道字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "销售类型",
                DictType = "sys_sales_type",
                OrderNum = 5,
                Status = 0,

                Remark = "销售类型字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "销售条件",
                DictType = "sys_sales_condition",
                OrderNum = 6,
                Status = 0,

                Remark = "销售条件字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "销售计划类型",
                DictType = "sys_sales_plan_type",
                OrderNum = 7,
                Status = 0,

                Remark = "销售计划类型字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "客户类型",
                DictType = "sys_customer_type",
                OrderNum = 8,
                Status = 0,

                Remark = "客户类型字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "客户等级",
                DictType = "sys_customer_level",
                OrderNum = 9,
                Status = 0,

                Remark = "客户等级字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            },
            new HbtDictType
            {
                DictName = "客户状态",
                DictType = "sys_customer_status",
                OrderNum = 10,
                Status = 0,

                Remark = "客户状态字典",
                CreateBy = "Hbt365",
                CreateTime = DateTime.Now,
                UpdateBy = "Hbt365",
                UpdateTime = DateTime.Now
            }
        };

        foreach (var dictType in defaultDictTypes)
        {
            var existingDictType = await _dictTypeRepository.GetFirstAsync(d => d.DictType == dictType.DictType);
            if (existingDictType == null)
            {

                dictType.CreateBy = "Hbt365";
                dictType.CreateTime = DateTime.Now;
                dictType.UpdateBy = "Hbt365";
                dictType.UpdateTime = DateTime.Now;
                await _dictTypeRepository.CreateAsync(dictType);
                insertCount++;
                _logger.Info($"[创建] 销售字典类型 '{dictType.DictName}' 创建成功");
            }
            else
            {
                existingDictType.DictName = dictType.DictName;
                existingDictType.DictType = dictType.DictType;
                existingDictType.IsBuiltin = dictType.IsBuiltin;
                existingDictType.OrderNum = dictType.OrderNum;
                existingDictType.Status = dictType.Status;

                existingDictType.Remark = dictType.Remark;
                existingDictType.CreateBy = dictType.CreateBy;
                existingDictType.CreateTime = dictType.CreateTime;
                existingDictType.UpdateBy = "Hbt365";
                existingDictType.UpdateTime = DateTime.Now;

                await _dictTypeRepository.UpdateAsync(existingDictType);
                updateCount++;
                _logger.Info($"[更新] 销售字典类型 '{existingDictType.DictName}' 更新成功");
            }
        }

        return (insertCount, updateCount);
    }
}