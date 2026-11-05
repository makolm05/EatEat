using EatEat.Services.RewardAPI.Messages;

namespace EatEat.Services.RewardsAPI.Services.Interfaces
{
    public interface IRewardsService
    {
        Task UpdateRewards(RewardsMessage message);
    }
}