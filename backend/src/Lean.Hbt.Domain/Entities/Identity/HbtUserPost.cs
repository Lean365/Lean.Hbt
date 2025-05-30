//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtUserPost.cs
// 创建者 : Lean365
// 创建时间: 2024-01-16 11:50
// 版本号 : V0.0.1
// 描述    : 用户岗位关联实体类
//===================================================================

using SqlSugar;

namespace Lean.Hbt.Domain.Entities.Identity
{
    /// <summary>
    /// 用户岗位关联实体
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-16
    /// </remarks>
    [SugarTable("hbt_identity_user_post", TableDescription = "用户岗位关联表")]
    [SugarIndex("ix_user_post", nameof(UserId), OrderByType.Asc, nameof(PostId), OrderByType.Asc, true)]
    public class HbtUserPost : HbtBaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID", ColumnDataType = "bigint", IsNullable = false)]
        public long UserId { get; set; }

        /// <summary>
        /// 岗位ID
        /// </summary>
        [SugarColumn(ColumnName = "post_id", ColumnDescription = "岗位ID", ColumnDataType = "bigint", IsNullable = false)]
        public long PostId { get; set; }

        /// <summary>
        /// 用户导航属性
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(UserId))]
        public HbtUser? User { get; set; }

        /// <summary>
        /// 岗位导航属性
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(PostId))]
        public HbtPost? Post { get; set; }
    }
}