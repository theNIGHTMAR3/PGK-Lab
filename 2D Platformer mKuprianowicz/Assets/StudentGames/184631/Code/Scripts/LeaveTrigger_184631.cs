using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
		{
			if (!LevelGenerator.Instance.shouldFinish)
			{
				LevelGenerator.Instance.AddPiece();
				LevelGenerator.Instance.RemoveOldestPiece();
			}		
		}	
	}
}
