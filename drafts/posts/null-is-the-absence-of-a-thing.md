Title: Null, it's not there and it's your responsibility
Drafted: 07/22/2017
Published: 11/22/2022
Tags:
    - C#
    - Language
    - Nullability
---

# Null, the mistake we still have not learned

# What do I know?

I have turned on C# nullability in two of the popular MVVM frameworks for .NET, ReactiveUI and Prism (WIP).  Currently turning on nullabilty in a code base with almost one million lines of C#.

## Value Object vs Reference Object


Key points

- Handle `null` as if it where an exception
- Don't return `null` if you can return a default value
- `bool?` is not the new "3 way state"
- methods can accept null arguments and still guard against them being null
    - example is `Application.Current?` you don't expect it to be null, but it _can_ be null
- nullable default method parameters are fine internal to a system, but shouldn't cross "subsystem" boundaries
    - hint, changing them is a breaking API change
- Use the `MaybeNull` and like attributes!
- Encapsulate your nullability, don't force every consumer to check for null if you can guard against it for them!
- Don't act like it's not there.  This "see no evil" approach causes defects!
- C# 8 feature
    - Turning on the feature will help you appreciate how the absence of a thing™ can severly hurt your software
- Only Return Null When
    - Doing so won't kill your application ... yes ... I have seen devs do this
    - The application can recover from the null value being passed
    - You expect ever consumer of the method to gracefully handle a `null` return value
- The `default` value of a reference object is `null`


Tips for enabling
- File or Assembly at a time
- Start with either the data layer or the UI layer
- Small commits so you can easily walk backwards
    - ReactiveUI
    - Prism
- Turn on C# nullability warnings as errors
- Don't use Null Reference Exceptions as your catch all unhandled exception, it's not exceptional, you control it!
- Don't use nullable enums, rather default the enum with a `None` or `NA` or even `Default`.