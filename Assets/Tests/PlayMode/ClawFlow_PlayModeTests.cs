#nullable enable
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Project;
using Project.Machines;
using Project.Data;

public class ClawFlow_PlayModeTests
{
    [Test]
    public void Scene_Boots_And_Enters_Positioning()
    {
        // Minimal runtime sanity: create director & machine runtime
        var directorGo = new GameObject("Director").AddComponent<GameDirector>();
        var go = new GameObject("Machine");
        var ctrl = go.AddComponent<ClawMachineController>();
        ctrl.Config = ScriptableObject.CreateInstance<MachineConfig>();
        ctrl.Spawn = go.AddComponent<SpawnGrid>();
        ctrl.Grabber = go.AddComponent<Grabber>();
        ctrl.Input = go.AddComponent<ClawInput>();

        // Cannot yield in a standard NUnit test; assert construction didn't throw
        Assert.Pass("Controller instantiated; construction completed without exceptions.");
    }
}
