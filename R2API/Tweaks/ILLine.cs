﻿using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using System;
using System.Diagnostics;
using System.Reflection;

namespace R2API.Tweaks {

    /// <summary>
    /// Show IL Instruction Line in stack traces instead of mangled data
    /// </summary>
    [R2APISubmodule]
    internal static class ILLine {
        private static ILHook? _hook;

        /// <summary>
        /// Return true if the submodule is loaded.
        /// </summary>
        public static bool Loaded {
            get => _loaded;
            internal set => _loaded = value;
        }

        private static bool _loaded;

        [R2APISubmoduleInit(Stage = InitStage.SetHooks)]
        internal static void SetHooks() {
            if (_hook != null) {
                return;
            }

            _hook = new ILHook(typeof(StackTrace).GetMethod("AddFrames", BindingFlags.Instance | BindingFlags.NonPublic), new ILContext.Manipulator(IlHook));

            Loaded = true;
        }

        [R2APISubmoduleInit(Stage = InitStage.UnsetHooks)]
        internal static void UnsetHooks() {
            if (_hook == null) {
                return;
            }

            _hook.Undo();
            _hook = null;
            Loaded = false;
        }

        [R2APISubmoduleInit(Stage = InitStage.LoadCheck)]
        internal static void ShouldLoad(out bool shouldload) {
            shouldload = true;
        }

        //replaces the call to GetFileLineNumber to a call to GetLineOrIL
        private static void IlHook(ILContext il) {
            var cursor = new ILCursor(il);
            cursor.GotoNext(
                x => x.MatchCallOrCallvirt(typeof(StackFrame).GetMethod("GetFileLineNumber", BindingFlags.Instance | BindingFlags.Public))
            );

            cursor.RemoveRange(2);
            cursor.EmitDelegate<Func<StackFrame, string>>(GetLineOrIL);
        }

        //first gets the debug line number (C#) and only if that is not available returns the IL offset (jit might change it a bit)
        private static string GetLineOrIL(StackFrame instace) {
            var line = instace.GetFileLineNumber();
            if (line != StackFrame.OFFSET_UNKNOWN && line != 0) {
                return line.ToString();
            }

            return "IL_" + instace.GetILOffset().ToString("X4");
        }
    }
}
