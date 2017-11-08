# Asp.Net Core - Http Basic Example Project
[![Build Status](https://jenkins.dangl.me/buildStatus/icon?job=AspNetCoreBasicAuthentication.Tests)](https://jenkins.dangl.me/job/AspNetCoreBasicAuthentication.Tests/)

The purpose of this project is to demonstrate the implementation of Http Basic Authentication with Asp.Net Core 2.0s authentication model.
Additionally, unit tests demonstrate the functionality. The app is also enhanced with a custom authentication policy to support
multiple authentication handlers / authentication schemes easily. The implementation of the `HttpBasicAuthenticationHandler` can be
used as template to create custom authentication handlers.

The project supports authentication both Cookie based and via Http Basic Authentication. It's configured to use an `InMemory` database
with a default user with `User` / `Password` credentials.

## Configuration for Http Basic Authentication

The following points of interests are required to enable Http Basic Authentication in an Asp.Net Core 2.0+ app:

  1. A handler, see `Authentication/HttpBasicAuthenticationHandler` for the implementation
  2. Registration of the handler in `Startup`, see the `ConfigureServices` method
  3. Not required but nice to have is a `defaultPolicy` that allows multiple authentication schemes to be used in
  the app without manual specification which schemes to use per action. See [this blog post](https://blog.dangl.me/archive/use-any-authentication-handler-for-all-actions-in-aspnet-core-20/)
  for more details

## Unit Tests

The `AspNetCoreBasicAuthentication.Tests` contains tests that check that both Cookie Authentication and Http Basic Authentication work.
Tests can either be run directly from Visual Studio 2017 (just build the project) or via the `Tests.ps1` script in the project root.

---

[MIT License](LICENSE.md)
