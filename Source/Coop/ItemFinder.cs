﻿using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using SIT.Core.Coop.ItemControllerPatches;
using SIT.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SIT.Core.Coop
{
    public static class ItemFinder
    {
        public static bool TryFindItemOnPlayer(EFT.Player player, string templateId, string itemId, out EFT.InventoryLogic.Item item)
        {
            item = null;

            if (!string.IsNullOrEmpty(templateId))
            {
                var allItemsOfTemplate = player.Profile.Inventory.GetAllItemByTemplate(templateId);

                if (!allItemsOfTemplate.Any())
                    return false;

                item = allItemsOfTemplate.FirstOrDefault(x => x.Id == itemId);
            }
            else
            {
                item = player.Profile.Inventory.AllPlayerItems.FirstOrDefault(x => x.Id == itemId);
            }
            return item != null;
        }

        public static bool TryFindItemInWorld(string itemId, out EFT.InventoryLogic.Item item)
        {
            item = null;

            var itemFindResult = Singleton<GameWorld>.Instance.FindItemById(itemId);
            if (itemFindResult.Succeeded)
            {
                item = itemFindResult.Value;
            }

            return item != null;

        }

        public static bool TryFindItem(string itemId, out EFT.InventoryLogic.Item item)
        {
            if (TryFindItemInWorld(itemId, out item))
                return item != null;
            else
            {
                var coopGC = CoopGameComponent.GetCoopGameComponent();
                var players = coopGC.Players;
                if (players == null)
                    return false;

                foreach (var player in players)
                {
                    if (TryFindItemOnPlayer(player.Value, null, itemId, out item))
                        return true;
                }
            }

            return false;
        }

        public static IEnumerable<object> GetItemComponentsInChildren(Item item, Type componentType)
        {
            MethodInfo method = ReflectionHelpers.GetMethodForType(item.GetType(), "GetItemComponentsInChildren");
            MethodInfo generic = method.MakeGenericMethod(componentType);
            var itemComponent = (IEnumerable<object>)generic.Invoke(item, null);
            return itemComponent;
        }

        public static EFT.Player.PlayerInventoryController GetPlayerInventoryController(EFT.Player player)
        {
            var inventoryController = ReflectionHelpers.GetFieldFromTypeByFieldType(player.GetType(), typeof(InventoryController)).GetValue(player) as EFT.Player.PlayerInventoryController;
            return inventoryController;
        }

        public static bool TryFindItemController(string controllerId, ref ItemController itemController)
        {
            // Find in World
            var icById = Singleton<GameWorld>.Instance.FindControllerById(controllerId);
            if (icById != null)
            {
                itemController = icById;
                return true;
            }

            // Find a Player
            var coopGC = CoopGameComponent.GetCoopGameComponent();
            if (coopGC == null)
            {
                return false;
            }

            if (!coopGC.Players.ContainsKey(controllerId))
                return false;

            var plInvC = GetPlayerInventoryController(coopGC.Players[controllerId]);
            if (plInvC != null)
            {
                itemController = plInvC;
                return true;    
            }

            return itemController != null;

        }
    }
}
