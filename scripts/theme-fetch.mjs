// Download Solid State theme assets into public/assets/** without relying on Wyam output
// Usage: node scripts/theme-fetch.mjs
// Optional env:
//   THEME_BASE: override base URL (default: Wyam theme assets on GitHub)

import https from 'node:https';
import { mkdir, writeFile, cp as copy } from 'node:fs/promises';
import { existsSync } from 'node:fs';
import { dirname, join } from 'node:path';

const BASE = process.env.THEME_BASE || 'https://raw.githubusercontent.com/Wyamio/Wyam/develop/themes/Blog/SolidState/assets';

// Minimal set needed by Base.astro
const files = [
  'css/main.css',
  'css/noscript.css',
  'js/jquery.min.js',
  'js/jquery.scrollex.min.js',
  'js/skel.min.js',
  'js/util.js',
  'js/main.js',
];

function fetchText(url) {
  return new Promise((resolve, reject) => {
    https
      .get(url, (res) => {
        if (res.statusCode && res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
          // Follow redirects
          return resolve(fetchText(res.headers.location));
        }
        if (res.statusCode !== 200) {
          reject(new Error(`Failed to fetch ${url} (status ${res.statusCode})`));
          return;
        }
        let data = '';
        res.setEncoding('utf8');
        res.on('data', (chunk) => (data += chunk));
        res.on('end', () => resolve(data));
      })
      .on('error', reject);
  });
}

async function main() {
  // Prefer local assets if present in repo (no Wyam build required, just previously generated assets)
  const localAssetsDir = join('output', 'assets');
  if (existsSync(localAssetsDir)) {
    const dest = join('public', 'assets');
    await mkdir('public', { recursive: true });
    await copy(localAssetsDir, dest, { recursive: true });
    console.log(`✔ copied ${localAssetsDir} -> ${dest}`);
    // Ensure /assets/css/main.css exists (Wyam theme ships it under assets/sass/main.css)
    const wyamMain = join(dest, 'sass', 'main.css');
    const cssDir = join(dest, 'css');
    if (existsSync(wyamMain)) {
      await mkdir(cssDir, { recursive: true });
      await copy(wyamMain, join(cssDir, 'main.css'));
      console.log(`✔ ensured ${join('public','assets','css','main.css')} (from assets/sass/main.css)`);
    }
    return;
  }

  const targets = files.map((f) => ({
    url: `${BASE}/${f}`,
    out: join('public', 'assets', f),
  }));

  let ok = 0;
  for (const t of targets) {
    try {
      const content = await fetchText(t.url);
      await mkdir(dirname(t.out), { recursive: true });
      await writeFile(t.out, content, 'utf8');
      console.log(`✔ downloaded ${t.url} -> ${t.out}`);
      ok++;
    } catch (e) {
      console.error(`✖ failed ${t.url}: ${e.message}`);
    }
  }

  if (ok === 0) {
    console.error('No files downloaded. Check your internet connection or set THEME_BASE to a valid URL.');
    process.exitCode = 1;
  } else {
    console.log(`Done. ${ok}/${targets.length} files written under public/assets.`);
  }
}

main().catch((e) => {
  console.error(e);
  process.exit(1);
});
