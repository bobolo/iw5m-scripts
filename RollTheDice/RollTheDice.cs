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
        public const int NumOfRolls = 11;
        public List<string> PlayerStop = new List<string>();
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
                tickcount = 0;
            }
        }

        void RollTheDice_PlayerConnected(Entity obj)
        {
            obj.SpawnedPlayer += () => OnPlayerSpawned(obj);
            obj.OnNotify("disconnect", entity => PlayerStop.Add(obj.GetField<string>("name")));
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            PlayerStop.Add(player.GetField<string>("name"));
        }

        public override void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc)
        {
        }

        public override void OnSay(Entity player, string name, string message)
        {
#if DEBUG
            if (message.StartsWith("!roll "))
            {
                PlayerStop.Add(player.GetField<string>("name"));
                Thread.Sleep(500);
                PlayerStop.Remove(player.GetField<string>("name"));
                DoRandom(player, int.Parse(message.Split(' ')[1]));
            }
#endif
        }

        public void OnPlayerSpawned(Entity player)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                PlayerStop.Remove(player.GetField<string>("name"));
            ResetPlayer(player);
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
                    rollname = "^2Unlimited XM25";
                    OnInterval(50, () => Stock(player, 99));
                    OnInterval(50, () => Weapon(player, "xm25_mp", "akimbo", null));
                    break;
                case 2:
                    rollname = "^2No Recoil";
                    OnInterval(50, () => Recoil(player, 0f));
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
                    player.SetField("maxhealth", player.Health*3);
                    player.Health = player.Health*3;
                    break;
                case 6:
                    rollname = "^2All Perks";
                    player.SetPerk("specialty__longersprint", true, false);
                    player.SetPerk("specialty__fastreload", true, false);
                    player.SetPerk("specialty__scavenger", true, false);
                    player.SetPerk("specialty__blindeye", true, false);
                    player.SetPerk("specialty__paint", true, false);
                    player.SetPerk("specialty__hardline", true, false);
                    player.SetPerk("specialty__coldblooded", true, false);
                    player.SetPerk("specialty__quickdraw", true, false);
                    player.SetPerk("specialty__twoprimaries", true, false);
                    player.SetPerk("specialty__assists", true, false);
                    player.SetPerk("_specialty__blastshield", true, false);
                    player.SetPerk("specialty__detectexplosive", true, false);
                    player.SetPerk("specialty__autospot", true, false);
                    player.SetPerk("specialty__bulletaccuracy", true, false);
                    player.SetPerk("specialty__quieter", true, false);
                    player.SetPerk("specialty__stalker", true, false);
                    break;
                case 7:
                    rollname = "^2Unlimited Frag Grenades";
                    OnInterval(50, () => Nades(player, 99));
                    break;
                case 8:
                    rollname = "^2Go Get 'em Makarov";
                    OnInterval(50, () => Weapon(player, "iw5_mg36_mp_grip_xmags", "", null));
                    break;
                case 9:
                    rollname = "^1Darkness";
                    OnInterval(50, () => Vision(player, "cheat_chaplinnight", false));
                    break;
                case 10:
                    rollname = "^2Thermal vision";
                    OnInterval(50, () => Vision(player, "thermal_mp", true));
                    break;
            }
            PrintRollNames(player, rollname, 0, roll);
        }

        public void PrintRollNames(Entity player, string name, int index, int? roll)
        {
            HudElem elem = player.HasField("rtd_rolls") ? player.GetField<HudElem>("rtd_rolls") : HudElem.CreateFontString(player, "bigfixed", 0.6f);
            elem.SetPoint("RIGHT", "RIGHT", -90, 165 - ((index - 1)*13));
            elem.SetText(string.Format("[{0}] {1}", roll+1, name));
            player.SetField("rtd_rolls", new Parameter(elem));
            player.Call("iPrintLnBold", string.Format("You rolled {0} - {1}", roll+1, name));
            Call(334, string.Format("{0} rolled [{1}] - {2}", player.GetField<string>("name"), roll+1, name));
        }

        public void ResetPlayer(Entity player)
        {
            player.Call("setmovespeedscale", 1f);
        }

        public bool Speed(Entity player, double scale)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            player.Call("setmovespeedscale", new Parameter((float)scale));
            return true;
        }

        public bool Stock(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            var wep = player.CurrentWeapon;
            player.Call("setweaponammostock", wep, amount);
            return true;
        }

        public bool Ammo(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            var wep = player.CurrentWeapon;
            player.Call("setweaponammoclip", wep, amount);
            player.Call("setweaponammoclip", wep, amount, "left");
            player.Call("setweaponammoclip", wep, amount, "right");
            return true;
        }

        public bool Vision(Entity player, string vision, bool thermal)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            player.Call(thermal ? "visionsetthermalforplayer" : "visionsetnakedforplayer", vision, 1);
            return true;
        }

        public bool Nades(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            var offhand = Call<string>("getcurrentoffhand");
            if (offhand != "frag_grenade_mp")
            {
                player.TakeWeapon(offhand);
                player.GiveWeapon("frag_grenade_mp");
            }
            player.Call("setweaponammoclip", offhand, amount);
            player.Call("givemaxammo", offhand);
            return true;
        }

        public bool Weapon(Entity player, string weapon, string add, string weapon2)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            if (player.CurrentWeapon.Contains(weapon))
                return true;
            player.TakeAllWeapons();
            player.Call("giveweapon", weapon, 8, (add == "akimbo"));
            player.SwitchToWeaponImmediate(weapon);
            Ammo(player, 999);
            return true;
        }

        public bool Recoil(Entity player, float scale)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            player.Call("recoilscaleon", scale);
            return true;
        }
    }
}
