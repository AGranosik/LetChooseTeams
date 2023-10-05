namespace LCT.Api.Configuration.Models
{
    public record ErrorResponseModel(Guid RequestId, string ErrorMessage);
}
