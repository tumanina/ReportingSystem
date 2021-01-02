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
    public class ReportManagerTests
    {
        private readonly Guid _reportId = Guid.NewGuid();
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly string _reportToolGroupId = Guid.NewGuid().ToString();
        private readonly Guid _templateId = Guid.NewGuid();
        private readonly Guid _templateVersionId = Guid.NewGuid();
        private readonly string _fileName = "test_1.109.pbx";

        private Mock<IReportService> _reportServiceMock = new Mock<IReportService>();
        private Mock<ITemplateService> _templateServiceMock = new Mock<ITemplateService>();
        private Mock<IFileStorage> _fileStorageMock = new Mock<IFileStorage>();
        private Mock<IReportEngineTool> _reportEngineTool = new Mock<IReportEngineTool>();

        [TestCleanup]
        public void TestCleanUp()
        {
            _reportServiceMock.Invocations.Clear();
            _templateServiceMock.Invocations.Clear();
            _fileStorageMock.Invocations.Clear();
            _reportEngineTool.Invocations.Clear();
        }

        [TestMethod]
        public async Task Deploy_ReportToolImplementedTemplateExistsAndHasSpecifiedVersions_DeployInReportTool()
        {
            _reportServiceMock.Setup(m => m.GetReport(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestReport()));
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _reportEngineTool.Setup(m => m.ReportEngineTool).Returns(() => ReportEngineToolEnum.PowerBi);
            _reportEngineTool.Setup(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(new ReportEngineToolReportModel()));

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            _fileStorageMock.Setup(m => m.GetFile(It.IsAny<string>())).Returns(() => Task.FromResult(new FileModel { FileStream = fileContent }));

            var manager = new ReportManager(_reportServiceMock.Object, _templateServiceMock.Object, _fileStorageMock.Object, 
                new List<IReportEngineTool> { _reportEngineTool.Object });
            await manager.Deploy(ReportEngineToolEnum.PowerBi, _reportId, _templateVersionId);

            _reportServiceMock.Verify(m => m.GetReport(_reportId), Times.Once());
            _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
            _reportEngineTool.Verify(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _fileStorageMock.Verify(m => m.GetFile(_fileName), Times.Once());
        }

        [TestMethod]
        public async Task Deploy_ReportNotFound_ThrowsException()
        {
            _reportServiceMock.Setup(m => m.GetReport(It.IsAny<Guid>())).Returns(() => Task.FromResult((ReportModel)null));
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _reportEngineTool.Setup(m => m.ReportEngineTool).Returns(() => ReportEngineToolEnum.PowerBi);
            _reportEngineTool.Setup(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(new ReportEngineToolReportModel()));

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            _fileStorageMock.Setup(m => m.GetFile(It.IsAny<string>())).Returns(() => Task.FromResult(new FileModel { FileStream = fileContent }));

            var manager = new ReportManager(_reportServiceMock.Object, _templateServiceMock.Object, _fileStorageMock.Object, new List<IReportEngineTool> { _reportEngineTool.Object });
            
            try
            {
                await manager.Deploy(ReportEngineToolEnum.PowerBi, _reportId, _templateVersionId);
                Assert.Fail();
            }
            catch
            {
                _reportServiceMock.Verify(m => m.GetReport(_reportId), Times.Once());
                _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Never());
                _reportEngineTool.Verify(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                _fileStorageMock.Verify(m => m.GetFile(_fileName), Times.Never());
            }
        }

        [TestMethod]
        public async Task Deploy_TemplateVersionNotFound_ThrowsException()
        {
            _reportServiceMock.Setup(m => m.GetReport(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestReport()));
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _reportEngineTool.Setup(m => m.ReportEngineTool).Returns(() => ReportEngineToolEnum.PowerBi);
            _reportEngineTool.Setup(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(new ReportEngineToolReportModel()));

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            _fileStorageMock.Setup(m => m.GetFile(It.IsAny<string>())).Returns(() => Task.FromResult(new FileModel { FileStream = fileContent }));

            var manager = new ReportManager(_reportServiceMock.Object, _templateServiceMock.Object, _fileStorageMock.Object, new List<IReportEngineTool> { _reportEngineTool.Object });

            try
            {
                await manager.Deploy(ReportEngineToolEnum.PowerBi, _reportId, Guid.NewGuid());
                Assert.Fail();
            }
            catch
            {
                _reportServiceMock.Verify(m => m.GetReport(_reportId), Times.Once());
                _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
                _reportEngineTool.Verify(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                _fileStorageMock.Verify(m => m.GetFile(_fileName), Times.Never());
            }
        }

        [TestMethod]
        public async Task Deploy_ReportingToolIsNotImplemented_ThrowsException()
        {
            _reportServiceMock.Setup(m => m.GetReport(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestReport()));
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _reportEngineTool.Setup(m => m.ReportEngineTool).Returns(() => ReportEngineToolEnum.PowerBi);
            _reportEngineTool.Setup(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(new ReportEngineToolReportModel()));

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            _fileStorageMock.Setup(m => m.GetFile(It.IsAny<string>())).Returns(() => Task.FromResult(new FileModel { FileStream = fileContent }));

            var manager = new ReportManager(_reportServiceMock.Object, _templateServiceMock.Object, _fileStorageMock.Object, new List<IReportEngineTool>());

            try
            {
                await manager.Deploy(ReportEngineToolEnum.PowerBi, _reportId, _templateVersionId);
                Assert.Fail();
            }
            catch
            {
                _reportServiceMock.Verify(m => m.GetReport(_reportId), Times.Once());
                _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
                _reportEngineTool.Verify(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                _fileStorageMock.Verify(m => m.GetFile(_fileName), Times.Never());
            }
        }

        [TestMethod]
        public async Task Deploy_GroupDoesNotContainRelationToReportingToolGroups_ThrowsException()
        {
            _reportServiceMock.Setup(m => m.GetReport(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestReport(hasReportingToolGroupLink: false)));
            _templateServiceMock.Setup(m => m.GetTemplate(It.IsAny<Guid>())).Returns(() => Task.FromResult(GetTestTemplate()));
            _reportEngineTool.Setup(m => m.ReportEngineTool).Returns(() => ReportEngineToolEnum.PowerBi);
            _reportEngineTool.Setup(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => Task.FromResult(new ReportEngineToolReportModel()));

            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            _fileStorageMock.Setup(m => m.GetFile(It.IsAny<string>())).Returns(() => Task.FromResult(new FileModel { FileStream = fileContent }));

            var manager = new ReportManager(_reportServiceMock.Object, _templateServiceMock.Object, _fileStorageMock.Object, new List<IReportEngineTool> { _reportEngineTool.Object });

            try
            {
                await manager.Deploy(ReportEngineToolEnum.PowerBi, _reportId, _templateVersionId);
                Assert.Fail();
            }
            catch
            {
                _reportServiceMock.Verify(m => m.GetReport(_reportId), Times.Once());
                _templateServiceMock.Verify(m => m.GetTemplate(_templateId), Times.Once());
                _reportEngineTool.Verify(m => m.Deploy(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                _fileStorageMock.Verify(m => m.GetFile(_fileName), Times.Once());
            }
        }

        private ReportModel GetTestReport(bool hasReportingToolGroupLink = true)
        {
            return new ReportModel
            {
                Id = _reportId,
                TemplateId = _templateId,
                GroupId = _groupId,
                Group = new GroupModel
                {
                    Id = _groupId,
                    ReportEngineToolGroups = hasReportingToolGroupLink 
                    ? new List<ReportEngineToolGroupModel>
                        {
                            new ReportEngineToolGroupModel { ReportEngineTool = ReportEngineToolEnum.PowerBi, GroupId = _reportToolGroupId }
                        }
                    : new List<ReportEngineToolGroupModel>()
                }
            };
        }

        private TemplateModel GetTestTemplate()
        {
            return new TemplateModel
            {
                Id = _templateId,
                Versions = new List<TemplateVersionModel>
                    {
                        new TemplateVersionModel
                        {
                            Id = _templateVersionId,
                            Version = "1.108",
                            FileName = _fileName,
                            CreatedDate = DateTime.UtcNow.AddDays(-5) }
                    }
            };
        }
    }
}
