namespace Game1
{
	partial class LeaderPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelLeader = new System.Windows.Forms.Label();
			this.buttonInfo = new System.Windows.Forms.Button();
			this.labelNumber = new System.Windows.Forms.Label();
			this.buttonLeader = new System.Windows.Forms.Button();
			this.labelLeaderLv = new System.Windows.Forms.Label();
			this.labelUnit = new System.Windows.Forms.Label();
			this.labelLeaderUp = new System.Windows.Forms.Label();
			this.labelUnitLv = new System.Windows.Forms.Label();
			this.labelUnitUp = new System.Windows.Forms.Label();
			this.buttonUnit = new System.Windows.Forms.Button();
			this.buttonHeal = new System.Windows.Forms.Button();
			this.labelHeal = new System.Windows.Forms.Label();
			this.labelHire = new System.Windows.Forms.Label();
			this.buttonSp = new System.Windows.Forms.Button();
			this.labelSp = new System.Windows.Forms.Label();
			this.buttonDisband = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelLeader
			// 
			this.labelLeader.AutoSize = true;
			this.labelLeader.Location = new System.Drawing.Point(30, 6);
			this.labelLeader.Name = "labelLeader";
			this.labelLeader.Size = new System.Drawing.Size(29, 12);
			this.labelLeader.TabIndex = 0;
			this.labelLeader.Text = "名字";
			// 
			// buttonInfo
			// 
			this.buttonInfo.Location = new System.Drawing.Point(0, 0);
			this.buttonInfo.Name = "buttonInfo";
			this.buttonInfo.Size = new System.Drawing.Size(24, 24);
			this.buttonInfo.TabIndex = 1;
			this.buttonInfo.Text = "i";
			this.buttonInfo.UseVisualStyleBackColor = true;
			this.buttonInfo.Click += new System.EventHandler(this.buttonInfo_Click);
			// 
			// labelNumber
			// 
			this.labelNumber.AutoSize = true;
			this.labelNumber.Location = new System.Drawing.Point(30, 105);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(29, 12);
			this.labelNumber.TabIndex = 2;
			this.labelNumber.Text = "兵力";
			// 
			// buttonLeader
			// 
			this.buttonLeader.Location = new System.Drawing.Point(113, 10);
			this.buttonLeader.Name = "buttonLeader";
			this.buttonLeader.Size = new System.Drawing.Size(32, 32);
			this.buttonLeader.TabIndex = 3;
			this.buttonLeader.Text = "+";
			this.buttonLeader.UseVisualStyleBackColor = true;
			this.buttonLeader.Click += new System.EventHandler(this.buttonLeader_Click);
			// 
			// labelLeaderLv
			// 
			this.labelLeaderLv.AutoSize = true;
			this.labelLeaderLv.Location = new System.Drawing.Point(30, 18);
			this.labelLeaderLv.Name = "labelLeaderLv";
			this.labelLeaderLv.Size = new System.Drawing.Size(30, 12);
			this.labelLeaderLv.TabIndex = 4;
			this.labelLeaderLv.Text = "Lv: 0";
			// 
			// labelUnit
			// 
			this.labelUnit.AutoSize = true;
			this.labelUnit.Location = new System.Drawing.Point(30, 56);
			this.labelUnit.Name = "labelUnit";
			this.labelUnit.Size = new System.Drawing.Size(37, 12);
			this.labelUnit.TabIndex = 5;
			this.labelUnit.Text = "(兵種)";
			// 
			// labelLeaderUp
			// 
			this.labelLeaderUp.AutoSize = true;
			this.labelLeaderUp.Location = new System.Drawing.Point(30, 30);
			this.labelLeaderUp.Name = "labelLeaderUp";
			this.labelLeaderUp.Size = new System.Drawing.Size(65, 12);
			this.labelLeaderUp.TabIndex = 6;
			this.labelLeaderUp.Text = "升級需點數";
			// 
			// labelUnitLv
			// 
			this.labelUnitLv.AutoSize = true;
			this.labelUnitLv.Location = new System.Drawing.Point(30, 68);
			this.labelUnitLv.Name = "labelUnitLv";
			this.labelUnitLv.Size = new System.Drawing.Size(30, 12);
			this.labelUnitLv.TabIndex = 7;
			this.labelUnitLv.Text = "Lv: 0";
			// 
			// labelUnitUp
			// 
			this.labelUnitUp.AutoSize = true;
			this.labelUnitUp.Location = new System.Drawing.Point(30, 80);
			this.labelUnitUp.Name = "labelUnitUp";
			this.labelUnitUp.Size = new System.Drawing.Size(65, 12);
			this.labelUnitUp.TabIndex = 8;
			this.labelUnitUp.Text = "升級需點數";
			// 
			// buttonUnit
			// 
			this.buttonUnit.Location = new System.Drawing.Point(113, 60);
			this.buttonUnit.Name = "buttonUnit";
			this.buttonUnit.Size = new System.Drawing.Size(32, 32);
			this.buttonUnit.TabIndex = 9;
			this.buttonUnit.Text = "+";
			this.buttonUnit.UseVisualStyleBackColor = true;
			this.buttonUnit.Click += new System.EventHandler(this.buttonUnit_Click);
			// 
			// buttonHeal
			// 
			this.buttonHeal.Location = new System.Drawing.Point(113, 107);
			this.buttonHeal.Name = "buttonHeal";
			this.buttonHeal.Size = new System.Drawing.Size(32, 32);
			this.buttonHeal.TabIndex = 10;
			this.buttonHeal.Text = "R";
			this.buttonHeal.UseVisualStyleBackColor = true;
			this.buttonHeal.Click += new System.EventHandler(this.buttonHeal_Click);
			// 
			// labelHeal
			// 
			this.labelHeal.AutoSize = true;
			this.labelHeal.Location = new System.Drawing.Point(30, 117);
			this.labelHeal.Name = "labelHeal";
			this.labelHeal.Size = new System.Drawing.Size(65, 12);
			this.labelHeal.TabIndex = 11;
			this.labelHeal.Text = "回復需點數";
			// 
			// labelHire
			// 
			this.labelHire.AutoSize = true;
			this.labelHire.Location = new System.Drawing.Point(30, 129);
			this.labelHire.Name = "labelHire";
			this.labelHire.Size = new System.Drawing.Size(65, 12);
			this.labelHire.TabIndex = 12;
			this.labelHire.Text = "雇用需點數";
			// 
			// buttonSp
			// 
			this.buttonSp.Location = new System.Drawing.Point(-1, 127);
			this.buttonSp.Name = "buttonSp";
			this.buttonSp.Size = new System.Drawing.Size(32, 32);
			this.buttonSp.TabIndex = 13;
			this.buttonSp.Text = "SP+";
			this.buttonSp.UseVisualStyleBackColor = true;
			this.buttonSp.Click += new System.EventHandler(this.buttonSp_Click);
			// 
			// labelSp
			// 
			this.labelSp.AutoSize = true;
			this.labelSp.Location = new System.Drawing.Point(30, 141);
			this.labelSp.Name = "labelSp";
			this.labelSp.Size = new System.Drawing.Size(17, 12);
			this.labelSp.TabIndex = 14;
			this.labelSp.Text = "SP";
			// 
			// buttonDisband
			// 
			this.buttonDisband.Location = new System.Drawing.Point(141, -1);
			this.buttonDisband.Name = "buttonDisband";
			this.buttonDisband.Size = new System.Drawing.Size(18, 18);
			this.buttonDisband.TabIndex = 15;
			this.buttonDisband.Text = "X";
			this.buttonDisband.UseVisualStyleBackColor = true;
			this.buttonDisband.Click += new System.EventHandler(this.buttonDisband_Click);
			// 
			// LeaderPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.buttonDisband);
			this.Controls.Add(this.labelSp);
			this.Controls.Add(this.buttonSp);
			this.Controls.Add(this.labelHire);
			this.Controls.Add(this.labelHeal);
			this.Controls.Add(this.buttonHeal);
			this.Controls.Add(this.buttonUnit);
			this.Controls.Add(this.labelUnitUp);
			this.Controls.Add(this.labelUnitLv);
			this.Controls.Add(this.labelLeaderUp);
			this.Controls.Add(this.labelUnit);
			this.Controls.Add(this.labelLeaderLv);
			this.Controls.Add(this.buttonLeader);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.buttonInfo);
			this.Controls.Add(this.labelLeader);
			this.Name = "LeaderPanel";
			this.Size = new System.Drawing.Size(158, 158);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelLeader;
		private System.Windows.Forms.Button buttonInfo;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.Button buttonLeader;
		private System.Windows.Forms.Label labelLeaderLv;
		private System.Windows.Forms.Label labelUnit;
		private System.Windows.Forms.Label labelLeaderUp;
		private System.Windows.Forms.Label labelUnitLv;
		private System.Windows.Forms.Label labelUnitUp;
		private System.Windows.Forms.Button buttonUnit;
		private System.Windows.Forms.Button buttonHeal;
		private System.Windows.Forms.Label labelHeal;
		private System.Windows.Forms.Label labelHire;
		private System.Windows.Forms.Button buttonSp;
		private System.Windows.Forms.Label labelSp;
		private System.Windows.Forms.Button buttonDisband;
	}
}
