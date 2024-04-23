using UnityEngine;

namespace dev.klebersilva.tools.bitmapfontcreator
{
	internal static class TextureNavigationHandler
	{
		public static void HandleTextureNavigation(Rect rect, Vector2 zoomRange, ref float zoom, ref Vector2 scrollPos)
		{
			var e = Event.current;
			if (!rect.Contains(e.mousePosition)) return;
			if (e.control && e.isScrollWheel) HandleZoom(e, zoomRange, ref zoom);
			else if (e.isMouse && e.button == 2) HandlePan(e, ref scrollPos);
		}

		private static void HandleZoom(Event e, Vector2 zoomRange, ref float zoom)
		{
			zoom = Mathf.Clamp(zoom - e.delta.y, zoomRange.x, zoomRange.y);
			GUI.changed = true;
			e.Use();
		}

		private static void HandlePan(Event e, ref Vector2 scrollPos)
		{
			if (!(e.button == 2 && e.type == EventType.MouseDrag)) return;
			scrollPos.x -= e.delta.x;
			scrollPos.y -= e.delta.y;
			e.Use();
		}
	}
}