using System.Net.Http.Headers;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    public static class MultipartFormDataContentExtensions
    {
        public static MultipartFormDataContent AddFormFile(this MultipartFormDataContent mutMultipartFormDataContent, IFormFile formFile)
        {
            // Ensure the stream is disposed of by the caller of this method, as the stream is encapsulated within MultipartFormDataContent
            var fileStream = formFile.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);

            mutMultipartFormDataContent.Add(fileContent, "data", Path.GetFileName(formFile.FileName));

            return mutMultipartFormDataContent;
        }
    }
}