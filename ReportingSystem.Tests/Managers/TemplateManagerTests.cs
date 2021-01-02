using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReportingSystem.Logic.Managers;
using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;

namespace ReportingSystem.Tests.Managers
{
    [TestClass]
    public class TemplateManagerTests
    {
        private readonly Guid _templateId = Guid.NewGuid();

        private Mock<ITemplateService> _templateServiceMock = new Mock<ITemplateService>();
        private Mock<IFileStorage> _fileStorageMock = new Mock<IFileStorage>();

        [TestCleanup]
        public void TestCleanUp()
        {
            _templateServiceMock.Invocations.Clear();
            _fileStorageMock.Invocations.Clear();
        }

        [TestMethod]
        public async Task UploadTemplateFile_TemplateHasVersionsWith3NumbersAfterPointsMinorChangesType_FileUploadedVersionIncrementCorrect()
        {
            var fileName = "";
            var version = "";
            var uploadedFileName = "";

            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Guid id, string templateVersion, string name) => { version = templateVersion; fileName = name; });

            var fileStorageMock = new Mock<IFileStorage>();
            fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback((string name, Stream content) => uploadedFileName = name);

            var manager = new TemplateManager(_templateServiceMock.Object, fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx", changesType: ChangesType.Minor);

            Assert.AreEqual(uploadedFileName, "test_1.109.pbx");
            Assert.AreEqual(fileName, "test_1.109.pbx");
            Assert.AreEqual(version, "1.109");
            _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
            _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once());
        }

        [TestMethod]
        public async Task UploadTemplateFile_TemplateHasVersionWith2NumbersAfterPointMinorChangesType_FileUploadedVersionIncrementCorrect()
        {
            var fileName = "";
            var version = "";
            var uploadedFileName = "";

            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate(twoNumbersAfterPoint: true)));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Guid id, string templateVersion, string name) => { version = templateVersion; fileName = name; });

            _fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback((string name, Stream content) => uploadedFileName = name);

            var manager = new TemplateManager(_templateServiceMock.Object, _fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx", changesType: ChangesType.Minor);

            Assert.AreEqual(uploadedFileName, "test_1.19.pbx");
            Assert.AreEqual(fileName, "test_1.19.pbx");
            Assert.AreEqual(version, "1.19");
            _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
            _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once());
        }

        [TestMethod]
        public async Task UploadTemplateFile_TemplateHasVersionsMagorChangesType_FileUploadedVersionIncrementCorrect()
        {
            var fileName = "";
            var version = "";
            var uploadedFileName = "";

            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Guid id, string templateVersion, string name) => { version = templateVersion; fileName = name; });

            _fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback((string name, Stream content) => uploadedFileName = name);

            var manager = new TemplateManager(_templateServiceMock.Object, _fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx", changesType: ChangesType.Major);

            Assert.AreEqual(uploadedFileName, "test_2.00.pbx");
            Assert.AreEqual(fileName, "test_2.00.pbx");
            Assert.AreEqual(version, "2.00");
            _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
            _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once());
        }

        [TestMethod]
        public async Task UploadTemplateFile_TemplateDoesNotHaveVersionsMinorChangesType_FileUploadedVersionIncrementCorrect()
        {
            var fileName = "";
            var version = "";
            var uploadedFileName = "";

            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>()))
                .Returns(() => Task.FromResult(new TemplateModel { Id = _templateId }));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Guid id, string templateVersion, string name) => { version = templateVersion; fileName = name; });
            _fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback((string name, Stream content) => uploadedFileName = name);

            var manager = new TemplateManager(_templateServiceMock.Object, _fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx", changesType: ChangesType.Minor);

            Assert.AreEqual(uploadedFileName, "test_1.00.pbx");
            Assert.AreEqual(fileName, "test_1.00.pbx");
            Assert.AreEqual(version, "1.00");
            _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
            _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once());
        }

        [TestMethod]
        public async Task UploadTemplateFile_TemplateHasVersionsSpecifiedVersion_FileUploadedWithSpecifiedVersion()
        {
            var fileName = "";
            var version = "";
            var uploadedFileName = "";

            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((Guid id, string templateVersion, string name) => { version = templateVersion; fileName = name; });
            _fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback((string name, Stream content) => uploadedFileName = name);

            var manager = new TemplateManager(_templateServiceMock.Object, _fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx", version: "1.2");

            Assert.AreEqual(uploadedFileName, "test_1.2.pbx");
            Assert.AreEqual(fileName, "test_1.2.pbx");
            Assert.AreEqual(version, "1.2");
            _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
            _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once());
        }

        [TestMethod]
        public async Task UploadTemplateFile_NoSpecifiedVersionOrChangeType_ThrowException()
        {
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
            _fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()));

            var manager = new TemplateManager(_templateServiceMock.Object, _fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            
            try
            {
                await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx");
                Assert.Fail();
            }
            catch
            {
                _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Never());
                _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                _fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never());
            }
        }

        public async Task UploadTemplateFile_TemplateNotFound_ThrowException()
        {
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult((TemplateModel)null));
            _templateServiceMock.Setup(m => m.AddTemplateVersion(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
            _fileStorageMock.Setup(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()));

            var manager = new TemplateManager(_templateServiceMock.Object, _fileStorageMock.Object);
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));

            try
            {
                await manager.UploadTemplateFile(_templateId, fileContent, "test.pbx", version: "1.2");
                Assert.Fail();
            }
            catch
            {
                _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
                _templateServiceMock.Verify(m => m.AddTemplateVersion(_templateId, It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                _fileStorageMock.Verify(m => m.UploadFile(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never());
            }
        }

        private TemplateModel GetTestTemplate(bool twoNumbersAfterPoint = false)
        {
            return new TemplateModel
            {
                Id = _templateId,
                Versions = new List<TemplateVersionModel>
                {
                    new TemplateVersionModel { Version = twoNumbersAfterPoint ? "1.17" : "1.107", CreatedDate = DateTime.UtcNow.AddDays(-5) },
                    new TemplateVersionModel { Version = twoNumbersAfterPoint ? "1.18" : "1.108", CreatedDate = DateTime.UtcNow.AddDays(-3) },
                    new TemplateVersionModel { Version = twoNumbersAfterPoint ? "1.16" : "1.106", CreatedDate = DateTime.UtcNow.AddDays(-8) }
                }
            };
        }
    }
}
