using UnityEngine;

namespace Project147.UnityPresentation.Debug
{
    public sealed class DebugActorVisual
    {
        public DebugActorVisual(GameObject gameObject, Renderer primaryRenderer, Material defaultMaterial)
        {
            GameObject = gameObject;
            PrimaryRenderer = primaryRenderer;
            DefaultMaterial = defaultMaterial;
        }

        public GameObject GameObject { get; }

        public Renderer PrimaryRenderer { get; }

        public Material DefaultMaterial { get; }
    }
}
