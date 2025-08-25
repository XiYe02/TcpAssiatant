using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace TcpAssistant
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private List<TcpClient> connectedClients = new List<TcpClient>();// 已连接客户端列表
        private bool isServerRunning = false;//服务器运行状态
        private System.Threading.Thread listenThread;//监听线程

        private List<byte> receiveBuffer = new List<byte>();// 接收缓冲区
        private byte[] sendBuffer = new byte[1024];// 发送缓冲区

    
        // 数据解析器
        private DataParser dataParser = new DataParser();
        private int totalPacketsReceived = 0; // 接收到的数据包总数
        private int validPacketsReceived = 0; // 有效数据包总数
        private int invalidPacketsReceived = 0; // 无效数据包总数



        public static string folder = ConfigurationManager.AppSettings["folder"].ToString();//存储文件夹
        public static List<SensorData> SaveData = new List<SensorData>();//保存数据
        public Form1()
        {
            InitializeComponent();
            InitializeUI();
        }

        #region 初始化界面
        private void InitializeUI()
        {
           


            // 设置默认值 - 服务器监听地址和端口
            IP_txb.Text = "192.168.31.1";  // 监听所有网络接口
            port_txb.Text = "777";    // 默认端口777，与SSCOM客户端匹配

            // 初始状态
            UpdateServerStatus(false);
        }

        private void UpdateServerStatus(bool running)
        {
            isServerRunning = running;
            open_btn.Text = running ? "停止服务器" : "启动服务器";
            open_btn.BackColor = running ? Color.LightCoral : Color.LightGreen;

            IP_txb.Enabled = !running;
            port_txb.Enabled = !running;
        }
        #endregion

        #region 启动和停止服务器
        private void StartServer()
        {
            try
            {
                // 验证输入
                if (string.IsNullOrEmpty(IP_txb.Text) || string.IsNullOrEmpty(port_txb.Text))
                {
                    MessageBox.Show("请输入IP地址和端口号！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 解析IP地址和端口
                IPAddress serverIP;
                if (!IPAddress.TryParse(IP_txb.Text, out serverIP))
                {
                    MessageBox.Show("IP地址格式不正确！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int port;
                if (!int.TryParse(port_txb.Text, out port) || port < 1 || port > 65535)
                {
                    MessageBox.Show("端口号必须在1-65535之间！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 创建TCP监听器并启动
                tcpListener = new TcpListener(serverIP, port);
                tcpListener.Start();

                // 更新UI状态
                UpdateServerStatus(true);

                // 显示服务器启动消息
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 服务器启动成功，监听地址: {IP_txb.Text}:{port}\r\n");

                // 启动监听线程，并且接收客户端数据
                StartListening();

                MessageBox.Show($"服务器启动成功，监听地址: {IP_txb.Text}:{port}", "服务器启动", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"服务器启动失败: {ex.Message}", "启动错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 服务器启动失败: {ex.Message}\r\n");
            }
        }

        private void StopServer()
        {
            try
            {
                // 首先设置停止标志
                isServerRunning = false;

                // 停止TCP监听器（这会中断AcceptTcpClient的阻塞）
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                    tcpListener = null;
                }

                // 断开所有客户端连接
                foreach (var client in connectedClients.ToList())
                {
                    try
                    {
                        if (client.Connected)
                        {
                            client.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 关闭客户端连接时出错: {ex.Message}\r\n");
                    }
                }
                connectedClients.Clear();

                // 等待监听线程结束，但设置超时时间避免无限等待
                if (listenThread != null && listenThread.IsAlive)
                {
                    if (!listenThread.Join(2000)) // 等待最多2秒
                    {
                        receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 监听线程未能在2秒内正常结束\r\n");
                    }
                }

                // 更新UI状态
                UpdateServerStatus(false);

                // 显示服务器停止消息
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 服务器已停止\r\n");

                MessageBox.Show("服务器已停止", "服务器停止", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"停止服务器时发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 停止服务器时发生错误: {ex.Message}\r\n");
            }
        }
        #endregion

        #region 监听线程和接收线程
        private void StartListening()
        {
            listenThread = new System.Threading.Thread(() =>
            {
                while (isServerRunning && tcpListener != null)
                {
                    try
                    {
                        // 等待客户端连接
                        TcpClient client = tcpListener.AcceptTcpClient();

                        // 服务器停止时关闭客户端连接
                        if (!isServerRunning)
                        {
                            client.Close();
                            break;
                        }

                        // 修改UI线程的数据
                        this.Invoke(new Action(() =>
                        {
                            string clientEndPoint = client.Client.RemoteEndPoint.ToString();// 获取客户端IP地址和端口
                            receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 新客户端连接: {clientEndPoint}\r\n");
                            connectedClients.Add(client);
                        }));

                        // 为每个客户端启动接收线程
                        System.Threading.Thread clientThread = new System.Threading.Thread(() => HandleClient(client));
                        clientThread.Start();
                    }
                    catch (SocketException ex)
                    {
                        // SocketException通常发生在服务器停止时，这是正常的
                        if (isServerRunning)
                        {
                            this.Invoke(new Action(() =>
                            {
                                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 接受连接错误: {ex.Message}\r\n");
                            }));
                        }
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        // TcpListener被释放时抛出此异常，这是正常的停止过程
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (isServerRunning)
                        {
                            this.Invoke(new Action(() =>
                            {
                                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 接受连接时发生未知错误: {ex.Message}\r\n");
                            }));
                        }
                        break;
                    }
                }
            });
            listenThread.Start();
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            string clientEndPoint = client.Client.RemoteEndPoint.ToString();

            while (client.Connected && isServerRunning)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        // 只添加实际读取的数据到接收缓冲区
                        byte[] actualData = new byte[bytesRead];
                        Array.Copy(buffer, 0, actualData, 0, bytesRead);
                        receiveBuffer.AddRange(actualData);
                       
                        // 在UI线程中更新接收区
                        this.Invoke(new Action(() =>
                        {
                            if (AnalysisData_chb.Checked == false)
                            {
                                if (!reHex_chb.Checked)
                                {
                                    string receivedData = Encoding.UTF8.GetString(actualData, 0, actualData.Length);
                                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 来自 {clientEndPoint}: {receivedData}\r\n");
                                    receive_rtb.ScrollToCaret();
                                }
                                else
                                {
                                    string hexData = Transform.ToHexString(actualData, " ");
                                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 来自 {clientEndPoint}: {hexData}\r\n");
                                    receive_rtb.ScrollToCaret();
                                }
                            }
                            else
                            {
                                // 解析数据
                                ProcessSensorData(actualData, clientEndPoint);
                            }

                        }));
                    }
                    else
                    {
                        // 客户端主动断开连接
                        break;
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 流被释放，客户端已断开
                    break;
                }
                catch (IOException)
                {
                    // 网络IO异常，客户端可能断开连接
                    break;
                }
                catch (Exception ex)
                {
                    if (client.Connected && isServerRunning)
                    {
                        this.Invoke(new Action(() =>
                        {
                            receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 客户端 {clientEndPoint} 处理数据时出错: {ex.Message}\r\n");
                        }));
                    }
                    break;
                }
            }

            // 客户端断开连接，从列表中移除
            this.Invoke(new Action(() =>
            {
                if (connectedClients.Contains(client))
                {
                    connectedClients.Remove(client);
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 客户端 {clientEndPoint} 已从连接列表中移除\r\n");
                }
            }));
        }
        #endregion

        #region 发送数据
        private void SendDataToAllClients(string data)
        {
            if (!isServerRunning || connectedClients.Count == 0)
            {
                MessageBox.Show("没有客户端连接！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<TcpClient> disconnectedClients = new List<TcpClient>();

            foreach (var client in connectedClients)
            {
                try
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = Encoding.UTF8.GetBytes(data);
                        stream.Write(buffer, 0, buffer.Length);
                        stream.Flush();
                    }
                    else
                    {
                        disconnectedClients.Add(client);
                    }
                }
                catch (Exception ex)
                {
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 发送数据到客户端失败: {ex.Message}\r\n");
                    disconnectedClients.Add(client);
                }
            }

            // 移除断开的客户端
            foreach (var client in disconnectedClients)
            {
                connectedClients.Remove(client);
            }

            // 显示发送的消息
            receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 发送到所有客户端: {data}\r\n");
            receive_rtb.ScrollToCaret();
        } 
        #endregion

        // 发送按钮事件处理
        private void send_btn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(send_rtb.Text))
            {
                SendDataToAllClients(send_rtb.Text);
                send_rtb.Clear();
            }
        }

        // 窗体关闭时清理资源
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isServerRunning)
            {
                StopServer();
            }
            base.OnFormClosing(e);
        }

        private void open_btn_Click_1(object sender, EventArgs e)
        {
            if (!isServerRunning)
            {
                StartServer();
            }
            else
            {
                StopServer();
            }
        }

        private void reclear_btn_Click(object sender, EventArgs e)
        {
            receive_rtb.Clear();
            // 如果启用了数据解析，也清空解析器缓冲区
            if (AnalysisData_chb.Checked)
            {
                ClearDataParserBuffer();
            }
        }

        private void sendclear_btn_Click(object sender, EventArgs e)
        {
            send_rtb.Clear();
        }

        #region 数据解析功能
        /// <summary>
        /// 处理传感器数据
        /// </summary>
        /// <param name="data">接收到的数据</param>
        /// <param name="clientEndPoint">客户端地址</param>
        private void ProcessSensorData(byte[] data, string clientEndPoint)
        {
            try
            {
                // 显示原始十六进制数据
                string hexData = Transform.ToHexString(data, " ");
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 来自 {clientEndPoint}: {hexData}\r\n");
                
                // 解析数据
                List<SensorData> sensorDataList = dataParser.ParseData(data);
                SaveData.AddRange(sensorDataList);
                
                if (sensorDataList != null && sensorDataList.Count > 0)
                {
                    // 有效数据包计数
                    validPacketsReceived += sensorDataList.Count;
                    totalPacketsReceived += sensorDataList.Count;
                    
                    // 更新最新的传感器数据到UI
                    SensorData latestData = sensorDataList[sensorDataList.Count - 1];
                    UpdateSensorDisplay(latestData);
                    
                    // 显示解析结果
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 解析成功 - 有效数据包: {sensorDataList.Count}\r\n");
                    foreach (var sensorData in sensorDataList)
                    {
                        receive_rtb.AppendText($"  {sensorData}\r\n");
                    }
                }
                else
                {
                    // 无效数据包计数
                    invalidPacketsReceived++;
                    totalPacketsReceived++;
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 数据包解析失败或数据不完整\r\n");
                    
                    // 输出调试信息
                    System.Diagnostics.Debug.WriteLine($"数据包解析失败: {hexData}");
                }
                
                receive_rtb.ScrollToCaret();
                
                // 显示统计信息
                ShowStatistics();
            }
            catch (Exception ex)
            {
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 数据处理错误: {ex.Message}\r\n");
                System.Diagnostics.Debug.WriteLine($"数据处理异常: {ex.Message}\n{ex.StackTrace}");
                invalidPacketsReceived++;
                totalPacketsReceived++;
                ShowStatistics();
            }
        }

        /// <summary>
        /// 更新传感器数据显示
        /// </summary>
        /// <param name="data">传感器数据</param>
        private void UpdateSensorDisplay(SensorData data)
        {
            // 在UI线程中更新文本框
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<SensorData>(UpdateSensorDisplay), data);
                return;
            }

            Temperature_txt.Text = data.Temperature.ToString("F2");
            Pressure_txt.Text = data.Pressure.ToString("F2");
            Speed_txt.Text = data.Speed.ToString("F2");
            Humidity_txt.Text = data.Humidity.ToString("F2");
        }

        /// <summary>
        /// 显示统计信息
        /// </summary>
        private void ShowStatistics()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ShowStatistics));
                return;
            }

            // 在接收区显示统计信息
            receive_rtb.AppendText($"[统计] 总包数: {totalPacketsReceived}, 有效: {validPacketsReceived}, 无效: {invalidPacketsReceived}, 缓冲区: {dataParser.BufferSize}字节\r\n");
        }

        /// <summary>
        /// 清空数据解析器缓冲区
        /// </summary>
        private void ClearDataParserBuffer()
        {
            dataParser.ClearBuffer();
            totalPacketsReceived = 0;
            validPacketsReceived = 0;
            invalidPacketsReceived = 0;
            receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 数据解析缓冲区已清空\r\n");
        }




        #endregion

       


        private void SaveToFile()
        {
            try
            {
                // 确保文件夹存在
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                
                // 创建文件名（Data+日期时间）
                string fileName = $"Data_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(folder, fileName);
                
                // 创建要保存的数据列表，包含一分钟内接收的所有数据
                var dataList = new List<Dictionary<string, float>>();
                
                // 将每条传感器数据添加到列表中
                foreach (var sensorData in SaveData)
                {
                    var dataItem = new Dictionary<string, float>
                    {
                        { "Temperature", sensorData.Temperature },
                        { "Pressure", sensorData.Pressure },
                        { "Speed", sensorData.Speed },
                        { "Humidity", sensorData.Humidity }
                    };
                    dataList.Add(dataItem);
                }
                
                // 序列化并保存到文件
                string json = JsonConvert.SerializeObject(dataList, Formatting.Indented);
                File.WriteAllText(filePath, json);
                
                // 显示保存成功消息
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 已保存{SaveData.Count}条数据到文件: {fileName}\r\n");
                receive_rtb.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存数据时出错: {ex.Message}", "保存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 保存数据时出错: {ex.Message}\r\n");
            }
        }

        // 保存数据计数器和定时器
        private int saveCount = 0;
        private System.Windows.Forms.Timer saveTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer countdownTimer = new System.Windows.Forms.Timer();
        private int remainingSeconds = 60; // 剩余秒数
        
        private void SaveData_btn_Click(object sender, EventArgs e)
        {
            // 初始化计数器和定时器
            saveCount = 0;
            saveTimer.Stop();
            saveTimer.Interval = 20000; // 1分钟 = 60000毫秒
            saveTimer.Tick += SaveTimer_Tick;
            
            // 初始化倒计时定时器
            countdownTimer.Stop();
            countdownTimer.Interval = 1000; // 1秒 = 1000毫秒
            countdownTimer.Tick += CountdownTimer_Tick;
            
            // 重置剩余时间
            remainingSeconds = 20;
            UpdateTimesLabel();
            
            // 清空之前的数据，准备接收新数据
            SaveData.Clear();
            
            // 显示开始收集数据的消息
            receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 开始收集数据，将在1分钟后进行第1次保存\r\n");
            receive_rtb.ScrollToCaret();
            
            // 启动定时器，1分钟后进行第一次保存
            saveTimer.Start();
            countdownTimer.Start();
        }
        
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            // 每秒减少剩余时间
            remainingSeconds--;
            
            // 更新界面显示
            UpdateTimesLabel();
            
            // 如果倒计时结束，停止倒计时定时器
            if (remainingSeconds <= 0)
            {
                countdownTimer.Stop();
            }
        }
        
        private void UpdateTimesLabel()
        {
            // 更新剩余时间显示
            times_lab.Text = $"{remainingSeconds}秒";
        }
        
        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            // 定时器触发时，暂停定时器并询问用户是否继续
            saveTimer.Stop();
            countdownTimer.Stop();
            SaveAndContinue();
        }
        
        private void SaveAndContinue()
        {
            // 保存当前数据
            if (SaveData.Count > 0)
            {
                SaveToFile();//将数据写进文件
                SaveData.Clear(); // 保存后清空数据
                saveCount++;
                
                receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 已完成第 {saveCount} 次数据保存\r\n");
                
                // 检查是否已完成三次保存
                if (saveCount >= 3)
                {
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 已完成全部3次数据保存任务\r\n");
                    MessageBox.Show("数据保存完成！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    times_lab.Text = $"0秒";
                    return; // 结束保存过程
                }
                
                // 询问用户是否继续
                DialogResult result = MessageBox.Show(
                    $"已完成第 {saveCount} 次数据保存，是否继续下一次保存？", 
                    "继续保存", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // 重置倒计时
                    remainingSeconds = 20;
                    UpdateTimesLabel();
                    
                    // 继续下一次保存，启动定时器
                    saveTimer.Start();
                    countdownTimer.Start();
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 继续收集数据，将在1分钟后进行第 {saveCount + 1} 次保存\r\n");
                }
                else
                {
                    receive_rtb.AppendText($"[{DateTime.Now:HH:mm:ss}] 用户取消了后续保存操作\r\n");
                    // 重置倒计时显示
                    times_lab.Text = "0秒";
                }
            }
            else
            {
                MessageBox.Show("没有可保存的数据！请确保已接收并解析了传感器数据。", "无数据", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
