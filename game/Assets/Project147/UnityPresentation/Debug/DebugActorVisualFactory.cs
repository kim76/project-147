using Project147.GameCore.Combat;
using UnityEngine;

namespace Project147.UnityPresentation.Debug
{
    public static class DebugActorVisualFactory
    {
        public static GameObject CreateTower(
            Transform parent,
            Vector3 localPosition,
            TowerDefinition definition)
        {
            var tower = new GameObject($"Debug Tower {definition.Id}");
            tower.transform.SetParent(parent, false);
            tower.transform.localPosition = localPosition;

            if (definition.DamageType == DamageType.Explosive)
            {
                CreateMortarTower(tower.transform);
            }
            else if (definition.DamageType == DamageType.Energy)
            {
                CreateEnergyTower(tower.transform);
            }
            else
            {
                CreateRailgunTower(tower.transform);
            }

            return tower;
        }

        public static DebugActorVisual CreateAlien(
            Transform parent,
            Vector3 localPosition,
            AlienDefinition definition,
            DebugAlienVisualRole role,
            Material fallbackMaterial)
        {
            var alien = new GameObject($"Debug Alien {definition.Id}");
            alien.transform.SetParent(parent, false);
            alien.transform.localPosition = localPosition;

            switch (role)
            {
                case DebugAlienVisualRole.Fast:
                    return CreateFastAlien(alien, definition);
                case DebugAlienVisualRole.Armoured:
                    return CreateArmouredAlien(alien, definition);
                case DebugAlienVisualRole.Shielded:
                    return CreateShieldedAlien(alien, definition);
                case DebugAlienVisualRole.Burrower:
                    return CreateBurrowerAlien(alien, definition);
                case DebugAlienVisualRole.Boss:
                    return CreateBossAlien(alien, definition);
                default:
                    return CreateBasicAlien(alien, definition, fallbackMaterial);
            }
        }

        public static Material CreateDebugMaterial(Color colour)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = colour;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", colour);
            }

            return material;
        }

        private static void CreateRailgunTower(Transform parent)
        {
            CreatePart(
                parent,
                PrimitiveType.Cube,
                "Railgun Base",
                new Vector3(0, -0.17f, 0),
                new Vector3(0.64f, 0.18f, 0.64f),
                Vector3.zero,
                new Color(0.18f, 0.2f, 0.24f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cylinder,
                "Railgun Stem",
                new Vector3(0, 0.1f, 0),
                new Vector3(0.26f, 0.52f, 0.26f),
                Vector3.zero,
                new Color(0.94f, 0.12f, 0.24f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Sphere,
                "Railgun Core",
                new Vector3(0, 0.42f, 0),
                new Vector3(0.42f, 0.42f, 0.42f),
                Vector3.zero,
                new Color(1f, 0.22f, 0.36f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cube,
                "Railgun Barrel",
                new Vector3(0.32f, 0.46f, 0),
                new Vector3(0.56f, 0.11f, 0.16f),
                Vector3.zero,
                new Color(0.12f, 0.1f, 0.12f, 1f));
        }

        private static void CreateMortarTower(Transform parent)
        {
            CreatePart(
                parent,
                PrimitiveType.Cube,
                "Mortar Base",
                new Vector3(0, -0.12f, 0),
                new Vector3(0.68f, 0.28f, 0.68f),
                Vector3.zero,
                new Color(1f, 0.55f, 0.12f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cylinder,
                "Mortar Tube",
                new Vector3(0.08f, 0.28f, 0),
                new Vector3(0.34f, 0.52f, 0.34f),
                new Vector3(0, 0, -18),
                new Color(0.16f, 0.13f, 0.11f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cylinder,
                "Mortar Rim",
                new Vector3(0.23f, 0.55f, 0),
                new Vector3(0.42f, 0.08f, 0.42f),
                new Vector3(0, 0, -18),
                new Color(0.95f, 0.82f, 0.34f, 1f));
        }

        private static void CreateEnergyTower(Transform parent)
        {
            CreatePart(
                parent,
                PrimitiveType.Cube,
                "Energy Base",
                new Vector3(0, -0.16f, 0),
                new Vector3(0.58f, 0.2f, 0.58f),
                new Vector3(0, 35, 0),
                new Color(0.08f, 0.28f, 0.34f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cylinder,
                "Energy Coil",
                new Vector3(0, 0.18f, 0),
                new Vector3(0.24f, 0.58f, 0.24f),
                Vector3.zero,
                new Color(0.12f, 0.75f, 0.85f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Sphere,
                "Energy Core",
                new Vector3(0, 0.55f, 0),
                new Vector3(0.34f, 0.34f, 0.34f),
                Vector3.zero,
                new Color(0.9f, 1f, 0.34f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cube,
                "Energy Prong",
                new Vector3(0.24f, 0.38f, 0),
                new Vector3(0.12f, 0.32f, 0.12f),
                Vector3.zero,
                new Color(0.88f, 0.96f, 1f, 1f));
            CreatePart(
                parent,
                PrimitiveType.Cube,
                "Energy Prong",
                new Vector3(-0.24f, 0.38f, 0),
                new Vector3(0.12f, 0.32f, 0.12f),
                Vector3.zero,
                new Color(0.88f, 0.96f, 1f, 1f));
        }

        private static DebugActorVisual CreateBasicAlien(
            GameObject root,
            AlienDefinition definition,
            Material fallbackMaterial)
        {
            var material = CreateDebugMaterial(SelectFallbackColour(fallbackMaterial));
            var body = CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Basic Body",
                Vector3.zero,
                new Vector3(0.45f, 0.45f, 0.45f),
                Vector3.zero,
                material);
            CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Basic Eye",
                new Vector3(0.18f, 0.1f, -0.22f),
                new Vector3(0.12f, 0.12f, 0.12f),
                Vector3.zero,
                new Color(0.95f, 1f, 0.35f, 1f));
            return new DebugActorVisual(root, body.GetComponent<Renderer>(), material);
        }

        private static DebugActorVisual CreateFastAlien(GameObject root, AlienDefinition definition)
        {
            var material = CreateDebugMaterial(new Color(0.12f, 0.85f, 1f, 1f));
            var body = CreatePart(
                root.transform,
                PrimitiveType.Capsule,
                "Fast Body",
                Vector3.zero,
                new Vector3(0.32f, 0.5f, 0.32f),
                new Vector3(0, 0, 28),
                material);
            CreatePart(
                root.transform,
                PrimitiveType.Cube,
                "Fast Fin",
                new Vector3(-0.2f, 0.02f, 0.18f),
                new Vector3(0.12f, 0.16f, 0.34f),
                new Vector3(0, 0, 28),
                new Color(0.95f, 0.95f, 1f, 1f));
            return new DebugActorVisual(root, body.GetComponent<Renderer>(), material);
        }

        private static DebugActorVisual CreateArmouredAlien(GameObject root, AlienDefinition definition)
        {
            var material = CreateDebugMaterial(new Color(0.72f, 0.62f, 0.48f, 1f));
            var body = CreatePart(
                root.transform,
                PrimitiveType.Cube,
                "Armoured Body",
                Vector3.zero,
                new Vector3(0.52f, 0.46f, 0.52f),
                Vector3.zero,
                material);
            CreatePart(
                root.transform,
                PrimitiveType.Cube,
                "Armoured Shell",
                new Vector3(0, 0.25f, 0),
                new Vector3(0.62f, 0.12f, 0.62f),
                Vector3.zero,
                new Color(0.34f, 0.32f, 0.32f, 1f));
            CreatePart(
                root.transform,
                PrimitiveType.Cube,
                "Armoured Visor",
                new Vector3(0.02f, 0.06f, -0.28f),
                new Vector3(0.28f, 0.08f, 0.08f),
                Vector3.zero,
                new Color(0.2f, 0.95f, 0.58f, 1f));
            return new DebugActorVisual(root, body.GetComponent<Renderer>(), material);
        }

        private static DebugActorVisual CreateShieldedAlien(GameObject root, AlienDefinition definition)
        {
            var material = CreateDebugMaterial(new Color(0.18f, 0.62f, 0.88f, 1f));
            var body = CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Shielded Body",
                Vector3.zero,
                new Vector3(0.42f, 0.42f, 0.42f),
                Vector3.zero,
                material);
            CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Shield Shell",
                Vector3.zero,
                new Vector3(0.66f, 0.66f, 0.66f),
                Vector3.zero,
                new Color(0.72f, 0.98f, 1f, 0.78f));
            CreatePart(
                root.transform,
                PrimitiveType.Cube,
                "Shielded Visor",
                new Vector3(0, 0.06f, -0.31f),
                new Vector3(0.26f, 0.08f, 0.08f),
                Vector3.zero,
                new Color(1f, 0.95f, 0.24f, 1f));
            return new DebugActorVisual(root, body.GetComponent<Renderer>(), material);
        }

        private static DebugActorVisual CreateBurrowerAlien(GameObject root, AlienDefinition definition)
        {
            var material = CreateDebugMaterial(new Color(0.46f, 0.24f, 0.18f, 1f));
            var body = CreatePart(
                root.transform,
                PrimitiveType.Capsule,
                "Burrower Body",
                new Vector3(0, -0.08f, 0),
                new Vector3(0.38f, 0.34f, 0.38f),
                new Vector3(90, 0, 0),
                material);
            CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Burrower Nose",
                new Vector3(0, -0.06f, -0.28f),
                new Vector3(0.26f, 0.2f, 0.26f),
                new Vector3(90, 0, 0),
                new Color(0.92f, 0.6f, 0.22f, 1f));
            CreatePart(
                root.transform,
                PrimitiveType.Cylinder,
                "Burrower Dust",
                new Vector3(0, -0.25f, 0),
                new Vector3(0.72f, 0.04f, 0.72f),
                Vector3.zero,
                new Color(0.24f, 0.18f, 0.14f, 1f));
            return new DebugActorVisual(root, body.GetComponent<Renderer>(), material);
        }

        private static DebugActorVisual CreateBossAlien(GameObject root, AlienDefinition definition)
        {
            var material = CreateDebugMaterial(new Color(0.55f, 0.18f, 0.72f, 1f));
            var body = CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Boss Core",
                Vector3.zero,
                new Vector3(0.82f, 0.58f, 0.82f),
                Vector3.zero,
                material);
            CreatePart(
                root.transform,
                PrimitiveType.Cube,
                "Boss Crown",
                new Vector3(0, 0.42f, 0),
                new Vector3(0.76f, 0.18f, 0.76f),
                new Vector3(0, 35, 0),
                new Color(0.12f, 0.08f, 0.14f, 1f));
            CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Boss Eye",
                new Vector3(0.26f, 0.08f, -0.37f),
                new Vector3(0.18f, 0.18f, 0.18f),
                Vector3.zero,
                new Color(1f, 0.9f, 0.28f, 1f));
            CreatePart(
                root.transform,
                PrimitiveType.Sphere,
                "Boss Eye",
                new Vector3(-0.26f, 0.08f, -0.37f),
                new Vector3(0.18f, 0.18f, 0.18f),
                Vector3.zero,
                new Color(1f, 0.9f, 0.28f, 1f));
            return new DebugActorVisual(root, body.GetComponent<Renderer>(), material);
        }

        private static GameObject CreatePart(
            Transform parent,
            PrimitiveType primitiveType,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Vector3 localEulerAngles,
            Color colour)
        {
            return CreatePart(
                parent,
                primitiveType,
                name,
                localPosition,
                localScale,
                localEulerAngles,
                CreateDebugMaterial(colour));
        }

        private static GameObject CreatePart(
            Transform parent,
            PrimitiveType primitiveType,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Vector3 localEulerAngles,
            Material material)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localRotation = Quaternion.Euler(localEulerAngles);
            part.transform.localScale = localScale;

            var renderer = part.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material = material;
            }

            return part;
        }

        private static Color SelectFallbackColour(Material fallbackMaterial)
        {
            if (fallbackMaterial != null && fallbackMaterial.HasProperty("_BaseColor"))
            {
                return fallbackMaterial.GetColor("_BaseColor");
            }

            return new Color(0.52f, 0.27f, 0.85f, 1f);
        }
    }
}
