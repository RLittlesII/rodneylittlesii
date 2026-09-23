# rodneylittlesii

Personal blog of a developer trying to learn all the things.

[Rodney Littles, II](https://rodneylittlesii.com)

[![github-license-badge]][github-license]
[![Netlify Status](https://api.netlify.com/api/v1/badges/954e5bef-1eb2-4220-a9f2-2ea2a9473d90/deploy-status)](https://app.netlify.com/sites/rodneylittlesii/deploys)
[![github-badge]][github]

Built with [Astro](https://astro.build). Theme: [Solid State](https://html5up.net/solid-state) by HTML5 UP, vendored under `public/assets/`.

## Requirements

- Node 24 (see `.nvmrc`). Astro 7 requires Node 22.12 or newer.

## Scripts

```sh
npm ci            # install
npm run dev       # dev server at http://localhost:4321
npm run build     # static build to dist/
npm run preview   # serve dist/
npm run check     # astro check (types + templates)
```

## Writing a post

Create `src/content/posts/topic/<slug>.md`. The slug becomes the URL: `/posts/topic/<slug>/`.

```yaml
---
title: "Post title"
published: 2026-01-31
drafted: 2026-01-15      # optional
edited: 2026-02-01       # optional
tags:
  - Reactive Extensions
  - C#
draft: false             # true hides the post from the site and feed
---
```

- Posts with a future `published` date, or `draft: true`, are not built.
- Images go in `public/images/` and are referenced as `/images/<file>`.
- Tags are grouped case-insensitively. Tag URLs replace spaces with `-`, drop a leading `.`, and turn `#` into `Sharp` (`.NET` becomes `/tags/NET/`, `C#` becomes `/tags/CSharp/`).
- Static pages live in `src/content/pages/` and render at `/<id>/`. Add a nav entry in `src/site.ts`.
- Frontmatter is validated by the schema in `src/content.config.ts`.

## Layout

| Path | Purpose |
| --- | --- |
| `src/content/` | Markdown content collections (`posts`, `pages`) |
| `src/pages/` | Routes: home, archive, post, tags, static pages, 404, `feed.rss` |
| `src/layouts/Base.astro` | Solid State shell: head, header, menu, footer, scripts |
| `src/components/` | Banner, Section, PostCard, PostListItem, TagButtons, Disqus, SocialLinks |
| `src/lib/posts.ts` | Query, tag slug, date and excerpt helpers |
| `src/site.ts` | Site title, nav, social links |
| `public/_redirects` | Netlify 301s from legacy Wyam `.html` URLs |

## Deployment

GitHub Actions (`.github/workflows/publish.yml`) builds on every push to `main` and `draft/*` and on pull requests.
Pushes to `main` deploy `dist/` to Netlify with `netlify-cli` using the `NETLIFY_TOKEN` secret.
Pull requests from this repository get a preview deploy aliased `pr-<number>`.

[github-license]: https://github.com/rlittlesii/rodneylittlesii/blob/main/LICENSE
[github-license-badge]: https://img.shields.io/github/license/rlittlesii/rodneylittlesii.svg?style=flat "License"
[github]: https://github.com/rlittlesii/rodneylittlesii/actions/workflows/publish.yml
[github-badge]: https://img.shields.io/github/actions/workflow/status/rlittlesii/rodneylittlesii/publish.yml?branch=main&label=github&logo=github&style=flat "GitHub Actions Status"
