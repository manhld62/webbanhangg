using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using VanilaBakery.Models;
using System.Net;
using System.IO;
using System.Data.Entity;

namespace Vanila.Controllers
{
    public class AdminController : Controller
    {

        VanilaEntities db = new VanilaEntities();

        // GET: Admin/
        public ActionResult Index()
        {
            
            //điều hướng
            if (Session["UserName"] == null)
            {
                return RedirectToAction("QLSanPham");
            }
           
            if(Session["admin"] == null){
                return RedirectToAction("QLSanPham");
            }
            
            var user = db.ADMINs.OrderByDescending(n => n.UserAdmin).Where(i => i.IsAdmin == 0 && i.Allowed == 1);
            return View(user);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(ADMIN user)
        {
            //string passwordMD5 = Conmon.EncryptMD5(password);
            ADMIN User = db.ADMINs.SingleOrDefault(x => x.UserName == user.UserName && x.Password == user.Password && x.Allowed == 1);
            ADMIN ad = db.ADMINs.SingleOrDefault(n => n.UserName == user.UserName && n.Password == user.Password && n.Allowed == 1 && n.IsAdmin == 1);
            if (user != null)
            {
                Session["UserName"] = User.UserName;
                Session["FullName"] = User.FullName;
                Session["Avatar"] = User.Avatar;
                Session["admin"] = ad;

                return RedirectToAction("Index");
            }
            ViewBag.error = "Đăng Nhập sai bạn không có quyền vào!!!!";

            return View();
        }
        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["FullName"] = null;
            Session["Avatar"] = null;
            Session["admin"] = null;
            return RedirectToAction("Login");
        }
        //xóa tài khoản
        public ActionResult Xoa(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ADMIN admin = db.ADMINs.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }
        [HttpPost, ActionName("Xoa")]
        [ValidateAntiForgeryToken]
        public ActionResult Xoa(int id)
        {
            ADMIN admin = db.ADMINs.Find(id);
            admin.Allowed = 0;
            db.Entry(admin).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //thêm user
        [ChildActionOnly]
        public ActionResult Them(int? so)
        {
            if (so == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Them(ADMIN ad, HttpPostedFileBase Avatar)
        {
            ADMIN admin = db.ADMINs.Add(ad);
            admin.Allowed = 1;
            admin.IsAdmin = 0;
            db.SaveChanges();
            if (ModelState.IsValid)
            {
                if (Avatar.ContentLength > 0 && Avatar != null)
                {
                    try
                    {
                        string fileName = admin.UserAdmin + Path.GetExtension(Avatar.FileName);
                        string path = Path.Combine(Server.MapPath("~/Content/Admin/images/"), fileName);
                        Avatar.SaveAs(path);
                        admin.Avatar = fileName;
                    }
                    catch (Exception ex)
                    {
                        return Content("Upload lỗi" + ex);
                    }
                }
                db.Entry(admin).State = EntityState.Modified;
                //lưu thay đổi
                db.SaveChanges();
                //RedirectToAction bắt buộc phải return vào 1 view
                return RedirectToAction("Index");
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Layid(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Sua", new { @id = id });
        }
        //sửa
        public ActionResult Sua(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ADMIN ad = db.ADMINs.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua(ADMIN admin, HttpPostedFileBase Avatar)
        {
            ADMIN ad = db.ADMINs.SingleOrDefault(n => n.UserAdmin == admin.UserAdmin);
            ad.UserName = admin.UserName;
            ad.FullName = admin.FullName;
            ad.Email = admin.Email;
            ad.Password = admin.Password;
            if (Avatar.ContentLength > 0 && Avatar != null)
            {
                try
                {
                    string name = ad.UserAdmin + Path.GetExtension(Avatar.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Admin/images/"), name);
                    Avatar.SaveAs(path);
                    ad.Avatar = name;
                }
                catch (Exception ex)
                {
                    return Content("Upload lỗi" + ex);
                }
            }
            db.Entry(ad).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult QLSanPham()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login");
            }

            var list = db.BANHs.OrderByDescending(n => n.MaBanh);
            return View(list);
        }
        public ActionResult QLDonHang()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login");
            }

            var list = db.DONDATHANGs.OrderByDescending(n => n.MaDonHang);
            return View(list);
        }

        //thêm sản phẩm
        [ChildActionOnly]
        public ActionResult Them1(int? so)
        {
            if (so == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult Them1(BANH ba, HttpPostedFileBase Hinhanh)
        {
            BANH admin = db.BANHs.Add(ba);
            db.SaveChanges();
            if (ModelState.IsValid)
            {
                if (Hinhanh.ContentLength > 0 && Hinhanh != null)
                {
                    try
                    {
                        string fileName =admin.MaBanh + Path.GetExtension(Hinhanh.FileName);
                        string path = Path.Combine(Server.MapPath("~/images/cake/"), fileName);
                        Hinhanh.SaveAs(path);
                        admin.Hinhanh = fileName;
                    }
                    catch (Exception ex)
                    {
                        return Content("Upload lỗi" + ex);
                    }
                }
                db.Entry(admin).State = EntityState.Modified;
                //lưu thay đổi
                db.SaveChanges();
                //RedirectToAction bắt buộc phải return vào 1 view
                return RedirectToAction("QlSanPham");
            }
            db.SaveChanges();
            return RedirectToAction("QlSanPham", "Admin");
        }
     
        public ActionResult Sua1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BANH ad = db.BANHs.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }
        //Sua san pham
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua1(BANH admin, HttpPostedFileBase Hinhanh)
        {
            BANH ad = db.BANHs.SingleOrDefault(n => n.MaBanh == admin.MaBanh);
            ad.TenBanh = admin.TenBanh;
            ad.GiaChuaGiam = admin.GiaChuaGiam;
            ad.GiamGia = admin.GiamGia;
            ad.Mota = admin.Mota;
            ad.Ngaycapnhat = admin.Ngaycapnhat;
            ad.Soluongton = admin.Soluongton;
            ad.MaLoai = admin.MaLoai;

            if (!(Hinhanh == null))
            {

            
           
                try
                {
                    string name = ad.MaBanh + Path.GetExtension(Hinhanh.FileName);
                    string path = Path.Combine(Server.MapPath("~/images/cake/"), name);
                    Hinhanh.SaveAs(path);
                    ad.Hinhanh = name;
                }
                catch (Exception ex)
                {
                    return Content("Upload lỗi" + ex);
                }
            
            }
            db.Entry(ad).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("QLSanPham");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua3(DONDATHANG admin)
        {
            DONDATHANG ad = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == admin.MaDonHang);
            ad.MaDonHang = admin.MaDonHang;
            ad.Tinhtranggiaohang = admin.Tinhtranggiaohang;
            ad.Ngaydat = admin.Ngaydat;
            ad.Ngaygiao = admin.Ngaygiao;
            ad.Ghichu = admin.Ghichu;
            ad.KHACHHANG.HoTen = admin.KHACHHANG.HoTen;
            ad.KHACHHANG.DienthoaiKH = admin.KHACHHANG.DienthoaiKH;

       
            db.Entry(ad).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("QLDonHang");
        }

        //Xóa sản Phẩm
        public ActionResult Xoa1(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BANH admin = db.BANHs.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }
        [HttpPost, ActionName("Xoa1")]
        [ValidateAntiForgeryToken]
        public ActionResult Xoa1(int id)
        {
            BANH admin = db.BANHs.Find(id);
            db.Entry(admin).State = EntityState.Modified;
            db.BANHs.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("QLSanPham");
        }
        [HttpPost, ActionName("Xoa3")]
        [ValidateAntiForgeryToken]
        public ActionResult Xoa3(int id)
        {
            CHITIETDONTHANG admin = db.CHITIETDONTHANGs.Find(id);
            db.Entry(admin).State = EntityState.Modified;
            db.CHITIETDONTHANGs.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("QLDonHang");
        }
        // Ql Khach hang
        public ActionResult QLKhachHang()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("QLSanPham");
            }
            var khachhang = db.KHACHHANGs.OrderByDescending(k => k.MaKH);
            return View(khachhang);
        }
        public ActionResult ChitietDonHang()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login");
            }

            var list = db.CHITIETDONTHANGs.OrderByDescending(n => n.MaDonHang);
            return View(list);
        }
        //thêm khách hàng
        [ChildActionOnly]
        public ActionResult Them2(int? so)
        {
            if (so == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult Them2(KHACHHANG kh)
        {
            KHACHHANG admin = db.KHACHHANGs.Add(kh);
            db.SaveChanges();
            //RedirectToAction bắt buộc phải return vào 1 view
            return RedirectToAction("QLKhachHang", "Admin");
        }

        public ActionResult Sua2 (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG ad = db.KHACHHANGs.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }
        public ActionResult Sua3(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONDATHANG ad = db.DONDATHANGs.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        //Sua thong tin khach hang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sua2(KHACHHANG admin )
        {
           KHACHHANG ad = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == admin.MaKH);
            ad.HoTen = admin.HoTen;
            ad.Taikhoan = admin.Taikhoan;
            ad.Matkhau = admin.Matkhau;
            ad.Email = admin.Email;
            ad.DiachiKH = admin.DiachiKH;
            ad.DienthoaiKH = admin.DienthoaiKH;
            ad.DONDATHANGs = admin.DONDATHANGs;
            db.Entry(ad).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("QLKhachHang");
        }
        //xoa khach hang
        public ActionResult Xoa2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHACHHANG admin = db.KHACHHANGs.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }
        //xoa khach hang
        [HttpPost, ActionName("Xoa2")]
        [ValidateAntiForgeryToken]
        public ActionResult Xoa2(int id)
        {
            KHACHHANG admin = db.KHACHHANGs.Find(id);
            db.Entry(admin).State = EntityState.Modified;
            db.KHACHHANGs.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("QLKhachHang");
        }
    }
}
