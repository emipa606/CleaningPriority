using UnityEngine;
using Verse;

namespace CleaningPriority.UserInterface
{
	[StaticConstructorOnStartup]
	class TextureLoader
	{
		public static Texture2D priorityWindowButton = ContentFinder<Texture2D>.Get("cleanPrioritiesIcon", true);
		public static Texture2D plusSign = ContentFinder<Texture2D>.Get("grayscalePlus", true);
		public static Texture2D addIcon = SolidColorMaterials.NewSolidColorTexture(Color.grey);

		public static Texture2D dragHash = ContentFinder<Texture2D>.Get("UI/Buttons/DragHash", true);
		public static Texture2D delete = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
		public static Texture2D clean = ContentFinder<Texture2D>.Get("Things/Mote/Clean", true);
	}
}