---
title: "INotifyPropertyChanged as an Observable"
published: 2022-09-22
drafted: 2022-09-22
draft: true
tags:
    - Events
    - Reactive Extensions
---

# Event based programming
[Event-driven programming](https://en.wikipedia.org/wiki/Event-driven_programming) is a reactionary, declarative model of programming where an application relies on events to happen for processing as a result of those events to take place.  This is the basis for a lot of GUI based applications. Frameworks have several choices when building extensions for consumers.  They can go with a commanding pattern "Give us your command and we will execute it for you when the framework sees fit".  Another approach is to emit an event that can be handled when something happens inside your framework.  This leaves how, when, and why to handle the event to the consumer and the platform doesn't have to execute anything by proxy for the consumer.  Events can be anything from a button clicked, to network connectivity is degraded.  A savvy MVVM developer learns when and where to delegate work to the framework and when to respond outside the bounds of the framework.  Responding to events is something that happens during the course of normal application programming.

[Event Driven Architecture](driven_architecture)

# Observable Streams

# UI Frameworks
- The reason auto generated extensions were originally needed

# ReactiveUI.Events
- Mono.Cecil
- Here there be dragons

# Pharmacist
- Next Gen using Cecil

# ReactiveMarbles Source Generators

# Anything that projects an event gets a wrapped observable