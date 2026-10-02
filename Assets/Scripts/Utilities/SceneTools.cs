using UnityEngine;

public static class SceneTools
{
    public static Grid FindGrid()
    {
        return Object.FindObjectOfType<Grid>();
    }

    public static Transform FindPlayerTransform()
    {
        Player player = Object.FindObjectOfType<Player>();
        return player != null ? player.transform : null;
    }

    public static bool TryFindComponentWithTag<T>(string tag, out T component) where T : Component
    {
        component = null;
        GameObject target = GameObject.FindWithTag(tag);
        return target != null && target.TryGetComponent(out component);
    }

    public static Transform GetOrCreateTaggedParent(string tag)
    {
        GameObject target = GameObject.FindWithTag(tag);
        if (target != null)
            return target.transform;

        GameObject parent = new GameObject(tag);
        parent.tag = tag;
        return parent.transform;
    }
}
