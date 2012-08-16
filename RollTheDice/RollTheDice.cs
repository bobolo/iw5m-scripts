/* Roll The Dice
 * Ported from RTDv3 for MW2
 */

using System;
using System.Runtime.InteropServices;
using InfinityScript;

namespace RollTheDice
{
    public class RollTheDice : BaseScript
    {
        [DllImport("iw5m.dll", EntryPoint = "GI_Print", CharSet = CharSet.Ansi)]
        public static extern void Print(string text);

        public RollTheDice()
        {
            PlayerConnected += RollTheDice_PlayerConnected;
            Call(42, "lowAmmoWarningNoAmmoColor1", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningNoAmmoColor2", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningColor1", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningColor2", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningNoReloadColor1", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningNoReloadColor1", 0, 0, 0, 0);
            Call(42, "g_deadchat", 1);
            Call(42, "painVisionTriggerHealth", 0);
            Tick += () => Print("Tick. " + DateTime.UtcNow.ToString());
        }

        void RollTheDice_PlayerConnected(Entity obj)
        {
            obj.SpawnedPlayer += () => OnPlayerSpawned(obj);
        }

        public void OnPlayerSpawned(Entity player)
        {
        }

        public void DoRandom(int desiredNumber)
        {
            
        }
    }
}
