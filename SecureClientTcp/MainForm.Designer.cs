namespace SecureClientTcp
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.regButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.autoButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.loginTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "ipAdress";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "port";
            // 
            // regButton
            // 
            this.regButton.Location = new System.Drawing.Point(536, 12);
            this.regButton.Name = "regButton";
            this.regButton.Size = new System.Drawing.Size(154, 34);
            this.regButton.TabIndex = 5;
            this.regButton.Text = "Зарегестрироваться";
            this.regButton.UseVisualStyleBackColor = true;
            this.regButton.Click += new System.EventHandler(this.regButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(101, 20);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "localhost";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(65, 58);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(101, 20);
            this.textBox2.TabIndex = 7;
            this.textBox2.Text = "11000";
            // 
            // autoButton
            // 
            this.autoButton.Location = new System.Drawing.Point(536, 61);
            this.autoButton.Name = "autoButton";
            this.autoButton.Size = new System.Drawing.Size(154, 34);
            this.autoButton.TabIndex = 8;
            this.autoButton.Text = "Авторизоваться";
            this.autoButton.UseVisualStyleBackColor = true;
            this.autoButton.Click += new System.EventHandler(this.autoButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(216, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Логин";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(275, 52);
            this.passwordTextBox.Multiline = true;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(177, 22);
            this.passwordTextBox.TabIndex = 15;
            // 
            // loginTextBox
            // 
            this.loginTextBox.Location = new System.Drawing.Point(275, 25);
            this.loginTextBox.Multiline = true;
            this.loginTextBox.Name = "loginTextBox";
            this.loginTextBox.Size = new System.Drawing.Size(177, 19);
            this.loginTextBox.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(216, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Пароль";
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(54, 171);
            this.chatTextBox.Multiline = true;
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(470, 218);
            this.chatTextBox.TabIndex = 19;
            this.chatTextBox.Visible = false;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(462, 120);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(121, 21);
            this.sendButton.TabIndex = 20;
            this.sendButton.Text = "Отправить";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Visible = false;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Введите сообщение";
            this.label7.Visible = false;
            // 
            // messageTextBox
            // 
            this.messageTextBox.Location = new System.Drawing.Point(127, 121);
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(309, 20);
            this.messageTextBox.TabIndex = 22;
            this.messageTextBox.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 409);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.loginTextBox);
            this.Controls.Add(this.autoButton);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.regButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button regButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button autoButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox loginTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox chatTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox messageTextBox;
    }
}

