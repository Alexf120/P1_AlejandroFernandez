using UnityEngine;

public class MovingPlatformClase : MonoBehaviour
{
    private Vector3 posicionInicial;
    private Vector3 posicionFinal;

    [SerializeField] private float velocidad = 2.0f;
    [SerializeField] private GameObject destino;

    private Rigidbody rb;

    // esta variable indica si la plataforma se esttá moviendo hacia la posición final
    private bool voyALaFinal = false; 

    private void Start()
    {
        posicionInicial =  transform.position;
        posicionFinal = destino.transform.position;

        destino.GetComponent<MeshRenderer>().enabled = false;

        rb = GetComponent<Rigidbody>();
    }

    // Utilizamos fixedUpdate cuando movemos/aplicamos fuerzas/aplicamos velociad a un RIGIDBODY
    private void FixedUpdate() 
    {
        Vector3 posActual = transform.position;

        Vector3 newPosition;

        if (voyALaFinal)
        {
            newPosition = Vector3.MoveTowards(
                posActual,
                posicionFinal,
                velocidad * Time.fixedDeltaTime
            );
        }
        else
        {
            newPosition = Vector3.MoveTowards(
                posActual,
                posicionInicial,
                velocidad * Time.fixedDeltaTime
            );
        }

        rb.MovePosition(newPosition);

        if (Vector3.Distance(transform.position, posicionFinal) < 0.01f)
        {
            voyALaFinal = false;
        }
        else if (Vector3.Distance(transform.position, posicionInicial) < 0.01f)
        {
            voyALaFinal = true;
        }
    }
}
