using System;
using Keiwando.BigInteger;

public interface IHasHpUI
{
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public Action OnResetHPUI { get; set; }
    public Action<bool> OnActiveHpUI { get; set; }
}