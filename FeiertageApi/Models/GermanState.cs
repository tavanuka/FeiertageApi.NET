using System.ComponentModel;

namespace FeiertageApi.Models;

/// <summary>
/// Represents the 16 federal states (Bundesländer) of Germany.
/// Each state has a two-letter code used by the Feiertage API.
/// </summary>
public enum GermanState
{
    /// <summary>
    /// Baden-Württemberg (bw)
    /// </summary>
    [Description("bw")]
    BadenWuerttemberg,

    /// <summary>
    /// Bavaria / Bayern (by)
    /// </summary>
    [Description("by")]
    Bavaria,

    /// <summary>
    /// Berlin (be)
    /// </summary>
    [Description("be")]
    Berlin,

    /// <summary>
    /// Brandenburg (bb)
    /// </summary>
    [Description("bb")]
    Brandenburg,

    /// <summary>
    /// Bremen (hb)
    /// </summary>
    [Description("hb")]
    Bremen,

    /// <summary>
    /// Hamburg (hh)
    /// </summary>
    [Description("hh")]
    Hamburg,

    /// <summary>
    /// Hesse / Hessen (he)
    /// </summary>
    [Description("he")]
    Hesse,

    /// <summary>
    /// Mecklenburg-Vorpommern (mv)
    /// </summary>
    [Description("mv")]
    MecklenburgVorpommern,

    /// <summary>
    /// Lower Saxony / Niedersachsen (ni)
    /// </summary>
    [Description("ni")]
    LowerSaxony,

    /// <summary>
    /// North Rhine-Westphalia / Nordrhein-Westfalen (nw)
    /// </summary>
    [Description("nw")]
    NorthRhineWestphalia,

    /// <summary>
    /// Rhineland-Palatinate / Rheinland-Pfalz (rp)
    /// </summary>
    [Description("rp")]
    RhinelandPalatinate,

    /// <summary>
    /// Saarland (sl)
    /// </summary>
    [Description("sl")]
    Saarland,

    /// <summary>
    /// Saxony / Sachsen (sn)
    /// </summary>
    [Description("sn")]
    Saxony,

    /// <summary>
    /// Saxony-Anhalt / Sachsen-Anhalt (st)
    /// </summary>
    [Description("st")]
    SaxonyAnhalt,

    /// <summary>
    /// Schleswig-Holstein (sh)
    /// </summary>
    [Description("sh")]
    SchleswigHolstein,

    /// <summary>
    /// Thuringia / Thüringen (th)
    /// </summary>
    [Description("th")]
    Thuringia
}
