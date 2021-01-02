using ReportingSystem.Shared.Enums;
using ReportingSystem.Shared.Interfaces;
using ReportingSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReportingSystem.Logic.Managers
{
    public class TemplateManager : ITemplateManager
    {
        private readonly ITemplateService _templateService;
        private readonly IFileStorage _fileStorage;

        public TemplateManager(ITemplateService templateService, IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
            _templateService = templateService;
        }

        public async Task<IEnumerable<TemplateModel>> GetTemplates()
        {
            return await _templateService.GetTemplates();
        }

        public async Task UploadTemplateFile(Guid templateId, Stream fileContent, string fileName, string version = null, ChangesType? changesType = null)
        {
            if (string.IsNullOrEmpty(version) && !changesType.HasValue)
            {
                throw new Exception($"Both Version and ChangesType are null or empty.");
            }
            
            var template = await _templateService.GetTemplate(templateId);
            if (template == null)
            {
                throw new Exception($"Template {templateId} does not found.");
            }

            if (string.IsNullOrEmpty(version))
            {
                var lastVersion = template.Versions.OrderBy(v => v.CreatedDate).LastOrDefault();

                if (lastVersion == null)
                {
                    version = "1.00";
                }
                else
                {
                    if (decimal.TryParse(lastVersion.Version, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal currentVersion))
                    {
                        version = IncrementVersionNumber(currentVersion, changesType.Value);
                    }
                    else
                    {
                        throw new Exception($"Template last version must be Decimal, {lastVersion.Version} is not valid value.");
                    }
                }
            }

            var fileExtension = Path.GetExtension(fileName);
            var fileNameWithVersion = fileName.Replace(fileExtension, $"_{version.Trim()}{fileExtension}");

            await _fileStorage.UploadFile(fileNameWithVersion, fileContent);

            await _templateService.AddTemplateVersion(templateId, version, fileNameWithVersion);
        }


        private string IncrementVersionNumber(decimal currentVersion, ChangesType reportChangesType)
        {
            var version = currentVersion;
            switch (reportChangesType)
            {
                case ChangesType.Minor:
                    var minorVersion = currentVersion % 1 * (decimal)Math.Pow(10, CountDecimalDigits(currentVersion));
                    version = minorVersion < 99 ? currentVersion + 0.01m : (minorVersion == 99 ? Math.Floor(currentVersion) + 0.100m : currentVersion + 0.001m);
                    break;
                case ChangesType.Major:
                    version = Math.Floor(currentVersion) + 1.00m;
                    break;
            }

            return version.ToString(CultureInfo.InvariantCulture);
        }

        private int CountDecimalDigits(decimal n)
        {
            return n.ToString(CultureInfo.InvariantCulture)
                    .SkipWhile(c => c != '.')
                    .Skip(1)
                    .Count();
        }
    }
}
