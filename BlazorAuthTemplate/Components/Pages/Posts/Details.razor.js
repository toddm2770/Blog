// this will execute when the content on the page changes, e.g. similar to
// blazor's OnAfterRender lifecycle method (which is not available in SSR)
export async function onUpdate() {
    // core prism support
    await import('/prism.js');
    // template's prism plugins
    await import("/assets/vendor/prismjs/components/prism-markup.min.js");
    await import("/assets/vendor/prismjs/components/prism-clike.min.js");
    await import("/assets/vendor/prismjs/plugins/toolbar/prism-toolbar.min.js");
    await import("/assets/vendor/prismjs/plugins/copy-to-clipboard/prism-copy-to-clipboard.min.js");
    await import("/assets/vendor/prismjs/plugins/line-numbers/prism-line-numbers.min.js");

    Prism.highlightAll();
}