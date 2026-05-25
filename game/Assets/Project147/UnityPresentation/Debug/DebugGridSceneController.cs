using System.Collections.Generic;
using Project147.GameCore.Abilities;
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
        private readonly GridPathfinder pathfinder = new GridPathfinder();
        private readonly TowerTargetSelector targetSelector = new TowerTargetSelector();
        private TowerPlacementValidator placementValidator;
        private AttackResolver attackResolver;
        private SplashDamageResolver splashDamageResolver;
        private PlayerAbilityState freezePulseState;
        private TowerLoadout towerLoadout;
        private TowerUpgradeDefinition towerUpgradeDefinition;
        private RewardCalculator rewardCalculator;
        private BaseState currentBase;
        private CurrencyWallet wallet;
        private LevelEventFeed eventFeed;
        private bool waveActive;
        private bool won;
        private bool lost;
        private int completedWaves;
        private int waveStartBaseHealth;
        private WaveDefinition currentWaveDefinition;
        private WaveSpawnState waveSpawnState;
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

            if (placedTowers.Contains(coordinate))
            {
                TryUpgradeTower(coordinate);
                return;
            }

            if (!wallet.CanSpend(SelectedTower.Cost))
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

            wallet = wallet.Spend(SelectedTower.Cost);
            placedTowers.Add(coordinate);
            var towerObject = CreateTowerObject(coordinate, SelectedTower);
            towers.Add(new RuntimeTower(coordinate, new TowerState(SelectedTower), towerObject));
            RebuildTiles();
            HidePlacementPreviewIfAny();
            RecordEvent($"Placed {SelectedTower.Id} at {coordinate}. Scrap: {wallet.Balance}.");
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
                && tower.State.Level < config.MaxTowerLevel
                && wallet.CanSpend(towerUpgradeDefinition.Cost);
            var colour = canPlace || canUpgrade
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
            freezePulseState = new PlayerAbilityState(config.CreateFreezePulseAbilityDefinition());
            eventFeed = new LevelEventFeed(EventFeedCapacity).Add("Ready. Place towers, then start wave.");
            waveActive = false;
            won = false;
            lost = false;
            completedWaves = 0;
            waveStartBaseHealth = currentBase.CurrentHealth;
            currentWaveDefinition = null;
            waveSpawnState = null;
            RebuildTiles();
        }

        private bool CanPreviewPlacement(GridCoordinate coordinate)
        {
            if (waveActive || won || lost || !wallet.CanSpend(SelectedTower.Cost))
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
            towerUpgradeDefinition = config.CreateTowerUpgradeDefinition();
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

        private void StartNextWave()
        {
            if (waveActive || won || lost || completedWaves >= config.TotalWaves)
            {
                return;
            }

            waveActive = true;
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
                    RecordEvent($"{FormatAlienLabel(alien.State.Definition.Id)} destroyed. +{reward.Amount} scrap.");
                    continue;
                }

                UpdateAlienHitFlash(alien, deltaSeconds);
                alien.State = alien.State.TickStatusEffects(deltaSeconds);

                if (MoveAlien(alien, deltaSeconds))
                {
                    currentBase = currentBase.ApplyLeakDamage(1);
                    Destroy(alien.GameObject);
                    activeAliens.RemoveAt(index);
                    RecordEvent($"{FormatAlienLabel(alien.State.Definition.Id)} leaked. Base: {currentBase.CurrentHealth}/{currentBase.MaxHealth}.");

                    if (currentBase.IsDestroyed)
                    {
                        lost = true;
                        waveActive = false;
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

                var attack = attackResolver.Resolve(tower.State.Definition, alien.State);
                alien.State = attack.Target;
                tower.State = tower.State.MarkFired();

                if (!attack.Damage.WasDodged && attack.Damage.FinalAmount > 0 && alien.State.IsAlive)
                {
                    foreach (var statusEffect in tower.State.Definition.StatusEffects)
                    {
                        alien.State = alien.State.ApplyStatusEffect(statusEffect);
                    }
                }

                ApplySplashDamage(tower, alien, attack.Damage);
                ShowShotFeedback(tower.Coordinate, alien);
            }
        }

        private void ApplySplashDamage(RuntimeTower tower, RuntimeAlien primaryAlien, DamageResult primaryDamage)
        {
            var definition = tower.State.Definition;

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
            RecordEvent(wasPerfectWave
                ? $"Wave cleared perfectly. +{reward.Amount} scrap."
                : $"Wave cleared. +{reward.Amount} scrap.");
            waveActive = false;
            currentWaveDefinition = null;
            waveSpawnState = null;

            if (completedWaves >= config.TotalWaves)
            {
                won = true;
                RecordEvent("Victory. All waves cleared.");
            }
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
                || freezePulseState == null)
            {
                return;
            }

            const int left = 16;
            var top = 16;
            GUI.Box(new Rect(left, top, 280, 384), "Project 147 First Slice");
            top += 28;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Base: {currentBase.CurrentHealth}/{currentBase.MaxHealth}");
            top += 22;
            GUI.Label(new Rect(left + 12, top, 260, 24), $"Scrap: {wallet.Balance}  Tower cost: {SelectedTower.Cost}");
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
            top += 30;

            var previousEnabled = GUI.enabled;
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

            if (!waveActive && !won && !lost && GUI.Button(new Rect(left + 12, top, 120, 28), "Start Wave"))
            {
                StartNextWave();
            }

            if (GUI.Button(new Rect(left + 144, top, 120, 28), "Restart"))
            {
                ResetSlice();
            }

            top += 34;

            var status = waveActive ? "Wave running" : "Place towers, then start wave";

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
