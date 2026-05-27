using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project147.GameCore.Abilities;
using Project147.GameCore.Choices;
using Project147.GameCore.Combat;
using Project147.GameCore.Grid;
using Project147.GameCore.Level;
using Project147.GameData.Debug;
using Project147.PlatformServices.Analytics;
using Project147.PlatformServices.Monetisation;
using Project147.PlatformServices.Save;
using UnityEngine;

namespace Project147.UnityPresentation.Debug
{
    public sealed class DebugGridSceneController : MonoBehaviour
    {
        private const int EventFeedCapacity = 7;
        private const int RunChoiceOfferSize = 3;
        private const float TowerSellRefundMultiplier = 0.75f;

        [SerializeField]
        private int width = 8;

        [SerializeField]
        private int height = 5;

        [SerializeField]
        private Vector2Int spawn = new Vector2Int(0, 2);

        [SerializeField]
        private Vector2Int goal = new Vector2Int(7, 2);

        [SerializeField]
        private Vector2Int[] blockedCells =
        {
            new Vector2Int(2, 1),
            new Vector2Int(2, 2),
            new Vector2Int(2, 3),
            new Vector2Int(5, 1),
            new Vector2Int(5, 2),
            new Vector2Int(5, 3)
        };

        [SerializeField]
        private float cellSize = 1.1f;

        [SerializeField]
        private float tileHeight = 0.12f;

        [SerializeField]
        private Material openCellMaterial;

        [SerializeField]
        private Material blockedCellMaterial;

        [SerializeField]
        private Material pathCellMaterial;

        [SerializeField]
        private Material spawnCellMaterial;

        [SerializeField]
        private Material goalCellMaterial;

        [SerializeField]
        private Material towerCellMaterial;

        [SerializeField]
        private Material alienMaterial;

        [SerializeField]
        private Material alienHitMaterial;

        [SerializeField]
        private Material shotLineMaterial;

        [SerializeField]
        private DebugFirstSliceConfig config;

        private readonly List<GameObject> tileObjects = new List<GameObject>();
        private readonly List<GameObject> towerObjects = new List<GameObject>();
        private readonly List<GameObject> shotObjects = new List<GameObject>();
        private readonly List<RuntimeTower> towers = new List<RuntimeTower>();
        private readonly List<RuntimeAlien> activeAliens = new List<RuntimeAlien>();
        private readonly HashSet<GridCoordinate> placedTowers = new HashSet<GridCoordinate>();

        private readonly FreezePulseResolver freezePulseResolver = new FreezePulseResolver();
        private readonly OrbitalStrikeResolver orbitalStrikeResolver = new OrbitalStrikeResolver(new DamageResolver());
        private readonly BaseShieldBurstResolver baseShieldBurstResolver = new BaseShieldBurstResolver();
        private readonly TowerOverchargeResolver towerOverchargeResolver = new TowerOverchargeResolver();
        private readonly RunChoiceResolver runChoiceResolver = new RunChoiceResolver();
        private readonly RunChoiceOfferSelector runChoiceOfferSelector =
            new RunChoiceOfferSelector(new RandomChoiceIndexPicker());
        private readonly WaveIntelBuilder waveIntelBuilder = new WaveIntelBuilder();
        private readonly GridPathfinder pathfinder = new GridPathfinder();
        private readonly TowerTargetSelector targetSelector = new TowerTargetSelector();
        private readonly TowerSaleCalculator towerSaleCalculator = new TowerSaleCalculator();
        private readonly Project147RewardedAdCatalog rewardedAdCatalog = new Project147RewardedAdCatalog();
        private TowerPlacementValidator placementValidator;
        private AttackResolver attackResolver;
        private SplashDamageResolver splashDamageResolver;
        private InMemoryAnalyticsRecorder analyticsRecorder;
        private RewardedAdOfferTracker rewardedAdOfferTracker;
        private PlayerAbilityState freezePulseState;
        private PlayerAbilityState orbitalStrikeState;
        private PlayerAbilityState shieldBurstState;
        private PlayerAbilityState towerOverchargeState;
        private TowerUnlockState towerUnlockState;
        private TowerLoadout towerLoadout;
        private TowerLoadoutPlanSet towerLoadoutPlans;
        private TowerUpgradeLoadout towerUpgradeLoadout;
        private RewardCalculator rewardCalculator;
        private BaseState currentBase;
        private CurrencyWallet wallet;
        private RunModifierState runModifiers;
        private RunSummaryState runSummary;
        private ICampaignProgressStore campaignProgressStore;
        private CampaignProgressState campaignProgress;
        private CampaignLevelUnlockState levelUnlockState;
        private LevelProgressApplicationResult lastProgressResult;
        private IReadOnlyList<LevelRunDefinition> levelDefinitions;
        private GameSpeedState gameSpeed;
        private GamePauseState gamePause;
        private LevelEventFeed eventFeed;
        private bool waveActive;
        private bool won;
        private bool lost;
        private bool sellMode;
        private bool retargetMode;
        private bool lastRunUnlockedLevel;
        private int selectedLevelLayoutIndex;
        private GridCoordinate? inspectedTowerCoordinate;
        private int completedWaves;
        private int waveStartBaseHealth;
        private WaveDefinition currentWaveDefinition;
        private WaveSpawnState waveSpawnState;
        private IReadOnlyList<RunChoiceDefinition> pendingRunChoices;
        private RewardedAdOpportunityDefinition pendingRewardedAdOpportunity;
        private int pendingRewardedAdScrapAmount;
        private GameObject placementPreview;
        private LineRenderer placementPreviewLine;
        private GridCoordinate? previewCoordinate;
        private string combatBannerText;
        private float combatBannerSeconds;
        private string saveStatusText;

        private void Start()
        {
            InitialiseRules();
            ResetSlice();
        }

        private void Update()
        {
            if (won || lost)
            {
                return;
            }

            var deltaSeconds = ScaledDeltaSeconds;
            TickCombatBanner(deltaSeconds);

            if (orbitalStrikeState != null)
            {
                orbitalStrikeState = orbitalStrikeState.Tick(deltaSeconds);
            }

            if (freezePulseState != null)
            {
                freezePulseState = freezePulseState.Tick(deltaSeconds);
            }

            if (shieldBurstState != null)
            {
                shieldBurstState = shieldBurstState.Tick(deltaSeconds);
            }

            if (towerOverchargeState != null)
            {
                towerOverchargeState = towerOverchargeState.Tick(deltaSeconds);
            }

            UpdateWaveSpawning(deltaSeconds);
            UpdateAliens(deltaSeconds);
            UpdateTowers(deltaSeconds);
            UpdateAlienStatusVisuals();
            CompleteWaveIfReady();
        }

        public void TryPlaceTower(GridCoordinate coordinate)
        {
            if (waveActive || won || lost)
            {
                RecordEvent("Towers can only be placed between waves.");
                return;
            }

            if (HasPendingRunChoice)
            {
                RecordEvent("Choose a reward before building.");
                return;
            }

            if (placedTowers.Contains(coordinate))
            {
                inspectedTowerCoordinate = coordinate;

                if (sellMode)
                {
                    TrySellTower(coordinate);
                }
                else if (retargetMode)
                {
                    TryCycleTowerTargetingMode(coordinate);
                }
                else
                {
                    TryUpgradeTower(coordinate);
                }

                return;
            }

            var selectedTowerCost = GetSelectedTowerCost();

            if (!wallet.CanSpend(selectedTowerCost))
            {
                RecordEvent("Not enough scrap for tower.");
                return;
            }

            var bounds = new GridBounds(width, height);
            var grid = CreateGrid(bounds);
            var start = ToGridCoordinate(spawn);
            var end = ToGridCoordinate(goal);
            var result = placementValidator.ValidatePlacement(grid, coordinate, start, end);

            if (!result.IsValid)
            {
                RecordEvent($"Cannot place tower at {coordinate}: {result.FailureReason}.");
                return;
            }

            wallet = wallet.Spend(selectedTowerCost);
            var discountApplied = SelectedTower.Cost - selectedTowerCost;

            if (discountApplied > 0)
            {
                runModifiers = runModifiers.ConsumeNextTowerDiscount();
            }

            placedTowers.Add(coordinate);
            var towerObject = CreateTowerObject(coordinate, SelectedTower);
            towers.Add(new RuntimeTower(coordinate, new TowerState(SelectedTower), towerObject));
            RebuildTiles();
            HidePlacementPreviewIfAny();
            RecordEvent(discountApplied > 0
                ? $"Placed {SelectedTower.Id} at {coordinate}. Discount {discountApplied}. Scrap: {wallet.Balance}."
                : $"Placed {SelectedTower.Id} at {coordinate}. Scrap: {wallet.Balance}.");
            TrackAnalytics(
                "tower_placed",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "tower_id", SelectedTower.Id },
                    { "scrap_cost", selectedTowerCost.ToString() }
                });
        }

        public void ShowPlacementPreview(GridCoordinate coordinate)
        {
            if (towerLoadout == null || towerUpgradeLoadout == null || currentBase == null || wallet == null)
            {
                return;
            }

            if (waveActive || won || lost || HasPendingRunChoice)
            {
                HidePlacementPreviewIfAny();
                return;
            }

            previewCoordinate = coordinate;
            EnsurePlacementPreview();

            var canPlace = CanPreviewPlacement(coordinate);
            var tower = FindRuntimeTower(coordinate);
            var canUpgrade = tower != null
                && !waveActive
                && !won
                && !lost
                && !sellMode
                && !retargetMode
                && tower.State.Level < config.MaxTowerLevel
                && wallet.CanSpend(SelectedTowerUpgrade.Cost);
            var canSell = tower != null
                && !waveActive
                && !won
                && !lost
                && sellMode
                && !HasPendingRunChoice;
            var canRetarget = tower != null
                && !waveActive
                && !won
                && !lost
                && retargetMode
                && !HasPendingRunChoice;
            var colour = canPlace || canUpgrade || canSell || canRetarget
                ? new Color(0.25f, 1f, 0.35f, 0.48f)
                : new Color(1f, 0.2f, 0.2f, 0.48f);
            var range = tower == null ? SelectedTower.Range : tower.State.Definition.Range;
            placementPreview.transform.localPosition = ToWorldPosition(coordinate, 0.18f);
            placementPreviewLine.startColor = colour;
            placementPreviewLine.endColor = colour;
            BuildPlacementPreviewCircle(range);
            SetPreviewMaterialColour(colour);
            placementPreview.SetActive(true);
        }

        public void HidePlacementPreview(GridCoordinate coordinate)
        {
            if (!previewCoordinate.HasValue || previewCoordinate.Value != coordinate)
            {
                return;
            }

            previewCoordinate = null;

            if (placementPreview != null)
            {
                placementPreview.SetActive(false);
            }
        }

        [ContextMenu("Restart First Slice")]
        private void ResetSlice()
        {
            HidePlacementPreviewIfAny();
            ApplySelectedLevelLayout();
            ClearActors();
            ClearTiles();
            placedTowers.Clear();
            towers.Clear();
            rewardCalculator = new RewardCalculator(SelectedLevel.PerfectWaveScrapBonus);
            currentBase = new BaseState(SelectedLevel.BaseHealth);
            wallet = new CurrencyWallet(SelectedLevel.StartingCurrency);
            towerLoadout = towerLoadoutPlans.SelectedPlan.CreateLoadout();
            runModifiers = new RunModifierState();
            runSummary = new RunSummaryState();
            gameSpeed = new GameSpeedState();
            gamePause = new GamePauseState();
            freezePulseState = new PlayerAbilityState(config.CreateFreezePulseAbilityDefinition());
            orbitalStrikeState = new PlayerAbilityState(config.CreateOrbitalStrikeAbilityDefinition());
            shieldBurstState = new PlayerAbilityState(config.CreateShieldBurstAbilityDefinition());
            towerOverchargeState = new PlayerAbilityState(config.CreateTowerOverchargeAbilityDefinition());
            eventFeed = new LevelEventFeed(EventFeedCapacity).Add("Ready. Place towers, then start wave.");
            waveActive = false;
            won = false;
            lost = false;
            sellMode = false;
            retargetMode = false;
            lastRunUnlockedLevel = false;
            inspectedTowerCoordinate = null;
            completedWaves = 0;
            waveStartBaseHealth = currentBase.CurrentHealth;
            currentWaveDefinition = null;
            waveSpawnState = null;
            pendingRunChoices = new List<RunChoiceDefinition>();
            pendingRewardedAdOpportunity = null;
            pendingRewardedAdScrapAmount = 0;
            rewardedAdOfferTracker = new RewardedAdOfferTracker();
            analyticsRecorder = new InMemoryAnalyticsRecorder(new Project147AnalyticsCatalog().CreateGameplayEvents());
            combatBannerText = null;
            combatBannerSeconds = 0;
            RebuildTiles();
            TrackAnalytics(
                "level_started",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "loadout_id", towerLoadoutPlans.SelectedPlan.Id }
                });
        }

        private void ApplySelectedLevelLayout()
        {
            if (levelDefinitions == null || levelDefinitions.Count == 0)
            {
                return;
            }

            if (selectedLevelLayoutIndex < 0 || selectedLevelLayoutIndex >= levelDefinitions.Count)
            {
                selectedLevelLayoutIndex = 0;
            }

            var layout = SelectedLevelLayout;
            width = layout.Width;
            height = layout.Height;
            spawn = ToVector2Int(layout.Spawn);
            goal = ToVector2Int(layout.Goal);

            var cells = new Vector2Int[layout.BlockedCells.Count];

            for (var index = 0; index < layout.BlockedCells.Count; index++)
            {
                cells[index] = ToVector2Int(layout.BlockedCells[index]);
            }

            blockedCells = cells;
        }

        private bool CanPreviewPlacement(GridCoordinate coordinate)
        {
            if (waveActive || won || lost || HasPendingRunChoice || !wallet.CanSpend(GetSelectedTowerCost()))
            {
                return false;
            }

            var bounds = new GridBounds(width, height);
            var grid = CreateGrid(bounds);
            var result = placementValidator.ValidatePlacement(
                grid,
                coordinate,
                ToGridCoordinate(spawn),
                ToGridCoordinate(goal));

            return result.IsValid;
        }

        private void InitialiseRules()
        {
            if (config == null)
            {
                UnityEngine.Debug.LogWarning("Debug first-slice config is missing. Using in-memory defaults. Recreate the scene from Project147 > Debug > Create Grid Scene to create a tuneable asset.");
                config = ScriptableObject.CreateInstance<DebugFirstSliceConfig>();
            }

            placementValidator = new TowerPlacementValidator(pathfinder);
            var damageResolver = new DamageResolver();
            attackResolver = new AttackResolver(damageResolver);
            splashDamageResolver = new SplashDamageResolver(damageResolver);
            levelDefinitions = config.CreateLevelDefinitions();
            rewardCalculator = new RewardCalculator(SelectedLevel.PerfectWaveScrapBonus);
            towerUnlockState = config.CreateInitialTowerUnlockState();
            towerLoadoutPlans = new TowerLoadoutPlanSet(config.CreateTowerLoadoutPlans(towerUnlockState));
            towerLoadout = towerLoadoutPlans.SelectedPlan.CreateLoadout();
            freezePulseState = new PlayerAbilityState(config.CreateFreezePulseAbilityDefinition());
            orbitalStrikeState = new PlayerAbilityState(config.CreateOrbitalStrikeAbilityDefinition());
            shieldBurstState = new PlayerAbilityState(config.CreateShieldBurstAbilityDefinition());
            towerOverchargeState = new PlayerAbilityState(config.CreateTowerOverchargeAbilityDefinition());
            towerUpgradeLoadout = new TowerUpgradeLoadout(config.CreateTowerUpgradeDefinitions());
            analyticsRecorder = new InMemoryAnalyticsRecorder(new Project147AnalyticsCatalog().CreateGameplayEvents());
            rewardedAdOfferTracker = new RewardedAdOfferTracker();
            campaignProgressStore = CreateCampaignProgressStore();
            campaignProgress = LoadCampaignProgress();
            RefreshLevelUnlockState();
            SelectFirstUnlockedLevelIfCurrentIsLocked();
            gameSpeed = gameSpeed ?? new GameSpeedState();
            gamePause = gamePause ?? new GamePauseState();
        }

        private static ICampaignProgressStore CreateCampaignProgressStore()
        {
            var path = Path.Combine(Application.persistentDataPath, "project147-campaign-progress.save");
            return new CampaignProgressStore(
                new FileTextSaveStorage(path),
                new CampaignProgressSaveCodec());
        }

        private CampaignProgressState LoadCampaignProgress()
        {
            try
            {
                var loaded = campaignProgressStore.LoadOrDefault();
                saveStatusText = campaignProgressStore.Exists ? "Progress loaded" : "No saved progress";
                return loaded;
            }
            catch (System.Exception exception)
            {
                UnityEngine.Debug.LogWarning($"Could not load campaign progress: {exception.Message}");
                saveStatusText = "Save load failed";
                return new CampaignProgressState();
            }
        }

        private TowerDefinition SelectedTower
        {
            get { return towerLoadout.SelectedTower; }
        }

        private LevelLayoutDefinition SelectedLevelLayout
        {
            get { return SelectedLevel.Layout; }
        }

        private LevelRunDefinition SelectedLevel
        {
            get { return levelDefinitions[selectedLevelLayoutIndex]; }
        }

        private TowerUpgradeDefinition SelectedTowerUpgrade
        {
            get { return towerUpgradeLoadout.SelectedUpgrade; }
        }

        private float ScaledDeltaSeconds
        {
            get
            {
                var speedScaledDeltaSeconds = (gameSpeed ?? new GameSpeedState()).ScaleDeltaSeconds(Time.deltaTime);
                return (gamePause ?? new GamePauseState()).ScaleDeltaSeconds(speedScaledDeltaSeconds);
            }
        }

        private void SelectNextGameSpeed()
        {
            gameSpeed = (gameSpeed ?? new GameSpeedState()).SelectNext();
            RecordEvent($"Game speed: {gameSpeed.Multiplier:0.#}x.");
        }

        private void TogglePause()
        {
            gamePause = (gamePause ?? new GamePauseState()).Toggle();
            RecordEvent(gamePause.IsPaused ? "Paused." : "Resumed.");
        }

        private void SelectPreviousTower()
        {
            towerLoadout = towerLoadout.SelectPrevious();
            HidePlacementPreviewIfAny();
            RecordEvent($"Selected tower: {SelectedTower.Id}.");
        }

        private void SelectNextTower()
        {
            towerLoadout = towerLoadout.SelectNext();
            HidePlacementPreviewIfAny();
            RecordEvent($"Selected tower: {SelectedTower.Id}.");
        }

        private void SelectPreviousTowerLoadoutPlan()
        {
            towerLoadoutPlans = towerLoadoutPlans.SelectPrevious();
            ResetSlice();
            RecordEvent($"Selected loadout: {towerLoadoutPlans.SelectedPlan.Id}.");
        }

        private void SelectNextTowerLoadoutPlan()
        {
            towerLoadoutPlans = towerLoadoutPlans.SelectNext();
            ResetSlice();
            RecordEvent($"Selected loadout: {towerLoadoutPlans.SelectedPlan.Id}.");
        }

        private void SelectPreviousTowerUpgrade()
        {
            towerUpgradeLoadout = towerUpgradeLoadout.SelectPrevious();
            HidePlacementPreviewIfAny();
            RecordEvent($"Selected upgrade: {SelectedTowerUpgrade.Id}.");
        }

        private void SelectNextTowerUpgrade()
        {
            towerUpgradeLoadout = towerUpgradeLoadout.SelectNext();
            HidePlacementPreviewIfAny();
            RecordEvent($"Selected upgrade: {SelectedTowerUpgrade.Id}.");
        }

        private void SelectPreviousLevelLayout()
        {
            selectedLevelLayoutIndex = SelectUnlockedLevelIndex(-1);
            ResetSlice();
            RecordEvent($"Selected level: {SelectedLevelLayout.Id}.");
        }

        private void SelectNextLevelLayout()
        {
            selectedLevelLayoutIndex = SelectUnlockedLevelIndex(1);
            ResetSlice();
            RecordEvent($"Selected level: {SelectedLevelLayout.Id}.");
        }

        private int SelectUnlockedLevelIndex(int direction)
        {
            if (levelDefinitions == null || levelDefinitions.Count == 0)
            {
                return 0;
            }

            var step = direction < 0 ? -1 : 1;

            for (var offset = 1; offset <= levelDefinitions.Count; offset++)
            {
                var index = (selectedLevelLayoutIndex + offset * step + levelDefinitions.Count) % levelDefinitions.Count;

                if (IsLevelUnlocked(levelDefinitions[index].Layout.Id))
                {
                    return index;
                }
            }

            return selectedLevelLayoutIndex;
        }

        private void TryUpgradeTower(GridCoordinate coordinate)
        {
            var tower = FindRuntimeTower(coordinate);

            if (tower == null)
            {
                RecordEvent($"No tower found at {coordinate}.");
                return;
            }

            if (tower.State.Level >= config.MaxTowerLevel)
            {
                RecordEvent($"Tower at {coordinate} is already level {tower.State.Level}.");
                return;
            }

            if (!wallet.CanSpend(SelectedTowerUpgrade.Cost))
            {
                RecordEvent("Not enough scrap for tower upgrade.");
                return;
            }

            var selectedUpgrade = SelectedTowerUpgrade;
            wallet = wallet.Spend(selectedUpgrade.Cost);
            tower.State = tower.State.Upgrade(selectedUpgrade);
            UpdateTowerObject(tower);
            HidePlacementPreviewIfAny();
            RecordEvent($"Upgraded tower at {coordinate} with {selectedUpgrade.Id} to level {tower.State.Level}.");
            TrackAnalytics(
                "tower_upgraded",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "tower_id", tower.State.Definition.Id },
                    { "upgrade_id", selectedUpgrade.Id },
                    { "tower_level", tower.State.Level.ToString() }
                });
        }

        private void TrySellTower(GridCoordinate coordinate)
        {
            var tower = FindRuntimeTower(coordinate);

            if (tower == null)
            {
                RecordEvent($"No tower found at {coordinate}.");
                return;
            }

            var refund = towerSaleCalculator.CalculateRefund(tower.State, TowerSellRefundMultiplier);
            wallet = wallet.Add(refund);
            towers.Remove(tower);
            placedTowers.Remove(coordinate);
            inspectedTowerCoordinate = null;

            if (tower.GameObject != null)
            {
                towerObjects.Remove(tower.GameObject);
                Destroy(tower.GameObject);
            }

            RebuildTiles();
            HidePlacementPreviewIfAny();
            RecordEvent($"Sold tower at {coordinate}. +{refund} scrap.");
            TrackAnalytics(
                "tower_sold",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "tower_id", tower.State.Definition.Id },
                    { "refund_amount", refund.ToString() }
                });
        }

        private void TryCycleTowerTargetingMode(GridCoordinate coordinate)
        {
            var tower = FindRuntimeTower(coordinate);

            if (tower == null)
            {
                RecordEvent($"No tower found at {coordinate}.");
                return;
            }

            tower.State = tower.State.SelectNextTargetingMode();
            HidePlacementPreviewIfAny();
            RecordEvent($"Tower at {coordinate} targeting: {tower.State.TargetingMode}.");
        }

        private void StartNextWave()
        {
            if (waveActive || won || lost || completedWaves >= SelectedLevel.TotalWaves)
            {
                return;
            }

            if (HasPendingRunChoice)
            {
                RecordEvent("Choose a reward before starting the next wave.");
                return;
            }

            waveActive = true;
            sellMode = false;
            retargetMode = false;
            HidePlacementPreviewIfAny();
            runModifiers = runModifiers.StartWave();
            waveStartBaseHealth = currentBase.CurrentHealth;
            currentWaveDefinition = config.CreateWaveDefinition(SelectedLevel, completedWaves);
            waveSpawnState = new WaveSpawnState(currentWaveDefinition);
            ShowCombatBanner($"Wave {completedWaves + 1}");
            RecordEvent($"Wave {completedWaves + 1} started: {BuildWaveSummary(currentWaveDefinition)}.");
            TrackAnalytics(
                "wave_started",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "wave_number", (completedWaves + 1).ToString() },
                    { "alien_count", currentWaveDefinition.AlienCount.ToString() }
                });
        }

        private void TryActivateFreezePulse()
        {
            if (won || lost)
            {
                return;
            }

            if (!waveActive)
            {
                RecordEvent("Freeze Pulse can only be used during a wave.");
                return;
            }

            if (!freezePulseState.CanActivate)
            {
                RecordEvent($"Freeze Pulse cooling down: {freezePulseState.RemainingCooldownSeconds:0.0}s.");
                return;
            }

            var targets = new List<AlienState>();

            foreach (var alien in activeAliens)
            {
                if (alien.State.IsAlive)
                {
                    targets.Add(alien.State);
                }
            }

            if (targets.Count == 0)
            {
                RecordEvent("Freeze Pulse needs active aliens.");
                return;
            }

            var results = freezePulseResolver.Resolve(freezePulseState.Definition, targets);
            freezePulseState = freezePulseState.Activate();
            runSummary = runSummary.RecordFreezePulseUsed();

            foreach (var result in results)
            {
                var alien = FindRuntimeAlien(result.Source);

                if (alien == null)
                {
                    continue;
                }

                alien.State = result.Target;
                FlashAlien(alien);
            }

            ShowFreezePulseFeedback();
            RecordEvent($"Freeze Pulse slowed {results.Count} alien(s).");
            TrackAbilityUsed(freezePulseState.Definition.Id);
        }

        private void TryActivateOrbitalStrike()
        {
            if (won || lost)
            {
                return;
            }

            if (!waveActive)
            {
                RecordEvent("Orbital Strike can only be used during a wave.");
                return;
            }

            if (!orbitalStrikeState.CanActivate)
            {
                RecordEvent($"Orbital Strike cooling down: {orbitalStrikeState.RemainingCooldownSeconds:0.0}s.");
                return;
            }

            var targets = new List<AlienState>();

            foreach (var alien in activeAliens)
            {
                if (alien.State.IsAlive)
                {
                    targets.Add(alien.State);
                }
            }

            if (targets.Count == 0)
            {
                RecordEvent("Orbital Strike needs active aliens.");
                return;
            }

            var results = orbitalStrikeResolver.Resolve(orbitalStrikeState.Definition, targets);
            orbitalStrikeState = orbitalStrikeState.Activate();
            runSummary = runSummary.RecordOrbitalStrikeUsed();

            foreach (var result in results)
            {
                var alien = FindRuntimeAlien(result.Source);

                if (alien == null)
                {
                    continue;
                }

                alien.State = result.Target;
                FlashAlien(alien);
                ShowOrbitalStrikeFeedback(alien.GameObject.transform.localPosition);
            }

            RecordEvent($"Orbital Strike hit {results.Count} alien(s).");
            TrackAbilityUsed(orbitalStrikeState.Definition.Id);
        }

        private void TryActivateShieldBurst()
        {
            if (won || lost)
            {
                return;
            }

            if (!waveActive)
            {
                RecordEvent("Shield Burst can only be used during a wave.");
                return;
            }

            if (!shieldBurstState.CanActivate)
            {
                RecordEvent($"Shield Burst cooling down: {shieldBurstState.RemainingCooldownSeconds:0.0}s.");
                return;
            }

            currentBase = baseShieldBurstResolver.Resolve(shieldBurstState.Definition, currentBase);
            shieldBurstState = shieldBurstState.Activate();
            runSummary = runSummary.RecordShieldBurstUsed();
            RecordEvent($"Shield Burst added {shieldBurstState.Definition.BaseShieldAmount} base shield.");
            TrackAbilityUsed(shieldBurstState.Definition.Id);
        }

        private void TryActivateTowerOvercharge()
        {
            if (won || lost)
            {
                return;
            }

            if (!waveActive)
            {
                RecordEvent("Tower Overcharge can only be used during a wave.");
                return;
            }

            if (towers.Count == 0)
            {
                RecordEvent("Tower Overcharge needs at least one tower.");
                return;
            }

            if (!towerOverchargeState.CanActivate)
            {
                RecordEvent($"Tower Overcharge cooling down: {towerOverchargeState.RemainingCooldownSeconds:0.0}s.");
                return;
            }

            runModifiers = towerOverchargeResolver.Resolve(towerOverchargeState.Definition, runModifiers);
            towerOverchargeState = towerOverchargeState.Activate();
            runSummary = runSummary.RecordTowerOverchargeUsed();
            RecordEvent(
                $"Tower Overcharge: +{towerOverchargeState.Definition.TowerDamagePercent}% damage, +{towerOverchargeState.Definition.TowerFireRatePercent}% rate this wave.");
            TrackAbilityUsed(towerOverchargeState.Definition.Id);
        }

        private void SelectRunChoice(int choiceIndex)
        {
            if (!HasPendingRunChoice)
            {
                return;
            }

            if (choiceIndex < 0 || choiceIndex >= pendingRunChoices.Count)
            {
                RecordEvent("Invalid reward choice.");
                return;
            }

            var choice = pendingRunChoices[choiceIndex];
            var result = runChoiceResolver.Apply(choice, currentBase, wallet, runModifiers);
            currentBase = result.BaseState;
            wallet = result.Wallet;
            runModifiers = result.RunModifiers;
            runSummary = runSummary.RecordRewardChosen();
            pendingRunChoices = new List<RunChoiceDefinition>();
            RecordEvent($"Chose {choice.Label}: {FormatRunChoiceEffect(choice)}.");
            TrackAnalytics(
                "reward_choice_selected",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "choice_id", choice.Id },
                    { "wave_number", completedWaves.ToString() }
                });
        }

        private void ClaimPendingRewardedAdOffer()
        {
            if (!HasPendingRewardedAdOffer)
            {
                return;
            }

            wallet = wallet.Add(pendingRewardedAdScrapAmount);
            rewardedAdOfferTracker = rewardedAdOfferTracker.RecordOffer(pendingRewardedAdOpportunity);
            RecordEvent($"Fake rewarded ad claimed: +{pendingRewardedAdScrapAmount} scrap.");
            ClearPendingRewardedAdOffer();
        }

        private void SkipPendingRewardedAdOffer()
        {
            if (!HasPendingRewardedAdOffer)
            {
                return;
            }

            RecordEvent($"Skipped fake rewarded ad: {pendingRewardedAdOpportunity.Id}.");
            ClearPendingRewardedAdOffer();
        }

        private void ClearPendingRewardedAdOffer()
        {
            pendingRewardedAdOpportunity = null;
            pendingRewardedAdScrapAmount = 0;
        }

        private void OfferRewardedAdIfAvailable(int waveRewardAmount)
        {
            var opportunity = rewardedAdCatalog.GetRequiredOpportunity("double-wave-clear-scrap");

            if (!rewardedAdOfferTracker.CanOffer(opportunity))
            {
                return;
            }

            pendingRewardedAdOpportunity = opportunity;
            pendingRewardedAdScrapAmount = waveRewardAmount;
            TrackAnalytics(
                "rewarded_ad_offer_shown",
                new Dictionary<string, string>
                {
                    { "placement_id", opportunity.Id },
                    { "level_id", SelectedLevelLayout.Id }
                });
            RecordEvent($"Fake rewarded ad offer: +{waveRewardAmount} scrap.");
        }

        private void UpdateWaveSpawning(float deltaSeconds)
        {
            if (!waveActive || waveSpawnState == null || waveSpawnState.HasCompletedSpawning)
            {
                return;
            }

            var spawnResult = waveSpawnState.Tick(deltaSeconds);
            waveSpawnState = spawnResult.State;

            foreach (var spawnEntry in spawnResult.SpawnEntries)
            {
                SpawnAlien(spawnEntry.AlienId);
            }
        }

        private void SpawnAlien(string alienId)
        {
            var bounds = new GridBounds(width, height);
            var path = pathfinder.FindShortestPath(CreateGrid(bounds), ToGridCoordinate(spawn), ToGridCoordinate(goal));

            if (path.Count == 0)
            {
                RecordEvent("No path exists for alien spawn.");
                return;
            }

            var definition = config.CreateAlienDefinition(alienId, completedWaves);
            var visual = DebugActorVisualFactory.CreateAlien(
                transform,
                ToWorldPosition(path[0], 0.35f),
                definition,
                SelectAlienVisualRole(definition),
                alienMaterial);

            var alien = new RuntimeAlien(
                visual.GameObject,
                new AlienState(definition),
                path,
                visual.PrimaryRenderer,
                visual.DefaultMaterial);
            AttachAlienStatusVisuals(alien);
            activeAliens.Add(alien);
        }

        private void UpdateAliens(float deltaSeconds)
        {
            for (var index = activeAliens.Count - 1; index >= 0; index--)
            {
                var alien = activeAliens[index];

                if (!alien.State.IsAlive)
                {
                    Destroy(alien.GameObject);
                    activeAliens.RemoveAt(index);
                    var reward = rewardCalculator.CalculateAlienKillReward(alien.State.Definition);
                    wallet = wallet.Add(reward.Amount);
                    runSummary = runSummary.RecordAlienDestroyed(reward.Amount);
                    RecordEvent($"{FormatAlienLabel(alien.State.Definition.Id)} destroyed. +{reward.Amount} scrap.");
                    continue;
                }

                UpdateAlienHitFlash(alien, deltaSeconds);
                alien.State = alien.State.Tick(deltaSeconds);

                if (MoveAlien(alien, deltaSeconds))
                {
                    currentBase = currentBase.ApplyLeakDamage(1);
                    runSummary = runSummary.RecordAlienLeaked();
                    Destroy(alien.GameObject);
                    activeAliens.RemoveAt(index);
                    RecordEvent($"{FormatAlienLabel(alien.State.Definition.Id)} leaked. Base: {currentBase.CurrentHealth}/{currentBase.MaxHealth}.");

                    if (currentBase.IsDestroyed && !lost)
                    {
                        lost = true;
                        waveActive = false;
                        runSummary = runSummary.Complete(RunOutcome.Defeat);
                        ApplyCompletedRunProgress();
                        ShowCombatBanner("Defeat");
                        RecordEvent("Defeat. Base destroyed.");
                        TrackLevelCompleted();
                    }
                }
            }
        }

        private void UpdateAlienHitFlash(RuntimeAlien alien, float deltaSeconds)
        {
            if (alien.HitFlashSeconds <= 0)
            {
                return;
            }

            alien.HitFlashSeconds -= deltaSeconds;

            if (alien.HitFlashSeconds <= 0 && alien.Renderer != null)
            {
                alien.Renderer.sharedMaterial = alien.DefaultMaterial;
            }
        }

        private void AttachAlienStatusVisuals(RuntimeAlien alien)
        {
            alien.StatusVisualRoot = new GameObject("Status Visuals");
            alien.StatusVisualRoot.transform.SetParent(alien.GameObject.transform, false);
            alien.StatusVisualRoot.transform.localPosition = Vector3.zero;

            alien.HealthBarBackground = CreateIndicatorPart(
                alien.StatusVisualRoot.transform,
                PrimitiveType.Cube,
                "Health Bar Background",
                new Vector3(0, 0.74f, -0.34f),
                new Vector3(0.66f, 0.04f, 0.06f),
                new Color(0.08f, 0.08f, 0.08f, 1f));
            alien.HealthBarForeground = CreateIndicatorPart(
                alien.StatusVisualRoot.transform,
                PrimitiveType.Cube,
                "Health Bar",
                new Vector3(0, 0.75f, -0.34f),
                new Vector3(0.62f, 0.05f, 0.07f),
                new Color(0.25f, 1f, 0.32f, 1f));
            alien.ShieldBarForeground = CreateIndicatorPart(
                alien.StatusVisualRoot.transform,
                PrimitiveType.Cube,
                "Shield Bar",
                new Vector3(0, 0.84f, -0.34f),
                new Vector3(0.62f, 0.05f, 0.07f),
                new Color(0.2f, 0.82f, 1f, 1f));
            alien.StatusMarker = CreateIndicatorPart(
                alien.StatusVisualRoot.transform,
                PrimitiveType.Sphere,
                "Status Marker",
                new Vector3(0.42f, 0.82f, -0.34f),
                new Vector3(0.12f, 0.12f, 0.12f),
                new Color(0.82f, 1f, 0.24f, 1f));

            UpdateAlienStatusVisual(alien);
        }

        private void UpdateAlienStatusVisuals()
        {
            foreach (var alien in activeAliens)
            {
                UpdateAlienStatusVisual(alien);
            }
        }

        private void UpdateAlienStatusVisual(RuntimeAlien alien)
        {
            if (alien.GameObject == null || alien.StatusVisualRoot == null)
            {
                return;
            }

            UpdateBar(alien.HealthBarForeground, alien.State.CurrentHealth / alien.State.Definition.MaxHealth, 0.62f);

            var shieldRatio = alien.State.Definition.ShieldCapacity <= 0
                ? 0
                : alien.State.CurrentShield / alien.State.Definition.ShieldCapacity;
            UpdateBar(alien.ShieldBarForeground, shieldRatio, 0.62f);

            if (alien.StatusMarker == null)
            {
                return;
            }

            var hasStatus = alien.State.ActiveStatusEffects.Count > 0;
            alien.StatusMarker.SetActive(hasStatus);

            if (hasStatus)
            {
                var colour = alien.State.ActiveStatusEffects[0].Definition.Type == AlienStatusEffectType.Poison
                    ? new Color(0.62f, 1f, 0.18f, 1f)
                    : new Color(0.35f, 0.92f, 1f, 1f);
                SetRendererColour(alien.StatusMarker, colour);
            }
        }

        private static void UpdateBar(GameObject bar, float ratio, float fullWidth)
        {
            if (bar == null)
            {
                return;
            }

            var clampedRatio = Mathf.Clamp01(ratio);
            bar.SetActive(clampedRatio > 0);
            bar.transform.localScale = new Vector3(fullWidth * clampedRatio, bar.transform.localScale.y, bar.transform.localScale.z);
            bar.transform.localPosition = new Vector3(
                -fullWidth * (1 - clampedRatio) * 0.5f,
                bar.transform.localPosition.y,
                bar.transform.localPosition.z);
        }

        private bool MoveAlien(RuntimeAlien alien, float deltaSeconds)
        {
            if (alien.NextPathIndex >= alien.Path.Count)
            {
                return true;
            }

            var target = ToWorldPosition(alien.Path[alien.NextPathIndex], 0.35f);
            var speed = alien.State.Definition.SpeedCellsPerSecond * alien.State.MovementSpeedMultiplier * cellSize;
            alien.GameObject.transform.localPosition = Vector3.MoveTowards(
                alien.GameObject.transform.localPosition,
                target,
                speed * deltaSeconds);

            if (Vector3.Distance(alien.GameObject.transform.localPosition, target) <= 0.001f)
            {
                alien.NextPathIndex++;
                alien.PathProgress = alien.NextPathIndex;
            }

            return alien.NextPathIndex >= alien.Path.Count;
        }

        private void UpdateTowers(float deltaSeconds)
        {
            for (var towerIndex = 0; towerIndex < towers.Count; towerIndex++)
            {
                var tower = towers[towerIndex];
                tower.State = tower.State.Tick(deltaSeconds);

                if (!tower.State.CanFire)
                {
                    continue;
                }

                var candidates = BuildTargetCandidates(tower);
                var selected = targetSelector.SelectTarget(candidates, tower.State.TargetingMode);

                if (!selected.HasValue)
                {
                    continue;
                }

                var alien = FindRuntimeAlien(selected.Value.Alien);

                if (alien == null)
                {
                    continue;
                }

                var effectiveTowerDefinition = BuildEffectiveTowerDefinition(tower.State.Definition);
                var attack = attackResolver.Resolve(effectiveTowerDefinition, alien.State);
                alien.State = attack.Target;
                tower.State = tower.State.MarkFired(effectiveTowerDefinition.FireRatePerSecond);

                if (!attack.Damage.WasDodged && attack.Damage.FinalAmount > 0 && alien.State.IsAlive)
                {
                    foreach (var statusEffect in effectiveTowerDefinition.StatusEffects)
                    {
                        alien.State = alien.State.ApplyStatusEffect(statusEffect);
                    }
                }

                ApplySplashDamage(tower, effectiveTowerDefinition, alien, attack.Damage);
                ShowShotFeedback(tower.Coordinate, alien);
                ShowDamageFeedback(alien.GameObject.transform.localPosition, attack.Damage);
            }
        }

        private void ApplySplashDamage(
            RuntimeTower tower,
            TowerDefinition definition,
            RuntimeAlien primaryAlien,
            DamageResult primaryDamage)
        {
            if (primaryDamage.WasDodged
                || primaryDamage.FinalAmount <= 0
                || definition.SplashRadius <= 0
                || definition.SplashDamageMultiplier <= 0)
            {
                return;
            }

            var candidates = new List<SplashDamageCandidate>();
            var impactPosition = primaryAlien.GameObject.transform.localPosition;

            foreach (var alien in activeAliens)
            {
                if (alien == primaryAlien || !alien.State.IsAlive)
                {
                    continue;
                }

                var distance = Vector3.Distance(impactPosition, alien.GameObject.transform.localPosition) / cellSize;
                candidates.Add(new SplashDamageCandidate(alien.State, distance));
            }

            var results = splashDamageResolver.Resolve(definition, candidates);
            var hitCount = 0;

            foreach (var result in results)
            {
                var alien = FindRuntimeAlien(result.Source);

                if (alien == null)
                {
                    continue;
                }

                alien.State = result.Target;
                FlashAlien(alien);
                ShowDamageFeedback(alien.GameObject.transform.localPosition, result.Damage);
                hitCount++;
            }

            if (hitCount <= 0)
            {
                return;
            }

            ShowSplashFeedback(impactPosition, definition.SplashRadius);
            RecordEvent($"{definition.Id} splash hit {hitCount} alien(s).");
        }

        private List<TargetCandidate> BuildTargetCandidates(RuntimeTower tower)
        {
            var candidates = new List<TargetCandidate>();
            var towerPosition = ToWorldPosition(tower.Coordinate, 0.35f);

            foreach (var alien in activeAliens)
            {
                if (!alien.State.IsAlive)
                {
                    continue;
                }

                var distance = Vector3.Distance(towerPosition, alien.GameObject.transform.localPosition) / cellSize;

                if (distance <= tower.State.Definition.Range)
                {
                    candidates.Add(new TargetCandidate(alien.State, alien.PathProgress, distance));
                }
            }

            return candidates;
        }

        private RuntimeAlien FindRuntimeAlien(AlienState state)
        {
            foreach (var alien in activeAliens)
            {
                if (ReferenceEquals(alien.State, state))
                {
                    return alien;
                }
            }

            return null;
        }

        private RuntimeTower FindRuntimeTower(GridCoordinate coordinate)
        {
            foreach (var tower in towers)
            {
                if (tower.Coordinate == coordinate)
                {
                    return tower;
                }
            }

            return null;
        }

        private void CompleteWaveIfReady()
        {
            if (!waveActive
                || waveSpawnState == null
                || !waveSpawnState.HasCompletedSpawning
                || activeAliens.Count > 0
                || lost)
            {
                return;
            }

            completedWaves++;
            var wasPerfectWave = currentBase.CurrentHealth == waveStartBaseHealth;
            var reward = rewardCalculator.CalculateWaveClearReward(
                currentWaveDefinition,
                wasPerfectWave);
            wallet = wallet.Add(reward.Amount);
            runSummary = runSummary.RecordWaveCleared(reward.Amount, wasPerfectWave);
            RecordEvent(wasPerfectWave
                ? $"Wave cleared perfectly. +{reward.Amount} scrap."
                : $"Wave cleared. +{reward.Amount} scrap.");
            TrackAnalytics(
                "wave_cleared",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "wave_number", completedWaves.ToString() },
                    { "perfect_wave", wasPerfectWave.ToString() }
                });
            ShowCombatBanner(wasPerfectWave ? "Perfect Wave" : "Wave Clear");
            waveActive = false;
            currentWaveDefinition = null;
            waveSpawnState = null;
            runModifiers = runModifiers.EndWave();

            if (completedWaves >= SelectedLevel.TotalWaves)
            {
                won = true;
                runSummary = runSummary.Complete(RunOutcome.Victory);
                ApplyCompletedRunProgress();
                ShowCombatBanner(lastRunUnlockedLevel ? "Level Unlocked" : "Victory");
                RecordEvent("Victory. All waves cleared.");
                TrackLevelCompleted();
                return;
            }

            pendingRunChoices = runChoiceOfferSelector.SelectOffer(
                config.CreateRunChoiceDefinitions(),
                RunChoiceOfferSize);
            OfferRewardedAdIfAvailable(reward.Amount);
            RecordEvent("Choose a reward before the next wave.");
        }

        private void RebuildTiles()
        {
            ClearTiles();

            var bounds = new GridBounds(width, height);
            var grid = CreateGrid(bounds);
            var start = ToGridCoordinate(spawn);
            var end = ToGridCoordinate(goal);
            var shortestPath = pathfinder.FindShortestPath(grid, start, end);
            var path = new HashSet<GridCoordinate>(shortestPath);

            foreach (var coordinate in bounds.Coordinates())
            {
                var material = SelectMaterial(grid, coordinate, path, start, end);
                CreateTile(coordinate, material);
            }
        }

        private Material SelectMaterial(
            TacticalGrid grid,
            GridCoordinate coordinate,
            ISet<GridCoordinate> path,
            GridCoordinate start,
            GridCoordinate end)
        {
            if (coordinate == start)
            {
                return spawnCellMaterial;
            }

            if (coordinate == end)
            {
                return goalCellMaterial;
            }

            if (placedTowers.Contains(coordinate))
            {
                return towerCellMaterial;
            }

            if (grid.IsBlocked(coordinate))
            {
                return blockedCellMaterial;
            }

            if (path.Contains(coordinate))
            {
                return pathCellMaterial;
            }

            return openCellMaterial;
        }

        private void CreateTile(GridCoordinate coordinate, Material material)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = $"Cell_{coordinate.Column}_{coordinate.Row}";
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = ToWorldPosition(coordinate, 0);
            tile.transform.localScale = new Vector3(1, tileHeight, 1);

            var renderer = tile.GetComponent<Renderer>();

            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }

            var cellView = tile.AddComponent<DebugGridCellView>();
            cellView.Initialise(this, coordinate);
            tileObjects.Add(tile);
        }

        private GameObject CreateTowerObject(GridCoordinate coordinate, TowerDefinition definition)
        {
            var tower = DebugActorVisualFactory.CreateTower(
                transform,
                ToWorldPosition(coordinate, 0.45f),
                definition);
            tower.name = $"Debug Tower {coordinate}";
            towerObjects.Add(tower);
            return tower;
        }

        private static GameObject CreateIndicatorPart(
            Transform parent,
            PrimitiveType primitiveType,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Color colour)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            SetRendererColour(part, colour);
            return part;
        }

        private static void SetRendererColour(GameObject target, Color colour)
        {
            var renderer = target.GetComponent<Renderer>();

            if (renderer != null)
            {
                var material = renderer.material;
                material.color = colour;

                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", colour);
                }
            }
        }

        private DebugAlienVisualRole SelectAlienVisualRole(AlienDefinition definition)
        {
            if (definition.Id == config.FastAlienId)
            {
                return DebugAlienVisualRole.Fast;
            }

            if (definition.Id == config.ArmouredAlienId)
            {
                return DebugAlienVisualRole.Armoured;
            }

            if (definition.Id == config.ShieldedAlienId)
            {
                return DebugAlienVisualRole.Shielded;
            }

            if (definition.Id == config.BurrowerAlienId)
            {
                return DebugAlienVisualRole.Burrower;
            }

            if (definition.Id == config.RegeneratorAlienId)
            {
                return DebugAlienVisualRole.Regenerator;
            }

            if (definition.Id == config.BossAlienId)
            {
                return DebugAlienVisualRole.Boss;
            }

            return DebugAlienVisualRole.Basic;
        }

        private void UpdateTowerObject(RuntimeTower tower)
        {
            if (tower.GameObject == null)
            {
                return;
            }

            var levelBonus = (tower.State.Level - 1) * 0.16f;
            tower.GameObject.transform.localPosition = ToWorldPosition(tower.Coordinate, 0.45f + levelBonus * 0.25f);
            tower.GameObject.transform.localScale = Vector3.one * (1 + levelBonus);
            UpdateTowerUpgradeMarkers(tower);
        }

        private void UpdateTowerUpgradeMarkers(RuntimeTower tower)
        {
            if (tower.UpgradeMarkerRoot != null)
            {
                Destroy(tower.UpgradeMarkerRoot);
                tower.UpgradeMarkerRoot = null;
            }

            var markerCount = tower.State.Level - 1;

            if (markerCount <= 0)
            {
                return;
            }

            tower.UpgradeMarkerRoot = new GameObject("Upgrade Markers");
            tower.UpgradeMarkerRoot.transform.SetParent(tower.GameObject.transform, false);
            tower.UpgradeMarkerRoot.transform.localPosition = Vector3.zero;

            const float spacing = 0.16f;
            var startX = -spacing * (markerCount - 1) * 0.5f;

            for (var index = 0; index < markerCount; index++)
            {
                CreateIndicatorPart(
                    tower.UpgradeMarkerRoot.transform,
                    PrimitiveType.Cube,
                    "Upgrade Pip",
                    new Vector3(startX + index * spacing, 0.86f, -0.34f),
                    new Vector3(0.1f, 0.06f, 0.1f),
                    new Color(1f, 0.92f, 0.22f, 1f));
            }
        }

        private TacticalGrid CreateGrid(GridBounds bounds)
        {
            var blocked = new List<GridCoordinate>(ToGridCoordinates(blockedCells));
            blocked.AddRange(placedTowers);
            return new TacticalGrid(bounds, blocked);
        }

        private void ClearActors()
        {
            HidePlacementPreviewIfAny();

            foreach (var alien in activeAliens)
            {
                if (alien.GameObject != null)
                {
                    Destroy(alien.GameObject);
                }
            }

            foreach (var tower in towerObjects)
            {
                if (tower != null)
                {
                    Destroy(tower);
                }
            }

            foreach (var shot in shotObjects)
            {
                if (shot != null)
                {
                    Destroy(shot);
                }
            }

            activeAliens.Clear();
            towerObjects.Clear();
            shotObjects.Clear();
        }

        private void ShowShotFeedback(GridCoordinate towerCoordinate, RuntimeAlien alien)
        {
            var start = ToWorldPosition(towerCoordinate, 0.8f);
            var end = alien.GameObject.transform.localPosition;
            end.y = 0.45f;

            var shot = new GameObject("Debug Shot");
            shot.transform.SetParent(transform, false);

            var line = shot.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.useWorldSpace = false;
            line.widthMultiplier = 0.08f;
            line.SetPosition(0, start);
            line.SetPosition(1, end);

            if (shotLineMaterial != null)
            {
                line.sharedMaterial = shotLineMaterial;
            }

            shotObjects.Add(shot);
            Destroy(shot, 0.08f);

            if (alien.Renderer != null && alienHitMaterial != null)
            {
                FlashAlien(alien);
            }
        }

        private void ShowDamageFeedback(Vector3 position, DamageResult damage)
        {
            var textObject = new GameObject("Debug Damage Text");
            textObject.transform.SetParent(transform, false);
            textObject.transform.localPosition = new Vector3(position.x, 1.05f, position.z - 0.28f);
            textObject.transform.localRotation = Quaternion.Euler(62, 0, 0);

            var text = textObject.AddComponent<TextMesh>();
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.characterSize = 0.13f;
            text.fontSize = 56;
            text.text = BuildDamageFeedbackText(damage);
            text.color = SelectDamageFeedbackColour(damage);

            shotObjects.Add(textObject);
            Destroy(textObject, 0.45f);
        }

        private static string BuildDamageFeedbackText(DamageResult damage)
        {
            if (damage.WasDodged)
            {
                return "DODGE";
            }

            var amount = Mathf.CeilToInt(damage.FinalAmount);
            return damage.WasCritical ? $"CRIT {amount}" : amount.ToString();
        }

        private static Color SelectDamageFeedbackColour(DamageResult damage)
        {
            if (damage.WasDodged)
            {
                return new Color(0.72f, 0.82f, 1f, 1f);
            }

            if (damage.WasCritical)
            {
                return new Color(1f, 0.92f, 0.18f, 1f);
            }

            if (damage.Resistance > 0)
            {
                return new Color(0.92f, 0.68f, 0.46f, 1f);
            }

            return new Color(1f, 1f, 1f, 1f);
        }

        private void ShowSplashFeedback(Vector3 impactPosition, float radiusCells)
        {
            var splash = new GameObject("Debug Splash");
            splash.transform.SetParent(transform, false);

            var line = splash.AddComponent<LineRenderer>();
            line.loop = true;
            line.useWorldSpace = false;
            line.positionCount = 64;
            line.widthMultiplier = 0.045f;

            if (shotLineMaterial != null)
            {
                line.sharedMaterial = shotLineMaterial;
            }

            var radius = radiusCells * cellSize;
            var centre = impactPosition;
            centre.y = 0.48f;

            for (var index = 0; index < line.positionCount; index++)
            {
                var radians = index / (float)line.positionCount * Mathf.PI * 2;
                var x = centre.x + Mathf.Cos(radians) * radius;
                var z = centre.z + Mathf.Sin(radians) * radius;
                line.SetPosition(index, new Vector3(x, centre.y, z));
            }

            shotObjects.Add(splash);
            Destroy(splash, 0.12f);
        }

        private void ShowFreezePulseFeedback()
        {
            var pulse = new GameObject("Debug Freeze Pulse");
            pulse.transform.SetParent(transform, false);

            var line = pulse.AddComponent<LineRenderer>();
            line.loop = true;
            line.useWorldSpace = false;
            line.positionCount = 96;
            line.widthMultiplier = 0.08f;
            line.material = DebugActorVisualFactory.CreateDebugMaterial(new Color(0.12f, 0.85f, 1f, 0.95f));

            var centre = ToWorldPosition(
                new GridCoordinate(width / 2, height / 2),
                0.56f);
            var radius = Mathf.Max(width, height) * cellSize * 0.56f;

            for (var index = 0; index < line.positionCount; index++)
            {
                var radians = index / (float)line.positionCount * Mathf.PI * 2;
                var x = centre.x + Mathf.Cos(radians) * radius;
                var z = centre.z + Mathf.Sin(radians) * radius;
                line.SetPosition(index, new Vector3(x, centre.y, z));
            }

            shotObjects.Add(pulse);
            Destroy(pulse, 0.18f);
        }

        private void ShowOrbitalStrikeFeedback(Vector3 impactPosition)
        {
            var strike = new GameObject("Debug Orbital Strike");
            strike.transform.SetParent(transform, false);

            var line = strike.AddComponent<LineRenderer>();
            line.loop = true;
            line.useWorldSpace = false;
            line.positionCount = 48;
            line.widthMultiplier = 0.075f;
            line.material = DebugActorVisualFactory.CreateDebugMaterial(new Color(1f, 0.62f, 0.08f, 0.95f));

            var centre = impactPosition;
            centre.y = 0.72f;
            var radius = cellSize * 0.42f;

            for (var index = 0; index < line.positionCount; index++)
            {
                var radians = index / (float)line.positionCount * Mathf.PI * 2;
                var x = centre.x + Mathf.Cos(radians) * radius;
                var z = centre.z + Mathf.Sin(radians) * radius;
                line.SetPosition(index, new Vector3(x, centre.y, z));
            }

            shotObjects.Add(strike);
            Destroy(strike, 0.16f);
        }

        private void FlashAlien(RuntimeAlien alien)
        {
            if (alien.Renderer == null || alienHitMaterial == null)
            {
                return;
            }

            alien.Renderer.sharedMaterial = alienHitMaterial;
            alien.HitFlashSeconds = 0.08f;
        }

        private void EnsurePlacementPreview()
        {
            if (placementPreview != null)
            {
                return;
            }

            placementPreview = new GameObject("Placement Range Preview");
            placementPreview.transform.SetParent(transform, false);
            placementPreviewLine = placementPreview.AddComponent<LineRenderer>();
            placementPreviewLine.loop = true;
            placementPreviewLine.useWorldSpace = false;
            placementPreviewLine.positionCount = 72;
            placementPreviewLine.widthMultiplier = 0.035f;
            placementPreviewLine.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            ConfigurePreviewMaterial(placementPreviewLine.material);
            BuildPlacementPreviewCircle(SelectedTower.Range);
            placementPreview.SetActive(false);
        }

        private static void ConfigurePreviewMaterial(Material material)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty("_Surface"))
            {
                material.SetFloat("_Surface", 1);
            }

            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private void SetPreviewMaterialColour(Color colour)
        {
            if (placementPreviewLine.material == null)
            {
                return;
            }

            placementPreviewLine.material.color = colour;

            if (placementPreviewLine.material.HasProperty("_BaseColor"))
            {
                placementPreviewLine.material.SetColor("_BaseColor", colour);
            }

            if (placementPreviewLine.material.HasProperty("_Color"))
            {
                placementPreviewLine.material.SetColor("_Color", colour);
            }
        }

        private void BuildPlacementPreviewCircle(float range)
        {
            var radius = range * cellSize;

            for (var index = 0; index < placementPreviewLine.positionCount; index++)
            {
                var radians = index / (float)placementPreviewLine.positionCount * Mathf.PI * 2;
                var x = Mathf.Cos(radians) * radius;
                var z = Mathf.Sin(radians) * radius;
                placementPreviewLine.SetPosition(index, new Vector3(x, 0, z));
            }
        }

        private void HidePlacementPreviewIfAny()
        {
            previewCoordinate = null;

            if (placementPreview != null)
            {
                placementPreview.SetActive(false);
            }
        }

        private void ClearTiles()
        {
            for (var index = tileObjects.Count - 1; index >= 0; index--)
            {
                var tile = tileObjects[index];

                if (tile == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(tile);
                }
                else
                {
                    DestroyImmediate(tile);
                }
            }

            tileObjects.Clear();
        }

        private void OnGUI()
        {
            if (currentBase == null
                || wallet == null
                || levelDefinitions == null
                || towerUnlockState == null
                || campaignProgressStore == null
                || towerLoadoutPlans == null
                || towerLoadout == null
                || towerUpgradeLoadout == null
                || rewardCalculator == null
                || levelUnlockState == null
                || freezePulseState == null
                || orbitalStrikeState == null
                || shieldBurstState == null
                || towerOverchargeState == null
                || runSummary == null
                || gameSpeed == null
                || gamePause == null
                || analyticsRecorder == null
                || rewardedAdOfferTracker == null)
            {
                return;
            }

            const int left = 16;
            var top = 16;
            GUI.Box(new Rect(left, top, 280, 856), "Project 147 First Slice");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Base: {currentBase.CurrentHealth}/{currentBase.MaxHealth}  Shield: {currentBase.CurrentShield}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Level: {SelectedLevelLayout.Id}");
            top += 24;

            var previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost && towers.Count == 0 && completedWaves == 0;

            if (GUI.Button(new Rect(left + 12, top, 54, 24), "<"))
            {
                SelectPreviousLevelLayout();
            }

            if (GUI.Button(new Rect(left + 74, top, 54, 24), ">"))
            {
                SelectNextLevelLayout();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 2, 130, 24), $"{selectedLevelLayoutIndex + 1}/{levelDefinitions.Count}");
            top += 30;
            GUI.Label(new Rect(left + 12, top, 260, 24), BuildLevelUnlockStatusText());
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Loadout: {towerLoadoutPlans.SelectedPlan.Id}");
            top += 24;

            previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost && towers.Count == 0 && completedWaves == 0;

            if (GUI.Button(new Rect(left + 12, top, 54, 24), "<"))
            {
                SelectPreviousTowerLoadoutPlan();
            }

            if (GUI.Button(new Rect(left + 74, top, 54, 24), ">"))
            {
                SelectNextTowerLoadoutPlan();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 2, 130, 24), $"{towerLoadoutPlans.SelectedIndex + 1}/{towerLoadoutPlans.Plans.Count}");
            top += 30;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Unlocked towers: {towerUnlockState.UnlockedTowerIds.Count}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Scrap: {wallet.Balance}  {BuildTowerCostText()}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Selected: {SelectedTower.Id}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Dmg {SelectedTower.Damage}  Rng {SelectedTower.Range}  Rate {SelectedTower.FireRatePerSecond}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), BuildSelectedTowerAbilityText());
            top += 30;

            previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost;

            if (GUI.Button(new Rect(left + 12, top, 54, 24), "<"))
            {
                SelectPreviousTower();
            }

            if (GUI.Button(new Rect(left + 74, top, 54, 24), ">"))
            {
                SelectNextTower();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 2, 130, 24), $"{towerLoadout.SelectedIndex + 1}/{towerLoadout.Towers.Count}");
            top += 30;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Upgrade path: {SelectedTowerUpgrade.Id}");
            top += 24;

            previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost;

            if (GUI.Button(new Rect(left + 12, top, 54, 24), "<"))
            {
                SelectPreviousTowerUpgrade();
            }

            if (GUI.Button(new Rect(left + 74, top, 54, 24), ">"))
            {
                SelectNextTowerUpgrade();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 2, 130, 24), $"Path {towerUpgradeLoadout.SelectedIndex + 1}/{towerUpgradeLoadout.Upgrades.Count}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Upgrade cost: {SelectedTowerUpgrade.Cost}  Max level: {config.MaxTowerLevel}");
            top += 30;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Perfect wave bonus: {SelectedLevel.PerfectWaveScrapBonus}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Wave: {completedWaves}/{SelectedLevel.TotalWaves}  Alien cap: L{config.MaxAlienLevel}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Active aliens: {activeAliens.Count}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), BuildRunModifierStatusText());
            top += 30;

            previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost && !HasPendingRunChoice;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), sellMode ? "Sell Mode: On" : "Sell Mode: Off"))
            {
                sellMode = !sellMode;
                retargetMode = false;
                HidePlacementPreviewIfAny();
                RecordEvent(sellMode ? "Sell mode enabled." : "Sell mode disabled.");
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), $"Refund {TowerSellRefundMultiplier:P0}");
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost && !HasPendingRunChoice;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), retargetMode ? "Target: On" : "Target: Off"))
            {
                retargetMode = !retargetMode;
                sellMode = false;
                HidePlacementPreviewIfAny();
                RecordEvent(retargetMode ? "Retarget mode enabled." : "Retarget mode disabled.");
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), "Click tower");
            top += 34;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildSpeedButtonText()))
            {
                SelectNextGameSpeed();
            }

            GUI.Label(new Rect(left + 144, top + 4, 130, 24), "Wave speed");
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = waveActive && !won && !lost;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildPauseButtonText()))
            {
                TogglePause();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), gamePause.IsPaused ? "Frozen" : "Running");
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = waveActive
                && !won
                && !lost
                && !gamePause.IsPaused
                && activeAliens.Count > 0
                && freezePulseState.CanActivate;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildFreezePulseButtonText()))
            {
                TryActivateFreezePulse();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), BuildFreezePulseStatusText());
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = waveActive
                && !won
                && !lost
                && !gamePause.IsPaused
                && activeAliens.Count > 0
                && orbitalStrikeState.CanActivate;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildOrbitalStrikeButtonText()))
            {
                TryActivateOrbitalStrike();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), BuildOrbitalStrikeStatusText());
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = waveActive
                && !won
                && !lost
                && !gamePause.IsPaused
                && shieldBurstState.CanActivate;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildShieldBurstButtonText()))
            {
                TryActivateShieldBurst();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), BuildShieldBurstStatusText());
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = waveActive
                && !won
                && !lost
                && !gamePause.IsPaused
                && towers.Count > 0
                && towerOverchargeState.CanActivate;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildTowerOverchargeButtonText()))
            {
                TryActivateTowerOvercharge();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), BuildTowerOverchargeStatusText());
            top += 34;

            var startWaveEnabled = !waveActive && !won && !lost && !HasPendingRunChoice && !HasPendingRewardedAdOffer;
            var previousStartWaveEnabled = GUI.enabled;
            GUI.enabled = startWaveEnabled;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), "Start Wave"))
            {
                StartNextWave();
            }

            GUI.enabled = previousStartWaveEnabled;

            if (GUI.Button(new Rect(left + 144, top, 120, 28), "Restart"))
            {
                ResetSlice();
            }

            top += 34;

            var status = HasPendingRunChoice
                ? "Choose reward"
                : HasPendingRewardedAdOffer
                    ? "Choose fake ad offer"
                : waveActive
                    ? gamePause.IsPaused ? "Paused" : "Wave running"
                    : sellMode
                        ? "Click tower to sell"
                        : retargetMode ? "Click tower to retarget" : "Place towers, then start wave";

            if (won)
            {
                status = "Victory";
            }
            else if (lost)
            {
                status = "Defeat";
            }

            GUI.Label(new Rect(left + 12, top, 260, 24), status);
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), BuildInstrumentationStatusText());
            DrawEventFeed(left + 296, 16);
            DrawWaveIntelPanel(left + 296, 232);
            DrawRunChoicePanel(left + 296, 444);
            DrawRewardedAdOfferPanel(left + 296, HasPendingRunChoice ? 592 : 444);
            DrawRunSummaryPanel(left + 296, 232);
            DrawSessionProgressPanel(left + 296, BuildSessionProgressPanelTop());
            DrawInspectedTowerPanel(left + 296, BuildInspectedTowerPanelTop());
            DrawCombatBanner();
        }

        private string BuildSelectedTowerAbilityText()
        {
            if (SelectedTower.SplashRadius > 0 && SelectedTower.SplashDamageMultiplier > 0)
            {
                return $"Ability: splash {SelectedTower.SplashRadius} cells at {SelectedTower.SplashDamageMultiplier:P0} dmg";
            }

            if (SelectedTower.StatusEffects.Count > 0)
            {
                var effect = SelectedTower.StatusEffects[0];

                if (effect.Type == AlienStatusEffectType.Poison)
                {
                    return $"Ability: poison {effect.DurationSeconds:0.#}s at {effect.DamagePerSecond:0.#}/s";
                }

                return $"Ability: slow {effect.DurationSeconds:0.#}s at {effect.MovementSpeedMultiplier:P0} speed";
            }

            return "Ability: none";
        }

        private string BuildFreezePulseButtonText()
        {
            return freezePulseState.CanActivate
                ? "Freeze Pulse"
                : $"Freeze {Mathf.CeilToInt(freezePulseState.RemainingCooldownSeconds)}s";
        }

        private string BuildSpeedButtonText()
        {
            return $"Speed {gameSpeed.Multiplier:0.#}x";
        }

        private string BuildPauseButtonText()
        {
            return gamePause.IsPaused ? "Resume" : "Pause";
        }

        private string BuildFreezePulseStatusText()
        {
            return freezePulseState.CanActivate
                ? "Ability ready"
                : $"Cooldown {freezePulseState.RemainingCooldownSeconds:0.0}s";
        }

        private string BuildOrbitalStrikeButtonText()
        {
            return orbitalStrikeState.CanActivate
                ? "Orbital Strike"
                : $"Strike {Mathf.CeilToInt(orbitalStrikeState.RemainingCooldownSeconds)}s";
        }

        private string BuildOrbitalStrikeStatusText()
        {
            return orbitalStrikeState.CanActivate
                ? $"{orbitalStrikeState.Definition.DamageAmount:0} {orbitalStrikeState.Definition.DamageType} dmg"
                : $"Cooldown {orbitalStrikeState.RemainingCooldownSeconds:0.0}s";
        }

        private string BuildShieldBurstButtonText()
        {
            return shieldBurstState.CanActivate
                ? "Shield Burst"
                : $"Shield {Mathf.CeilToInt(shieldBurstState.RemainingCooldownSeconds)}s";
        }

        private string BuildShieldBurstStatusText()
        {
            return shieldBurstState.CanActivate
                ? $"+{shieldBurstState.Definition.BaseShieldAmount} base shield"
                : $"Cooldown {shieldBurstState.RemainingCooldownSeconds:0.0}s";
        }

        private string BuildTowerOverchargeButtonText()
        {
            return towerOverchargeState.CanActivate
                ? "Overcharge"
                : $"Charge {Mathf.CeilToInt(towerOverchargeState.RemainingCooldownSeconds)}s";
        }

        private string BuildTowerOverchargeStatusText()
        {
            return towerOverchargeState.CanActivate
                ? $"+{towerOverchargeState.Definition.TowerDamagePercent}% dmg, +{towerOverchargeState.Definition.TowerFireRatePercent}% rate"
                : $"Cooldown {towerOverchargeState.RemainingCooldownSeconds:0.0}s";
        }

        private string BuildInstrumentationStatusText()
        {
            return $"Analytics: {analyticsRecorder.Records.Count}  Fake ads: {rewardedAdOfferTracker.TotalOffers}";
        }

        private int BuildSessionProgressPanelTop()
        {
            if (won || lost)
            {
                return 444;
            }

            var top = HasPendingRunChoice ? 592 : 444;
            return HasPendingRewardedAdOffer ? top + 112 : top;
        }

        private int BuildInspectedTowerPanelTop()
        {
            return BuildSessionProgressPanelTop() + 206;
        }

        private void DrawRunChoicePanel(int left, int top)
        {
            if (!HasPendingRunChoice)
            {
                return;
            }

            var panelHeight = 46 + pendingRunChoices.Count * 30;
            GUI.Box(new Rect(left, top, 360, panelHeight), "Choose Reward");
            top += 30;

            for (var index = 0; index < pendingRunChoices.Count; index++)
            {
                var choice = pendingRunChoices[index];

                if (GUI.Button(new Rect(left + 12, top, 336, 26), BuildRunChoiceButtonText(choice)))
                {
                    SelectRunChoice(index);
                }

                top += 30;
            }
        }

        private void DrawRewardedAdOfferPanel(int left, int top)
        {
            if (!HasPendingRewardedAdOffer || won || lost)
            {
                return;
            }

            GUI.Box(new Rect(left, top, 360, 104), "Fake Rewarded Ad");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"+{pendingRewardedAdScrapAmount} scrap after a pretend ad.");
            top += 26;

            if (GUI.Button(new Rect(left + 12, top, 164, 26), "Claim Fake Ad Reward"))
            {
                ClaimPendingRewardedAdOffer();
            }

            if (GUI.Button(new Rect(left + 184, top, 164, 26), "Skip"))
            {
                SkipPendingRewardedAdOffer();
            }
        }

        private void DrawWaveIntelPanel(int left, int top)
        {
            if (won || lost || waveActive || completedWaves >= SelectedLevel.TotalWaves)
            {
                return;
            }

            var nextWave = config.CreateWaveDefinition(SelectedLevel, completedWaves);
            var intel = waveIntelBuilder.Build(
                completedWaves,
                nextWave,
                config.FastAlienId,
                config.ArmouredAlienId,
                config.BossAlienId,
                config.ShieldedAlienId,
                config.BurrowerAlienId,
                config.RegeneratorAlienId);

            var panelHeight = intel.TraitHints.Count > 0 ? 200 : 156;
            GUI.Box(new Rect(left, top, 360, panelHeight), "Next Wave");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Wave {intel.WaveNumber}: {intel.TotalAliens} aliens  Reward {intel.ClearReward}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Threat: {intel.ThreatRating}/5");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Types: {BuildWaveIntelEntriesText(intel)}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Tags: {BuildWaveIntelTagsText(intel)}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), BuildWaveCounterHintsText(intel));

            if (intel.TraitHints.Count > 0)
            {
                top += 22;
                GUI.Label(new Rect(left + 12, top, 336, 44), BuildWaveTraitHintsText(intel));
            }
        }

        private void DrawRunSummaryPanel(int left, int top)
        {
            if (!won && !lost)
            {
                return;
            }

            GUI.Box(new Rect(left, top, 360, 202), "Run Summary");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Outcome: {runSummary.Outcome}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Stars: {runSummary.StarRating}/3");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Waves: {runSummary.WavesCleared}/{SelectedLevel.TotalWaves}  Perfect: {runSummary.PerfectWaves}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Aliens destroyed: {runSummary.AliensDestroyed}  Leaked: {runSummary.AliensLeaked}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Scrap earned: {runSummary.ScrapEarned}  Rewards: {runSummary.RewardsChosen}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Abilities: Freeze {runSummary.FreezePulseUses}  Strike {runSummary.OrbitalStrikeUses}  Shield {runSummary.ShieldBurstUses}  Charge {runSummary.TowerOverchargeUses}");
        }

        private void DrawSessionProgressPanel(int left, int top)
        {
            if (campaignProgress == null || levelDefinitions == null)
            {
                return;
            }

            var levelProgress = campaignProgress.GetProgress(SelectedLevelLayout.Id);

            GUI.Box(new Rect(left, top, 260, 190), "Session Progress");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 236, 22), FormatProgressLevelLabel(SelectedLevelLayout.Id));
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Runs: {levelProgress.RunsCompleted}  Victories: {levelProgress.Victories}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Best stars: {levelProgress.BestStars}/3");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Best waves: {levelProgress.BestWavesCleared}/{SelectedLevel.TotalWaves}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Perfect waves: {levelProgress.BestPerfectWaves}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), BuildProgressStatusText());
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), saveStatusText ?? "No save status");
            top += 24;

            var previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost;

            if (GUI.Button(new Rect(left + 12, top, 116, 24), "Reset Save"))
            {
                ResetSavedCampaignProgress();
            }

            GUI.enabled = previousEnabled;
        }

        private void DrawInspectedTowerPanel(int left, int top)
        {
            if (!inspectedTowerCoordinate.HasValue)
            {
                return;
            }

            var tower = FindRuntimeTower(inspectedTowerCoordinate.Value);

            if (tower == null)
            {
                inspectedTowerCoordinate = null;
                return;
            }

            GUI.Box(new Rect(left, top, 260, 178), "Tower Detail");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Cell: {tower.Coordinate}  Level: {tower.State.Level}/{config.MaxTowerLevel}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), BuildTowerStatsText(tower.State));
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Targeting: {tower.State.TargetingMode}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Invested: {tower.State.TotalSpend}  Upgrades used: {tower.State.UpgradeHistory.Count}/{config.MaxTowerLevel - 1}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 44), BuildTowerUpgradeHistoryText(tower.State));
        }

        private void DrawEventFeed(int left, int top)
        {
            if (eventFeed == null)
            {
                return;
            }

            GUI.Box(new Rect(left, top, 360, 204), "Event Feed");
            top += 28;

            foreach (var entry in eventFeed.Entries)
            {
                GUI.Label(new Rect(left + 12, top, 336, 22), entry);
                top += 22;
            }
        }

        private void DrawCombatBanner()
        {
            if (string.IsNullOrWhiteSpace(combatBannerText) || combatBannerSeconds <= 0)
            {
                return;
            }

            var width = 320;
            GUI.Box(new Rect(Screen.width * 0.5f - width * 0.5f, 18, width, 42), combatBannerText);
        }

        private void RecordEvent(string message)
        {
            eventFeed = (eventFeed ?? new LevelEventFeed(EventFeedCapacity)).Add(message);
            UnityEngine.Debug.Log(message);
        }

        private void TrackAbilityUsed(string abilityId)
        {
            TrackAnalytics(
                "ability_used",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "ability_id", abilityId },
                    { "wave_number", (completedWaves + 1).ToString() }
                });
        }

        private void TrackLevelCompleted()
        {
            TrackAnalytics(
                "level_completed",
                new Dictionary<string, string>
                {
                    { "level_id", SelectedLevelLayout.Id },
                    { "outcome", runSummary.Outcome.ToString() },
                    { "stars", runSummary.StarRating.ToString() },
                    { "waves_cleared", runSummary.WavesCleared.ToString() }
                });
        }

        private void TrackAnalytics(string eventName, IReadOnlyDictionary<string, string> properties)
        {
            if (analyticsRecorder == null)
            {
                return;
            }

            try
            {
                analyticsRecorder.Track(eventName, properties);
            }
            catch (System.Exception exception)
            {
                UnityEngine.Debug.LogWarning($"Analytics event rejected: {exception.Message}");
            }
        }

        private void ShowCombatBanner(string message)
        {
            combatBannerText = message;
            combatBannerSeconds = 1.8f;
        }

        private void TickCombatBanner(float deltaSeconds)
        {
            if (combatBannerSeconds <= 0)
            {
                return;
            }

            combatBannerSeconds = Mathf.Max(0, combatBannerSeconds - deltaSeconds);
        }

        private void RefreshLevelUnlockState()
        {
            levelUnlockState = new CampaignLevelUnlockState(
                config.CreateLevelUnlockRules(),
                campaignProgress ?? new CampaignProgressState());
        }

        private void SelectFirstUnlockedLevelIfCurrentIsLocked()
        {
            if (IsLevelUnlocked(SelectedLevelLayout.Id))
            {
                return;
            }

            for (var index = 0; index < levelDefinitions.Count; index++)
            {
                if (IsLevelUnlocked(levelDefinitions[index].Layout.Id))
                {
                    selectedLevelLayoutIndex = index;
                    return;
                }
            }

            selectedLevelLayoutIndex = 0;
        }

        private bool IsLevelUnlocked(string levelId)
        {
            return levelUnlockState == null || levelUnlockState.IsUnlocked(levelId);
        }

        private void ApplyCompletedRunProgress()
        {
            var previousUnlockedCount = levelUnlockState.UnlockedLevelIds.Count;
            var result = campaignProgress.ApplyRunSummary(SelectedLevelLayout.Id, runSummary);
            campaignProgress = result.Campaign;
            lastProgressResult = result.LevelResult;
            RefreshLevelUnlockState();
            lastRunUnlockedLevel = levelUnlockState.UnlockedLevelIds.Count > previousUnlockedCount;
            SaveCampaignProgress();

            if (lastRunUnlockedLevel)
            {
                RecordEvent("New level unlocked.");
            }
        }

        private void SaveCampaignProgress()
        {
            try
            {
                campaignProgressStore.Save(campaignProgress);
                saveStatusText = "Progress saved";
            }
            catch (System.Exception exception)
            {
                UnityEngine.Debug.LogWarning($"Could not save campaign progress: {exception.Message}");
                saveStatusText = "Save failed";
            }
        }

        private void ResetSavedCampaignProgress()
        {
            campaignProgressStore.Clear();
            campaignProgress = new CampaignProgressState();
            lastProgressResult = null;
            RefreshLevelUnlockState();
            selectedLevelLayoutIndex = 0;
            saveStatusText = "Saved progress reset";
            ResetSlice();
            RecordEvent("Saved campaign progress reset.");
        }

        private bool HasPendingRunChoice
        {
            get { return pendingRunChoices != null && pendingRunChoices.Count > 0; }
        }

        private bool HasPendingRewardedAdOffer
        {
            get { return pendingRewardedAdOpportunity != null; }
        }

        private static string BuildRunChoiceButtonText(RunChoiceDefinition choice)
        {
            return $"{choice.Label}: {FormatRunChoiceEffect(choice)}";
        }

        private static string BuildTowerUpgradeHistoryText(TowerState tower)
        {
            return tower.UpgradeHistory.Count == 0
                ? "Paths: none"
                : $"Paths: {string.Join(", ", tower.UpgradeHistory)}";
        }

        private static string BuildTowerStatsText(TowerState tower)
        {
            return $"Dmg {tower.Definition.Damage:0.#}  Rng {tower.Definition.Range:0.##}  Rate {tower.Definition.FireRatePerSecond:0.##}";
        }

        private static string FormatRunChoiceEffect(RunChoiceDefinition choice)
        {
            switch (choice.EffectType)
            {
                case RunChoiceEffectType.AddScrap:
                    return $"+{choice.Amount} scrap";
                case RunChoiceEffectType.RepairBase:
                    return $"+{choice.Amount} base";
                case RunChoiceEffectType.AddNextTowerDiscount:
                    return $"-{choice.Amount} next tower";
                case RunChoiceEffectType.AddNextWaveTowerDamagePercent:
                    return $"+{choice.Amount}% next wave dmg";
                case RunChoiceEffectType.AddNextWaveTowerFireRatePercent:
                    return $"+{choice.Amount}% next wave rate";
                default:
                    return $"{choice.Amount}";
            }
        }

        private int GetSelectedTowerCost()
        {
            return runModifiers == null
                ? SelectedTower.Cost
                : runModifiers.CalculateTowerCost(SelectedTower.Cost);
        }

        private string BuildTowerCostText()
        {
            var currentCost = GetSelectedTowerCost();

            return currentCost == SelectedTower.Cost
                ? $"Tower cost: {currentCost}"
                : $"Tower cost: {currentCost} (was {SelectedTower.Cost})";
        }

        private string BuildRunModifierStatusText()
        {
            if (runModifiers == null)
            {
                return "Modifiers: none";
            }

            var parts = new List<string>();

            if (runModifiers.HasNextTowerDiscount)
            {
                parts.Add($"-{runModifiers.NextTowerDiscountAmount} next tower");
            }

            if (runModifiers.HasPendingWaveTowerDamageBoost)
            {
                parts.Add($"+{runModifiers.PendingWaveTowerDamagePercent}% next wave dmg");
            }

            if (runModifiers.HasPendingWaveTowerFireRateBoost)
            {
                parts.Add($"+{runModifiers.PendingWaveTowerFireRatePercent}% next wave rate");
            }

            if (runModifiers.HasActiveWaveTowerDamageBoost)
            {
                parts.Add($"+{runModifiers.ActiveWaveTowerDamagePercent}% wave dmg");
            }

            if (runModifiers.HasActiveWaveTowerFireRateBoost)
            {
                parts.Add($"+{runModifiers.ActiveWaveTowerFireRatePercent}% wave rate");
            }

            return parts.Count == 0
                ? "Modifiers: none"
                : $"Modifiers: {string.Join(", ", parts)}";
        }

        private TowerDefinition BuildEffectiveTowerDefinition(TowerDefinition definition)
        {
            if (runModifiers == null
                || (!runModifiers.HasActiveWaveTowerDamageBoost
                    && !runModifiers.HasActiveWaveTowerFireRateBoost))
            {
                return definition;
            }

            return new TowerDefinition(
                definition.Id,
                definition.Cost,
                definition.Range,
                definition.FireRatePerSecond * runModifiers.ActiveWaveTowerFireRateMultiplier,
                definition.Damage * runModifiers.ActiveWaveTowerDamageMultiplier,
                definition.DamageType,
                definition.DefaultTargetingMode,
                definition.CriticalChance,
                definition.CriticalDamageMultiplier,
                definition.SplashRadius,
                definition.SplashDamageMultiplier,
                definition.StatusEffects);
        }

        private static string BuildWaveIntelEntriesText(WaveIntelSummary intel)
        {
            var parts = new List<string>();

            foreach (var entry in intel.Entries)
            {
                parts.Add($"{entry.Count} {FormatAlienLabel(entry.AlienId)}");
            }

            return string.Join(", ", parts);
        }

        private static string BuildWaveIntelTagsText(WaveIntelSummary intel)
        {
            return intel.Tags.Count == 0
                ? "Basic"
                : string.Join(", ", intel.Tags);
        }

        private static string BuildWaveCounterHintsText(WaveIntelSummary intel)
        {
            var hints = new List<string>();

            if (intel.Tags.Contains("Shielded"))
            {
                hints.Add("energy vs shields");
            }

            if (intel.Tags.Contains("Regenerator"))
            {
                hints.Add("poison pressure");
            }

            if (intel.Tags.Contains("Burrower"))
            {
                hints.Add("path depth");
            }

            if (intel.Tags.Contains("Armoured"))
            {
                hints.Add("explosive or crits");
            }

            if (intel.Tags.Contains("Boss"))
            {
                hints.Add("upgrade focus");
            }

            return hints.Count == 0
                ? "Counters: standard"
                : $"Counters: {string.Join(", ", hints)}";
        }

        private static string BuildWaveTraitHintsText(WaveIntelSummary intel)
        {
            return intel.TraitHints.Count == 0
                ? "Traits: none"
                : $"Traits: {string.Join(" ", intel.TraitHints)}";
        }

        private string BuildProgressStatusText()
        {
            if (lastProgressResult == null)
            {
                return "No completed runs yet";
            }

            return lastProgressResult.HasAnyImprovement
                ? "New best recorded"
                : "No new best this run";
        }

        private string BuildLevelUnlockStatusText()
        {
            return $"Stars: {campaignProgress.TotalStars}  Levels unlocked: {levelUnlockState.UnlockedLevelIds.Count}/{levelDefinitions.Count}";
        }

        private static string FormatProgressLevelLabel(string levelId)
        {
            const string debugPrefix = "debug-";
            var label = levelId != null && levelId.StartsWith(debugPrefix)
                ? levelId.Substring(debugPrefix.Length)
                : levelId;

            return $"Level progress: {label}";
        }

        private string BuildWaveSummary(WaveDefinition wave)
        {
            var counts = new Dictionary<string, int>();

            foreach (var entry in wave.SpawnEntries)
            {
                if (!counts.ContainsKey(entry.AlienId))
                {
                    counts.Add(entry.AlienId, 0);
                }

                counts[entry.AlienId]++;
            }

            var parts = new List<string>();

            foreach (var count in counts)
            {
                parts.Add($"{count.Value} {FormatAlienLabel(count.Key)}");
            }

            return string.Join(", ", parts);
        }

        private static string FormatAlienLabel(string alienId)
        {
            const string debugPrefix = "debug-";

            return alienId != null && alienId.StartsWith(debugPrefix)
                ? alienId.Substring(debugPrefix.Length)
                : alienId;
        }

        private Vector3 ToWorldPosition(GridCoordinate coordinate, float y)
        {
            return new Vector3(coordinate.Column * cellSize, y, coordinate.Row * cellSize);
        }

        private static IEnumerable<GridCoordinate> ToGridCoordinates(IEnumerable<Vector2Int> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                yield return ToGridCoordinate(coordinate);
            }
        }

        private static GridCoordinate ToGridCoordinate(Vector2Int coordinate)
        {
            return new GridCoordinate(coordinate.x, coordinate.y);
        }

        private static Vector2Int ToVector2Int(GridCoordinate coordinate)
        {
            return new Vector2Int(coordinate.Column, coordinate.Row);
        }

        private sealed class RuntimeTower
        {
            public RuntimeTower(GridCoordinate coordinate, TowerState state, GameObject gameObject)
            {
                Coordinate = coordinate;
                State = state;
                GameObject = gameObject;
            }

            public GridCoordinate Coordinate { get; }

            public TowerState State { get; set; }

            public GameObject GameObject { get; }

            public GameObject UpgradeMarkerRoot { get; set; }
        }

        private sealed class RuntimeAlien
        {
            public RuntimeAlien(
                GameObject gameObject,
                AlienState state,
                IReadOnlyList<GridCoordinate> path,
                Renderer renderer,
                Material defaultMaterial)
            {
                GameObject = gameObject;
                State = state;
                Path = path;
                Renderer = renderer;
                DefaultMaterial = defaultMaterial;
                NextPathIndex = 1;
                PathProgress = 0;
            }

            public GameObject GameObject { get; }

            public AlienState State { get; set; }

            public IReadOnlyList<GridCoordinate> Path { get; }

            public Renderer Renderer { get; }

            public Material DefaultMaterial { get; }

            public int NextPathIndex { get; set; }

            public float PathProgress { get; set; }

            public float HitFlashSeconds { get; set; }

            public GameObject StatusVisualRoot { get; set; }

            public GameObject HealthBarBackground { get; set; }

            public GameObject HealthBarForeground { get; set; }

            public GameObject ShieldBarForeground { get; set; }

            public GameObject StatusMarker { get; set; }
        }
    }
}
