#nullable enable

//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtGenTableController.cs
// 创建者 : Lean365
// 创建时间: 2024-03-20
// 版本号 : V0.0.1
// 描述    : 代码生成表控制器
//===================================================================

using Lean.Hbt.Application.Dtos.Generator;
using Lean.Hbt.Application.Services.Generator;

namespace Lean.Hbt.WebApi.Controllers.Generator;

/// <summary>
/// 代码生成表控制器
/// </summary>
[Route("api/[controller]", Name = "代码生成表")]
[ApiController]
[ApiModule("generator", "代码生成")]
public class HbtGenTableController : HbtBaseController
{
    private readonly IHbtGenTableService _genTableService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="genTableService">代码生成表服务</param>
    /// <param name="localization">本地化服务</param>
    /// <param name="logger">日志服务</param>
    public HbtGenTableController(
        IHbtGenTableService genTableService,
            IHbtLocalizationService localization,
            IHbtLogger logger) : base(localization, logger)
    {
        _genTableService = genTableService;
    }

    /// <summary>
    /// 获取代码生成表分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页结果</returns>
    [HttpGet("list")]
    [HbtPerm("generator:table:list")]
    public async Task<IActionResult> GetList([FromQuery] HbtGenTableQueryDto query)
    {
        var result = await _genTableService.GetListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取代码生成表详情
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns>代码生成表信息</returns>
    [HttpGet("{id}")]
    [HbtPerm("generator:table:query")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _genTableService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }
        return Success(result);
    }

    /// <summary>
    /// 获取表字段列表
    /// </summary>
    /// <param name="tableId">表ID</param>
    /// <returns>字段列表</returns>
    [HttpGet("{tableId}/columns")]
    [HbtPerm("generator:table:query")]
    public async Task<IActionResult> GetColumnList(long tableId)
    {
        var result = await _genTableService.GetColumnListAsync(tableId);
        return Success(result);
    }

    /// <summary>
    /// 新增代码生成表
    /// </summary>
    /// <param name="dto">代码生成表信息</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    [HbtPerm("generator:table:create")]
    public async Task<IActionResult> Create([FromBody] HbtGenTableDto dto)
    {
        var result = await _genTableService.CreateAsync(dto);
        return Success(result);
    }

    /// <summary>
    /// 修改代码生成表
    /// </summary>
    /// <param name="dto">代码生成表信息</param>
    /// <returns>操作结果</returns>
    [HttpPut]
    [HbtPerm("generator:table:update")]
    public async Task<IActionResult> Update([FromBody] HbtGenTableUpdateDto dto)
    {
        var result = await _genTableService.UpdateAsync(dto);
        return Success(result);
    }

    /// <summary>
    /// 删除代码生成表
    /// </summary>
    /// <param name="id">主键ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    [HbtPerm("generator:table:delete")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _genTableService.DeleteAsync(id);
        return result
            ? Success("删除成功")
            : Error("删除失败");
    }

    /// <summary>
    /// 导入代码生成表
    /// </summary>
    /// <param name="input">导入参数</param>
    /// <returns>操作结果</returns>
    [HttpPost("import")]
    [HbtPerm("generator:table:import")]
    public async Task<IActionResult> Import([FromBody] HbtGenTableImportDto input)
    {
        var result = await _genTableService.ImportTablesAsync(input);
        return Success(result);
    }

    /// <summary>
    /// 导出代码生成表
    /// </summary>
    /// <returns>操作结果</returns>
    [HttpGet("export")]
    [HbtPerm("generator:table:export")]
    public async Task<IActionResult> Export()
    {
        var result = await _genTableService.ExportTablesAsync();
        return Success(result);
    }

    /// <summary>
    /// 获取数据库列表
    /// </summary>
    /// <returns>数据库列表</returns>
    [HttpGet("databases")]
    [HbtPerm("generator:table:query")]
    public async Task<IActionResult> GetDatabaseList()
    {
        var result = await _genTableService.GetDatabaseListAsync();
        return Success(result);
    }

    /// <summary>
    /// 获取表列表
    /// </summary>
    /// <param name="databaseName">数据库名称</param>
    /// <returns>表列表</returns>
    [HttpGet("tables/{databaseName}")]
    [HbtPerm("generator:table:query")]
    public async Task<IActionResult> GetTableList(string databaseName)
    {
        var result = await _genTableService.GetTableListAsync(databaseName);
        return Success(result);
    }

    /// <summary>
    /// 获取表字段列表
    /// </summary>
    /// <param name="databaseName">数据库名称</param>
    /// <param name="tableName">表名</param>
    /// <returns>字段列表</returns>
    [HttpGet("columns/{databaseName}/{tableName}")]
    [HbtPerm("generator:table:query")]
    public async Task<IActionResult> GetTableColumnList(string databaseName, string tableName)
    {
        var result = await _genTableService.GetTableColumnListAsync(databaseName, tableName);
        return Success(result);
    }

    /// <summary>
    /// 同步表结构
    /// </summary>
    /// <param name="id">表ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/sync")]
    [HbtPerm("generator:table:sync")]
    public async Task<IActionResult> SyncTable(long id)
    {
        var result = await _genTableService.SyncTableAsync(id);
        return result
            ? Success("同步成功")
            : Error("同步失败");
    }

    /// <summary>
    /// 预览代码
    /// </summary>
    /// <param name="id">表ID</param>
    /// <returns>预览结果</returns>
    [HttpGet("{id}/preview")]
    [HbtPerm("generator:table:preview")]
    public async Task<IActionResult> PreviewCode(long id)
    {
        var result = await _genTableService.PreviewCodeAsync(id);
        return Success(result);
    }

    /// <summary>
    /// 生成代码
    /// </summary>
    /// <param name="id">表ID</param>
    /// <returns>操作结果</returns>
    [HttpPost("{id}/generate")]
    [HbtPerm("generator:table:generate")]
    public async Task<IActionResult> GenerateCode(long id)
    {
        var result = await _genTableService.GenerateCodeAsync(id);
        return result
            ? Success("生成成功")
            : Error("生成失败");
    }

    /// <summary>
    /// 批量生成代码
    /// </summary>
    /// <param name="ids">表ID集合</param>
    /// <returns>操作结果</returns>
    [HttpPost("batch-generate")]
    [HbtPerm("generator:table:generate")]
    public async Task<IActionResult> BatchGenerateCode([FromBody] long[] ids)
    {
        var result = await _genTableService.BatchGenerateCodeAsync(ids);
        return Success(result);
    }

    /// <summary>
    /// 下载代码
    /// </summary>
    /// <param name="id">表ID</param>
    /// <returns>代码文件</returns>
    [HttpGet("{id}/download")]
    [HbtPerm("generator:table:download")]
    public async Task<IActionResult> DownloadCode(long id)
    {
        var result = await _genTableService.DownloadCodeAsync(id);
        return File(result, "application/zip", "code.zip");
    }

    /// <summary>
    /// 导入表及其所有字段信息
    /// </summary>
    /// <param name="databaseName">数据库名</param>
    /// <param name="tableName">表名</param>
    /// <returns>操作结果</returns>
    [HttpPost("import-table-and-columns/{databaseName}/{tableName}")]
    [HbtPerm("generator:table:import")]
    public async Task<IActionResult> ImportTableAndColumns(string databaseName, string tableName)
    {
        var result = await _genTableService.ImportTableAndColumnsAsync(databaseName, tableName);
        return result ? Success("导入成功") : Error("导入失败");
    }
}