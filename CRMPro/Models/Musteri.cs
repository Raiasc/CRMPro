using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMPro.Models
{
    public class Musteri
    {
        public int Id { get; set; }
        public int MusteriId { get; set; }
        public string AdSoyad { get; set; }
        public string Eposta { get; set; }
        public string Sifre { get; set; }
        public string FirmaAdi { get; set; }
        public int MusteriSkoru { get; set; }
        public string Adres { get; set; }
        public string Telefon { get; set; }
        public int RolId { get; set; }
        public byte[] ProfilResmi { get; set; }
        
    }
}
