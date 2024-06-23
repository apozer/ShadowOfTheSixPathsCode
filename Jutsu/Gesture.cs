/*namespace Jutsu
{ 
    using System;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

using NamedConditionSet = System.Tuple<string, System.Func<bool>[]>;
using NamedCondition = System.Tuple<string, System.Func<bool>>;

/// <summary>
/// A struct representing a target position for a hand.
/// </summary>
/// <param name="Position">Target position</param>
/// <param name="Rotation">Target rotation</param>
/// <param name="Gripping">Should the hand be gripping?</param>
/// <param name="Triggering">Should the hand be triggering?</param>
/// <param name="Radius">Maximum tolerance for the position of the hand</param>
public record struct GestureTarget(Vector3 Position, Quaternion Rotation, bool Gripping, bool Triggering, float Radius);

/// <summary>
/// A gesture on one or both hands.
/// </summary>
public class Gesture {
    /// <summary>
    /// Set the handedness of the tester. If you change this to Side.Left,
    /// it will swap which hands Gesture.Left or Gesture.Right point to.
    /// </summary>
    public static Side handedness = Side.Right;
    /// <summary>
    /// Either hand, or both.
    /// </summary>
    public enum HandSide {
        Left,
        Right,
        Both
    }

    protected Func<HandSide> side;
    protected HandSide GetSide => side();
    protected Direction palm = Direction.Any;
    protected Direction thumb = Direction.Any;
    protected Direction point = Direction.Any;
    protected (Direction direction, float speed) moving;
    protected Position position;
    protected bool still = false;
    protected bool? gripping = null;
    protected bool? triggering = null;
    protected List<(Direction direction, float amount)> offsets;

    /// <summary>
    /// Create a gesture on a particular side.
    /// </summary>
    public Gesture(HandSide side) { this.side = () => side; }
    
    /// <summary>
    /// Create a gesture on a side that is dynamically determined at runtime.
    /// </summary>
    /// <example>
    /// <code>
    /// var gesture = new Gesture(() => item.mainHandler.side);
    /// </code>
    /// </example>
    public Gesture(Func<HandSide> sideFunc) { side = sideFunc; }

    /// <summary>
    /// Take in a Side and switch it depending on Gesture.handedness.
    /// </summary>
    public static Side FixHandedness(Side side) => handedness == Side.Right
        ? side
        : side switch {
            Side.Left => Side.Right,
            Side.Right => Side.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
        
    /// <summary>
    /// Take in a HandSide and switch it depending on Gesture.handedness.
    /// </summary>
    public static HandSide FixHandedness(HandSide side) => handedness == Side.Right
        ? side
        : side switch {
            HandSide.Left => HandSide.Right,
            HandSide.Right => HandSide.Left,
            HandSide.Both => HandSide.Both,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

    /// <summary>
    /// A gesture on the left hand.
    /// </summary>
    public static Gesture Left => new(FixHandedness(HandSide.Left));
    
    /// <summary>
    /// A gesture on the right hand.
    /// </summary>
    public static Gesture Right => new(FixHandedness(HandSide.Right));
    
    /// <summary>
    /// A gesture on both hands.
    /// </summary>
    public static Gesture Both => new(HandSide.Both);
    
    /// <summary>
    /// A gesture on a particular side.
    /// </summary>
    public static Gesture OnSide(Side side)
        => new(FixHandedness(side == Side.Left ? HandSide.Left : HandSide.Right));

    /// <summary>
    /// Test whether the palm is facing a direction.
    /// </summary>
    public Gesture Palm(Direction direction) {
        palm = direction;
        return this;
    }

    /// <summary>
    /// Test whether the thumb is pointing in a direction.
    /// </summary>
    public Gesture Thumb(Direction direction) {
        thumb = direction;
        return this;
    }

    /// <summary>
    /// Test whether the hand is pointing in a direction.
    /// </summary>
    public Gesture Point(Direction direction) {
        point = direction;
        return this;
    }

    /// <summary>
    /// Test whether the hand is moving in a direction.
    /// </summary>
    public Gesture Moving(Direction direction, float velocity = 3) {
        moving = (direction, velocity);
        return this;
    }

    /// <summary>
    /// Test whether the hand is gripping.
    /// </summary>
    public Gesture Gripping {
        get {
            gripping = true;
            return this;
        }
    }
    
    /// <summary>
    /// Test whether the hand is triggering.
    /// </summary>
    public Gesture Triggering {
        get {
            triggering = true;
            return this;
        }
    }

    /// <summary>
    /// Test whether the hand is open (not gripping or triggering).
    /// </summary>
    public Gesture Open {
        get {
            gripping = false;
            triggering = false;
            return this;
        }
    }

    /// <summary>
    /// Test whether the hand is making a fist (gripping and triggering).
    /// </summary>
    public Gesture Fist {
        get {
            gripping = true;
            triggering = true;
            return this;
        }
    }

    /// <summary>
    /// Test whether the hand is still (not moving).
    /// </summary>
    public Gesture Still {
        get {
            still = true;
            return this;
        }
    }

    /// <summary>
    /// Test whether the hand is at a position relative to the player's body.
    /// </summary>
    public Gesture At(Position position) {
        this.position = position;
        return this;
    }

    /// <summary>
    /// If using .At(), offset the target position in a direction by an amount.
    /// </summary>
    public Gesture Offset(Direction direction, float amount = 0.2f) {
        offsets ??= new List<(Direction direction, float amount)>();
        offsets.Add((direction, amount));
        return this;
    }

    protected bool ForValidHands(Func<RagdollHand, bool> func) {
        return GetSide switch {
            HandSide.Both => func(Player.currentCreature.handLeft) && func(Player.currentCreature.handRight),
            HandSide.Left => func(Player.currentCreature.handLeft),
            HandSide.Right => func(Player.currentCreature.handRight),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected Vector3 CalculateOffset(Side side) {
        if (offsets == null) return Vector3.zero;
        var amount = Vector3.zero;
        for (var i = 0; i < offsets.Count; i++) {
            amount += ToViewDir(side, offsets[i].direction).ToWorldViewPlaneVector() * offsets[i].amount;
        }

        return amount;
    }

    protected (Vector3 position, float radius) GetPosition(Side side) => position switch {
        Position.Waist => (
            Player.currentCreature.ragdoll.rootPart.transform.position
            + new Vector3(side == Side.Right ? 0.25f : -0.25f, 0.1f, 0.3f).LocalToViewPlaneSpace(), 0.4f),
        Position.Face => (
            Player.local.head.transform.position
            + new Vector3(side == Side.Right ? 0.1f : -0.1f, 0, 0.1f).LocalToViewPlaneSpace(), 0.2f),
        _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
    };

    protected Quaternion GetRotation(Side side, Vector3 position) {
        Vector3 forward = default, up = default;
        float sideMult = side == Side.Left ? -1 : 1;
        if (point != Direction.Any) {
            forward = ToViewDir(side, point).ToWorldViewPlaneVector();
        } else if (palm != Direction.Any && thumb != Direction.Any) {
            forward = Vector3.Cross(ToViewDir(side, thumb).ToWorldViewPlaneVector(),
                sideMult * ToViewDir(side, palm).ToWorldViewPlaneVector());
        }

        if (thumb != Direction.Any) {
            up = ToViewDir(side, thumb).ToWorldViewPlaneVector();
        } else if (palm != Direction.Any && point != Direction.Any) {
            up = Vector3.Cross(sideMult * ToViewDir(side, palm).ToWorldViewPlaneVector(),
                ToViewDir(side, point).ToWorldViewPlaneVector());
        }

        if (palm != Direction.Any) {
            var palmDir = ToViewDir(side, palm).ToWorldViewPlaneVector();
            if (forward == default || up == default) {
                if (forward != default) {
                    up = Vector3.Cross(forward, palmDir) * sideMult;
                } else if (up != default) {
                    forward = Vector3.Cross(up, palmDir) * sideMult;
                } else {
                    var viewForward = ViewDir.Forward.ToWorldViewPlaneVector();
                    var viewUp = ViewDir.Up.ToWorldViewPlaneVector();
                    (forward, up) = palm switch {
                        Direction.Up or Direction.Down => (viewForward,
                            Vector3.Cross(palmDir, viewForward) * sideMult),
                        _ => (viewUp, Vector3.Cross(viewUp, palmDir) * sideMult)
                    };
                }
            }
        }

        if (forward != default && up != default) {
            return Quaternion.LookRotation(forward, up);
        }

        if (forward != default) {
            return Quaternion.LookRotation(forward);
        }

        if (up != default) {
            return Quaternion.LookRotation(
                Vector3.Cross(up,
                    Vector3.Cross(position - Player.currentCreature.ragdoll.rootPart.transform.position, up)), up);
        }

        return Quaternion.LookRotation(position - Player.currentCreature.ragdoll.rootPart.transform.position);
    }

    /// <summary>
    /// Returns a GestureTarget that can show where the hand needs to be located.
    /// </summary>
    public GestureTarget? GetTarget(Side side) {
        if (ToSide(GetSide) == side || GetSide == HandSide.Both) {
            (var pos, float rad) = GetPosition(side);
            var offsetPosition = pos + CalculateOffset(side);
            var rotation = GetRotation(side, offsetPosition);
            return new GestureTarget(offsetPosition, rotation, gripping ?? false, triggering ?? false, rad);
        }

        return null;
    }

    /// <summary>
    /// Turn a HandSide into a Side. Returns null for HandSide.Both.
    /// </summary>
    public static Side? ToSide(HandSide side) => side switch {
        HandSide.Both => null,
        HandSide.Left => Side.Left,
        HandSide.Right => Side.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };

    /// <summary>
    /// Turn a Side into a HandSide.
    /// </summary>
    public static HandSide ToHandSide(Side side) => side switch {
        Side.Left => HandSide.Left,
        Side.Right => HandSide.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };

    protected bool IsAtPosition(Vector3 handPos, Side side) {
        (var target, float radius) = GetPosition(side);
        return (handPos - (CalculateOffset(side) + target)).sqrMagnitude < radius * radius;
    }

    protected Func<bool> Tester => () => ForValidHands(hand
        => (position == Position.Any || IsAtPosition(hand.Palm(), hand.side))
           && (!still || hand.Velocity().magnitude < 0.5f)
           && (gripping == null || hand.Gripping() == gripping)
           && (triggering == null || hand.Triggering() == triggering)
           && (moving.direction == Direction.Any
               || hand.LocalVelocity().WorldToViewPlaneSpace()
                   .InDirection(ToViewDir(hand.side, moving.direction), moving.speed))
           && (palm == Direction.Any
               || hand.PalmDir.WorldToViewPlaneSpace().InDirection(ToViewDir(hand.side, palm)))
           && (thumb == Direction.Any
               || hand.ThumbDir.WorldToViewPlaneSpace().InDirection(ToViewDir(hand.side, thumb)))
           && (point == Direction.Any
               || hand.PointDir.WorldToViewPlaneSpace().InDirection(ToViewDir(hand.side, point)))
    );

    /// <summary>
    /// Take a Side and Direction and return a ViewDir (player view-aligned direction)
    /// </summary>
    protected static ViewDir ToViewDir(Side side, Direction direction) {
        return direction switch {
            Direction.Backward => ViewDir.Back,
            Direction.Forward => ViewDir.Forward,
            Direction.Inwards => side == Side.Left ? ViewDir.Right : ViewDir.Left,
            Direction.Outwards => side == Side.Left ? ViewDir.Left : ViewDir.Right,
            Direction.Up => ViewDir.Up,
            Direction.Down => ViewDir.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    /// <summary>
    /// Get a human-readable description of the gesture.
    /// </summary>
    public string Description {
        get {
            return ", ".Join(GetSide switch {
                    HandSide.Both => "Both hands",
                    HandSide.Left => "Left hand",
                    HandSide.Right => "Right hand",
                    _ => throw new ArgumentOutOfRangeException()
                },
                position switch {
                    Position.Any => null,
                    Position.Face => "beside face",
                    Position.Waist => "beside waist",
                    _ => throw new ArgumentOutOfRangeException()
                },
                offsets is { Count: > 0 }
                    ? $"offset [ {string.Join(", ", offsets.Select(offset => $"{offset.direction.ToString().ToLower()} by {offset.amount}m"))} ]"
                    : null,
                DirectionString("palm", palm),
                DirectionString("thumb", thumb),
                DirectionString("point", point),
                DirectionString("moving", moving.direction), still ? "still" : null);
        }
    }

    protected string DirectionString(string prefix, Direction direction) => direction switch {
        Direction.Any => null,
        _ => prefix + " " + direction.ToString().ToLower()
    };

    /// <summary>
    /// Test whether the gesture is being performed.
    /// </summary>
    public bool Test() => Tester();

    /// <summary>
    /// Test the gesture on a particular hand.
    /// </summary>
    public bool TestOn(Side side) => TestOn(ToHandSide(side));
        
    
    /// <summary>
    /// Test the gesture on a particular hand.
    /// </summary>
    public bool TestOn(HandSide side) {
        var oldSide = this.side;
        this.side = () => side;
        bool value = Test();
        this.side = oldSide;
        return value;
    }
    
    /// <summary>
    /// Test whether the gesture is not being performed.
    /// </summary>
    public bool Not() => !Test();

    public static implicit operator NamedCondition(Gesture gesture) => new NamedCondition(gesture.Description, gesture.Tester);
    public static implicit operator GestureStep(Gesture gesture) => new GestureStep(gesture);
        
    /// <summary>
    /// Add another gesture requirement to this gesture.
    /// </summary>
    public GestureStep And(Gesture gesture) => new GestureStep(this).And(gesture);
    
    /// <summary>
    /// Add another gesture requirement to this gesture.
    /// </summary>
    public GestureStep And(GestureStep step) => new GestureStep(this).And(step);
}

/// <summary>
/// A step in a sequence of gestures. Can contain one or multiple gestures.
/// </summary>
/// <remarks>
/// You should not need to use this, the engine will convert between Gesture and GestureStep seamlessly.
/// </remarks>
public class GestureStep {
    public List<Gesture> gestures;
    public GestureStep(params Gesture[] gestures) {
        this.gestures = gestures.ToList();
    }

    /// <summary>
    /// Get a human-readable description of the gesture.
    /// </summary>
    public static string ToSentence(string input) {
        if (input.Length < 1)
            return input;

        string sentence = input.ToLower();
        return sentence[0].ToString().ToUpper() + sentence.Substring(1);
    }

    /// <summary>
    /// Convert to a type understandable by the Sequence Tracker.
    /// </summary>
    public static implicit operator NamedCondition(GestureStep step) {
        return new NamedCondition(
            ToSentence(string.Join(" and ", step.gestures.Select(gesture => gesture.Description.ToLower()))),
            () => step.Tester());
    }

    /// <summary>
    /// Get the GestureTarget for a hand.
    /// </summary>
    public GestureTarget? GetTargetForHand(Side side) {
        GestureTarget? target = null;
        for (var i = 0; i < gestures.Count; i++) {
            if (gestures[i].GetTarget(side) is GestureTarget eachTarget) {
                target = eachTarget;
            }
        }

        return target;
    }

    private static readonly int Flex = Animator.StringToHash("Flex");

    /// <summary>
    /// Orient two GameObjects to the positions of where the hands should be.
    /// </summary>
    /// <remarks>
    /// Deactivates the object if the hand is not used in the gesture.
    /// </remarks>
    public void UpdateTargets(GameObject handLeft, GameObject handRight) {
        if (GetTargetForHand(Side.Left) is GestureTarget left) {
            handLeft.GetComponentInChildren<Animator>()?.SetFloat(Flex,
                Mathf.Lerp(handLeft.GetComponentInChildren<Animator>().GetFloat(Flex), left.Gripping ? 1 : 0,
                    Time.deltaTime * 10));
            if (!handLeft.activeSelf) {
                handLeft.SetActive(true);
                handLeft.transform.SetPositionAndRotation(left.Position, left.Rotation);
            } else {
                handLeft.transform.SetPositionAndRotation(
                    Vector3.Lerp(handLeft.transform.position, left.Position, Time.deltaTime * 10),
                    Quaternion.Slerp(handLeft.transform.rotation, left.Rotation, Time.deltaTime * 10));
            }
        } else {
            handLeft.SetActive(false);
        }

        if (GetTargetForHand(Side.Right) is GestureTarget right) {
            handRight.GetComponentInChildren<Animator>()?.SetFloat(Flex,
                Mathf.Lerp(handRight.GetComponentInChildren<Animator>().GetFloat(Flex), right.Gripping ? 1 : 0,
                    Time.deltaTime * 10));
            if (!handRight.activeSelf) {
                handRight.SetActive(true);
                handRight.transform.SetPositionAndRotation(right.Position, right.Rotation);
            } else {
                handRight.transform.SetPositionAndRotation(
                    Vector3.Lerp(handRight.transform.position, right.Position, Time.deltaTime * 10),
                    Quaternion.Slerp(handRight.transform.rotation, right.Rotation, Time.deltaTime * 10));
            }
        } else {
            handRight.SetActive(false);
        }
    }

    /// <summary>
    /// Add another gesture requirement to this gesture.
    /// </summary>
    public GestureStep And(Gesture gesture) {
        gestures.Add(gesture);
        return this;
    }

    /// <summary>
    /// Add another gesture requirement to this gesture.
    /// </summary>
    public GestureStep And(GestureStep step) {
        gestures.AddRange(step.gestures);
        return this;
    }

    /// <summary>
    /// Test all the gestures in this step.
    /// </summary>
    public Func<bool> Tester => () => gestures.All(gesture => gesture.Test());
        
    /// <summary>
    /// Return a SequenceTracker step testing the inverse of this gesture.
    /// </summary>
    public NamedCondition Not() {
        return new NamedCondition(
            ToSentence("NOT " + string.Join(" and ", gestures.Select(gesture => gesture.Description.ToLower()))),
            () => !Tester());
    }
}

/// <summary>
/// A player-aligned direction. Inwards is to the right for the left hand and to the left from the right,
/// and vice versa for outwards.
/// </summary>
public enum Direction {
    Any,
    Forward,
    Backward,
    Inwards,
    Outwards,
    Up,
    Down
}

public enum Position {
    Any,
    Waist,
    Face
}
}*/