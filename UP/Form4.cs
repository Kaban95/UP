using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UP
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        
        void code()
        {
            string text = "https://t.me/toysmag1309_bot";
            QRCodeEncoder code = new QRCodeEncoder();
            Bitmap code1 = code.Encode(text);
            pictureBox1.Image = code1 as Image;
            
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            code();
            pictureBox1 .Visible = true;
        }
        

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form8 = new Form1();
            form8.Show();
            this.Hide();
            MessageBox.Show("Вы успешно вернулись назад!", "Назад", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
        
    }
}
