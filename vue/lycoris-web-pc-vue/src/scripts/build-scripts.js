/* eslint-disable no-undef */
const fs = require('fs');
const { parse } = require('node-html-parser');
const { glob } = require('glob');
const urlRegex = require('url-regex');
const { build } = require('../config.json');

const urlPattern = /(https?:\/\/[^/]*)/i;

async function searchDomain() {
  const urls = new Set();
  const files = await glob('dist/**/*.{html,css,js}');
  for (let file of files) {
    const source = fs.readFileSync(file, 'utf-8');
    const matches = source.match(urlRegex({ strict: true }));
    if (matches) {
      matches.forEach(url => {
        const match = url.match(urlPattern);
        if (match && match[1] && !match[1].includes('localhost') && !match[1].includes('lycoris.cloud') && !match[1].includes('beian.miit.gov.cn')) {
          if (!match[1].includes('.${')) {
            urls.add(match[1]);
          }
        }
      });
    }
  }

  return [...urls].map(url => `<link rel="dns-prefetch" href="${url}"></link>`).join('\n');
}

function includeScripts(src) {
  for (let item of build.esm) {
    if (src.includes(item)) {
      return true;
    }
  }

  return false;
}

async function _main() {
  const links = await searchDomain();

  const files = await glob('dist/**/*.html');

  for (let file of files) {
    const html = fs.readFileSync(file, 'utf-8');
    const root = parse(html);
    const header = root.querySelector('head');

    if (links && links.length) {
      header.insertAdjacentHTML('afterbegin', links);
    }

    let scripts = header.querySelectorAll('script');
    if (scripts && scripts.length) {
      for (let script of scripts) {
        if (includeScripts(script.getAttribute('src'))) {
          script.setAttribute('type', 'module');
        }
      }
    }

    fs.writeFileSync(file, root.toString());
  }
}

_main();
