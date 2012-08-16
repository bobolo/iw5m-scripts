/* Roll The Dice
 * Ported from RTDv3 for MW2
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using InfinityScript;

namespace RollTheDice
{
    public class RollTheDice : BaseScript
    {
        public const int NumOfRolls = 3;
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
        }

        void RollTheDice_PlayerConnected(Entity obj)
        {
            obj.SpawnedPlayer += () => OnPlayerSpawned(obj);
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
        }

        public void OnPlayerSpawned(Entity player)
        {
            player.Call(33395);
            player.SetPerk("specialty_longersprint", false, true);
            player.SetPerk("specialty_fastreload", false, true);
            player.SetPerk("specialty_falldamage", false, true);
            player.Call("setKillstreaks", "none", "none", "none");
            DoRandom(player, null);
        }

        public void DoRandom(Entity player, int? desiredNumber)
        {
            int? roll = new Random().Next(NumOfRolls-1);
            if (desiredNumber != null)
                roll = desiredNumber;
            var rollname = "";
            switch (roll)
            {
                case 0:
                    rollname = "^2Extra Speed";
                    new Thread(() => Speed(player, 1.5)).Start();
                    break;
                case 1:
                    rollname = "XM25 Akimbo";
                    Stock(player, 99);
                    new Thread(() => Weapon(player, "xm25", "akimbo", null)).Start();
                    break;
                case 2:
                    rollname = "^2No Recoil";
                    player.Call("recoilscaleon", 0);
                    break;
            }
        }

        public void PrintRollNames(Entity player, List<string> names, int index)
        {
            
        }

        public void Speed(Entity player, double scale)
        {
            var loop = true;
            player.OnNotify("disconnect", entity => loop = false);
            player.OnNotify("death", entity => loop = false);
            while (loop)
            {
                player.Call("setmovespeedscale", new Parameter((float)scale));
                Thread.Sleep(50);
            }
        }

        public void Stock(Entity player, int amount)
        {
            var loop = true;
            player.OnNotify("disconnect", entity => loop = false);
            player.OnNotify("death", entity => loop = false);
            while (loop)
            {
                
            }
        }

        public void Vision(Entity player, string vision, bool thermal)
        {
            var loop = true;
            player.OnNotify("disconnect", entity => loop = false);
            player.OnNotify("death", entity => loop = false);
            while (loop)
            {

            }
        }

        public void Nades(Entity player, int amount)
        {
            var loop = true;
            player.OnNotify("disconnect", entity => loop = false);
            player.OnNotify("death", entity => loop = false);
            while (loop)
            {

            }
        }

        public void Weapon(Entity player, string weapon, string add, string weapon2)
        {
            var loop = true;
            player.OnNotify("disconnect", entity => loop = false);
            player.OnNotify("death", entity => loop = false);
            while (loop)
            {

            }
        }
    }
}
