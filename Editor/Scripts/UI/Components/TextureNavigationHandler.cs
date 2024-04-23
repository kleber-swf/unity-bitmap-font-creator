using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class TextureNavigationHandler
	{
		public static void HandleTextureNavigation(Rect rect, Vector2 zoomRange, ref float zoom)
		{
			var e = Event.current;
			if (e.control && e.isScrollWheel) HandleZoom(e, rect, zoomRange, ref zoom);
		}

		private static void HandleZoom(Event e, Rect rect, Vector2 zoomRange, ref float zoom)
		{
			if (!rect.Contains(e.mousePosition)) return;

			zoom = Mathf.Clamp(zoom - e.delta.y, zoomRange.x, zoomRange.y);
			e.Use();
			GUI.changed = true;
		}
	}
}