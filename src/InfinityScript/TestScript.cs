using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfinityScript
{
    public abstract class TestScript : BaseScript
    {
        public TestScript() : base()
        {
            /*OnNotify("weapon_fired", weaponName =>
            {
                GameInterface.Script_PushString((string)weaponName);
                GameInterface.Script_Call(362, 0, 1);
            });*/

            /*OnNotify("connected", player =>
            {
                Log.Write(LogLevel.Trace, "connected {0}", player);
                
                player.As<Entity>().Notified += (type, paras) =>
                {
                    Log.Write(LogLevel.Trace, "tyep {0}", type);
                };

                player.As<Entity>().OnNotify("spawned_player", playerEnt =>
                {
                    Log.Write(LogLevel.Trace, "spawned {0}", playerEnt);

                    Call("iprintlnbold", "0mgSniPeZZ by xXxNTAxXx");

                    playerEnt.Call("takeallweapons");
                    playerEnt.Call("giveweapon", "iw5_l96a1_mp_l96a1scope");
                    playerEnt.Call("setperk", "specialty_quickdraw");

                    Log.Write(LogLevel.Trace, "endSpawned {0}", playerEnt);
                });
            });*/

            /*PlayerConnected += new Action<Entity>(entity =>
            {
                Log.Write(LogLevel.Trace, "connected {0}", entity);

                entity.SpawnedPlayer += new Action(() =>
                {
                    Log.Write(LogLevel.Trace, "spawned {0} {1} {2} {3}", entity, entity.GetField<string>("classname"), entity.GetField<string>("sessionteam"), entity.GetField<Vector3>("origin"));

                    entity.TakeAllWeapons();
                    //entity.GiveWeapon("iw5_l96a1_mp_l96a1scope");
                    //entity.SwitchToWeaponImmediate("iw5_l96a1_mp_l96a1scope");
                    //entity.Call("givemaxammo", "iw5_l96a1_mp_l96a1scope");
                    entity.GiveWeapon("iw5_mp7_mp");
                    entity.SwitchToWeaponImmediate("iw5_mp7_mp");
                    entity.Call("givemaxammo", "iw5_mp7_mp");

                    entity.Call("iprintlnbold", "0mgMP7 by xXxNTAxXx");
                    entity.Call("setperk", "specialty_quickdraw", true, false);
                    entity.Call("setperk", "specialty_fastreload", true, false);
                });

                //var elem = HudElem.CreateFontString(entity, "default", 1.2f);
                //elem.X = 200;
                //elem.Y = 20;
                //elem.SetText("stuff. just ^2stuff^7.");
            });*/

            //Notified += TestScript_Notified;
            Tick += new Action(TestScript_Tick);
        }

        int _lastTime;

        void TestScript_Tick()
        {
            /*var time = Function.Call<int>(283);

            if ((time - _lastTime) > 10)
            {
                Function.Call(363, time);
                _lastTime = time;
            }*/
        }

        void TestScript_Notified(string arg1, Parameter[] arg2)
        {
            if (arg1 == "trigger" || arg1 == "weapon_fired" || arg1 == "touch")
            {
                return;
            }

            GameInterface.Script_PushString(arg1);
            GameInterface.Script_Call(363, 0, 1);
        }
    }
}
