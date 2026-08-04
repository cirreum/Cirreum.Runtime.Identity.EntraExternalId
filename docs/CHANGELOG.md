# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.10] - 2026-08-04

### Updated

- Updated NuGet packages (Cirreum spine 4.2.0 wave repins).

## [1.0.9] - 2026-07-29

### Updated

- Updated NuGet packages.

## [1.0.8] - 2026-07-24

### Fixed

- The README's provisioner walkthrough never showed the `IProvisionedIdentity.Claims`
  projection that mints what reaches the token. Since `Cirreum.IdentityProvider 2.0.0` stopped
  treating roles as a privileged concept, a `Roles` property on the app's own user type mints
  nothing until it is projected through `Claims` — the README now leads with that type.
  Documentation only; no API or behavior change.

### Updated

- Updated NuGet packages.

## [1.0.7] - 2026-07-23

### Updated

- Updated NuGet packages.

## [1.0.6] - 2026-07-20

### Updated

- Updated NuGet packages.

## [1.0.5] - 2026-07-19

### Updated

- Updated NuGet packages.

## [1.0.4] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.3] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.2] - 2026-05-01

### Updated
- Updated NuGet packages.

