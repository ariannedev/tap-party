using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// Handles creation and joing of Unity Relay allocations
/// </summary>
public static class RelayHelper
{
    private const int MaxConnections = 8;

    public static async Task<(string joinCode, Allocation allocation)> CreateAllocationAsync()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"RelayHelper: Allocation created, join code: {joinCode}");
            return (joinCode, allocation);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RelayHelper: Failed to create allocation - {e.Message}");
            throw;
        }
    }

    public static async Task<JoinAllocation> JoinAllocationAsync(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log($"RelayHelper: Joined allocation with code: {joinCode}");
            return allocation;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"RelayHelper: Failed to join allocation - {e.Message}");
            throw;
        }
    }
}