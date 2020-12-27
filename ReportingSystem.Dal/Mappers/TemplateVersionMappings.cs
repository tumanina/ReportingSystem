using ReportingSystem.Dal.Entities;
using ReportingSystem.Shared.Models;

namespace ReportingSystem.Dal.Mappers
{
    public static class TemplateVersionMappings
    {
        public static TemplateVersionEntity Map(this TemplateVersionModel model)
        {
            var entity = new TemplateVersionEntity { Id = model.Id };
            model.Map(entity);
            return entity;
        }

        public static void Map(this TemplateVersionModel model, TemplateVersionEntity entity)
        {
            entity.TemplateId = model.TemplateId;
            entity.FileName = model.FileName;
            entity.Version = model.Version;
        }

        public static TemplateVersionModel Map(this TemplateVersionEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new TemplateVersionModel
            {
                Id = entity.Id,
                FileName = entity.FileName,
                Version = entity.Version,
                CreatedDate = entity.CreatedDate
            };
        }
    }
}
