//===================================================================
// 项目名 : Lean.Hbt.Application
// 文件名 : HbtRoleDeptDto.cs
// 创建者 : Lean365
// 创建时间: 2024-06-xx
// 版本号 : V0.0.1
// 描述   : 角色部门关联DTO
//===================================================================

using System;
using System.Collections.Generic;

namespace Lean.Hbt.Application.Dtos.Identity
{
    /// <summary>
    /// 角色部门关联DTO
    /// </summary>
    public class HbtRoleDeptDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public long DeptId { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public long TenantId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string? CreateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string? UpdateBy { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 角色名称（导航属性）
        /// </summary>
        public string? RoleName { get; set; }

        /// <summary>
        /// 部门名称（导航属性）
        /// </summary>
        public string? DeptName { get; set; }

        /// <summary>
        /// 租户名称（导航属性）
        /// </summary>
        public string? TenantName { get; set; }

        /// <summary>
        /// 部门ID集合（用于批量操作）
        /// </summary>
        public long[] DeptIds { get; set; } = Array.Empty<long>();

        /// <summary>
        /// 已分配部门列表
        /// </summary>
        public List<HbtDeptDto> AssignedDepts { get; set; } = new();

        /// <summary>
        /// 可选部门列表
        /// </summary>
        public List<HbtDeptDto> OptionalDepts { get; set; } = new();
    }
} 