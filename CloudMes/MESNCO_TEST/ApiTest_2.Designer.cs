
namespace MESNCO_TEST
{
    partial class ApiTest_2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApiTest_2));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMESServic = new System.Windows.Forms.TabPage();
            this.tbLoginMsg = new System.Windows.Forms.TextBox();
            this.labelLoginMsg = new System.Windows.Forms.Label();
            this.labelFunctionName = new System.Windows.Forms.Label();
            this.cbbFunction = new System.Windows.Forms.ComboBox();
            this.cbbBU = new System.Windows.Forms.ComboBox();
            this.labelApiOutput = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tbApiOutput = new System.Windows.Forms.TextBox();
            this.labelApiInput = new System.Windows.Forms.Label();
            this.tbApiInput = new System.Windows.Forms.TextBox();
            this.tbPwd = new System.Windows.Forms.TextBox();
            this.tbEmp = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.labelBU = new System.Windows.Forms.Label();
            this.labelPwd = new System.Windows.Forms.Label();
            this.labelEmp = new System.Windows.Forms.Label();
            this.tbMESServic = new System.Windows.Forms.TextBox();
            this.labelMESServic = new System.Windows.Forms.Label();
            this.tabWebAPI = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tbOutputData = new System.Windows.Forms.TextBox();
            this.labelOutputData = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.tbInputData = new System.Windows.Forms.TextBox();
            this.labelInputData = new System.Windows.Forms.Label();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.labelUrl = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabMESServic.SuspendLayout();
            this.tabWebAPI.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMESServic);
            this.tabControl1.Controls.Add(this.tabWebAPI);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1015, 747);
            this.tabControl1.TabIndex = 0;
            // 
            // tabMESServic
            // 
            this.tabMESServic.Controls.Add(this.button2);
            this.tabMESServic.Controls.Add(this.tbLoginMsg);
            this.tabMESServic.Controls.Add(this.labelLoginMsg);
            this.tabMESServic.Controls.Add(this.labelFunctionName);
            this.tabMESServic.Controls.Add(this.cbbFunction);
            this.tabMESServic.Controls.Add(this.cbbBU);
            this.tabMESServic.Controls.Add(this.labelApiOutput);
            this.tabMESServic.Controls.Add(this.btnSubmit);
            this.tabMESServic.Controls.Add(this.tbApiOutput);
            this.tabMESServic.Controls.Add(this.labelApiInput);
            this.tabMESServic.Controls.Add(this.tbApiInput);
            this.tabMESServic.Controls.Add(this.tbPwd);
            this.tabMESServic.Controls.Add(this.tbEmp);
            this.tabMESServic.Controls.Add(this.btnLogin);
            this.tabMESServic.Controls.Add(this.labelBU);
            this.tabMESServic.Controls.Add(this.labelPwd);
            this.tabMESServic.Controls.Add(this.labelEmp);
            this.tabMESServic.Controls.Add(this.tbMESServic);
            this.tabMESServic.Controls.Add(this.labelMESServic);
            this.tabMESServic.Location = new System.Drawing.Point(4, 22);
            this.tabMESServic.Name = "tabMESServic";
            this.tabMESServic.Padding = new System.Windows.Forms.Padding(3);
            this.tabMESServic.Size = new System.Drawing.Size(1007, 721);
            this.tabMESServic.TabIndex = 0;
            this.tabMESServic.Text = "MESServic";
            this.tabMESServic.UseVisualStyleBackColor = true;
            // 
            // tbLoginMsg
            // 
            this.tbLoginMsg.Location = new System.Drawing.Point(88, 115);
            this.tbLoginMsg.Multiline = true;
            this.tbLoginMsg.Name = "tbLoginMsg";
            this.tbLoginMsg.ReadOnly = true;
            this.tbLoginMsg.Size = new System.Drawing.Size(643, 41);
            this.tbLoginMsg.TabIndex = 18;
            // 
            // labelLoginMsg
            // 
            this.labelLoginMsg.AutoSize = true;
            this.labelLoginMsg.Location = new System.Drawing.Point(24, 115);
            this.labelLoginMsg.Name = "labelLoginMsg";
            this.labelLoginMsg.Size = new System.Drawing.Size(65, 12);
            this.labelLoginMsg.TabIndex = 17;
            this.labelLoginMsg.Text = "Login MSG:";
            // 
            // labelFunctionName
            // 
            this.labelFunctionName.AutoSize = true;
            this.labelFunctionName.Location = new System.Drawing.Point(27, 173);
            this.labelFunctionName.Name = "labelFunctionName";
            this.labelFunctionName.Size = new System.Drawing.Size(89, 12);
            this.labelFunctionName.TabIndex = 16;
            this.labelFunctionName.Text = "Fucntion Name:";
            // 
            // cbbFunction
            // 
            this.cbbFunction.FormattingEnabled = true;
            this.cbbFunction.Items.AddRange(new object[] {
            "UpdateRotationDetail"});
            this.cbbFunction.Location = new System.Drawing.Point(115, 169);
            this.cbbFunction.Name = "cbbFunction";
            this.cbbFunction.Size = new System.Drawing.Size(272, 20);
            this.cbbFunction.TabIndex = 15;
            // 
            // cbbBU
            // 
            this.cbbBU.FormattingEnabled = true;
            this.cbbBU.Items.AddRange(new object[] {
            "VNDCN"});
            this.cbbBU.Location = new System.Drawing.Point(459, 31);
            this.cbbBU.Name = "cbbBU";
            this.cbbBU.Size = new System.Drawing.Size(272, 20);
            this.cbbBU.TabIndex = 14;
            this.cbbBU.Text = "VNDCN";
            // 
            // labelApiOutput
            // 
            this.labelApiOutput.AutoSize = true;
            this.labelApiOutput.Location = new System.Drawing.Point(579, 194);
            this.labelApiOutput.Name = "labelApiOutput";
            this.labelApiOutput.Size = new System.Drawing.Size(65, 12);
            this.labelApiOutput.TabIndex = 13;
            this.labelApiOutput.Text = "ApiOutput:";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(448, 371);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(115, 66);
            this.btnSubmit.TabIndex = 12;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // tbApiOutput
            // 
            this.tbApiOutput.Location = new System.Drawing.Point(582, 222);
            this.tbApiOutput.Multiline = true;
            this.tbApiOutput.Name = "tbApiOutput";
            this.tbApiOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbApiOutput.Size = new System.Drawing.Size(401, 486);
            this.tbApiOutput.TabIndex = 11;
            // 
            // labelApiInput
            // 
            this.labelApiInput.AutoSize = true;
            this.labelApiInput.Location = new System.Drawing.Point(28, 194);
            this.labelApiInput.Name = "labelApiInput";
            this.labelApiInput.Size = new System.Drawing.Size(59, 12);
            this.labelApiInput.TabIndex = 10;
            this.labelApiInput.Text = "ApiInput:";
            // 
            // tbApiInput
            // 
            this.tbApiInput.Location = new System.Drawing.Point(30, 222);
            this.tbApiInput.Multiline = true;
            this.tbApiInput.Name = "tbApiInput";
            this.tbApiInput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbApiInput.Size = new System.Drawing.Size(399, 486);
            this.tbApiInput.TabIndex = 9;
            this.tbApiInput.Text = resources.GetString("tbApiInput.Text");
            // 
            // tbPwd
            // 
            this.tbPwd.Location = new System.Drawing.Point(459, 60);
            this.tbPwd.Name = "tbPwd";
            this.tbPwd.PasswordChar = '*';
            this.tbPwd.Size = new System.Drawing.Size(272, 21);
            this.tbPwd.TabIndex = 7;
            this.tbPwd.Text = "MESTEST";
            // 
            // tbEmp
            // 
            this.tbEmp.Location = new System.Drawing.Point(88, 57);
            this.tbEmp.Name = "tbEmp";
            this.tbEmp.Size = new System.Drawing.Size(272, 21);
            this.tbEmp.TabIndex = 6;
            this.tbEmp.Text = "TEST";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(88, 89);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(140, 21);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // labelBU
            // 
            this.labelBU.AutoSize = true;
            this.labelBU.Location = new System.Drawing.Point(417, 33);
            this.labelBU.Name = "labelBU";
            this.labelBU.Size = new System.Drawing.Size(23, 12);
            this.labelBU.TabIndex = 4;
            this.labelBU.Text = "BU:";
            // 
            // labelPwd
            // 
            this.labelPwd.AutoSize = true;
            this.labelPwd.Location = new System.Drawing.Point(386, 63);
            this.labelPwd.Name = "labelPwd";
            this.labelPwd.Size = new System.Drawing.Size(59, 12);
            this.labelPwd.TabIndex = 3;
            this.labelPwd.Text = "Password:";
            // 
            // labelEmp
            // 
            this.labelEmp.AutoSize = true;
            this.labelEmp.Location = new System.Drawing.Point(24, 60);
            this.labelEmp.Name = "labelEmp";
            this.labelEmp.Size = new System.Drawing.Size(53, 12);
            this.labelEmp.TabIndex = 2;
            this.labelEmp.Text = "Emp NO.:";
            // 
            // tbMESServic
            // 
            this.tbMESServic.Location = new System.Drawing.Point(88, 29);
            this.tbMESServic.Name = "tbMESServic";
            this.tbMESServic.Size = new System.Drawing.Size(272, 21);
            this.tbMESServic.TabIndex = 1;
            this.tbMESServic.Text = "ws://127.0.0.1:2130/ReportService";
            // 
            // labelMESServic
            // 
            this.labelMESServic.AutoSize = true;
            this.labelMESServic.Location = new System.Drawing.Point(21, 31);
            this.labelMESServic.Name = "labelMESServic";
            this.labelMESServic.Size = new System.Drawing.Size(65, 12);
            this.labelMESServic.TabIndex = 0;
            this.labelMESServic.Text = "MESServic:";
            // 
            // tabWebAPI
            // 
            this.tabWebAPI.Controls.Add(this.button1);
            this.tabWebAPI.Controls.Add(this.tbOutputData);
            this.tabWebAPI.Controls.Add(this.labelOutputData);
            this.tabWebAPI.Controls.Add(this.btnPost);
            this.tabWebAPI.Controls.Add(this.tbInputData);
            this.tabWebAPI.Controls.Add(this.labelInputData);
            this.tabWebAPI.Controls.Add(this.tbUrl);
            this.tabWebAPI.Controls.Add(this.labelUrl);
            this.tabWebAPI.Location = new System.Drawing.Point(4, 22);
            this.tabWebAPI.Name = "tabWebAPI";
            this.tabWebAPI.Padding = new System.Windows.Forms.Padding(3);
            this.tabWebAPI.Size = new System.Drawing.Size(1007, 721);
            this.tabWebAPI.TabIndex = 1;
            this.tabWebAPI.Text = "WebAPI";
            this.tabWebAPI.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(448, 173);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbOutputData
            // 
            this.tbOutputData.Location = new System.Drawing.Point(577, 99);
            this.tbOutputData.Multiline = true;
            this.tbOutputData.Name = "tbOutputData";
            this.tbOutputData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutputData.Size = new System.Drawing.Size(399, 497);
            this.tbOutputData.TabIndex = 9;
            // 
            // labelOutputData
            // 
            this.labelOutputData.AutoSize = true;
            this.labelOutputData.Location = new System.Drawing.Point(574, 76);
            this.labelOutputData.Name = "labelOutputData";
            this.labelOutputData.Size = new System.Drawing.Size(77, 12);
            this.labelOutputData.TabIndex = 8;
            this.labelOutputData.Text = "Output Data:";
            // 
            // btnPost
            // 
            this.btnPost.Location = new System.Drawing.Point(448, 126);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 21);
            this.btnPost.TabIndex = 7;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // tbInputData
            // 
            this.tbInputData.Location = new System.Drawing.Point(33, 99);
            this.tbInputData.Multiline = true;
            this.tbInputData.Name = "tbInputData";
            this.tbInputData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbInputData.Size = new System.Drawing.Size(365, 509);
            this.tbInputData.TabIndex = 5;
            this.tbInputData.Text = resources.GetString("tbInputData.Text");
            // 
            // labelInputData
            // 
            this.labelInputData.AutoSize = true;
            this.labelInputData.Location = new System.Drawing.Point(30, 76);
            this.labelInputData.Name = "labelInputData";
            this.labelInputData.Size = new System.Drawing.Size(71, 12);
            this.labelInputData.TabIndex = 4;
            this.labelInputData.Text = "Input Data:";
            // 
            // tbUrl
            // 
            this.tbUrl.Location = new System.Drawing.Point(125, 22);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(606, 21);
            this.tbUrl.TabIndex = 1;
            this.tbUrl.Text = "http://localhost:63492/MESAPI/LoadTest?msg=1234";
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(54, 25);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(77, 12);
            this.labelUrl.TabIndex = 0;
            this.labelUrl.Text = "Web Api Url:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(448, 469);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ApiTest_2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 747);
            this.Controls.Add(this.tabControl1);
            this.Name = "ApiTest_2";
            this.Text = "ApiTest_2";
            this.Load += new System.EventHandler(this.ApiTest_2_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabMESServic.ResumeLayout(false);
            this.tabMESServic.PerformLayout();
            this.tabWebAPI.ResumeLayout(false);
            this.tabWebAPI.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMESServic;
        private System.Windows.Forms.TabPage tabWebAPI;
        private System.Windows.Forms.Label labelMESServic;
        private System.Windows.Forms.TextBox tbMESServic;
        private System.Windows.Forms.TextBox tbPwd;
        private System.Windows.Forms.TextBox tbEmp;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label labelBU;
        private System.Windows.Forms.Label labelPwd;
        private System.Windows.Forms.Label labelEmp;
        private System.Windows.Forms.Label labelApiInput;
        private System.Windows.Forms.TextBox tbApiInput;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox tbApiOutput;
        private System.Windows.Forms.Label labelApiOutput;
        private System.Windows.Forms.ComboBox cbbBU;
        private System.Windows.Forms.ComboBox cbbFunction;
        private System.Windows.Forms.Label labelFunctionName;
        private System.Windows.Forms.Label labelLoginMsg;
        private System.Windows.Forms.TextBox tbLoginMsg;
        private System.Windows.Forms.TextBox tbOutputData;
        private System.Windows.Forms.Label labelOutputData;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.TextBox tbInputData;
        private System.Windows.Forms.Label labelInputData;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}