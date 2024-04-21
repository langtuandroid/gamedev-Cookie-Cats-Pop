using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class FiberHelper
{
    public static IEnumerator RunParallel(params IEnumerator[] enums)
    {
        yield return FiberHelper.RunParallel((ICollection<IEnumerator>)enums);
        yield break;
    }

    public static IEnumerator RunParallel(ICollection<IEnumerator> enums)
    {
        List<Fiber> fibers = new List<Fiber>(enums.Count);
        foreach (IEnumerator main in enums)
        {
            fibers.Add(new Fiber(main, FiberBucket.Manual));
        }
        yield return new Fiber.OnExit(delegate ()
        {
            foreach (Fiber fiber2 in fibers)
            {
                fiber2.Terminate();
            }
        });
        bool finished = false;
        while (!finished)
        {
            finished = true;
            foreach (Fiber fiber in fibers)
            {
                if (fiber.Step())
                {
                    finished = false;
                }
            }
            yield return null;
        }
        yield break;
    }

    [Obsolete("Use RunParallel() instead. Phasing out confusing names.")]
    public static IEnumerator Compound(params IEnumerator[] enums)
    {
        return FiberHelper.RunParallel(enums);
    }

    public static IEnumerator RunSerial(params IEnumerator[] enums)
    {
        yield return FiberHelper.RunSerial((ICollection<IEnumerator>)enums);
        yield break;
    }

    public static IEnumerator RunSerial(ICollection<IEnumerator> enums)
    {
        List<Fiber> fibers = new List<Fiber>(enums.Count);
        foreach (IEnumerator main in enums)
        {
            fibers.Add(new Fiber(main, FiberBucket.Manual));
        }
        yield return new Fiber.OnExit(delegate ()
        {
            foreach (Fiber fiber2 in fibers)
            {
                fiber2.Terminate();
            }
        });
        foreach (Fiber fiber in fibers)
        {
            while (fiber.Step())
            {
                yield return null;
            }
        }
        yield break;
    }

    public static IEnumerator RunWhile(IEnumerator code, Func<bool> predicate)
    {
        Fiber fiber = new Fiber(code, FiberBucket.Manual);
        yield return new Fiber.OnExit(new Fiber.OnExitHandler(fiber.Terminate));
        while (predicate() && fiber.Step())
        {
            yield return null;
        }
        yield break;
    }

    public static IEnumerator RunDelayed(float delay, Action function)
    {
        yield return FiberHelper.Wait(delay, (FiberHelper.WaitFlag)0);
        function();
        yield break;
    }

    [Obsolete("Use RunSerial() instead. Phasing out confusing names.")]
    public static IEnumerator Combine(params IEnumerator[] enums)
    {
        return FiberHelper.RunSerial(enums);
    }

    public static IEnumerator WaitForRealtime(float duration, FiberHelper.WaitFlag flags = (FiberHelper.WaitFlag)0)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < duration)
        {
            if (flags == FiberHelper.WaitFlag.StopOnMouseDown && Input.GetMouseButton(0))
            {
                yield break;
            }
            yield return null;
        }
        yield break;
    }

    public static IEnumerator Wait(float duration, FiberHelper.WaitFlag flags = (FiberHelper.WaitFlag)0)
    {
        while (duration > 0f)
        {
            if (flags == FiberHelper.WaitFlag.StopOnMouseDown && Input.GetMouseButton(0))
            {
                yield break;
            }
            yield return null;
            duration -= Time.deltaTime;
        }
        yield break;
    }

    public static IEnumerator WaitForFrames(int frames, FiberHelper.WaitFlag flags = (FiberHelper.WaitFlag)0)
    {
        while (frames > 0)
        {
            if (flags == FiberHelper.WaitFlag.StopOnMouseDown && Input.GetMouseButton(0))
            {
                yield break;
            }
            yield return null;
            frames--;
        }
        yield break;
    }

    [Flags]
    public enum WaitFlag
    {
        StopOnMouseDown = 1
    }
}
