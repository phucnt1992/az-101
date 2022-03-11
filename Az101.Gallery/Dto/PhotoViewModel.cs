using System;

namespace Az101.Gallery.Dto;
public class PhotoViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Alt { get; set; }
    public string FileUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}

