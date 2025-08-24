namespace TcpAssistant
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.open_btn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.port_txb = new System.Windows.Forms.TextBox();
            this.IP_txb = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.reclear_btn = new System.Windows.Forms.Button();
            this.reHex_chb = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.sendclear_btn = new System.Windows.Forms.Button();
            this.sendHex_chb = new System.Windows.Forms.CheckBox();
            this.send_btn = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.receive_rtb = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.send_rtb = new System.Windows.Forms.RichTextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.Humidity_txt = new System.Windows.Forms.TextBox();
            this.Speed_txt = new System.Windows.Forms.TextBox();
            this.Pressure_txt = new System.Windows.Forms.TextBox();
            this.Temperature_txt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.AnalysisData_chb = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.open_btn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.port_txb);
            this.groupBox1.Controls.Add(this.IP_txb);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 267);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "TCP配置";
            // 
            // open_btn
            // 
            this.open_btn.Location = new System.Drawing.Point(136, 164);
            this.open_btn.Name = "open_btn";
            this.open_btn.Size = new System.Drawing.Size(122, 46);
            this.open_btn.TabIndex = 4;
            this.open_btn.Text = "启动服务器";
            this.open_btn.UseVisualStyleBackColor = true;
            this.open_btn.Click += new System.EventHandler(this.open_btn_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "端口号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP地址：";
            // 
            // port_txb
            // 
            this.port_txb.Location = new System.Drawing.Point(94, 109);
            this.port_txb.Name = "port_txb";
            this.port_txb.Size = new System.Drawing.Size(130, 25);
            this.port_txb.TabIndex = 1;
            // 
            // IP_txb
            // 
            this.IP_txb.Location = new System.Drawing.Point(94, 51);
            this.IP_txb.Name = "IP_txb";
            this.IP_txb.Size = new System.Drawing.Size(130, 25);
            this.IP_txb.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.reclear_btn);
            this.groupBox2.Controls.Add(this.reHex_chb);
            this.groupBox2.Location = new System.Drawing.Point(12, 273);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(282, 185);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "接收配置";
            // 
            // reclear_btn
            // 
            this.reclear_btn.Location = new System.Drawing.Point(136, 66);
            this.reclear_btn.Name = "reclear_btn";
            this.reclear_btn.Size = new System.Drawing.Size(75, 23);
            this.reclear_btn.TabIndex = 1;
            this.reclear_btn.Text = "清空";
            this.reclear_btn.UseVisualStyleBackColor = true;
            this.reclear_btn.Click += new System.EventHandler(this.reclear_btn_Click);
            // 
            // reHex_chb
            // 
            this.reHex_chb.AutoSize = true;
            this.reHex_chb.Location = new System.Drawing.Point(28, 66);
            this.reHex_chb.Name = "reHex_chb";
            this.reHex_chb.Size = new System.Drawing.Size(89, 19);
            this.reHex_chb.TabIndex = 0;
            this.reHex_chb.Text = "十六进制";
            this.reHex_chb.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.sendclear_btn);
            this.groupBox3.Controls.Add(this.sendHex_chb);
            this.groupBox3.Controls.Add(this.send_btn);
            this.groupBox3.Location = new System.Drawing.Point(12, 469);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(282, 208);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "发送配置";
            // 
            // sendclear_btn
            // 
            this.sendclear_btn.Location = new System.Drawing.Point(137, 134);
            this.sendclear_btn.Name = "sendclear_btn";
            this.sendclear_btn.Size = new System.Drawing.Size(75, 23);
            this.sendclear_btn.TabIndex = 7;
            this.sendclear_btn.Text = "清空";
            this.sendclear_btn.UseVisualStyleBackColor = true;
            this.sendclear_btn.Click += new System.EventHandler(this.sendclear_btn_Click);
            // 
            // sendHex_chb
            // 
            this.sendHex_chb.AutoSize = true;
            this.sendHex_chb.Location = new System.Drawing.Point(28, 37);
            this.sendHex_chb.Name = "sendHex_chb";
            this.sendHex_chb.Size = new System.Drawing.Size(89, 19);
            this.sendHex_chb.TabIndex = 6;
            this.sendHex_chb.Text = "十六进制";
            this.sendHex_chb.UseVisualStyleBackColor = true;
            // 
            // send_btn
            // 
            this.send_btn.Location = new System.Drawing.Point(137, 37);
            this.send_btn.Name = "send_btn";
            this.send_btn.Size = new System.Drawing.Size(90, 43);
            this.send_btn.TabIndex = 5;
            this.send_btn.Text = "发送";
            this.send_btn.UseVisualStyleBackColor = true;
            this.send_btn.Click += new System.EventHandler(this.send_btn_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.receive_rtb);
            this.groupBox4.Location = new System.Drawing.Point(328, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(563, 436);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "接收区";
            // 
            // receive_rtb
            // 
            this.receive_rtb.BackColor = System.Drawing.SystemColors.Info;
            this.receive_rtb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.receive_rtb.Location = new System.Drawing.Point(3, 21);
            this.receive_rtb.Name = "receive_rtb";
            this.receive_rtb.Size = new System.Drawing.Size(557, 412);
            this.receive_rtb.TabIndex = 0;
            this.receive_rtb.Text = "";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.send_rtb);
            this.groupBox5.Location = new System.Drawing.Point(328, 469);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(563, 208);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "发送区";
            // 
            // send_rtb
            // 
            this.send_rtb.BackColor = System.Drawing.SystemColors.Info;
            this.send_rtb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.send_rtb.Location = new System.Drawing.Point(3, 21);
            this.send_rtb.Name = "send_rtb";
            this.send_rtb.Size = new System.Drawing.Size(557, 184);
            this.send_rtb.TabIndex = 0;
            this.send_rtb.Text = "";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.AnalysisData_chb);
            this.groupBox6.Controls.Add(this.Humidity_txt);
            this.groupBox6.Controls.Add(this.Speed_txt);
            this.groupBox6.Controls.Add(this.Pressure_txt);
            this.groupBox6.Controls.Add(this.Temperature_txt);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Location = new System.Drawing.Point(908, 27);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(258, 660);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "报文解析";
            // 
            // Humidity_txt
            // 
            this.Humidity_txt.Location = new System.Drawing.Point(100, 215);
            this.Humidity_txt.Name = "Humidity_txt";
            this.Humidity_txt.Size = new System.Drawing.Size(133, 25);
            this.Humidity_txt.TabIndex = 7;
            // 
            // Speed_txt
            // 
            this.Speed_txt.Location = new System.Drawing.Point(100, 158);
            this.Speed_txt.Name = "Speed_txt";
            this.Speed_txt.Size = new System.Drawing.Size(133, 25);
            this.Speed_txt.TabIndex = 6;
            // 
            // Pressure_txt
            // 
            this.Pressure_txt.Location = new System.Drawing.Point(100, 89);
            this.Pressure_txt.Name = "Pressure_txt";
            this.Pressure_txt.Size = new System.Drawing.Size(133, 25);
            this.Pressure_txt.TabIndex = 5;
            // 
            // Temperature_txt
            // 
            this.Temperature_txt.Location = new System.Drawing.Point(100, 33);
            this.Temperature_txt.Name = "Temperature_txt";
            this.Temperature_txt.Size = new System.Drawing.Size(133, 25);
            this.Temperature_txt.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 225);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 3;
            this.label6.Text = "湿度：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "速度：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "压力：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "温度：";
            // 
            // AnalysisData_chb
            // 
            this.AnalysisData_chb.AutoSize = true;
            this.AnalysisData_chb.Location = new System.Drawing.Point(28, 300);
            this.AnalysisData_chb.Name = "AnalysisData_chb";
            this.AnalysisData_chb.Size = new System.Drawing.Size(119, 19);
            this.AnalysisData_chb.TabIndex = 8;
            this.AnalysisData_chb.Text = "启动数据解析";
            this.AnalysisData_chb.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1190, 706);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "TCP助手";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RichTextBox receive_rtb;
        private System.Windows.Forms.RichTextBox send_rtb;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button open_btn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox port_txb;
        private System.Windows.Forms.TextBox IP_txb;
        private System.Windows.Forms.Button send_btn;
        private System.Windows.Forms.CheckBox reHex_chb;
        private System.Windows.Forms.CheckBox sendHex_chb;
        private System.Windows.Forms.Button reclear_btn;
        private System.Windows.Forms.Button sendclear_btn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Humidity_txt;
        private System.Windows.Forms.TextBox Speed_txt;
        private System.Windows.Forms.TextBox Pressure_txt;
        private System.Windows.Forms.TextBox Temperature_txt;
        private System.Windows.Forms.CheckBox AnalysisData_chb;
    }
}

