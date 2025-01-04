// CHANGE LOG
// 
// CHANGES || version VERSION
//
// "Enable/Disable Headbob, Changed look rotations - should result in reduced camera jitters" || version 1.0.1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using System.Net;
#endif

public class FirstPersonController : MonoBehaviour
{
  private Rigidbody rb;

  #region Camera Movement Variables

  public Camera playerCamera;

  public float fov = 60f;
  public bool invertCamera = false;
  public bool cameraCanMove = true;
  public float mouseSensitivity = 2f;
  public float maxLookAngle = 50f;

  // Crosshair
  public bool lockCursor = true;
  public bool crosshair = true;
  public Sprite crosshairImage;
  public Color crosshairColor = Color.white;

  // Internal Variables
  private float yaw = 0.0f;
  private float pitch = 0.0f;

  public float initialPitch = -90f; // Add this line to define the initial pitch angle

  private Image crosshairObject;

  #region Camera Zoom Variables

  public bool enableZoom = true;
  public bool holdToZoom = false;
  public KeyCode zoomKey = KeyCode.Mouse1;
  public float zoomFOV = 30f;
  public float zoomStepTime = 5f;

  // Internal Variables
  private bool isZoomed = false;

  #endregion
  #endregion

  #region Movement Variables

  public bool playerCanMove = true;
  public float walkSpeed = 5f;
  public float maxVelocityChange = 10f;
  private Vector3 initialPosition;

  // Internal Variables
  private bool isWalking = false;

  #region Sprint

  public bool enableSprint = true;
  public bool unlimitedSprint = false;
  public KeyCode sprintKey = KeyCode.C;
  public float sprintSpeed = 7f;
  public float sprintDuration = 5f;
  public float sprintCooldown = .5f;
  public float sprintFOV = 80f;
  public float sprintFOVStepTime = 10f;

  // Sprint Bar
  public bool useSprintBar = true;
  public bool hideSprintBarWhenFull = true;
  public Image sprintBarBG;
  public Image sprintBar;
  public float sprintBarWidthPercent = .3f;
  public float sprintBarHeightPercent = .015f;

  // Internal Variables for Sprint Bar 
  private CanvasGroup sprintBarCG;
  private bool isSprinting = false;
  private float sprintRemaining;
  private float sprintBarWidth;
  private float sprintBarHeight;
  private bool isSprintCooldown = false;
  private float sprintCooldownReset;

  #endregion

  #region Dash

  public bool enableDash = true;
  public bool unlimitedDash = false;
  public KeyCode dashKey = KeyCode.LeftShift;
  public float dashDistance = 10f;
  public float dashDuration = 0.2f;
  private float dashStartTime;
  private Vector3 dashStartPosition;
  private Vector3 dashEndPosition;
  public float dashCooldown = 0f;
  public float dashFOV = 80f;
  public float dashFOVStepTime = 10f;

  // Dash Bar

  public bool useDashBar = true;
  public bool hideDashBarWhenFull = true;
  public Image dashBarBG;
  public Image dashBar;
  public float dashBarWidthPercent = .3f;
  public float dashBarHeightPercent = .015f;

  // Internal Variables for Dash Bar 
  private CanvasGroup dashBarCG;

  private bool isDashing = false;

  private float dashRemaining;

  private float dashBarWidth;

  private float dashBarHeight;

  private bool isDashCooldown = false;

  private float dashCooldownReset;

  #endregion

  #region Jump

  public bool enableJump = true;
  public KeyCode jumpKey = KeyCode.Space;
  public float jumpPower = 5f;

  // Internal Variables
  private bool isGrounded = false;

  #endregion

  // Internal Variables
  private Vector3 originalScale;

  #endregion

  #region Head Bob

  public bool enableHeadBob = true;
  public Transform joint;
  public float bobSpeed = 10f;
  public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);

  // Internal Variables
  private Vector3 jointOriginalPos;
  private float timer = 0;

  #endregion

  private void Awake()
  {
    rb = GetComponent<Rigidbody>();

    crosshairObject = GetComponentInChildren<Image>();

    // Set internal variables
    playerCamera.fieldOfView = fov;
    originalScale = transform.localScale;
    jointOriginalPos = joint.localPosition;

    if (!unlimitedSprint)
    {
      sprintRemaining = sprintDuration;
      sprintCooldownReset = sprintCooldown;
    }

    if (!unlimitedDash)
    {
      dashRemaining = dashDuration;
      dashCooldownReset = dashCooldown;
    }

    // Initial position for resetting on death
    initialPosition = transform.position;
  }

  public void ResetPosition()
  {
    transform.position = initialPosition;
    rb.linearVelocity = Vector3.zero;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Obstacle"))
    {
      ResetPosition();
    }
  }

  void Start()
  {
    if (lockCursor)
    {
      Cursor.lockState = CursorLockMode.Locked;
    }

    if (crosshair)
    {
      crosshairObject.sprite = crosshairImage;
      crosshairObject.color = crosshairColor;
    }
    else
    {
      crosshairObject.gameObject.SetActive(false);
    }

    // Set the initial pitch angle
    pitch = initialPitch;
    playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);

    #region Sprint Bar

    sprintBarCG = GetComponentInChildren<CanvasGroup>();

    if (useSprintBar)
    {
      sprintBarBG.gameObject.SetActive(true);
      sprintBar.gameObject.SetActive(true);

      float screenWidth = Screen.width;
      float screenHeight = Screen.height;

      sprintBarWidth = screenWidth * sprintBarWidthPercent;
      sprintBarHeight = screenHeight * sprintBarHeightPercent;

      sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
      sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

      if (hideSprintBarWhenFull)
      {
        sprintBarCG.alpha = 0;
      }
    }
    else
    {
      sprintBarBG.gameObject.SetActive(false);
      sprintBar.gameObject.SetActive(false);
    }

    #endregion

    #region Dash Bar

    dashBarCG = GetComponentInChildren<CanvasGroup>();

    if (useDashBar)
    {
      dashBarBG.gameObject.SetActive(true);
      dashBar.gameObject.SetActive(true);

      float screenWidth = Screen.width;
      float screenHeight = Screen.height;

      dashBarWidth = screenWidth * dashBarWidthPercent;
      dashBarHeight = screenHeight * dashBarHeightPercent;

      dashBarBG.rectTransform.sizeDelta = new Vector3(dashBarWidth, dashBarHeight, 0f);
      dashBar.rectTransform.sizeDelta = new Vector3(dashBarWidth - 2, dashBarHeight - 2, 0f);

      if (hideDashBarWhenFull)
      {
        dashBarCG.alpha = 0;
      }
    }
    else
    {
      dashBarBG.gameObject.SetActive(false);
      dashBar.gameObject.SetActive(false);
    }

    #endregion

  }

  float camRotation;

  private void Update()
  {
    // Test for reset position
    if (Input.GetKeyDown(KeyCode.R))
    {
      ResetPosition();
    }

    #region Camera

    // Control camera movement
    if (cameraCanMove)
    {
      yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * mouseSensitivity;

      if (!invertCamera)
      {
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
      }
      else
      {
        // Inverted Y
        pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
      }

      // Clamp pitch between lookAngle
      pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

      transform.localEulerAngles = new Vector3(0, yaw, 0);
      playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    #region Camera Zoom

    if (enableZoom)
    {
      // Changes isZoomed when key is pressed
      // Behavior for toogle zoom
      if (Input.GetKeyDown(zoomKey) && !holdToZoom && !isSprinting)
      {
        if (!isZoomed)
        {
          isZoomed = true;
        }
        else
        {
          isZoomed = false;
        }
      }

      // Changes isZoomed when key is pressed
      // Behavior for hold to zoom
      if (holdToZoom && !isSprinting)
      {
        if (Input.GetKeyDown(zoomKey))
        {
          isZoomed = true;
        }
        else if (Input.GetKeyUp(zoomKey))
        {
          isZoomed = false;
        }
      }

      // Lerps camera.fieldOfView to allow for a smooth transistion
      if (isZoomed)
      {
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, zoomStepTime * Time.deltaTime);
      }
      else if (!isZoomed && !isSprinting)
      {
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, zoomStepTime * Time.deltaTime);
      }
    }

    #endregion
    #endregion

    #region Sprint

    if (enableSprint)
    {
      if (isSprinting)
      {
        isZoomed = false;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, sprintFOVStepTime * Time.deltaTime);

        // Drain sprint remaining while sprinting
        if (!unlimitedSprint)
        {
          sprintRemaining -= 1 * Time.deltaTime;
          if (sprintRemaining <= 0)
          {
            isSprinting = false;
            isSprintCooldown = true;
          }
        }
      }
      else
      {
        // Regain sprint while not sprinting
        sprintRemaining = Mathf.Clamp(sprintRemaining += 1 * Time.deltaTime, 0, sprintDuration);
      }

      // Handles sprint cooldown 
      // When sprint remaining == 0 stops sprint ability until hitting cooldown
      if (isSprintCooldown)
      {
        sprintCooldown -= 1 * Time.deltaTime;
        if (sprintCooldown <= 0)
        {
          isSprintCooldown = false;
        }
      }
      else
      {
        sprintCooldown = sprintCooldownReset;
      }

      // Handles sprintBar 
      if (useSprintBar && !unlimitedSprint)
      {
        float sprintRemainingPercent = sprintRemaining / sprintDuration;
        sprintBar.transform.localScale = new Vector3(sprintRemainingPercent, 1f, 1f);
      }
    }

    #endregion

    #region Dash

    if (enableDash)
    {
      if (isDashing)
      {
        float dashProgress = (Time.time - dashStartTime) / dashDuration;
        if (dashProgress < 1f)
        {
          transform.position = Vector3.Lerp(dashStartPosition, dashEndPosition, dashProgress);
        }
        else
        {
          isDashing = false;
        }

        isZoomed = false;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, dashFOV, dashFOVStepTime * Time.deltaTime);

        // Drain dash remaining while dashing
        if (!unlimitedDash)
        {
          dashRemaining = 0; // dash set to 0 if dash is used
          if (dashRemaining <= 0)
          {
            isDashing = false;
            isDashCooldown = true;
          }
        }
      }
      else
      {
        // Regain dash while not dashing
        dashRemaining = Mathf.Clamp(dashRemaining += 1 * Time.deltaTime, 0, dashDuration);
      }

      // Handles dash cooldown 
      // When dash remaining == 0 stops dash ability until hitting cooldown
      if (isDashCooldown)
      {
        dashCooldown -= 1 * Time.deltaTime;
        if (dashCooldown <= 0)
        {
          isDashCooldown = false;
        }
      }
      else
      {
        dashCooldown = dashCooldownReset;
      }

      // Handles dashBar 
      if (useDashBar && !unlimitedDash)
      {
        float dashRemainingPercent = dashRemaining / dashDuration;
        dashBar.transform.localScale = new Vector3(dashRemainingPercent, 1f, 1f);
      }
    }

    #endregion

    #region Jump

    // Gets input and calls jump method
    if (enableJump && Input.GetKeyDown(jumpKey) && isGrounded)
    {
      Jump();
    }

    #endregion

    CheckGround();

    if (enableHeadBob)
    {
      HeadBob();
    }
  }

  void FixedUpdate()
  {
    #region Movement

    if (playerCanMove)
    {
      // Calculate how fast we should be moving
      Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

      // All movement calculations while dash is active
      if (enableDash && Input.GetKeyDown(dashKey) && dashRemaining > 0f && !isDashCooldown)
      {

        // Set dashing state
        isDashing = true;
        dashStartTime = Time.time;

        // Apply the dash movement
        Vector3 dashDirection = transform.forward;
        dashDirection.y = 0; // Ensure the dash does not affect the Y-axis
        rb.AddForce(dashDirection.normalized * dashDistance, ForceMode.VelocityChange);

        // Show dash bar if applicable
        if (hideDashBarWhenFull && !unlimitedDash)
        {
          dashBarCG.alpha += 5 * Time.deltaTime;
        }

        // Drain dash remaining
        if (!unlimitedDash)
        {
          dashRemaining = 0;
          if (dashRemaining <= 0)
          {
            isDashCooldown = true;
          }
        }
      }

      // Checks if player is walking and isGrounded
      // Will allow head bob
      if (targetVelocity.x != 0 || targetVelocity.z != 0 && isGrounded)
      {
        isWalking = true;
      }
      else
      {
        isWalking = false;
      }

      // All movement calculations while sprint is active
      if (enableSprint && Input.GetKey(sprintKey) && sprintRemaining > 0f && !isSprintCooldown)
      {
        targetVelocity = transform.TransformDirection(targetVelocity) * sprintSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        // Player is only moving when valocity change != 0
        // Makes sure fov change only happens during movement
        if (velocityChange.x != 0 || velocityChange.z != 0)
        {
          isSprinting = true;

          if (hideSprintBarWhenFull && !unlimitedSprint)
          {
            sprintBarCG.alpha += 5 * Time.deltaTime;
          }
        }

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
      }
      // All movement calculations while walking
      else
      {
        isSprinting = false;

        if (hideSprintBarWhenFull && sprintRemaining == sprintDuration)
        {
          sprintBarCG.alpha -= 3 * Time.deltaTime;
        }

        targetVelocity = transform.TransformDirection(targetVelocity) * walkSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
      }
    }

    #endregion
  }

  // Sets isGrounded based on a raycast sent straigth down from the player object
  private void CheckGround()
  {
    Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
    Vector3 direction = transform.TransformDirection(Vector3.down);
    float distance = .75f;

    if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
    {
      Debug.DrawRay(origin, direction * distance, Color.red);
      isGrounded = true;
    }
    else
    {
      isGrounded = false;
    }
  }

  private void Jump()
  {
    // Adds force to the player rigidbody to jump
    if (isGrounded)
    {
      rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
      isGrounded = false;
    }
  }

  private void HeadBob()
  {
    if (isWalking)
    {
      // Calculates HeadBob speed during sprint
      if (isSprinting)
      {
        timer += Time.deltaTime * (bobSpeed + sprintSpeed);
      }
      // Calculates HeadBob speed during walking
      else
      {
        timer += Time.deltaTime * bobSpeed;
      }
      // Applies HeadBob movement
      joint.localPosition = new Vector3(jointOriginalPos.x + Mathf.Sin(timer) * bobAmount.x, jointOriginalPos.y + Mathf.Sin(timer) * bobAmount.y, jointOriginalPos.z + Mathf.Sin(timer) * bobAmount.z);
    }
    else
    {
      // Resets when play stops moving
      timer = 0;
      joint.localPosition = new Vector3(Mathf.Lerp(joint.localPosition.x, jointOriginalPos.x, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.y, jointOriginalPos.y, Time.deltaTime * bobSpeed), Mathf.Lerp(joint.localPosition.z, jointOriginalPos.z, Time.deltaTime * bobSpeed));
    }
  }
}



// Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
public class FirstPersonControllerEditor : Editor
{
  FirstPersonController fpc;
  SerializedObject SerFPC;

  private void OnEnable()
  {
    fpc = (FirstPersonController)target;
    SerFPC = new SerializedObject(fpc);
  }

  public override void OnInspectorGUI()
  {
    SerFPC.Update();

    EditorGUILayout.Space();
    GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
    GUILayout.Label("By Jess Case", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
    GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
    EditorGUILayout.Space();

    #region Camera Setup

    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
    EditorGUILayout.Space();

    fpc.playerCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Camera", "Camera attached to the controller."), fpc.playerCamera, typeof(Camera), true);
    fpc.fov = EditorGUILayout.Slider(new GUIContent("Field of View", "The camera’s view angle. Changes the player camera directly."), fpc.fov, fpc.zoomFOV, 179f);
    fpc.cameraCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Camera Rotation", "Determines if the camera is allowed to move."), fpc.cameraCanMove);

    GUI.enabled = fpc.cameraCanMove;
    fpc.invertCamera = EditorGUILayout.ToggleLeft(new GUIContent("Invert Camera Rotation", "Inverts the up and down movement of the camera."), fpc.invertCamera);
    fpc.mouseSensitivity = EditorGUILayout.Slider(new GUIContent("Look Sensitivity", "Determines how sensitive the mouse movement is."), fpc.mouseSensitivity, .1f, 10f);
    fpc.maxLookAngle = EditorGUILayout.Slider(new GUIContent("Max Look Angle", "Determines the max and min angle the player camera is able to look."), fpc.maxLookAngle, 40, 150);
    GUI.enabled = true;

    fpc.lockCursor = EditorGUILayout.ToggleLeft(new GUIContent("Lock and Hide Cursor", "Turns off the cursor visibility and locks it to the middle of the screen."), fpc.lockCursor);

    fpc.crosshair = EditorGUILayout.ToggleLeft(new GUIContent("Auto Crosshair", "Determines if the basic crosshair will be turned on, and sets is to the center of the screen."), fpc.crosshair);

    // Only displays crosshair options if crosshair is enabled
    if (fpc.crosshair)
    {
      EditorGUI.indentLevel++;
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PrefixLabel(new GUIContent("Crosshair Image", "Sprite to use as the crosshair."));
      fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField(fpc.crosshairImage, typeof(Sprite), false);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      fpc.crosshairColor = EditorGUILayout.ColorField(new GUIContent("Crosshair Color", "Determines the color of the crosshair."), fpc.crosshairColor);
      EditorGUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }

    EditorGUILayout.Space();

    #region Camera Zoom Setup

    GUILayout.Label("Zoom", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

    fpc.enableZoom = EditorGUILayout.ToggleLeft(new GUIContent("Enable Zoom", "Determines if the player is able to zoom in while playing."), fpc.enableZoom);

    GUI.enabled = fpc.enableZoom;
    fpc.holdToZoom = EditorGUILayout.ToggleLeft(new GUIContent("Hold to Zoom", "Requires the player to hold the zoom key instead if pressing to zoom and unzoom."), fpc.holdToZoom);
    fpc.zoomKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Zoom Key", "Determines what key is used to zoom."), fpc.zoomKey);
    fpc.zoomFOV = EditorGUILayout.Slider(new GUIContent("Zoom FOV", "Determines the field of view the camera zooms to."), fpc.zoomFOV, .1f, fpc.fov);
    fpc.zoomStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while zooming in."), fpc.zoomStepTime, .1f, 10f);
    GUI.enabled = true;

    #endregion

    #endregion

    #region Movement Setup

    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
    EditorGUILayout.Space();

    fpc.playerCanMove = EditorGUILayout.ToggleLeft(new GUIContent("Enable Player Movement", "Determines if the player is allowed to move."), fpc.playerCanMove);

    GUI.enabled = fpc.playerCanMove;
    fpc.walkSpeed = EditorGUILayout.Slider(new GUIContent("Walk Speed", "Determines how fast the player will move while walking."), fpc.walkSpeed, .1f, fpc.sprintSpeed);
    GUI.enabled = true;

    EditorGUILayout.Space();

    #region Sprint

    GUILayout.Label("Sprint", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

    fpc.enableSprint = EditorGUILayout.ToggleLeft(new GUIContent("Enable Sprint", "Determines if the player is allowed to sprint."), fpc.enableSprint);

    GUI.enabled = fpc.enableSprint;
    fpc.unlimitedSprint = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Sprint", "Determines if 'Sprint Duration' is enabled. Turning this on will allow for unlimited sprint."), fpc.unlimitedSprint);
    fpc.sprintKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Sprint Key", "Determines what key is used to sprint."), fpc.sprintKey);
    fpc.sprintSpeed = EditorGUILayout.Slider(new GUIContent("Sprint Speed", "Determines how fast the player will move while sprinting."), fpc.sprintSpeed, fpc.walkSpeed, 20f);

    //GUI.enabled = !fpc.unlimitedSprint;
    fpc.sprintDuration = EditorGUILayout.Slider(new GUIContent("Sprint Duration", "Determines how long the player can sprint while unlimited sprint is disabled."), fpc.sprintDuration, 1f, 20f);
    fpc.sprintCooldown = EditorGUILayout.Slider(new GUIContent("Sprint Cooldown", "Determines how long the recovery time is when the player runs out of sprint."), fpc.sprintCooldown, .1f, fpc.sprintDuration);
    //GUI.enabled = true;

    fpc.sprintFOV = EditorGUILayout.Slider(new GUIContent("Sprint FOV", "Determines the field of view the camera changes to while sprinting."), fpc.sprintFOV, fpc.fov, 179f);
    fpc.sprintFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while sprinting."), fpc.sprintFOVStepTime, .1f, 20f);

    fpc.useSprintBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Sprint Bar", "Determines if the default sprint bar will appear on screen."), fpc.useSprintBar);

    // Only displays sprint bar options if sprint bar is enabled
    if (fpc.useSprintBar)
    {
      EditorGUI.indentLevel++;

      EditorGUILayout.BeginHorizontal();
      fpc.hideSprintBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the sprint bar when sprint duration is full, and fades the bar in when sprinting. Disabling this will leave the bar on screen at all times when the sprint bar is enabled."), fpc.hideSprintBarWhenFull);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
      fpc.sprintBarBG = (Image)EditorGUILayout.ObjectField(fpc.sprintBarBG, typeof(Image), true);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as sprint bar foreground."));
      fpc.sprintBar = (Image)EditorGUILayout.ObjectField(fpc.sprintBar, typeof(Image), true);
      EditorGUILayout.EndHorizontal();


      EditorGUILayout.BeginHorizontal();
      fpc.sprintBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the sprint bar."), fpc.sprintBarWidthPercent, .1f, .5f);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      fpc.sprintBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the sprint bar."), fpc.sprintBarHeightPercent, .001f, .025f);
      EditorGUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }
    GUI.enabled = true;

    EditorGUILayout.Space();

    #endregion

    #region Dash

    GUILayout.Label("Dash", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

    fpc.enableDash = EditorGUILayout.ToggleLeft(new GUIContent("Enable Dash", "Determines if the player is allowed to dash."), fpc.enableDash);

    GUI.enabled = fpc.enableDash;
    fpc.unlimitedDash = EditorGUILayout.ToggleLeft(new GUIContent("Unlimited Dash", "Determines if 'Dash Duration' is enabled. Turning this on will allow for unlimited dash."), fpc.unlimitedDash);
    fpc.dashKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Dash Key", "Determines what key is used to dash."), fpc.dashKey);
    fpc.dashDistance = EditorGUILayout.Slider(new GUIContent("Dash Distance", "Determines how far the player will move when dashing."), fpc.dashDistance, fpc.walkSpeed, 99999f);


    //GUI.enabled = !fpc.unlimitedDash;
    fpc.dashDuration = EditorGUILayout.Slider(new GUIContent("Dash Duration", "Determines how long the player can dash while unlimited dash is disabled."), fpc.dashDuration, 1f, 20f);
    fpc.dashCooldown = EditorGUILayout.Slider(new GUIContent("Dash Cooldown", "Determines how long the recovery time is when the player runs out of dash."), fpc.dashCooldown, 0f, 20f);
    //GUI.enabled = true;

    fpc.dashFOV = EditorGUILayout.Slider(new GUIContent("Dash FOV", "Determines the field of view the camera changes to while dashing."), fpc.dashFOV, fpc.fov, 179f);
    fpc.dashFOVStepTime = EditorGUILayout.Slider(new GUIContent("Step Time", "Determines how fast the FOV transitions while dashing."), fpc.dashFOVStepTime, .1f, 20f);

    fpc.useDashBar = EditorGUILayout.ToggleLeft(new GUIContent("Use Dash Bar", "Determines if the default dash bar will appear on screen."), fpc.useDashBar);

    // Only displays dash bar options if dash bar is enabled
    if (fpc.useDashBar)
    {
      EditorGUI.indentLevel++;

      EditorGUILayout.BeginHorizontal();
      fpc.hideDashBarWhenFull = EditorGUILayout.ToggleLeft(new GUIContent("Hide Full Bar", "Hides the dash bar when dash duration is full, and fades the bar in when dashing. Disabling this will leave the bar on screen at all times when the dash bar is enabled."), fpc.hideDashBarWhenFull);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PrefixLabel(new GUIContent("Bar BG", "Object to be used as sprint bar background."));
      fpc.dashBarBG = (Image)EditorGUILayout.ObjectField(fpc.dashBarBG, typeof(Image), true);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PrefixLabel(new GUIContent("Bar", "Object to be used as dash bar foreground."));
      fpc.dashBar = (Image)EditorGUILayout.ObjectField(fpc.dashBar, typeof(Image), true);
      EditorGUILayout.EndHorizontal();


      EditorGUILayout.BeginHorizontal();
      fpc.dashBarWidthPercent = EditorGUILayout.Slider(new GUIContent("Bar Width", "Determines the width of the dash bar."), fpc.dashBarWidthPercent, .1f, .5f);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      fpc.dashBarHeightPercent = EditorGUILayout.Slider(new GUIContent("Bar Height", "Determines the height of the dash bar."), fpc.dashBarHeightPercent, .001f, .025f);
      EditorGUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }
    GUI.enabled = true;

    EditorGUILayout.Space();

    #endregion

    #region Jump

    GUILayout.Label("Jump", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));

    fpc.enableJump = EditorGUILayout.ToggleLeft(new GUIContent("Enable Jump", "Determines if the player is allowed to jump."), fpc.enableJump);

    GUI.enabled = fpc.enableJump;
    fpc.jumpKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Jump Key", "Determines what key is used to jump."), fpc.jumpKey);
    fpc.jumpPower = EditorGUILayout.Slider(new GUIContent("Jump Power", "Determines how high the player will jump."), fpc.jumpPower, .1f, 20f);
    GUI.enabled = true;

    EditorGUILayout.Space();

    #endregion

    #endregion

    #region Head Bob

    EditorGUILayout.Space();
    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
    EditorGUILayout.Space();

    fpc.enableHeadBob = EditorGUILayout.ToggleLeft(new GUIContent("Enable Head Bob", "Determines if the camera will bob while the player is walking."), fpc.enableHeadBob);


    GUI.enabled = fpc.enableHeadBob;
    fpc.joint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Camera Joint", "Joint object position is moved while head bob is active."), fpc.joint, typeof(Transform), true);
    fpc.bobSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Determines how often a bob rotation is completed."), fpc.bobSpeed, 1, 20);
    fpc.bobAmount = EditorGUILayout.Vector3Field(new GUIContent("Bob Amount", "Determines the amount the joint moves in both directions on every axes."), fpc.bobAmount);
    GUI.enabled = true;

    #endregion

    //Sets any changes from the prefab
    if (GUI.changed)
    {
      EditorUtility.SetDirty(fpc);
      Undo.RecordObject(fpc, "FPC Change");
      SerFPC.ApplyModifiedProperties();
    }
  }

}

#endif