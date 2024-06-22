namespace Jutsu
{
    using ThunderRoad;
    using UnityEngine;
    public class Seals
    {
        
        internal bool HandDistance(bool activateChidori)
        {
            return Vector3.Distance(Player.local.handRight.ragdollHand.transform.position,
                Player.local.handLeft.ragdollHand.transform.position) < 1f && !activateChidori;
        }

        internal bool TigerSeal()
        {
            if(Player.local.handLeft.ragdollHand.playerHand.controlHand.alternateUsePressed && !Player.local.handRight.ragdollHand.playerHand.controlHand.alternateUsePressed) Debug.Log("Tiger");
            return Player.local.handLeft.ragdollHand.playerHand.controlHand.alternateUsePressed && !Player.local.handRight.ragdollHand.playerHand.controlHand.alternateUsePressed;
        }

        internal bool MonkeySeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.alternateUsePressed) Debug.Log("Monkey");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.alternateUsePressed;
        }

        internal bool SnakeSeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
               Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed) Debug.Log("Snake");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
                   Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed;
        }

        internal bool OxSeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
               !Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed) Debug.Log("Ox");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
                !Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed;
        }

        internal bool DogSeal()
        {
            if(Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
               !Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed) Debug.Log("Dog");
            return Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
                   !Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed;
        }

        internal bool RatSeal()
        {
            if(Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed &&
               !Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed) Debug.Log("Rat");
            return Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed &&
                   !Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed;
        }

        internal bool HorseSeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed &&
               !Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed) Debug.Log("Horse");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed &&
                   !Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed;
        }

        internal bool BoarSeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed &&
               Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed) Debug.Log("Boar");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed &&
                   Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed;
        }

        internal bool HareSeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
               Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed &&
               !(Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
                 Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed)) Debug.Log("Hare");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
                   Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed &&
                   !(Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
                     Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed);
        }

        internal bool RamSeal()
        {
            if(Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
               Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed &&
               !(Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
                 Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed)) Debug.Log("Ram");
            return Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
                   Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed &&
                   !(Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
                     Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed);
        }

        internal bool BirdSeal()
        {
            if(Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
               Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed
               && Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
               Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed) Debug.Log("Bird");
            return Player.local.handLeft.ragdollHand.playerHand.controlHand.usePressed &&
                   Player.local.handLeft.ragdollHand.playerHand.controlHand.gripPressed
                   && Player.local.handRight.ragdollHand.playerHand.controlHand.usePressed &&
                   Player.local.handRight.ragdollHand.playerHand.controlHand.gripPressed;
        }

        internal bool DragonSeal()
        {
            if(Player.local.handRight.ragdollHand.playerHand.controlHand.alternateUsePressed && Player.local.handLeft.ragdollHand.playerHand.controlHand.alternateUsePressed) Debug.Log("Dragon");
            return Player.local.handRight.ragdollHand.playerHand.controlHand.alternateUsePressed && Player.local.handLeft.ragdollHand.playerHand.controlHand.alternateUsePressed;
        }

    }
}