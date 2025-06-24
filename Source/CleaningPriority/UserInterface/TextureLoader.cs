using UnityEngine;
using Verse;

namespace CleaningPriority.UserInterface;

[StaticConstructorOnStartup]
internal class TextureLoader
{
    public static readonly Texture2D PriorityWindowButton = ContentFinder<Texture2D>.Get("cleanPrioritiesIcon");
    public static readonly Texture2D Delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
    public static readonly Texture2D Clean = ContentFinder<Texture2D>.Get("Things/Mote/Clean");
}