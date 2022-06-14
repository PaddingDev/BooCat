using System;

namespace KevinZonda.Bookie.FunctionApp.Model;
internal class ErrModel
{
    public string Msg { get; set; }
    public string? Source { get; set; }

    public ErrModel(string msg, string? source = null)
    {
        Msg = msg;
        Source = source;
    }

    public static explicit operator ErrModel(Exception v)
    {
        return new ErrModel(v.Message, v.Source);
    }

    public static explicit operator ErrModel(string v)
    {
        return new ErrModel(v);
    }
}