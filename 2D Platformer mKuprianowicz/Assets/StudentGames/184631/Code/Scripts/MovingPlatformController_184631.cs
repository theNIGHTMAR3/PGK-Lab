using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
	[Header("Movement parameters")]
	[Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 1.0f; // moving speed of the player
	[Range(0.01f, 20.0f)][SerializeField] private float moveRange = 2.0f; // moving speed of the player
	public bool isMovingRight = false;

	private float startPositionX;


	void Awake()
	{
		startPositionX = transform.position.x;
	}

	// Start is called before the first frame update
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
		if (isMovingRight)
		{
			if (transform.position.x <= startPositionX + moveRange)
			{
				MoveRight();
			}
			else
			{
				isMovingRight = false;
				MoveLeft();
			}
		}
		else
		{
			if (transform.position.x >= startPositionX - moveRange)
			{
				MoveLeft();
			}
			else
			{
				isMovingRight = true;
				MoveRight();

			}
		}
	}

	private void MoveRight()
	{

		transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
	}

	private void MoveLeft()
	{

		transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
	}
}
