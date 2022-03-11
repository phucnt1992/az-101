using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Az101.Gallery.Dto;

public class PhotoForm
{
    [Required]
    public string Title { get; set; }
    public string Alt { get; set; }

    [Required]
    public IFormFile File { get; set; }

}
