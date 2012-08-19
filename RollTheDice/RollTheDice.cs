/* Roll The Dice
 * Ported from RTDv3 for MW2
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using InfinityScript;

namespace RollTheDice
{
    public class RollTheDice : BaseScript
    {
        public const int NumOfRolls = 40;
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
            if (message.StartsWith("!kill"))
            {
                var dest = player.Origin;
                dest.Z = dest.Z - 1000;
                Call("magicbullet", "uav_strike_projectile_mp", player.Origin, dest, player);
            }
            if (message.StartsWith("!weapon"))
            {
                player.Call("iprintlnbold", player.CurrentWeapon);
            }
            if (message.StartsWith("!give "))
            {
                player.TakeAllWeapons();
                player.GiveWeapon(message.Split(' ')[1]);
            }
            if (message.StartsWith("!list"))
            {
                var list = player.Call<string[]>("getweaponslistall");
                foreach (var str in list)
                    player.Call("sayall", str);
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
                    OnInterval(100, () => Speed(player, 1.5));
                    break;
                case 1:
                    rollname = "^2Unlimited XM25";
                    OnInterval(100, () => Stock(player, 99));
                    OnInterval(100, () => Weapon(player, "xm25_mp", "", null));
                    break;
                case 2:
                    rollname = "^2No Recoil";
                    OnInterval(100, () => Recoil(player, 0f));
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
                    player.SetPerk("specialty_longersprint", true, false);
                    player.SetPerk("specialty_fastreload", true, false);
                    player.SetPerk("specialty_scavenger", true, false);
                    player.SetPerk("specialty_blindeye", true, false);
                    player.SetPerk("specialty_paint", true, false);
                    player.SetPerk("specialty_hardline", true, false);
                    player.SetPerk("specialty_coldblooded", true, false);
                    player.SetPerk("specialty_quickdraw", true, false);
                    player.SetPerk("specialty_twoprimaries", true, false);
                    player.SetPerk("specialty_assists", true, false);
                    player.SetPerk("_specialty_blastshield", true, false);
                    player.SetPerk("specialty_detectexplosive", true, false);
                    player.SetPerk("specialty_autospot", true, false);
                    player.SetPerk("specialty_bulletaccuracy", true, false);
                    player.SetPerk("specialty_quieter", true, false);
                    player.SetPerk("specialty_stalker", true, false);
                    break;
                case 7:
                    rollname = "^2Unlimited Grenades";
                    OnInterval(100, () => Nades(player, 99));
                    break;
                case 8:
                    rollname = "^2Go Get 'em Makarov";
                    OnInterval(100, () => Weapon(player, "iw5_mg36_mp_grip_xmags", "", null));
                    OnInterval(100, () => Stock(player, 999));
                    break;
                case 9:
                    rollname = "^1Darkness";
                    OnInterval(100, () => Vision(player, "cheat_chaplinnight", false));
                    break;
                case 10:
                    rollname = "^2Thermal Vision";
                    OnInterval(100, () => Vision(player, "ac130_thermal_mp", true));
                    break;
                case 11:
                    rollname = "^2Barrett Roll";
                    OnInterval(100, () => Recoil(player, 0f));
                    OnInterval(100, () => Stock(player, 99));
                    OnInterval(100, () => Weapon(player, "iw5_barrett_mp_xmags", "", null));
                    break;
                case 12:
                    rollname = "^1Negative";
                    OnInterval(100, () => Vision(player, "cheat_invert_contrast", false));
                    break;
                case 13:
                    rollname = "^2Knife Runner";
                    player.Call(33395);
                    player.SetPerk("specialty_longersprint", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.SetPerk("specialty_fastermelee", true, true);
                    OnInterval(100, () => Weapon(player, "iw5_44magnum_mp_tactical", "", null));
                    OnInterval(100, () => Speed(player, 1.2f));
                    OnInterval(100, () => Ammo(player, 0));
                    OnInterval(100, () => Stock(player, 0));
                    break;
                case 14:
                    rollname = "^1Turtle";
                    OnInterval(100, () => Speed(player, 0.4f));
                    break;
                case 15:
                    rollname = "^1Supermodel 1887";
                    player.Call(33395);
                    player.SetPerk("specialty_bulletaccuracy", true, true);
                    OnInterval(100, () => Weapon(player, "iw5_1887_mp", "", null));
                    break;
                case 16:
                    rollname = "^1Fallout";
                    OnInterval(100, () => Vision(player, "mpnuke", false));
                    break;
                case 17:
                    rollname = "^2Unlimited Ammo";
                    OnInterval(100, () => Ammo(player, 99));
                    OnInterval(100, () => Stock(player, 99));
                    break;
                case 18:
                    rollname = "^2Wallhack for 40 seconds";
                    player.Call("thermalvisionfofoverlayon");
                    player.AfterDelay(40000, entity =>
                                                  {
                                                     player.Call("thermalvisionfofoverlayoff");
                                                     player.Call("iprintlnbold", "Wallhack Off");
                                                 });
                    break;
                case 19:
                    rollname = "^2Double HP and roll again!";
                    player.SetField("maxhealth", player.Health*2);
                    player.Health = player.Health*2;
                    player.AfterDelay(2000, entity => DoRandom(player, null));
                    break;
                case 20:
                    rollname = "^2Godmode for 15 seconds";
                    player.Health = -1;
                    player.AfterDelay(15000, entity =>
                                                 {
                                                     player.Call("iprintlnbold", "Godmode Off");
                                                     player.Health = player.GetField<int>("maxhealth");
                                                     player.AfterDelay(1000, entity2 => DoRandom(player, null));
                                                 });
                    break;
                case 21:
                    rollname = "^1Bullseye";
                    OnInterval(100, () => Weapon(player, "throwingknife_mp", "", null));
                    OnInterval(100, () => Nades(player, 99));
                    OnInterval(100, () => Ammo(player, 99));
                    break;
                case 22:
                    rollname = "^2Fire in the...";
                    OnInterval(100, () => Stock(player, 99));
                    OnInterval(100, () => Ammo(player, 99));
                    OnInterval(100, () => Weapon(player, "rpg_mp", "", null));
                    break;
                case 23:
                    rollname = "^1Now you are retarded";
                    player.Call("allowads", false);
                    player.Call("allowsprint", false);
                    player.Call("allowjump", false);
                    break;
                case 24:
                    rollname = "AZUMOOB's Sub Setup";
                    player.TakeAllWeapons();
                    player.Call(33395);
                    player.SetPerk("specialty_fastermelee", true, true);
                    player.SetPerk("specialty_bulletaccuracy", true, true);
                    player.SetPerk("specialty_bulletdamage", true, true);
                    player.GiveWeapon("iw5_ump45_mp_silencer_xmags");
                    player.GiveWeapon("iw5_aa12_mp_xmags_grip_akimbo");
                    player.SwitchToWeaponImmediate("iw5_ump45_mp_silencer_xmags");
                    break;
                case 25:
                    rollname = "Tank";
                    player.SetPerk("specialty_fastermelee", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.TakeAllWeapons();
                    player.GiveWeapon("riotshield_mp");
                    player.SwitchToWeaponImmediate("riotshield_mp");
                    player.Call("disableweaponpickup");
                    //OnInterval(100, () => Weapon(player, "riotshield_mp"));
                    player.AfterDelay(150,
                                      entity =>
                                      player.Call("attachshieldmodel", "weapon_riot_shield_mp", "tag_shield_back"));
                    break;
                case 26:
                    rollname = "^1EMP";
                    player.Call("setempjammed", true);
                    break;
                case 27:
                    rollname = "Morpheus";
                    player.Call(33395);
                    player.SetPerk("specialty_longersprint", true, true);
                    player.SetPerk("specialty_lightweight", true, true);
                    player.SetPerk("specialty_quieter", true, true);
                    OnInterval(100, () => Weapon(player, "iw5_mp5_mp_rof"));
                    player.AfterDelay(150, entity => player.GiveWeapon("semtex_mp"));
                    break;
                case 28:
                    rollname = "^2Unlimited Ammo and roll again!";
                    OnInterval(100, () => Nades(player, 99));
                    OnInterval(100, () => Ammo(player, 99));
                    player.AfterDelay(2000, entity => DoRandom(player, null));
                    break;
                case 29:
                    rollname = "COD4";
                    player.SetPerk("specialty_bulletdamage", true, true);
                    player.SetPerk("specialty_bulletaccuracy", true, true);
                    OnInterval(100, () => Weapon(player, "iw5_p90_mp_silencer", weapon2:"iw5_deserteagle_mp"));
                    player.AfterDelay(150, entity => player.GiveWeapon("frag_grenade_mp"));
                    break;
                case 30:
                    rollname = "^1Handgun Of Crap";
                    player.TakeAllWeapons();
                    player.Call("disableweaponpickup");
                    player.GiveWeapon("iw5_usp45_mp_akimbo");
                    player.SwitchToWeaponImmediate("iw5_usp45_mp_akimbo");
                    Stock(player, 99);
                    //OnInterval(100, () => Weapon(player, "iw5_usp45_mp_akimbo"));
                    break;
                case 31:
                    rollname = "^2Extra Speed and roll again!";
                    OnInterval(100, () => Speed(player, 1.5));
                    player.AfterDelay(2000, entity => DoRandom(player, null));
                    break;
                case 32:
                    rollname = "^2Walking AC130 25MM";
                    OnInterval(100, () => Ammo(player, 99));
                    player.TakeAllWeapons();
                    player.GiveWeapon("ac130_25mm_mp");
                    player.SwitchToWeaponImmediate("ac130_25mm_mp");
                    player.Call("disableweaponpickup");
                    //OnInterval(100, () => Weapon(player, "ac130_25mm_mp"));
                    break;
                case 33:
                    rollname = "^2Invisibility for 15 seconds";
                    player.Call("hide");
                    player.AfterDelay(15000, entity =>
                                                 {
                                                     player.Call("iprintlnbold", "Invisibility Off");
                                                     player.Call("show");
                                                     player.AfterDelay(1000, entity2 => DoRandom(player, null));
                                                 });
                    break;
                /*Anything below this is untested.*/
                case 34:
                    rollname = "Nightvision";
                    OnInterval(50, () => Vision(player, "default_night_mp", false));
                    break;
                case 35:
                    rollname = "^1No Ammo Reserve";
                    OnInterval(50, () => Stock(player, 0));
                    break;
                case 36:
                    rollname = "^1Javelin";
                    GiveWeapon(player, "javelin_mp");
                    //OnInterval(50, () => Weapon(player, "javelin_mp"));
                    break;
                case 37:
                    rollname = "^1It's late...";
                    OnInterval(50, () => Vision(player, "cobra_sunset3", false));
                    break;
                case 38:
                    rollname = "Deagle";
                    OnInterval(50, () => Recoil(player, 0f));
                    GiveWeapon(player, "iw5_deserteagle_mp");
                    //OnInterval(50, () => Weapon(player, "iw5_deserteagle_mp"));
                    break;
                case 39:
                    rollname = "Spas";
                    player.SetPerk("specialty_bulletaccuracy", true, true);
                    player.SetPerk("specialty_bulletdamage", true, true);
                    OnInterval(50, () => Ammo(player, 99));
                    GiveWeapon(player, "iw5_deserteagle_mp");
                    //OnInterval(50, () => Weapon(player, "iw5_spas12_mp_fmj_grip"));
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
            if (thermal)
                player.Call("ThermalVisionOn");
            else
                player.Call("visionsetnakedforplayer", vision, 1);
            return true;
        }

        public bool Nades(Entity player, int amount)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            var offhand = player.Call<string>("getcurrentoffhand");
            player.Call("setweaponammoclip", offhand, amount);
            player.Call("givemaxammo", offhand);
            return true;
        }

        public bool Weapon(Entity player, string weapon, string add = "", string weapon2 = "", bool strip = true)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            if (player.CurrentWeapon.Contains(GetWeaponName(weapon)) || (weapon2 != null && player.CurrentWeapon.Contains(GetWeaponName(weapon2))))
                return true;
            if (strip)
                player.TakeAllWeapons();
            if (add == "akimbo")
                weapon = weapon + "_akimbo";
            player.GiveWeapon(weapon);
            player.SwitchToWeapon(weapon);
            if (!string.IsNullOrEmpty(weapon2))
                player.GiveWeapon(weapon2);
            player.Call("disableweaponpickup");
            Stock(player, 999);
            return true;
        }

        public void GiveWeapon(Entity player, params string[] weapons)
        {
            player.TakeAllWeapons();
            foreach (var weapon in weapons)
                player.GiveWeapon(weapon);
            player.SwitchToWeaponImmediate(weapons[0]);
            player.Call("disableweaponpickup");
            Stock(player, 99);
        }

        public bool Recoil(Entity player, float scale)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            player.Call("recoilscaleon", scale);
            return true;
        }

        public bool Attach(Entity player, string model, string tag)
        {
            if (PlayerStop.Contains(player.GetField<string>("name")))
                return false;
            player.Call("attachshieldmodel", model, tag);
            return true;
        }

        public static string GetWeaponName(string name)
        {
            var parts = name.Split('_');
            return parts[0] == "iw5" ? parts[1] : parts[0];
        }
    }
}
