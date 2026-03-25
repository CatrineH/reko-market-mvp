namespace reko_mini_project.Server.Features.ImageProcessing.Validation;

public interface IImageValidator
{
    void Validate(IFormFile file);
}