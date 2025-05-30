//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : IHbtMenuService.cs
// 创建者 : Lean365
// 创建时间: 2024-01-20 16:30
// 版本号 : V0.0.1
// 描述   : 菜单服务接口
//===================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Lean.Hbt.Common.Models;
using Lean.Hbt.Application.Dtos.Identity;

namespace Lean.Hbt.Application.Services.Identity
{
    /// <summary>
    /// 菜单服务接口
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-20
    /// </remarks>
    public interface IHbtMenuService
    {
        /// <summary>
        /// 获取菜单分页列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>分页结果</returns>
        Task<HbtPagedResult<HbtMenuDto>> GetListAsync(HbtMenuQueryDto query);

        /// <summary>
        /// 获取菜单详情
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜单详情</returns>
        Task<HbtMenuDto> GetByIdAsync(long menuId);

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="input">创建对象</param>
        /// <returns>菜单ID</returns>
        Task<long> CreateAsync(HbtMenuCreateDto input);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="input">更新对象</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(HbtMenuUpdateDto input);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(long menuId);

        /// <summary>
        /// 批量删除菜单
        /// </summary>
        /// <param name="menuIds">菜单ID列表</param>
        /// <returns>是否成功</returns>
        Task<bool> BatchDeleteAsync(long[] menuIds);

        /// <summary>
        /// 导出菜单数据
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>导出的Excel文件字节数组</returns>
        Task<(string fileName, byte[] content)> ExportAsync(HbtMenuQueryDto query, string sheetName = "菜单数据");

        /// <summary>
        /// 更新菜单状态
        /// </summary>
        /// <param name="input">状态更新对象</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateStatusAsync(HbtMenuStatusDto input);

        /// <summary>
        /// 获取导入模板
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>包含文件名和内容的元组</returns>
        Task<(string fileName, byte[] content)> GetTemplateAsync(string sheetName = "HbtMenu");

        /// <summary>
        /// 导入菜单数据
        /// </summary>
        /// <param name="fileStream">Excel文件流</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>返回导入结果(success:成功数量,fail:失败数量)</returns>
        Task<(int success, int fail)> ImportAsync(Stream fileStream, string sheetName = "HbtMenu");

        /// <summary>
        /// 更新菜单排序
        /// </summary>
        /// <param name="input">排序对象</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateOrderAsync(HbtMenuOrderDto input);

        /// <summary>
        /// 获取菜单树形结构
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>返回树形菜单列表</returns>
        Task<List<HbtMenuDto>> GetTreeAsync(HbtMenuQueryDto query = null);

        /// <summary>
        /// 获取当前用户的菜单树
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>返回当前用户的菜单树</returns>
        Task<List<HbtMenuDto>> GetCurrentUserMenusAsync(long userId);

        /// <summary>
        /// 获取菜单选项列表
        /// </summary>
        /// <returns>菜单选项列表</returns>
        Task<List<HbtSelectOption>> GetOptionsAsync();
    }
} 