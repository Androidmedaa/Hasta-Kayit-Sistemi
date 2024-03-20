using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Hasta_Kayıt_Sistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e){}


        private void VeritabaniKayitlariniGoruntule()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // Veritabanından kayıtları çek
                    string sql = "SELECT * FROM TableName";
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // DataGridView'e verileri bağla
                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message);
                }
            }
        }

        #region veri tabanımızı programa ekiyoruz

        string connString = "Host=localhost;Username=postgres;Password=1234;Database=\"Hasta Kayıt Sistemi\"";

        #endregion


        #region Gereksiz tıklamalar
        private void goruntuleBox_TextChanged(object sender, EventArgs e){}
        private void label11_Click(object sender, EventArgs e){}
        private void gecmisTextBox_TextChanged(object sender, EventArgs e){}
        private void tabPage3_Click(object sender, EventArgs e){}
        private void tabPage1_Click(object sender, EventArgs e){}
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e){}
        private void dateTime_ValueChanged(object sender, EventArgs e){}
        private void tabPage5_Click(object sender, EventArgs e){}
        #endregion



        #region İleri ve geri gitme ve temizleme butonları

        private void button1_Click(object sender, EventArgs e)
        {
            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex + 1; // Bir sonraki sayfaya geç
        }
        private void button7_Click(object sender, EventArgs e)
        {

            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex - 1;
        }

        private void button9_Click(object sender, EventArgs e)
        {

            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex - 1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex + 1; // Bir sonraki sayfaya geç
        }
        private void cleanbutton_Click(object sender, EventArgs e)
                {
                    // TextBox'ların içeriğini temizle
                    adSoyadBox.Text = "";
                    tcBox.Text = "";
                    tlfBox.Text = "";
                    dateTime.Value = DateTime.Now; // veya başka bir başlangıç değeri
                    comboBox1.SelectedIndex = -1; // veya başka bir başlangıç durumu
                    adresBox.Text = "";
                    hastalıkBox.Text = "";
                    ilacBox.Text = "";
                }
        private void button3_Click(object sender, EventArgs e)
        {
            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex + 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex + 1;

        }
        #endregion

        
        


        #region Yeni Hasta ekleme buttonu
        // TextBox'ların içeriğini temizliyorum
        private void yeniHastaEklebutton_Click(object sender, EventArgs e)
        {
            adSoyadBox.Text = "";
            tcBox.Text = "";
            tlfBox.Text = "";
            dateTime.Value = DateTime.Now; // veya başka bir başlangıç değeri
            comboBox1.SelectedIndex = -1; // veya başka bir başlangıç durumu
            adresBox.Text = "";
            hastalıkBox.Text = "";
            ilacBox.Text = "";
        }
        #endregion



        #region Kaydet buttonu her tıkladıgımızda kullanıcı verileri listine ekleme yapıyor aynı zamanda tcKimlik numarası herkeste sadece bir tane oldugu için hem veri tabanında hem geçiş tabpagemi kontrol ediyoruz 
        private List<string> kullaniciVerileri = new List<string>();

        private int i = 0;
        private void kaydetbutton_Click(object sender, EventArgs e)
        {
            string tcKimlik = tcBox.Text;

            // Veritabanında aynı TC Kimlik numarasına sahip hasta var mı kontrol et hem datafridview kısmı için hem de veritabanımız için kontrol edicez
            bool tcVarMi = dataGridView1.Rows.Cast<DataGridViewRow>().Any(row => row.Cells[1].Value != null && row.Cells[1].Value.ToString() == tcKimlik);

            // Eğer aynı TC Kimlik numarasına sahip hasta varsa
            if (tcVarMi)
            {
                MessageBox.Show("Bu TC Kimlik numarasına sahip hasta zaten var. Güncellemek için Güncelleme ekranına gidin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // Veritabanında aynı TC Kimlik numarasına sahip hasta var mı kontrol et
            if (HastaVarMi(tcKimlik))
            {
                MessageBox.Show("Bu TC Kimlik numarasına sahip hasta zaten var. Güncellemek için Güncelleme ekranına gidin.");
                return;
            }

            // Gerekli kontroller
            if (string.IsNullOrWhiteSpace(adSoyadBox.Text) ||
                string.IsNullOrWhiteSpace(tcBox.Text) ||
                string.IsNullOrWhiteSpace(tlfBox.Text) ||
                string.IsNullOrWhiteSpace(dateTime.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(adresBox.Text) ||
                string.IsNullOrWhiteSpace(hastalıkBox.Text) ||
                string.IsNullOrWhiteSpace(ilacBox.Text))
            {
                // Eksik alanları doldurunuz uyarısı
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
                return; // Fonksiyonu burada sonlandır
            }

            // Yeni kullanıcı verilerini birleştir
            string yeniVeri = $"Adı Soyadı: {adSoyadBox.Text}{Environment.NewLine}" +
                              $"TC Kimlik: {tcBox.Text}{Environment.NewLine}" +
                              $"Cep tlf: {tlfBox.Text}{Environment.NewLine}" +
                              $"Doğum Tarihi: {dateTime.Value.ToString()}{Environment.NewLine}" +
                              $"Doğum Yeri: {comboBox1.Text}{Environment.NewLine}" +
                              $"Adres: {adresBox.Text}{Environment.NewLine}" +
                              $"Hastalık: {hastalıkBox.Text}{Environment.NewLine}" +
                              $"İlaç: {ilacBox.Text}{Environment.NewLine}";

            // Yeni veriyi List'e ekle
            kullaniciVerileri.Add(yeniVeri);

            // List'teki tüm verileri birleştir
            string gecmisVeri = string.Join(Environment.NewLine, kullaniciVerileri);

            // gecmisTextBox'e yazdır
            gecmisTextBox.Text = gecmisVeri;

            // gecmisTextBox'i sadece okunabilir yap
            gecmisTextBox.ReadOnly = true;

            // datagridview kısmı
            int rowIndex = dataGridView1.Rows.Add();
            dataGridView1.Rows[rowIndex].Cells[0].Value = adSoyadBox.Text;
            dataGridView1.Rows[rowIndex].Cells[1].Value = tcBox.Text;
            dataGridView1.Rows[rowIndex].Cells[2].Value = tlfBox.Text;
            dataGridView1.Rows[rowIndex].Cells[3].Value = dateTime.Text;
            dataGridView1.Rows[rowIndex].Cells[4].Value = comboBox1.Text;
            dataGridView1.Rows[rowIndex].Cells[5].Value = adresBox.Text;
            dataGridView1.Rows[rowIndex].Cells[6].Value = hastalıkBox.Text;
            dataGridView1.Rows[rowIndex].Cells[7].Value = ilacBox.Text;
            i++;
        }
        // Veritabanında aynı TC Kimlik numarasına sahip hasta var mı kontrolü
        private bool HastaVarMi(string tcKimlik)
        {
            string connString = "Host=localhost;Username=postgres;Password=1234;Database=\"Hasta Kayıt Sistemi\"";
            string sql = $"SELECT COUNT(*) FROM public.\"HASTA\" WHERE \"tcKimlik\" = '{tcKimlik}'";

            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                int rowCount = Convert.ToInt32(command.ExecuteScalar());

                return rowCount > 0;
            }
        }

        #endregion




        #region Hasta Guncelleme tabpagemizdeki tc kimlik numarasına gore daha once yukarıda tanımladıgımız kullanıcıverileri listinden veri cekip bize gosteriyor duruma gore messageBox kulllanılıyor
        private void searchbutton_Click(object sender, EventArgs e)
        {
            // tcGirBox içindeki TC Kimlik Numarasını al
            string arananTc = tcGirBox.Text;

            // Aranan TC Kimlik Numarasını kullanarak hastayı bul
            string bulunanVeri = kullaniciVerileri.FirstOrDefault(veri => veri.Contains($"TC Kimlik: {arananTc}"));

            if (bulunanVeri != null)
            {
                // gecmisTextBox'i sadece okunabilir yap
                gecmisTextBox.ReadOnly = true;

                // Arama sonucunu goruntuleBox'a yazdır
                goruntuleBox.Text = "Arama Sonucu:\n" + bulunanVeri;
            }
            else
            {
                // Bulunamayan durumu için bir uyarı mesajı göster
                MessageBox.Show("Aranan TC Kimlik Numarasına ait hasta bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Arama sonucunu temizle
                goruntuleBox.Text = "";
            }
        }
        #endregion



        #region tcKimlikve dogum Tarihi dısındaki verileri bos bırakıyor ve bizi hasta kayıt ekranına yonelendiriyor bu sayede veri guncellemesi yapabiliyoruz
        private void updatebutton_Click(object sender, EventArgs e)
        {
            //bu buttona tıklanınca once mevcut hastanın tc ve dogum tarihi dışındaki verileri silsin ve yeni veri girişi yapsın
            //aynı zamanda Hasta Kayıt sayfasına yonlendirsin
            string silinecekTc = tcGirBox.Text;

            // TC Kimlik Numarasına göre hastanın verilerini bul
            string bulunanVeri = kullaniciVerileri.FirstOrDefault(veri => veri.Contains($"TC Kimlik: {silinecekTc}"));
            gecmisTextBox.Text = gecmisTextBox.Text.Replace(bulunanVeri, "");
            adSoyadBox.Text = "";
            tcBox.Text = silinecekTc;
            tlfBox.Text = "";
            dateTime.Text = dateTime.Text; // veya başka bir başlangıç değeri
            comboBox1.SelectedIndex = -1; // veya başka bir başlangıç durumu
            adresBox.Text = "";
            hastalıkBox.Text = "";
            ilacBox.Text = "";
            int currentIndex = tabControl1.SelectedIndex;
            tabControl1.SelectedIndex = currentIndex - 2; //hasta kayıt sayfasına gidicek ve tckimlik dogum tarihi gibi degerler eskisi gibi kalıcak am yine de değiştirilebilir 
            //değiştirilmemesini istersek enabled false kullanılabilir

            //ayrıca sil butonuna tıklamayı unutmamamız gerekiyor 
        }
        #endregion



        #region kullanıcıVerileri listinden girilen tc kimlige ait bulunan veri kısmını siliyor gecmiTextBoxtan
        private void deletebutton_Click(object sender, EventArgs e)
        {
            // Silinecek TC Kimlik Numarasını al
            string silinecekTc = tcGirBox.Text;

            // TC Kimlik Numarasına göre hastanın verilerini bul
            string bulunanVeri = kullaniciVerileri.FirstOrDefault(veri => veri.Contains($"TC Kimlik: {silinecekTc}")); 

            if (bulunanVeri != null)
            {
                // Bulunan veriyi gecmisTextBox'tan çıkart
                gecmisTextBox.Text = gecmisTextBox.Text.Replace(bulunanVeri, "");

                MessageBox.Show("Veri başarıyla silindi.");
            }
            else
            {
                MessageBox.Show("Silinecek veri bulunamadı.");
            }
        }
        #endregion





        #region veri tabanına veri aktarıyoruz bunu yaparkende aynı tc ye sahip olup olmamasına dikkat ediyorum
        private void button5_Click(object sender, EventArgs e)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                try
                {
                    connection.Open(); //bir baglantı açar
                    MessageBox.Show("Bağlantı başarıyla açıldı.");

                    foreach (DataGridViewRow row in dataGridView1.Rows)  //her satıra değer dondurmek için
                    {
                        if (!row.IsNewRow)
                        {
                            // Verileri sırasıyla al
                            string tc = row.Cells[1].Value.ToString();
                            //ilk basta bir tc alıyor ve bunun kac kere bulundugunun sorgusunu yaoıyorum

                            // SQL sorgusunu oluştur
                            string checkSql = $"SELECT COUNT(*) FROM public.\"HASTA\" WHERE \"tcKimlik\" = '{tc}'"; //kontrol

                            using (NpgsqlCommand checkCommand = new NpgsqlCommand(checkSql, connection)) //sonra veri tabanına yolluyorum
                            {
                                int rowCount = Convert.ToInt32(checkCommand.ExecuteScalar());//sayısını alıyor

                                if (rowCount > 0) //eger zaten biri varsa guncelleme yaptırıyoruz
                                {
                                    string adSoyad = row.Cells[0].Value.ToString();
                                    string telefon = row.Cells[2].Value.ToString();
                                    string dogumTarihi = row.Cells[3].Value.ToString();
                                    string dogumYeri = row.Cells[4].Value.ToString();
                                    string adres = row.Cells[5].Value.ToString();
                                    string hastalik = row.Cells[6].Value.ToString();
                                    string ilac = row.Cells[7].Value.ToString();

                                    string updateSql = $"UPDATE public.\"HASTA\" SET \"adiSoyadi\" = '{adSoyad}', \"telefonNo\" = '{telefon}', " +
                                                       $"\"dTarihi\" = '{dogumTarihi}', \"dYeri\" = '{dogumYeri}', \"adres\" = '{adres}', " +
                                                       $"\"hastalik\" = '{hastalik}', \"ilac\" = '{ilac}' WHERE \"tcKimlik\" = '{tc}'";

                                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateSql, connection))
                                    {
                                        updateCommand.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // TC Kimlik numarasına sahip kişi yok, yeni kayıt ekliyoruz
                                    string adSoyad = row.Cells[0].Value.ToString();
                                    string telefon = row.Cells[2].Value.ToString();
                                    string dogumTarihi = row.Cells[3].Value.ToString();
                                    string dogumYeri = row.Cells[4].Value.ToString();
                                    string adres = row.Cells[5].Value.ToString();
                                    string hastalik = row.Cells[6].Value.ToString();
                                    string ilac = row.Cells[7].Value.ToString();

                                    string insertSql = "INSERT INTO public.\"HASTA\" (\"adiSoyadi\", \"tcKimlik\", \"telefonNo\", " +
                                                      $"\"dTarihi\", \"dYeri\", \"adres\", \"hastalik\", \"ilac\") " +
                                                      $"VALUES ('{adSoyad}', '{tc}', '{telefon}', '{dogumTarihi}', '{dogumYeri}', '{adres}', '{hastalik}', '{ilac}')";

                                    using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertSql, connection))
                                    {
                                        insertCommand.ExecuteNonQuery();

                                        //ExecuteNonQuery metodunu kullanarak, veritabanına etkileşimde bulunuluyor ve sql komutu çalıştırılıyor.
                                        //Bu metot insert, update  delete gibi değişiklik yapıcı sql komutları için kullanılır.
                                        //ExecuteNonQuery metodunun döndüğü değer etkilenen satır sayısıdır.
                                    }
                                }
                            }
                        }
                    }

                    MessageBox.Show("Veriler PostgreSQL veritabanına başarıyla eklendi veya güncellendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message); //exception mesajı kullanıtoryuz
                }
            }
        }

        #endregion

        private void button8_Click(object sender, EventArgs e)
        {
            // Veritabanına yeni verileri ekleyelim
            VeritabaninaVeriEkle();

            // Veritabanındaki verileri hastalık adına göre sıralayalım
            VeritabanindakiVerileriSiralama("hastalik");

            // DataGridView'i güncelleyelim
            VeritabaniKayitlariniGoruntule();
        }

        // Veritabanına yeni verileri eklemek için kullanılacak metod
        private void VeritabaninaVeriEkle()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            // Verileri sırasıyla al
                            string tc = row.Cells[1].Value.ToString();

                            // SQL sorgusunu oluştur
                            string checkSql = $"SELECT COUNT(*) FROM public.\"HASTA\" WHERE \"tcKimlik\" = '{tc}'";

                            using (NpgsqlCommand checkCommand = new NpgsqlCommand(checkSql, connection))
                            {
                                int rowCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                                if (rowCount > 0)
                                {
                                    // Hasta varsa güncelle
                                    string adSoyad = row.Cells[0].Value.ToString();
                                    string telefon = row.Cells[2].Value.ToString();
                                    string dogumTarihi = row.Cells[3].Value.ToString();
                                    string dogumYeri = row.Cells[4].Value.ToString();
                                    string adres = row.Cells[5].Value.ToString();
                                    string hastalik = row.Cells[6].Value.ToString();
                                    string ilac = row.Cells[7].Value.ToString();

                                    string updateSql = $"UPDATE public.\"HASTA\" SET \"adiSoyadi\" = '{adSoyad}', \"telefonNo\" = '{telefon}', " +
                                                       $"\"dTarihi\" = '{dogumTarihi}', \"dYeri\" = '{dogumYeri}', \"adres\" = '{adres}', " +
                                                       $"\"hastalik\" = '{hastalik}', \"ilac\" = '{ilac}' WHERE \"tcKimlik\" = '{tc}'";

                                    using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateSql, connection))
                                    {
                                        updateCommand.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Hasta yoksa ekle
                                    string adSoyad = row.Cells[0].Value.ToString();
                                    string telefon = row.Cells[2].Value.ToString();
                                    string dogumTarihi = row.Cells[3].Value.ToString();
                                    string dogumYeri = row.Cells[4].Value.ToString();
                                    string adres = row.Cells[5].Value.ToString();
                                    string hastalik = row.Cells[6].Value.ToString();
                                    string ilac = row.Cells[7].Value.ToString();

                                    string insertSql = "INSERT INTO public.\"HASTA\" (\"adiSoyadi\", \"tcKimlik\", \"telefonNo\", " +
                                                      $"\"dTarihi\", \"dYeri\", \"adres\", \"hastalik\", \"ilac\") " +
                                                      $"VALUES ('{adSoyad}', '{tc}', '{telefon}', '{dogumTarihi}', '{dogumYeri}', '{adres}', '{hastalik}', '{ilac}')";

                                    using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertSql, connection))
                                    {
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }

                    MessageBox.Show("Veriler PostgreSQL veritabanına başarıyla eklendi veya güncellendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message);
                }
            }
        }

        // Veritabanındaki verileri belirli bir sütuna göre sıralamak için kullanılacak metod
        private void VeritabanindakiVerileriSiralama(string siraSutunu)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // Veritabanındaki verileri sıralama sorgusu
                    string sortSql = $"SELECT * FROM public.\"HASTA\" ORDER BY \"{siraSutunu}\" ASC";

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sortSql, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // DataGridView'e verileri bağla
                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı hatası: " + ex.Message);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Veritabanına yeni verileri ekleyelim
            VeritabaninaVeriEkle();

            // Veritabanındaki verileri ad soyada göre sıralayalım
            VeritabanindakiVerileriSiralama("adiSoyadi");

            // DataGridView'i güncelleyelim
            VeritabaniKayitlariniGoruntule();
        }

    }

}
