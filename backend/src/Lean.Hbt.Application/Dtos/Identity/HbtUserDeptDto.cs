//===================================================================
// 项目名 : Lean.Hbt.Application
// 文件名 : HbtUserDeptDto.cs
// 创建者 : Lean365
// 创建时间: 2024-06-xx
// 版本号 : V0.0.1
// 描述   : 用户部门关联DTO
//===================================================================

using System;

namespace Lean.Hbt.Application.Dtos.Identity
{
    /// <summary>
    /// 用户部门关联DTO
    /// </summary>
    public class HbtUserDeptDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

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
        /// 用户名称（导航属性）
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 部门名称（导航属性）
        /// </summary>
        public string? DeptName { get; set; }

        /// <summary>
        /// 租户名称（导航属性）
        /// </summary>
        public string? TenantName { get; set; }
    }
} 