using System;

namespace CRMPro.Models
{
    public class TalepMesaj
    {
        public int Id { get; set; }
        public int TalepId { get; set; }
        public string GonderenRol { get; set; }
        public string MesajMetni { get; set; }
        public DateTime Tarih { get; set; }
    }
}