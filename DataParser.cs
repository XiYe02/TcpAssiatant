using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TcpAssistant
{
    /// <summary>
    /// 传感器数据结构
    /// </summary>
    public class SensorData
    {
        public float Temperature { get; set; }  // 温度
        public float Pressure { get; set; }     // 压力
        public float Speed { get; set; }        // 速度
        public float Humidity { get; set; }     // 湿度
        public DateTime Timestamp { get; set; } // 时间戳

        public override string ToString()
        {
            return $"温度: {Temperature:F2}°C, 压力: {Pressure:F2}Pa, 速度: {Speed:F2}m/s, 湿度: {Humidity:F2}%";
        }
    }

    /// <summary>
    /// 数据解析器类，用于解析从SSCOM发送的传感器数据
    /// </summary>
    public class DataParser
    {
        // 数据缓冲区，用于存储接收到的数据
        private List<byte> buffer = new List<byte>();

        // 数据帧格式定义
        private const byte FRAME_HEADER = 0xAA;  // 帧头
        private const byte FRAME_TAIL = 0x55;    // 帧尾
        private const int MIN_FRAME_LENGTH = 18; // 最小帧长度：帧头(1) + 温度(4) + 压力(4) + 速度(4) + 湿度(4) + 帧尾(1)

        /// <summary>
        /// 获取当前缓冲区大小
        /// </summary>
        public int BufferSize => buffer.Count;

        /// <summary>
        /// 解析接收到的数据
        /// </summary>
        /// <param name="data">接收到的数据字节数组</param>
        /// <returns>解析出的传感器数据列表</returns>
        public List<SensorData> ParseData(byte[] data)
        {
            // 将接收到的数据添加到缓冲区
            if (data != null && data.Length > 0)
            {
                buffer.AddRange(data);
            }

            // 如果缓冲区数据不足以构成一个完整的数据帧，则返回空列表
            if (buffer.Count < MIN_FRAME_LENGTH)
            {
                return new List<SensorData>();
            }

            List<SensorData> result = new List<SensorData>();

            // 循环查找并解析数据帧
            int index = 0;
            while (index < buffer.Count - MIN_FRAME_LENGTH + 1)
            {
                // 查找帧头
                if (buffer[index] == FRAME_HEADER)
                {
                    // 查找帧尾
                    int tailIndex = -1;
                    for (int i = index + 1; i < buffer.Count; i++)
                    {
                        if (buffer[i] == FRAME_TAIL)
                        {
                            tailIndex = i;
                            break;
                        }
                    }

                    // 如果找到帧尾，并且帧长度符合要求
                    if (tailIndex != -1 && tailIndex - index >= MIN_FRAME_LENGTH - 1)
                    {
                        try
                        {
                            // 提取数据帧
                            byte[] frame = buffer.GetRange(index, tailIndex - index + 1).ToArray();
                            
                            // 解析数据帧
                            SensorData sensorData = ExtractSensorData(frame);
                            if (sensorData != null)
                            {
                                result.Add(sensorData);
                            }

                            // 移动索引到帧尾之后
                            index = tailIndex + 1;
                        }
                        catch (Exception ex)
                        {
                            // 解析异常，移动到下一个字节继续查找
                            System.Diagnostics.Debug.WriteLine($"解析数据帧异常: {ex.Message}");
                            index++;
                        }
                    }
                    else
                    {
                        // 没有找到帧尾或帧长度不符合要求，移动到下一个字节继续查找
                        index++;
                    }
                }
                else
                {
                    // 不是帧头，移动到下一个字节继续查找
                    index++;
                }
            }

            // 清理已处理的数据
            if (index > 0)
            {
                buffer.RemoveRange(0, index);
            }

            return result;
        }

        /// <summary>
        /// 从数据帧中提取传感器数据
        /// </summary>
        /// <param name="frame">完整的数据帧</param>
        /// <returns>传感器数据对象</returns>
        private SensorData ExtractSensorData(byte[] frame)
        {
            // 验证帧头和帧尾
            if (frame[0] != FRAME_HEADER || frame[frame.Length - 1] != FRAME_TAIL)
            {
                return null;
            }

            // 确保数据长度足够
            if (frame.Length < MIN_FRAME_LENGTH)
            {
                return null;
            }

            try
            {
                // 创建传感器数据对象
                SensorData data = new SensorData
                {
                    Timestamp = DateTime.Now
                };

                // 解析温度数据 (4字节浮点数)
                data.Temperature = BitConverter.ToSingle(frame, 1);

                // 解析压力数据 (4字节浮点数)
                data.Pressure = BitConverter.ToSingle(frame, 5);

                // 解析速度数据 (4字节浮点数)
                data.Speed = BitConverter.ToSingle(frame, 9);

                // 解析湿度数据 (4字节浮点数)
                data.Humidity = BitConverter.ToSingle(frame, 13);

                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"提取传感器数据异常: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            buffer.Clear();
        }
    }
}