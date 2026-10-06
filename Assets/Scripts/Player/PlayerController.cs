using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Main player controller for third-person movement and input handling
/// Separates input from character data for clean architecture
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private string playerId = "player_1";
    [SerializeField] private string playerName = "Player";
    [SerializeField] private int playerLevel = GameConstants.INITIAL_PLAYER_LEVEL;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTarget;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraDistance = 3f;
    [SerializeField] private float cameraHeight = 1.5f;
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("UI")]
    [SerializeField] private Transform nameTagPosition;
    [SerializeField] private Canvas worldSpaceCanvas;

    private CharacterData characterData;
    private Vector3 moveDirection = Vector3.zero;
    private float gravity = -9.81f;
    private bool isSprinting = false;
    private bool isDashing = false;
    private float dashCooldown = 0f;
    private Vector3 lastFacingDirection = Vector3.forward;

    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    private int starsCollected = 0;

    private void Start()
    {
        characterData = GetComponent<CharacterData>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterPlayer(this);

        if (NetworkManager.Instance != null)
            NetworkManager.Instance.ConnectPlayer(playerId);
    }

    private void Update()
    {
        if (!characterData.IsAlive())
            return;

        HandleInput();
        HandleMovement();
        HandleCamera();
        UpdateCharacterData();
    }

    private void HandleInput()
    {
        // Movement input
        Vector3 moveInput = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveInput += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveInput -= transform.forward;
        if (Input.GetKey(KeyCode.D)) moveInput += transform.right;
        if (Input.GetKey(KeyCode.A)) moveInput -= transform.right;

        moveInput = moveInput.normalized;

        // Sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift) && moveInput.magnitude > 0;

        // Dash
        if (Input.GetKeyDown(KeyCode.Space) && dashCooldown <= 0 && moveInput.magnitude > 0)
        {
            isDashing = true;
            dashCooldown = 1f;
        }

        // Combat inputs
        if (Input.GetMouseButtonDown(0)) // Basic attack
        {
            characterData.ExecuteBasicAttack(transform.position + transform.forward * 5f);
        }
        if (Input.GetKeyDown(KeyCode.Q)) // Special ability
        {
            characterData.ExecuteAbility(transform.position + transform.forward * 5f);
        }
        if (Input.GetKeyDown(KeyCode.E)) // Ultimate ability
        {
            characterData.ExecuteUltimate(transform.position + transform.forward * 5f);
        }

        moveDirection = moveInput;
    }

    private void HandleMovement()
    {
        if (!characterController.enabled) return;

        float speed = characterData.GetMovementSpeed();
        if (isSprinting) speed = characterData.GetSprintSpeed();
        if (isDashing) speed = characterData.GetDashSpeed();

        Vector3 movement = moveDirection * speed;

        // Apply gravity
        if (!characterController.isGrounded)
        {
            movement.y += gravity * Time.deltaTime;
        }
        else
        {
            movement.y = -0.1f; // Small downward force to keep grounded
        }

        characterController.Move(movement * Time.deltaTime);

        // Rotate character to face movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            lastFacingDirection = moveDirection;
        }

        // End dash
        if (isDashing)
        {
            isDashing = false;
        }

        dashCooldown -= Time.deltaTime;
    }

    private void HandleCamera()
    {
        if (mainCamera == null) return;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraYaw += mouseX;
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -30f, 60f);

        // Position camera
        Vector3 cameraPos = transform.position + 
                           Quaternion.Euler(0, cameraYaw, 0) * (Vector3.back * cameraDistance) +
                           Vector3.up * cameraHeight +
                           Quaternion.Euler(cameraPitch, cameraYaw, 0) * Vector3.zero;

        mainCamera.transform.position = cameraPos;
        mainCamera.transform.LookAt(transform.position + Vector3.up * 0.6f);
    }

    private void UpdateCharacterData()
    {
        characterData.UpdateEnergy(Time.deltaTime);
    }

    // Player Info
    public string GetPlayerId() => playerId;
    public string GetPlayerName() => playerName;
    public int GetPlayerLevel() => playerLevel;
    public int GetStarsCollected() => starsCollected;

    // Star collection
    public void CollectStar()
    {
        starsCollected++;
        if (GameManager.Instance != null)
            GameManager.Instance.AddStarToPlayer(playerId, 1);
        EventSystem.Instance.TriggerPlayerStarCollected(starsCollected);
        AudioManager.Instance?.PlaySFX("StarCollect");
    }

    // Character Data Access
    public CharacterData GetCharacterData() => characterData;
    public bool IsAlive() => characterData?.IsAlive() ?? false;

    // Respawn
    public void Respawn(Vector3 spawnPosition)
    {
        characterData.Respawn(spawnPosition);
        starsCollected = 0;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterPlayer(this);
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.DisconnectPlayer(playerId);
    }

    public void SetPlayerId(string id) => playerId = id;
    public void SetPlayerName(string name) => playerName = name;
}
