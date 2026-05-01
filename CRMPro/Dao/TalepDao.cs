using CRMPro.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMPro.Models
{
    public class TalepDao
    {
        private readonly SqlBaglanti _sqlBaglanti;
        public TalepDao(SqlBaglanti sqlBaglanti) { _sqlBaglanti = sqlBaglanti; }

        public Dictionary<string, int> TalepIstatistikleriniGetir(string musteriEmail)
        {
            // DEĞİŞİKLİK BURADA: Islemde -> İşlemde, Tamamlandi -> Tamamlandı
            Dictionary<string, int> istatistikler = new Dictionary<string, int>
            {
                { "Bekliyor", 0 },
                { "İşlemde", 0 },
                { "Tamamlandı", 0 }
            };

            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = @"SELECT Durum, COUNT(*) as Sayi 
                        FROM Talepler 
                        WHERE MusteriEmail = @email 
                        GROUP BY Durum";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", musteriEmail);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string durum = dr["Durum"].ToString();
                    int sayi = Convert.ToInt32(dr["Sayi"]);
                    if (istatistikler.ContainsKey(durum))
                        istatistikler[durum] = sayi;
                }
            }
            return istatistikler;
        }
        public bool TalepEkle(Talep t)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                // Yeni kolon (SeriNo) sorguya eklendi
                string sorgu = "INSERT INTO Talepler (MusteriEmail, CihazAdi, ArizaAciklamasi, SeriNo, Durum) " +
                               "VALUES (@p1, @p2, @p3, @p4, 'Bekliyor')";

                SqlCommand cmd = new SqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@p1", t.MusteriEmail);
                cmd.Parameters.AddWithValue("@p2", t.CihazAdi); // Cihaz Adı
                cmd.Parameters.AddWithValue("@p3", t.ArizaAciklamasi); // Arıza Açıklaması
                cmd.Parameters.AddWithValue("@p4", t.SeriNo ?? (object)DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public Dictionary<string, int> IstatistikleriGetir(string email)
        {
            // DEĞİŞİKLİK BURADA: Islemde -> İşlemde, Tamamlandi -> Tamamlandı
            var istatistik = new Dictionary<string, int> { { "Bekliyor", 0 }, { "İşlemde", 0 }, { "Tamamlandı", 0 } };
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "SELECT Durum, COUNT(*) FROM Talepler WHERE MusteriEmail=@p1 GROUP BY Durum";
                SqlCommand cmd = new SqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@p1", email);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string durum = dr[0].ToString();
                        if (istatistik.ContainsKey(durum))
                            istatistik[durum] = (int)dr[1];
                    }
                }
            }
            return istatistik;
        }

        public List<Talep> TalepleriGetirDurumaGore(string email, string durum)
        {
            List<Talep> liste = new List<Talep>();
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                // DEĞİŞİKLİK: Sorguya SeriNo, TeknikerNotu ve TahminiTeslimTarihi eklendi.
                string sorgu = "SELECT Id, CihazAdi, ArizaAciklamasi, SeriNo, TeknikerNotu, TahminiTeslimTarihi, OlusturmaTarihi FROM Talepler WHERE MusteriEmail=@p1 AND Durum=@p2 ORDER BY OlusturmaTarihi DESC";
                SqlCommand cmd = new SqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@p1", email);
                cmd.Parameters.AddWithValue("@p2", durum);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    liste.Add(new Talep
                    {
                        // DEĞİŞİKLİK: Nesneye veritabanındaki gerçek Id atandı.
                        Id = Convert.ToInt32(dr["Id"]),
                        CihazAdi = dr["CihazAdi"].ToString(),
                        ArizaAciklamasi = dr["ArizaAciklamasi"].ToString(),
                        OlusturmaTarihi = Convert.ToDateTime(dr["OlusturmaTarihi"]),

                        // YENİ EKLENEN VERİLER
                        SeriNo = dr["SeriNo"].ToString(),
                        TeknikerNotu = dr["TeknikerNotu"].ToString(),
                        // Tarih verisi veritabanında boş (NULL) olabileceği için hata vermemesi adına DBNull kontrolü yapıyoruz:
                        TahminiTeslimTarihi = dr["TahminiTeslimTarihi"] != DBNull.Value ? Convert.ToDateTime(dr["TahminiTeslimTarihi"]) : (DateTime?)null
                    });
                }
            }
            return liste;
        }
        public bool TalepSil(int talepId)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "DELETE FROM Talepler WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", talepId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool AdminTalepCevapla(int talepId, string teknikerNotu, DateTime? tahminiTeslim, decimal? fiyat, string yeniDurum)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                // Durum "Tamamlandı" ise TeslimTarihi'ni de güncelleyeceğiz, değilse eski halinde veya NULL kalacak.
                string sorgu = @"UPDATE Talepler 
                         SET TeknikerNotu = @not, 
                             TahminiTeslimTarihi = @tahmini, 
                             Fiyat = @fiyat, 
                             Durum = @durum,
                             TeslimTarihi = CASE WHEN @durum = 'Tamamlandı' THEN GETDATE() ELSE TeslimTarihi END
                         WHERE Id = @id";

                SqlCommand cmd = new SqlCommand(sorgu, conn);
                cmd.Parameters.AddWithValue("@id", talepId);
                cmd.Parameters.AddWithValue("@not", string.IsNullOrWhiteSpace(teknikerNotu) ? (object)DBNull.Value : teknikerNotu);
                cmd.Parameters.AddWithValue("@tahmini", tahminiTeslim.HasValue ? (object)tahminiTeslim.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@fiyat", fiyat.HasValue ? (object)fiyat.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@durum", yeniDurum); // "İşlemde" veya "Tamamlandı" gelecek

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // TalepDao.cs içine ekle:
        public List<Talep> TumTalepleriGetir()
        {
            List<Talep> liste = new List<Talep>();
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                // Personel ekranı için tüm verileri çeken sorgu
                string sorgu = @"SELECT Id, MusteriEmail, CihazAdi, ArizaAciklamasi, SeriNo, 
                                TeknikerNotu, Durum, OlusturmaTarihi, TahminiTeslimTarihi, Fiyat 
                         FROM Talepler 
                         ORDER BY OlusturmaTarihi DESC";

                SqlCommand cmd = new SqlCommand(sorgu, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    liste.Add(new Talep
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        MusteriEmail = dr["MusteriEmail"].ToString(),
                        CihazAdi = dr["CihazAdi"].ToString(),
                        ArizaAciklamasi = dr["ArizaAciklamasi"].ToString(),
                        SeriNo = dr["SeriNo"]?.ToString(),
                        TeknikerNotu = dr["TeknikerNotu"]?.ToString(),
                        Durum = dr["Durum"].ToString(),
                        OlusturmaTarihi = Convert.ToDateTime(dr["OlusturmaTarihi"]),
                        TahminiTeslimTarihi = dr["TahminiTeslimTarihi"] != DBNull.Value ? Convert.ToDateTime(dr["TahminiTeslimTarihi"]) : (DateTime?)null,
                        Fiyat = dr["Fiyat"] != DBNull.Value ? Convert.ToDecimal(dr["Fiyat"]) : (decimal?)null
                    });
                }
            }
            return liste;
        }

        public List<Talep> MusterininTalepleriniGetir(string musteriEmail)
        {
            List<Talep> liste = new List<Talep>();

            using (SqlConnection conn = _sqlBaglanti.BaglantiAl()) // _baglanti veya _sqlBaglanti, sende hangisiyse
            {
                // Sorguyu senin veritabanındaki MusteriEmail kolonuna göre düzelttik
                string query = "SELECT * FROM Talepler WHERE MusteriEmail = @pEmail";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@pEmail", musteriEmail);

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    liste.Add(new Talep
                    {
                        Id = (int)dr["Id"],
                        MusteriEmail = dr["MusteriEmail"].ToString(), // Bunu da ekledik
                        CihazAdi = dr["CihazAdi"].ToString(),
                        ArizaAciklamasi = dr["ArizaAciklamasi"].ToString(),
                        TeknikerNotu = dr["TeknikerNotu"].ToString(),
                        Fiyat = dr["Fiyat"] != DBNull.Value ? Convert.ToDecimal(dr["Fiyat"]) : (decimal?)null,
                        Durum = dr["Durum"].ToString(),
                        TahminiTeslimTarihi = dr["TahminiTeslimTarihi"] != DBNull.Value ? Convert.ToDateTime(dr["TahminiTeslimTarihi"]) : (DateTime?)null
                        // Başka sütunların varsa buraya eklemeye devam edebilirsin
                    });
                }
            }
            return liste;
        }
    }
}
