/* Roll The Dice
 * Ported from RTDv3 for MW2
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using InfinityScript;

namespace RollTheDice
{
    public class RollTheDice : BaseScript
    {
        public const int NumOfRolls = 6;
        public List<int> PlayerStop = new List<int>();
        public int tickcount = 0;
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
            Tick += RollTheDice_Tick;
        }

        void RollTheDice_Tick()
        {
            tickcount++;
            if (tickcount % 10 == 0)
            {
                foreach (var roll in from player in Players where PlayerStop.Contains(player.GetHashCode()) where player.HasField("rtd_rolls") select player.GetField<HudElem>("rtd_rolls"))
                {
                    Debug.WriteLine("Destroying elems for someone");
                    roll.Alpha = 0;
                }
            }
        }

        void RollTheDice_PlayerConnected(Entity obj)
        {
            obj.SpawnedPlayer += () => OnPlayerSpawned(obj);
            obj.OnNotify("disconnect", entity => PlayerStop.Add(obj.GetHashCode()));
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (PlayerStop.Contains(player.GetHashCode()))
                PlayerStop.Remove(player.GetHashCode());
            player.Call(33395);
            player.SetPerk("specialty_longersprint", false, true);
            player.SetPerk("specialty_fastreload", false, true);
            player.SetPerk("specialty_falldamage", false, true);
            //player.Call(@"maps\mp\gametypes\_class::setKillstreaks", "none", "none", "none");
            DoRandom(player, null);
        }

        public void DoRandom(Entity player, int? desiredNumber)
        {
            int? roll = new Random().Next(NumOfRolls);
            if (desiredNumber != null)
                roll = desiredNumber;
            var rollname = "";
            switch (roll)
            {
                case 0:
                    rollname = "^2Extra Speed";
                    OnInterval(50, () => Speed(player, 1.5));
                    break;
                case 1:
                    rollname = "XM25 Akimbo";
                    OnInterval(50, () => Stock(player, 99));
                    OnInterval(50, () => Weapon(player, "xm25", "akimbo", null));
                    break;
                case 2:
                    rollname = "^2No Recoil";
                    player.Call("player_recoilscaleon", 0);
                    break;
                case 3:
                    rollname = "^1You are a one hit kill";
                    player.SetField("maxhealth", 1);
                    player.Health = 1;
                    break;
                case 4:
                    rollname = "^1No ADS";
                    player.Call("allowads", false);
                    break;
                case 5:
                    rollname = "^2Triple HP";
                    player.SetField("maxhealth", 300);
                    player.Health = 300;
                    break;
            }
            PrintRollNames(player, rollname, 0, roll);
        }

        public void PrintRollNames(Entity player, string name, int index, int? roll)
        {
            var elem = HudElem.CreateFontString(player, "bigfixed", 0.6f);
            elem.SetPoint("RIGHT", "RIGHT", -90, 165 - ((index - 1)*13));
            elem.SetText(string.Format("[{0}] {1}", roll+1, name));
            player.SetField("rtd_rolls", new Parameter(elem));
            player.Call("iPrintLnBold", string.Format("You rolled {0} - {1}", roll+1, name));
            Call(334, string.Format("{0} rolled [{1}] - {2}", player.GetField<string>("name"), roll+1, name));
        }

        public bool Speed(Entity player, double scale)
        {
            if (PlayerStop.Contains(player.GetHashCode()))
                return false;
            player.Call("setmovespeedscale", new Parameter((float)scale));
            return true;
        }

        public bool Stock(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetHashCode()))
                return false;
            var wep = player.CurrentWeapon;
            player.Call("setweaponammostock", wep, amount);
            return true;
        }

        public bool Ammo(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetHashCode()))
                return false;
            var wep = player.CurrentWeapon;
            player.Call("setweaponammoclip", wep, amount);
            player.Call("setweaponammoclip", wep, amount, "left");
            player.Call("setweaponammoclip", wep, amount, "right");
            return true;
        }

        public void Vision(Entity player, string vision, bool thermal)
        {
        }

        public bool Nades(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetHashCode()))
                return false;
            var offhand = Call<string>("getcurrentoffhand");
            player.Call("setweaponammoclip", offhand, amount);
            player.Call("givemaxammo", offhand);
            return true;
        }

        public bool Weapon(Entity player, string weapon, string add, string weapon2)
        {
            if (PlayerStop.Contains(player.GetHashCode()))
                return false;
            player.TakeAllWeapons();
            player.Call("giveweapon", weapon, (add == "akimbo"));
            Ammo(player, 999);
            return true;
        }
    }
}
