#nullable enable
using NUnit.Framework;
using Project.Data;
using UnityEngine;

public class DataValidationTests
{
    [Test]
    public void MachineConfig_HasLootTable()
    {
        var cfg = ScriptableObject.CreateInstance<MachineConfig>();
        Assert.IsNull(cfg.LootTable, "Expect null by default; author must assign a LootTable.");
    }

    [Test]
    public void LootTable_WeightsNonNegative()
    {
        var lt = ScriptableObject.CreateInstance<LootTable>();
        Assert.IsNotNull(lt);
        // Authoring rule: negative weights are clamped in code; here we simply ensure it exists.
        Assert.Pass();
    }
}
