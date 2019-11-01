namespace crawler
{
    partial class crawler
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblip = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblstarttime = new System.Windows.Forms.Label();
            this.lblend = new System.Windows.Forms.Label();
            this.lblendtime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblelapsedtime = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.lbltotalpages = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbltotalkey = new System.Windows.Forms.Label();
            this.lbltotalkeyc = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblthreadfailed = new System.Windows.Forms.Label();
            this.lblcrawled = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.lbltotalthread = new System.Windows.Forms.Label();
            this.lblrunnig = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblgooglecrawled = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(37, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(197, 54);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start Crawling";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(275, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(206, 53);
            this.button2.TabIndex = 1;
            this.button2.Text = "End Crawling";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Public IP Address :";
            // 
            // lblip
            // 
            this.lblip.AutoSize = true;
            this.lblip.Location = new System.Drawing.Point(150, 82);
            this.lblip.Name = "lblip";
            this.lblip.Size = new System.Drawing.Size(0, 13);
            this.lblip.TabIndex = 3;
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Location = new System.Drawing.Point(37, 104);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(58, 13);
            this.lblStart.TabIndex = 4;
            this.lblStart.Text = "Start Time:";
            // 
            // lblstarttime
            // 
            this.lblstarttime.AutoSize = true;
            this.lblstarttime.Location = new System.Drawing.Point(139, 104);
            this.lblstarttime.Name = "lblstarttime";
            this.lblstarttime.Size = new System.Drawing.Size(0, 13);
            this.lblstarttime.TabIndex = 5;
            // 
            // lblend
            // 
            this.lblend.AutoSize = true;
            this.lblend.Location = new System.Drawing.Point(272, 104);
            this.lblend.Name = "lblend";
            this.lblend.Size = new System.Drawing.Size(58, 13);
            this.lblend.TabIndex = 6;
            this.lblend.Text = "End Time :";
            // 
            // lblendtime
            // 
            this.lblendtime.AutoSize = true;
            this.lblendtime.Location = new System.Drawing.Point(352, 104);
            this.lblendtime.Name = "lblendtime";
            this.lblendtime.Size = new System.Drawing.Size(0, 13);
            this.lblendtime.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Elapsed :";
            // 
            // lblelapsedtime
            // 
            this.lblelapsedtime.AutoSize = true;
            this.lblelapsedtime.Location = new System.Drawing.Point(140, 127);
            this.lblelapsedtime.Name = "lblelapsedtime";
            this.lblelapsedtime.Size = new System.Drawing.Size(0, 13);
            this.lblelapsedtime.TabIndex = 9;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(508, 12);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(224, 209);
            this.dataGridView2.TabIndex = 11;
            // 
            // timer1
            // 
            this.timer1.Interval = 20000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(275, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Total Pages Crawled :";
            // 
            // lbltotalpages
            // 
            this.lbltotalpages.AutoSize = true;
            this.lbltotalpages.Location = new System.Drawing.Point(425, 195);
            this.lbltotalpages.Name = "lbltotalpages";
            this.lbltotalpages.Size = new System.Drawing.Size(0, 13);
            this.lbltotalpages.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(275, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Total Keywords :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(275, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Total Keywords Crawled :";
            // 
            // lbltotalkey
            // 
            this.lbltotalkey.AutoSize = true;
            this.lbltotalkey.Location = new System.Drawing.Point(425, 150);
            this.lbltotalkey.Name = "lbltotalkey";
            this.lbltotalkey.Size = new System.Drawing.Size(0, 13);
            this.lbltotalkey.TabIndex = 19;
            // 
            // lbltotalkeyc
            // 
            this.lbltotalkeyc.AutoSize = true;
            this.lbltotalkeyc.Location = new System.Drawing.Point(425, 172);
            this.lbltotalkeyc.Name = "lbltotalkeyc";
            this.lbltotalkeyc.Size = new System.Drawing.Size(0, 13);
            this.lbltotalkeyc.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 219);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Threads Failed :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(38, 195);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Threads Ended :";
            // 
            // lblthreadfailed
            // 
            this.lblthreadfailed.AutoSize = true;
            this.lblthreadfailed.BackColor = System.Drawing.Color.Red;
            this.lblthreadfailed.Location = new System.Drawing.Point(175, 219);
            this.lblthreadfailed.Name = "lblthreadfailed";
            this.lblthreadfailed.Size = new System.Drawing.Size(0, 13);
            this.lblthreadfailed.TabIndex = 23;
            // 
            // lblcrawled
            // 
            this.lblcrawled.AutoSize = true;
            this.lblcrawled.BackColor = System.Drawing.Color.Yellow;
            this.lblcrawled.Location = new System.Drawing.Point(175, 196);
            this.lblcrawled.Name = "lblcrawled";
            this.lblcrawled.Size = new System.Drawing.Size(0, 13);
            this.lblcrawled.TabIndex = 24;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 247);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(743, 209);
            this.dataGridView1.TabIndex = 25;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Total Threads :";
            // 
            // lbltotalthread
            // 
            this.lbltotalthread.AutoSize = true;
            this.lbltotalthread.Location = new System.Drawing.Point(175, 150);
            this.lbltotalthread.Name = "lbltotalthread";
            this.lbltotalthread.Size = new System.Drawing.Size(0, 13);
            this.lbltotalthread.TabIndex = 14;
            // 
            // lblrunnig
            // 
            this.lblrunnig.AutoSize = true;
            this.lblrunnig.BackColor = System.Drawing.Color.Green;
            this.lblrunnig.Location = new System.Drawing.Point(175, 172);
            this.lblrunnig.Name = "lblrunnig";
            this.lblrunnig.Size = new System.Drawing.Size(0, 13);
            this.lblrunnig.TabIndex = 27;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(37, 172);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(112, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Threads On Running :";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(275, 219);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Total Google Crawled :";
            // 
            // lblgooglecrawled
            // 
            this.lblgooglecrawled.AutoSize = true;
            this.lblgooglecrawled.Location = new System.Drawing.Point(425, 219);
            this.lblgooglecrawled.Name = "lblgooglecrawled";
            this.lblgooglecrawled.Size = new System.Drawing.Size(0, 13);
            this.lblgooglecrawled.TabIndex = 29;
            // 
            // crawler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(744, 457);
            this.Controls.Add(this.lblgooglecrawled);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblrunnig);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lblcrawled);
            this.Controls.Add(this.lblthreadfailed);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbltotalkeyc);
            this.Controls.Add(this.lbltotalkey);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbltotalpages);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbltotalthread);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.lblelapsedtime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblendtime);
            this.Controls.Add(this.lblend);
            this.Controls.Add(this.lblstarttime);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.lblip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.Name = "crawler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "crawler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.crawler_FormClosing);
            this.Load += new System.EventHandler(this.crawler_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblip;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblstarttime;
        private System.Windows.Forms.Label lblend;
        private System.Windows.Forms.Label lblendtime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblelapsedtime;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbltotalpages;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbltotalkey;
        private System.Windows.Forms.Label lbltotalkeyc;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblthreadfailed;
        private System.Windows.Forms.Label lblcrawled;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbltotalthread;
        private System.Windows.Forms.Label lblrunnig;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblgooglecrawled;
    }
}