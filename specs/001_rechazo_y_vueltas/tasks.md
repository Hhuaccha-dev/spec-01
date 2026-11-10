# Rechazo y nueva vuelta · tareas

## Tests

- [x] T1 (RF-01) Añadir test de rechazo válido que verifica firmante, motivo, versión e instante registrados · 25 min
- [ ] T2 (RF-02) Añadir test de rechazo antes de la versión 3 que verifica que el estado pasa a `Devuelto` · 20 min
- [ ] T3 (RF-03) Añadir test de rechazo en la versión 3 que verifica el estado `Rechazado` y que no se admite reenviar · 25 min
- [ ] T4 (RF-04) Añadir test de reenvío válido del autor que verifica el incremento de versión, el estado `EnCircuito` y el turno del primer firmante · 25 min
- [ ] T5 (RF-05) Añadir test que verifica que las firmas anteriores se conservan y no completan la aprobación de la versión vigente · 25 min
- [ ] T6 (RF-06) Añadir test de motivo vacío o compuesto por espacios que verifica el mensaje exacto `"El motivo de rechazo es obligatorio."` y que no cambia estado ni historial · 20 min
- [ ] T7 (RF-07) Añadir test de rechazo fuera de turno que verifica el mismo error exacto que al aprobar y que no cambia estado ni historial · 20 min
- [ ] T8 (RF-08) Añadir test de reenvío por alguien distinto del autor que verifica el mensaje exacto `"Solo el autor puede reenviar el documento."` y que no cambia versión ni estado · 20 min
- [ ] T9 (RF-09) Añadir test de rechazo o reenvío desde estados no permitidos que verifica el error y que no cambia estado ni historial · 25 min
- [ ] T10 (RF-10) Añadir test de compatibilidad que compila y ejercita las firmas públicas existentes del constructor, `Enviar` y `Aprobar` · 20 min

## Implementación

- [ ] T11 (RF-01) Crear el record inmutable `Rechazo`, almacenar el historial e implementar el registro de los datos del rechazo válido · 30 min
- [ ] T12 (RF-02) Agregar el estado `Devuelto` y pasar a ese estado al rechazar antes de la versión 3 · 25 min
- [ ] T13 (RF-03) Agregar el estado `Rechazado` y cerrar el documento cuando se rechaza durante la versión 3 · 25 min
- [ ] T14 (RF-06) Implementar la validación del motivo y lanzar el mensaje especificado antes de modificar el circuito · 20 min
- [ ] T15 (RF-07) Implementar la validación del firmante en turno para rechazar con el mismo error que al aprobar · 25 min
- [ ] T16 (RF-04) Implementar el reenvío del autor, incrementando la versión, pasando a `EnCircuito` y asignando el primer turno · 30 min
- [ ] T17 (RF-08) Implementar la validación de autor al reenviar y lanzar el mensaje especificado sin modificar el circuito · 20 min
- [ ] T18 (RF-09) Implementar las validaciones de estado para rechazar y reenviar, evitando cambios parciales ante errores · 25 min
- [ ] T19 (RF-05) Implementar el cálculo del turno y de aprobación contando solo las firmas de la versión vigente y conservando las anteriores · 30 min
- [ ] T20 (RF-10) Implementar las nuevas operaciones sin cambiar las firmas públicas existentes del constructor, `Enviar` y `Aprobar` · 20 min

## Verificación

- [ ] T21 (CF-04) Verificar que todos los miembros públicos nuevos tienen XML docs en español · 20 min
- [ ] T22 (CF-03) Verificar que las firmas públicas existentes permanecen intactas y que los 5 tests originales no se modificaron · 20 min
- [ ] T23 (CF-02) Verificar que cada RF-01 a RF-10 tiene al menos un test cuyo nombre incluye su identificador · 20 min
- [ ] T24 (CF-01) Ejecutar `dotnet test` y verificar código de salida 0 con los tests de todos los RF en verde · 30 min
