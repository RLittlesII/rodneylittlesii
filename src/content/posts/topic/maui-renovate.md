---
title: "Add Renovate for long term dotnet support"
published: 2023-01-09
drafted: 2023-01-09
draft: true
tags:
    - .NET
    - Extensibility
    - dotnet
    - MAUI
    - Open Source
    - Renovate
    - Xamarin
---

# Reasons

## The Science

To me an aspect of the science in what we do as developers is being able to test each variable in isolation and verify it's effect on the whole.  Some might argue this is the crux of root cause analysis is being able to pint point the exact line of code that is causing the problem.  So scaling that at the application level.  I want to apply that same science to each dependency my application runs on.  This can be something you do from the start, but the sooner you start the better. The important thing is being able to ensure which dependency upgrades might have risk associated by isolating their entry to the main line.  This also makes it easy to rollback and assess

## Changing Landscape

With the migration to net6.0, we will find ourselves in a new world as dotnet mobile developers.  We will have to adhere to dotnet's new support cycles.  The cadence currently is a new Long Term Support(LTS) version every two years.  This means that this process will either be repeated every two years in a painful rush to ensure we maintain support, or a inceremental push towards the goal.

## Automation of the task™

Years ago when I first started depending on nuget.org it would take one developer a two week sprint to keep the packages up to date.  Today we can automate the task.

1. Checking the dependency needs a version bump
2. Creating a Pull Request against the system
3. Allow CI to give a Red/Green as to the level of effort involved in the change

This allows us to have background tasks for developers to pick up if they are waiting on story definition or a pull request to finish.

# Pros

- Automated
- Future Proof
- [Success Stories](https://www.mend.io/customer-success-stories/)
- [Languages](https://www.mend.io/languages/)

#### Cons

- Allow access to code via token
- Manual Hosting

# Alternatives

### dotnet outdated

#### Pros

- Produces json
- Allows version descrimination (major vs minor)

#### Cons

- Manual execution
- Manual interpretation
- Manual Change
- Manual Pull Request
- Could build a custom tool around it which would take time

### Excel

- Full Manual Tracking of the entire process

<https://docs.renovatebot.com/dependency-pinning/#grouping-related-packages>

# The Process

### Renovate Does

- Opens Pull Requests
- Manages Specific Configuration when issues are encountered
  - Pinning
- Manages Dependency Updates

### Developer Does

- Reviews the Pull Request
- Pulls the Target Branch
- Runs Developer Smoke Test

### Tester Does

- Executes Test Cases
  - Smoke
  - Automated
  - Business

### Project Manager Does
- 

## In Case of Emergency Revert

The sell to this, each dependency is updated in isolation.  So once you find the culprit during test case execution, you can just revert the Pull Request and triage each dependency in isolation.