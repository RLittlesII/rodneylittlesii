import { getCollection, type CollectionEntry } from 'astro:content';

export type Post = CollectionEntry<'posts'>;

/** Tag slug: whitespace -> "-", leading dots stripped, "#" -> "sharp", lower-cased. Netlify normalizes URLs to lower case and decodes %23 before file lookup, so ".NET Foundation" -> "net-foundation" and "C#" -> "csharp". */
export const tagSlug = (tag: string) => tag.trim().replace(/\s+/g, "-").replace(/^\.+/, "").replace(/#/g, "sharp").toLowerCase();
/** Wyam CaseInsensitiveTags: group by lower-cased name. */
export const tagKey = (tag: string) => tag.trim().toLowerCase();
export const tagUrl = (slug: string) => `/tags/${encodeURIComponent(slug)}/`;
export const postUrl = (post: Post) => `/posts/${post.id}/`;
/** Disqus identifier == Wyam FileNameWithoutExtension == last id segment. */
export const disqusId = (post: Post) => post.id.split('/').pop()!;

export async function getPublishedPosts(): Promise<Post[]> {
  const now = Date.now();
  const posts = await getCollection(
    'posts',
    ({ data }) => !data.draft && data.published.getTime() <= now,
  );
  return posts.sort((a, b) => b.data.published.getTime() - a.data.published.getTime());
}

export interface TagGroup {
  name: string;
  slug: string;
  posts: Post[];
}

export function groupTags(posts: Post[]): TagGroup[] {
  const groups = new Map<string, TagGroup>();
  for (const post of posts) {
    for (const tag of post.data.tags) {
      const key = tagKey(tag);
      const group = groups.get(key) ?? { name: tag.trim(), slug: tagSlug(tag), posts: [] };
      group.posts.push(post);
      groups.set(key, group);
    }
  }
  return [...groups.values()].sort(
    (a, b) => b.posts.length - a.posts.length || a.name.localeCompare(b.name),
  );
}

const dateParts = new Intl.DateTimeFormat('en-GB', {
  weekday: 'long',
  day: 'numeric',
  month: 'long',
  year: 'numeric',
  timeZone: 'UTC',
});

/** "Friday 15 November 2019" — Wyam's `dddd d MMMM yyyy`, no comma, UTC. */
export function formatDate(date: Date): string {
  const p: Record<string, string> = {};
  for (const part of dateParts.formatToParts(date)) {
    if (part.type !== 'literal') p[part.type] = part.value;
  }
  return `${p.weekday} ${p.day} ${p.month} ${p.year}`;
}

/** First prose paragraph of a markdown body, lightly de-markdowned. */
export function excerpt(body: string | undefined): string {
  const paragraph =
    (body ?? '')
      .split(/\n\s*\n/)
      .map((s) => s.trim())
      .find((s) => s && !/^(#|!\[|<|```|\||>|[-*]\s|\d+\.\s)/.test(s)) ?? '';
  return paragraph
    .replace(/\[([^\]]+)\]\([^)]*\)/g, '$1')
    .replace(/[`*_]/g, '')
    .replace(/\s+/g, ' ');
}

export function groupByYear(posts: Post[]): { year: number; posts: Post[] }[] {
  const years = new Map<number, Post[]>();
  for (const post of posts) {
    const year = post.data.published.getUTCFullYear();
    years.set(year, [...(years.get(year) ?? []), post]);
  }
  return [...years.entries()]
    .sort((a, b) => b[0] - a[0])
    .map(([year, posts]) => ({ year, posts }));
}
