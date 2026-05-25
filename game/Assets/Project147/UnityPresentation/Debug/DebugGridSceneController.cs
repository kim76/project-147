using System.Collections.Generic;
using Project147.GameCore.Abilities;
using Project147.GameCore.Choices;
using Project147.GameCore.Combat;
using Project147.GameCore.Grid;
using Project147.GameCore.Level;
using Project147.GameData.Debug;
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
        private readonly RunChoiceResolver runChoiceResolver = new RunChoiceResolver();
        private readonly RunChoiceOfferSelector runChoiceOfferSelector =
            new RunChoiceOfferSelector(new RandomChoiceIndexPicker());
        private readonly WaveIntelBuilder waveIntelBuilder = new WaveIntelBuilder();
        private readonly GridPathfinder pathfinder = new GridPathfinder();
        private readonly TowerTargetSelector targetSelector = new TowerTargetSelector();
        private readonly TowerSaleCalculator towerSaleCalculator = new TowerSaleCalculator();
        private TowerPlacementValidator placementValidator;
        private AttackResolver attackResolver;
        private SplashDamageResolver splashDamageResolver;
        private PlayerAbilityState freezePulseState;
        private PlayerAbilityState orbitalStrikeState;
        private TowerLoadout towerLoadout;
        private TowerUpgradeDefinition towerUpgradeDefinition;
        private RewardCalculator rewardCalculator;
        private BaseState currentBase;
        private CurrencyWallet wallet;
        private RunModifierState runModifiers;
        private RunSummaryState runSummary;
        private LevelProgressState levelProgress;
        private LevelProgressApplicationResult lastProgressResult;
        private LevelEventFeed eventFeed;
        private bool waveActive;
        private bool won;
        private bool lost;
        private bool sellMode;
        private int completedWaves;
        private int waveStartBaseHealth;
        private WaveDefinition currentWaveDefinition;
        private WaveSpawnState waveSpawnState;
        private IReadOnlyList<RunChoiceDefinition> pendingRunChoices;
        private GameObject placementPreview;
        private LineRenderer placementPreviewLine;
        private GridCoordinate? previewCoordinate;

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

            if (freezePulseState != null)
            {
                freezePulseState = freezePulseState.Tick(Time.deltaTime);
            }

            if (orbitalStrikeState != null)
            {
                orbitalStrikeState = orbitalStrikeState.Tick(Time.deltaTime);
            }

            UpdateWaveSpawning(Time.deltaTime);
            UpdateAliens(Time.deltaTime);
            UpdateTowers(Time.deltaTime);
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
                if (sellMode)
                {
                    TrySellTower(coordinate);
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
        }

        public void ShowPlacementPreview(GridCoordinate coordinate)
        {
            if (towerLoadout == null || towerUpgradeDefinition == null || currentBase == null || wallet == null)
            {
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
                && tower.State.Level < config.MaxTowerLevel
                && wallet.CanSpend(towerUpgradeDefinition.Cost);
            var canSell = tower != null
                && !waveActive
                && !won
                && !lost
                && sellMode
                && !HasPendingRunChoice;
            var colour = canPlace || canUpgrade || canSell
                ? new Color(0.25f, 1f, 0.35f, 0.95f)
                : new Color(1f, 0.2f, 0.2f, 0.95f);
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
            ClearActors();
            ClearTiles();
            placedTowers.Clear();
            towers.Clear();
            currentBase = new BaseState(config.BaseHealth);
            wallet = new CurrencyWallet(config.StartingCurrency);
            runModifiers = new RunModifierState();
            runSummary = new RunSummaryState();
            freezePulseState = new PlayerAbilityState(config.CreateFreezePulseAbilityDefinition());
            orbitalStrikeState = new PlayerAbilityState(config.CreateOrbitalStrikeAbilityDefinition());
            eventFeed = new LevelEventFeed(EventFeedCapacity).Add("Ready. Place towers, then start wave.");
            waveActive = false;
            won = false;
            lost = false;
            sellMode = false;
            completedWaves = 0;
            waveStartBaseHealth = currentBase.CurrentHealth;
            currentWaveDefinition = null;
            waveSpawnState = null;
            pendingRunChoices = new List<RunChoiceDefinition>();
            RebuildTiles();
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
            rewardCalculator = new RewardCalculator(config.PerfectWaveScrapBonus);
            towerLoadout = new TowerLoadout(config.CreateTowerDefinitions());
            freezePulseState = new PlayerAbilityState(config.CreateFreezePulseAbilityDefinition());
            orbitalStrikeState = new PlayerAbilityState(config.CreateOrbitalStrikeAbilityDefinition());
            towerUpgradeDefinition = config.CreateTowerUpgradeDefinition();
            levelProgress = levelProgress ?? new LevelProgressState();
        }

        private TowerDefinition SelectedTower
        {
            get { return towerLoadout.SelectedTower; }
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

            if (!wallet.CanSpend(towerUpgradeDefinition.Cost))
            {
                RecordEvent("Not enough scrap for tower upgrade.");
                return;
            }

            wallet = wallet.Spend(towerUpgradeDefinition.Cost);
            tower.State = tower.State.Upgrade(towerUpgradeDefinition);
            UpdateTowerObject(tower);
            HidePlacementPreviewIfAny();
            RecordEvent($"Upgraded tower at {coordinate} to level {tower.State.Level}.");
        }

        private void TrySellTower(GridCoordinate coordinate)
        {
            var tower = FindRuntimeTower(coordinate);

            if (tower == null)
            {
                RecordEvent($"No tower found at {coordinate}.");
                return;
            }

            var refund = towerSaleCalculator.CalculateRefund(
                tower.State,
                towerUpgradeDefinition,
                TowerSellRefundMultiplier);
            wallet = wallet.Add(refund);
            towers.Remove(tower);
            placedTowers.Remove(coordinate);

            if (tower.GameObject != null)
            {
                towerObjects.Remove(tower.GameObject);
                Destroy(tower.GameObject);
            }

            RebuildTiles();
            HidePlacementPreviewIfAny();
            RecordEvent($"Sold tower at {coordinate}. +{refund} scrap.");
        }

        private void StartNextWave()
        {
            if (waveActive || won || lost || completedWaves >= config.TotalWaves)
            {
                return;
            }

            if (HasPendingRunChoice)
            {
                RecordEvent("Choose a reward before starting the next wave.");
                return;
            }

            waveActive = true;
            runModifiers = runModifiers.StartWave();
            waveStartBaseHealth = currentBase.CurrentHealth;
            currentWaveDefinition = config.CreateWaveDefinition(completedWaves);
            waveSpawnState = new WaveSpawnState(currentWaveDefinition);
            RecordEvent($"Wave {completedWaves + 1} started: {BuildWaveSummary(currentWaveDefinition)}.");
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

            activeAliens.Add(new RuntimeAlien(
                visual.GameObject,
                new AlienState(definition),
                path,
                visual.PrimaryRenderer,
                visual.DefaultMaterial));
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
                alien.State = alien.State.TickStatusEffects(deltaSeconds);

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
                        RecordEvent("Defeat. Base destroyed.");
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
                var selected = targetSelector.SelectTarget(candidates, tower.State.Definition.DefaultTargetingMode);

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
                tower.State = tower.State.MarkFired();

                if (!attack.Damage.WasDodged && attack.Damage.FinalAmount > 0 && alien.State.IsAlive)
                {
                    foreach (var statusEffect in effectiveTowerDefinition.StatusEffects)
                    {
                        alien.State = alien.State.ApplyStatusEffect(statusEffect);
                    }
                }

                ApplySplashDamage(tower, effectiveTowerDefinition, alien, attack.Damage);
                ShowShotFeedback(tower.Coordinate, alien);
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
            waveActive = false;
            currentWaveDefinition = null;
            waveSpawnState = null;
            runModifiers = runModifiers.EndWave();

            if (completedWaves >= config.TotalWaves)
            {
                won = true;
                runSummary = runSummary.Complete(RunOutcome.Victory);
                ApplyCompletedRunProgress();
                RecordEvent("Victory. All waves cleared.");
                return;
            }

            pendingRunChoices = runChoiceOfferSelector.SelectOffer(
                config.CreateRunChoiceDefinitions(),
                RunChoiceOfferSize);
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
            placementPreviewLine.widthMultiplier = 0.06f;
            placementPreviewLine.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            BuildPlacementPreviewCircle(SelectedTower.Range);
            placementPreview.SetActive(false);
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
                || towerLoadout == null
                || towerUpgradeDefinition == null
                || rewardCalculator == null
                || freezePulseState == null
                || orbitalStrikeState == null
                || runSummary == null)
            {
                return;
            }

            const int left = 16;
            var top = 16;
            GUI.Box(new Rect(left, top, 280, 458), "Project 147 First Slice");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Base: {currentBase.CurrentHealth}/{currentBase.MaxHealth}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Scrap: {wallet.Balance}  {BuildTowerCostText()}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Selected: {SelectedTower.Id}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Dmg {SelectedTower.Damage}  Rng {SelectedTower.Range}  Rate {SelectedTower.FireRatePerSecond}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), BuildSelectedTowerAbilityText());
            top += 30;

            if (!waveActive && !won && !lost && GUI.Button(new Rect(left + 12, top, 54, 24), "<"))
            {
                SelectPreviousTower();
            }

            if (!waveActive && !won && !lost && GUI.Button(new Rect(left + 74, top, 54, 24), ">"))
            {
                SelectNextTower();
            }

            GUI.Label(new Rect(left + 144, top + 2, 130, 24), $"{towerLoadout.SelectedIndex + 1}/{towerLoadout.Towers.Count}");
            top += 30;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Upgrade: {towerUpgradeDefinition.Cost}  Max tower level: {config.MaxTowerLevel}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Perfect wave bonus: {config.PerfectWaveScrapBonus}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Wave: {completedWaves}/{config.TotalWaves}  Alien cap: L{config.MaxAlienLevel}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Active aliens: {activeAliens.Count}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), BuildRunModifierStatusText());
            top += 30;

            var previousEnabled = GUI.enabled;
            GUI.enabled = !waveActive && !won && !lost && !HasPendingRunChoice;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), sellMode ? "Sell Mode: On" : "Sell Mode: Off"))
            {
                sellMode = !sellMode;
                HidePlacementPreviewIfAny();
                RecordEvent(sellMode ? "Sell mode enabled." : "Sell mode disabled.");
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), $"Refund {TowerSellRefundMultiplier:P0}");
            top += 34;

            previousEnabled = GUI.enabled;
            GUI.enabled = waveActive
                && !won
                && !lost
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
                && activeAliens.Count > 0
                && orbitalStrikeState.CanActivate;

            if (GUI.Button(new Rect(left + 12, top, 120, 28), BuildOrbitalStrikeButtonText()))
            {
                TryActivateOrbitalStrike();
            }

            GUI.enabled = previousEnabled;
            GUI.Label(new Rect(left + 144, top + 4, 130, 24), BuildOrbitalStrikeStatusText());
            top += 34;

            var startWaveEnabled = !waveActive && !won && !lost && !HasPendingRunChoice;
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
                : waveActive ? "Wave running" : sellMode ? "Click tower to sell" : "Place towers, then start wave";

            if (won)
            {
                status = "Victory";
            }
            else if (lost)
            {
                status = "Defeat";
            }

            GUI.Label(new Rect(left + 12, top, 260, 24), status);
            DrawEventFeed(left + 296, 16);
            DrawSessionProgressPanel(left + 668, 16);
            DrawWaveIntelPanel(left + 296, 232);
            DrawRunChoicePanel(left + 296, 356);
            DrawRunSummaryPanel(left + 296, 232);
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
                return $"Ability: {effect.Type} {effect.DurationSeconds:0.#}s";
            }

            return "Ability: none";
        }

        private string BuildFreezePulseButtonText()
        {
            return freezePulseState.CanActivate
                ? "Freeze Pulse"
                : $"Freeze {Mathf.CeilToInt(freezePulseState.RemainingCooldownSeconds)}s";
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

        private void DrawWaveIntelPanel(int left, int top)
        {
            if (won || lost || waveActive || completedWaves >= config.TotalWaves)
            {
                return;
            }

            var nextWave = config.CreateWaveDefinition(completedWaves);
            var intel = waveIntelBuilder.Build(
                completedWaves,
                nextWave,
                config.FastAlienId,
                config.ArmouredAlienId,
                config.BossAlienId,
                config.ShieldedAlienId,
                config.BurrowerAlienId);

            GUI.Box(new Rect(left, top, 360, 112), "Next Wave");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Wave {intel.WaveNumber}: {intel.TotalAliens} aliens  Reward {intel.ClearReward}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Types: {BuildWaveIntelEntriesText(intel)}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Tags: {BuildWaveIntelTagsText(intel)}");
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
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Waves: {runSummary.WavesCleared}/{config.TotalWaves}  Perfect: {runSummary.PerfectWaves}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Aliens destroyed: {runSummary.AliensDestroyed}  Leaked: {runSummary.AliensLeaked}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Scrap earned: {runSummary.ScrapEarned}  Rewards: {runSummary.RewardsChosen}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 336, 22), $"Abilities: Freeze {runSummary.FreezePulseUses}  Strike {runSummary.OrbitalStrikeUses}");
        }

        private void DrawSessionProgressPanel(int left, int top)
        {
            if (levelProgress == null)
            {
                return;
            }

            GUI.Box(new Rect(left, top, 260, 134), "Session Progress");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Runs: {levelProgress.RunsCompleted}  Victories: {levelProgress.Victories}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Best stars: {levelProgress.BestStars}/3");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Best waves: {levelProgress.BestWavesCleared}/{config.TotalWaves}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), $"Perfect waves: {levelProgress.BestPerfectWaves}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 236, 22), BuildProgressStatusText());
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

        private void RecordEvent(string message)
        {
            eventFeed = (eventFeed ?? new LevelEventFeed(EventFeedCapacity)).Add(message);
            UnityEngine.Debug.Log(message);
        }

        private void ApplyCompletedRunProgress()
        {
            lastProgressResult = levelProgress.ApplyRunSummary(runSummary);
            levelProgress = lastProgressResult.State;
        }

        private bool HasPendingRunChoice
        {
            get { return pendingRunChoices != null && pendingRunChoices.Count > 0; }
        }

        private static string BuildRunChoiceButtonText(RunChoiceDefinition choice)
        {
            return $"{choice.Label}: {FormatRunChoiceEffect(choice)}";
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
        }
    }
}
