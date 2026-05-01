using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMPro.Models
{
    public class Talep
    {
        public int Id { get; set; }
        public string MusteriEmail { get; set; }
        public string CihazAdi { get; set; } // Bu artık "Cihaz Adı" olacak
        public string ArizaAciklamasi { get; set; } // Bu artık "Arıza Açıklaması" olacak
        public string SeriNo { get; set; }
        public string TeknikerNotu { get; set; }
        public string Durum { get; set; } = "Bekliyor";
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? TahminiTeslimTarihi { get; set; }
        public decimal? Fiyat { get; set; }
    }
}
