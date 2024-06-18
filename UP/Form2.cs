﻿using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace UP
{

    enum RowState
    {
        Existed,
        New,
        Modified,
        Deleted,
        ModifiedNew
    }
    public partial class Form2 : Form
    {
        Database database = new Database();
        int selectedRow;

        public Form2()
        {
           
            InitializeComponent();
        }
        private void CreateColumns()
        {
           dataGridView1.Columns.Add("ProductId","ProductId");
           dataGridView1.Columns.Add("name_book", "Название игрушки");
           dataGridView1.Columns.Add("price", "Цена");
           dataGridView1.Columns.Add("discount", "Материал");
           dataGridView1.Columns.Add("opisanie", "Описание");
           dataGridView1.Columns.Add("proizvoditel", "Производитель");
           dataGridView1.Columns.Add("IsNew",string.Empty);  
        }

        private void ReadSingleRow(DataGridView dgw,IDataRecord record) 
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt32(2), record.GetString(3), record.GetString(4), record.GetString(5), RowState.ModifiedNew);
        }
        private void RefreshDataGried(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select ProductId,name_book as 'Название',price as 'Цена',discount as 'Скидка',opisanie as 'Описание',proizvoditel as 'Производитель' from books";
            SqlCommand command = new SqlCommand(queryString,database.GetConnection());

            database.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) 
            {
                ReadSingleRow(dgw,reader);
            }
            reader.Close();
        }
        
        private void Form2_Load(object sender, EventArgs e)
        {


            CreateColumns();
            RefreshDataGried(dataGridView1);
            dataGridView1.Columns["IsNew"].Visible = false;
            dataGridView1.Columns["ProductId"].Visible = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           selectedRow = e.RowIndex;
            if (e.RowIndex>=0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];
                textBox1.Text = row.Cells[0].Value.ToString();
                textBox_name_book.Text = row.Cells[1].Value.ToString();
                textBox_Price.Text = row.Cells[2].Value.ToString();
                textBox_Discount.Text = row.Cells[3].Value.ToString();
                textBox_opisanie.Text = row.Cells[4].Value.ToString();
                textBox_proizvoditel.Text = row.Cells[5].Value.ToString();  
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataGried(dataGridView1);
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            addform addfrm = new addform();
            addfrm.Show();
        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string searchstring = $"select ProductId,name_book,price,discount,opisanie,proizvoditel from books where concat (ProductId,name_book,price,discount,opisanie,proizvoditel) like '%"+textBox_search.Text+"%'";
            SqlCommand com = new SqlCommand(searchstring,database.GetConnection());
            database.OpenConnection();
            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }

        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;
            string productId = dataGridView1.Rows[index].Cells[0].Value.ToString();

            // Если значение ProductId пустое, значит, это новая строка, которую нужно удалить только из таблицы DataGridView
            if (string.IsNullOrEmpty(productId))
            {
                dataGridView1.Rows[index].Visible = false;
                dataGridView1.Rows[index].Cells[5].Value = RowState.Deleted;
                return;
            }

            // Иначе, удалить строку из базы данных
            string queryString = $"DELETE FROM books WHERE ProductId = {productId}";
            SqlCommand command = new SqlCommand(queryString, database.GetConnection());

            try
            {
                database.OpenConnection();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    dataGridView1.Rows.RemoveAt(index);
                    MessageBox.Show("Строка успешно удалена.");
                }
                else
                {
                    MessageBox.Show("Не удалось удалить строку.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления строки: {ex.Message}");
            }
            finally
            {
                database.CloseConnection();
            }
        }


        private void Update()
        {
            database.OpenConnection();
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[6].Value;
                if (rowState== RowState.Existed)
                {
                    continue;
                }
                if (rowState==RowState.Deleted)
                {
                    var ProductId = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deletequery = $"delete from books where ProductId = {ProductId}";
                    var command = new SqlCommand (deletequery,database.GetConnection());
                    command.ExecuteNonQuery ();

                }

                if (rowState == RowState.Modified)
                {
                    var ProductId = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var name_book = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var price = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var discount = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var opisanie = dataGridView1.Rows[index].Cells[4].Value.ToString();
                    var proizvoditel = dataGridView1.Rows[index].Cells[5].Value.ToString();

                    var changeQuery = $"update books set name_book='{name_book}',price = '{price}',discount = '{discount}',opisanie = '{opisanie}',proizvoditel = '{proizvoditel}'where ProductId = {ProductId}";
                    var command = new SqlCommand(changeQuery,database.GetConnection());
                    command.ExecuteNonQuery();

                }
            }
           
            database.CloseConnection();
        }

        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Update();
            MessageBox.Show("Успешно сохранено, можете обновить таблицу", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void Change()
        {
            var selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            var ProductId = textBox1.Text;
            var name = textBox_name_book.Text;
            int price;
            var discount = textBox_Discount.Text;
            var opisanie = textBox_opisanie.Text;
            var proizvoditel = textBox_proizvoditel.Text;
           
            if (dataGridView1.Rows[selectedRowIndex].Cells[0].Value.ToString()!=string.Empty)
            {
                if (int.TryParse(textBox_Price.Text, out price))
                {
                    dataGridView1.Rows[selectedRowIndex].SetValues(ProductId,name,price, discount, opisanie, proizvoditel);
                    dataGridView1.Rows[selectedRowIndex].Cells[6].Value = RowState.Modified;
                }
                else
                    MessageBox.Show("Цена должна иметь числовой формат");

            }
        }
        private void button_edit_Click(object sender, EventArgs e)
        {
            Change();
            MessageBox.Show("Изменено, нажмите кнопку сохранить", "Изменение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form9 = new Form1();
            form9.Show();
            this.Hide();
            MessageBox.Show("Вы успешно вернулись назад!", "Назад", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                Excel.Application exApp = new Excel.Application();

                exApp.Workbooks.Add();
                Excel.Worksheet wsh = (Excel.Worksheet)exApp.ActiveSheet;
                int i, j;
                for (i = 0; i <= dataGridView1.RowCount - 2; i++)
                {
                    for (j = 0; j <= dataGridView1.ColumnCount - 1; j++)
                    {
                        wsh.Cells[i + 1, j + 1] = dataGridView1[j, i].Value.ToString();
                    }
                }
                exApp.Visible = true;
                MessageBox.Show("Отчет в Excel создан!", "Отчет успешен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
