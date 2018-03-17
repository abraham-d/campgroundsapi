using System;

namespace campgrounds_api.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public DateTime Created { get; set; }
        public User User { get; set; }
    }
}
