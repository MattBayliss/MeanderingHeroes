using Godot;
using LaYumba.Functional;
using MeanderingHeroes.Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using static LaYumba.Functional.F;

public partial class Hero : Node2D
{
    private record PositionUpdate(bool AtDestination, Vector2 NewPosition, Action AnimationAction);
    [NotNull]
    public Option<AnimatedSprite2D> Animator { get; private set; } = None;
    public Option<Vector2> Destination { get; private set; } = None;
    [Export]
    public float HeroSpeed { get; set; } = 50f;

    public override void _Ready()
    {
        Animator = GetChild<AnimatedSprite2D>(0);
    }
    public void SetDestination(Vector2 destination)
    {
        if(destination == Position)
        {
            Destination = None;
        }
        else
        {
            Destination = Some(destination);
            PlayWalking();
        }
    }
    public void PlayIdle() => Animator.Do(a => a.Play("default"));
    public void PlayWalking() => Animator.Do(a => a.Play("walk"));
    private static Func<Vector2, Func<Vector2, (Vector2 currentPosition, Vector2 newPosition)>> CalcNewPosition(float speed, float delta) =>
        currentPosition => destination =>
        {
            var vector = destination - currentPosition;
            vector = vector.LimitLength(speed * delta);

            return (currentPosition, newPosition: currentPosition + vector);
        };
    public override void _PhysicsProcess(double delta)
    {
        Destination
            .Map(CalcNewPosition(HeroSpeed, (float)delta)(Position))
            .Map(pp => pp switch
            {
                var (cp, np) when cp == np => new PositionUpdate(true, np, PlayIdle),
                var (_, np) => new PositionUpdate(false, np, () => { })
            }).Do(pu =>
            {
                if(pu.AtDestination)
                {
                    Destination = None;
                }
                Position = pu.NewPosition;
                pu.AnimationAction();
            });
    }
}
