namespace WinFormClient
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIVRCode = new System.Windows.Forms.TextBox();
            this.txtSubscriberId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMemberSeq = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAuthTypeId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDateOfBirth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSelectedAuthType = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(35, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Hello World";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ivrCode";
            // 
            // txtIVRCode
            // 
            this.txtIVRCode.Location = new System.Drawing.Point(121, 86);
            this.txtIVRCode.Name = "txtIVRCode";
            this.txtIVRCode.Size = new System.Drawing.Size(127, 20);
            this.txtIVRCode.TabIndex = 2;
            this.txtIVRCode.Text = "8621517";
            // 
            // txtSubscriberId
            // 
            this.txtSubscriberId.Location = new System.Drawing.Point(121, 131);
            this.txtSubscriberId.Name = "txtSubscriberId";
            this.txtSubscriberId.Size = new System.Drawing.Size(127, 20);
            this.txtSubscriberId.TabIndex = 4;
            this.txtSubscriberId.Text = "101584922001";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "subscriberId";
            // 
            // txtMemberSeq
            // 
            this.txtMemberSeq.Location = new System.Drawing.Point(121, 169);
            this.txtMemberSeq.Name = "txtMemberSeq";
            this.txtMemberSeq.Size = new System.Drawing.Size(127, 20);
            this.txtMemberSeq.TabIndex = 6;
            this.txtMemberSeq.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "memberSeq";
            // 
            // txtAuthTypeId
            // 
            this.txtAuthTypeId.Location = new System.Drawing.Point(121, 212);
            this.txtAuthTypeId.Name = "txtAuthTypeId";
            this.txtAuthTypeId.Size = new System.Drawing.Size(127, 20);
            this.txtAuthTypeId.TabIndex = 8;
            this.txtAuthTypeId.Text = "PTOT";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "authTypeId";
            // 
            // txtDateOfBirth
            // 
            this.txtDateOfBirth.Location = new System.Drawing.Point(121, 258);
            this.txtDateOfBirth.Name = "txtDateOfBirth";
            this.txtDateOfBirth.Size = new System.Drawing.Size(127, 20);
            this.txtDateOfBirth.TabIndex = 10;
            this.txtDateOfBirth.Text = "5/6/1956";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 258);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "dateOfBirth";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 301);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "startDate";
            // 
            // txtSelectedAuthType
            // 
            this.txtSelectedAuthType.Location = new System.Drawing.Point(121, 340);
            this.txtSelectedAuthType.Name = "txtSelectedAuthType";
            this.txtSelectedAuthType.Size = new System.Drawing.Size(127, 20);
            this.txtSelectedAuthType.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 340);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "selectedAuthType";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(44, 386);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(123, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Check Member";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(121, 301);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(200, 20);
            this.dtpStartDate.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 499);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtSelectedAuthType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDateOfBirth);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAuthTypeId);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtMemberSeq);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSubscriberId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtIVRCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIVRCode;
        private System.Windows.Forms.TextBox txtSubscriberId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMemberSeq;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAuthTypeId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDateOfBirth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSelectedAuthType;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
    }
}

