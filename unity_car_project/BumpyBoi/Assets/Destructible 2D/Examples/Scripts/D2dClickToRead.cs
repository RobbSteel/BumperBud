using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dClickToRead))]
	public class D2dClickToRead_Editor : D2dEditor<D2dClickToRead>
	{
		protected override void OnInspector()
		{
			DrawDefault("Requires");
			DrawDefault("Intercept");
		}
	}
}
#endif

namespace Destructible2D
{
	/// <summary>This component reads the current color of any D2dDestructible under the mouse when holding the specified button.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dClickToRead")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Read")]
	public class D2dClickToRead : MonoBehaviour
	{
		[Tooltip("The key you must hold down to spawn")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The z position the prefab should spawn at")]
		public float Intercept;

		protected virtual void Update()
		{
			// Required key is down?
			if (Input.GetKeyDown(Requires) == true)
			{
				// Main camera exists?
				var mainCamera = Camera.main;

				if (mainCamera != null)
				{
					// World position of the mouse
					var position = D2dHelper.ScreenToWorldPosition(Input.mousePosition, Intercept, mainCamera);

					// Read the destructible and alpha at this position
					var destructible = default(D2dDestructible);
					var alpha        = default(Color32);

					if (D2dDestructible.TrySampleAlphaAll(position, ref destructible, ref alpha) == true)
					{
						Debug.Log("Read " + destructible + " with alpha: " + alpha);
					}
					else
					{
						Debug.Log("Read nothing.");
					}
				}
			}
		}
	}
}