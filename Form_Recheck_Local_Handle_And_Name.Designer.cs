namespace Tool_PieHotKey
{
    partial class Form_Recheck_Local_Handle_And_Name
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
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LV_SimilarHandleName = new System.Windows.Forms.ListView();
            this.LV_OriginalHandleName = new System.Windows.Forms.ListView();
            this.Btn_Completed = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(25, 230);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 21);
            this.label4.TabIndex = 11;
            this.label4.Text = "附註：將會以原視窗名儲存\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(281, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 21);
            this.label3.TabIndex = 10;
            this.label3.Text = "最接近之視窗名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(25, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 21);
            this.label2.TabIndex = 9;
            this.label2.Text = "原視窗名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(25, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 21);
            this.label1.TabIndex = 8;
            this.label1.Text = "列表為未找到對應視窗的視窗名字";
            // 
            // LV_SimilarHandleName
            // 
            this.LV_SimilarHandleName.HideSelection = false;
            this.LV_SimilarHandleName.Location = new System.Drawing.Point(281, 91);
            this.LV_SimilarHandleName.Name = "LV_SimilarHandleName";
            this.LV_SimilarHandleName.Size = new System.Drawing.Size(250, 136);
            this.LV_SimilarHandleName.TabIndex = 7;
            this.LV_SimilarHandleName.UseCompatibleStateImageBehavior = false;
            // 
            // LV_OriginalHandleName
            // 
            this.LV_OriginalHandleName.HideSelection = false;
            this.LV_OriginalHandleName.Location = new System.Drawing.Point(25, 91);
            this.LV_OriginalHandleName.Name = "LV_OriginalHandleName";
            this.LV_OriginalHandleName.Size = new System.Drawing.Size(250, 136);
            this.LV_OriginalHandleName.TabIndex = 6;
            this.LV_OriginalHandleName.UseCompatibleStateImageBehavior = false;
            this.LV_OriginalHandleName.Click += new System.EventHandler(this.LV_OriginalHandleName_Click);
            // 
            // Btn_Completed
            // 
            this.Btn_Completed.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Btn_Completed.Location = new System.Drawing.Point(441, 248);
            this.Btn_Completed.Name = "Btn_Completed";
            this.Btn_Completed.Size = new System.Drawing.Size(90, 32);
            this.Btn_Completed.TabIndex = 12;
            this.Btn_Completed.Text = "完成";
            this.Btn_Completed.UseVisualStyleBackColor = true;
            this.Btn_Completed.Click += new System.EventHandler(this.Btn_Completed_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button1.Location = new System.Drawing.Point(488, 195);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 32);
            this.button1.TabIndex = 13;
            this.button1.Text = "選擇";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form_Recheck_Local_Handle_And_Name
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 292);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Btn_Completed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LV_SimilarHandleName);
            this.Controls.Add(this.LV_OriginalHandleName);
            this.Name = "Form_Recheck_Local_Handle_And_Name";
            this.Text = "Form_Recheck_Local_Handle_And_Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView LV_SimilarHandleName;
        private System.Windows.Forms.ListView LV_OriginalHandleName;
        private System.Windows.Forms.Button Btn_Completed;
        private System.Windows.Forms.Button button1;
    }
}