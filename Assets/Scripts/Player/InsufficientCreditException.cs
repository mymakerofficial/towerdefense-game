using System;

public class InsufficientCreditException : Exception
{
    /// <summary>
    /// Credits Needed for operation 
    /// </summary>
    public long CreditNeeded { get; }
    /// <summary>
    /// given Credits 
    /// </summary>
    public long CreditProvided { get; }
    

    public InsufficientCreditException(string message,Exception innerException, long creditNeeded, long creditProvided) : base(message,innerException)
    {
        this.CreditNeeded = creditNeeded;
        this.CreditProvided = creditProvided;
    }
    public InsufficientCreditException(string message, long creditNeeded, long creditProvided) : base(message)
    {
        this.CreditNeeded = creditNeeded;
        this.CreditProvided = creditProvided;
    }
    
    public InsufficientCreditException(long creditNeeded, long creditProvided)
    {
        this.CreditNeeded = creditNeeded;
        this.CreditProvided = creditProvided;
    }
}