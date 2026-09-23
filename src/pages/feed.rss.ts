import rss from '@astrojs/rss';
import type { APIContext } from 'astro';
import MarkdownIt from 'markdown-it';
import sanitizeHtml from 'sanitize-html';
import { excerpt, getPublishedPosts, postUrl } from '../lib/posts';
import { site } from '../site';

const markdown = new MarkdownIt({ html: true });

export async function GET(context: APIContext) {
  const posts = await getPublishedPosts();
  return rss({
    title: site.title,
    description: site.description,
    site: context.site ?? site.url,
    customData: '<language>en-us</language>',
    items: posts.map((post) => ({
      title: post.data.title,
      pubDate: post.data.published,
      link: postUrl(post),
      description: excerpt(post.body),
      categories: [...post.data.tags],
      content: sanitizeHtml(markdown.render(post.body ?? ''), {
        allowedTags: [...sanitizeHtml.defaults.allowedTags, 'img'],
      }).replaceAll('src="/images/', `src="${site.url}/images/`),
    })),
  });
}
