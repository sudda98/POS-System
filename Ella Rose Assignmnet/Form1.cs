using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;

namespace Ella_Rose_Assignment
{
    public partial class Form1 : Form
    {
        private DataTable reportData;
        private string connectionString;
        private PrintDocument printDocument;
        private int rowIndex;

        public Form1()
        {
            InitializeComponent();
            reportData = new DataTable();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ReportDB"].ConnectionString;
            LoadReportData();

            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
        }

        private void LoadReportData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT DateTime, Type, Employee, Staff, TillNo, Amount FROM VoidReports", connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(reportData);
                    }
                }

                dgvVoidReport.DataSource = reportData;
                UpdateTotalAmount(reportData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date.AddDays(1).AddTicks(-1);

            if (fromDate > toDate)
            {
                MessageBox.Show("The 'From' date cannot be later than the 'To' date.", "Date Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataView filteredView = new DataView(reportData)
            {
                RowFilter = $"DateTime >= #{fromDate}# AND DateTime <= #{toDate}#"
            };

            dgvVoidReport.DataSource = filteredView;
            UpdateTotalAmount(filteredView.ToTable());
        }

        private void UpdateTotalAmount(DataTable data)
        {
            decimal total = 0;
            foreach (DataRow row in data.Rows)
            {
                total += Convert.ToDecimal(row["Amount"]);
            }
            lblTotalAmount.Text = $"Total Amount: {total:C}";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Save an Export File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder csvContent = new StringBuilder();
                    foreach (DataColumn column in reportData.Columns)
                    {
                        csvContent.Append(column.ColumnName + ",");
                    }
                    csvContent.Length--;
                    csvContent.AppendLine();

                    foreach (DataRow row in reportData.Rows)
                    {
                        for (int i = 0; i < reportData.Columns.Count; i++)
                        {
                            csvContent.Append(row[i].ToString() + ",");
                        }
                        csvContent.Length--;
                        csvContent.AppendLine();
                    }

                    File.WriteAllText(saveFileDialog.FileName, csvContent.ToString());
                    MessageBox.Show("Data exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog
            {
                Document = printDocument
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    printDocument.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error printing document: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font font = new Font("Arial", 10);
            float lineHeight = font.GetHeight(e.Graphics) + 4;

            e.Graphics.DrawString("Void Report", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top);

            int columnCount = dgvVoidReport.Columns.Count;
            for (int i = 0; i < columnCount; i++)
            {
                e.Graphics.DrawString(dgvVoidReport.Columns[i].HeaderText, font, Brushes.Black, e.MarginBounds.Left + (i * 100), e.MarginBounds.Top + lineHeight);
            }

            rowIndex = 0;
            while (rowIndex < dgvVoidReport.Rows.Count)
            {
                DataGridViewRow row = dgvVoidReport.Rows[rowIndex];
                for (int i = 0; i < columnCount; i++)
                {
                    e.Graphics.DrawString(row.Cells[i].Value?.ToString(), font, Brushes.Black, e.MarginBounds.Left + (i * 100), e.MarginBounds.Top + (lineHeight * (rowIndex + 2)));
                }
                rowIndex++;

                if (e.MarginBounds.Top + (lineHeight * (rowIndex + 2)) > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }
        }
    }
}
