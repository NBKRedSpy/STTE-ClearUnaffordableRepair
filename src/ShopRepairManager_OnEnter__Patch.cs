using HarmonyLib;
using RST;
using RST.PlaymakerAction;
using RST.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace ClearUnaffordableRepair
{


    [HarmonyPatch(typeof(ShopRepairManager), nameof(ShopRepairManager.OnEnter))]
    public static class ShopRepairManager_OnEnter__Patch
    {
        private static bool IsInited = false;

        private static ClickableObject ClearAllRightClick = null;

        /// <summary>
        /// The reference to the game's shop repair instance.  I don't think this will ever be deallocated, but just in case.
        /// </summary>
        private static WeakReference<ShopRepairManager> ShopRepairManagerRef = null;

        public static void Postfix(ShopRepairManager __instance)
        {
            if(IsInited) return;

            ShopRepairManagerRef = new WeakReference<ShopRepairManager>(__instance);

            ClearAllRightClick = ((Button) __instance.unmarkAllDamageButton.Value).gameObject.AddComponent<ClickableObject>();
            ClearAllRightClick.OnRightClick += ClearAllRightClick_OnRightClick;

            IsInited = true;

        }

        private static void ClearAllRightClick_OnRightClick(object sender, UnityEngine.EventSystems.PointerEventData e)
        {
            ShopRepairManager shopRepair;
            if (!ShopRepairManagerRef.TryGetTarget(out shopRepair)) return;

            if (shopRepair.sh == null) return;

            //there is only ever one player ship.  Maybe left over from removed game concepts.
            PlayerData playerData = shopRepair.pd;
            Ship ship = shopRepair.pd.Ships[0];

            //The damage points that can be repaired based on the player's available credits
            int availableRepairPoints = playerData.Credits / shopRepair.sh.healthPointPrice;

            if (availableRepairPoints == 0) return;

            IOrderedEnumerable<DamageToken> damages = ship.DamageTokens
                .Where(x => x.isMarked == true)
                .OrderBy(x => x.IsCritical)
                .ThenBy(x => x.IsCrewRepairable == false)
                .ThenBy(x => x.damage);

            foreach (DamageToken damage in damages)
            {
                if (damage.damage < availableRepairPoints)
                {
                    availableRepairPoints -= damage.damage;
                }
                else
                {
                    damage.isMarked = false;
                }
            }

            //The game's next Update will handle refreshing the UI.
            //Oddly it does a full cost compute on every update.
        }
    }

}
