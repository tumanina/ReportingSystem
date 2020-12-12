using ReportingSystem.Dal.Entities;
using ReportingSystem.Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReportingSystem.Dal.Mappers
{
    internal static class ActionMapping
    {
        public static IQueryable<ActionModel> Map(this IQueryable<ActionEntity> entities)
        {
            return entities.Select(MapEntityExpression);
        }

        public static readonly Expression<Func<ActionEntity, ActionModel>> MapEntityExpression = e =>
            new ActionModel
            {
                Name = e.Name,
            };

        public static ActionModel Map(this ActionEntity entity)
        {
            return entity == null ? null : new ActionModel
            {
                Name = entity.Name,
            };
        }

        public static ActionEntity Map(this ActionModel model)
        {
            return model == null ? null : new ActionEntity
            {
                Name = model.Name,
            };
        }

    }
}
