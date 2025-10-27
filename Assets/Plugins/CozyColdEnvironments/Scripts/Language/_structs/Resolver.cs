using CommunityToolkit.Diagnostics;
using System;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable S3236
#pragma warning disable S3400
#pragma warning disable IDE1006
namespace CCEnvs.Language
{
    public readonly struct Resolver
    {
        private readonly static Func<bool> @true = () => true;
        private readonly static Func<bool> @false = () => false;

        private readonly Func<bool> @if;

        public Resolver(Func<bool> @if)
        {
            this.@if = @if;
        }

        public Resolver(bool @if)
        {
            if (@if)
                this.@if = @true;
            else
                this.@if = @false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static implicit operator Resolver(bool source)
        {
            return new Resolver(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Resolver(Func<bool> source)
        {
            return new Resolver(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator bool(Resolver source)
        {
            return source.@if?.Invoke() ?? false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Resolver If(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (@if())
            {
                action();
                return true;
            }

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TOut> If<TOut>(Func<TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (@if())
                return selector();

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Resolver Else(Action action)
        {
            Guard.IsNotNull(action, nameof(action));

            if (!@if())
            {
                action();
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TOut> Else<TOut>(Func<TOut> selector)
        {
            Guard.IsNotNull(selector, nameof(selector));

            if (!@if())
                return selector();

            return default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Resolver Match(Action onIf, Action onElse)
        {
            Guard.IsNotNull(onIf, nameof(onIf));
            Guard.IsNotNull(onElse, nameof(onElse));

            if (@if())
            {
                onIf();
                return true;
            }
            else
            {
                onElse();
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Maybe<TOut> Match<TOut>(Func<TOut> onIf, Func<TOut> onElse)
        {
            Guard.IsNotNull(onIf, nameof(onIf));
            Guard.IsNotNull(onElse, nameof(onElse));

            if (@if())
                return onIf();
            else
                return onElse();
        }
    }
}
