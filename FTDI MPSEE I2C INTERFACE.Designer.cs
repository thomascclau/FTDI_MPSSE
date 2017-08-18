namespace KeypadLED
{
    partial class FormMain
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
            this.buttonInit = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.Label_Status = new System.Windows.Forms.Label();
            this.labelDUTStatus = new System.Windows.Forms.Label();
            this.buttonRed = new System.Windows.Forms.Button();
            this.buttonGreen = new System.Windows.Forms.Button();
            this.buttonBlue = new System.Windows.Forms.Button();
            this.labelFTDIStatus = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelFTDIType = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonKey1 = new System.Windows.Forms.Button();
            this.buttonKey2 = new System.Windows.Forms.Button();
            this.buttonKey3 = new System.Windows.Forms.Button();
            this.buttonKey4 = new System.Windows.Forms.Button();
            this.buttonKey5 = new System.Windows.Forms.Button();
            this.buttonKey6 = new System.Windows.Forms.Button();
            this.buttonKey7 = new System.Windows.Forms.Button();
            this.buttonKey8 = new System.Windows.Forms.Button();
            this.buttonKey9 = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonInit
            // 
            this.buttonInit.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold);
            this.buttonInit.Location = new System.Drawing.Point(39, 64);
            this.buttonInit.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonInit.Name = "buttonInit";
            this.buttonInit.Size = new System.Drawing.Size(105, 83);
            this.buttonInit.TabIndex = 0;
            this.buttonInit.Text = "Initialise";
            this.buttonInit.UseVisualStyleBackColor = true;
            this.buttonInit.Click += new System.EventHandler(this.buttonInit_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(398, 172);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(105, 50);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // Label_Status
            // 
            this.Label_Status.Location = new System.Drawing.Point(0, 0);
            this.Label_Status.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label_Status.Name = "Label_Status";
            this.Label_Status.Size = new System.Drawing.Size(167, 35);
            this.Label_Status.TabIndex = 14;
            // 
            // labelDUTStatus
            // 
            this.labelDUTStatus.AutoSize = true;
            this.labelDUTStatus.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelDUTStatus.Location = new System.Drawing.Point(30, 35);
            this.labelDUTStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelDUTStatus.Name = "labelDUTStatus";
            this.labelDUTStatus.Size = new System.Drawing.Size(15, 19);
            this.labelDUTStatus.TabIndex = 19;
            this.labelDUTStatus.Text = "-";
            // 
            // buttonRed
            // 
            this.buttonRed.BackColor = System.Drawing.Color.Red;
            this.buttonRed.Location = new System.Drawing.Point(168, 65);
            this.buttonRed.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonRed.Name = "buttonRed";
            this.buttonRed.Size = new System.Drawing.Size(105, 83);
            this.buttonRed.TabIndex = 31;
            this.buttonRed.Text = "RED";
            this.buttonRed.UseVisualStyleBackColor = false;
            this.buttonRed.Click += new System.EventHandler(this.buttonRed_Click);
            // 
            // buttonGreen
            // 
            this.buttonGreen.BackColor = System.Drawing.Color.Lime;
            this.buttonGreen.Location = new System.Drawing.Point(283, 66);
            this.buttonGreen.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonGreen.Name = "buttonGreen";
            this.buttonGreen.Size = new System.Drawing.Size(105, 83);
            this.buttonGreen.TabIndex = 32;
            this.buttonGreen.Text = "GREEN";
            this.buttonGreen.UseVisualStyleBackColor = false;
            this.buttonGreen.Click += new System.EventHandler(this.buttonGreen_Click);
            // 
            // buttonBlue
            // 
            this.buttonBlue.BackColor = System.Drawing.Color.Blue;
            this.buttonBlue.Location = new System.Drawing.Point(398, 66);
            this.buttonBlue.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonBlue.Name = "buttonBlue";
            this.buttonBlue.Size = new System.Drawing.Size(105, 83);
            this.buttonBlue.TabIndex = 33;
            this.buttonBlue.Text = "BLUE";
            this.buttonBlue.UseVisualStyleBackColor = false;
            this.buttonBlue.Click += new System.EventHandler(this.buttonBlue_Click);
            // 
            // labelFTDIStatus
            // 
            this.labelFTDIStatus.AutoSize = true;
            this.labelFTDIStatus.Location = new System.Drawing.Point(164, 35);
            this.labelFTDIStatus.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelFTDIStatus.Name = "labelFTDIStatus";
            this.labelFTDIStatus.Size = new System.Drawing.Size(18, 19);
            this.labelFTDIStatus.TabIndex = 15;
            this.labelFTDIStatus.Text = "0";
            this.labelFTDIStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelFTDIType);
            this.groupBox1.Controls.Add(this.labelFTDIStatus);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox1.Size = new System.Drawing.Size(355, 73);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FTDI Cable Status";
            // 
            // labelFTDIType
            // 
            this.labelFTDIType.AutoSize = true;
            this.labelFTDIType.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelFTDIType.Location = new System.Drawing.Point(35, 35);
            this.labelFTDIType.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelFTDIType.Name = "labelFTDIType";
            this.labelFTDIType.Size = new System.Drawing.Size(15, 19);
            this.labelFTDIType.TabIndex = 20;
            this.labelFTDIType.Text = "-";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelDUTStatus);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(455, 14);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox2.Size = new System.Drawing.Size(290, 73);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DUT Status";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonInit);
            this.groupBox3.Controls.Add(this.buttonRed);
            this.groupBox3.Controls.Add(this.buttonGreen);
            this.groupBox3.Controls.Add(this.buttonBlue);
            this.groupBox3.Controls.Add(this.buttonClose);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(14, 97);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox3.Size = new System.Drawing.Size(551, 259);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "G7 Keypad LED Control";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(892, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 36;
            this.label2.Text = "SCL - ORG";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(892, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 20);
            this.label3.TabIndex = 37;
            this.label3.Text = "SDA - YEL,  GRN";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(892, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 20);
            this.label4.TabIndex = 38;
            this.label4.Text = "VCC - RED";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(892, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 20);
            this.label5.TabIndex = 39;
            this.label5.Text = "GND - BLK";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(797, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(277, 24);
            this.label6.TabIndex = 40;
            this.label6.Text = "MPSSE CABLE I2C SIGNALs";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(869, 198);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(205, 20);
            this.label7.TabIndex = 41;
            this.label7.Text = "GPIOL0 - GRY (ADBUS4)";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(48, 64);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 83);
            this.button1.TabIndex = 42;
            this.button1.Text = "Scan";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonKey1
            // 
            this.buttonKey1.BackColor = System.Drawing.Color.White;
            this.buttonKey1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey1.Location = new System.Drawing.Point(541, 237);
            this.buttonKey1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey1.Name = "buttonKey1";
            this.buttonKey1.Size = new System.Drawing.Size(52, 48);
            this.buttonKey1.TabIndex = 44;
            this.buttonKey1.Text = "<";
            this.buttonKey1.UseVisualStyleBackColor = false;
            // 
            // buttonKey2
            // 
            this.buttonKey2.BackColor = System.Drawing.Color.White;
            this.buttonKey2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey2.Location = new System.Drawing.Point(541, 182);
            this.buttonKey2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey2.Name = "buttonKey2";
            this.buttonKey2.Size = new System.Drawing.Size(52, 45);
            this.buttonKey2.TabIndex = 45;
            this.buttonKey2.Text = "<";
            this.buttonKey2.UseVisualStyleBackColor = false;
            // 
            // buttonKey3
            // 
            this.buttonKey3.BackColor = System.Drawing.Color.White;
            this.buttonKey3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey3.Location = new System.Drawing.Point(541, 131);
            this.buttonKey3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey3.Name = "buttonKey3";
            this.buttonKey3.Size = new System.Drawing.Size(52, 41);
            this.buttonKey3.TabIndex = 46;
            this.buttonKey3.Text = "<";
            this.buttonKey3.UseVisualStyleBackColor = false;
            // 
            // buttonKey4
            // 
            this.buttonKey4.BackColor = System.Drawing.Color.White;
            this.buttonKey4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey4.Location = new System.Drawing.Point(541, 76);
            this.buttonKey4.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey4.Name = "buttonKey4";
            this.buttonKey4.Size = new System.Drawing.Size(52, 45);
            this.buttonKey4.TabIndex = 47;
            this.buttonKey4.Text = "<";
            this.buttonKey4.UseVisualStyleBackColor = false;
            // 
            // buttonKey5
            // 
            this.buttonKey5.BackColor = System.Drawing.Color.White;
            this.buttonKey5.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey5.Location = new System.Drawing.Point(430, 152);
            this.buttonKey5.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey5.Name = "buttonKey5";
            this.buttonKey5.Size = new System.Drawing.Size(72, 54);
            this.buttonKey5.TabIndex = 48;
            this.buttonKey5.Text = "Help";
            this.buttonKey5.UseVisualStyleBackColor = false;
            // 
            // buttonKey6
            // 
            this.buttonKey6.BackColor = System.Drawing.Color.White;
            this.buttonKey6.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey6.Location = new System.Drawing.Point(339, 76);
            this.buttonKey6.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey6.Name = "buttonKey6";
            this.buttonKey6.Size = new System.Drawing.Size(52, 45);
            this.buttonKey6.TabIndex = 49;
            this.buttonKey6.Text = ">";
            this.buttonKey6.UseVisualStyleBackColor = false;
            // 
            // buttonKey7
            // 
            this.buttonKey7.BackColor = System.Drawing.Color.White;
            this.buttonKey7.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey7.Location = new System.Drawing.Point(339, 127);
            this.buttonKey7.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey7.Name = "buttonKey7";
            this.buttonKey7.Size = new System.Drawing.Size(52, 45);
            this.buttonKey7.TabIndex = 50;
            this.buttonKey7.Text = ">";
            this.buttonKey7.UseVisualStyleBackColor = false;
            // 
            // buttonKey8
            // 
            this.buttonKey8.BackColor = System.Drawing.Color.White;
            this.buttonKey8.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey8.Location = new System.Drawing.Point(339, 182);
            this.buttonKey8.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey8.Name = "buttonKey8";
            this.buttonKey8.Size = new System.Drawing.Size(52, 45);
            this.buttonKey8.TabIndex = 51;
            this.buttonKey8.Text = ">";
            this.buttonKey8.UseVisualStyleBackColor = false;
            // 
            // buttonKey9
            // 
            this.buttonKey9.BackColor = System.Drawing.Color.White;
            this.buttonKey9.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonKey9.Location = new System.Drawing.Point(339, 237);
            this.buttonKey9.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonKey9.Name = "buttonKey9";
            this.buttonKey9.Size = new System.Drawing.Size(52, 45);
            this.buttonKey9.TabIndex = 52;
            this.buttonKey9.Text = ">";
            this.buttonKey9.UseVisualStyleBackColor = false;
            // 
            // buttonStop
            // 
            this.buttonStop.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Bold);
            this.buttonStop.Location = new System.Drawing.Point(198, 64);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(105, 83);
            this.buttonStop.TabIndex = 53;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonKey1);
            this.groupBox4.Controls.Add(this.buttonStop);
            this.groupBox4.Controls.Add(this.buttonKey2);
            this.groupBox4.Controls.Add(this.buttonKey9);
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.buttonKey3);
            this.groupBox4.Controls.Add(this.buttonKey8);
            this.groupBox4.Controls.Add(this.buttonKey4);
            this.groupBox4.Controls.Add(this.buttonKey7);
            this.groupBox4.Controls.Add(this.buttonKey5);
            this.groupBox4.Controls.Add(this.buttonKey6);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(14, 379);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.groupBox4.Size = new System.Drawing.Size(623, 345);
            this.groupBox4.TabIndex = 54;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "G7ADA Monitor";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1582, 833);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Label_Status);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximumSize = new System.Drawing.Size(1600, 900);
            this.MinimumSize = new System.Drawing.Size(921, 329);
            this.Name = "FormMain";
            this.Text = "FTDI MPSSE INTERFACE";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonInit;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label Label_Status;
        private System.Windows.Forms.Label labelDUTStatus;
        private System.Windows.Forms.Button buttonRed;
        private System.Windows.Forms.Button buttonGreen;
        private System.Windows.Forms.Button buttonBlue;
        private System.Windows.Forms.Label labelFTDIStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelFTDIType;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonKey1;
        private System.Windows.Forms.Button buttonKey2;
        private System.Windows.Forms.Button buttonKey3;
        private System.Windows.Forms.Button buttonKey4;
        private System.Windows.Forms.Button buttonKey5;
        private System.Windows.Forms.Button buttonKey6;
        private System.Windows.Forms.Button buttonKey7;
        private System.Windows.Forms.Button buttonKey8;
        private System.Windows.Forms.Button buttonKey9;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}

