using ImageShareLikes.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageShareLikes.Data;

namespace ImageShareLikes.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _webHostEnviorment;


        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnviorment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new ImageShareLikesRepo(connectionString);
            var images = repo.GetAll();
            return View(new IndexViewModel { Images = images});
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile image, string title)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";

            string filePath = Path.Combine(_webHostEnviorment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new ImageShareLikesRepo(connectionString);

            repo.AddImage(fileName, title);

            return Redirect("/");
        }

        
        public IActionResult ViewImage(int id, bool liked)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new ImageShareLikesRepo(connectionString);
            var image = repo.GetById(id);
            if (image == null)
            {
                return Redirect("/");
            }

            List<int> ids = HttpContext.Session.Get<List<int>>("Ids");

            if (ids == null)
            {
                ids = new List<int>();
            }
            if (liked)
            {
                ids.Add(id);
                repo.LikeImage(id);
            }
            HttpContext.Session.Set("Ids", ids);

            return View(new ImageViewModel { Image = image, Ids = ids });
        }

        [HttpPost]
        public void LikeImage(int id)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new ImageShareLikesRepo(connectionString);
            repo.LikeImage(id);
        }

        public IActionResult GetLikes(int id)
        {
            var connectionString = _configuration.GetConnectionString("ConStr");
            var repo = new ImageShareLikesRepo(connectionString);
            int result = repo.GetLikes(id);
            return Json(result);
        }
    }
}
