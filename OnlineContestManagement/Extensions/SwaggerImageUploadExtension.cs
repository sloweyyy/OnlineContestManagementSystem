using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

public class SwaggerImageUploadExtension : IConfigureOptions<SwaggerUIOptions>
{
  public void Configure(SwaggerUIOptions options)
  {
    options.InjectJavascript("swaggerImageUpload", @"
var uploadButton = document.createElement('button');
uploadButton.textContent = 'Upload Image';
uploadButton.addEventListener('click', function() {
    var fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = 'image/*';
    fileInput.click();

    fileInput.onchange = function() {
        var file = fileInput.files[0];
        var reader = new FileReader();

        reader.onload = function(e) {
            var base64Data = e.target.result;
            // Send base64Data to your API endpoint
            fetch('/api/contest', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    // ... other contest data
                    ImageUrl: base64Data
                })
            })
            .then(response => response.json())
            .then(data => {
                // Handle the response from your API
                console.log(data);
            })
            .catch(error => {
                console.error('Error uploading image:', error);
            });
        };

        reader.readAsDataURL(file);
    };
});
document.getElementById('swagger-ui').appendChild(uploadButton);           ");
  }
}