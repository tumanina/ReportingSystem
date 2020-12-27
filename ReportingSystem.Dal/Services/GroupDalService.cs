using IreckonU.Framework.DependencyInjection.Shared;
using IreckonU.Framework.System.Result;
using IreckonU.Reports.Common.Interfaces.DalServices;
using IreckonU.Reports.Common.Models.Reports;
using IreckonU.Reports.Dal.DbContexts;
using IreckonU.Reports.Dal.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IreckonU.Reports.Dal.Services
{
    internal class GroupDalService : BaseDalService, IGroupDalService
    {
        public GroupDalService(IDependencyResolver dependencyResolver) : base(dependencyResolver)
        { 
        }

        public async Task<BaseResult<IEnumerable<GroupModel>>> GetGroups(string customerCode)
        {
            return await ExecuteAsync<ReportsDbContext, IEnumerable<GroupModel>>(async (context) =>
            {
                var groups = await context.Grous.Where(g => g.CustomerCode == customerCode).ToListAsync();
                return groups.Select(r => r.Map());
            });
        }

        public async Task<BaseResult<GroupModel>> GetGroup(string customerCode, long groupId)
        {
            return await ExecuteAsync<ReportsDbContext, GroupModel>(async (context) =>
            {
                var group = await context.Grous.Where(t => t.Id == groupId && t.CustomerCode == customerCode).FirstOrDefaultAsync();
                if (group == null)
                {
                    return null;
                }

                return group?.Map();
            });
        }

        public async Task<BaseResult<GroupModel>> GetGroupByCode(string customerCode, string code)
        {
            return await ExecuteAsync<ReportsDbContext, GroupModel>(async (context) =>
            {
                var group = await context.Grous.Where(t => t.Code == code && t.CustomerCode == customerCode).FirstOrDefaultAsync();
                if (group == null)
                {
                    return null;
                }

                return group?.Map();
            });
        }

        public async Task<BaseResult<long>> AddGroup(GroupModel model)
        {
            return await ExecuteAsync<ReportsDbContext, long>(async (context) =>
            {
                var entity = model.Map();
                context.Grous.Add(entity);

                await context.SaveChangesAsync();

                return entity.Id;
            });
        }

        public async Task<BaseResult> UpdateGroup(GroupModel model)
        {
            var entity = model.Map();
            return await ExecuteAsync<ReportsDbContext>(async (context) =>
            {
                entity.ChangedDateTimeUtc = DateTime.UtcNow;
                context.Update(entity);

                await context.SaveChangesAsync();
            });
        }

        public async Task<BaseResult> DeleteGroup(GroupModel model)
        {
            return await ExecuteAsync<ReportsDbContext>(async (context) =>
            {
                var entity = model.Map();
                context.Grous.Remove(entity);
                await context.SaveChangesAsync();
            });
        }
    }
}
