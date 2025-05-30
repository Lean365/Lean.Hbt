//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtMailTplService.cs
// 创建者 : Lean365
// 创建时间: 2024-03-07 16:30
// 版本号 : V1.0.0
// 描述   : 邮件模板服务实现
//===================================================================

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Lean.Hbt.Common.Models;
using Lean.Hbt.Domain.Entities.Routine;
using Lean.Hbt.Application.Dtos.Routine;
using Lean.Hbt.Common.Exceptions;
using Lean.Hbt.Common.Helpers;
using Lean.Hbt.Domain.Repositories;
using SqlSugar;
using Mapster;

namespace Lean.Hbt.Application.Services.Routine
{
    /// <summary>
    /// 邮件模板服务实现
    /// </summary>
    public class HbtMailTplService : HbtBaseService, IHbtMailTplService
    {
        private readonly IHbtRepository<HbtMailTpl> _tmplRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="tmplRepository">模板仓储</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器</param>
        /// <param name="currentUser">当前用户服务</param>
        /// <param name="currentTenant">当前租户服务</param>
        /// <param name="localization">本地化服务</param>
        public HbtMailTplService(
            IHbtLogger logger,
            IHbtRepository<HbtMailTpl> tmplRepository,
            IHttpContextAccessor httpContextAccessor,
            IHbtCurrentUser currentUser,
            IHbtCurrentTenant currentTenant,
            IHbtLocalizationService localization) : base(logger, httpContextAccessor, currentUser, currentTenant, localization)
        {
            _tmplRepository = tmplRepository;
        }

        /// <summary>
        /// 获取邮件模板分页列表
        /// </summary>
        public async Task<HbtPagedResult<HbtMailTplDto>> GetListAsync(HbtMailTplQueryDto query)
        {
            var exp = Expressionable.Create<HbtMailTpl>();

            if (!string.IsNullOrEmpty(query.TmplName))
                exp.And(x => x.TmplName.Contains(query.TmplName));

            if (!string.IsNullOrEmpty(query.TmplCode))
                exp.And(x => x.TmplCode.Contains(query.TmplCode));

            if (query.TmplStatus.HasValue)
                exp.And(x => x.TmplStatus == query.TmplStatus.Value);

            if (query.StartTime.HasValue)
                exp.And(x => x.CreateTime >= query.StartTime.Value);

            if (query.EndTime.HasValue)
                exp.And(x => x.CreateTime <= query.EndTime.Value);

            var result = await _tmplRepository.GetPagedListAsync(
                exp.ToExpression(),
                query.PageIndex,
                query.PageSize,
                x => x.Id,
                OrderByType.Asc);

            return new HbtPagedResult<HbtMailTplDto>
            {
                TotalNum = result.TotalNum,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                Rows = result.Rows.Adapt<List<HbtMailTplDto>>()
            };
        }

        /// <summary>
        /// 获取邮件模板详情
        /// </summary>
        public async Task<HbtMailTplDto> GetByIdAsync(long tmplId)
        {
            var tmpl = await _tmplRepository.GetByIdAsync(tmplId);
            if (tmpl == null)
                throw new HbtException(L("MailTmpl.NotFound", tmplId));

            return tmpl.Adapt<HbtMailTplDto>();
        }

        /// <summary>
        /// 创建邮件模板
        /// </summary>
        public async Task<long> CreateAsync(HbtMailTplCreateDto input)
        {
            var tmpl = input.Adapt<HbtMailTpl>();
            tmpl.CreateTime = DateTime.Now;

            // 验证模板编码是否已存在
            var existingTmpl = await _tmplRepository.GetFirstAsync(x => x.TmplCode == input.TmplCode);
            if (existingTmpl != null)
                throw new HbtException(L("MailTmpl.CodeExists", input.TmplCode));

            var result = await _tmplRepository.CreateAsync(tmpl);
            if (result <= 0)
                throw new HbtException(L("MailTmpl.CreateFailed"));

            return tmpl.Id;
        }

        /// <summary>
        /// 更新邮件模板
        /// </summary>
        public async Task<bool> UpdateAsync(long tmplId, HbtMailTplDto input)
        {
            var tmpl = await _tmplRepository.GetByIdAsync(tmplId);
            if (tmpl == null)
                throw new HbtException(L("MailTmpl.NotFound", tmplId));

            // 验证模板编码是否已存在
            var existingTmpl = await _tmplRepository.GetFirstAsync(x => x.TmplCode == input.TmplCode && x.Id != tmplId);
            if (existingTmpl != null)
                throw new HbtException(L("MailTmpl.CodeExists", input.TmplCode));

            input.Adapt(tmpl);
            var result = await _tmplRepository.UpdateAsync(tmpl);
            return result > 0;
        }

        /// <summary>
        /// 删除邮件模板
        /// </summary>
        public async Task<bool> DeleteAsync(long tmplId)
        {
            var tmpl = await _tmplRepository.GetByIdAsync(tmplId);
            if (tmpl == null)
                throw new HbtException(L("MailTmpl.NotFound", tmplId));

            var result = await _tmplRepository.DeleteAsync(tmplId);
            return result > 0;
        }

        /// <summary>
        /// 批量删除邮件模板
        /// </summary>
        public async Task<bool> BatchDeleteAsync(long[] tmplIds)
        {
            if (tmplIds == null || tmplIds.Length == 0)
                throw new HbtException(L("MailTmpl.SelectToDelete"));

            foreach (var tmplId in tmplIds)
            {
                await DeleteAsync(tmplId);
            }
            return true;
        }

        /// <summary>
        /// 导出邮件模板数据
        /// </summary>
        public async Task<(string fileName, byte[] content)> ExportAsync(HbtMailTplQueryDto query, string sheetName = "MailTmpl")
        {
            try
            {
                var list = await _tmplRepository.GetListAsync(KpMailTmplQueryExpression(query));
                return await HbtExcelHelper.ExportAsync(list.Adapt<List<HbtMailTplExportDto>>(), sheetName, L("MailTmpl.ExportTitle"));
            }
            catch (Exception ex)
            {
                _logger.Error(L("MailTmpl.ExportFailed"), ex);
                throw new HbtException(L("MailTmpl.ExportFailed"));
            }
        }

        private Expression<Func<HbtMailTpl, bool>> KpMailTmplQueryExpression(HbtMailTplQueryDto query)
        {
            return Expressionable.Create<HbtMailTpl>()
                .AndIF(!string.IsNullOrEmpty(query.TmplName), x => x.TmplName.Contains(query.TmplName))
                .AndIF(!string.IsNullOrEmpty(query.TmplCode), x => x.TmplCode.Contains(query.TmplCode))
                .AndIF(query.TmplStatus.HasValue, x => x.TmplStatus == query.TmplStatus.Value)
                .AndIF(query.StartTime.HasValue, x => x.CreateTime >= query.StartTime.Value)
                .AndIF(query.EndTime.HasValue, x => x.CreateTime <= query.EndTime.Value)
                .ToExpression();
        }
    }
} 