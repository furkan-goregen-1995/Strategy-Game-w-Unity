using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameaTile : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
	public Transform arrow;
	

    GameaTile north, east, south, west, nextOnPath;


    int distance;
	GameTileContent content;

	public GameaTile NextTileOnPath => nextOnPath;

    
	static Quaternion  //Nesneleri döndürme (transform.rotation())
		northRotation = Quaternion.Euler(90f, 0f, 0f),
		eastRotation = Quaternion.Euler(90f, 90f, 0f),
		southRotation = Quaternion.Euler(90f, 180f, 0f),
		westRotation = Quaternion.Euler(90f, 270f, 0f);


	public bool IsAlternative { get; set; }

	public Direction PathDirection { get; private set; }


  public static void MakeEastWestNeighbors (GameaTile east, GameaTile west) {
		Debug.Assert(
			west.east == null && east.west == null, "Redefined neighbors!"
		);
		west.east = east;
		east.west = west;
	}

    public static void MakeNorthSouthNeighbors (GameaTile north, GameaTile south) {
		Debug.Assert(
			south.north == null && north.south == null, "Redefined neighbors!"
		);
		south.north = north;
		north.south = south;
	}

public void ClearPath () {
		distance = int.MaxValue;
		nextOnPath = null;
	}

    public void BecomeDestination () {
		distance = 0;
		nextOnPath = null;
		ExitPoint = transform.localPosition;
	}

    public bool HasPath => distance != int.MaxValue;

    public GameaTile GrowPathNorth () => GrowPathTo(north, Direction.South);

	public GameaTile GrowPathEast () => GrowPathTo(east, Direction.West);

	public GameaTile GrowPathSouth () => GrowPathTo(south, Direction.North);

	public GameaTile GrowPathWest () => GrowPathTo(west, Direction.East);

	public Vector3 ExitPoint { get; private set; }

    GameaTile GrowPathTo (GameaTile neighbor, Direction direction) {
		if (!HasPath || neighbor == null || neighbor.HasPath) {
			return null;
		}
		neighbor.distance = distance + 1;
		neighbor.nextOnPath = this;
		neighbor.ExitPoint =
			neighbor.transform.localPosition + direction.GetHalfVector();
		neighbor.PathDirection = direction;
		return
			neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
	}

    public void ShowPath () {
		if (distance == 0) {
			arrow.gameObject.SetActive(false);
			return;
		}
		arrow.gameObject.SetActive(true);
		arrow.localRotation =
			nextOnPath == north ? northRotation :
			nextOnPath == east ? eastRotation :
			nextOnPath == south ? southRotation :
			westRotation;
	}



public GameTileContent Content {
		get => content;
		set {
			Debug.Assert(value != null, "Null assigned to content!");
			if (content != null) {
				content.Recycle();
			}
			content = value;
			content.transform.localPosition = transform.localPosition;
		}
	}

		public void HidePath () {
		arrow.gameObject.SetActive(false);
	}

}
