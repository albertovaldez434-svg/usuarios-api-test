using WSTestJSON_API.Models;

namespace WSTestJSON_API.Services
{
    public interface IUploadImgService
    {
        Task<ImagenesUsuarios> UploadImage(IFormFile file, int IdUser);
    }
}
