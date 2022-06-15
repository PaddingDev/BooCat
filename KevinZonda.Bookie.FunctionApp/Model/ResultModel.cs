using KevinZonda.Bookie.Library.Models;

using System;

namespace KevinZonda.Bookie.FunctionApp.Model;

internal sealed class ResultModel
{
    public bool Success { get; set; }
    public BookInfo[]? Books { get; set; }
    public ErrModel? Err { get; set; }

    public static explicit operator ResultModel(ErrModel v)
    {
        return new ResultModel()
        {
            Success = false,
            Err = v
        };
    }

    public static explicit operator ResultModel(Exception v)
    {
        return (ResultModel)(ErrModel)v;
    }

    public static explicit operator ResultModel(BookInfo[] v)
    {
        return new ResultModel()
        {
            Success = true,
            Books = v
        };
    }
}
