## Summary

- What changed?
- Why is it needed?

## Commit Message

- Use a Conventional Commit prefix in the branch's commits:
  - `feat:` for new features
  - `fix:` for bug fixes
  - `refactor:`, `perf:`, `docs:`, `test:`, `ci:`, `build:`, `chore:`, `style:` for changes that should appear under `Changed`
  - `remove:`, `delete:`, `drop:` for removals
  - `sec:`, `security:` for security-related changes

## Verification

- [ ] `dotnet test DataverseGen.sln`
- [ ] Release workflow impact checked, if applicable

## Release Notes

- [ ] This change is ready to appear in Keep a Changelog sections
- [ ] Commit titles are specific enough to map cleanly to `Added`, `Changed`, `Fixed`, `Removed`, or `Security`
