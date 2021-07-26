using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Cathei.BakingSheet.Tests
{
    public class CsvImportTests : IDisposable
    {
        private TestFileSystem _fileSystem;
        private Mock<ILogger> _loggerMock;
        private TestSheetContainer _container;
        private CsvSheetConverter _converter;

        public CsvImportTests()
        {
            _fileSystem = new TestFileSystem();
            _loggerMock = new Mock<ILogger>();
            _container = new TestSheetContainer(_loggerMock.Object);
            _converter = new CsvSheetConverter("testdata", TimeZoneInfo.Utc, fileSystem: _fileSystem);
        }

        public void Dispose()
        {
            _fileSystem.Dispose();
        }

        [Fact]
        public async Task TestImportEmptyCsv()
        {
            _fileSystem.SetTestData("testdata/Tests.csv", "Id");

            var result = await _container.Bake(_converter);

            Assert.True(result);
            Assert.Empty(_container.Tests);
            Assert.Equal(nameof(TestSheetContainer.Tests), _container.Tests.Name);
        }

        [Fact]
        public async Task TestImportMissingColumn()
        {
            _fileSystem.SetTestData("testdata/Tests.csv", "");

            var result = await _container.Bake(_converter);

            Assert.True(result);
            Assert.Empty(_container.Tests);
            Assert.Equal(nameof(TestSheetContainer.Tests), _container.Tests.Name);

            _loggerMock.VerifyLog(LogLevel.Error, "[Tests] First column must be named \"Id\"", Times.Once());
        }

        [Fact]
        public async Task TestImportConvertError()
        {
            _fileSystem.SetTestData("testdata/Types.csv", "Id,EnumColumn\nTest,WrongEnum");

            var result = await _container.Bake(_converter);

            Assert.True(result);
            Assert.Empty(_container.Types);

            _loggerMock.VerifyLog(LogLevel.Error, "[Types>Test>EnumColumn]", Times.Once());
        }
    }
}