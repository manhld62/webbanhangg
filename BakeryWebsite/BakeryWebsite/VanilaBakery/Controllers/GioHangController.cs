using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using VanilaBakery.Models;
using VanilaBakery.Common;
using VanilaBakery.VNPayCofig;

using System.Web;
using System.Net.Http;

namespace VanilaBakery.Controllers
{
    public class GioHangController : Controller
    {
        VanilaEntities data = new VanilaEntities();
        public static Dictionary<string, string> _vnp_TransactionStatus = new Dictionary<string, string>()
        {
            {"00","Giao dịch thành công" },
            {"01","Giao dịch chưa hoàn tất" },
            {"02","Giao dịch bị lỗi" },
            {"04","Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)" },
            {"05","VNPAY đang xử lý giao dịch này (GD hoàn tiền)" },
            {"06","VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)" },
            {"07","Giao dịch bị nghi ngờ gian lận" },
            {"09","GD Hoàn trả bị từ chối" }
        };
      

        // GET: GioHang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int iMabanh, string strURL)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.Find(n => n.iMabanh == iMabanh);

            if (sanpham == null)
            {
                sanpham = new GioHang(iMabanh);
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }   
            else
            {
                var productDb = data.BANHs.Where(c => c.MaBanh == iMabanh).FirstOrDefault();
                if (!(sanpham.iSoluong >= productDb.Soluongton))
                {
                    sanpham.iSoluong++;
                }
                
                return Redirect(strURL);
            }

        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        public ActionResult GioHang(string mess)
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "VanilaBakery");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            if (mess != null)
            {
                ViewBag.Mess = mess;
            }
              
            return View(lstGioHang);
        }
        public ActionResult GioHangCon()
        {
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView(lstGioHang);
        }
        public ActionResult XoaGioHang(int iMaSP)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMabanh == iMaSP);
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMabanh == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "VanilaBakery");
            }
            return RedirectToAction("GioHang");
        }


        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            var productDb = data.BANHs.Where(c => c.MaBanh == iMaSP).FirstOrDefault();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMabanh == iMaSP);
            if (sanpham != null)
            {
                var numbernew = int.Parse(f["txtSoLuong"].ToString());
                if (numbernew <= productDb.Soluongton && numbernew >= 1)
                {
                    sanpham.iSoluong = numbernew;
                }
                else if (sanpham.iSoluong < 0)
                {
                    return RedirectToAction("GioHang", new { mess = $"Vui lòng nhập số lượng trên 1" });
                }
                else
                {
                    return RedirectToAction("GioHang", new { mess = $"Số lượng bánh còn {productDb.Soluongton}"});
                }
               
              
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "VanilaBakery");
        }

        //CAP NHAP GIO HANG CHI TIET
        public ActionResult CapNhatGioHang1(int iMaSP, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMabanh == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("ChiTiet", "VanilaBakery");
        }

        [HttpGet]
        public ActionResult DatHang(string messvnpay)
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Nguoidung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "VanilaBakery");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            if (!string.IsNullOrEmpty(messvnpay))
            {
                ViewBag.Messvnpay = messvnpay;
            }
           
            return View(lstGioHang);
        }
      
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {        
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<GioHang> gh = LayGioHang();
            if (gh.Count() < 1)
            {
                return RedirectToAction("XacNhanDonHang", "GioHang", new { mess = "Đặt hàng thất bại !, Vui lòng kiểm tra lại" });
            }
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            var ghichu = collection["Ghichu"];
            ddh.Ghichu = ghichu;
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();
            if (collection["payment"] == "0")
            {            
                foreach (var item in gh)
                {
                    CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                    ctdh.MaDonHang = ddh.MaDonHang;
                    ctdh.MaBanh = item.iMabanh;
                    ctdh.Soluong = item.iSoluong;
                    ctdh.Dongia = (decimal)item.dDongia;
                    data.CHITIETDONTHANGs.Add(ctdh);

                    var banh = data.BANHs.Where(c => c.MaBanh == ctdh.MaBanh).FirstOrDefault();
                    if (banh != null)
                    {
                        banh.Soluongton = banh.Soluongton - item.iSoluong;
                        data.Entry(banh).State = EntityState.Modified;
                    }
                }
                data.SaveChanges();
                Session["GioHang"] = null;
                return RedirectToAction("XacNhanDonHang", "GioHang", new { mess = "Đơn hàng của quý khách đã được chúng tôi ghi nhận.Chúng tôi sẽ liên hệ quý khách trong thời gian sớm nhất và giao hàng đúng hẹn." });
            }
            else
            {
                if (string.IsNullOrEmpty(VNPayConfig.vnp_TmnCode) || string.IsNullOrEmpty(VNPayConfig.vnp_HashSecret))
                {
                    ViewBag.SuccessMessage = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret";
                    return View();
                }
                //Hóa đơn VNpay
                OrderInfo order = new OrderInfo();
                order.OrderId = ddh.MaDonHang.ToString(); // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
                order.Amount = LayGioHang().Sum(c=>c.dThanhtien); // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
                order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending"                   
                order.CreatedDate = ddh.Ngaydat.Value;


                VnPayLibrary vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", VNPayConfig.vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());//Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
                                                                                    //if (!string.IsNullOrEmpty(request.NganHang))
                vnpay.AddRequestData("vnp_BankCode", "NCB" /*request.NganHang*/);
                vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", VnPayLibrary.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);

                vnpay.AddRequestData("vnp_ReturnUrl", VNPayConfig.vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());// Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
                                                                             //Add Params of 2.1.0 Versio                                                                            //vnpay.AddRequestData("vnp_ExpireDate", request.ThoiHanThanhToan);
                                                                             //Billing
                vnpay.AddRequestData("vnp_Bill_Mobile", kh.DienthoaiKH.Trim());
                vnpay.AddRequestData("vnp_Bill_Email", kh.Email.Trim());
                var fullName = kh.HoTen.Trim();
                //if (!String.IsNullOrEmpty(fullName))
                //{
                //    var indexof = fullName.IndexOf(' ');
                //    vnpay.AddRequestData("vnp_Bill_FirstName", fullName.Substring(0, indexof));
                //    vnpay.AddRequestData("vnp_Bill_LastName", fullName.Substring(indexof + 1, fullName.Length - indexof - 1));
                //}
                vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
                                                                // Kích hoạt Session
                Session["OrderId"] = order.OrderId;// Thiết lập giá trị cho Session            
                string paymentUrl = vnpay.CreateRequestUrl(VNPayConfig.vnp_Url, VNPayConfig.vnp_HashSecret);
                return Redirect(paymentUrl);

            }
            
        }
        [HttpGet]
        public ActionResult VNPayReturn (string vnp_Amount, string vnp_BankCode, string vnp_BankTranNo, string vnp_CardType, string vnp_OrderInfo, string vnp_PayDate, string vnp_ResponseCode, string vnp_TmnCode, string vnp_TransactionNo, string vnp_TransactionStatus, string vnp_TxnRef, string vnp_SecureHash)
        {
            var orderId = Convert.ToInt32(Session["OrderId"].ToString());
            string message = "Không xác định được trạng thái";
            var hoadon = data.DONDATHANGs.Where(c => c.MaDonHang == orderId).FirstOrDefault();
            if (_vnp_TransactionStatus.ContainsKey(vnp_TransactionStatus))
            {
                message = _vnp_TransactionStatus[vnp_TransactionStatus];
            }
           
            if (vnp_TransactionStatus == "00")
            {

              
                hoadon.Dathanhtoan = true;
                data.Entry(hoadon).State = EntityState.Modified;
                
                foreach (var item in LayGioHang())
                {
                    CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                    ctdh.MaDonHang = hoadon.MaDonHang;
                    ctdh.MaBanh = item.iMabanh;
                    ctdh.Soluong = item.iSoluong;
                    ctdh.Dongia = (decimal)item.dDongia;
                    data.CHITIETDONTHANGs.Add(ctdh);


                    var banh = data.BANHs.Where(c => c.MaBanh == ctdh.MaBanh).FirstOrDefault();
                    if (banh != null)
                    {
                        banh.Soluongton = banh.Soluongton - item.iSoluong;
                        data.Entry(banh).State = EntityState.Modified;
                    }
                }
                data.SaveChanges();
                Session["GioHang"] = null;
                Session["OrderId"] = null;
                return RedirectToAction("XacNhanDonHang", "GioHang", new { mess = "Thanh toán thành công,Đơn hàng của quý khách đã được chúng tôi ghi nhận.Chúng tôi sẽ liên hệ quý khách trong thời gian sớm nhất và giao hàng đúng hẹn. " });
            }
            else if (vnp_TransactionStatus =="02")
            {
               
                if (hoadon != null)
                {

                    data.DONDATHANGs.Remove(hoadon);
                    data.SaveChanges();

                }
                Session["OrderId"] = null;
                return RedirectToAction("DatHang", "GioHang");
            }
            else 
            {
                
                
                if (hoadon != null) {

                    data.DONDATHANGs.Remove(hoadon);
                    data.SaveChanges();
                }
                Session["OrderId"] = null;
                //Thanh toán thất bại
                return RedirectToAction("DatHang", "GioHang", new { messvnpay = message });
            }           
        
        }
       
        public ActionResult XacNhanDonHang(string mess = null)
        {
            ViewBag.Mess = mess; 
            return View();
        }
    }
}