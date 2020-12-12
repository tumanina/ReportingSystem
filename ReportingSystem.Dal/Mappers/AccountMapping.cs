using ReportingSystem.Dal.Entities;
using ReportingSystem.Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ReportingSystem.Dal.Mappers
{
    internal static class AccountMapping
    {
        public static IQueryable<AccountModel> Map(this IQueryable<AccountEntity> entities)
        {
            return entities.Select(MapEntityExpression);
        }

        public static readonly Expression<Func<AccountEntity, AccountModel>> MapEntityExpression = e =>
            new AccountModel
            {
                Id = e.Id,
                Username = e.Username,
                Password = e.Password
            };

        public static AccountModel Map(this AccountEntity entity)
        {
            return entity == null ? null : new AccountModel
            {
                Id = entity.Id,
                Username = entity.Username,
                Password = entity.Password,
                Actions = entity.Actions?.Select(c => c.Action.Map())
            };
        }

        public static AccountEntity Map(this AccountModel model)
        {
            return model == null ? null : new AccountEntity
            {
                Id = model.Id,

                Username = model.Username,
                Password = model.Password
            };
        }

    }
}
