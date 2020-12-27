using ReportingSystem.Dal.Entities;
using ReportingSystem.Shared.Models;
using System.Linq;

namespace ReportingSystem.Dal.Mappers
{
    public static class TemplateMappings
    {
        public static TemplateEntity Map(this TemplateModel model)
        {
            var entity = new TemplateEntity { Id = model.Id };
            model.Map(entity);
            return entity;
        }

        public static void Map(this TemplateModel model, TemplateEntity entity)
        {
            entity.Name = model.Name;
        }

        public static TemplateModel Map(this TemplateEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new TemplateModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Versions = entity.Versions?.Select(v => v.Map())
            };
        }
    }
}
