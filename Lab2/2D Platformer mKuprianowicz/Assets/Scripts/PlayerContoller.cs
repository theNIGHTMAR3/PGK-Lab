using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerContoller : MonoBehaviour
{
	private const float rayLength = 1.3f;


	[Header("Movement parameters")]
	[Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.2f; // moving speed of the player
	[Range(0.01f, 20.0f)][SerializeField] private float jumpForce = 3.5f; // moving speed of the player
	[Space(10)]

	private Rigidbody2D rigidBody;
	private Animator animator;
	private bool isWalking = false;
	private bool isFacingRight = true;
	private bool isOnLadder = false;
	private bool hasFinished = false;

	private int score = 0;
	private int lives = 3;
	private int keysFound = 0;
	private Vector2 startPosition;
	
	public LayerMask groundLayer;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		startPosition = transform.position;
	}


	// Start is called before the first frame update
	void Start()
	{

	}

    // Update is called once per frame
    void Update()
    {

		isWalking = false;
		if(!hasFinished && !animator.GetBool("isDead"))
		{
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			{
				isWalking = true;
				if (!isFacingRight)
				{
					Flip();
				}
				transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
			}
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				isWalking = true;
				if (isFacingRight)
				{
					Flip();
				}
				transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
			}
			if (isOnLadder)
			{
				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
				{
					isWalking = true;

					transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f, Space.World);
				}
				if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
				{
					isWalking = true;

					transform.Translate(0.0f, -moveSpeed * Time.deltaTime, 0.0f, Space.World);
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				Jump();
			}
		}

		animator.SetBool("isGrounded", IsGrounded());
		animator.SetBool("isWalking", isWalking);
	}

	private bool IsGrounded()
	{
		return Physics2D.Raycast(new Vector2(transform.position.x - 0.4f, transform.position.y), Vector2.down, rayLength, groundLayer.value) || 
			Physics2D.Raycast(new Vector2(transform.position.x + 0.4f, transform.position.y), Vector2.down, rayLength, groundLayer.value);

	}

	private void Jump()
	{
		if(IsGrounded() || isOnLadder)
		{
			rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
	}

	private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Bonus"))
		{
			score += 10;
			Debug.Log("Score: " + score);
			other.gameObject.SetActive(false);
		}
		if(other.CompareTag("Ladder"))
		{
			rigidBody.gravityScale = 0;
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
			isOnLadder = true;
		}
		if(other.CompareTag("Enemy"))
		{
			if(transform.position.y > other.gameObject.transform.position.y)
			{
				score += 10;
				Debug.Log("Killed enemy");
			}
            else
            {
				Debug.Log("Killed by enemy");
				Death();
			}
        }
		if(other.CompareTag("FallLevel"))
		{
			Debug.Log("Fell aout of map");
			Death();
		}
		if (other.CompareTag("Key"))
		{
			Debug.Log("Found a key");
			keysFound++;
			other.gameObject.SetActive(false);
		}
		if (other.CompareTag("Heart"))
		{
			Debug.Log("Found an extra live, current lives: "+lives);
			lives++;
			other.gameObject.SetActive(false);
		}
		if (other.CompareTag("Finish"))
		{
			if(keysFound == 3)
			{
				Debug.Log("Level comleted");
				hasFinished = true;
			}
			else
			{
				Debug.Log("Find all gems to finish the level");
			}

		}
		if (other.CompareTag("MovingPlatform"))
		{
			transform.SetParent(other.transform);

		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Ladder"))
		{
			rigidBody.gravityScale = 1;
			isOnLadder = false;
		}
		if (other.CompareTag("MovingPlatform"))
		{
			transform.SetParent(null);
		}
	}

	private void Death()
	{
		animator.SetBool("isDead", true);
		if (lives == 0)
		{
			Debug.Log("No more lives, GAME OVER");
		}
		else
		{
			lives--;
			transform.position = startPosition;
			rigidBody.velocity = new Vector2(0f, 0f);
			animator.SetBool("isDead", false);
			Debug.Log("Respawned, lives left: "+ lives);
		}
	}
}
