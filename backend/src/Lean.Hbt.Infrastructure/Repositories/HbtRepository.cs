//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtRepository.cs
// 创建者 : Lean365
// 创建时间: 2024-01-16 14:20
// 版本号 : V0.0.1
// 描述    : SqlSugar仓储实现
//===================================================================

using System.Linq.Expressions;
using Lean.Hbt.Common.Models;
using Lean.Hbt.Domain.Entities.Identity;

namespace Lean.Hbt.Infrastructure.Repositories
{
    /// <summary>
    /// SqlSugar通用仓储实现
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-16
    /// </remarks>
    public class HbtRepository<TEntity> : IHbtRepository<TEntity> where TEntity : class, new()
    {
        private readonly SqlSugarScope _db;
        private readonly SimpleClient<TEntity> _entities;
        private readonly IHbtCurrentUser _currentUser;
        private readonly IHbtCurrentTenant _currentTenant;

        /// <summary>
        /// 日志服务
        /// </summary>
        protected readonly IHbtLogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="db">SqlSugar客户端</param>
        /// <param name="currentUser">当前用户服务</param>
        /// <param name="currentTenant">当前租户服务</param>
        /// <param name="logger">日志服务</param>
        public HbtRepository(SqlSugarScope db, IHbtCurrentUser currentUser,
            IHbtCurrentTenant currentTenant,
            IHbtLogger logger)
        {
            _db = db;
            _entities = _db.GetSimpleClient<TEntity>();
            _currentUser = currentUser;
            _currentTenant = currentTenant;
            _logger = logger;
        }

        /// <summary>
        /// 获取SqlSugar客户端
        /// </summary>
        public ISqlSugarClient SqlSugarClient => _db;

        /// <summary>
        /// 获取SimpleClient对象
        /// </summary>
        public SimpleClient<TEntity> SimpleClient => _entities;

        /// <summary>
        /// 应用租户过滤
        /// </summary>
        private ISugarQueryable<TEntity> ApplyTenantFilter(ISugarQueryable<TEntity> query)
        {
            // 如果实体实现了ITenantEntity接口，且租户ID不为-1（租户功能已启用），则添加租户过滤条件
            if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
            {
                var tenantId = _currentTenant.TenantId;
                if (tenantId != -1) // 租户功能已启用
                {
                    query = query.Where("tenant_id = @tenantId", new { tenantId });
                }
            }
            return query;
        }

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <returns>返回ISugarQueryable查询对象</returns>
        public ISugarQueryable<TEntity> AsQueryable()
        {
            var query = _db.Queryable<TEntity>();
            return ApplyTenantFilter(query);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>返回实体对象,如果未找到返回null</returns>
        public async Task<TEntity?> GetByIdAsync(object id)
        {
            var query = _db.Queryable<TEntity>();

            // 非管理员需要过滤已删除记录
            if (_currentUser.UserType != 2 && typeof(TEntity).IsSubclassOf(typeof(HbtBaseEntity)))
            {
                query = query.Where("is_deleted = 0");
            }

            // 应用租户过滤
            query = ApplyTenantFilter(query);

            return await query.InSingleAsync(id);
        }

        /// <summary>
        /// 获取第一个符合条件的实体
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns>返回实体对象,如果未找到返回null</returns>
        public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> condition)
        {
            var query = _db.Queryable<TEntity>();

            // 非管理员需要过滤已删除记录
            if (_currentUser.UserType != 2 && typeof(TEntity).IsSubclassOf(typeof(HbtBaseEntity)))
            {
                query = query.Where("is_deleted = 0");
            }

            // 应用租户过滤
            query = ApplyTenantFilter(query);

            return await query.Where(condition).FirstAsync();
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns>实体列表</returns>
        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? condition = null)
        {
            var query = _db.Queryable<TEntity>();

            // 非管理员需要过滤已删除记录
            if (_currentUser.UserType != 2 && typeof(TEntity).IsSubclassOf(typeof(HbtBaseEntity)))
            {
                query = query.Where("is_deleted = 0");
            }

            // 应用租户过滤
            query = ApplyTenantFilter(query);

            if (condition != null)
            {
                query = query.Where(condition);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="orderByType">排序类型</param>
        /// <returns>分页结果</returns>
        public async Task<HbtPagedResult<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>>? condition = null,
            int pageIndex = 1,
            int pageSize = 20,
            Expression<Func<TEntity, object>>? orderByExpression = null,
            OrderByType orderByType = OrderByType.Desc)
        {
            var query = _db.Queryable<TEntity>();

            // 非管理员需要过滤已删除记录
            if (_currentUser.UserType != 2 && typeof(TEntity).IsSubclassOf(typeof(HbtBaseEntity)))
            {
                query = query.Where("is_deleted = 0");
            }

            // 应用租户过滤
            query = ApplyTenantFilter(query);

            if (condition != null)
            {
                query = query.Where(condition);
            }

            if (orderByExpression != null)
            {
                query = orderByType == OrderByType.Asc
                    ? query.OrderBy(orderByExpression)
                    : query.OrderBy(orderByExpression, OrderByType.Desc);
            }

            var total = await query.CountAsync();
            var list = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new HbtPagedResult<TEntity>
            {
                Rows = list,
                TotalNum = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 获取分页列表(支持多个排序条件)
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="orderByExpressions">排序表达式列表</param>
        /// <returns>实体列表和总记录数</returns>
        public async Task<HbtPagedResult<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>>? condition = null,
            int pageIndex = 1,
            int pageSize = 20,
            List<(Expression<Func<TEntity, object>> Expression, OrderByType Type)>? orderByExpressions = null)
        {
            var query = _db.Queryable<TEntity>();

            // 添加查询条件
            if (condition != null)
            {
                query = query.Where(condition);
            }

            // 添加排序条件
            if (orderByExpressions != null && orderByExpressions.Count > 0)
            {
                var isFirst = true;
                foreach (var (expression, type) in orderByExpressions)
                {
                    if (isFirst)
                    {
                        query = type == OrderByType.Asc
                            ? query.OrderBy(expression)
                            : query.OrderBy(expression, OrderByType.Desc);
                        isFirst = false;
                    }
                    else
                    {
                        query = type == OrderByType.Asc
                            ? query.OrderByIF(true, expression, OrderByType.Asc)
                            : query.OrderByIF(true, expression, OrderByType.Desc);
                    }
                }
            }

            // 获取总数
            var total = await query.CountAsync();

            // 分页查询
            var list = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new HbtPagedResult<TEntity>
            {
                Rows = list,
                TotalNum = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 新增单个实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> CreateAsync(TEntity entity)
        {
            if (entity is HbtBaseEntity baseEntity)
            {
                baseEntity.CreateTime = DateTime.Now;
                if (string.IsNullOrEmpty(baseEntity.CreateBy))
                {
                    baseEntity.CreateBy = _currentUser.UserName ?? "Hbt365";
                }
                _logger.Info($"[CreateAsync] 实体类型: {typeof(TEntity).Name}, CreateBy: '{baseEntity.CreateBy}'");
            }
            var result = await _db.Insertable(entity).ExecuteCommandAsync();

            return result;
        }

        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <param name="entities">实体对象列表</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> CreateRangeAsync(List<TEntity> entities)
        {
            var now = DateTime.Now;
            var userName = _currentUser.UserName ?? "Hbt365";

            foreach (var entity in entities)
            {
                if (entity is HbtBaseEntity baseEntity)
                {
                    baseEntity.CreateTime = now;
                    if (string.IsNullOrEmpty(baseEntity.CreateBy))
                    {
                        baseEntity.CreateBy = userName;
                    }
                }
            }

            // 超大数据量时使用Fastest批量插入（同步）
            if (entities.Count > 50000)
            {
                _db.Fastest<TEntity>().PageSize(100000).BulkCopy(entities);
                return entities.Count;
            }

            // 普通批量插入使用PageSize分批（异步）
            return await _db.Insertable(entities).PageSize(10000).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity is HbtBaseEntity baseEntity)
            {
                baseEntity.UpdateTime = DateTime.Now;
                if (string.IsNullOrEmpty(baseEntity.UpdateBy))
                {
                    baseEntity.UpdateBy = _currentUser.UserName ?? "Hbt365";
                }
            }
            return await _db.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体对象列表</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> UpdateRangeAsync(List<TEntity> entities)
        {
            var now = DateTime.Now;
            var userName = _currentUser.UserName ?? "Hbt365";

            foreach (var entity in entities)
            {
                if (entity is HbtBaseEntity baseEntity)
                {
                    baseEntity.UpdateTime = now;
                    if (string.IsNullOrEmpty(baseEntity.UpdateBy))
                    {
                        baseEntity.UpdateBy = userName;
                    }
                }
            }

            // 超大数据量时使用分批更新
            if (entities.Count > 50000)
            {
                var total = 0;
                // 每批10000条
                for (int i = 0; i < entities.Count; i += 10000)
                {
                    var batch = entities.Skip(i).Take(10000).ToList();
                    total += await _db.Updateable(batch).ExecuteCommandAsync();
                }
                return total;
            }

            // 普通批量更新使用PageSize分批
            return await _db.Updateable(entities).PageSize(10000).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return 0;
            return await DeleteAsync(entity);
        }

        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> DeleteAsync(TEntity entity)
        {
            // 检查是否有Status字段
            var statusProperty = typeof(TEntity).GetProperty("Status");
            if (statusProperty != null)
            {
                // 更新Status为1
                statusProperty.SetValue(entity, 1);
                var updateByProperty = typeof(TEntity).GetProperty("UpdateBy");
                var updateTimeProperty = typeof(TEntity).GetProperty("UpdateTime");
                if (updateByProperty != null && string.IsNullOrEmpty(updateByProperty.GetValue(entity)?.ToString()))
                {
                    updateByProperty.SetValue(entity, _currentUser.UserName ?? "Hbt365");
                }
                if (updateTimeProperty != null)
                {
                    updateTimeProperty.SetValue(entity, DateTime.Now);
                }
            }

            // 设置删除标记
            if (entity is HbtBaseEntity baseEntity)
            {
                baseEntity.IsDeleted = 1;
                baseEntity.DeleteTime = DateTime.Now;
                if (string.IsNullOrEmpty(baseEntity.DeleteBy))
                {
                    baseEntity.DeleteBy = _currentUser.UserName ?? "Hbt365";
                }
            }
            return await _db.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据主键批量删除
        /// </summary>
        /// <param name="ids">主键值列表</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> DeleteRangeAsync(List<object> ids)
        {
            var entities = new List<TEntity>();
            foreach (var id in ids)
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            if (!entities.Any()) return 0;
            return await DeleteRangeAsync(entities);
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="entities">实体对象列表</param>
        /// <returns>返回受影响的行数</returns>
        public async Task<int> DeleteRangeAsync(List<TEntity> entities)
        {
            var now = DateTime.Now;
            var userName = _currentUser.UserName ?? "Hbt365";

            // 检查是否有Status字段
            var statusProperty = typeof(TEntity).GetProperty("Status");
            var updateByProperty = typeof(TEntity).GetProperty("UpdateBy");
            var updateTimeProperty = typeof(TEntity).GetProperty("UpdateTime");

            foreach (var entity in entities)
            {
                // 更新Status为1
                if (statusProperty != null)
                {
                    statusProperty.SetValue(entity, 1);
                    if (updateByProperty != null && string.IsNullOrEmpty(updateByProperty.GetValue(entity)?.ToString()))
                    {
                        updateByProperty.SetValue(entity, userName);
                    }
                    if (updateTimeProperty != null)
                    {
                        updateTimeProperty.SetValue(entity, now);
                    }
                }

                // 设置删除标记
                if (entity is HbtBaseEntity baseEntity)
                {
                    baseEntity.IsDeleted = 1;
                    baseEntity.DeleteTime = now;
                    if (string.IsNullOrEmpty(baseEntity.DeleteBy))
                    {
                        baseEntity.DeleteBy = userName;
                    }
                }
            }
            return await _db.Updateable(entities).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取用户角色列表
        /// </summary>
        public virtual async Task<List<string>> GetUserRolesAsync(long userId)
        {
            var roles = await _db.Queryable<HbtUserRole>()
                .LeftJoin<HbtRole>((ur, r) => ur.RoleId == r.Id)
                .Where(ur => ur.UserId == userId)
                .Select((ur, r) => r.RoleKey)
                .ToListAsync() ?? new List<string>();

            return roles.Where(r => !string.IsNullOrEmpty(r)).ToList();
        }

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        public virtual async Task<List<string>> GetUserPermissionsAsync(long userId)
        {
            _logger.Info("[权限仓储] 开始查询用户权限: UserId={UserId}", userId);

            var permissionStrings = await _db.Queryable<HbtUserRole>()
                .LeftJoin<HbtRole>((ur, r) => ur.RoleId == r.Id)
                .LeftJoin<HbtRoleMenu>((ur, r, rm) => r.Id == rm.RoleId)
                .LeftJoin<HbtMenu>((ur, r, rm, m) => rm.MenuId == m.Id)
                .Where(ur => ur.UserId == userId)
                .Select((ur, r, rm, m) => new { Perms = m.Perms })
                .MergeTable()
                .Where(x => x.Perms != null && x.Perms != string.Empty)
                .Select(x => x.Perms)
                .ToListAsync() ?? new List<string>();

            var permissions = permissionStrings
                .SelectMany(perms => perms.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .ToList();

            return permissions;
        }

        /// <summary>
        /// 获取用户租户列表
        /// </summary>
        public virtual async Task<List<long>> GetUserTenantsAsync(long userId)
        {
            _logger.Info("[租户仓储] 开始查询用户租户: UserId={UserId}", userId);

            var tenantIds = await _db.Queryable<HbtUserTenant>()
                .Where(ut => ut.UserId == userId && ut.IsDeleted == 0)
                .Select(ut => ut.TenantId)
                .ToListAsync() ?? new List<long>();

            return tenantIds;
        }

        /// <summary>
        /// 获取用户岗位列表
        /// </summary>
        public virtual async Task<List<long>> GetUserPostsAsync(long userId)
        {
            _logger.Info("[岗位仓储] 开始查询用户岗位: UserId={UserId}", userId);

            var postIds = await _db.Queryable<HbtUserPost>()
                .Where(up => up.UserId == userId && up.IsDeleted == 0)
                .Select(up => up.PostId)
                .ToListAsync() ?? new List<long>();

            return postIds;
        }

        /// <summary>
        /// 获取用户部门列表
        /// </summary>
        public virtual async Task<List<long>> GetUserDeptsAsync(long userId)
        {
            _logger.Info("[部门仓储] 开始查询用户部门: UserId={UserId}", userId);

            var deptIds = await _db.Queryable<HbtUserDept>()
                .Where(ud => ud.UserId == userId && ud.IsDeleted == 0)
                .Select(ud => ud.DeptId)
                .ToListAsync() ?? new List<long>();

            return deptIds;
        }

        /// <summary>
        /// 获取角色菜单列表
        /// </summary>
        public virtual async Task<List<long>> GetRoleMenusAsync(long roleId)
        {
            _logger.Info("[菜单仓储] 开始查询角色菜单: RoleId={RoleId}", roleId);

            var menuIds = await _db.Queryable<HbtRoleMenu>()
                .Where(rm => rm.RoleId == roleId && rm.IsDeleted == 0)
                .Select(rm => rm.MenuId)
                .ToListAsync() ?? new List<long>();

            return menuIds;
        }

    }
}