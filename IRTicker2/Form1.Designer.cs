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
            this.UITimer = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.timerSleep = new System.Windows.Forms.MaskedTextBox();
            this.orders_processed_stat_label = new System.Windows.Forms.Label();
            this.orders_processed_dyn_label = new System.Windows.Forms.Label();
            this.WSClient_thread = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // spread_dyn_label
            // 
            this.spread_dyn_label.AutoSize = true;
            this.spread_dyn_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spread_dyn_label.Location = new System.Drawing.Point(112, 12);
            this.spread_dyn_label.Name = "spread_dyn_label";
            this.spread_dyn_label.Size = new System.Drawing.Size(0, 25);
            this.spread_dyn_label.TabIndex = 0;
            // 
            // spread_stat_label
            // 
            this.spread_stat_label.AutoSize = true;
            this.spread_stat_label.Location = new System.Drawing.Point(40, 18);
            this.spread_stat_label.Name = "spread_stat_label";
            this.spread_stat_label.Size = new System.Drawing.Size(44, 13);
            this.spread_stat_label.TabIndex = 1;
            this.spread_stat_label.Text = "Spread:";
            // 
            // best_offer_stat_label
            // 
            this.best_offer_stat_label.AutoSize = true;
            this.best_offer_stat_label.Location = new System.Drawing.Point(144, 50);
            this.best_offer_stat_label.Name = "best_offer_stat_label";
            this.best_offer_stat_label.Size = new System.Drawing.Size(55, 13);
            this.best_offer_stat_label.TabIndex = 2;
            this.best_offer_stat_label.Text = "Best offer:";
            // 
            // best_bid_stat_label
            // 
            this.best_bid_stat_label.AutoSize = true;
            this.best_bid_stat_label.Location = new System.Drawing.Point(11, 50);
            this.best_bid_stat_label.Name = "best_bid_stat_label";
            this.best_bid_stat_label.Size = new System.Drawing.Size(48, 13);
            this.best_bid_stat_label.TabIndex = 3;
            this.best_bid_stat_label.Text = "Best bid:";
            // 
            // best_bid_dyn_label
            // 
            this.best_bid_dyn_label.AutoSize = true;
            this.best_bid_dyn_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.best_bid_dyn_label.Location = new System.Drawing.Point(6, 82);
            this.best_bid_dyn_label.Name = "best_bid_dyn_label";
            this.best_bid_dyn_label.Size = new System.Drawing.Size(24, 25);
            this.best_bid_dyn_label.TabIndex = 4;
            this.best_bid_dyn_label.Text = "0";
            // 
            // best_offer_dyn_label
            // 
            this.best_offer_dyn_label.AutoSize = true;
            this.best_offer_dyn_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.best_offer_dyn_label.Location = new System.Drawing.Point(136, 82);
            this.best_offer_dyn_label.Name = "best_offer_dyn_label";
            this.best_offer_dyn_label.Size = new System.Drawing.Size(24, 25);
            this.best_offer_dyn_label.TabIndex = 5;
            this.best_offer_dyn_label.Text = "0";
            // 
            // UITimer
            // 
            this.UITimer.WorkerReportsProgress = true;
            this.UITimer.WorkerSupportsCancellation = true;
            this.UITimer.DoWork += new System.ComponentModel.DoWorkEventHandler(this.UITimer_DoWork);
            this.UITimer.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.UITimer_ProgressChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 165);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "set new timer";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timerSleep
            // 
            this.timerSleep.Location = new System.Drawing.Point(12, 120);
            this.timerSleep.Mask = "0000000000";
            this.timerSleep.Name = "timerSleep";
            this.timerSleep.Size = new System.Drawing.Size(100, 20);
            this.timerSleep.TabIndex = 8;
            this.timerSleep.Text = "500";
            this.timerSleep.ValidatingType = typeof(int);
            // 
            // orders_processed_stat_label
            // 
            this.orders_processed_stat_label.AutoSize = true;
            this.orders_processed_stat_label.Location = new System.Drawing.Point(121, 123);
            this.orders_processed_stat_label.Name = "orders_processed_stat_label";
            this.orders_processed_stat_label.Size = new System.Drawing.Size(140, 13);
            this.orders_processed_stat_label.TabIndex = 9;
            this.orders_processed_stat_label.Text = "Orders processed this cycle:";
            // 
            // orders_processed_dyn_label
            // 
            this.orders_processed_dyn_label.AutoSize = true;
            this.orders_processed_dyn_label.Location = new System.Drawing.Point(179, 154);
            this.orders_processed_dyn_label.Name = "orders_processed_dyn_label";
            this.orders_processed_dyn_label.Size = new System.Drawing.Size(13, 13);
            this.orders_processed_dyn_label.TabIndex = 10;
            this.orders_processed_dyn_label.Text = "0";
            // 
            // WSClient_thread
            // 
            this.WSClient_thread.WorkerReportsProgress = true;
            this.WSClient_thread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WSClient_thread_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 201);
            this.Controls.Add(this.orders_processed_dyn_label);
            this.Controls.Add(this.orders_processed_stat_label);
            this.Controls.Add(this.timerSleep);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.best_offer_dyn_label);
            this.Controls.Add(this.best_bid_dyn_label);
            this.Controls.Add(this.best_bid_stat_label);
            this.Controls.Add(this.best_offer_stat_label);
            this.Controls.Add(this.spread_stat_label);
            this.Controls.Add(this.spread_dyn_label);
            this.MaximumSize = new System.Drawing.Size(300, 240);
            this.MinimumSize = new System.Drawing.Size(300, 240);
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
        private System.ComponentModel.BackgroundWorker UITimer;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MaskedTextBox timerSleep;
        private System.Windows.Forms.Label orders_processed_stat_label;
        private System.Windows.Forms.Label orders_processed_dyn_label;
        private System.ComponentModel.BackgroundWorker WSClient_thread;
    }
}

