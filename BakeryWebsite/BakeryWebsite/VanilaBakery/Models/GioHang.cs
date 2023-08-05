using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VanilaBakery.Models;
namespace VanilaBakery.Models
{
    public class GioHang
    {
        VanilaEntities db = new VanilaEntities();
        public int iMabanh { set; get; }
        public string sTenbanh { set; get; }
        public string sHinhanh { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public GioHang(int Mabanh)
        {
            iMabanh = Mabanh;
            BANH banh = db.BANHs.Single(n => n.MaBanh == iMabanh);
            sTenbanh = banh.TenBanh;
            sHinhanh = banh.Hinhanh;
            dDongia = double.Parse(banh.Giaban.ToString());
            iSoluong = 1;
        }
    }
}