using System.Collections.Generic;

public enum AnimationsBool
{
    run,
    walk,
    idle,
    none,
}

public struct AnimationController
{

    public AnimationsBool current;

    public List<AnimationsBool> nexts;

}