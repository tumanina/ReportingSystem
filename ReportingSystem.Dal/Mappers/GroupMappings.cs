using ReportingSystem.Dal.Entities;
using ReportingSystem.Dal.Mappers;
using ReportingSystem.Shared.Models;
using System;

namespace ReportingSystem.Dal.Mappers
{
    public static class GroupMappings
    {
        public static GroupEntity Map(this GroupModel model)
        {
            var entity = new GroupEntity { Id = model.Id };
            model.Map(entity);
            return entity;
        }

        public static void Map(this GroupModel model, GroupEntity entity)
        {
            entity.Name = model.Name;
        }

        public static GroupModel Map(this GroupEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new GroupModel
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }
    }
}
