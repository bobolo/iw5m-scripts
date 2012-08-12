using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfinityScript
{
    public abstract class BaseScript : Notifiable
    {
        #region events
        protected event Action Tick;
        protected event Action<Entity> PlayerConnecting;
        protected event Action<Entity> PlayerConnected;
        protected event Action<Entity> PlayerDisconnected;
        #endregion

        #region player list
        public List<Entity> Players { get; private set; }
        #endregion

        public BaseScript()
        {
            Players = new List<Entity>();

            OnNotify("connecting", entity =>
            {
                Players.Add(entity.As<Entity>());

                if (PlayerConnecting != null)
                {
                    PlayerConnecting(entity.As<Entity>());
                }
            });

            OnNotify("connected", entity =>
            {
                if (PlayerConnected != null)
                {
                    PlayerConnected(entity.As<Entity>());
                }
            });
        }

        #region virtual call functions
        public virtual void OnStartGameType() { }
        public virtual void OnPlayerDisconnect(Entity player)
        {
            Players.Remove(player);

            if (PlayerDisconnected != null)
            {
                PlayerDisconnected(player);
            }
        }

        public virtual void OnPlayerDamage(Entity player, Entity inflictor, Entity attacker, int damage, int dFlags, string mod, string weapon, Vector3 point, Vector3 dir, string hitLoc) { }
        public virtual void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc) { }

        public virtual void OnSay(Entity player, string name, string message) { }
        public virtual void OnExitLevel() { }
        #endregion

        #region onnotify
        public void OnNotify(string type, Action handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter, Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter, Parameter, Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter, Parameter, Parameter, Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter, Parameter, Parameter, Parameter, Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }

        public void OnNotify(string type, Action<Parameter, Parameter, Parameter, Parameter, Parameter, Parameter, Parameter, Parameter> handler)
        {
            OnNotifyInternal(type, handler);
        }
        #endregion

        #region timer adders
        public void OnInterval(int interval, Func<bool> function)
        {
            _timers.Add(new ScriptTimer()
            {
                func = function,
                triggerTime = 0,
                interval = interval
            });
        }

        public void AfterDelay(int delay, Action function)
        {
            _timers.Add(new ScriptTimer()
            {
                func = function,
                triggerTime = (_currentTime + delay),
                interval = -1
            });
        }
        #endregion

        #region runframe
        internal void RunFrame()
        {
            // handle tick
            if (Tick != null)
            {
                try
                {
                    Tick();
                }
                catch (Exception ex)
                {
                    Log.Write(LogLevel.Error, "Exception during Tick on script {0}: {1}", GetType().Name, ex.ToString());
                }
            }

            ProcessTimers();
            ProcessNotifications();
        }
        #endregion

        #region calls
        public void Call(string func, params Parameter[] parameters)
        {
            Function.SetEntRef(-1);
            Function.Call(func, parameters);
        }

        public void Call(int identifier, params Parameter[] parameters)
        {
            Function.SetEntRef(-1);
            Function.Call(identifier, parameters);
        }

        public TReturn Call<TReturn>(string func, params Parameter[] parameters)
        {
            Function.SetEntRef(-1);
            return Function.Call<TReturn>(func, parameters);
        }

        public TReturn Call<TReturn>(int identifier, params Parameter[] parameters)
        {
            Function.SetEntRef(-1);
            return Function.Call<TReturn>(identifier, parameters);
        }
        #endregion
    }
}
