using System;

namespace Az101.Gallery.Models;

public class Photo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Alt { get; set; }
    public string FileName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }

}

