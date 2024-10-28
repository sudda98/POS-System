using System.Drawing; 
using System.Windows.Forms;

namespace Ella_Rose_Assignment
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Label lblFromDate;
        private System.Windows.Forms.Label lblToDate;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.DataGridView dataGridViewReports;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.lblFromDate = new System.Windows.Forms.Label();
            this.lblToDate = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.dataGridViewReports = new System.Windows.Forms.DataGridView();

            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(850, 680);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.BackColor = Color.WhiteSmoke;
            this.Name = "Form1";
            this.Text = "Report Viewer";

            this.reportViewer.LocalReport.ReportEmbeddedResource = "Ella_Rose_Assignment.Report1.rdlc";
            this.reportViewer.Location = new System.Drawing.Point(20, 15);
            this.reportViewer.Size = new System.Drawing.Size(800, 320);
            this.reportViewer.TabIndex = 0;

            this.dtpFromDate.Location = new System.Drawing.Point(20, 355);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(200, 22);
            this.dtpFromDate.TabIndex = 1;

            this.dtpToDate.Location = new System.Drawing.Point(250, 355);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(200, 22);
            this.dtpToDate.TabIndex = 2;

            SetButtonStyle(btnFilter, "Filter", new Point(470, 350), Color.CornflowerBlue);
            SetButtonStyle(btnExport, "Export", new Point(570, 350), Color.MediumSeaGreen);
            SetButtonStyle(btnPreview, "Preview", new Point(670, 350), Color.Coral);

            this.btnFilter.Click += new System.EventHandler(this.BtnFilter_Click);
            this.btnExport.Click += new System.EventHandler(this.BtnExport_Click);
            this.btnPreview.Click += new System.EventHandler(this.BtnPreview_Click);

            SetLabelStyle(lblFromDate, "From Date:", new Point(20, 335));
            SetLabelStyle(lblToDate, "To Date:", new Point(250, 335));
            SetLabelStyle(lblTotalAmount, "Total Amount:", new Point(20, 300));

            this.dataGridViewReports.Location = new System.Drawing.Point(20, 30);
            this.dataGridViewReports.Size = new System.Drawing.Size(800, 260);
            this.dataGridViewReports.BorderStyle = BorderStyle.FixedSingle;
            this.dataGridViewReports.BackgroundColor = Color.White;
            this.dataGridViewReports.TabIndex = 9;

            this.Controls.Add(this.reportViewer);
            this.Controls.Add(this.dtpFromDate);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.lblFromDate);
            this.Controls.Add(this.lblToDate);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.dataGridViewReports);

            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private void SetButtonStyle(Button button, string text, Point location, Color color)
        {
            button.Text = text;
            button.Location = location;
            button.Size = new System.Drawing.Size(90, 30);
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            button.TabIndex = 3;
        }

        private void SetLabelStyle(Label label, string text, Point location)
        {
            label.Text = text;
            label.Location = location;
            label.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            label.ForeColor = Color.Black;
            label.AutoSize = true;
        }
    }
}
