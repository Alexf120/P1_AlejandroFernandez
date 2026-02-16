1) Control del jugador y cámara (PlayerControler.cs)
Movimiento WASD relativo a la cámara

Para el movimiento he buscado que W siempre avance hacia donde mira la cámara, independientemente de la orientación del personaje. Para ello:
Leo los ejes Horizontal y Vertical.
Tomo los vectores forward y right de la cámara.
Proyecto esos vectores sobre el plano horizontal (para ignorar la inclinación vertical de la cámara) usando Vector3.ProjectOnPlane(..., Vector3.up).
Combino camRight * horizontal + camForward * vertical y normalizo el resultado para evitar que en diagonal se mueva más rápido.

Después aplico el desplazamiento en Space.World para que sea consistente con el mundo.

Opcional destacado: movimiento orientado a cámara (no solo en ejes globales).

Rotación con ratón (yaw/pitch)
He separado la rotación en dos partes:
Yaw (giro en Y): rota el personaje completo (transform.rotation = Quaternion.Euler(0, yaw, 0)).
Pitch (mirar arriba/abajo): rota un cameraPivot (un objeto padre de la cámara) para inclinar solo la cámara.
Además, el pitch está limitado con Mathf.Clamp(minPitch, maxPitch) para evitar giros extremos.

Opcional destacado: cámara tipo “FPS/3ª persona simple” con clamp vertical y cursor bloqueado (Cursor.lockState = Locked).

Salto + salto extra en el aire
El salto se hace con AddForce(..., Impulse) para que sea inmediato. Antes de aplicar fuerza, pongo la velocidad vertical a 0 para que el salto sea consistente incluso si caigo o estoy subiendo.
Además, he añadido un contador de saltos en el aire (extraAirJumps). Cuando el jugador está en el suelo se resetea, y si está en el aire permite un número limitado de saltos extra.

Opcional destacado: doble salto / salto extra en el aire configurable.
Detección de suelo
Uso colisiones con objetos taggeados como "Ground":
OnCollisionEnter/Stay: isGrounded = true
OnCollisionExit: isGrounded = false

Esto simplifica la lógica del salto y el reseteo de saltos extra.
Notas de configuración:
El suelo debe tener tag "Ground".
El jugador necesita Rigidbody + Collider.
Asignar cameraPivot (objeto que rota en X) y opcionalmente cameraTransform (si no se asigna, intenta usar Camera.main).

2) Plataforma móvil (MovingPlatformClase.cs)
Movimiento entre dos puntos (ida y vuelta)
La plataforma se mueve entre:
posicionInicial (donde empieza la plataforma)
posicionFinal (posición de un objeto “destino” en la escena)
Cada FixedUpdate calculo la siguiente posición con Vector3.MoveTowards(...) en función de si está yendo a la final o volviendo al inicio. Cuando está lo suficientemente cerca de uno de los extremos (distancia < 0.01), invierto el sentido (voyALaFinal).

Uso de Rigidbody para mover
En lugar de modificar directamente transform.position, utilizo rb.MovePosition(newPosition) para que el movimiento sea más consistente con la física y las colisiones.

Extra: ocultar el punto destino
El objeto destino se usa como “marcador” de posición final, pero lo oculto en ejecución desactivando su MeshRenderer, así puedo colocarlo visualmente en el editor sin que aparezca en el juego.
Opcional destacado: uso de waypoint (destino) oculto + movimiento con Rigidbody.MovePosition.

Notas de configuración:
La plataforma debe tener Rigidbody (normalmente cinemático si no queremos que la gravedad la afecte).
Asignar en el inspector el GameObject destino como punto final.

3) Plataforma que cae al pisarla (FallingPlatform.cs)
Activación al detectar al jugador

La plataforma detecta colisión con el jugador usando un tag configurable (playerTag = "Player"). Para evitar múltiples activaciones, uso una bandera activated (si ya está activada, ignoro nuevas colisiones).

Opcional: “solo si se pisa desde arriba”

He añadido la opción onlyFromAbove para que no se active si el jugador choca lateralmente.
Para ello reviso los contactos de la colisión (collision.contacts) y compruebo si existe alguna normal con componente Y positiva (normal.y > 0.5f), lo cual indica que el contacto viene desde “arriba” de la plataforma.

Opcional destacado: activación únicamente si se pisa desde arriba (usando normales de contacto).

Secuencia completa con corrutina (caer y reiniciar)
La lógica principal está en una corrutina FallAndReset():
Espera antes de caer (fallDelay) para dar feedback al jugador.
Caída controlada: baja a velocidad constante fallSpeed hasta una altura mínima minY (calculada con fallDistance).
Espera abajo (resetWait).
Vuelta a la posición inicial con Vector3.MoveTowards usando returnSpeed.
Reinicio de estado (activated = false) para poder reutilizar la plataforma.

Para que el movimiento sea completamente controlado por script:
Rigidbody está en kinematic y sin gravedad.
El desplazamiento se hace con rb.MovePosition(...) sincronizado con FixedUpdate (WaitForFixedUpdate).

Opcional destacado: plataforma reutilizable (cae + vuelve) con tiempos configurables.
Notas de configuración:
El jugador debe tener tag "Player" (o cambiar playerTag).
La plataforma necesita Rigidbody + Collider.
fallDistance, fallSpeed, returnSpeed, fallDelay se pueden ajustar desde el inspector.
