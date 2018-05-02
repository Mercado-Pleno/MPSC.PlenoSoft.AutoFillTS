namespace MPSC.AutoFillTS.View
{
	partial class TSForm
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
			this.txtHorarios = new System.Windows.Forms.TextBox();
			this.btProcessar = new System.Windows.Forms.Button();
			this.dgvHorarios = new System.Windows.Forms.DataGridView();
			this.ckAutoSaveClick = new System.Windows.Forms.CheckBox();
			this.ckPreencherMongeral = new System.Windows.Forms.CheckBox();
			this.ckPreencherProvider = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dgvHorarios)).BeginInit();
			this.SuspendLayout();
			// 
			// txtHorarios
			// 
			this.txtHorarios.AcceptsReturn = true;
			this.txtHorarios.AcceptsTab = true;
			this.txtHorarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtHorarios.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtHorarios.Location = new System.Drawing.Point(0, 0);
			this.txtHorarios.Multiline = true;
			this.txtHorarios.Name = "txtHorarios";
			this.txtHorarios.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtHorarios.Size = new System.Drawing.Size(792, 219);
			this.txtHorarios.TabIndex = 0;
			this.txtHorarios.WordWrap = false;
			this.txtHorarios.TextChanged += new System.EventHandler(this.Analisar);
			this.txtHorarios.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Observar);
			// 
			// btProcessar
			// 
			this.btProcessar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btProcessar.Location = new System.Drawing.Point(717, 551);
			this.btProcessar.Name = "btProcessar";
			this.btProcessar.Size = new System.Drawing.Size(75, 23);
			this.btProcessar.TabIndex = 1;
			this.btProcessar.Text = "Processar";
			this.btProcessar.UseVisualStyleBackColor = true;
			this.btProcessar.Click += new System.EventHandler(this.btProcessar_Click);
			// 
			// dgvHorarios
			// 
			this.dgvHorarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvHorarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvHorarios.Location = new System.Drawing.Point(0, 225);
			this.dgvHorarios.Name = "dgvHorarios";
			this.dgvHorarios.Size = new System.Drawing.Size(792, 323);
			this.dgvHorarios.TabIndex = 2;
			// 
			// ckAutoSaveClick
			// 
			this.ckAutoSaveClick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ckAutoSaveClick.AutoSize = true;
			this.ckAutoSaveClick.Checked = true;
			this.ckAutoSaveClick.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckAutoSaveClick.Location = new System.Drawing.Point(598, 555);
			this.ckAutoSaveClick.Name = "ckAutoSaveClick";
			this.ckAutoSaveClick.Size = new System.Drawing.Size(113, 17);
			this.ckAutoSaveClick.TabIndex = 3;
			this.ckAutoSaveClick.Text = "Auto Click (Salvar)";
			this.ckAutoSaveClick.UseVisualStyleBackColor = true;
			// 
			// ckPreencherMongeral
			// 
			this.ckPreencherMongeral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ckPreencherMongeral.AutoSize = true;
			this.ckPreencherMongeral.Checked = true;
			this.ckPreencherMongeral.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ckPreencherMongeral.Location = new System.Drawing.Point(470, 555);
			this.ckPreencherMongeral.Name = "ckPreencherMongeral";
			this.ckPreencherMongeral.Size = new System.Drawing.Size(122, 17);
			this.ckPreencherMongeral.TabIndex = 4;
			this.ckPreencherMongeral.Text = "Preencher Mongeral";
			this.ckPreencherMongeral.UseVisualStyleBackColor = true;
			// 
			// ckPreencherProvider
			// 
			this.ckPreencherProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ckPreencherProvider.AutoSize = true;
			this.ckPreencherProvider.Location = new System.Drawing.Point(347, 555);
			this.ckPreencherProvider.Name = "ckPreencherProvider";
			this.ckPreencherProvider.Size = new System.Drawing.Size(117, 17);
			this.ckPreencherProvider.TabIndex = 6;
			this.ckPreencherProvider.Text = "Preencher Provider";
			this.ckPreencherProvider.UseVisualStyleBackColor = true;
			// 
			// TSForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 573);
			this.Controls.Add(this.ckPreencherProvider);
			this.Controls.Add(this.ckPreencherMongeral);
			this.Controls.Add(this.ckAutoSaveClick);
			this.Controls.Add(this.dgvHorarios);
			this.Controls.Add(this.btProcessar);
			this.Controls.Add(this.txtHorarios);
			this.Name = "TSForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Auto-Fill Time Sheet";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			((System.ComponentModel.ISupportInitialize)(this.dgvHorarios)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtHorarios;
		private System.Windows.Forms.Button btProcessar;
		private System.Windows.Forms.DataGridView dgvHorarios;
		private System.Windows.Forms.CheckBox ckAutoSaveClick;
		private System.Windows.Forms.CheckBox ckPreencherMongeral;
		private System.Windows.Forms.CheckBox ckPreencherProvider;
	}
}