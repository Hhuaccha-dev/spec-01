# Rechazo y nueva vuelta · plan

## 1. Archivos a crear o modificar

- Modificar `FirmaDocs/Circuito/CircuitoFirma.cs`: gestionar rechazos, reenvíos, estados terminales e índice del firmante en turno por versión.
- Modificar `FirmaDocs/Circuito/EstadoDocumento.cs`: agregar los estados `Devuelto` y `Rechazado`.
- Crear `FirmaDocs/Circuito/Rechazo.cs`: representar un rechazo inmutable con firmante, motivo, versión e instante.
- Modificar `FirmaDocs.Tests/Circuito/CircuitoFirmaTests.cs`: agregar pruebas por requisito RF-01 a RF-10 sin modificar los 5 tests existentes.

No se prevén cambios en `Program.cs`, dependencias ni firmas públicas existentes. Este plan no modifica la spec ni otros archivos.

## 2. Tipos nuevos y miembros públicos nuevos

- Tipo nuevo: `Rechazo`, un `record` inmutable con `Firmante`, `Motivo`, `Version` e `Instante`.
- Tipo existente: ampliar `EstadoDocumento` con `Devuelto` y `Rechazado`; no crear otro enum.
- Miembro público nuevo: `Rechazar(Firmante firmante, string motivo, DateTimeOffset instante)`. El instante se recibe como argumento; no se consulta el reloj del sistema.
- Miembro público nuevo: `Reenviar(string autorId)`, para validar que quien reenvía corresponde a `AutorId`.
- Miembro público nuevo propuesto: `IReadOnlyList<Rechazo> Rechazos`, para consultar el historial inmutable de rechazos. La spec deja abierta su exposición pública; confirmar esta propuesta antes de implementar.

Documentar en español todos los miembros públicos nuevos. No cambiar las firmas existentes de constructor, `Enviar` ni `Aprobar`.

## 3. Cálculo del turno por versión

El turno se calcula contando únicamente las firmas cuya `Version` coincide con la versión vigente. El índice de la siguiente persona es ese conteo; `FirmanteEnTurno` devuelve el firmante en ese índice solo si el documento está `EnCircuito`.

Al aprobar, se agrega una firma con la versión vigente y se vuelve a contar únicamente esa versión para determinar si se completaron todas las aprobaciones. No se debe usar `_firmas.Count` global para determinar el turno ni completar el circuito. Después de reenviar, la versión aumenta y no existen firmas de esa versión, por lo que el turno corresponde al primer firmante.

## 4. Alternativas de diseño descartadas

1. **Borrar las firmas previas al reenviar.** Descartada porque destruye evidencia y contradice la conservación del historial; las firmas deben permanecer asociadas a sus versiones.
2. **Mantener un contador de turno independiente y reiniciarlo en cada reenvío.** Descartada porque duplica información derivable de las firmas y puede desincronizarse del historial. Filtrar por versión mantiene el turno vinculado a la evidencia registrada.

## 5. Riesgos y mitigaciones

- **Contar firmas de versiones anteriores y saltarse firmantes.** Calcular turno y aprobación solo con las firmas de `Version`; cubrir reenvíos con varios firmantes en pruebas.
- **Aceptar reenvíos o rechazos en estados incorrectos, o reenvíos no autorizados.** Validar estado y autor antes de modificar versión, estado o historial; probar que los fallos no dejan cambios parciales.
- **Permitir más de 3 vueltas.** Usar la versión vigente como límite: un rechazo en versión 3 pasa a `Rechazado`, y solo se reenvían documentos `Devuelto`.
- **Perder o alterar evidencia del rechazo.** Representar cada rechazo con un record inmutable y conservarlo en el historial.
- **Exponer una API de historial no acordada.** La visibilidad pública de `Rechazos` sigue abierta en la spec; confirmar la propuesta antes de implementarla.
- **Interpretación de espacios en el motivo y mensajes para estados inválidos.** La spec solo fija el error para motivo vacío o compuesto por espacios. Confirmar si el motivo válido se almacena literalmente y acordar los mensajes no especificados antes de cerrar las pruebas correspondientes.
- **Llamadas concurrentes.** La spec no define atomicidad. Tratar el modelo como secuencial hasta que se decida si se requiere sincronización.
- **Contenido reenviado sin cambios.** El modelo no representa contenido, así que no puede verificar si hubo una corrección; mantener esta limitación explícita.

## 6. Orden de implementación

1. Confirmar las preguntas abiertas de la spec que afectan el contrato público o los mensajes de error, en particular la exposición de `Rechazos`.
2. Añadir primero pruebas xUnit por requisito, identificando cada test con el RF correspondiente; conservar intactos los 5 tests existentes.
3. Crear el record `Rechazo` y agregar `Devuelto` y `Rechazado` al enum, con XML docs en español.
4. Implementar el historial de rechazos y `Rechazar`, validando estado, turno y motivo antes de registrar el rechazo.
5. Implementar `Reenviar`, validando autor y estado antes de incrementar versión, volver a `EnCircuito` y reiniciar el turno mediante el cálculo por versión.
6. Corregir el cálculo de turno y finalización de `Aprobar` para que considere solo la versión vigente, preservando sus firmas públicas y el comportamiento existente.
7. Ejecutar `dotnet test`; verificar todos los RF, los 5 tests previos y los XML docs de los miembros públicos nuevos.
