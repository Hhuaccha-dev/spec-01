namespace FirmaDocs;

/// <summary>Rechazo registrado por un firmante sobre una versión del documento.</summary>
/// <param name="Firmante">Quién rechazó.</param>
/// <param name="Motivo">Motivo indicado para el rechazo.</param>
/// <param name="Version">Versión del documento que rechazó.</param>
/// <param name="Instante">Momento del rechazo (inyectado, nunca el reloj del sistema).</param>
public record Rechazo(Firmante Firmante, string Motivo, int Version, DateTimeOffset Instante);
