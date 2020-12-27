using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportingSystem.Shared.Interfaces.DalServices
{
	public interface IGroupDalService
	{
		Task<BaseResult<IEnumerable<GroupModel>>> GetGroups(string customerCode);
		Task<BaseResult<GroupModel>> GetGroup(string customerCode, long groupId);
		Task<BaseResult<GroupModel>> GetGroupByCode(string customerCode, string code);
		Task<BaseResult<long>> AddGroup(GroupModel model);
		Task<BaseResult> UpdateGroup(GroupModel model);
		Task<BaseResult> DeleteGroup(GroupModel model);
	}
}
