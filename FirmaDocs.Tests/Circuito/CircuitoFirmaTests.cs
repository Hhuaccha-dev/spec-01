using FirmaDocs;

namespace FirmaDocs.Tests.Circuito;

public class CircuitoFirmaTests
{
    private static readonly Firmante Ana = new("ana", "Ana Legal");
    private static readonly Firmante Luis = new("luis", "Luis Finanzas");
    private static readonly DateTimeOffset Ahora = new(2026, 11, 9, 10, 0, 0, TimeSpan.FromHours(-5));

    private static CircuitoFirma CircuitoEnviado()
    {
        var circuito = new CircuitoFirma("DOC-001", "marta", [Ana, Luis]);
        circuito.Enviar();
        return circuito;
    }

    [Fact]
    public void Aprobar_TodosEnOrden_DocumentoAprobado()
    {
        var circuito = CircuitoEnviado();

        circuito.Aprobar(Ana, Ahora);
        circuito.Aprobar(Luis, Ahora.AddHours(1));

        Assert.Equal(EstadoDocumento.Aprobado, circuito.Estado);
        Assert.Equal(2, circuito.Firmas.Count);
        Assert.Null(circuito.FirmanteEnTurno);
    }

    [Fact]
    public void Aprobar_PrimerFirmante_PasaElTurnoAlSiguiente()
    {
        var circuito = CircuitoEnviado();

        circuito.Aprobar(Ana, Ahora);

        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(Luis, circuito.FirmanteEnTurno);
        Assert.Equal(new Firma(Ana, 1, Ahora), circuito.Firmas[0]);
    }

    [Fact]
    public void Aprobar_FueraDeTurno_LanzaExcepcion()
    {
        var circuito = CircuitoEnviado();

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Aprobar(Luis, Ahora));

        Assert.Equal("No es el turno de Luis Finanzas.", ex.Message);
    }

    [Fact]
    public void Aprobar_SinEnviar_LanzaExcepcion()
    {
        var circuito = new CircuitoFirma("DOC-001", "marta", [Ana, Luis]);

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Aprobar(Ana, Ahora));

        Assert.Equal("El documento no está en circuito de firma.", ex.Message);
    }

    [Fact]
    public void Constructor_FirmanteRepetido_LanzaExcepcion()
    {
        var ex = Assert.Throws<CircuitoFirmaException>(() => new CircuitoFirma("DOC-001", "marta", [Ana, Ana]));

        Assert.Equal("Un firmante no puede repetirse en el circuito.", ex.Message);
    }

    [Fact]
    public void RF01_Rechazar_FirmanteEnTurno_RegistraRechazoYDevuelveDocumento()
    {
        var circuito = CircuitoEnviado();

        circuito.Rechazar(Ana, "Falta el anexo.", Ahora);

        Assert.Equal(EstadoDocumento.Devuelto, circuito.Estado);
        Assert.Equal(new Rechazo(Ana, "Falta el anexo.", 1, Ahora), Assert.Single(circuito.Rechazos));
    }

    [Fact]
    public void RF02_Reenviar_DocumentoDevuelto_IncrementaVersionYReiniciaTurno()
    {
        var circuito = CircuitoEnviado();
        circuito.Rechazar(Ana, "Corregir el anexo.", Ahora);

        circuito.Reenviar("marta");

        Assert.Equal(2, circuito.Version);
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(Ana, circuito.FirmanteEnTurno);
    }

    [Fact]
    public void RF03_Reenviar_ConservaFirmasPreviasSinContarlasEnVersionVigente()
    {
        var circuito = CircuitoEnviado();
        circuito.Aprobar(Ana, Ahora);
        circuito.Rechazar(Luis, "Corregir cifras.", Ahora.AddHours(1));
        circuito.Reenviar("marta");

        circuito.Aprobar(Ana, Ahora.AddDays(1));

        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
        Assert.Equal(Luis, circuito.FirmanteEnTurno);
        Assert.Equal(2, circuito.Firmas.Count);
        Assert.Equal(new[] { 1, 2 }, circuito.Firmas.Select(f => f.Version));

        circuito.Aprobar(Luis, Ahora.AddDays(1).AddHours(1));
        Assert.Equal(EstadoDocumento.Aprobado, circuito.Estado);
    }

    [Fact]
    public void RF04_Rechazar_EnTerceraVersion_FinalizaDocumentoYNoPermiteReenvio()
    {
        var circuito = CircuitoEnviado();
        circuito.Rechazar(Ana, "Primera corrección.", Ahora);
        circuito.Reenviar("marta");
        circuito.Rechazar(Ana, "Segunda corrección.", Ahora.AddDays(1));
        circuito.Reenviar("marta");

        circuito.Rechazar(Ana, "Tercera corrección.", Ahora.AddDays(2));

        Assert.Equal(EstadoDocumento.Rechazado, circuito.Estado);
        Assert.Equal(3, Assert.Single(circuito.Rechazos, r => r.Version == 3).Version);
        Assert.Throws<CircuitoFirmaException>(() => circuito.Reenviar("marta"));
    }

    [Fact]
    public void RF05_Rechazar_MotivoVacioOLlenoDeEspacios_LanzaExcepcion()
    {
        var circuito = CircuitoEnviado();

        var ex = Assert.Throws<CircuitoFirmaException>(() => circuito.Rechazar(Ana, "   ", Ahora));

        Assert.Equal("El motivo de rechazo es obligatorio.", ex.Message);
        Assert.Empty(circuito.Rechazos);
        Assert.Equal(EstadoDocumento.EnCircuito, circuito.Estado);
    }
}
