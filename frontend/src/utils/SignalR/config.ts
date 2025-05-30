import { HubConnection, LogLevel, RetryContext, HubConnectionBuilder, HttpTransportType, IHttpConnectionOptions } from '@microsoft/signalr'
import { getToken } from '@/utils/auth'
import { getDeviceInfo } from '@/utils/device'
import { message } from 'ant-design-vue'

// SignalR 全局配置
export const signalRConfig = {
  logLevel: LogLevel.Information,
  reconnectInterval: 5000,
  maxRetries: 5,
  heartbeatInterval: 15000,  // 与后端 KeepAliveInterval 匹配
  heartbeatTimeout: 60000,   // 与后端 ClientTimeoutInterval 匹配
  baseUrl: `${import.meta.env.VITE_API_BASE_URL}/signalr/hbthub`,
  transport: HttpTransportType.LongPolling, // 首先使用 LongPolling
  debug: import.meta.env.DEV, // 仅在开发环境启用调试
  skipNegotiation: false,    // 允许协商过程
  serverTimeoutInMilliseconds: 30000, // 服务器超时时间
  keepAliveIntervalInMilliseconds: 15000, // 保持连接活跃的间隔
  maximumReceiveMessageSize: 10485760, // 10MB，与后端 MaximumReceiveMessageSize 匹配
  streamBufferCapacity: 10,  // 与后端 StreamBufferCapacity 匹配
  handshakeTimeout: 15000,   // 与后端 HandshakeTimeout 匹配
  closeTimeout: 5000,        // 与后端 WebSockets.CloseTimeout 匹配
  enableDetailedErrors: import.meta.env.DEV // 仅在开发环境启用详细错误
}

// 重试策略配置
export const retryPolicy = {
  maxRetries: 5,
  maxDelay: 30000, // 30秒
  baseDelay: 1000, // 1秒
  nextRetryDelayInMilliseconds: (retryContext: RetryContext) => {
    if (retryContext.previousRetryCount >= retryPolicy.maxRetries) {
      return null // 停止重试
    }
    
    // 指数退避算法
    const delay = Math.min(
      retryPolicy.baseDelay * Math.pow(2, retryContext.previousRetryCount),
      retryPolicy.maxDelay
    )
    
    console.log(`[SignalR] 第 ${retryContext.previousRetryCount + 1} 次重试,延迟 ${delay}ms`)
    return delay
  }
}

// 自定义日志记录器
const customLogger = {
  log(logLevel: LogLevel, message: string) {
    const now = new Date()
    const localTime = now.toLocaleString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
      timeZone: 'Asia/Shanghai'
    })
    
    // 如果消息中包含UTC时间戳，则不输出
    if (message.match(/^\[\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z\]/)) {
      return
    }
    
    console.log(`[${localTime}] ${LogLevel[logLevel]}: ${message}`)
  }
}

// 获取连接头信息
export const getConnectionHeaders = async (): Promise<Record<string, string>> => {
  const deviceInfo = await getDeviceInfo()
  console.log('[SignalR] 获取设备信息:', deviceInfo)

  if (!deviceInfo.deviceId) {
    throw new Error('无法获取设备ID')
  }

  const headers: Record<string, string> = {
    'X-Device-Id': deviceInfo.deviceId || '',
    'X-Device-Name': deviceInfo.deviceName || '',
    'X-Device-Type': (deviceInfo.deviceType || 0).toString(),
    'X-Device-Model': deviceInfo.deviceModel || '',
    'X-OS-Type': (deviceInfo.osType || 0).toString(),
    'X-OS-Version': deviceInfo.osVersion || '',
    'X-Browser-Type': (deviceInfo.browserType || 0).toString(),
    'X-Browser-Version': deviceInfo.browserVersion || '',
    'X-Resolution': deviceInfo.resolution || '',
    'X-Location': deviceInfo.location || '',
    'X-Language': navigator.language || '',
    'X-Timezone': deviceInfo.environment?.timezone || Intl.DateTimeFormat().resolvedOptions().timeZone || '',
    'X-User-Agent': navigator.userAgent,
    'X-Device-Fingerprint': deviceInfo.deviceFingerprint || ''
  }

  console.log('[SignalR] 生成的连接头信息:', headers)
  return headers
}

export const createHubConnection = async (): Promise<HubConnection> => {
    try {
        const deviceInfo = await getDeviceInfo();
        if (!deviceInfo.deviceId) {
            throw new Error('无法获取设备ID');
        }

        const token = await getToken();
        if (!token) {
            throw new Error('未找到访问令牌');
        }

        const url = signalRConfig.baseUrl;
        console.log('[SignalR] 连接URL:', url);

        const connection = new HubConnectionBuilder()
            .withUrl(url, {
                transport: signalRConfig.transport,
                withCredentials: true,
                skipNegotiation: signalRConfig.skipNegotiation,
                accessTokenFactory: () => token,
                headers: {
                    'X-Device-Id': deviceInfo.deviceId || '',
                    'X-Device-Name': deviceInfo.deviceName || '',
                    'X-Device-Type': (deviceInfo.deviceType || 0).toString(),
                    'X-Device-Model': deviceInfo.deviceModel || '',
                    'X-OS-Type': (deviceInfo.osType || 0).toString(),
                    'X-OS-Version': deviceInfo.osVersion || '',
                    'X-Browser-Type': (deviceInfo.browserType || 0).toString(),
                    'X-Browser-Version': deviceInfo.browserVersion || '',
                    'X-Resolution': deviceInfo.resolution || '',
                    'X-Location': deviceInfo.location || '',
                    'X-Language': navigator.language || '',
                    'X-Timezone': deviceInfo.environment?.timezone || Intl.DateTimeFormat().resolvedOptions().timeZone || '',
                    'X-User-Agent': navigator.userAgent,
                    'X-Device-Fingerprint': deviceInfo.deviceFingerprint || ''
                }
            })
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: (retryContext) => {
                    if (retryContext.previousRetryCount >= retryPolicy.maxRetries) {
                        return null;
                    }
                    const delay = Math.min(
                        retryPolicy.baseDelay * Math.pow(2, retryContext.previousRetryCount),
                        retryPolicy.maxDelay
                    );
                    console.log(`[SignalR] 第 ${retryContext.previousRetryCount + 1} 次重试,延迟 ${delay}ms`);
                    return delay;
                }
            })
            .configureLogging(customLogger)
            .withServerTimeout(signalRConfig.serverTimeoutInMilliseconds)
            .withKeepAliveInterval(signalRConfig.keepAliveIntervalInMilliseconds)
            .build();

        // 设置连接事件处理
        connection.onclose((error) => {
            console.error('[SignalR] 连接已关闭:', error);
            message.error('连接已断开，正在尝试重新连接...');
        });

        connection.onreconnecting((error) => {
            console.warn('[SignalR] 正在重新连接:', error);
            message.warning('正在重新连接...');
        });

        connection.onreconnected((connectionId) => {
            console.log('[SignalR] 重新连接成功:', connectionId);
            message.success('重新连接成功');
        });

        return connection;
    } catch (error) {
        console.error('[SignalR] 创建连接失败:', error);
        throw error;
    }
};

// 启动连接
export const startConnection = async (connection: HubConnection): Promise<void> => {
  try {
    await connection.start()
    console.log('SignalR连接已建立')
    message.success('连接成功')
  } catch (error) {
    console.error('启动SignalR连接失败:', error)
    throw error
  }
}

// 停止连接
export const stopConnection = async (connection: HubConnection): Promise<void> => {
  try {
    await connection.stop()
    console.log('SignalR连接已停止')
  } catch (error) {
    console.error('停止SignalR连接失败:', error)
    throw error
  }
}

// 心跳检测
export const startHeartbeat = (connection: HubConnection) => {
  let heartbeatInterval: NodeJS.Timeout | null = null

  const start = () => {
    if (heartbeatInterval) {
      clearInterval(heartbeatInterval)
    }

    heartbeatInterval = setInterval(async () => {
      try {
        if (connection.state === 'Connected') {
          await connection.invoke('SendHeartbeat')
          console.log('[SignalR] 心跳检测成功')
        } else {
          console.log('[SignalR] 跳过心跳检测，当前连接状态:', connection.state)
        }
      } catch (error) {
        console.error('[SignalR] 心跳检测失败:', error)
      }
    }, signalRConfig.heartbeatInterval)
  }

  const stop = () => {
    if (heartbeatInterval) {
      clearInterval(heartbeatInterval)
      heartbeatInterval = null
    }
  }

  return { start, stop }
}

// 错误处理
export const handleConnectionError = (error: Error) => {
  console.error('[SignalR] 连接错误:', error)
  if (error.message.includes('401')) {
    // 认证错误处理
    return 'AUTH_ERROR'
  }
  return 'CONNECTION_ERROR'
} 