using System;
using System.Linq;
using Facebook;
using System.Configuration;
using System.Web.Mvc;
using VanilaBakery.Models;
using System.Data.Common;
using System.Net.Mail;
using Common;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Net;
using PagedList;
using System.Text;
using System.Text.RegularExpressions;

namespace VanilaBakery.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        VanilaEntities db = new VanilaEntities();
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection collection,string Matkhau ,KHACHHANG kh)
        {
            
            var hoten = collection["hotenkh"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            string passwordMD5 = MaHoa.EncryptMD5(matkhau);
            var nhaplaimatkhau = collection["Nhaplai"];
            var diachi = collection["Diachi"];
            var email = collection["email"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên không được để trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Bạn chưa nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(nhaplaimatkhau))
            {
                ViewData["Loi4"] = "Vui lòng nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được bỏ trống";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Vui lòng nhập số điện thoại";
            }
            else if(this.IsCaptchaValid(""))
            {
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = passwordMD5;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                db.KHACHHANGs.Add(kh);
                db.SaveChanges();
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Mail.html"));

                content = content.Replace("{{Hoten}}", hoten);
                content = content.Replace("{{username}}", tendn);
                content = content.Replace("{{pass}}", matkhau);
                content = content.Replace("{{phone}}", dienthoai);
                content = content.Replace("{{email}}", email);
                content = content.Replace("{{address}}", diachi);
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                new MailHelper().SendMail(email, "Thông tin đăng ký thành viên", content);
                new MailHelper().SendMail(toEmail, "Thông tin đăng ký thành viên", content);
                return RedirectToAction("DangNhap", "Nguoidung");
                
            }
            else
            {
                ViewBag.Loi = "Sai mã Captcha";
            }
            return this.DangKy();
        }
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection, string Matkhau)
        {
            var tendn = collection["Tendangnhap"];
            var matkhau = collection["Matkhau"];
            string passwordMD5 = MaHoa.EncryptMD5(matkhau);
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Bạn chưa nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == passwordMD5);
                if (kh != null)
                {
                    //ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    ViewBag.User = tendn;
                    return RedirectToAction("Index", "VanilaBakery");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return this.DangNhap();
            
        }
        public ActionResult LienHe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LienHe(FormCollection collection, LIENHE lh)
        {
            var hoten = collection["hoten"];
            var email = collection["email"];
            var dienthoai = collection["tel"];
            var noidung = collection["noidung"];
            lh.HoTen = hoten;
            lh.Email = email;
            lh.DienthoaiKH = dienthoai;
            lh.NoiDung = noidung;
            db.LIENHEs.Add(lh);
            db.SaveChanges();
            return this.LienHe();
        }
        public ActionResult Subcribe()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Subcribe(SUBCRIBE sub)
        {
            db.SUBCRIBEs.Add(sub);
            db.SaveChanges();
            return this.Subcribe();
        }
        //tim kiem ajax
        [HttpPost]
        public ActionResult laytukhoa(string searchString, int? page, int? id)
        {
            return RedirectToAction("LstTimKiem", new { @searchString = searchString, @page = page, @id = id });
        }

        public ActionResult LstTimkiem(string searchString, int? page, int? id)
        {
            if (searchString == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int pageSize = 5;
            int pageNum = (page ?? 1);
            ViewBag.search = searchString;
            var lstSP = db.BANHs.Where(delegate (BANH c)
            {
                if (ConvertToUnSign(c.TenBanh).IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    return true;
                else
                    return false;
            }).AsQueryable();
            if (lstSP == null)
            {
                return HttpNotFound();
            }
            ViewBag.search = searchString;
            return PartialView(lstSP.OrderBy(n => n.MaBanh).ToPagedList(pageNum, pageSize));
        }
        //
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id=ConfigurationManager.AppSettings["FbAppId"],
                client_secret= ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri=RedirectUri.AbsoluteUri,
                response_type="code",
                scope="email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            if(!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string tendn = me.email;
                string id = me.id;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                var khachhang = new KHACHHANG();
                khachhang.Taikhoan = tendn;
                khachhang.HoTen = firstname;
                khachhang.Matkhau = id;
                if(khachhang!=null)
                {
                    
                    db.KHACHHANGs.Add(khachhang);
                    db.SaveChanges();
                    Session["Taikhoan"] = khachhang;
                }
                else
                { 
                    khachhang = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn);
                    Session["Taikhoan"] = khachhang;
                    ViewBag.User = tendn;
                }
            }
            return Redirect("/");
        }
        public ActionResult HienTen()
        {
            return PartialView();
        }
        public ActionResult SignOut()
        {
            Session["Taikhoan"] = null;
            return RedirectToAction("Index", "VanilaBakery");
        }
//kiem tra tai khoan ton tai
[HttpPost]
        public JsonResult CheckUsername(string username)
        {
            bool isValid = db.KHACHHANGs.ToList().Exists(p => p.Taikhoan.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            return Json(isValid);
        }
//kiem tra email ton tai
        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            bool isValid = db.KHACHHANGs.ToList().Exists(p => p.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
            return Json(isValid);
        }
        //tim kiem blog ajax
        [HttpPost]
        public ActionResult LayTu(string searchString, int? page, int? id)
        {
            return RedirectToAction("TimKiemBlog", new { @searchString = searchString, @page = page, @id = id });
        }

        public ActionResult TimKiemBlog(string searchString, int? page, int? id)
        {
            if (searchString == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int pageSize = 5;
            int pageNum = (page ?? 1);
            ViewBag.search = searchString;
            var lstSP = db.BLOGs.Where(delegate (BLOG c)
            {
                if (ConvertToUnSign(c.TieuDe).IndexOf(searchString, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    return true;
                else
                    return false;
            }).AsQueryable();
            if (lstSP == null)
            {
                return HttpNotFound();
            }
            ViewBag.search = searchString;
            return PartialView(lstSP.OrderBy(n => n.NgayDangTin).ToPagedList(pageNum, pageSize));
        }
        //truy van khong dau
        private string ConvertToUnSign(string input)
        {
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            return str2;
        }
    }
}