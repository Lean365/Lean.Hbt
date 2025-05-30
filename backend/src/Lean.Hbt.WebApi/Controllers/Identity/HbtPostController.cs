//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtPostController.cs
// 创建者 : Lean365
// 创建时间: 2024-01-20 16:30
// 版本号 : V0.0.1
// 描述   : 岗位控制器
//===================================================================

using Lean.Hbt.Application.Dtos.Identity;
using Lean.Hbt.Application.Services.Identity;

namespace Lean.Hbt.WebApi.Controllers.Identity
{
    /// <summary>
    /// 岗位控制器
    /// </summary>
    /// <remarks>
    /// 创建者: Lean365
    /// 创建时间: 2024-01-20
    /// </remarks>
    [Route("api/[controller]", Name = "岗位")]
    [ApiController]
    [ApiModule("identity", "身份认证")]
    public class HbtPostController : HbtBaseController
    {
        private readonly IHbtPostService _postService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="postService">岗位服务</param>
        /// <param name="logger">日志服务</param>
        /// <param name="currentUser">当前用户服务</param>
        /// <param name="currentTenant">当前租户服务</param>
        /// <param name="localization">本地化服务</param>
        public HbtPostController(
            IHbtPostService postService,
            IHbtLogger logger,
            IHbtCurrentUser currentUser,
            IHbtCurrentTenant currentTenant,
            IHbtLocalizationService localization) : base(logger, currentUser, currentTenant, localization)
        {
            _postService = postService;
        }

        /// <summary>
        /// 获取岗位分页列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>岗位分页列表</returns>
        [HttpGet("list")]
        [HbtPerm("identity:post:query")]
        public async Task<IActionResult> GetListAsync([FromQuery] HbtPostQueryDto query)
        {
            var result = await _postService.GetListAsync(query);
            return Success(result);
        }

        /// <summary>
        /// 获取岗位详情
        /// </summary>
        /// <param name="postId">岗位ID</param>
        /// <returns>岗位详情</returns>
        [HttpGet("{postId}")]
        [HbtPerm("identity:post:query")]
        public async Task<IActionResult> GetByIdAsync(long postId)
        {
            var result = await _postService.GetByIdAsync(postId);
            return Success(result);
        }

        /// <summary>
        /// 创建岗位
        /// </summary>
        /// <param name="input">创建对象</param>
        /// <returns>岗位ID</returns>
        [HttpPost]
        [HbtPerm("identity:post:create")]
        [HbtLog("创建岗位")]
        public async Task<IActionResult> CreateAsync([FromBody] HbtPostCreateDto input)
        {
            var result = await _postService.CreateAsync(input);
            return Success(result);
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        /// <param name="input">更新对象</param>
        /// <returns>是否成功</returns>
        [HttpPut]
        [HbtPerm("identity:post:update")]
        [HbtLog("更新岗位")]
        public async Task<IActionResult> UpdateAsync([FromBody] HbtPostUpdateDto input)
        {
            var result = await _postService.UpdateAsync(input);
            return Success(result);
        }

        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="postId">岗位ID</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{postId}")]
        [HbtPerm("identity:post:delete")]
        [HbtLog("删除岗位")]
        public async Task<IActionResult> DeleteAsync(long postId)
        {
            var result = await _postService.DeleteAsync(postId);
            return Success(result);
        }

        /// <summary>
        /// 批量删除岗位
        /// </summary>
        /// <param name="postIds">岗位ID集合</param>
        /// <returns>是否成功</returns>
        [HttpDelete("batch")]
        [HbtPerm("identity:post:delete")]
        [HbtLog("批量删除岗位")]
        public async Task<IActionResult> BatchDeleteAsync([FromBody] long[] postIds)
        {
            var result = await _postService.BatchDeleteAsync(postIds);
            return Success(result);
        }

        /// <summary>
        /// 导入岗位数据
        /// </summary>
        /// <param name="file">Excel文件</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>导入结果</returns>
        [HttpPost("import")]
        [HbtPerm("identity:post:import")]
        [HbtLog("导入岗位数据")]
        public async Task<IActionResult> ImportAsync(IFormFile file, [FromQuery] string sheetName = "Sheet1")
        {
            using var stream = file.OpenReadStream();
            var result = await _postService.ImportAsync(stream, sheetName);
            return Success(result);
        }

        /// <summary>
        /// 导出岗位数据
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>导出的Excel文件</returns>
        [HttpGet("export")]
        [HbtPerm("identity:post:export")]
        public async Task<IActionResult> ExportAsync([FromQuery] HbtPostQueryDto query, [FromQuery] string sheetName = "岗位数据")
        {
            var (fileName, content) = await _postService.ExportAsync(query, sheetName);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// 获取导入模板
        /// </summary>
        /// <param name="sheetName">工作表名称</param>
        /// <returns>导入模板Excel文件</returns>
        [HttpGet("template")]
        [HbtPerm("identity:post:import")]
        public async Task<IActionResult> GetImportTemplateAsync([FromQuery] string sheetName = "岗位导入模板")
        {
            var result = await _postService.GenerateTemplateAsync(sheetName);
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "岗位导入模板.xlsx");
        }

        /// <summary>
        /// 更新岗位状态
        /// </summary>
        /// <param name="postId">岗位ID</param>
        /// <param name="status">状态</param>
        /// <returns>是否成功</returns>
        [HttpPut("{postId}/status")]
        [HbtPerm("identity:post:update")]
        [HbtLog("更新岗位状态")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatusAsync(long postId, [FromQuery] int status)
        {
            var input = new HbtPostStatusDto
            {
                PostId = postId,
                Status = status
            };
            var result = await _postService.UpdateStatusAsync(input);
            return Success(result);
        }

        /// <summary>
        /// 获取岗位选项列表
        /// </summary>
        /// <returns>岗位选项列表</returns>
        [HttpGet("options")]
        [HbtPerm("identity:post:query")]
        public async Task<IActionResult> GetOptionsAsync()
        {
            var posts = await _postService.GetOptionsAsync();
            return Success(posts);
        }

        /// <summary>
        /// 获取岗位用户列表
        /// </summary>
        /// <param name="postId">岗位ID</param>
        /// <returns>岗位用户列表</returns>
        [HttpGet("{postId}/users")]
        [HbtPerm("identity:post:query")]
        public async Task<IActionResult> GetPostUsersAsync(long postId)
        {
            var result = await _postService.GetPostUsersAsync(postId);
            return Success(result);
        }

        /// <summary>
        /// 分配岗位用户
        /// </summary>
        /// <param name="postId">岗位ID</param>
        /// <param name="userIds">用户ID集合</param>
        /// <returns>是否成功</returns>
        [HttpPost("{postId}/users")]
        [HbtPerm("identity:post:allocate")]
        [HbtLog("分配岗位用户")]
        public async Task<IActionResult> AssignPostUsersAsync(long postId, [FromBody] long[] userIds)
        {
            var result = await _postService.AssignPostUsersAsync(postId, userIds);
            return Success(result);
        }

        /// <summary>
        /// 移除岗位用户
        /// </summary>
        /// <param name="postId">岗位ID</param>
        /// <param name="userIds">用户ID集合</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{postId}/users")]
        [HbtPerm("identity:post:allocate")]
        [HbtLog("移除岗位用户")]
        public async Task<IActionResult> RemovePostUsersAsync(long postId, [FromBody] long[] userIds)
        {
            var result = await _postService.RemovePostUsersAsync(postId, userIds);
            return Success(result);
        }
    }
}