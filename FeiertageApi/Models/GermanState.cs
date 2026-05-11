namespace FeiertageApi.Models;

/// <summary>
/// Represents the 16 federal states (Bundesländer) of Germany.
/// Each state has a two-letter code used by the Feiertage API; the mapping lives in
/// <see cref="FeiertageApi.Extensions.GermanStateExtensions.ToStateCode"/>.
/// </summary>
public enum GermanState
{
    /// <summary>Baden-Württemberg (bw)</summary>
    BadenWuerttemberg,

    /// <summary>Bavaria / Bayern (by)</summary>
    Bavaria,

    /// <summary>Berlin (be)</summary>
    Berlin,

    /// <summary>Brandenburg (bb)</summary>
    Brandenburg,

    /// <summary>Bremen (hb)</summary>
    Bremen,

    /// <summary>Hamburg (hh)</summary>
    Hamburg,

    /// <summary>Hesse / Hessen (he)</summary>
    Hesse,

    /// <summary>Mecklenburg-Vorpommern (mv)</summary>
    MecklenburgVorpommern,

    /// <summary>Lower Saxony / Niedersachsen (ni)</summary>
    LowerSaxony,

    /// <summary>North Rhine-Westphalia / Nordrhein-Westfalen (nw)</summary>
    NorthRhineWestphalia,

    /// <summary>Rhineland-Palatinate / Rheinland-Pfalz (rp)</summary>
    RhinelandPalatinate,

    /// <summary>Saarland (sl)</summary>
    Saarland,

    /// <summary>Saxony / Sachsen (sn)</summary>
    Saxony,

    /// <summary>Saxony-Anhalt / Sachsen-Anhalt (st)</summary>
    SaxonyAnhalt,

    /// <summary>Schleswig-Holstein (sh)</summary>
    SchleswigHolstein,

    /// <summary>Thuringia / Thüringen (th)</summary>
    Thuringia
}