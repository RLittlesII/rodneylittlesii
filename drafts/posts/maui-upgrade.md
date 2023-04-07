Title: Xamarin to MAUI for a large Mvvm mobile application
Drafted: 01/09/2023
Published: 01/09/2023
Tags:
    - .NET
    - dotnet
    - Framework
    - MAUI
    - Xamarin
    - Mvvm
    - Prism
    - ReactiveUI
    - Open Source
    - Extensibility
Series:
    - Xamarin => MAUI
---

# Requirements

- [ ] Package Upgrades
    - [ ] Assess Packages that can upgrade
    - [ ] Assess Packages with no .net6 or higher version
    - [ ] Assess long term package strategy
    - [ ] Long term strategy for dotnet versions
        - [ ] [Docker Hosted Renovate](https://docs.renovatebot.com/getting-started/running/#docker-image)
            - [SelfHosting](https://docs.renovatebot.com/examples/self-hosting/)
            - [Docker Image](https://docs.renovatebot.com/examples/self-hosting/#docker)

- [ ] Package Deprecation
 - [ ] Identify packages we no longer consume

- [ ] Make Build Better
    - [ ] Add Coverage
        - [ ] [add code coverage](https://www.jetbrains.com/help/dotcover/Running_Coverage_Analysis_from_the_Command_LIne.html#basic)
    - [ ] Add [Code Cleanup](https://www.jetbrains.com/help/resharper/CleanupCode.html#configuring-cleanupcode-with-dotsettings)


- MAUI Upgrade Available
 - [ ] Check packages for `net6.0` or greater targets
 - If no package exists
    - [ ] determine if there is a comparable package
    - [ ] Forkable?
        - Effort?

- Migrate external dependencies
    - [ ] Pep.GeoFencing
    - [ ] Pep.SQLite-net
    - [ ] BrothersPrinterBindings
    - [ ] Honeywell Dex
    - [ ] Scandit
    - [ ] Unitech

# MAUI BRANCH

- Extended Libraries
    - [ ] Prism Navigation, Scoped Navigation
    - [ ] DryIoc, ScopeContext

# Links

- [docker-renovate](https://github.com/renovatebot/docker-renovate)
- [Getting Started](https://docs.renovatebot.com/getting-started/running/)
- [Self-Hosting Renovate](https://docs.renovatebot.com/getting-started/running/#self-hosting-renovate)
- [Self-Hosting Examples](https://docs.renovatebot.com/examples/self-hosting/)
- [Renovate on Azure Dev Ops](https://docs.renovatebot.com/modules/platform/azure/)
- [renovate-bot-azure-devops](https://github.com/MartinFaartoft/renovate-bot-azure-devops)
- [12 Self-Host Tips](https://jerrynsh.com/12-tips-to-self-host-renovate-bot/)
- [Renovate Bot Cheatsheet](https://www.augmentedmind.de/2021/07/25/renovate-bot-cheat-sheet/)
- [Success Stories](https://www.mend.io/customer-success-stories/)
- [Languages](https://www.mend.io/languages/)
- [Renovate Azure Devops](https://github.com/MartinFaartoft/renovate-bot-azure-devops)

