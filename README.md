# rodneylittlesii
Personal blog of a developer trying to learn all the things

[Rodney Littles, II](https://rodneylittlesii.com)


[![github-license-badge]][github-license]

# Status
<!-- badges -->
[![Netlify Status](https://api.netlify.com/api/v1/badges/954e5bef-1eb2-4220-a9f2-2ea2a9473d90/deploy-status)](https://app.netlify.com/sites/rodneylittlesii/deploys)
<!-- badges -->

<!-- history badges -->
| GitHub Actions |
| -------------- |
| [![github-badge]][github] |
| [![github-history-badge]][github] |
<!-- history badges -->

[github-release]: https://github.com/rlittlesii/rodneylittlesii/releases/latest
[github-release-badge]: https://img.shields.io/github/release/rlittlesii/rodneylittlesii.svg?logo=github&style=flat "Latest Release"
[github-license]: https://github.com/rlittlesii/rodneylittlesii/blob/master/LICENSE
[github-license-badge]: https://img.shields.io/github/license/rlittlesii/rodneylittlesii.svg?style=flat "License"
[codecov]: https://codecov.io/gh/rlittlesii/rodneylittlesii
[codecov-badge]: https://img.shields.io/codecov/c/github/rlittlesii/rodneylittlesii.svg?color=E03997&label=codecov&logo=codecov&logoColor=E03997&style=flat "Code Coverage"

[github]: https://github.com/rlittlesii/rodneylittlesii/actions?query=workflow%3Apublish
[github-badge]: https://img.shields.io/github/workflow/status/rlittlesii/rodneylittlesii/publish.svg?label=github&logo=github&color=b845fc&logoColor=b845fc&style=flat "GitHub Actions Status"
[github-history-badge]: https://buildstats.info/github/chart/rlittlesii/rodneylittlesii?includeBuildsFromPullRequest=false "GitHub Actions History"

## Astro Migration (No Markdown Changes Required)

This repository now includes an Astro setup that renders the existing Wyam/Solid State markdown files without modifying them.

Quick start:

1. Install dependencies

```
npm i
```

2. Fetch Solid State assets directly into `public/` (no Wyam needed)

```
npm run theme:fetch
```

This downloads the minimal Solid State CSS/JS from the Wyam SolidState theme into `public/assets/**` so paths like `/assets/css/main.css` resolve in dev/build. You can override the source with an environment variable:

```
THEME_BASE=https://example.com/your-assets-base npm run theme:fetch
```

Alternative: you can also vendor assets manually by downloading Solid State from https://html5up.net/solid-state and placing the CSS/JS under `public/assets/**` to match the paths used in `src/layouts/Base.astro`.

3. Run the site

```
npm run dev
```

Open routes like:

- `/` — blog index (lists posts with a `Published` value)
- `/posts/topic/building-github-actions-with-nuke` — renders a post from `src/posts/**`
- `/tags/<tag>` — lists posts by tag
- `/about`, `/reading`, `/talks`, `/office-hours` — render single-file pages from `src/*.md`

Notes:

- A small Vite transform in `astro.config.mjs` virtually inserts the missing opening `---` so Wyam-style frontmatter is parsed correctly. No `.md` changes are needed.
- Posts are discovered from `src/posts/**/*.md`. Drafts can be excluded by omitting `Published`.
- Theme assets are served from `public/assets/**`. They are fetched via `npm run theme:fetch` and committed or re-fetched during CI as needed.