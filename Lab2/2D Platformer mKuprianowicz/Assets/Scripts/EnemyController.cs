using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[Header("Movement parameters")]
	[Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 1.0f; // moving speed of the player
	[Range(0.01f, 20.0f)][SerializeField] private float moveRange = 2.0f; // moving speed of the player

	private Animator animator;

	private float startPositionX;

	public bool isFacingRight = false;
	public bool isMovingRight = false;

	private void Awake()
	{
		startPositionX = transform.position.x;
		animator = GetComponent<Animator>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(animator.GetBool("isDead") == false)
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
    }

	private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void MoveRight()
	{
		if (!isFacingRight)
		{
			Flip();
		}
		transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
	}

	private void MoveLeft()
	{
		if (isFacingRight)
		{
			Flip();
		}
		transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			if(other.gameObject.transform.position.y > transform.position.y)
			{
				animator.SetBool("isDead", true);
				StartCoroutine(KillOnAnimationEnd());
			}
		}
	}

	private IEnumerator KillOnAnimationEnd()
	{
		yield return new WaitForSeconds(0.65f);
		
		gameObject.SetActive(false);	
	}

}
