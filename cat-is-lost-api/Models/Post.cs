using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace cat_is_lost_api.Models
{
    public class Post
    {
        public int? Id { get; set; }
        public int? User_Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Pictures { get; set; }
        public double Lat {  get; set; }
        public double Lng { get; set; }
        public DateTime Date_Missing { get; set; } 
        public bool Found { get; set; }

        // Files used for data transfer from front end. 
        // Store the file path in Pictures field on successful write.
        [NotMapped]
        public List<IFormFile>? Files { get; set; }
    }
}
