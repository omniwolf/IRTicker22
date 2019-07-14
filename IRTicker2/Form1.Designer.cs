namespace IRTicker2 {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.spread_dyn_label = new System.Windows.Forms.Label();
            this.spread_stat_label = new System.Windows.Forms.Label();
            this.best_offer_stat_label = new System.Windows.Forms.Label();
            this.best_bid_stat_label = new System.Windows.Forms.Label();
            this.best_bid_dyn_label = new System.Windows.Forms.Label();
            this.best_offer_dyn_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // spread_dyn_label
            // 
            this.spread_dyn_label.AutoSize = true;
            this.spread_dyn_label.Location = new System.Drawing.Point(270, 48);
            this.spread_dyn_label.Name = "spread_dyn_label";
            this.spread_dyn_label.Size = new System.Drawing.Size(0, 13);
            this.spread_dyn_label.TabIndex = 0;
            // 
            // spread_stat_label
            // 
            this.spread_stat_label.AutoSize = true;
            this.spread_stat_label.Location = new System.Drawing.Point(166, 48);
            this.spread_stat_label.Name = "spread_stat_label";
            this.spread_stat_label.Size = new System.Drawing.Size(44, 13);
            this.spread_stat_label.TabIndex = 1;
            this.spread_stat_label.Text = "Spread:";
            // 
            // best_offer_stat_label
            // 
            this.best_offer_stat_label.AutoSize = true;
            this.best_offer_stat_label.Location = new System.Drawing.Point(37, 162);
            this.best_offer_stat_label.Name = "best_offer_stat_label";
            this.best_offer_stat_label.Size = new System.Drawing.Size(55, 13);
            this.best_offer_stat_label.TabIndex = 2;
            this.best_offer_stat_label.Text = "Best offer:";
            // 
            // best_bid_stat_label
            // 
            this.best_bid_stat_label.AutoSize = true;
            this.best_bid_stat_label.Location = new System.Drawing.Point(37, 119);
            this.best_bid_stat_label.Name = "best_bid_stat_label";
            this.best_bid_stat_label.Size = new System.Drawing.Size(48, 13);
            this.best_bid_stat_label.TabIndex = 3;
            this.best_bid_stat_label.Text = "Best bid:";
            // 
            // best_bid_dyn_label
            // 
            this.best_bid_dyn_label.AutoSize = true;
            this.best_bid_dyn_label.Location = new System.Drawing.Point(117, 119);
            this.best_bid_dyn_label.Name = "best_bid_dyn_label";
            this.best_bid_dyn_label.Size = new System.Drawing.Size(13, 13);
            this.best_bid_dyn_label.TabIndex = 4;
            this.best_bid_dyn_label.Text = "0";
            // 
            // best_offer_dyn_label
            // 
            this.best_offer_dyn_label.AutoSize = true;
            this.best_offer_dyn_label.Location = new System.Drawing.Point(117, 162);
            this.best_offer_dyn_label.Name = "best_offer_dyn_label";
            this.best_offer_dyn_label.Size = new System.Drawing.Size(13, 13);
            this.best_offer_dyn_label.TabIndex = 5;
            this.best_offer_dyn_label.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.best_offer_dyn_label);
            this.Controls.Add(this.best_bid_dyn_label);
            this.Controls.Add(this.best_bid_stat_label);
            this.Controls.Add(this.best_offer_stat_label);
            this.Controls.Add(this.spread_stat_label);
            this.Controls.Add(this.spread_dyn_label);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label spread_dyn_label;
        private System.Windows.Forms.Label spread_stat_label;
        private System.Windows.Forms.Label best_offer_stat_label;
        private System.Windows.Forms.Label best_bid_stat_label;
        private System.Windows.Forms.Label best_bid_dyn_label;
        private System.Windows.Forms.Label best_offer_dyn_label;
    }
}

