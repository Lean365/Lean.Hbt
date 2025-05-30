using System.IO.Compression;
using System.Linq.Expressions;
using Lean.Hbt.Common.Options;
using Lean.Hbt.Domain.Entities.Audit;
using Lean.Hbt.Domain.Entities.Core;
using Microsoft.Extensions.Options;

namespace Lean.Hbt.Application.Services.Extensions
{
    /// <summary>
    /// 日志归档服务实现类
    /// </summary>
    public class HbtLogArchiveService : IHbtLogArchiveService
    {
        private readonly IOptions<HbtLogArchiveOptions> _defaultOptions;
        private readonly IHbtRepository<HbtConfig> _configRepository;
        private readonly IHbtRepository<HbtOperLog> _operLogRepository;
        private readonly IHbtRepository<HbtLoginLog> _loginLogRepository;
        private readonly IHbtRepository<HbtExceptionLog> _exceptionLogRepository;
        private readonly IHbtRepository<HbtSqlDiffLog> _dbDiffLogRepository;
        private readonly IHbtLogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="defaultOptions"></param>
        /// <param name="configRepository"></param>
        /// <param name="operLogRepository"></param>
        /// <param name="loginLogRepository"></param>
        /// <param name="exceptionLogRepository"></param>
        /// <param name="dbDiffLogRepository"></param>
        /// <param name="logger"></param>
        public HbtLogArchiveService(
            IOptions<HbtLogArchiveOptions> defaultOptions,
            IHbtRepository<HbtConfig> configRepository,
            IHbtRepository<HbtOperLog> operLogRepository,
            IHbtRepository<HbtLoginLog> loginLogRepository,
            IHbtRepository<HbtExceptionLog> exceptionLogRepository,
            IHbtRepository<HbtSqlDiffLog> dbDiffLogRepository,
            IHbtLogger logger)
        {
            _defaultOptions = defaultOptions;
            _configRepository = configRepository;
            _operLogRepository = operLogRepository;
            _loginLogRepository = loginLogRepository;
            _exceptionLogRepository = exceptionLogRepository;
            _dbDiffLogRepository = dbDiffLogRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取日志归档配置
        /// </summary>
        public async Task<HbtLogArchiveOptions> GetConfigAsync()
        {
            var configs = await _configRepository.GetListAsync(c => c.ConfigKey.StartsWith("log.archive."));
            if (configs == null || !configs.Any())
                return _defaultOptions.Value;

            return new HbtLogArchiveOptions
            {
                Enabled = GetConfigValue(configs, "log.archive.enabled", _defaultOptions.Value.Enabled),
                ArchivePeriodMonths = GetConfigValue(configs, "log.archive.periodMonths", _defaultOptions.Value.ArchivePeriodMonths),
                ArchivePath = GetConfigValue(configs, "log.archive.path", _defaultOptions.Value.ArchivePath),
                FileNameFormat = GetConfigValue(configs, "log.archive.fileNameFormat", _defaultOptions.Value.FileNameFormat),
                BatchSize = GetConfigValue(configs, "log.archive.batchSize", _defaultOptions.Value.BatchSize),
                CronExpression = GetConfigValue(configs, "log.archive.cronExpression", _defaultOptions.Value.CronExpression)
            };
        }

        /// <summary>
        /// 执行日志归档
        /// </summary>
        public async Task ArchiveAsync()
        {
            var options = await GetConfigAsync();
            if (!options.Enabled)
            {
                _logger.Info("日志归档功能未启用");
                return;
            }

            var archiveDate = DateTime.Now.AddMonths(-options.ArchivePeriodMonths);
            var archivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.ArchivePath);
            Directory.CreateDirectory(archivePath);

            var fileName = string.Format(options.FileNameFormat, archiveDate);
            var archiveFile = Path.Combine(archivePath, fileName);

            using (var archive = ZipFile.Open(archiveFile, ZipArchiveMode.Create))
            {
                await ArchiveLogTable(archive, "hbt_oper_log.json", _operLogRepository, archiveDate, options.BatchSize);
                await ArchiveLogTable(archive, "hbt_login_log.json", _loginLogRepository, archiveDate, options.BatchSize);
                await ArchiveLogTable(archive, "hbt_exception_log.json", _exceptionLogRepository, archiveDate, options.BatchSize);
                await ArchiveLogTable(archive, "hbt_dbdiff_log.json", _dbDiffLogRepository, archiveDate, options.BatchSize);
            }

            _logger.Info($"日志归档完成: {archiveFile}");
        }

        /// <summary>
        /// 获取归档文件列表
        /// </summary>
        public async Task<List<string>> GetArchiveFilesAsync()
        {
            var options = await GetConfigAsync();
            var archivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.ArchivePath);

            if (!Directory.Exists(archivePath))
                return new List<string>();

            return Directory.GetFiles(archivePath, "*.zip")
                .Select(Path.GetFileName)
                .Where(name => name != null)
                .Cast<string>()
                .ToList();
        }

        /// <summary>
        /// 删除归档文件
        /// </summary>
        public async Task DeleteArchiveFileAsync(string fileName)
        {
            var options = await GetConfigAsync();
            var archivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.ArchivePath);
            var filePath = Path.Combine(archivePath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.Info($"删除归档文件: {fileName}");
            }
        }

        private async Task ArchiveLogTable<T>(ZipArchive archive, string entryName, IHbtRepository<T> repository, DateTime archiveDate, int batchSize)
            where T : HbtBaseEntity, new()
        {
            var entry = archive.CreateEntry(entryName);
            using var writer = new StreamWriter(entry.Open());

            var totalCount = 0;
            while (true)
            {
                var result = await repository.GetPagedListAsync(
                    x => x.CreateTime <= archiveDate,
                    1,
                    batchSize,
                    x => x.CreateTime,
                    OrderByType.Asc);

                if (result.Rows == null || !result.Rows.Any())
                    break;

                foreach (var log in result.Rows)
                {
                    await writer.WriteLineAsync(System.Text.Json.JsonSerializer.Serialize(log));
                    totalCount++;
                }

                var ids = result.Rows.Select(l => l.Id).ToList();
                Expression<Func<T, bool>> predicate = x => ids.Contains(x.Id);
                await repository.DeleteAsync(predicate);
            }

            _logger.Info($"已归档 {typeof(T).Name}: {totalCount} 条记录");
        }

        private T GetConfigValue<T>(List<HbtConfig> configs, string key, T defaultValue)
        {
            var config = configs.FirstOrDefault(c => c.ConfigKey == key);
            if (config == null)
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(config.ConfigValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}