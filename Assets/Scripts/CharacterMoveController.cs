using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{    
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    // added in "Jumping"
    [Header("Jump")]
    public float jumpAccel;

    private bool isJumping;
    private bool isOnGround;

    // Added in "Using Raycast & Layers"
    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;
    
    private Rigidbody2D rig;

    // Added in "Add jump animation"
    private Animator anim;

    // Added in "Add jumping sound"
    private CharacterSoundController sound;

    
    // Added in "Calculating score"
    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;

    // Added in "Game Over behaviour"
    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Added in "Add jumping sound"
        sound = GetComponent<CharacterSoundController>();

        // Added in "Calculating score"
        lastPositionX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Added in "Jumping"
        // read input
        if (Input.GetMouseButtonDown(0))
        {
            if (isOnGround)
            {
                isJumping = true;
                // Added in "Add jumping sound"
                sound.PlayJump();
            }
        }

        // change animation
        anim.SetBool("isOnGround", isOnGround);

        // calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        // game over
        if (transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }

    void FixedUpdate()
    {
        // raycast ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }

        // calculate velocity vector
        Vector2 velocityVector = rig.velocity;

        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);
        rig.velocity = velocityVector;
    }

    // raycast debug function
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }

    // Added in "game over behaviour"
    private void GameOver()
    {
        // set high score
        score.FinishScoring();

        // stop camera movement
        gameCamera.enabled = false;

        // show gameover
        gameOverScreen.SetActive(true);

        // disable this too
        this.enabled = false;
    }

    // Added in "Add jumping sound"
    // public void PlayJump()
    // {
    //     audioPlayer.PlayOneShot(jump);
    // }
}
