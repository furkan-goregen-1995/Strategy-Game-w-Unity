using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
	Transform ground = default;

	List<GameaTile> spawnPoints = new List<GameaTile>();

	[SerializeField]
	Texture2D gridTexture = default;

	public GameaTile tilePrefab;

	public GameaTile[] tiles;

	bool showGrid,showPaths;

	Vector2Int size;

	GameTileContentFactory contentFactory;
	
    Queue<GameaTile> searchFrontier = new Queue<GameaTile>();

    public void Initialize (Vector2Int size, GameTileContentFactory contentFactory) {
		this.size = size;
		this.contentFactory = contentFactory;
		ground.localScale = new Vector3(size.x, size.y, 1f);
		Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
		tiles = new GameaTile[size.x * size.y];
		for (int i = 0, y = 0; y < size.y; y++) {
			for (int x = 0; x < size.x; x++, i++) {
				GameaTile tile = tiles[i] = Instantiate(tilePrefab);
				tile.transform.SetParent(transform, false);
				tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
				if (x > 0) {
					GameaTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
				}
				if (y > 0) {
					GameaTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
				}
				tile.IsAlternative = (x & 1) == 0;
				if ((y & 1) == 0) {
					tile.IsAlternative = !tile.IsAlternative;
				}
				tile.Content = contentFactory.Get(GameTileContentType.Empty);
			}
		}
		//FindPaths();
		ToggleDestination(tiles[tiles.Length / 2]);
		ToggleSpawnPoint(tiles[0]);
	}

	bool FindPaths () {

		foreach (GameaTile ktile in tiles) {
			if (ktile.Content.Type == GameTileContentType.Destination) {
				ktile.BecomeDestination();
				searchFrontier.Enqueue(ktile);
			}
			else {
				ktile.ClearPath();
			}
		}

		if (searchFrontier.Count == 0) {
			return false;
		}

		//tiles[tiles.Length / 2].BecomeDestination();
		//searchFrontier.Enqueue(tiles[tiles.Length / 2]);
		while (searchFrontier.Count > 0) {
		GameaTile tile = searchFrontier.Dequeue();
		if (tile != null) {
				if (tile.IsAlternative) {
					searchFrontier.Enqueue(tile.GrowPathNorth());
					searchFrontier.Enqueue(tile.GrowPathSouth());
					searchFrontier.Enqueue(tile.GrowPathEast());
					searchFrontier.Enqueue(tile.GrowPathWest());
				}
				else {
					searchFrontier.Enqueue(tile.GrowPathWest());
					searchFrontier.Enqueue(tile.GrowPathEast());
					searchFrontier.Enqueue(tile.GrowPathSouth());
					searchFrontier.Enqueue(tile.GrowPathNorth());
				}
			}
		}

		foreach (GameaTile tile in tiles) {
			if (!tile.HasPath) {
				return false;
			}
		}

		foreach (GameaTile atile in tiles) {
			atile.ShowPath();
		}

		if (showPaths) {
			foreach (GameaTile tile in tiles) {
				tile.ShowPath();
			}
		}

		return true;
	}

	public bool ShowGrid {
		get => showGrid;
		set {
			showGrid = value;
			Material m = ground.GetComponent<MeshRenderer>().material;
			if (showGrid) {
				Debug.Log("oldu");
				m.mainTexture = gridTexture;
				m.SetTextureScale("_MainTex", size);
			}
			else {
				m.mainTexture = null;
			}
		}
	}


	public bool ShowPaths {
		get => showPaths;
		set {
			showPaths = value;
			if (showPaths) {
				foreach (GameaTile tile in tiles) {
					tile.ShowPath();
				}
			}
			else {
				foreach (GameaTile tile in tiles) {
					tile.HidePath();
				}
			}
		}
	}

public GameaTile GetTile (Ray ray) {
		Debug.Log(ray);
		RaycastHit Hit;
		if (Physics.Raycast(ray, out Hit, 10.0f)) {
				Debug.Log(ray);
			int x = (int)(Hit.point.x + size.x * 0.5f);
			int y = (int)(Hit.point.z + size.y * 0.5f);
			if (x >= 0 && x < size.x && y >= 0 && y < size.y) {
				return tiles[x + y * size.x];
			}
		}

		return null;
	}

public void ToggleDestination (GameaTile tile) {
		if (tile.Content.Type == GameTileContentType.Destination) {
			tile.Content = contentFactory.Get(GameTileContentType.Empty);
			if (!FindPaths()) {
				tile.Content =
					contentFactory.Get(GameTileContentType.Destination);
				FindPaths();
			}
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(GameTileContentType.Destination);
			FindPaths();
		}
	}

	public void ToggleWall (GameaTile tile) {
		if (tile.Content.Type == GameTileContentType.Wall) {
			tile.Content = contentFactory.Get(GameTileContentType.Empty);
			FindPaths();
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(GameTileContentType.Wall);
			if (!FindPaths()) {
				tile.Content = contentFactory.Get(GameTileContentType.Empty);
				FindPaths();
			}
		}
	}

	public void ToggleSpawnPoint (GameaTile tile) {
		if (tile.Content.Type == GameTileContentType.SpawnPoint) {
		if (spawnPoints.Count > 1) {
				spawnPoints.Remove(tile);
				tile.Content = contentFactory.Get(GameTileContentType.Empty);
			}
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
			spawnPoints.Add(tile);
		}
	}

		public GameaTile GetSpawnPoint (int index) {
		return spawnPoints[index];
	}

	public int SpawnPointCount => spawnPoints.Count;

}
