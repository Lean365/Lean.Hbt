#nullable enable

//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtDeviceExtendService.cs
// 创建者 : Lean365
// 创建时间: 2024-01-22 14:30
// 版本号 : V1.0.0
// 描述    : 设备扩展信息服务实现
//===================================================================

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mapster;
using SqlSugar;
using Lean.Hbt.Application.Dtos.Identity;
using Lean.Hbt.Common.Models;
using Lean.Hbt.Domain.Entities.Identity;
using Lean.Hbt.Common.Extensions;
using Lean.Hbt.Domain.IServices;
using Lean.Hbt.Common.Enums;
using Lean.Hbt.Domain.Repositories;
using Lean.Hbt.Common.Helpers;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Lean.Hbt.Application.Services.Identity
{
    /// <summary>
    /// 设备扩展信息服务实现
    /// </summary>
    public class HbtDeviceExtendService : IHbtDeviceExtendService
    {
        private readonly IHbtRepository<HbtDeviceExtend> _deviceExtendRepository;
        private readonly ILogger<HbtDeviceExtendService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HbtDeviceExtendService(
            IHbtRepository<HbtDeviceExtend> deviceExtendRepository,
            ILogger<HbtDeviceExtendService> logger)
        {
            _deviceExtendRepository = deviceExtendRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取设备扩展信息分页列表
        /// </summary>
        public async Task<HbtPagedResult<HbtDeviceExtendDto>> GetListAsync(HbtDeviceExtendQueryDto query)
        {
            var exp = Expressionable.Create<HbtDeviceExtend>();

            if (query.UserId.HasValue)
                exp.And(x => x.UserId == query.UserId.Value);

            if (query.TenantId.HasValue)
                exp.And(x => x.TenantId == query.TenantId.Value);

            if (query.DeviceType.HasValue)
                exp.And(x => x.DeviceType == query.DeviceType.Value);

            if (!string.IsNullOrEmpty(query.DeviceId))
                exp.And(x => x.DeviceId.Contains(query.DeviceId));

            if (!string.IsNullOrEmpty(query.DeviceName))
                exp.And(x => x.DeviceName.Contains(query.DeviceName));

            if (query.LoginStatus.HasValue)
                exp.And(x => x.DeviceStatus == query.LoginStatus.Value);

            if (query.LastOnlineTimeStart.HasValue)
                exp.And(x => x.LastOnlineTime >= query.LastOnlineTimeStart.Value);

            if (query.LastOnlineTimeEnd.HasValue)
                exp.And(x => x.LastOnlineTime <= query.LastOnlineTimeEnd.Value);

            var result = await _deviceExtendRepository.GetPagedListAsync(
                exp.ToExpression(),
                query.PageIndex,
                query.PageSize,
                x => x.OrderNum,
                OrderByType.Asc);

            return new HbtPagedResult<HbtDeviceExtendDto>
            {
                Rows = result.Rows.Adapt<List<HbtDeviceExtendDto>>(),
                TotalNum = result.TotalNum,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }

        /// <summary>
        /// 导出设备扩展信息
        /// </summary>
        public async Task<byte[]> ExportAsync(IEnumerable<HbtDeviceExtendDto> data, string sheetName = "设备扩展信息")
        {
            return await HbtExcelHelper.ExportAsync(data, sheetName);
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        public async Task<HbtDeviceExtendDto> UpdateDeviceInfoAsync(HbtDeviceExtendUpdateDto request)
        {
            var now = DateTime.Now;
            var deviceExtend = await _deviceExtendRepository.GetInfoAsync(x =>
                x.UserId == request.UserId &&
                x.DeviceId == request.DeviceId);

            if (deviceExtend == null)
            {
                deviceExtend = new HbtDeviceExtend
                {
                    UserId = request.UserId,
                    TenantId = request.TenantId,
                    DeviceId = request.DeviceId,
                    DeviceName = request.DeviceName,
                    DeviceType = request.DeviceType,
                    BrowserType = request.BrowserType,
                    BrowserVersion = request.BrowserVersion,
                    Resolution = request.Resolution,
                    DeviceStatus = (int)HbtDeviceStatus.Online,
                    LastOnlineTime = now
                };

                await _deviceExtendRepository.CreateAsync(deviceExtend);
            }
            else
            {
                deviceExtend.DeviceName = request.DeviceName;
                deviceExtend.DeviceType = request.DeviceType;
                deviceExtend.BrowserType = request.BrowserType;
                deviceExtend.BrowserVersion = request.BrowserVersion;
                deviceExtend.Resolution = request.Resolution;
                deviceExtend.DeviceStatus = (int)HbtDeviceStatus.Online;
                deviceExtend.LastOnlineTime = now;

                await _deviceExtendRepository.UpdateAsync(deviceExtend);
            }

            return deviceExtend.Adapt<HbtDeviceExtendDto>();
        }

        /// <summary>
        /// 更新设备离线信息
        /// </summary>
        public async Task<HbtDeviceExtendDto> UpdateOfflineInfoAsync(long userId, string deviceId)
        {
            var deviceExtend = await _deviceExtendRepository.GetInfoAsync(x =>
                x.UserId == userId &&
                x.DeviceId == deviceId);

            if (deviceExtend == null)
            {
                _logger.LogWarning("设备扩展信息不存在: userId={UserId}, deviceId={DeviceId}", userId, deviceId);
                throw new InvalidOperationException($"设备扩展信息不存在: userId={userId}, deviceId={deviceId}");
            }

            deviceExtend.DeviceStatus = (int)HbtDeviceStatus.Offline;
            deviceExtend.LastOfflineTime = DateTime.Now;

            await _deviceExtendRepository.UpdateAsync(deviceExtend);
            return deviceExtend.Adapt<HbtDeviceExtendDto>();
        }

        /// <summary>
        /// 更新设备在线时段
        /// </summary>
        public async Task<HbtDeviceExtendDto> UpdateOnlinePeriodAsync(HbtDeviceOnlinePeriodUpdateDto request)
        {
            var deviceExtend = await _deviceExtendRepository.GetInfoAsync(
                x => x.UserId == request.UserId && x.DeviceId == request.DeviceId);
            if (deviceExtend == null)
            {
                throw new InvalidOperationException($"用户{request.UserId}的设备{request.DeviceId}扩展信息不存在");
            }

            deviceExtend.TodayOnlinePeriods = request.OnlinePeriod;
            await _deviceExtendRepository.UpdateAsync(deviceExtend);

            return deviceExtend.Adapt<HbtDeviceExtendDto>();
        }

        /// <summary>
        /// 获取用户的设备扩展信息
        /// </summary>
        public async Task<HbtDeviceExtendDto?> GetByUserIdAndDeviceIdAsync(long userId, string deviceId)
        {
            var deviceExtend = await _deviceExtendRepository.GetInfoAsync(
                x => x.UserId == userId && x.DeviceId == deviceId);
            return deviceExtend?.Adapt<HbtDeviceExtendDto>();
        }

        /// <summary>
        /// 获取用户的所有设备扩展信息
        /// </summary>
        public async Task<List<HbtDeviceExtendDto>> GetByUserIdAsync(long userId)
        {
            var deviceExtends = await _deviceExtendRepository.GetListAsync(x => x.UserId == userId);
            return deviceExtends.Adapt<List<HbtDeviceExtendDto>>();
        }
    }
} 