using CRMPro.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRMPro.Data;

namespace CRMPro.Dao
{
    public class KullaniciDao
    {
        private readonly SqlBaglanti _sqlBaglanti;

        public KullaniciDao(SqlBaglanti sqlBaglanti)
        {
            _sqlBaglanti = sqlBaglanti;
        }

        public bool SifreGuncelle(string email, string yeniSifre)
        {
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "UPDATE Kullanicilar SET Sifre = @Sifre WHERE Email = @Email";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@Sifre", yeniSifre);
                komut.Parameters.AddWithValue("@Email", email);
                baglanti.Open();
                int etkilenenSatir = komut.ExecuteNonQuery();
                return etkilenenSatir > 0;
            }
        }

        public Kullanici GirisYap(string email, string sifre)
        {
            Kullanici bulunanKullanici = null;
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                
                string sorgu = "SELECT * FROM Kullanicilar WHERE Email = @p1 AND Sifre = @p2 AND AktifMi = 1";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", email);
                komut.Parameters.AddWithValue("@p2", sifre);

                baglanti.Open();
                using (SqlDataReader okuyucu = komut.ExecuteReader())
                {
                    if (okuyucu.Read())
                    {
                        bulunanKullanici = new Kullanici
                        {
                            Id = Convert.ToInt32(okuyucu["Id"]),
                            RolId = Convert.ToInt32(okuyucu["RolId"]),
                            Email = okuyucu["Email"].ToString()
                        };
                    }
                }

                
                if (bulunanKullanici != null)
                {
                    string updateSorgu = "UPDATE Kullanicilar SET SonGirisTarihi = GETDATE() WHERE Id = @id";
                    SqlCommand updateKomut = new SqlCommand(updateSorgu, baglanti);
                    updateKomut.Parameters.AddWithValue("@id", bulunanKullanici.Id);
                    updateKomut.ExecuteNonQuery();
                }
            }
            return bulunanKullanici;
        }

        public Kullanici KullaniciGetirById(int id)
        {
            using (var baglanti = _sqlBaglanti.BaglantiAl())
            {
                
                string sorgu = "SELECT Id, Email, RolId, SonGirisTarihi, AktifMi FROM Kullanicilar WHERE Id = @id";
                return baglanti.QueryFirstOrDefault<Kullanici>(sorgu, new { id = id });
            }
        }

        
        public bool KullaniciEkle(string email, string sifre, int rolId)
        {
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "INSERT INTO Kullanicilar (Email, Sifre, RolId) VALUES (@p1, @p2, @p3)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                komut.Parameters.AddWithValue("@p1", email);
                komut.Parameters.AddWithValue("@p2", sifre);
                komut.Parameters.AddWithValue("@p3", rolId);

                baglanti.Open();
                return komut.ExecuteNonQuery() > 0;
            }
        }

        
        public bool KullaniciSil(string email)
        {
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "DELETE FROM Kullanicilar WHERE Email = @p1";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);

                komut.Parameters.AddWithValue("@p1", email);

                baglanti.Open();
                return komut.ExecuteNonQuery() > 0;
            }
        }

        public bool EmailKayitliMi(string email)
        {
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "SELECT COUNT(*) FROM Kullanicilar WHERE Email = @Email";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@Email", email);
                baglanti.Open();
                int kayitSayisi = (int)komut.ExecuteScalar();
                return kayitSayisi > 0;
            }
        }

        public bool RolGuncelle(int id, int yeniRolId)
        {
            using (var baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "UPDATE Kullanicilar SET RolId = @rolId WHERE Id = @id";
                int etkilenenSatir = baglanti.Execute(sorgu, new { rolId = yeniRolId, id = id });
                return etkilenenSatir > 0;
            }
        }

        public Kullanici KullaniciGetirFiltreIle(string kriter)
        {
            using (var baglanti = _sqlBaglanti.BaglantiAl())
            {
                
                string sorgu = @"SELECT * FROM Kullanicilar 
                         WHERE Email = @kriter OR CAST(Id AS VARCHAR) = @kriter";

                return baglanti.QueryFirstOrDefault<Kullanici>(sorgu, new { kriter = kriter });
            }
        }

        public List<Kullanici> TumKullanicilariGetir()
        {
            using (var baglanti = _sqlBaglanti.BaglantiAl())
            {
                
                string sorgu = "SELECT Id, Email, RolId, AktifMi, SonGirisTarihi FROM Kullanicilar";
                return baglanti.Query<Kullanici>(sorgu).ToList();
            }
        }

        public bool KullaniciEkle(Kullanici yeniKullanici)
        {
            bool basariliMi = false;
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = "INSERT INTO Kullanicilar (RolId, Email, Sifre, AktifMi) VALUES (@p1, @p2, @p3, 1)";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", yeniKullanici.RolId);
                komut.Parameters.AddWithValue("@p2", yeniKullanici.Email);
                komut.Parameters.AddWithValue("@p3", yeniKullanici.Sifre);

                try
                {
                    baglanti.Open();
                    int etkilenenSatir = komut.ExecuteNonQuery();
                    basariliMi = etkilenenSatir > 0;
                }
                catch (Exception)
                {
                    basariliMi = false;
                }
            }
            return basariliMi;
        }
    }
}