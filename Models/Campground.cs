using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace campgrounds_api.Models
{
    public class Campground
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public User User { get; set; }
        public DateTime Created { get; set; }
        public Campground()
        {
            Photos = new Collection<Photo>();
        }
    }
}