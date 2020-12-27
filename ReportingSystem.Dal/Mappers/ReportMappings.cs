using ReportingSystem.Dal.Entities;
using ReportingSystem.Shared.Models;
using System;

namespace ReportingSystem.Dal.Mappers
{
    public static class ReportMappings
    {
        public static ReportEntity Map(this ReportModel model)
        {
            var entity = new ReportEntity { Id = model.Id };
            model.Map(entity);
            return entity;
        }

        public static void Map(this ReportModel model, ReportEntity entity)
        {
            entity.Name = model.Name;
            entity.TemplateId = model.TemplateId;
            entity.GroupId = model.TemplateId;

            if (entity.Id != Guid.Empty)
            {
                entity.Id = model.Id;
            }
        }

        public static ReportModel Map(this ReportEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new ReportModel
            {
                Id = entity.Id,
                Name = entity.Name,
                GroupId = entity.GroupId,
                TemplateId = entity.TemplateId,
                Group = entity.Group?.Map(),
                Template = entity.Template?.Map(),
            };
        }
    }
}
