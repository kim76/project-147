using System;
using System.Collections.Generic;

namespace Project147.GameCore.Combat
{
    public sealed class AlienUpgradePlanPresetSet
    {
        public AlienUpgradePlanPresetSet(IReadOnlyList<AlienUpgradePlanPreset> presets)
            : this(presets, 0)
        {
        }

        private AlienUpgradePlanPresetSet(IReadOnlyList<AlienUpgradePlanPreset> presets, int selectedIndex)
        {
            if (presets == null)
            {
                throw new ArgumentNullException(nameof(presets));
            }

            if (presets.Count == 0)
            {
                throw new ArgumentException("Alien upgrade preset set requires at least one preset.", nameof(presets));
            }

            var presetIds = new HashSet<string>();

            foreach (var preset in presets)
            {
                if (preset == null)
                {
                    throw new ArgumentNullException(nameof(presets), "Alien upgrade preset set cannot contain null presets.");
                }

                if (!presetIds.Add(preset.Id))
                {
                    throw new ArgumentException("Alien upgrade preset set cannot contain duplicate presets.", nameof(presets));
                }
            }

            if (selectedIndex < 0 || selectedIndex >= presets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(selectedIndex), "Selected alien upgrade preset index is outside the set.");
            }

            Presets = new List<AlienUpgradePlanPreset>(presets);
            SelectedIndex = selectedIndex;
        }

        public IReadOnlyList<AlienUpgradePlanPreset> Presets { get; }

        public int SelectedIndex { get; }

        public AlienUpgradePlanPreset SelectedPreset
        {
            get { return Presets[SelectedIndex]; }
        }

        public AlienUpgradeChoicePlan SelectedPlan
        {
            get { return SelectedPreset.Plan; }
        }

        public AlienUpgradePlanPresetSet SelectNext()
        {
            return new AlienUpgradePlanPresetSet(Presets, (SelectedIndex + 1) % Presets.Count);
        }

        public AlienUpgradePlanPresetSet SelectPrevious()
        {
            return new AlienUpgradePlanPresetSet(Presets, (SelectedIndex + Presets.Count - 1) % Presets.Count);
        }
    }
}
