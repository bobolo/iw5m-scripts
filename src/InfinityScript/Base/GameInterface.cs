using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace InfinityScript
{
    public enum VariableType
    {
        Entity = 1,
        String = 2,
        Vector = 4,
        Float = 5,
        Integer = 6
    }

    static class GameInterface
    {
        [DllImport("iw5m.dll", EntryPoint = "GI_PushString", CharSet = CharSet.Ansi)]
        public static extern void Script_PushString(string str);

        [DllImport("iw5m.dll", EntryPoint = "GI_PushInt")]
        public static extern void Script_PushInt(int value);

        [DllImport("iw5m.dll", EntryPoint = "GI_PushFloat")]
        public static extern void Script_PushFloat(float value);

        [DllImport("iw5m.dll", EntryPoint = "GI_PushEntRef")]
        public static extern void Script_PushEntRef(int value);

        [DllImport("iw5m.dll", EntryPoint = "GI_PushVector")]
        public static extern void Script_PushVector(float x, float y, float z);

        [DllImport("iw5m.dll", EntryPoint = "GI_Call")]
        public static extern void Script_Call(int functionID, int entref, int numParams);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetType")]
        public static extern VariableType Script_GetType(int index);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetString", CharSet = CharSet.Ansi)]
        public static extern IntPtr Script_GetString(int index);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetInt")]
        public static extern int Script_GetInt(int index);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetFloat")]
        public static extern float Script_GetFloat(int index);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetEntRef")]
        public static extern int Script_GetEntRef(int index);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetVector")]
        public static extern void Script_GetVector(int index, out Vector3 vector);

        [DllImport("iw5m.dll", EntryPoint = "GI_NotifyNumArgs")]
        public static extern int Notify_NumArgs();

        [DllImport("iw5m.dll", EntryPoint = "GI_CleanReturnStack")]
        public static extern void Script_CleanReturnStack();

        [DllImport("iw5m.dll", EntryPoint = "GI_NotifyType", CharSet = CharSet.Ansi)]
        public static extern IntPtr Notify_Type();

        [DllImport("iw5m.dll", EntryPoint = "GI_GetObjectType")]
        public static extern int Script_GetObjectType(int obj);

        [DllImport("iw5m.dll", EntryPoint = "GI_ToEntRef")]
        public static extern int Script_ToEntRef(int obj);

        [DllImport("iw5m.dll", EntryPoint = "GI_GetField")]
        public static extern int Script_GetField(int entref, int field);

        [DllImport("iw5m.dll", EntryPoint = "GI_SetField")]
        public static extern int Script_SetField(int entref, int field);

        [DllImport("iw5m.dll", EntryPoint = "GI_NotifyNum", CharSet = CharSet.Ansi)]
        public static extern void Script_NotifyNum(int entref, string notify, int numArgs);

        [DllImport("iw5m.dll", EntryPoint = "GI_Print", CharSet = CharSet.Ansi)]
        public static extern void Print(string text);

        [DllImport("iw5m.dll", EntryPoint = "GI_SetDropItemEnabled")]
        public static extern bool SetDropItemEnabled(bool enabled);

        [DllImport("iw5m.dll", EntryPoint = "GI_Dvar_InfoString_Big")]
        public static extern IntPtr Dvar_InfoString_Big(int flag);

        // do not use
        [DllImport("iw5m.dll", EntryPoint = "GI_TempFunc")]
        public static extern void TempFunc();
    }
}
