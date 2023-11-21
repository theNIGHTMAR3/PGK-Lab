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
	[Header("Audio clips")]
	public AudioClip coinSound;
	public AudioClip gemSound;
	public AudioClip enemyKilledSound;
	public AudioClip diedByEnemySound;
	public AudioClip fellOutOfMapSound;
	public AudioClip foundHeartSound;
	public AudioClip finishedLevelSound;
	[Space(10)]

	private Rigidbody2D rigidBody;
	private Animator animator;
	private AudioSource source;

	private bool isWalking = false;
	private bool isFacingRight = true;
	private bool isOnLadder = false;
	private bool hasFinished = false;


	private bool leftClicked;
	private bool rightClicked;
	private bool downClicked;

	private int lives = 3;
	private Vector2 startPosition;
	
	public LayerMask groundLayer;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		startPosition = transform.position;
		source  = GetComponent<AudioSource>();
	}


	// Start is called before the first frame update
	void Start()
	{

	}

    // Update is called once per frame
    void Update()
    {

		if (GameManager.instance.currentGameState == GameState.GS_GAME )
		{
			isWalking = false;
			if (!hasFinished && !animator.GetBool("isDead"))
			{
				if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || rightClicked)
				{
					isWalking = true;
					if (!isFacingRight)
					{
						Flip();
					}
					transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
				}
				if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || leftClicked)
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
					if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || downClicked)
					{
						isWalking = true;

						transform.Translate(0.0f, -moveSpeed * Time.deltaTime, 0.0f, Space.World);
					}
				}

				if (Input.GetKeyDown(KeyCode.Space))
				{
					Jump();
				}
			}

			animator.SetBool("isGrounded", IsGrounded());
			animator.SetBool("isWalking", isWalking);
		}
	}

	private bool IsGrounded()
	{
		return Physics2D.Raycast(new Vector2(transform.position.x - 0.4f, transform.position.y), Vector2.down, rayLength, groundLayer.value) || 
			Physics2D.Raycast(new Vector2(transform.position.x + 0.4f, transform.position.y), Vector2.down, rayLength, groundLayer.value);

	}

	public void Jump()
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
			source.PlayOneShot(coinSound, AudioListener.volume + 10);
			GameManager.instance.AddCoins();	
			other.gameObject.SetActive(false);
		}
		if(other.CompareTag("Ladder"))
		{
			rigidBody.gravityScale = 0;
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
			isOnLadder = true;
		}
		if(other.CompareTag("Enemy") && !animator.GetBool("isDead"))
		{
			if(transform.position.y > other.gameObject.transform.position.y)
			{
				source.PlayOneShot(enemyKilledSound, AudioListener.volume + 10);
				GameManager.instance.EnemyKilled();
			}
            else
            {
				source.PlayOneShot(diedByEnemySound, AudioListener.volume + 10);
				Debug.Log("Killed by enemy");
				Death();
			}
        }
		if(other.CompareTag("FallLevel"))
		{
			Debug.Log("Fell out of map");
			source.PlayOneShot(fellOutOfMapSound, AudioListener.volume + 10);
			rigidBody.gravityScale = 0;
			rigidBody.velocity = new Vector2(0f,0f);
			Death();
		}
		if (other.CompareTag("Key"))
		{
			source.PlayOneShot(gemSound, AudioListener.volume + 10);
			GameManager.instance.AddKeys();
			other.gameObject.SetActive(false);
		}
		if (other.CompareTag("Heart"))
		{	
			lives++;
			Debug.Log("Found an extra live, current lives: " + lives);
			source.PlayOneShot(foundHeartSound, AudioListener.volume + 10);
			GameManager.instance.FoundLife(lives);
			other.gameObject.SetActive(false);
		}
		if (other.CompareTag("Finish"))
		{
			if(GameManager.instance.HasFoundAllKeys)
			{
				Debug.Log("Level completed");
				source.PlayOneShot(finishedLevelSound, AudioListener.volume + 10);
				GameManager.instance.LevelCompleted();
				hasFinished = true;
				isWalking = false;
			
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
		if(other.CompareTag("FallLevel"))
		{
			rigidBody.gravityScale = 1;
		}
	}

	private void Death()
	{
		lives--;
		animator.SetBool("isDead", true);

		if (lives == 0)
		{
			Debug.Log("No more lives, GAME OVER");
			GameManager.instance.GameOver();
		}
		else
		{
			GameManager.instance.PlayerKilled(lives);
			transform.position = startPosition;
			rigidBody.velocity = new Vector2(0f, 0f);
			animator.SetBool("isDead", false);
			Debug.Log("Respawned, lives left: "+ lives);
		}
	}

}
