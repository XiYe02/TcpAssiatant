using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private Queue<byte> bufferQueue = null;//解析数据队列
        private int frameLenth = 0; //数据帧长度


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
        }

        private void sendclear_btn_Click(object sender, EventArgs e)
        {
            send_rtb.Clear();
        }

       
    }
}
