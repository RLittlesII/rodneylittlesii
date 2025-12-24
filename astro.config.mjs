import { defineConfig } from 'astro/config';

function wyamFrontmatterFix() {
  return {
    name: 'vite-wyam-frontmatter-fix',
    enforce: 'pre',
    transform(code, id) {
      // Match .md files even when Vite/Astro adds query params (e.g., ?v=, ?import)
      if (!/\.md($|\?)/.test(id)) return null;
      if (!/\/src\//.test(id)) return null; // only transform source content
      if (code.startsWith('---')) return null; // already standard frontmatter

      // Heuristic: Wyam-style header lines followed by a standalone --- line (support LF or CRLF)
      const head = code.slice(0, 8000); // safety cap
      const looksLikeWyam = /^(Title:|Tags:|Order:|Published:|Drafted:)\s/m.test(head);
      // Match '---' on its own line using multiline mode, regardless of \r?\n
      const hasTerminator = /^---\s*$/m.test(head);
      if (looksLikeWyam && hasTerminator) {
        // Find the first closing '---' line which ends the Wyam header
        const termMatch = head.match(/^---\s*$/m);
        let out = code;
        if (termMatch) {
          const headerEndIdx = termMatch.index ?? 0; // start of the closing line
          const header = code.slice(0, headerEndIdx); // up to (but not including) closing ---
          const rest = code.slice(headerEndIdx); // includes closing --- and beyond

          // Fix unindented YAML sequence lines under Tags:
          // Convert:
          // Tags:\n- A\n- B\n
          // To:
          // Tags:\n  - A\n  - B\n
          const fixedHeader = header.replace(/(^Tags:\s*\r?\n)((?:-\s.*\r?\n)+)/m, (m, before, list) => {
            const indented = list.replace(/^-/gm, '  -');
            return before + indented;
          });

          out = `---\n${fixedHeader}${rest}`;
        } else {
          out = `---\n${code}`;
        }

        try {
          // eslint-disable-next-line no-console
          console.log(`[wyam-frontmatter-fix] patched: ${id}`);
        } catch {}
        return { code: out, map: null };
      }
      try {
        // eslint-disable-next-line no-console
        console.log(`[wyam-frontmatter-fix] skipped: ${id} (looksLikeWyam=${looksLikeWyam}, hasTerminator=${hasTerminator})`);
      } catch {}
      return null;
    },
  };
}

export default defineConfig({
  vite: {
    plugins: [wyamFrontmatterFix()],
    server: {
      fs: {
        // allow serving source files from this repo root
        allow: ['.'],
      },
    },
  },
});
