using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using InfinityScript;
using System.Xml.Serialization;

namespace iSnipe
{
    public class iSnipe : BaseScript
    {
        public Settings Settings;
        public List<string> PlayerStop = new List<string>();
        public iSnipe()
        {
            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                Log.Error("Something went wrong with reading the settings, using defaults.");
                Log.Write(LogLevel.Error, ex.ToString());
                Settings = new Settings();
            }

            PlayerConnected += entity =>
                                   {
                                       entity.SpawnedPlayer += () => OnPlayerSpawn(entity);
                                   };

            if (!Settings.EnableFallDamage)
            {
                Call("setdvar", "bg_fallDamageMinHeight", 9998);
                Call("setdvar", "bg_fallDamageMaxHeight", 9999);
            }
            if (!Settings.DamageDirectionIndicator)
                Call("setdvar", "cg_drawdamagedirection", 0);
            if (!Settings.ShowEnemyNames)
                Call("setdvar", "cg_drawcrosshairnames", 0);
            if (!Settings.DrawCrosshair)
                Call("setdvar", "cg_drawcrosshair", 0);
            if (!Settings.EnableKillstreaks)
                Call("setdvar", "scr_game_hardpoints", 0);
            if (!Settings.EnableKillcam)
                Call("setdvar", "scr_game_allowkillcam", 0);
            Call(42, "lowAmmoWarningNoAmmoColor1", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningNoAmmoColor2", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningColor1", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningColor2", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningNoReloadColor1", 0, 0, 0, 0);
            Call(42, "lowAmmoWarningNoReloadColor1", 0, 0, 0, 0);
            Call(42, "perk_weapSpreadMultiplier", 0.45f);
            Call(42, "cg_drawbreathhint", 0);
        }

        public void OnPlayerSpawn(Entity entity)
        {
            entity.SetField("maxhealth", Settings.PlayerMaxHealth);
            entity.Health = Settings.PlayerMaxHealth;
            entity.TakeAllWeapons();
            entity.GiveWeapon(Settings.MainWeapon);
            entity.AfterDelay(10, entity1 =>
                                      {
                                          entity.SwitchToWeapon(Settings.MainWeapon);
                                          entity.Call("givemaxammo", Settings.MainWeapon);
                                      });
            if (Settings.AntiHardscope)
            {
                //entity.OnInterval(50, entity1 => );
            }
            if (Settings.ThrowingKnife)
            {
                entity.Call("SetOffhandPrimaryClass", "throwingknife");
                entity.Call("setweaponammoclip", "throwingknife_mp", 1);
            }
            if (Settings.RemoveAllPerks)
            {
                entity.Call(33395);
            }
            if (Settings.DefaultSniperPerks)
            {
                entity.SetPerk("specialty_bulletpenetration", true, false);
                entity.SetPerk("specialty_longersprint", true, false);
                entity.SetPerk("specialty_fastreload", true, false);
                entity.SetPerk("specialty_holdbreathwhileads", true, false);
                entity.SetPerk("specialty_lightweight", true, false);
                entity.SetPerk("specialty_moredamage", true, false);
                entity.SetPerk("specialty_quickdraw", true, false);
                entity.SetPerk("specialty_quickswap", true, false);
                entity.SetPerk("specialty_fastmantle", true, false);
            }
            if (Settings.UseSecondary)
            {
                entity.GiveWeapon(Settings.SecondaryWeapon);
                if (!Settings.SecondaryHasAmmo)
                {
                    entity.Call("setweaponammoclip", Settings.SecondaryWeapon, 0);
                    entity.Call("setweaponammostock", Settings.SecondaryWeapon, 0);
                }
            }
            OnInterval(50, () =>
                               {
                                   if (entity.CurrentWeapon != Settings.MainWeapon)
                                       entity.SwitchToWeapon(Settings.MainWeapon);
                                   return true;
                               });
        }

        public void LoadSettings()
        {
            if (!File.Exists(@"scripts\isnipe\settings.xml"))
            {
                SaveSettings();
                return;
            }
            Settings = new Settings();
            using (var stream = new FileStream(@"scripts\isnipe\settings.xml", FileMode.Open))
                Settings = (Settings) (new XmlSerializer(Settings.GetType()).Deserialize(stream));
        }

        public void SaveSettings()
        {
            Settings = new Settings();
            Directory.CreateDirectory(@"scripts\isnipe");
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    new XmlSerializer(Settings.GetType()).Serialize(writer, Settings);
                    var xml = Encoding.UTF8.GetString(stream.ToArray());
                    File.WriteAllText(@"scripts\isnipe\settings.xml", xml);
                    Log.Info(@"Saved settings to scripts\isnipe\settings.xml");
                }
            }
        }
    }
}
