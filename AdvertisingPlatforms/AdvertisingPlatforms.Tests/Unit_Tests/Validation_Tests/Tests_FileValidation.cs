using AdvertisingPlatforms.Application.Validators;
using AdvertisingPlatforms.Core.Abstractions;
using AdvertisingPlatforms.Tests.TestResources.Models;
using Moq;
using Xunit;

namespace AdvertisingPlatforms.Tests.Unit_Tests.ValidationTest
{
    public class Tests_FileValidation
    {
        [Fact]
        public void Test_IsValid_NullFile_ReturnsFail()
        {
            /////////////
            // Arrange //
            /////////////
            
            IFormFile? formFile = null;
            IFileValidationParameters validationParameters = new AppParameters_Test() { };
            IFileValidator _fileValidator = new FileValidator(validationParameters);

            /////////
            // Act //
            /////////

            bool result = _fileValidator.IsValid(formFile, out string? errorValid);

            ////////////
            // Assert //
            ////////////

            Assert.False(result);
        }

        [Fact]
        public void Test_IsValid_EmptyFile_ReturnsFail()
        {
            /////////////
            // Arrange //
            /////////////

            var mockFile = new Mock<IFormFile>();
            mockFile.SetupGet(f => f.Length).Returns(0); 
            mockFile.SetupGet(f => f.FileName).Returns("empty.txt");
            IFileValidationParameters validationParameters = new AppParameters_Test() { };
            IFileValidator _fileValidator = new FileValidator(validationParameters);

            /////////
            // Act //
            /////////

            bool result = _fileValidator.IsValid(mockFile.Object, out string? errorValid);

            ////////////
            // Assert //
            ////////////

            Assert.False(result);
        }


        [Fact]

        public void Test_IsValid_CorrectValue_ReturnsSuccess()
        {
            /////////////
            // Arrange //
            /////////////

            var extensionFile = ".txt";
            var content = "Who are you?";
            var contentType = "text/plain";
            IFormFile formFile = CreateFormFile($"test{extensionFile}", content, contentType);
            IFileValidationParameters validationParameters = new AppParameters_Test() { };
            IFileValidator _fileValidator = new FileValidator(validationParameters);

            /////////
            // Act //
            /////////

            bool result = _fileValidator.IsValid(formFile, out string? errorValid);

            ////////////
            // Assert //
            ////////////

            Assert.True(result);
        }

        [Fact]
        public void Test_IsValid_IncorrectExtension_ReturnsFail()
        {
            /////////////
            // Arrange //
            /////////////

            var extensionFile = ".csv"; // Error
            var content = "Who are you?";
            var contentType = "text/plain";
            IFormFile formFile = CreateFormFile($"test{extensionFile}", content, contentType);
            IFileValidationParameters validationParameters = new AppParameters_Test() { };
            IFileValidator _fileValidator = new FileValidator(validationParameters);

            /////////
            // Act //
            /////////

            bool result = _fileValidator.IsValid(formFile, out string? errorValid);

            ////////////
            // Assert //
            ////////////

            Assert.False(result);
        }

        [Fact]
        public void Test_IsValid_IncorrectContentType_ReturnsFail()
        {
            /////////////
            // Arrange //
            /////////////
            
            var extensionFile = ".txt";
            var content = "Who are you?";
            var contentType = "text/csv"; // Error
            IFormFile formFile = CreateFormFile($"test{extensionFile}", content, contentType);
            IFileValidationParameters validationParameters = new AppParameters_Test() { };
            IFileValidator _fileValidator = new FileValidator(validationParameters);

            /////////
            // Act //
            /////////

            bool result = _fileValidator.IsValid(formFile, out string? errorValid);

            ////////////
            // Assert //
            ////////////

            Assert.False(result);
        }

        [Fact]
        public void Test_IsValid_IncorrectMaxSize_ReturnsFail()
        {
            /////////////
            // Arrange //
            /////////////
            
            var extensionFile = ".txt";
            var content = "Who are you?";
            var contentType = "text/plain"; // Error
            IFormFile formFile = CreateFormFile($"test{extensionFile}", content, contentType);
            IFileValidationParameters validationParameters = new AppParameters_Test() 
                                                                { MaxSizeFile = 10  }; // max size 10 byte
            IFileValidator _fileValidator = new FileValidator(validationParameters);

            /////////
            // Act //
            /////////

            bool result = _fileValidator.IsValid(formFile, out string? errorValid);

            ////////////
            // Assert //
            ////////////

            Assert.False(result);
        }
        
        /// <summary>
        /// Метод создания класса для передачи в валидатор
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private IFormFile CreateFormFile(string name, string content, string contentType)
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            var formFile = new FormFile(stream, 0, stream.Length, "file", name)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }
    }
}