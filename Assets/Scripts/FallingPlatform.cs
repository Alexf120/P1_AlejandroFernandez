using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool onlyFromAbove = true;

    [Header("Timing")]
    [SerializeField] private float fallDelay = 0.75f; // 0.5 - 1s recomendado
    [SerializeField] private float resetWait = 0.5f;

    [Header("Motion")]
    [SerializeField] private float fallSpeed = 4.0f;      // unidades/seg
    [SerializeField] private float fallDistance = 6.0f;   // cuánto baja desde su posición inicial
    [SerializeField] private float returnSpeed = 8.0f;    // velocidad al volver

    private Rigidbody rb;

    private Vector3 startPos;
    private Quaternion startRot;
    private float minY;

    private bool activated;
    private Coroutine routine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Plataforma controlada por script
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        startPos = transform.position;
        startRot = transform.rotation;
        minY = startPos.y - fallDistance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (activated) return;
        if (!collision.collider.CompareTag(playerTag)) return;

        if (onlyFromAbove)
        {
            // Buscamos un contacto cuya normal apunte hacia arriba
            bool steppedFromAbove = false;

            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.contacts[i].normal.y > 0.5f)
                {
                    steppedFromAbove = true;
                    break;
                }
            }

            if (!steppedFromAbove) return;
        }

        activated = true;
        routine = StartCoroutine(FallAndReset());
    }

    private IEnumerator FallAndReset()
    {
        // 1) Espera antes de caer
        yield return new WaitForSeconds(fallDelay);

        // 2) Cae a velocidad constante hasta minY
        while (rb.position.y > minY + 0.001f)
        {
            Vector3 p = rb.position;
            p.y -= fallSpeed * Time.fixedDeltaTime;

            if (p.y < minY) p.y = minY;

            rb.MovePosition(p);
            yield return new WaitForFixedUpdate();
        }

        // 3) Espera abajo
        yield return new WaitForSeconds(resetWait);

        // 4) Vuelve a la posición inicial
        while ((rb.position - startPos).sqrMagnitude > 0.0001f)
        {
            Vector3 p = Vector3.MoveTowards(rb.position, startPos, returnSpeed * Time.fixedDeltaTime);
            rb.MovePosition(p);

            transform.rotation = startRot;

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(startPos);
        transform.rotation = startRot;

        // 5) Reinicia para reutilizar
        activated = false;
        routine = null;
    }
}
