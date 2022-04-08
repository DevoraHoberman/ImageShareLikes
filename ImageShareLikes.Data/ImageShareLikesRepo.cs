using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageShareLikes.Data
{
    public class ImageShareLikesRepo
    {
        private string _connectionString;

        public ImageShareLikesRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Image> GetAll()
        {
            using var context = new ImageContext(_connectionString);
            return context.Images.OrderByDescending(i => i.Date).ToList();
        }

        public void AddImage(string fileName, string title)
        {
            using var context = new ImageContext(_connectionString);
            var image = new Image { FileName = fileName, Title = title, Date = DateTime.Now, Likes = 0 };
            context.Images.Add(image);
            context.SaveChanges();
        }

        public Image GetById(int id)
        {
            using var context = new ImageContext(_connectionString);
            return context.Images.FirstOrDefault(i => i.Id == id);
        }

        public void LikeImage(int id)
        {
            using var context = new ImageContext(_connectionString);
            var image = GetById(id);
            image.Likes++;
            context.Images.Attach(image);
            context.Entry(image).State = EntityState.Modified;
            context.SaveChanges();
        }

        public int GetLikes(int id)
        {
            using var context = new ImageContext(_connectionString);
            var image = context.Images.FirstOrDefault(i => i.Id == id);
            int likes = image.Likes;
            return likes;
        }
    }
}
