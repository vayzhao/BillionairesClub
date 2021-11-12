using System;
using System.Collections.Generic;

/// <summary>
/// A class to store loading data, it is used when switching from one
/// scene to another. 
/// The class contains a list of partial class 'LoadMethod' which 
/// stores a delegate method that is going to be called while loading
/// </summary>
public class Loader
{
    private List<LoadMethod> methods;  // a list to contains all the method that is going to be called along loading
    
    public partial class LoadMethod
    {
        public float atPhase;          // time frame to invoke the loading method
        public delegate void Method(); // define how the loading method is going to be stored
        public Method method;          // the actual loading method

        public LoadMethod(float atPhase, Method method)
        {
            this.atPhase = atPhase;
            this.method = method;
        }
    }

    public Loader() => methods = new List<LoadMethod>();

    /// <summary>
    /// Method to register a loading method for this loader
    /// </summary>
    /// <param name="atPhase">time frame to invoke the loading method</param>
    /// <param name="method">the actual loading method</param>
    public void RegisterLoadingMethod(float atPhase, LoadMethod.Method method)
    {
        methods.Add(new LoadMethod(atPhase, method));
    }
    
    /// <summary>
    /// Method to check whether or not it is time to invoke the next loading method
    /// </summary>
    /// <param name="progress">current loading progress</param>
    /// <returns></returns>
    public bool IsReady(float progress)
    {
        if (methods.Count == 0)
            return false;
        return progress > methods[0].atPhase;
    }

    /// <summary>
    /// Method to invoke the first loading method for this loader and remove it from the 
    /// loader after being invoked
    /// </summary>
    public void Run()
    {
        methods[0].method.Invoke();
        methods.RemoveAt(0);
    }
}
