# Amusoft.Toolkit.Mvvm

## Features

- Generates DataTemplates at runtime to bind a VM to a View
- Provides navigation functionality using INavigationService in a viewmodel based way
- Provides navigation hooks to respond to displaying a view or attempting to leave a view

## How to use it?

- Similar to Prism this package provides a RegionManager to register a view as a region that can be interacted with by
	using the INavigationService.
- The methods are pretty self explanatory and i recommend you have a look at the samples [here](./samples/TestApp.Wpf/)

## Why another MVVM framework?

- For a project i had to work with Prism, which is just a pure pain in the ass to work with to have it work the way you
	want it to work. Not just that, but it just silently fails if you do anything at all in a way the framework does not
	want you to do it

## Project state

[![.GitHub](https://github.com/taori/Amusoft.Toolkit.Mvvm/actions/workflows/CI.yml/badge.svg)](https://github.com/taori/Amusoft.Toolkit.Mvvm/actions/workflows/CI.yml)
[![GitHub issues](https://img.shields.io/github/issues/taori/Amusoft.Toolkit.Mvvm)](https://github.com/taori/Amusoft.Toolkit.Mvvm/issues)

| Package | Release | Prerelease                                                                                                                                                                                  |
|---------|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Amusoft.Toolkit.Mvvm.Core        | [![NuGet version (Amusoft.Toolkit.Mvvm.Core)](https://img.shields.io/nuget/v/Amusoft.Toolkit.Mvvm.Core.svg)](https://www.nuget.org/packages/Amusoft.Toolkit.Mvvm.Core/)        | [![NuGet version (Amusoft.Toolkit.Mvvm.Core)](https://img.shields.io/nuget/vpre/Amusoft.Toolkit.Mvvm.Core.svg)](https://www.nuget.org/packages/Amusoft.Toolkit.Mvvm.Core/latest/prerelease) |
| Amusoft.Toolkit.Mvvm.Wpf        | [![NuGet version (Amusoft.Toolkit.Mvvm.Wpf)](https://img.shields.io/nuget/v/Amusoft.Toolkit.Mvvm.Wpf.svg)](https://www.nuget.org/packages/Amusoft.Toolkit.Mvvm.Wpf/)        | [![NuGet version (Amusoft.Toolkit.Mvvm.Wpf)](https://img.shields.io/nuget/vpre/Amusoft.Toolkit.Mvvm.Wpf.svg)](https://www.nuget.org/packages/Amusoft.Toolkit.Mvvm.Wpf/latest/prerelease)  |


<!--CoverageStart-->
![Code Coverage](https://img.shields.io/badge/Code%20Coverage-100%25-success?style=flat)

Package | Line Rate | Branch Rate | Health
-------- | --------- | ----------- | ------
Amusoft.Toolkit.Mvvm.Core | 100% | 92% | ✔
Amusoft.Toolkit.Mvvm.Wpf | 100% | 85% | ✔
**Summary** | **100%** (356 / 356) | **91%** (113 / 124) | ✔

_Minimum allowed line rate is 80%_

[Coverage details]()
<!--CoverageEnd-->

## Description

This template was generated using [Amusoft.Templates](https://www.github.com/taori/Amusoft.Templates)