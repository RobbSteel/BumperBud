using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSplitForce))]
	public class D2dSplitForce_Editor : D2dEditor<D2dSplitForce>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Force == 0.0f));
				DrawDefault("Force");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	/// <summary>This component will automatically add outward force to each chunk of a destruction after it gets split or fractured.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dSplitForce")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Split Force")]
	public class D2dSplitForce : MonoBehaviour
	{
		/// <summary>The force applied to split chunks when their distance to the center is l</summary>
		public float Force = 1000.0f;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		[System.NonSerialized]
		private Vector2 center;

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();

			cachedDestructible.OnSplitStart += HandleSplitStart;
			cachedDestructible.OnSplitEnd   += HandleSplitEnd;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnSplitStart -= HandleSplitStart;
			cachedDestructible.OnSplitEnd   -= HandleSplitEnd;
		}

		private void HandleSplitStart()
		{
			var body = GetComponent<Rigidbody2D>();

			if (body != null)
			{
				center = transform.TransformPoint(body.centerOfMass);
			}
			else
			{
				center = transform.position;
			}
		}

		private void HandleSplitEnd(List<D2dDestructible> splitDestructibles)
		{
			for (var i = splitDestructibles.Count - 1; i >= 0; i--)
			{
				var splitDestructible = splitDestructibles[i];
				var body              = splitDestructible.GetComponent<Rigidbody2D>();

				if (body != null)
				{
					var point  = (Vector2)body.transform.TransformPoint(body.centerOfMass);
					var vector = point - center;

					if (vector.sqrMagnitude > 0.0f)
					{
						body.AddForceAtPosition(vector.normalized * (Force / vector.magnitude), center, ForceMode2D.Impulse);
					}
				}
			}
		}
	}
}
