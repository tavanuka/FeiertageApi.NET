# FeiertageApi.NET Changelog

## [Unreleased]

### Added
- Smoke tests against the real API
- More tests to improve code coverage and robustness
- Source generation for the Feiertage JSON context and converters

### Fixed
- Base URL's DNS could not be resolved
- Tests for the client were flaky and not representing the real API changes

### Changed
- Add more leniency for parsing 'status' values for 'HolidayResponse'
- Refactor `HolidayResponse` JSON converter to be more maintainable
- Refactor Exception definitions to be more consistent and protect against invocation outside of the library
- Drop `Moq` utilisation for single-purpose stubs and test harness
- (Extensions.AspNetCore): Explicitly create a typed client for the API
- Adjust certain constructor visibility to be more consistent with the library's design

## [0.1.1] - 2026-05-11

### Changed
- Introduce centralized package management
- Remove unnecessary package dependencies from the core library

### Fixed
- `FeiertageApi.Extensions.AspNetCore` did not contain any code. The package should have the correct extensions and dependencies now.


## [0.1.0] - 2026-05-11

### Added

- Initial release publication
- Semantic versioning using [SemVer](https://semver.org/). #14
- Continuous integration using GitHub Actions for release automation. #13
- Initial `CHANGELOG.md` following the Keep a Changelog format.


[Unreleased]: https://github.com/tavanuka/FeiertageApi.NET/compare/0.1.1...HEAD
[0.1.1]: https://github.com/tavanuka/FeiertageApi.NET/compare/0.1.0...0.1.1
[0.1.0]: https://github.com/tavanuka/FeiertageApi.NET/releases/tag/0.1.0
