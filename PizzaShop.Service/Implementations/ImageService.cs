using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using PizzaShop.Service.Interfaces;


namespace PizzaShop.Service.Implementations;

public class ImageService : IImageService
{
    public async Task<string> ImgPath(IFormFile? Img)
    {
        if (Img != null)
        {
            var fileGuid = Guid.NewGuid().ToString();

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            var fileExtension = Path.GetExtension(Img.FileName);
            var filePath = Path.Combine(uploadsFolder, fileGuid + fileExtension);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await Img.CopyToAsync(fileStream);
            }

            string path = fileGuid + fileExtension;
            return path;
        }
#pragma warning disable CS8603

        return null;
#pragma warning restore CS8603

    }

}