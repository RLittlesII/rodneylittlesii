Title: Null, it's not there and it's your responsibility
Drafted: 07/22/2017
Published: 11/22/2022
Tags:
    - C#
    - Language
    - Nullability
---

# Null, the mistake we still have not learned


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
    - Doing so won't kill your application ... yes ... I have seen devs do this.