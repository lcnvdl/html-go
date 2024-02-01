# Checklist
1. Increase csproj versions.
2. Update changelog.
3. Remove `dist` directory.
4. Build using `publish.bat`.
5. Commit changes.
6. Go to: [https://github.com/lcnvdl/html-go/releases/new](https://github.com/lcnvdl/html-go/releases/new).
7. Create the tag for the new version. Example: `v0.5.0-alpha`.
8. Release title. Same as tag. Example: `v0.5.0-alpha`.
9. Click on `Generate release notes`.
10. Set title `## What's Changed`.
11. Also, copy and include the content of `Changelog.md`.
12. Go to `.\dist\client\portable\win-x64` and zip `htmlgo.exe` to `htmlgo-v0.5.0-alpha-win-x64.zip`.
13. Go to `.\dist\server\portable\win-x64` and zip all files to `htmlgo-server-v0.5.0-alpha-win-x64.zip`.
14. Attach the zip files.
15. Publish release.
