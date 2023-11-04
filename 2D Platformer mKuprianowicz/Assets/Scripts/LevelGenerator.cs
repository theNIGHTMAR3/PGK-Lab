using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

	private static LevelGenerator instance;

	private LevelGenerator()
	{}

	public static LevelGenerator Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<LevelGenerator>();
				if (instance == null)
				{
					GameObject singletonObject = new GameObject("LevelGenerator");
					instance = singletonObject.AddComponent<LevelGenerator>();
				}
			}
			return instance;
		}
	}

	public Transform levelStartPoint;

	public List<LevelPieceBasic> levelPrefabs = new List<LevelPieceBasic>();
	public List<LevelPieceBasic> pieces = new List<LevelPieceBasic>();



	// Start is called before the first frame update
	void Start()
    {

		AddPiece();
		AddPiece();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddPiece()
	{
		int randomIndex = Random.Range(0, levelPrefabs.Count-1);
		LevelPieceBasic piece = (LevelPieceBasic)Instantiate(levelPrefabs[randomIndex]);
		piece.transform.SetParent(this.transform, false);

		if(pieces.Count==0)
		{
			piece.transform.position = new Vector2(
				levelStartPoint.position.x - piece.startPoint.localPosition.x,
				levelStartPoint.position.y - piece.startPoint.localPosition.y);
		}
		else
		{
			piece.transform.position = new Vector2(
				pieces[pieces.Count - 1].exitPoint.position.x - piece.startPoint.localPosition.x,
				pieces[pieces.Count - 1].exitPoint.position.y - piece.startPoint.localPosition.y);
		}

		pieces.Add(piece);

	}

	public void RemoveOldestPiece()
	{
		LevelPieceBasic oldestPiece = pieces[0];
		pieces.Remove(oldestPiece);
		Destroy(oldestPiece.gameObject);
	}
}
