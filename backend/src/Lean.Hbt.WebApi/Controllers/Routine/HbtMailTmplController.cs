//===================================================================
// 项目名 : Lean.Hbt.WebApi
// 文件名 : HbtMailTmplController.cs
// 创建者 : Lean365
// 创建时间: 2024-03-07 16:30
// 版本号 : V1.0.0
// 描述   : 邮件模板控制器
//===================================================================

using Lean.Hbt.Application.Dtos.Routine;
using Lean.Hbt.Application.Services.Routine;

namespace Lean.Hbt.WebApi.Controllers.Routine
{
    /// <summary>
    /// 邮件模板控制器
    /// </summary>
    [Route("api/[controller]", Name = "邮件模板")]
    [ApiController]
    [ApiModule("routine", "日常办公")]
    public class HbtMailTmplController : HbtBaseController
    {
        private readonly IHbtMailTmplService _tmplService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tmplService">邮件模板服务</param>
        /// <param name="localization">本地化服务</param>
        /// <param name="logger">日志服务</param>
        public HbtMailTmplController(
            IHbtMailTmplService tmplService,
                        IHbtLocalizationService localization,
            IHbtLogger logger) : base(localization, logger)
        {
            _tmplService = tmplService;
        }

        /// <summary>
        /// 获取邮件模板分页列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>邮件模板分页列表</returns>
        [HttpGet("list")]
        [HbtPerm("routine:mail:template:list")]
        public async Task<HbtPagedResult<HbtMailTmplDto>> GetListAsync([FromQuery] HbtMailTmplQueryDto query)
        {
            return await _tmplService.GetListAsync(query);
        }

        /// <summary>
        /// 获取邮件模板详情
        /// </summary>
        /// <param name="tmplId">模板ID</param>
        /// <returns>邮件模板详情</returns>
        [HttpGet("{tmplId}")]
        [HbtPerm("routine:mail:template:query")]
        public async Task<HbtMailTmplDto> GetByIdAsync(long tmplId)
        {
            return await _tmplService.GetByIdAsync(tmplId);
        }

        /// <summary>
        /// 创建邮件模板
        /// </summary>
        /// <param name="input">创建对象</param>
        /// <returns>邮件模板ID</returns>
        [HttpPost]
        [HbtPerm("routine:mail:template:create")]
        public async Task<long> CreateAsync([FromBody] HbtMailTmplCreateDto input)
        {
            return await _tmplService.CreateAsync(input);
        }

        /// <summary>
        /// 更新邮件模板
        /// </summary>
        /// <param name="tmplId">模板ID</param>
        /// <param name="input">更新对象</param>
        /// <returns>是否成功</returns>
        [HttpPut("{tmplId}")]
        [HbtPerm("routine:mail:template:update")]
        public async Task<bool> UpdateAsync(long tmplId, [FromBody] HbtMailTmplDto input)
        {
            return await _tmplService.UpdateAsync(tmplId, input);
        }

        /// <summary>
        /// 删除邮件模板
        /// </summary>
        /// <param name="tmplId">模板ID</param>
        /// <returns>是否成功</returns>
        [HttpDelete("{tmplId}")]
        [HbtPerm("routine:mail:template:delete")]
        public async Task<bool> DeleteAsync(long tmplId)
        {
            return await _tmplService.DeleteAsync(tmplId);
        }

        /// <summary>
        /// 批量删除邮件模板
        /// </summary>
        /// <param name="tmplIds">模板ID数组</param>
        /// <returns>是否成功</returns>
        [HttpDelete("batch")]
        [HbtPerm("routine:mail:template:delete")]
        public async Task<bool> BatchDeleteAsync([FromBody] long[] tmplIds)
        {
            return await _tmplService.BatchDeleteAsync(tmplIds);
        }

        /// <summary>
        /// 导出邮件模板数据
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns>Excel文件</returns>
        [HttpGet("export")]
        [HbtPerm("routine:mail:template:export")]
        public async Task<IActionResult> ExportAsync([FromQuery] HbtMailTmplQueryDto query)
        {
            var data = await _tmplService.ExportAsync(query);
            return File(data.content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", data.fileName);
        }
    }
}