﻿using EFT.InventoryLogic;
using SIT.Tarkov.Core;
using System.Collections.Generic;

namespace SIT.Core.Coop
{
    internal static class ItemAddressHelpers
    {
        private static string DICTNAMES_SlotItemAddressDescriptor { get; } = "sitad";
        private static string DICTNAMES_GridItemAddressDescriptor { get; } = "grad";
        private static string DICTNAMES_StackSlotItemAddressDescriptor { get; } = "ssad";

        public static void ConvertItemAddressToDescriptor(ItemAddress location
            , ref Dictionary<string, object> dictionary
            , out GridItemAddressDescriptor gridItemAddressDescriptor
            , out SlotItemAddressDescriptor slotItemAddressDescriptor
            , out StackSlotItemAddressDescriptor stackSlotItemAddressDescriptor
            )
        {
            gridItemAddressDescriptor = null;
            slotItemAddressDescriptor = null;
            stackSlotItemAddressDescriptor = null;

            if (location is GridItemAddress gridItemAddress)
            {
                gridItemAddressDescriptor = new();
                gridItemAddressDescriptor.Container = new ContainerDescriptor();
                gridItemAddressDescriptor.Container.ContainerId = location.Container.ID;
                gridItemAddressDescriptor.Container.ParentId = location.Container.ParentItem != null ? location.Container.ParentItem.Id : null;
                gridItemAddressDescriptor.LocationInGrid = gridItemAddress.LocationInGrid;
                dictionary.Add(DICTNAMES_GridItemAddressDescriptor, gridItemAddressDescriptor);
            }
            else if (location is SlotItemAddress slotItemAddress)
            {
                slotItemAddressDescriptor = new();
                slotItemAddressDescriptor.Container = new ContainerDescriptor();
                slotItemAddressDescriptor.Container.ContainerId = location.Container.ID;
                slotItemAddressDescriptor.Container.ParentId = location.Container.ParentItem != null ? location.Container.ParentItem.Id : null;

                dictionary.Add(DICTNAMES_SlotItemAddressDescriptor, slotItemAddressDescriptor);
            }
            else if (location is StackSlotItemAddress StackSlotItemAddress)
            {
                stackSlotItemAddressDescriptor = new();
                stackSlotItemAddressDescriptor.Container = new();
                stackSlotItemAddressDescriptor.Container.ContainerId = location.Container.ID;
                stackSlotItemAddressDescriptor.Container.ParentId = location.Container.ParentItem != null ? location.Container.ParentItem.Id : null;
                dictionary.Add(DICTNAMES_StackSlotItemAddressDescriptor, stackSlotItemAddressDescriptor);
            }
        }

        public static void ConvertDictionaryToAddress(
            Dictionary<string, object> dict,
            out GridItemAddressDescriptor gridItemAddressDescriptor,
            out SlotItemAddressDescriptor slotItemAddressDescriptor,
            out StackSlotItemAddressDescriptor stackSlotItemAddressDescriptor
            )
        {
            gridItemAddressDescriptor = null;
            slotItemAddressDescriptor = null;
            stackSlotItemAddressDescriptor = null;
            if (dict.ContainsKey(DICTNAMES_GridItemAddressDescriptor))
            {
                gridItemAddressDescriptor = PatchConstants.SITParseJson<GridItemAddressDescriptor>(dict[DICTNAMES_GridItemAddressDescriptor].ToString());
            }

            if (dict.ContainsKey(DICTNAMES_SlotItemAddressDescriptor))
            {
                slotItemAddressDescriptor = PatchConstants.SITParseJson<SlotItemAddressDescriptor>(dict[DICTNAMES_SlotItemAddressDescriptor].ToString());
            }

            if (dict.ContainsKey(DICTNAMES_StackSlotItemAddressDescriptor))
            {
                stackSlotItemAddressDescriptor = PatchConstants.SITParseJson<StackSlotItemAddressDescriptor>(dict[DICTNAMES_StackSlotItemAddressDescriptor].ToString());
            }
        }
    }
}
