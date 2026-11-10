# Rechazo y nueva vuelta · spec

## 1. Contexto

Hoy `CircuitoFirma` permite aprobar un documento en orden, pero no rechazarlo ni devolverlo al autor para corregirlo. Se necesita que el firmante en turno pueda rechazar con un motivo y que el autor pueda reenviar el documento, sin perder el historial de firmas.

La versión inicial es la 1 y el máximo es de 3 vueltas. Un rechazo durante la vuelta 3 cierra el documento como `Rechazado`. El modelo actual no representa el contenido del documento.

## 2. Historias de usuario

- Como firmante en turno quiero rechazar un documento indicando el motivo para que el autor pueda corregirlo.
- Como autor quiero reenviar un documento devuelto para iniciar una nueva vuelta de aprobación.
- Como participante del circuito quiero conservar las firmas de cada versión para consultar el historial del proceso.

## 3. Requisitos funcionales (EARS)

- RF-01 · CUANDO el firmante en turno rechace un documento `EnCircuito` con un motivo válido, el sistema DEBERÁ registrar el rechazo con firmante, motivo, versión e instante.
- RF-02 · CUANDO se rechace un documento cuya versión vigente sea menor que 3, el sistema DEBERÁ pasar el documento a `Devuelto`.
- RF-03 · CUANDO se rechace un documento durante la vuelta 3, el sistema DEBERÁ pasar el documento a `Rechazado` y NO DEBERÁ admitir más reenvíos.
- RF-04 · CUANDO el autor reenvíe un documento `Devuelto`, el sistema DEBERÁ incrementar la versión en 1, pasar el documento a `EnCircuito` y asignar el turno al primer firmante.
- RF-05 · El sistema DEBERÁ conservar las firmas de versiones anteriores y NO DEBERÁ contarlas para completar la aprobación de la versión vigente.
- RF-06 · SI el motivo de rechazo está vacío o solo contiene espacios, ENTONCES el sistema DEBERÁ lanzar `CircuitoFirmaException` con el mensaje `"El motivo de rechazo es obligatorio."` y NO DEBERÁ registrar el rechazo ni cambiar el estado.
- RF-07 · MIENTRAS el documento esté `EnCircuito`, solo el firmante en turno podrá rechazarlo; si otro firmante intenta hacerlo, el sistema DEBERÁ lanzar el mismo error de turno que al aprobar.
- RF-08 · CUANDO alguien distinto de `AutorId` intente reenviar el documento, el sistema DEBERÁ lanzar `CircuitoFirmaException` con el mensaje `"Solo el autor puede reenviar el documento."` y NO DEBERÁ cambiar la versión ni el estado.
- RF-09 · SI se intenta rechazar o reenviar desde un estado no permitido, ENTONCES el sistema DEBERÁ lanzar `CircuitoFirmaException` sin modificar el estado ni el historial.
- RF-10 · El sistema DEBERÁ conservar intactas las firmas públicas existentes del constructor, `Enviar` y `Aprobar`.

## 4. Casos límite y errores

- Rechazo por un firmante fuera de turno → lanzar el mismo error de turno que al aprobar; no registrar el rechazo ni cambiar el estado.
- Rechazo con motivo vacío o compuesto solo por espacios → lanzar el error de RF-06; no registrar el rechazo ni cambiar el estado.
- Reenvío por alguien distinto del autor → lanzar el error de RF-08; no incrementar la versión ni cambiar el estado.
- Rechazo durante la vuelta 3 → pasar el documento a `Rechazado`; no admitir reenvíos posteriores.
- Rechazo desde un estado distinto de `EnCircuito` o reenvío desde un estado distinto de `Devuelto` → lanzar `CircuitoFirmaException`; no modificar el estado ni el historial.
- Reenvío después de un rechazo en una vuelta anterior a la 3 → incrementar la versión en 1 y asignar el turno al primer firmante.
- Firmas de versiones anteriores → conservarlas asociadas a sus versiones y excluirlas del cómputo de aprobación de la versión vigente.

## 5. Criterios de finalización

- CF-01 · `dotnet test` termina con código de salida 0.
- CF-02 · Cada requisito RF-01 a RF-10 tiene al menos un test cuyo nombre incluye el identificador del requisito correspondiente.
- CF-03 · Las firmas públicas existentes del constructor, `Enviar` y `Aprobar` no cambian; los 5 tests existentes pasan y no se modifican.
- CF-04 · Todos los miembros públicos nuevos tienen XML docs en español.

## Preguntas abiertas

- ¿Se puede reenviar sin cambiar el contenido? El modelo actual no representa ni compara el contenido.
- ¿El historial de rechazos debe exponerse mediante una propiedad pública, además de conservarse?
- ¿El motivo se conserva literalmente o se normalizan sus espacios?
- ¿Qué mensajes deben usarse para intentos de rechazo o reenvío desde estados no permitidos?
- ¿Se requiere garantizar atomicidad ante operaciones concurrentes, o basta con definir el comportamiento para llamadas secuenciales?
