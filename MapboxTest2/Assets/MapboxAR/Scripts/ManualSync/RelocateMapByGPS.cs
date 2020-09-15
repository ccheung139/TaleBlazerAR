namespace Mapbox.Examples {
	using Mapbox.Unity.Location;
	using Mapbox.Unity.Map;
	using UnityEngine.UI;
	using UnityEngine;

	public class RelocateMapByGPS : MonoBehaviour {

		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		Button _button;

		[SerializeField]
		Transform _mapTransform;

		private float generateAfterSeconds = 5.0f;
		private float placementTimer = 0;

		private void Start () {
			_button.onClick.AddListener (UpdateMapLocation);
		}

		void Update () {
			if (placementTimer >= generateAfterSeconds) {
				placementTimer = 0;
				UpdateMapLocation ();
			} else {
				placementTimer += Time.deltaTime * 1.0f;
			}
		}

		public void UpdateMapLocation () {
			var location = LocationProviderFactory.Instance.DefaultLocationProvider.CurrentLocation;
			_map.UpdateMap (location.LatitudeLongitude, _map.AbsoluteZoom);
			var playerPos = Camera.main.transform.position;
			_mapTransform.position = new Vector3 (playerPos.x, _mapTransform.position.y, playerPos.z);
		}
	}
}