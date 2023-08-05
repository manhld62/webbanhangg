using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VanilaBakery.Models;
using PagedList;
using PagedList.Mvc;
using System.Net;
using System.Data;
using System.Data.Entity;


namespace VanilaBakery.Controllers
{

    public class VanilaBakeryController : Controller
    {

        // GET: VanilaBakery
        VanilaEntities data = new VanilaEntities();
        public ActionResult Index()
        {
            var banhmoi = BanhMoi(1);
            return View(banhmoi);
        }
        [ChildActionOnly]
        public ActionResult LoaiBanh()
        {
            var loaibanh = from lb in data.LOAIs select lb;
            return PartialView(loaibanh);
        }
        public List<BANH> BanhMoi(int count)
        {
            return data.BANHs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public List<BANH> BanhBanChay(int count)
        {
            return data.BANHs.OrderBy(Random => Random.Ngaycapnhat).Take(count).ToList();
        }
        [ChildActionOnly]
        public ActionResult BanhHot()
        {
            var hot = BanhBanChay(3);
            return PartialView(hot);
        }
       
      
        public ActionResult About()
        {
            return View();
        }
        public ActionResult ChiTiet(int id)
        {
            var chitiet = from ct in data.BANHs
                          where ct.MaBanh == id
                          select ct;
            return View(chitiet.Single());
        }
        [ChildActionOnly]
        public ActionResult Sales()
        {
            var sale = from s in data.BANHs select s;
            return PartialView(sale);
        }
        //show sản phẩm theo loại
        public ActionResult SPTheoLoai(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int pageSize = 5;
            int pageNum = (page ?? 1);
            ViewBag.id = id;
            var sptheoloai = from sp in data.BANHs where sp.MaLoai == id select sp;
            if (sptheoloai == null)
            {
                return HttpNotFound();
            }
            Session["Banh"] = sptheoloai;
            return View(sptheoloai.OrderBy(n => n.MaBanh).ToPagedList(pageNum, pageSize));
        }
        [ChildActionOnly]
        public ActionResult NewCake()
        {
            var newcake = BanhMoi(3);
            return PartialView(newcake);
        }
        public ActionResult GioiThieu()
        {
         
            return View();
        }
        public ActionResult ChiNhanh()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Tag()
        {
            var tag = from t in data.LOAIs select t;
            return PartialView(tag);
        }

        public List<BLOG> Blogs1(int count)
        {
            return data.BLOGs.OrderBy(a => a.MaBlog).Take(count).ToList();
        }
        public List<BLOG> Blogs2(int count)
        {
            return data.BLOGs.OrderByDescending(a => a.MaBlog).Take(count).ToList();
        }
        [ChildActionOnly]
        public ActionResult Blog()
        {
            var blogs1 = Blogs2(1);
            return PartialView(blogs1);
        }
        [ChildActionOnly]
        public ActionResult Blog2()
        {
            var blogs2 = Blogs1(2);
            return PartialView(blogs2);
        }
        public ActionResult ChiTietBlog(int id)
        {
            var chitietblog = from bl in data.BLOGs
                              where bl.MaBlog == id
                              select bl;
            return View(chitietblog.Single());
        }
        public ActionResult DanhSachBlog(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var dsblog = data.BLOGs.OrderByDescending(n => n.NgayDangTin);
            return View(dsblog.ToPagedList(pageNum, pageSize));
        }
        public ActionResult ThuVienAnh()
        {
            var thuvienanh = from tv in data.ThuVienAnhs select tv;
            return View(thuvienanh);
        }
        public ActionResult ShowBanh(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);
            var banh = data.BANHs.OrderByDescending(n => n.MaBanh);
            return View(banh.ToPagedList(pageNum,pageSize));
        }
    }
  }
