﻿using SIT.Coop.Core.Web;
using SIT.Tarkov.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SIT.Core.Coop.Player
{
    public class Player_SetInventoryOpened_Patch : ModuleReplicationPatch
    {
        public override Type InstanceType => typeof(EFT.Player);
        public override string MethodName => "SetInventoryOpened";

        protected override MethodBase GetTargetMethod()
        {
            var method = PatchConstants.GetMethodForType(InstanceType, MethodName);

            return method;
        }

        public static Dictionary<string, bool> CallLocally
            = new Dictionary<string, bool>();

        private static List<long> ProcessedCalls
            = new List<long>();

        [PatchPrefix]
        public static bool PrePatch(ref EFT.Player __instance)
        {
            var result = false;
            if (CallLocally.TryGetValue(__instance.Profile.AccountId, out var expecting) && expecting)
                result = true;

            return result;
        }

        [PatchPostfix]
        public static void PostPatch(
           ref EFT.Player __instance,
            ref bool opened
            )
        {
            var player = __instance;

            if (CallLocally.TryGetValue(player.Profile.AccountId, out var expecting) && expecting)
            {
                CallLocally.Remove(player.Profile.AccountId);
                return;
            }

            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("t", DateTime.Now.Ticks);
            dictionary.Add("o", opened.ToString());
            dictionary.Add("m", "SetInventoryOpened");
            ServerCommunication.PostLocalPlayerData(player, dictionary);
            dictionary.Clear();
            dictionary = null;
        }


        public override void Replicated(EFT.Player player, Dictionary<string, object> dict)
        {
            var timestamp = long.Parse(dict["t"].ToString());

            if (!ProcessedCalls.Contains(timestamp))
                ProcessedCalls.Add(timestamp);
            else
            {
                ProcessedCalls.RemoveAll(x => x <= DateTime.Now.AddHours(-1).Ticks);
                return;
            }

            try
            {
                CallLocally.Add(player.Profile.AccountId, true);
                var opened = Convert.ToBoolean(dict["o"].ToString());
                player.SetInventoryOpened(opened);
            }
            catch (Exception e)
            {
                Logger.LogInfo(e);
            }
        }
    }
}
