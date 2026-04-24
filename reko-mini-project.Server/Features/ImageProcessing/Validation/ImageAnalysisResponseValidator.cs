namespace reko_mini_project.Server.Features.ImageProcessing.Validation;

public static class ImageAnalysisResponseValidator
{
    public static IDictionary<string, string[]> Validate(ImageAnalysisResponse response)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(response.Name))
            errors["name"] = ["Name is required."];
        if (string.IsNullOrWhiteSpace(response.Category))
            errors["category"] = ["Category is required."];
        if (response.Weight is null or <= 0)
            errors["weight"] = ["Weight must be greater than zero."];
        if (response.Price is null or <= 0)
            errors["price"] = ["Price must be greater than zero."];

        return errors;
    }
}
