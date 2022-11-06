using UnityEngine;
using Verse;

namespace CleaningPriority.UserInterface;

[StaticConstructorOnStartup]
internal class TextureLoader
{
    public static readonly Texture2D priorityWindowButton = ContentFinder<Texture2D>.Get("cleanPrioritiesIcon");
    public static Texture2D plusSign = ContentFinder<Texture2D>.Get("grayscalePlus");
    public static Texture2D addIcon = SolidColorMaterials.NewSolidColorTexture(Color.grey);

    public static readonly Texture2D dragHash = ContentFinder<Texture2D>.Get("UI/Buttons/DragHash");
    public static readonly Texture2D delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
    public static readonly Texture2D clean = ContentFinder<Texture2D>.Get("Things/Mote/Clean");
}