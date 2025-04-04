//===================================================================
// 项目名 : Lean.Hbt
// 文件名 : HbtServerMonitorService.cs
// 创建者 : Lean365
// 创建时间: 2024-02-18 11:00
// 版本号 : V0.0.1
// 描述   : 服务器监控服务实现
//===================================================================

using System.Diagnostics;
using System.Runtime.InteropServices;
using Lean.Hbt.Application.Dtos.RealTime;
using Lean.Hbt.Common.Utils;
using LibreHardwareMonitor.Hardware;

namespace Lean.Hbt.Application.Services.RealTime;

/// <summary>
/// 服务器监控服务实现
/// </summary>
/// <remarks>
/// 提供以下功能：
/// 1. 获取服务器基本信息：操作系统、处理器、内存等
/// 2. 获取网络接口信息：网卡、IP地址、流量等
/// 3. 实时监控系统资源使用情况
/// </remarks>
public class HbtServerMonitorService : IHbtServerMonitorService
{
    private readonly DateTime _startTime;

    /// <summary>
    /// 构造函数
    /// </summary>
    public HbtServerMonitorService()
    {
        _startTime = DateTime.Now;
    }

    /// <summary>
    /// 获取服务器基本信息
    /// </summary>
    /// <returns>服务器基本信息DTO对象</returns>
    /// <remarks>
    /// 包含以下信息：
    /// - 操作系统信息：名称、版本、架构
    /// - 处理器信息：名称、核心数
    /// - 系统运行时间
    /// - .NET运行时信息
    /// - 资源使用情况：CPU、内存、磁盘
    /// </remarks>
    public async Task<HbtServerMonitorDto> GetServerInfoAsync()
    {
        return await Task.Run(() =>
        {
            var dto = new HbtServerMonitorDto
            {
                // 操作系统信息
                OsName = RuntimeInformation.OSDescription,
                OsArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                OsVersion = RuntimeInformation.OSDescription,
                
                // 处理器信息
                ProcessorName = HbtServerMonitorUtils.GetProcessorName(),
                ProcessorCount = Environment.ProcessorCount,
                
                // 系统运行时间
                SystemStartTime = HbtServerMonitorUtils.GetSystemStartTime(),
                SystemUptime = (DateTime.Now - HbtServerMonitorUtils.GetSystemStartTime()).TotalDays,
                
                // .NET运行时信息
                DotNetRuntimeVersion = RuntimeInformation.FrameworkDescription,
                ClrVersion = Environment.Version.ToString(),
                DotNetRuntimeDirectory = RuntimeEnvironment.GetRuntimeDirectory()
            };
            
            try
            {
                // 获取内存信息（转换为GB）
                var totalMemoryBytes = HbtServerMonitorUtils.GetTotalPhysicalMemory();
                var totalMemoryGB = totalMemoryBytes / (1024.0 * 1024 * 1024);
                dto.TotalMemory = totalMemoryGB;

                // 获取CPU使用率
                dto.CpuUsage = HbtServerMonitorUtils.GetSystemCpuUsage();

                // 获取磁盘信息（转换为GB）
                var drives = DriveInfo.GetDrives();
                var totalDiskSpace = drives.Sum(d => d.TotalSize) / (1024.0 * 1024 * 1024);
                var usedDiskSpace = drives.Sum(d => d.TotalSize - d.AvailableFreeSpace) / (1024.0 * 1024 * 1024);
                dto.TotalDiskSpace = totalDiskSpace;
                dto.UsedDiskSpace = usedDiskSpace;
                dto.DiskUsage = usedDiskSpace / totalDiskSpace * 100;
            }
            catch (Exception ex)
            {
                // 记录错误日志
                Debug.WriteLine($"获取服务器信息时发生错误: {ex.Message}");
            }

            return dto;
        });
    }

    /// <summary>
    /// 获取网络信息
    /// </summary>
    /// <returns>网络信息DTO对象列表</returns>
    /// <remarks>
    /// 包含以下信息：
    /// - 网络适配器名称
    /// - MAC地址
    /// - IP地址及地理位置
    /// - 网络流量：发送和接收速率
    /// </remarks>
    public async Task<List<HbtNetworkDto>> GetNetworkInfoAsync()
    {
        var networkInfoList = new List<HbtNetworkDto>();

        try
        {
            // 获取所有网络接口信息
            var interfaces = HbtServerMonitorUtils.GetNetworkInterfaces();
            foreach (var ni in interfaces)
            {
                var networkInfo = new HbtNetworkDto
                {
                    AdapterName = ni.Name,
                    MacAddress = ni.MacAddress,
                    IpAddress = ni.IpAddress,
                    IpLocation = "Unknown Location"
                };

                // 获取IP地理位置信息
                networkInfo.IpLocation = await GetLocationAsyncAsync(networkInfo.IpAddress);

                // 获取网络流量信息（KB/s）
                var (sendRate, receiveRate) = HbtServerMonitorUtils.GetNetworkInterfaceRates(ni.Name);
                networkInfo.SendRate = sendRate;
                networkInfo.ReceiveRate = receiveRate;

                networkInfoList.Add(networkInfo);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"获取网络信息时发生错误: {ex.Message}");
        }

        return networkInfoList;
    }

    /// <summary>
    /// 获取IP地址的地理位置信息
    /// </summary>
    /// <param name="ipAddress">IP地址</param>
    /// <returns>地理位置描述</returns>
    /// <remarks>
    /// 使用HbtIpLocationUtils工具类获取IP地理位置信息
    /// </remarks>
    private async Task<string> GetLocationAsyncAsync(string ipAddress)
    {
        try
        {
            return await HbtIpLocationUtils.GetLocationAsync(ipAddress);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"获取IP地理位置信息时发生错误: {ex.Message}");
            return "Unknown Location";
        }
    }
}
