using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    public float speed = 5f;
    public float speed2 = 7f;
    private bool facingRight = true;
    public Animator anim;
    public Transform flashlightTransform;

    public int maxHealth = 20;
    public int minHealth = 2;
    public int currentHealth;
    public HealthBar healthBar;

    public Image StaminaBar;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float RunCost = 10f;
    public float staminaRegenRate = 10f;
    public float oxygenBonusPercent = 0.5f;
    private bool canRun = true;

    public GameObject flashlightLightSource;
    public Image batteryBar;
    public float maxBattery = 15f;
    public float currentBattery;
    private bool isFlashlightOn = false;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);

        currentStamina = maxStamina;
        currentBattery = maxBattery;

        if (flashlightLightSource != null) flashlightLightSource.SetActive(false);

        UpdateStaminaUI();
        UpdateBatteryUI();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        if (currentStamina <= 0) canRun = false;
        else if (currentStamina > 20f) canRun = true;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && h != 0 && canRun;
        transform.Translate(speed * Time.deltaTime * h, 0, 0);

        if (isRunning)
        {
            transform.Translate(speed2 * Time.deltaTime * h, 0, 0);
            currentStamina -= RunCost * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
            UpdateStaminaUI();
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
                UpdateStaminaUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentBattery > 0)
            {
                isFlashlightOn = !isFlashlightOn;
                if (flashlightLightSource != null) flashlightLightSource.SetActive(isFlashlightOn);
            }
        }

        if (isFlashlightOn)
        {
            currentBattery -= Time.deltaTime;
            UpdateBatteryUI();

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isFlashlightOn = false;
                if (flashlightLightSource != null) flashlightLightSource.SetActive(false);
            }
        }

        if (h > 0 && !facingRight) FlipFlashlight();
        else if (h < 0 && facingRight) FlipFlashlight();
        HandleAnimations(h, isRunning);
        if (Input.GetKeyDown(KeyCode.Space)) TakeDamage(2);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Oxygen"))
        {
            float amountToRestore = maxStamina * oxygenBonusPercent;
            AddStamina(amountToRestore);
            Destroy(other.gameObject);
        }
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    void FlipFlashlight()
    {
        facingRight = !facingRight;
        if (flashlightTransform != null)
        {
            Vector3 currentRotation = flashlightTransform.localEulerAngles;
            currentRotation.y += 180;
            flashlightTransform.localEulerAngles = currentRotation;
        }
    }

    void HandleAnimations(float h, bool isRunning)
    {
        if (h > 0)
        {
            if (isRunning) anim.Play("RUN09");
            else anim.Play("WALKING09");
        }
        else if (h < 0)
        {
            if (isRunning) anim.Play("RUNL09");
            else anim.Play("WALKINGL09");
        }
        else
        {
            if (facingRight) anim.Play("IDLE09");
            else anim.Play("IDLEL09");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        if (currentHealth <= minHealth) GameOver();
    }

    void UpdateStaminaUI()
    {
        if (StaminaBar != null) StaminaBar.fillAmount = currentStamina / maxStamina;
    }

    void UpdateBatteryUI()
    {
        if (batteryBar != null) batteryBar.fillAmount = currentBattery / maxBattery;
    }

    void GameOver()
    {
        Destroy(gameObject);
    }
}