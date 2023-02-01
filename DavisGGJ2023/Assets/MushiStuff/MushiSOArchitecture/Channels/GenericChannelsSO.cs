using System;
using UnityEngine;

// GENERATED
public abstract class GenericEventChannelSO : DescriptionBaseSO
{
    public event Action OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent()
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke();
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1> : DescriptionBaseSO
{
    public event Action<T1> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2> : DescriptionBaseSO
{
    public event Action<T1,T2> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2,T3> : DescriptionBaseSO
{
    public event Action<T1,T2,T3> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2,T3 arg3)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2,arg3);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2,T3,T4> : DescriptionBaseSO
{
    public event Action<T1,T2,T3,T4> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2,T3 arg3,T4 arg4)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2,arg3,arg4);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2,T3,T4,T5> : DescriptionBaseSO
{
    public event Action<T1,T2,T3,T4,T5> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2,T3 arg3,T4 arg4,T5 arg5)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2,arg3,arg4,arg5);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2,T3,T4,T5,T6> : DescriptionBaseSO
{
    public event Action<T1,T2,T3,T4,T5,T6> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2,arg3,arg4,arg5,arg6);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2,T3,T4,T5,T6,T7> : DescriptionBaseSO
{
    public event Action<T1,T2,T3,T4,T5,T6,T7> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6,T7 arg7)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2,arg3,arg4,arg5,arg6,arg7);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericEventChannelSO<T1,T2,T3,T4,T5,T6,T7,T8> : DescriptionBaseSO
{
    public event Action<T1,T2,T3,T4,T5,T6,T7,T8> OnRaised;
    public bool HasListeners => OnRaised != null;
    
    public virtual void RaiseEvent(T1 arg1,T2 arg2,T3 arg3,T4 arg4,T5 arg5,T6 arg6,T7 arg7,T8 arg8)
    {
        if (OnRaised != null)
        {
            OnRaised.Invoke(arg1,arg2,arg3,arg4,arg5,arg6,arg7,arg8);
        }
        else
        {
            NoListenerMessage();
        }
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Event Channel {name} was raised but had no listeners.");
    }
}


// GENERATED
public abstract class GenericFuncChannelSO<TResult> : DescriptionBaseSO
{
    public event Func<TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc()
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke();
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,TResult> : DescriptionBaseSO
{
    public event Func<T1,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,T3,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,T3,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2,T3 t3)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2,t3);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,T3,T4,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,T3,T4,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2,T3 t3,T4 t4)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2,t3,t4);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,T3,T4,T5,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,T3,T4,T5,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2,t3,t4,t5);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,T3,T4,T5,T6,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,T3,T4,T5,T6,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2,t3,t4,t5,t6);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,T3,T4,T5,T6,T7,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,T3,T4,T5,T6,T7,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2,t3,t4,t5,t6,t7);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}



// GENERATED
public abstract class GenericFuncChannelSO<T1,T2,T3,T4,T5,T6,T7,T8,TResult> : DescriptionBaseSO
{
    public event Func<T1,T2,T3,T4,T5,T6,T7,T8,TResult> OnCalled;
    public bool HasListeners => OnCalled != null;
    
    public virtual TResult CallFunc(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6,T7 t7,T8 t8)
    {
        if (OnCalled != null)
        {
            return OnCalled.Invoke(t1,t2,t3,t4,t5,t6,t7,t8);
        }
        else
        {
            NoListenerMessage();
            return DefaultReturn();
        }
    }
    
    protected virtual TResult DefaultReturn()
    {
        return default(TResult);   
    }
    
    protected virtual void NoListenerMessage()
    {
        Debug.LogWarning($"Func Channel {name} was raised but had no listeners.");
    }
}


