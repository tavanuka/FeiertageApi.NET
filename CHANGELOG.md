# FeiertageApi.NET Changelog

## [Unreleased]

### Added

- NuGet packaging metadata, Source Link, and symbol packages (`.snupkg`) for the core `FeiertageApi.NET` library.
- NuGet packaging for `FeiertageApi.NET.Extensions.AspNetCore` with multi-targeting for `net8.0`, `net9.0`, and `net10.0`.
- Shared MSBuild configuration (`Directory.Build.props` and `Directory.Build.targets`) for package metadata, README/license inclusion, and Source Link.
- GitHub Actions release workflow that publishes both packages to NuGet.org when a GitHub release is published.
- Draft-release automation in the build workflow that prepares a release on every push to `master`.
- Initial `CHANGELOG.md` following the Keep a Changelog format.

[Unreleased]: https://github.com/tavanuka/FeiertageApi.NET/commits/master