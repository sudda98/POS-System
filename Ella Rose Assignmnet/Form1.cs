using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.IO;

namespace Ella_Rose_Assignment
{
    public partial class Form1 : Form
    {
        private DataTable reportData = new DataTable();
        private string connectionString;

        public Form1()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["ReportDB"].ConnectionString;
            LoadReportData(); 
            reportViewer.Visible = false;
        }

        private void LoadReportData()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show("Connection string is not set.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT Id, DateTime, Type, Employee, Staff, TillNo, Amount FROM VoidReports", connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        reportData.Clear();
                        adapter.Fill(reportData);
                    }
                }

                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data found in the report.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                dataGridViewReports.DataSource = reportData;
                CustomizeDataGridView();
                UpdateTotalAmount(reportData);

                SetupReportViewer(); 
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL error loading report data: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeDataGridView()
        {
            if (dataGridViewReports == null) return;

            dataGridViewReports.AutoGenerateColumns = false;
            dataGridViewReports.Columns.Clear();

            dataGridViewReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 50 });
            dataGridViewReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date", DataPropertyName = "DateTime", Width = 100 });
            dataGridViewReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Type", DataPropertyName = "Type", Width = 100 });
            dataGridViewReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Employee", DataPropertyName = "Employee", Width = 100 });
            dataGridViewReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Staff", DataPropertyName = "Staff", Width = 100 });
            dataGridViewReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Till No", DataPropertyName = "TillNo", Width = 100 });

            var amountColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Amount",
                DataPropertyName = "Amount",
                Width = 100
            };

            dataGridViewReports.Columns.Add(amountColumn);

            if (dataGridViewReports.Columns["Amount"] != null)
            {
                dataGridViewReports.Columns["Amount"].DefaultCellStyle.Format = "C2";
            }
        }

        private void UpdateTotalAmount(DataTable dataTable)
        {
            decimal totalAmount = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                if (decimal.TryParse(row["Amount"].ToString(), out decimal amount))
                {
                    totalAmount += amount;
                }
            }
            lblTotalAmount.Text = $"Total Amount: ${totalAmount:F2}";
        }

        private void SetupReportViewer()
        {
            try
            {
                if (reportData == null || reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data available for the report.", "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.ReportEmbeddedResource = "Ella_Rose_Assignment.VoidReport.rdlc"; 
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Dataset1", reportData)); 
                reportViewer.RefreshReport(); 
                reportViewer.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up report viewer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            if (dtpFromDate.Value > dtpToDate.Value)
            {
                MessageBox.Show("From Date cannot be greater than To Date.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataView dv = reportData.DefaultView;

            string filterExpression = $"DateTime >= #{dtpFromDate.Value}# AND DateTime <= #{dtpToDate.Value}#";
            dv.RowFilter = filterExpression;

            dataGridViewReports.DataSource = dv;
            UpdateTotalAmount(dv.ToTable());
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (reportData.Rows.Count == 0)
                {
                    MessageBox.Show("No data available to export.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = "VoidReportsExport";
                    saveFileDialog.DefaultExt = "csv";
                    saveFileDialog.Filter = "CSV files (*.csv)|*.csv";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            for (int i = 0; i < reportData.Columns.Count; i++)
                            {
                                writer.Write(reportData.Columns[i]);
                                if (i < reportData.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();

                            foreach (DataRow row in reportData.Rows)
                            {
                                for (int i = 0; i < reportData.Columns.Count; i++)
                                {
                                    writer.Write(row[i].ToString());
                                    if (i < reportData.Columns.Count - 1)
                                        writer.Write(",");
                                }
                                writer.WriteLine();
                            }
                        }
                        MessageBox.Show("Export successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during export: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (reportData.Rows.Count == 0)
            {
                MessageBox.Show("No data available for preview.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            DataTable dataToPreview = reportData.Clone(); 

            foreach (DataRowView rowView in reportData.DefaultView)
            {
                dataToPreview.ImportRow(rowView.Row); 
            }

            if (dataToPreview.Rows.Count == 0)
            {
                MessageBox.Show("No data available for the report preview.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Form reportPreviewForm = new Form();
            reportPreviewForm.Text = "Report Preview";
            reportPreviewForm.Size = new System.Drawing.Size(800, 600);
            reportPreviewForm.StartPosition = FormStartPosition.CenterScreen;

            ReportViewer previewViewer = new ReportViewer();
            previewViewer.Dock = DockStyle.Fill;
            previewViewer.LocalReport.ReportEmbeddedResource = "Ella_Rose_Assignment.VoidReport.rdlc"; 

            previewViewer.LocalReport.DataSources.Clear();
            previewViewer.LocalReport.DataSources.Add(new ReportDataSource("Dataset1", dataToPreview)); 
            previewViewer.RefreshReport(); 
            reportPreviewForm.Controls.Add(previewViewer);
            reportPreviewForm.Show();
        }

    }
}
