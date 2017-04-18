using UnityEngine;
using UnityEngine.Experimental.Input;

public abstract class SimpleCharacterControlBase : MonoBehaviour
{
    protected Vector3 m_NewPosition;
    protected Vector3 m_NewRotation;

    [SerializeField] private float m_GamepadPollingFrequency = 30.0f;
    public float gamepadPollingFrequency
    {
        get { return m_GamepadPollingFrequency; }
        set
        {
            m_GamepadPollingFrequency = value;
            InputSystem.nativeInputPollingFrequency = value;
        }
    }

    public GameObject projectilePrefab;
    private float m_TimeOfLastFire;

    public void OnEnable()
    {
        SetCursorLock(true);
        InputSystem.nativeInputPollingFrequency = gamepadPollingFrequency;
    }

    public void OnDisable()
    {
        SetCursorLock(false);
    }

    protected abstract void ProcessInput();

    public void FixedUpdate()
    {
        var transform = this.transform;
        var oldPosition = transform.position;
        var oldRotation = transform.localEulerAngles;
        m_NewPosition = oldPosition;

        ProcessInput();

        if (m_NewPosition != oldPosition)
            transform.position = m_NewPosition;
        if (m_NewRotation != oldRotation)
            transform.localEulerAngles = m_NewRotation;
    }

    private void SetCursorLock(bool value)
    {
        var pointer = Pointer.current;
        if (pointer != null && pointer.cursor != null)
            pointer.cursor.isLocked = value;
    }

    protected void FireProjectileInInterval()
    {
        var currentTime = Time.time;
        if ((currentTime - m_TimeOfLastFire) < 0.1f)
            return;

        var newProjectile = Instantiate(projectilePrefab);
        var newProjectileTransform = newProjectile.transform;
        var ourTransform = transform;

        newProjectileTransform.position = ourTransform.position + ourTransform.forward * 0.6f + ourTransform.up * 0.5f;
        newProjectileTransform.rotation = ourTransform.rotation;
        newProjectile.GetComponent<Rigidbody>().AddForce(ourTransform.forward * 20f, ForceMode.Impulse);
        newProjectile.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);

        m_TimeOfLastFire = currentTime;
    }
}
