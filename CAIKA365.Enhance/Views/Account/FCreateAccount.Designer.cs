namespace CAIKA365.Enhance.Views
{
    partial class FCreateAccount
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ChkCom = new System.Windows.Forms.CheckBox();
            this.ChkAgent = new System.Windows.Forms.CheckBox();
            this.ChkSystem = new System.Windows.Forms.CheckBox();
            this.standardGrid1 = new AosuApp.Windows.Controls.StandardGrid();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TxtParentAgent = new AosuApp.Windows.Controls.TextBoxExPopup();
            this.TxtConfirm = new AosuApp.Windows.Controls.TextBoxEx();
            this.TxtPassword = new AosuApp.Windows.Controls.TextBoxEx();
            this.TxtAccountID = new AosuApp.Windows.Controls.TextBoxEx();
            this.CBTypes = new AosuApp.Windows.Controls.ComboBoxEx();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.standardGrid1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnCancel);
            this.panel1.Controls.Add(this.BtnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 328);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(591, 43);
            this.panel1.TabIndex = 2;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(486, 7);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(100, 32);
            this.BtnCancel.TabIndex = 22;
            this.BtnCancel.Text = "取消(&C)";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOK
            // 
            this.BtnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOK.Location = new System.Drawing.Point(382, 7);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(100, 32);
            this.BtnOK.TabIndex = 21;
            this.BtnOK.Text = "确认(&O)";
            this.BtnOK.UseVisualStyleBackColor = true;
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "账号";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "密码";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(13, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 24);
            this.label3.TabIndex = 1;
            this.label3.Text = "用户类型";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChkCom
            // 
            this.ChkCom.Location = new System.Drawing.Point(96, 127);
            this.ChkCom.Name = "ChkCom";
            this.ChkCom.Size = new System.Drawing.Size(151, 24);
            this.ChkCom.TabIndex = 4;
            this.ChkCom.Tag = "caika.com";
            this.ChkCom.Text = "com域";
            this.ChkCom.UseVisualStyleBackColor = true;
            // 
            // ChkAgent
            // 
            this.ChkAgent.Location = new System.Drawing.Point(96, 155);
            this.ChkAgent.Name = "ChkAgent";
            this.ChkAgent.Size = new System.Drawing.Size(151, 24);
            this.ChkAgent.TabIndex = 5;
            this.ChkAgent.Tag = "caika.agent";
            this.ChkAgent.Text = "agent域";
            this.ChkAgent.UseVisualStyleBackColor = true;
            // 
            // ChkSystem
            // 
            this.ChkSystem.Location = new System.Drawing.Point(96, 183);
            this.ChkSystem.Name = "ChkSystem";
            this.ChkSystem.Size = new System.Drawing.Size(151, 24);
            this.ChkSystem.TabIndex = 7;
            this.ChkSystem.Tag = "caika.system";
            this.ChkSystem.Text = "system域";
            this.ChkSystem.UseVisualStyleBackColor = true;
            // 
            // standardGrid1
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.standardGrid1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.standardGrid1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.standardGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.standardGrid1.Location = new System.Drawing.Point(3, 19);
            this.standardGrid1.Name = "standardGrid1";
            this.standardGrid1.RowTemplate.Height = 23;
            this.standardGrid1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.standardGrid1.Size = new System.Drawing.Size(296, 303);
            this.standardGrid1.TabIndex = 17;
            this.standardGrid1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.standardGrid1_CellEnter);
            this.standardGrid1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.standardGrid1_CellFormatting);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "MCCanShu";
            this.Column1.HeaderText = "项";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 180;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.DataPropertyName = "CanShuZhi";
            this.Column2.HeaderText = "值";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 24);
            this.label4.TabIndex = 1;
            this.label4.Text = "密码确认";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TxtParentAgent);
            this.groupBox1.Controls.Add(this.TxtConfirm);
            this.groupBox1.Controls.Add(this.TxtPassword);
            this.groupBox1.Controls.Add(this.TxtAccountID);
            this.groupBox1.Controls.Add(this.CBTypes);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.ChkSystem);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.ChkAgent);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ChkCom);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 325);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "账户信息";
            // 
            // TxtParentAgent
            // 
            this.TxtParentAgent.EnableValidateEvent = true;
            this.TxtParentAgent.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtParentAgent.IsCode = true;
            this.TxtParentAgent.Location = new System.Drawing.Point(96, 213);
            this.TxtParentAgent.Margin = new System.Windows.Forms.Padding(0);
            this.TxtParentAgent.Name = "TxtParentAgent";
            this.TxtParentAgent.Size = new System.Drawing.Size(169, 23);
            this.TxtParentAgent.TabIndex = 8;
            this.TxtParentAgent.RequestValidationEvent += new System.EventHandler(this.Controls_RequestValidationEvent);
            this.TxtParentAgent.ShowListEvent += new AosuApp.Windows.Controls.ShowListEventHandler(this.TxtAgentCode_ShowListEvent);
            // 
            // TxtConfirm
            // 
            this.TxtConfirm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtConfirm.EnableValidateEvent = true;
            this.TxtConfirm.IsCode = false;
            this.TxtConfirm.Location = new System.Drawing.Point(96, 71);
            this.TxtConfirm.Name = "TxtConfirm";
            this.TxtConfirm.PasswordChar = '*';
            this.TxtConfirm.Size = new System.Drawing.Size(169, 23);
            this.TxtConfirm.TabIndex = 2;
            this.TxtConfirm.RequestValidationEvent += new System.EventHandler(this.Controls_RequestValidationEvent);
            // 
            // TxtPassword
            // 
            this.TxtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtPassword.EnableValidateEvent = true;
            this.TxtPassword.IsCode = false;
            this.TxtPassword.Location = new System.Drawing.Point(96, 46);
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.PasswordChar = '*';
            this.TxtPassword.Size = new System.Drawing.Size(169, 23);
            this.TxtPassword.TabIndex = 1;
            this.TxtPassword.RequestValidationEvent += new System.EventHandler(this.Controls_RequestValidationEvent);
            // 
            // TxtAccountID
            // 
            this.TxtAccountID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtAccountID.EnableValidateEvent = true;
            this.TxtAccountID.IsCode = false;
            this.TxtAccountID.Location = new System.Drawing.Point(96, 21);
            this.TxtAccountID.Name = "TxtAccountID";
            this.TxtAccountID.Size = new System.Drawing.Size(169, 23);
            this.TxtAccountID.TabIndex = 0;
            this.TxtAccountID.RequestValidationEvent += new System.EventHandler(this.Controls_RequestValidationEvent);
            // 
            // CBTypes
            // 
            this.CBTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBTypes.EnableValidateEvent = true;
            this.CBTypes.FormattingEnabled = true;
            this.CBTypes.Location = new System.Drawing.Point(96, 96);
            this.CBTypes.Name = "CBTypes";
            this.CBTypes.Size = new System.Drawing.Size(169, 25);
            this.CBTypes.TabIndex = 3;
            this.CBTypes.SelectedIndexChanged += new System.EventHandler(this.CBTypes_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 24);
            this.label5.TabIndex = 1;
            this.label5.Text = "上级代理编码";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.standardGrid1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(292, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 325);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "资金签名";
            // 
            // splitter1
            // 
            this.splitter1.Enabled = false;
            this.splitter1.Location = new System.Drawing.Point(287, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 325);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FCreateAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(597, 374);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "FCreateAccount";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "创建账户";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.standardGrid1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BtnOK;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ChkCom;
        private System.Windows.Forms.CheckBox ChkAgent;
        private System.Windows.Forms.CheckBox ChkSystem;
        private AosuApp.Windows.Controls.StandardGrid standardGrid1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private AosuApp.Windows.Controls.ComboBoxEx CBTypes;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private AosuApp.Windows.Controls.TextBoxEx TxtAccountID;
        private AosuApp.Windows.Controls.TextBoxEx TxtConfirm;
        private AosuApp.Windows.Controls.TextBoxEx TxtPassword;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label5;
        private AosuApp.Windows.Controls.TextBoxExPopup TxtParentAgent;
    }
}