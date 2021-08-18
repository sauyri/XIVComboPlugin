﻿using Dalamud.Game.ClientState.Structs.JobGauge;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class BRD
    {
        public const byte JobID = 23;

        public const uint
            HeavyShot = 97,
            StraightShot = 98,
            VenomousBite = 100,
            QuickNock = 106,
            Windbite = 113,
            WanderersMinuet = 3559,
            IronJaws = 3560,
            PitchPerfect = 7404,
            CausticBite = 7406,
            Stormbite = 7407,
            RefulgentArrow = 7409,
            BurstShot = 16495,
            ApexArrow = 16496;

        public static class Buffs
        {
            public const short
                StraightShotReady = 122;
        }

        public static class Debuffs
        {
            public const short
                VenomousBite = 124,
                Windbite = 129,
                CausticBite = 1200,
                Stormbite = 1201;
        }

        public static class Levels
        {
            public const byte
                Windbite = 30,
                IronJaws = 56,
                BiteUpgrade = 64,
                RefulgentArrow = 70,
                BurstShot = 76;
        }
    }

    internal class BardWanderersPitchPerfectFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardWanderersPitchPerfectFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.WanderersMinuet)
            {
                if (GetJobGauge<BRDGauge>().ActiveSong == CurrentSong.WANDERER)
                    return BRD.PitchPerfect;
            }

            return actionID;
        }
    }

    internal class BardStraightShotUpgradeFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardStraightShotUpgradeFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.HeavyShot || actionID == BRD.BurstShot)
            {
                // var gauge = GetJobGauge<BRDGauge>();
                // if (gauge.SoulVoiceValue == 100 && IsEnabled(CustomComboPreset.BardApexFeature))
                //     return BRD.ApexArrow;

                if (HasEffect(BRD.Buffs.StraightShotReady))
                    return OriginalHook(BRD.RefulgentArrow);
            }

            return actionID;
        }
    }

    internal class BardIronJawsFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.BardIronJawsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == BRD.IronJaws)
            {
                if (level < BRD.Levels.IronJaws)
                {
                    var venomous = FindTargetEffect(BRD.Debuffs.VenomousBite);
                    var windbite = FindTargetEffect(BRD.Debuffs.Windbite);
                    if (venomous.HasValue && windbite.HasValue)
                    {
                        if (venomous?.Duration < windbite?.Duration)
                            return BRD.VenomousBite;
                        return BRD.Windbite;
                    }
                    else if (windbite.HasValue || level < BRD.Levels.Windbite)
                        return BRD.VenomousBite;
                    return BRD.Windbite;
                }

                if (level < BRD.Levels.BiteUpgrade)
                {
                    var venomous = TargetHasEffect(BRD.Debuffs.VenomousBite);
                    var windbite = TargetHasEffect(BRD.Debuffs.Windbite);
                    if (venomous && windbite)
                        return BRD.IronJaws;
                    if (windbite)
                        return BRD.VenomousBite;
                    return BRD.Windbite;
                }

                var caustic = TargetHasEffect(BRD.Debuffs.CausticBite);
                var stormbite = TargetHasEffect(BRD.Debuffs.Stormbite);
                if (caustic && stormbite)
                    return BRD.IronJaws;
                if (stormbite)
                    return BRD.CausticBite;
                return BRD.Stormbite;
            }

            return actionID;
        }
    }

    // internal class BardApexFeature : CustomCombo
    // {
    //     protected override CustomComboPreset Preset => CustomComboPreset.BardApexFeature;
    // 
    //     protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    //     {
    //         if (actionID == BRD.QuickNock)
    //         {
    //             var gauge = GetJobGauge<BRDGauge>();
    //             if (gauge.SoulVoiceValue == 100)
    //                 return BRD.ApexArrow;
    //         }
    // 
    //         return actionID;
    //     }
    // }
}
